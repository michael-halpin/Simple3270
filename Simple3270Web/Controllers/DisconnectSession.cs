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
using Microsoft.AspNetCore.Mvc;


namespace Simple3270.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DisconnectSessionController : ControllerBase
    {
        [HttpPost]
        public ActionResult<bool> Get(string sessionId)
        {
            #region Dispose of and remove the requested session.
            if (!string.IsNullOrEmpty(sessionId)) //Skip invalid entries
            {
                int i = EstablishConnectionController.EmuIds.IndexOf(sessionId);
                if (i != -1)
                {
                    EstablishConnectionController.Emus[i].Dispose();
                    EstablishConnectionController.Emus.RemoveAt(i);
                    EstablishConnectionController.Timeout.RemoveAt(i);
                    EstablishConnectionController.EmuIds.RemoveAt(i);
                }
            }
            #endregion

            #region Remove other sessions that have timed out.
            for (int n = 0; n < EstablishConnectionController.Emus.Count; n++)
            { // Make sure the timeout check runs even if the input is invalid.
                if (EstablishConnectionController.Timeout[n] < DateTime.UtcNow.AddMinutes(-15))
                {
                    EstablishConnectionController.Emus[n].Dispose();
                    EstablishConnectionController.Emus.RemoveAt(n);
                    EstablishConnectionController.Timeout.RemoveAt(n);
                    EstablishConnectionController.EmuIds.RemoveAt(n);
                    n--;
                }
            }
            #endregion

            #region Data Validation
            if (string.IsNullOrEmpty(sessionId))
            {
                return BadRequest(nameof(sessionId));
            }
            #endregion

            return true;
        }
    }
}