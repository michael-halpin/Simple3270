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

namespace Simple3270.TN3270
{
	/// <summary>
	/// Summary description for ExtendedAttribute.
	/// </summary>
	internal class ExtendedAttribute
	{
public const byte  GR_BLINK	=0x01;
public const byte  GR_REVERSE	=0x02;
public const byte  GR_UNDERLINE	=0x04;
public const byte  GR_INTENSIFY	=0x08;

public const byte  CS_MASK		=0x03;	/* mask for specific character sets */
public const byte CS_GE		=0x04;	/* cs flag for Graphic Escape */

		internal ExtendedAttribute()
		{
			cs = 0;
			fg = 0;
			gr = 0;
			bg = 0;
		}
        internal void Clear()
        {
            cs = 0;
            fg = 0;
            gr = 0;
            bg = 0;
        }

		public byte cs;
		public byte fg;
		public byte bg;
		public byte gr;

		public bool IsZero
		{
			get { return (cs+fg+bg+gr)==0;}
		}
	}
}
