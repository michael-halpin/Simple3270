#region License
/* 
 *
 * Simple3270Web - A simple implementation of the TN3270/TN3270E protocol for Python and C#
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
using Simple3270.Library;

namespace Simple3270.TN3270Server
{
	/// <summary>
	/// Summary description for TN3270Server.
	/// </summary>
	public class TN3270Server
	{
		//TN3270ServerScript _Script;
		//int _port = 23;
		//bool bQuit = false;
		public TN3270Server()
		{
		}
		TN3270ServerEmulationBase system;
		ServerSocket server;
		public void Start(TN3270ServerEmulationBase system, int port)
		{
			this.system = system;

			server = new ServerSocket(ServerSocketType.RAW);
			//
			server.OnConnectRAW += new OnConnectionDelegateRAW(server_OnConnectRAW);
			server.Listen(port);
		}
		public void Stop()
		{
			server.Close();
		}

		private void server_OnConnectRAW(System.Net.Sockets.Socket sock)
		{
			Console.WriteLine("OnConnectRAW");
			//
			TN3270ServerEmulationBase instance = system.CreateInstance(sock);
			try
			{
				try
				{
					instance.Run();
				}
				catch (TN3270ServerException tse)
				{
					Console.WriteLine("tse = "+tse);
					throw;
				}
			}
			finally
			{
				instance.Disconnect();
			}
		}
	}
}
