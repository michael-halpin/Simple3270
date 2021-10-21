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


namespace Simple3270
{
	/// <summary>
	/// An error occured identifying a screen. Usually, this means that the screen didn't match
	/// any of the match rules you've defined.
	/// </summary>
	/// <remarks></remarks>
	public class TNIdentificationException : Exception
	{
		private string mPage;
		private string mDump;
		/// <summary>
		/// Identification exception
		/// </summary>
		/// <param name="page">The page we're coming from (not the page we're on!)</param>
		/// <param name="screen">The IXMLScreen object for the screen that we couldn't recognize</param>
		public TNIdentificationException(string page, IXMLScreen screen)
		{
			mPage = page;
			if (screen==null)
				mDump = null;
			else
				mDump = screen.Dump();
		}
		/// <summary>
		/// Provides a textual representation of the exception.
		/// </summary>
		/// <returns>Returns the textual representation of the exception.</returns>
		public override string ToString()
		{
			return "TNIdentificationException current screen='"+mPage+"'. Dump is \n\n"+mDump+"\n\n";
		}
	}
}