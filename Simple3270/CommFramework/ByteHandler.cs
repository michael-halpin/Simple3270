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

namespace Simple3270.Library
{
    internal class ByteHandler
    {
        static public int ToBytes(byte[] buffer, int offset, int data)
        {
            buffer[offset++] = (byte)(data & 0xff);
            buffer[offset++] = (byte)((data & 0xff00) / 0x100);
            buffer[offset++] = (byte)((data & 0xff0000) / 0x10000);
            buffer[offset++] = (byte)((data & 0xff000000) / 0x1000000);
            return offset;
        }
        static public int FromBytes(byte[] buffer, int offset, out int data)
        {
            data = 0;
            data = (int)(buffer[offset++]);
            data += (int)(buffer[offset++] * 0x100);
            data += (int)(buffer[offset++] * 0x10000);
            data += (int)(buffer[offset++] * 0x1000000);
            return offset;
        }
    }
}