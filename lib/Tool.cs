﻿/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/8/29
 * 时间: 21:42
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace YGOCore
{
    /// <summary>
    /// Description of Tool.
    /// </summary>
    public class Tool
    {
        #region Gets the Web site content
        public static string PostHtmlContentByUrl(string url, string param, int outtime = 30 * 1000)
        {
            string htmlContent = string.Empty;
            try
            {
                HttpWebRequest httpWebRequest =
                    (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = outtime;
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.94 Safari/537.36";

                byte[] bs = Encoding.UTF8.GetBytes(param);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.ContentLength = bs.Length;
                using (Stream reqStream = httpWebRequest.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                    reqStream.Close();
                }

                using (HttpWebResponse httpWebResponse =
                      (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream stream = httpWebResponse.GetResponseStream())
                    {
                        using (StreamReader streamReader =
                              new StreamReader(stream, Encoding.UTF8))
                        {
                            htmlContent = streamReader.ReadToEnd();
                            streamReader.Close();
                        }
                        stream.Close();
                    }
                    httpWebResponse.Close();
                }
                return htmlContent;
            }
            catch
            {

            }
            return "";
        }
        public static string GetHtmlContentByUrl(string url, int outtime = 30 * 1000)
        {
            string htmlContent = string.Empty;
            try
            {
                HttpWebRequest httpWebRequest =
                    (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Timeout = outtime;
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.94 Safari/537.36";
                using (HttpWebResponse httpWebResponse =
                      (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream stream = httpWebResponse.GetResponseStream())
                    {
                        using (StreamReader streamReader =
                              new StreamReader(stream, Encoding.UTF8))
                        {
                            htmlContent = streamReader.ReadToEnd();
                            streamReader.Close();
                        }
                        stream.Close();
                    }
                    httpWebResponse.Close();
                }
                return htmlContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return "";
        }
        #endregion
        #region MD5check

        public static string SubString(string str, int i, int len)
        {
            if (i > len) return str;
            if (str.Length < i + len)
            {
                return str.Substring(i, len);
            }
            if (str.Length < i)
            {
                return str;
            }
            return str.Substring(i, str.Length - i);
        }
        /// <summary>
        /// MD5　32Bit encryption
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMd5(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();//Instantiate a MD5
                                   // Encryption type is a byte array, choice of encoding UTF8/Unicode to note here
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // By using a loop, an array of type byte is converted to a string, which is derived from normal character formatting
            for (int i = 0; i < s.Length; i++)
            {
                // The resulting string uses the hexadecimal type format。Format characters are lowercase letters，If you use uppercase（X）The format characters are uppercase

                pwd = pwd + s[i].ToString("x2");

            }
            return pwd;
        }
        /// <summary>
        /// Calculate file MD5 checksum
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFileMD5(string fileName)
        {
            if (!File.Exists(fileName))
                return "";
            long filesize = 0;
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                filesize = file.Length;
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch //(Exception ex)
            {
                //throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
            return filesize.ToString();
        }
        #endregion

        #region File path
        public static string GetDir(string file)
        {
            string dir = null;
            try
            {
                dir = Path.GetDirectoryName(file);
            }
            catch { }
            return dir;
        }
        public static bool CreateDirectory(string dir)
        {
            if (dir == null)
            {
                return false;
            }
            bool ok = false;
            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch { ok = false; }
            }
            return ok;
        }
        public static string RemoveInvalid(string str)
        {
            if (str == null)
            {
                return "";
            }
            char[] chars = Path.GetInvalidFileNameChars();
            foreach (char c in chars)
            {
                str = str.Replace(c, '_');
            }
            return str;
        }

        /// <summary>
        /// Merge paths
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string Combine(params string[] paths)
        {
            if (paths.Length == 0)
            {
                throw new ArgumentException("please input path");
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                string spliter = "/";
                string firstPath = paths[0];
                if (firstPath.StartsWith("HTTP", StringComparison.OrdinalIgnoreCase))
                {
                    spliter = "/";
                }
                if (!firstPath.EndsWith(spliter))
                {
                    firstPath = firstPath + spliter;
                }
                builder.Append(firstPath);
                for (int i = 1; i < paths.Length; i++)
                {
                    string nextPath = paths[i];
                    if (nextPath.StartsWith("/") || nextPath.StartsWith("\\"))
                    {
                        nextPath = nextPath.Substring(1);
                    }
                    if (i != paths.Length - 1)//not the last one
                    {
                        if (nextPath.EndsWith("/") || nextPath.EndsWith("\\"))
                        {
                            nextPath = nextPath.Substring(0, nextPath.Length - 1) + spliter;
                        }
                        else
                        {
                            nextPath = nextPath + spliter;
                        }
                    }
                    builder.Append(nextPath);
                }
                return Path.GetFullPath(builder.ToString());
            }
        }
        #endregion

        #region string unicode
        /// <summary>
        /// String to Unicode code string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string StringToUnicode(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            char[] charbuffers = s.ToCharArray();
            byte[] buffer;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < charbuffers.Length; i++)
            {
                buffer = System.Text.Encoding.Unicode.GetBytes(charbuffers[i].ToString());
                sb.Append(String.Format("\\u{0:X2}{1:X2}", buffer[1], buffer[0]));
            }
            return sb.ToString();
        }
        /// <summary>
        /// Unicode String conversion to normal string
        /// </summary>
        /// <param name="srcText"></param>
        /// <returns></returns>
        public static string UnicodeToString(string srcText)
        {
            string dst = "";
            string src = srcText;
            int len = srcText.Length / 6;
            for (int i = 0; i <= len - 1; i++)
            {
                string str = "";
                str = src.Substring(0, 6).Substring(2);
                src = src.Substring(6);
                byte[] bytes = new byte[2];
                bytes[1] = byte.Parse(int.Parse(str.Substring(0, 2), NumberStyles.HexNumber).ToString());
                bytes[0] = byte.Parse(int.Parse(str.Substring(2, 2), NumberStyles.HexNumber).ToString());
                dst += Encoding.Unicode.GetString(bytes);
            }
            return dst;
        }
        #endregion

        #region des
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceString"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string Encrypt(string sourceString, string key, string iv)
        {
            byte[] btKey = Encoding.UTF8.GetBytes(key);

            byte[] btIV = Encoding.UTF8.GetBytes(iv);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            string rs = null;
            byte[] inData = Encoding.UTF8.GetBytes(sourceString);
            using (MemoryStream ms = new MemoryStream())
            {
                
                try
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);

                        cs.FlushFinalBlock();
                    }

                    rs =  Convert.ToBase64String(ms.ToArray());
                }
                catch
                {
                   
                }
            }
            if(rs == null)
            {
                try {
                    rs = Convert.ToBase64String(inData);
                }
                catch
                {

                }
            }
            return rs;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedString"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string Decrypt(string encryptedString, string key, string iv)
        {
            byte[] btKey = Encoding.UTF8.GetBytes(key);

            byte[] btIV = Encoding.UTF8.GetBytes(iv);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            string rs = null;
            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    byte[] inData = Convert.FromBase64String(encryptedString);
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);

                        cs.FlushFinalBlock();
                    }

                    rs =  Encoding.UTF8.GetString(ms.ToArray());
                }
                catch
                {
                   
                }
            }
            if(rs == null)
            {
                try
                {
                    byte[] inData = Convert.FromBase64String(encryptedString);
                    rs = Encoding.UTF8.GetString(inData);
                }
                catch
                {

                }
            }
            return rs;
        }
        #endregion
    }
}
