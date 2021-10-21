#region License
/* 
 *
 * SimplRPA - A simple RPA library for Python and C#
 *
 * Copyright (c) 2009-2021 Michael S. Halpin
 * Modifications (c) as per Git change history
 *
 * This Source Code Form is subject to the terms of the Mozilla
 * Public License, v. 2.0. If a copy of the MPL was not distributed
 * with this file, You can obtain one at
 * https://mozilla.org/MPL/2.0/.
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
 * LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using Newtonsoft.Json;

namespace Simple3270Console
{
    /// <summary>
    /// Used to facilitate cross platform communications on Windows machines.
    /// </summary>
    public class Win32Comm
    {
        private string _ServerToClient;
        private string _ClientToServer;
        private byte[] _Key;
        private byte[] _IV;
        public Win32Comm(string Key, string IV, string serverToClient, string clientToServer)
        {
            _Key = Encoding.UTF8.GetBytes(Key);
            _IV = Encoding.UTF8.GetBytes(IV);
            _ServerToClient = serverToClient;
            _ClientToServer = clientToServer;
        }
        /// <summary>
        /// Used by the client to query the server.
        /// </summary>
        /// <param name="json">The request to make.</param>
        /// <param name="serverToClient">The channel name to listen on.</param>
        /// <returns>dynamic</returns>
        public dynamic Query(string json)
        {
            string pkg = Steganography.Package(json, _Key, _IV);
            Set(pkg, _ClientToServer);
            string packet = Get(_ServerToClient);
            string rsp = Steganography.Unpackage(packet, _Key, _IV);
            dynamic resp = JsonConvert.DeserializeObject(rsp);
            if (resp["response"] == "EXCEPTION")
                throw new Exception(resp["content"].ToString());
            return resp;
        }
        /// <summary>
        /// Used by the client to query the server for binary data.
        /// </summary>
        /// <param name="json">The request to make.</param>
        /// <param name="serverToClient">The channel name to listen on.</param>
        /// <returns>byte[]</returns>
        public byte[] QueryToBytes(string json)
        {
            string pkg = Steganography.Package(json, _Key, _IV);
            Set(pkg, _ClientToServer);
            string packet = Get(_ServerToClient);
            dynamic resp = JsonConvert.DeserializeObject(packet);
            return new byte[0]; //Common.ProcessBytePackage(resp, _Key, _IV);
        }
        /// <summary>
        /// Used to get data from the server.
        /// </summary>
        /// <param name="serverToClient">The name of the channel to look on.</param>
        /// <returns>string</returns>
        public static string Get(string serverToClient)
        {
            var client = new NamedPipeClientStream(serverToClient);
            client.Connect();
            StreamReader reader = new StreamReader(client);

            return reader.ReadToEnd();
        }
        /// <summary>
        /// Used to send data to the server.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="clientToServer">The name of the channel to write to.</param>
        public static void Set(string message, string clientToServer)
        {
            var server = new NamedPipeServerStream(clientToServer);
            server.WaitForConnection();
            StreamWriter writer = new StreamWriter(server);

            if (message.Length < 1000)
            {
                writer.WriteLine(message);
                writer.Flush();
            }
            else
            {
                List<string> messages = new List<string>();
                while (message.Length > 1000)
                {
                    messages.Add(message.Substring(0, 1000));
                    message = message.Substring(1000);
                }
                messages.Add(message);

                if (messages[message.Length - 1].Length == 1000)
                    messages.Add("#END_OF_FILE");

                for (int i = 0; i < messages.Count; i++)
                {
                    writer.WriteLine(messages[i]);
                    writer.Flush();
                }
                writer.DisposeAsync();
            }
            server.DisposeAsync();
        }
    }
}