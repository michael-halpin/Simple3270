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
using Simple3270.Models;

namespace Simple3270.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReadScreenController : ControllerBase
    {
        [HttpPost]
        public ActionResult<List<SimpleOutput>> Get(string sessionId, [FromBody] List<SimpleInput> fields)
        {
            #region Data Validations
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrEmpty(sessionId))
            {
                return BadRequest(nameof(sessionId));
            }
            foreach (SimpleInput si in fields)
            {
                if (string.IsNullOrEmpty(si.Name))
                {
                    return BadRequest(si.Name);
                }
                if (si.X < 1)
                {
                    return BadRequest(si.X);
                }
                if (si.Y < 1)
                {
                    return BadRequest(si.Y);
                }
                if (si.L < 1)
                {
                    return BadRequest(si.L);
                }
            }
            #endregion

            int i = EstablishConnectionController.EmuIds.IndexOf(sessionId);
            if (i == -1)
            {
                return NotFound(nameof(sessionId));
            }
            var output = EstablishConnectionController.Emus[i].ReadScreen(fields);
            EstablishConnectionController.Timeout[i] = DateTime.UtcNow;
            
            return output;
        }
    }
}