using System;
using System.Security.Cryptography;
using System.Text;

namespace Ctrl.Core.Core.Security
{
    /// <summary>
    ///     3DES加密算法：<see cref="CipherMode.CBC">模式加密
    /// </summary>
    public class _3DESEncrypt
    {
        public const string IV = "97351320";
        public static byte[] Key = { 0xFF, 0xF1, 0xF2, 0x3, 0x14, 0x55, 0x36, 0x12, 0x90, 0x20, 0x2A, 0x9B, 0xD4, 0x5D, 0xFE, 0x5F };

        #region =======加密=======
        /// <summary>
        ///     加密
        /// </summary>
        /// <param name="Text">需要加密的内容</param>
        /// <returns></returns>
        public static string Encrypt(string Text) {
            return Encrypt(Text,Key);
        }
        /// <summary>
        ///     加密数据<seealso cref="Key"/>长度必须3*8
        /// </summary>
        /// <param name="Text">需要加密的内容</param>
        /// <param name="sKey">密钥</param>
        /// <returns></returns>
        public static string Encrypt(string Text,byte[] sKey) {
            try
            {
             
                var des = new TripleDESCryptoServiceProvider
                {
                    Key = sKey,
                    Mode = CipherMode.CBC,
                    IV = Encoding.UTF8.GetBytes(IV)
                };
                var desEncrypt = des.CreateEncryptor();
                byte[] buffer = Encoding.UTF8.GetBytes(Text);
                return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer,0,buffer.Length));
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
        }
        #endregion

        #region =======解密=======
        /// <summary>
        ///     解密数据
        /// </summary>
        /// <param name="Text">需要解密的内容</param>
        /// <returns></returns>
        public static string Decrypt(string Text)
        {
            return Decrypt(Text, Key);
        }

        /// <summary>
        ///     解密数据
        /// </summary>
        /// <param name="Text">需要解密的内容</param>
        /// <param name="sKey">密钥</param>
        /// <returns></returns>
        public static string Decrypt(string Text, byte[] sKey)
        {
            try
            {
                var des = new TripleDESCryptoServiceProvider
                {
                    Key = sKey,
                    Mode = CipherMode.CBC,
                    IV = Encoding.UTF8.GetBytes(IV),
                    Padding = PaddingMode.PKCS7
                };
                var desDecrypt = des.CreateDecryptor();
                byte[] buffer = Convert.FromBase64String(Text);
                return Encoding.UTF8.GetString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
        }
        #endregion

    }
}
