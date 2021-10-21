﻿#region License
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
using System.Net;
using System.Net.Sockets;

namespace Simple3270.Library
{
	internal delegate void OnConnectionDelegate(ClientSocket sock);
	internal delegate void OnConnectionDelegateRAW(Socket sock);

	internal enum ServerSocketType
	{
		RAW,
		ClientServer
	}
	/// <summary>
	/// Summary description for ServerSocket.
	/// </summary>
	internal class ServerSocket
	{
		public event OnConnectionDelegate OnConnect;
		public event OnConnectionDelegateRAW OnConnectRAW;
		Socket mSocket;
		ServerSocketType mSocketType;
		private AsyncCallback callbackProc ;

		public ServerSocket()
		{
			mSocketType = ServerSocketType.ClientServer;
		}
		public ServerSocket(ServerSocketType socketType)
		{
			mSocketType = socketType;
		}
		public void Close()
		{
			try
			{
				Console.WriteLine("ServerSocket.CLOSE");
				mSocket.Close();
			}
			catch (Exception)
			{
			}
			mSocket = null;
		}
		public void Listen(int port)
		{
			//IPHostEntry lipa = Dns.Resolve("host.contoso.com");
			IPEndPoint lep = new IPEndPoint(IPAddress.Any, port);

			mSocket				= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			// Create New EndPoint
			// This is a non blocking IO
			mSocket.Blocking		= false ;	

			mSocket.Bind(lep);
			//
			mSocket.Listen(1000);
			//
			// Assign Callback function to read from Asyncronous Socket
			callbackProc	= new AsyncCallback(ConnectCallback);
			//
			mSocket.BeginAccept(callbackProc, null);
		}
		private void ConnectCallback( IAsyncResult ar )
		{
			Socket newSocket = null;
			try
			{

				try
				{
					newSocket = mSocket.EndAccept(ar);
				}
				catch (System.ObjectDisposedException)
				{
					
					//Console.WriteLine("Server socket error - ConnectCallback failed "+ee.Message);
					mSocket = null;
					return;
				}

				try
				{
					Audit.WriteLine("Connection received - call OnConnect");
					//
					if (this.OnConnectRAW != null)
						this.OnConnectRAW(newSocket);
					//
					if (this.OnConnect != null)
					{
						ClientSocket socket = new ClientSocket(newSocket);
						socket.FXSocketType = this.mSocketType;
						this.OnConnect(socket);
					}

					// restart accept
					mSocket.BeginAccept(callbackProc, null);
				}
				catch (System.ObjectDisposedException)
				{
					newSocket.Close();
					newSocket = null;
				}
				catch (Exception e)
				{
					Console.WriteLine("Exception occured in AcceptCallback\n"+e);
					newSocket.Close();
					newSocket = null;
				}
			}
			finally
			{
				// wait for the next incoming connection
				if (mSocket != null)
					mSocket.BeginAccept(callbackProc, null);
			}
		}
	}
}
