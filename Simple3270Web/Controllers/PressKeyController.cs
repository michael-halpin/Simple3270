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
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Simple3270.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PressKeyController : ControllerBase
    {
        private static volatile List<string> _tnKeys = new 
        (
            new []
            {
                "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "F13", "F14", "F15", "F16",
                "F17", "F18", "F19", "F20", "F21", "F22", "F23", "F24", "Tab", "BackTab", "Enter", "Backspace", "Clear",
                "Delete", "DeleteField", "DeleteWord", "Left", "Left2", "Up", "Right", "Right2", "Down", "Attn",
                "CircumNot", "CursorSelect", "Dup", "Erase", "EraseEOF", "EraseInput", "FieldEnd", "FieldMark",
                "FieldExit", "Home", "Insert", "Interrupt", "Key", "Newline", "NextWord", "PAnn", "PreviousWord",
                "Reset", "SysReq", "Toggle", "ToggleInsert", "ToggleReverse", "PA1", "PA2", "PA3", "PA4", "PA5", "PA6",
                "PA7", "PA8", "PA9", "PA10", "PA11", "PA12"
            }
        );
        
        [HttpPost]
        public ActionResult<bool> Get(string sessionId, [FromBody] string key)
        {
            #region Data Validations
            key = key.Replace("\"", "");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrEmpty(sessionId))
            {
                return BadRequest(nameof(sessionId));
            }
            if (string.IsNullOrEmpty(key))
            {
                return BadRequest(nameof(key));
            }
            if (!_tnKeys.Contains(key))
            {
                return BadRequest(nameof(key));
            }
            #endregion

            int i = EstablishConnectionController.EmuIds.IndexOf(sessionId);
            if (i == -1)
            {
                return NotFound(nameof(sessionId));
            }
            bool response = EstablishConnectionController.Emus[i].PressKey(key);
            EstablishConnectionController.Timeout[i] = DateTime.UtcNow;
            return response;
        }
    }
}