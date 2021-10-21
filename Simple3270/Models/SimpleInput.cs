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

using System;

namespace Simple3270.Models
{
    public class SimpleInput
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int L { get; set; }
        public SimpleInput(string name, int x, int y, int l)
        {
            Name = name;
            X = x;
            Y = y;
            L = l;
        }

        public SimpleInput()
        {
            Name = "";
            X = 0;
            Y = 0;
            L = 0;
        }
        public SimpleInput(SimpleOutput field)
        {
            this.Name = field.Name;
            this.X = field.X;
            this.Y = field.Y;
            this.L = field.Value.Length;
        }

        public SimpleInput(WaitForRequest field)
        {
            this.Name = "WaitForText";
            this.X = field.X;
            this.Y = field.Y;
            this.L = field.Value.Length;
        }
    }
}