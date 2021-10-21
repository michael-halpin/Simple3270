#region License
/* 
 *
 * Simple3270 - A simple implementation of the TN3270/TN3270E protocol for Python and C#
 *
 * Copyright (c) 2004-2020 Michael Warriner
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
using System.Text;
using System.Collections;

namespace Simple3270.TN3270
{
	internal class EventNotification
	{
		public string error;
		object[] data;
		public EventNotification(string error, object[] data)
		{
			this.error = error;
			this.data = data;
		}
		public override string ToString()
		{
			return TraceFormatter.Format(error,data);
		}

	}
	internal delegate void Error(string error);
	/// <summary>
	/// Summary description for Events.
	/// </summary>
	internal class Events
	{
		Telnet telnet;
		ArrayList events;
		
		internal Events(Telnet tn)
		{
			telnet = tn;
			events = new ArrayList();
		}
		public void Clear()
		{
			events = new ArrayList();
		}
		public string GetErrorAsText()
		{
			if (events.Count==0)
				return null;
			StringBuilder builder = new StringBuilder();
			for (int i=0; i<events.Count; i++)
			{
				builder.Append(events[i].ToString());
			}
			
			return builder.ToString();

		}
		public bool IsError()
		{
			if (events.Count>0)
				return true;
			else
				return false;
		}
		
		public void ShowError(string error, params object[] args)
		{
			events.Add(new EventNotification(error, args));
			Console.WriteLine("ERROR"+TraceFormatter.Format(error,args));
			//telnet.FireEvent(error, args);
		}
		public void Warning(string warning)
		{
			Console.WriteLine("warning=="+warning);
		}
		public void RunScript(string where)
		{
			//Console.WriteLine("Run Script "+where);
			lock (telnet)
			{
				if ((telnet.Keyboard.keyboardLock | KeyboardConstants.DeferredUnlock) == KeyboardConstants.DeferredUnlock)
				{
					telnet.Keyboard.KeyboardLockClear(KeyboardConstants.DeferredUnlock, "defer_unlock");
					if (telnet.IsConnected)
						telnet.Controller.ProcessPendingInput();
				}
			}
																									 
																									
			
			if (telnet.TelnetApi != null)
				telnet.TelnetApi.RunScript(where);
			
		}
	}
}
