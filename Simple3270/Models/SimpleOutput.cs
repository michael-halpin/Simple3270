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
namespace Simple3270.Models
{
    /// <summary>
    /// Struct for 3270 output operations.
    /// </summary>
    public struct SimpleOutput
    {
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        public string Name { get; set; }
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        public int X { get; set; }
        // ReSharper disable once PropertyCanBeMadeInitOnly.Global
        public int Y { get; set; }
        public string Value { get; set; }

        public SimpleOutput(string name = "", int x = 0, int y = 0, string value = "")
        {
            Name = name;
            X = x;
            Y = y;
            Value = value;
        }
    }
}