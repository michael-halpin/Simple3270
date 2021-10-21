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
using System.Web;
using System.Text;
using System.Security; 
using System.Security.Permissions;
using System.IO;

namespace Simple3270.Library
{
	public interface IAuditable
	{
		void DumpTo(StringBuilder message, bool admin);
	}
	public enum AuditType
	{
		Exception
	}
	/// <summary>
	/// Summary description for Audit.
	/// </summary>
	internal class Audit
	{
		static string _auditFile;
		static bool   _auditOn = false;
		private static DateTime LoadTime = DateTime.Now;
		static Audit()
		{
			_auditOn = false;
			_auditFile = null;
		}
		static public bool AuditOn 
		{
			get { return _auditOn; }
			set { _auditOn = value; }
		}
		static public string AuditFile
		{
			get { return _auditFile; }
			set { _auditFile = value; }
		}
		public static void WriteLine(string text)
		{
			if (_auditOn)
			{
				if (Audit._auditFile != null)
				{
					lock (Audit._auditFile)
					{
						try
						{
							Console.WriteLine(text);
							//
							// Demand file permission so that we work within the Internet Explorer sandbox
							//
							FileIOPermission permission = new FileIOPermission( PermissionState.Unrestricted );
							permission.AddPathList(FileIOPermissionAccess.Append, AuditFile);
							permission.Demand(); 
							//
							StreamWriter sw = File.AppendText(_auditFile);
							try
							{
								string date = DateTime.Now.ToShortDateString()+"-"+DateTime.Now.ToShortTimeString()+"::";
								sw.WriteLine(date+text);
							}
							finally
							{
								sw.Close();
							}
							permission.Deny();
						}
						catch (Exception ee)
						{
							Console.WriteLine("EXCEPTION ON AUDIT "+ee);

						}
					}
				}
				else
				{
					Console.WriteLine(text);
				}
			}
		}

        private static void WriteAuditInternal(AuditType Type, string Text)
		{
			WriteLine(Text);
		}
	}
}
