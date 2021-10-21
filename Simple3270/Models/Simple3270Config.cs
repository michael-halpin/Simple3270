#region License
/* 
 *
 * Simple3270Web - A simple implementation of the TN3270/TN3270E protocol for C#
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
    /// Configuration struct for instantiating a new Simple3270Api
    /// </summary>
    public struct Simple3270Config
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Lu { get; set; }
        public bool UseSsl { get; set; }
        public bool FastScreenMode { get; set; }
        public string TerminalType { get; set; }
        public bool Debug { get; set; }
        public int ActionTimeout { get; set; }
        public int ColorDepth { get; set; }

        public Simple3270Config(string server, int port, bool useSsl = true, string lu = null, bool debug = false,
            string terminalType = "IBM-3278-2-E", bool fastScreenMode = true, int actionTimeout = 1000, int colorDepth = 2)
        {
            Server = server;
            Port = port;
            UseSsl = useSsl;
            Lu = lu;
            Debug = debug;
            TerminalType = terminalType;
            FastScreenMode = fastScreenMode;
            ActionTimeout = actionTimeout;
            ColorDepth = colorDepth;
        }
    }
}