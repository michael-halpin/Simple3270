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
using System.Collections;
using Simple3270.Library;

namespace Simple3270.TN3270Server
{
	/// <summary>
	/// Summary description for TN3270ServerEmulationBase.
	/// </summary>
	public abstract class TN3270ServerEmulationBase
	{
		private ClientSocket mSocket;
		private MySemaphore mDataSemaphore = new MySemaphore(0,9999);
		private Queue mData = new Queue();
		//
		public TN3270ServerEmulationBase()
		{
		}
		public void Init(System.Net.Sockets.Socket sock)
		{
			mSocket = new ClientSocket(sock);
			mSocket.FXSocketType = ServerSocketType.RAW;
			mSocket.OnData += new ClientDataNotify(cs_OnData);
			mSocket.OnNotify += new ClientSocketNotify(mSocket_OnNotify);
			mSocket.Start();
		}
		
		public virtual void Run()
		{
		}
		public abstract TN3270ServerEmulationBase CreateInstance(System.Net.Sockets.Socket sock);
		private void cs_OnData(byte[] data, int Length)
		{
			if (data==null)
			{
				Console.WriteLine("cs_OnData received disconnect, close down this instance");
				Disconnect();
			}
			else
			{
				//Console.WriteLine("\n\n--on data");
				AddData(data, Length);
			}
		}
		public void Send(string dataStream)
		{
			//Console.WriteLine("send "+dataStream);
			byte[] bytedata = ByteFromData(dataStream);
			mSocket.Send(bytedata);
		}
		public void Send(TNServerScreen s)
		{
			//Console.WriteLine("send screen");
			byte[] data = s.AsTN3270Buffer(true,true, _TN3270E);
			//Console.WriteLine("data.length="+data.Length);
			mSocket.Send(data);
		}

		private byte[] ByteFromData(string text)
		{
			string[] data = text.Split(new char[] {' '});
			byte[] bytedata = new byte[data.Length];
			for (int i=0; i<data.Length; i++)
			{
				bytedata[i] = (byte)System.Convert.ToInt32(data[i], 16);
			}
			return bytedata;
		}
		public string WaitForKey(TNServerScreen currentScreen)
		{
			//Console.WriteLine("--wait for key");
			byte[] data = null;
			bool screen = false;
			do
			{
				do
				{
					while (!mDataSemaphore.Acquire(1000))
					{
						if (mSocket==null)
							throw new TN3270ServerException("Connection dropped");
					}
					data = (byte[])mData.Dequeue();
				}
				while (data==null);
				if (data[0]==255 && data[1]==253)
				{
					// assume do/will string
					for (int i=0; i<data.Length; i++)
					{
						if (data[i]==253) 
						{
							data[i] = 251; // swap DO to WILL
							Console.WriteLine("DO "+data[i+1]);
						}
						else if (data[i]==251)
						{
							data[i] = 253; // swap WILL to DO
							Console.WriteLine("WILL "+data[i+1]);
						}
					}
					mSocket.Send(data);
					screen = false;
				}
				else
					screen = true;
			}
			while (!screen);
			return currentScreen.HandleTN3270Data(data, data.Length);
		}

		public int Wait(params string[] dataStream)
		{
			//Console.WriteLine("--wait for "+dataStream);
			while (!mDataSemaphore.Acquire(1000))
			{
				if (mSocket==null)
					throw new TN3270ServerException("Connection dropped");
			}
			byte[] data = (byte[])mData.Dequeue();

			for (int count=0; count<dataStream.Length; count++)
			{
				byte[] bytedata = ByteFromData(dataStream[count]);
				
				if (bytedata.Length==data.Length)
				{
					bool ok = true;
					for (int i=0; i<bytedata.Length; i++)
					{
						if (bytedata[i] != data[i])
						{
							ok = false;
						}
					}
					if (ok)
						return count;
				}
			}
			Console.WriteLine("\n\ndata match error");
			for (int count=0; count<dataStream.Length; count++)
			{
				Console.WriteLine("--expected "+dataStream);
			}
			Console.Write(    "--received ");
			for (int i=0; i<data.Length; i++)
			{
				Console.Write("{0:x2} ", data[i]);
			}
			Console.WriteLine();
			throw new TN3270ServerException("Error reading incoming data stream. Expected data missing. Check console log for details");
		}
		public void AddData(byte[] data, int length)
		{
			byte[] copy = new byte[length];
			for (int i=0; i<length;i++)
			{
				copy[i] = data[i];
			}
			mData.Enqueue(copy);
			mDataSemaphore.Release(1);
		}
		public void Disconnect()
		{
			if (mSocket != null)
			{
				mSocket.Disconnect();
				mSocket = null;
			}
		}

		private void mSocket_OnNotify(string eventName, Message message)
		{
			if (eventName=="Disconnect")
			{
				Disconnect();
			}
		}
		private bool _TN3270E = false;
		public bool TN3270E
		{
			get
			{
				return _TN3270E;
			}
			set 
			{
				_TN3270E = value;
			}
		}
	}
}
