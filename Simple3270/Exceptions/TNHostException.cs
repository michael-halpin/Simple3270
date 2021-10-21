﻿#region License
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

namespace Simple3270
{
	/// <summary>
	/// An object to return an exception from the 3270 host.
	/// </summary>
	public class TNHostException : Exception
	{
		private string mMessage = null;
		private string mAuditLog = null;
		private string mReason   = null;
		/// <summary>
		/// Constructor - used internally.
		/// </summary>
		/// <param name="message">The message text</param>
		/// <param name="auditlog">The audit log up to this exception</param>
		public TNHostException(string message, string reason, string auditlog)
		{
			mReason  = reason;
			mMessage = message;
			mAuditLog = auditlog;
		}
		/// <summary>
		/// Returns the audit log from the start to this exception. Useful for tracing an exception
		/// </summary>
		/// <value>The formatted audit log</value>
		public string AuditLog
		{
			get { return mAuditLog;  }
			set { mAuditLog = value; }
		}
		/// <summary>
		/// Returns a textual version of the error
		/// </summary>
		/// <returns>The error text.</returns>
		public override string ToString()
		{
			return "HostException '"+mMessage+"' "+Reason;
		}
		public override string Message
		{
			get
			{
				return mMessage;
			}
		}


		public string Reason
		{
			get { return mReason; }
			set { mReason = value; }
		}
	}

}
