# region License
"""
 * SimplRPA - A simple RPA library for Python and C#
 *
 * Copyright (c) 2009-2021 Michael S. Halpin
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
"""
# endregion
import base64
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad

# region Encryption/Decryption Methods
def decrypt(text, key, iv):
    """
    Decrypts a text to a string.
    :param text: The encrypted text to decrypt.
    :param key: The key used to perform encryption.
    :param iv: The initialization vector used to encrypt the text.
    :return: string
    """
    aes = AES.new(bytes(key, 'utf-8'), AES.MODE_CBC, bytes(iv, 'utf-8'))
    decd = aes.decrypt(base64.b64decode(text))
    text = str(decd, "utf-8")
    # region TODO: For some reson encryption/decryption is causing extra junk characters to be appended at the end
    # TODO: of the string.  Need to figure out why this is happening to avoid this work around.
    i = text.index('|')
    idx = i + 1
    lng = int(text[:i])
    text = text[idx:lng+idx]
    # endregion
    return text


def encrypt(text, key, iv):
    """
    Encrypts a string into an encrypted text.
    :param text: The unencrypted string to use.
    :param key: The key used to perform encryption.
    :param iv: The initialization vector used to encrypt the text.
    :return: string
    """
    _iv = bytes(iv, 'utf-8')
    _key = bytes(key, 'utf-8')
    aes = AES.new(_key, AES.MODE_CBC, _iv)
    # TODO: For some reson encryption/decryption is causing extra junk characters to be appended at the end
    # TODO: of the string.  Need to figure out why this is happening to avoid this work around.
    text = text + '|' + str(len(text))
    return base64.b64encode(_iv + aes.encrypt(pad(text.encode('utf-8'), AES.block_size))).decode()


def encrypt_bytes(bytez, key, iv):
    """
    Encrypts a byte array into an encrypted string.
    :param bytez: The byte array to encrypt.
    :param key: The key used to perform encryption.
    :param iv: The initialization vector used to encrypt the text.
    :return: string
    """
    _iv = bytes(iv, 'utf-8')
    _key = bytes(key, 'utf-8')
    aes = AES.new(_key, AES.MODE_CBC, _iv)
    return base64.b64encode(_iv + aes.encrypt(pad(bytez, AES.block_size))).decode()


def package(json, method, key, iv):
    request = '{"package":"' + encrypt(json, key, iv) + '", "method":"' + encrypt(method, key, iv) + '"}'
    return request


def unpackage(json, key, iv):
    txt = json["package"]
    response = decrypt(txt, key, iv)
    return response
# endregion
