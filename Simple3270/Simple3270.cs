#region License
/* 
 *
 * Simple3270 - A simple implementation of the TN3270/TN3270E protocol for C#
 *
 * Copyright (c) 2009-2021 Michael S. Halpin
 * Modifications (c) as per Git change history
 *
 * This Source Code Form is subject to the terms of the Mozilla
 * internal License, v. 2.0. If a copy of the MPL was not distributed
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
using System.Collections.Generic;
using System.Threading;
using System;
using System.Drawing;
using Newtonsoft.Json;
using Simple3270.Models;

namespace Simple3270
{
	/// <summary>
	/// Simple3270Api class for connecting to 3270 mainframes.
	/// </summary>
	public class Simple3270Api : IDisposable
	{
		private readonly int _maxRow;
		private readonly int _maxCol;
		private readonly int _colorDepth;
		private readonly bool _verbose = true;
		private string _screen;
		private readonly int _actionTimeOut;
		private readonly TNEmulator _emulator;
		private ConsoleColor[,] _foreColors;
		/// <summary>
		/// Constructs a new instance of the Simple3270Api class.
		/// </summary>
		/// <param name="config">The configuration object to use.</param>
		public Simple3270Api(Simple3270Config config)
		{
			_emulator = new TNEmulator();
			// emulator.Audit = this;
			_colorDepth = config.ColorDepth;
			_actionTimeOut = config.ActionTimeout;
			_emulator.Debug = config.Debug;
			_emulator.Config.TermType = config.TerminalType;
			_emulator.Config.FastScreenMode = config.FastScreenMode;
			_emulator.Config.UseSSL = config.UseSsl;
			_emulator.Connect(config.Server, config.Port, config.Lu);
			ClearForeColors();
			//Write();
			ReadAllText();
			Point pt = _emulator.GetDimensions();
			_maxRow = pt.X;
			_maxCol = pt.Y;
			_foreColors = new ConsoleColor[_maxCol, _maxRow];
		}
		/// <summary>
		/// Releases resources and disposes this object.
		/// </summary>
		public void Dispose()
        {
			_emulator.Dispose();
			_foreColors = null;
        }

		#region PUBLIC CAST TO CLASS METHODS
		/// <summary>
		/// Reads the fore colors for each field in the specified list.
		/// </summary>
		/// <typeparam name="T">The class to cast the data into.</typeparam>
		/// <param name="fields">The list of fields to get the colors of.</param>
		/// <returns>T</returns>
		public T ReadScreenColors<T>(List<SimpleInput> fields)
		{
			string json = JsonConvert.SerializeObject(fields);
			string output = ReadScreenColors(json);
			T obj = JsonConvert.DeserializeObject<T>(output);
			return obj;
		}
		/// <summary>
		/// Reads the values for all fields in the specified list.
		/// </summary>
		/// <param name="fields">The list of fields to gather.</param>
		/// <returns>dynamic</returns>
		public List<SimpleOutput> ReadScreen(IList<SimpleInput> fields)
		{
			List<SimpleOutput> output = new List<SimpleOutput>();
			for (int i = 0; i < fields.Count; i++)
			{
				int x = fields[i].X;
				int y = fields[i].Y;
				//ConsoleColor c = GetForeColor(x, y);
				string text = ReadField(fields[i]);
				output.Add(new SimpleOutput(fields[i].Name, x, y, text));
			}
			return output;
		}
		/// <summary>
		/// Writes all field values in the specified list to the mainframe screen.
		/// </summary>
		/// <param name="fields">The list of fields to gather.</param>
		/// <returns>dynamic</returns>
		public void WriteScreen(List<SimpleOutput> fields)
		{
			for (int i = 0; i < fields.Count; i++)
			{
				int x = fields[i].X;
				int y = fields[i].Y;
				_emulator.SetCursor(x - 1, y - 1);
				_emulator.SetText(fields[i].Value);
			}
		}
		/// <summary>
		/// Wait for the specified text to appear at the specified location.
		/// </summary>
		/// <param name="field">The field to wait for.</param>
		/// <returns>dynamic</returns>
		public bool WaitFor(WaitForRequest field)
		{
			DateTime end = DateTime.UtcNow.AddMilliseconds(field.Timeout);
			while (true)
			{
				if (DateTime.UtcNow > end)
				{
					return false;
				}
				string text = ReadField(new SimpleInput(field));
				if (field.Value == text)
				{
					return true;
				}
				Thread.Sleep(100);
			}
		}
		/// <summary>
		/// Reads all fore colors for all fields listed in the specified json map.
		/// </summary>
		/// <param name="json">The json field map to use.</param>
		/// <returns>json</returns>
		private string ReadScreenColors(string json)
		{
			List<SimpleInput> fields = JsonConvert.DeserializeObject<List<SimpleInput>>(json);
			string output = ReadScreenColors(fields);
			return output;
		}
		/// <summary>
		/// Presses the key specified in the json map.
		/// </summary>
		/// <param name="key">The key to press.</param>
		/// <returns>bool</returns>
		public bool PressKey(string key)
		{
            #region Perform key press.
            string text = _screen;
			TnKey press = (TnKey)Enum.Parse(typeof(TnKey), key);
			_emulator.SendKey(true, press, 60);
			int tries = 0;
            #endregion

            #region Handles if mainframe is clocking or otherwise returns a blank screen.
            while (string.IsNullOrEmpty(_screen.Trim()) || text == _screen)
			{
				ReadAllText();
				Thread.Sleep(25);
				tries++;
				if (tries >= 400)
					return false;
			}
			#endregion

			return true;
        }
		#endregion

		#region PRIVATE METHODS
		private string ReadScreenColors(List<SimpleInput> fields)
		{
			string output = "{";
			for (int i = 0; i < fields.Count; i++)
			{
				int x = fields[i].X;
				int y = fields[i].Y;
				ConsoleColor c = GetForeColor(x, y);
				string js = "\"" + fields[i].Name + "\":\"" + c.ToString() + "\"";
				output += js + ",";
			}
			output = output.Substring(0, output.Length - 1) + "}";
			return output;
		}
		private string ReadField(SimpleInput field)
		{
			string text = _emulator.GetText(field.X - 1, field.Y - 1, field.L);
			return text.Trim();
		}
		private void Write()
		{
			if (_verbose)
			{
				int retries = 0;
			retry:
				int r = Console.CursorTop;
				int c = Console.CursorLeft;
				try
				{
					// Get screen in the form of an array of rows.
					string[] rows = ReadScreenByRows();
					Console.Clear(); // Clear console before rewriting the screen.
					#region Go field by field and write each field on the screen with the assigned color.
					for (int i = 0; i < _emulator.CurrentScreenXML.Fields.Length; i++)
					{
						if (_emulator.CurrentScreenXML.Fields[i].Text != null)
						{
							int x = _emulator.CurrentScreenXML.Fields[i].Location.left;
							int y = _emulator.CurrentScreenXML.Fields[i].Location.top;
							int l = _emulator.CurrentScreenXML.Fields[i].Location.length;
							Console.SetCursorPosition(x, y);
							ConsoleColor clr = GetFieldColor(i);
							Console.ForegroundColor = clr;
							SetForeColor(x, y, l, clr);
							Console.Write(_emulator.CurrentScreenXML.Fields[i].Text);
							int d = _emulator.CurrentScreenXML.Fields[i].Location.top;
							rows[d] = rows[d].Remove(x, l);
							rows[d] = rows[d].Insert(x, string.Empty.PadRight(l, ' '));
						}
					}
					#endregion

					#region Write any text to the screen that is present and not mapped.
					for (int y = 0; y < rows.Length; y++)
					{
						// Skip blank lines.
						if (!string.IsNullOrEmpty(rows[y].Trim()))
						{
							string text = rows[y].Trim();
							int l = rows[y].IndexOf(text, StringComparison.Ordinal);
							Console.SetCursorPosition(l, y);
							Console.ForegroundColor = ConsoleColor.DarkGreen;
							Console.Write(text);
						}
					}
					#endregion
				}
				catch (Exception)
				{
					#region If we are stuck on a blank screen try to fix it by clearing it.
					if (string.IsNullOrEmpty(_emulator.GetText(0, 0, (_maxCol * _maxRow)).Trim()))
					{
						Console.Clear();
						Thread.Sleep(_actionTimeOut);
						goto retry;
					}
					#endregion

					#region Otherwise try to refresh the page up to 5 times.  If didnt work after 5x then skip redraw.
					retries++;
					if (retries >= 5)
						return;
					_emulator.Refresh();
					goto retry;
					#endregion
				}
				Console.SetCursorPosition(c, r);
			}
		}
		private ConsoleColor GetFieldColor(int i)
		{
			if (_colorDepth == 2)
			{
				if (_emulator.CurrentScreenXML.Fields[i].Attributes.FieldType == "High" &&
					_emulator.CurrentScreenXML.Fields[i].Attributes.Protected)
					return ConsoleColor.White;
				else if (_emulator.CurrentScreenXML.Fields[i].Attributes.FieldType == "High")
					return ConsoleColor.Red;
				else if (_emulator.CurrentScreenXML.Fields[i].Attributes.Protected)
					return ConsoleColor.Blue;
			}
			else if (_colorDepth == 4)
			{
				if (_emulator.CurrentScreenXML.Fields[i].Attributes.Foreground == null)
				{
					return ConsoleColor.Green;
				}
				switch (_emulator.CurrentScreenXML.Fields[i].Attributes.Foreground.ToLower())
				{
					case "blue":
						return ConsoleColor.Blue;
					case "turquoise":
						return ConsoleColor.Cyan;
					case "deepBlue":
						return ConsoleColor.DarkBlue;
					case "paleTurquoise":
						return ConsoleColor.DarkCyan;
					case "paleGreen":
						return ConsoleColor.DarkGreen;
					case "purple":
						return ConsoleColor.DarkMagenta;
					case "orange":
						return ConsoleColor.DarkYellow;
					case "neutralWhite":
						return ConsoleColor.Gray;
					case "green":
						return ConsoleColor.Green;
					case "pink":
						return ConsoleColor.Magenta;
					case "red":
						return ConsoleColor.Red;
					case "white":
						return ConsoleColor.White;
					case "yellow":
						return ConsoleColor.Yellow;
					default: // grey and black
						return ConsoleColor.DarkGray;
				}
			}
			return ConsoleColor.Green;
		}
		private void ClearForeColors()
		{
			for (int x = 0; x < _maxCol; x++)
			{
				for (int y = 0; y < _maxRow; y++)
				{
					_foreColors[x, y] = ConsoleColor.Green;
				}
			}
		}
		private ConsoleColor GetForeColor(int x, int y)
		{
			ConsoleColor clr = _foreColors[x - 1, y - 1];
			return clr;
		}
		private void SetForeColor(int x, int y, int l, ConsoleColor c)
		{
			int end = x + l;
			for (int z = x; z < end; z++)
				_foreColors[z, y] = c;
		}
		private void ReadAllText()
		{
			int lng = _maxRow * _maxCol;
			_screen = _emulator.GetText(0, 0, lng);

			//if (_Screen.ContainsAny(_Abends))
			//	throw new IbmException("The mainframe session has abended.");
		}
		private string[] ReadScreenByRows()
		{
			string screen = _emulator.CurrentScreenXML.Dump();
			string[] rows = screen.Replace("\r\n", "\n").Split('\n');

			string paddedScreen = string.Empty;
			for (int i = 0; i < 24; i++)
			{
				paddedScreen += rows[i].PadRight(80, ' ') + "\n";
			}

			paddedScreen = paddedScreen.Substring(0, paddedScreen.Length - 2);
			rows = paddedScreen.Split('\n');
			return rows;
		}
		#endregion
	}
}