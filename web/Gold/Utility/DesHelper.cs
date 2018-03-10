using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Security.Cryptography;
using System.IO;
using System.Text;


namespace Gold.Utility
{
    public class DesHelper
    {
        /// <summary>
        /// 加密解密的密钥
        /// </summary>
        static byte[] DESKey = new byte[] 
        { 0,1, 2, 3, 4, 5, 6,7,
          0,1, 2, 3, 4, 5, 6,7,
          0,1, 2, 3, 4, 5, 6,7,
          0,1, 2, 3, 4, 5, 6,7
        };

        #region DES加密解密
        /// <summary>
        /// DES加密（使用默认密钥）
        /// </summary>
        /// <param name="strSource">待加密字串</param>
        /// <param name="key">32位Key值</param>
        /// <returns>加密后的字符串</returns>
        public static string DESEncrypt(string strSource)
        {
            return DESEncrypt(strSource, DESKey);
        }
        public static string DESEncrypt(string strSource, byte[] key)
        {
            SymmetricAlgorithm sa = Rijndael.Create();
            sa.Key = key;
            sa.Mode = CipherMode.ECB;
            sa.Padding = PaddingMode.Zeros;//长度不够时补零
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, sa.CreateEncryptor(), CryptoStreamMode.Write);
            byte[] byt = Encoding.Unicode.GetBytes(strSource);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();
            cs.Close();
            return Convert.ToBase64String(ms.ToArray());
        }
        /// <summary>
        /// DES解密（使用默认密钥）
        /// </summary>
        /// <param name="strSource">待解密的字串</param>
        /// <param name="key">32位Key值</param>
        /// <returns>解密后的字符串</returns>
        public static string DESDecrypt(string strSource)
        {
            return DESDecrypt(strSource, DESKey);
        }
        public static string DESDecrypt(string strSource, byte[] key)
        {
            SymmetricAlgorithm sa = Rijndael.Create();
            sa.Key = key;
            sa.Mode = CipherMode.ECB;
            sa.Padding = PaddingMode.Zeros;//长度不够时补零
            ICryptoTransform ct = sa.CreateDecryptor();
            byte[] byt = Convert.FromBase64String(strSource);
            MemoryStream ms = new MemoryStream(byt);
            CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs, Encoding.Unicode);
            return sr.ReadToEnd();
        }
        #endregion

    }
}