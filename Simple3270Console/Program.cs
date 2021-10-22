using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Simple3270;
using Simple3270.Models;
using Newtonsoft.Json;

namespace Simple3270Console
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Load the json config file.
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string json = File.ReadAllText(dir + "\\Simple3270.json");
            dynamic config = JsonConvert.DeserializeObject(json);
            #endregion

            #region Set config settings from file.

            string session = args[0];
            string key = args[1];
            string iv = args[2];
            int i = int.Parse(args[3]);
            string input = string.Empty;
            string output = string.Empty;

            string server = config["tn3270"][i]["server"];
            string lu = config["tn3270"][i]["lu"];
            int port = config["tn3270"][i]["port"];
            bool useSsl = config["tn3270"][i]["use_ssl"];

            Simple3270Config cfg = new Simple3270Config(server, port, useSsl, lu);
            cfg.TerminalType = config["tn3270"][i]["terminal_type"];
            cfg.FastScreenMode = config["tn3270"][i]["fast_screen_mode"];
            cfg.Debug = config["tn3270"][i]["debug"];
            cfg.ColorDepth = config["tn3270"][i]["color_depth"];
            cfg.ActionTimeout = config["tn3270"][i]["action_timeout"];
            cfg.DrawScreen = config["tn3270"][i]["draw_screen"];
            
            //int verbose_level = config["verbose_level"];
            #endregion
            
            byte[] Key = Encoding.UTF8.GetBytes(key);
            byte[] IV = Encoding.UTF8.GetBytes(iv);

            #region Establish connection to mainframe.
            Simple3270Api emu = null;
            try
            {
                emu = new Simple3270Api(cfg);
            }
            catch (Exception)
            {
                string msg = "Unable to connect to mainframe at " + server + ":" + port + "@ssl=" + useSsl.ToString();
                string jsn = ("{'response':'EXCEPTION','message':'" + msg + "'}").ToJson();
                Win32Comm.Set(jsn, output);
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
                return;
            }
            //string jsn0 = ("{'response':'SUCCESS','message':'Connection established!'}").ToJson();
            //Win32Comm.Set(jsn0, "Simple3270_" + output);
            #endregion

            #region Run as a service for a Python client.
            RunWin32Pipes(Key, IV, session, emu);
            #endregion
            #region Otherwise run routines as a test in C#
            /*else
            {
                List<SimpleInput> fields = new List<SimpleInput>();
                SimpleInput so = new SimpleInput("Screen", 1, 1, 2000);
                fields.Add(so);
                
                //string inputJs = ("[{'name':'Enterprise','x':1,'y':1,'l':20},{'name':'Url','x':46,'y':2,'l':28},{'name':'Ip','x':63,'y':1,'l':14},{'name':'MainFrame','x':14,'y':5,'l':20},{'name':'NextGen','x':28,'y':15,'l':19}]").ToJson();
                string inputJs = ("[{'name':'Screen','x':1,'y':1,'l':1920}]").ToJson();
                string outputJs = ("[{'name':'Login','x':24,'y':1,'value':'TSO'}]").ToJson();
                string keyJs = ("{'button':'Enter'}").ToJson();
                string waitJs = ("{'name':'WaitFor','x':12,'y':1,'value':'ENTER USERID -'}").ToJson();

                List<SimpleOutput> resp1 = emu.ReadScreen(fields);
                //string resp2 = emu.ReadScreenColors(inputJs);
                //string resp3 = emu.WriteScreenText(outputJs);
                //string resp7 = emu.ReadScreenText(inputJs);
                //string resp4 = emu.PressKey(keyJs);
                //string resp6 = emu.ReadScreenText(inputJs);
                //string resp5 = emu.WaitForText(waitJs);
            }*/
            #endregion
        }
        // TODO: We can test to see if we can do the communication via the local file system
        // for Linux and Mac OS X clients.
        /// <summary>
        /// Runs Simple3270 as a service for a Python client.
        /// </summary>
        /// <param name="key">The encryption key to use.</param>
        /// <param name="iv">The initialization vector to use.</param>
        /// <param name="input">The input channel to listen on.</param>
        /// <param name="output">The output channel to write to.</param>
        /// <param name="emu">The emulator object we use for mainfram operation.</param>
        private static void RunWin32Pipes(byte[] key, byte[] iv, string session, Simple3270Api emu)
        {
            while (true)
            {
                try
                {
                    #region Listen for next request from the client.
                    string text = Win32Comm.Get("Simple3270_C2S_" + session).Trim();
                    dynamic obj = JsonConvert.DeserializeObject(text);
                    string pkg = obj["package"];
                    string mth = obj["method"];
                    string req = Steganography.Decrypt(pkg, key, iv);
                    string method = Steganography.Decrypt(mth, key, iv);
                    string reply = string.Empty;
                    #endregion

                    #region Run the corresponding method.
                    try
                    {
                        switch (method)
                        {
                            case "read_screen":
                                List<SimpleInput> sil = JsonConvert.DeserializeObject<List<SimpleInput>>(req);
                                List<SimpleOutput> resp = emu.ReadScreen(sil);
                                reply = JsonConvert.SerializeObject(resp);
                                break;
                            case "write_screen":
                                List<SimpleOutput> sol = JsonConvert.DeserializeObject<List<SimpleOutput>>(req);
                                emu.WriteScreen(sol);
                                reply = null;
                                break;
                            case "press_key":
                                dynamic jsn = JsonConvert.DeserializeObject(req);
                                string btn = jsn["key"];
                                emu.PressKey(btn);
                                reply = null;
                                break;
                            case "wait_for_text":
                                WaitForRequest wfr = JsonConvert.DeserializeObject<WaitForRequest>(req);
                                if (emu.WaitFor(wfr))
                                {
                                    reply = null;
                                }
                                else
                                {
                                    reply = "{'response':'EXCEPTION','content':'Wait for text timed out.'}".ToJson();
                                }
                                break;
                            case "disconnect_session":
                                if (req == "{\"content\":\"disconnect_session\"}")
                                {
                                    emu.Dispose();
                                    return;
                                }
                                break;
                        }
                    }
                    catch (Exception e) 
                    { 
                        reply = ("'response':'EXCEPTION','message':'" + e.Message + "'}").ToJson(); 
                    }
                   #endregion

                    #region Send the response back to the client.
                    if (reply == null)
                    {
                        string resp = ("{'response':'SUCCESS','content':null}").ToJson();
                        Win32Comm.Set(resp, "Simple3270_S2C_" + session);
                    }
                    else if (reply.Contains("\"response\":\"EXCEPTION\""))
                    {
                        Win32Comm.Set(reply, "Simple3270_S2C_" + session);
                    }
                    else if (reply.Contains("\"length\""))
                    {   // Handles sending binary files.
                        Win32Comm.Set(reply, "Simple3270_S2C_" + session);
                    }
                    else
                    {   // Handles sending text data.
                        string resp = ("{'response':'SUCCESS','content':'" + Steganography.Encrypt(reply, key, iv) + "'}").ToJson();
                        Win32Comm.Set(resp, "Simple3270_S2C_" + session);
                    }
                }
                catch (Exception e)
                {   // Handles sending response if exception is thrown.
                    Win32Comm.Set("{'response':'EXCEPTION','content':'" + e.Message + "'}", "Simple3270_S2C_" + session);
                }
                #endregion
            }
        }
    }
}