#region License
/* 
 *
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
 */
#endregion
using System;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Simple3270Console
{
    /// <summary>
    /// Used to encrypt/decrypt cross platform data.
    /// </summary>
    public static class Steganography
    {
        /// <summary>
        /// Used to encrypt string data.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="key">The encryption key to use.</param>
        /// <param name="iv">The encryption initialization vector.</param>
        /// <returns>string</returns>
        public static string Encrypt(string plainText, byte[] key, byte[] iv)
        {
            //TODO: For some reson encryption/decryption is causing extra junk characters to be appended at the end
            //TODO: of the string.  Need to figure out why this is happening to avoid this work around.
            plainText = plainText.Length + "|" + plainText;

            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted, Base64FormattingOptions.None);
        }
        /// <summary>
        /// Used to decrypt string data.
        /// </summary>
        /// <param name="encryptedText">The text to decrypt.</param>
        /// <param name="key">The encryption key to use.</param>
        /// <param name="iv">The encryption initialization vector.</param>
        /// <returns>string</returns>
        public static string Decrypt(string encryptedText, byte[] key, byte[] iv)
        {
            byte[] cipherText = Convert.FromBase64String(encryptedText);

            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            #region TODO: For some reson encryption/decryption is causing extra junk characters to be appended at the end
            // TODO: TODO: of the string.  Need to figure out why this is happening to avoid this work around.
            int i = plaintext.IndexOf('|');
            int lng = Convert.ToInt32(plaintext.Substring(i + 1));
            plaintext = plaintext.Substring(i - lng, lng);
            #endregion
            
            return plaintext;
        }
        /// <summary>
        /// Used to decrypt data to a binary array.
        /// </summary>
        /// <param name="encryptedText">The text to decrypt.</param>
        /// <param name="key">The encryption key to use.</param>
        /// <param name="iv">The encryption initialization vector.</param>
        /// <param name="length">The length of the binary data array.</param>
        /// <returns></returns>
        public static byte[] DecryptToBytes(string encryptedText, byte[] key, byte[] iv, int length)
        {
            byte[] cipherText = Convert.FromBase64String(encryptedText);

            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            byte[] ary;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(cipherText, 0, cipherText.Length);
                        cryptoStream.FlushFinalBlock();
                        ary = memoryStream.ToArray();
                    }
                }
            }

            return ary;
        }

        /// <summary>
        /// Takes a request and packages it up into an encrypted packet.
        /// </summary>
        /// <param name="json">The json message to encrypt.</param>
        /// <param name="method">The method that this is associated with on the server side.</param>
        /// <returns></returns>
        public static string Package(string json, byte[] key, byte[] iv)
        {
            string request = ("{'package':'%1'}").ToJson(Encrypt(json, key, iv));
            return request;
        }
        public static string Unpackage(string json, byte[] key, byte[] iv)
        {
            dynamic rsp = JsonConvert.DeserializeObject(json);
            string txt = rsp["package"];
            string response = Decrypt(txt, key, iv);
            return response;
        }
    }
    
}