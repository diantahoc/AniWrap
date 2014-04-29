using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using HtmlAgilityPack;

namespace AniWrap
{
    public static class Common
    {
        /// <summary>
        /// {0}: board, {1}: tim, {2}: ext
        /// </summary>
        public const string imageLink = @"http://i.4cdn.org/{0}/{1}.{2}";
        /// <summary>
        /// {0}:board, {1}: tim
        /// </summary>
        public const string thumbLink = @"http://t.4cdn.org/{0}/{1}s.jpg";

        public static readonly DateTime UnixEpoch = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static DateTime ParseUTC_Stamp(int timestamp)
        {
            return UnixEpoch.AddSeconds(timestamp); ;
        }

        public static string MD5(string s)
        {
            using (MD5CryptoServiceProvider md5s = new MD5CryptoServiceProvider())
            {
                return ByteArrayToString(md5s.ComputeHash(System.Text.Encoding.ASCII.GetBytes(s)));
            }
        }

        private static string ByteArrayToString(byte[] arrInput)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte x in arrInput)
            {
                sb.Append(x.ToString("X2"));
            }
            return sb.ToString().ToLower();
        }

        public static string DecodeHTML(string text)
        {
            if (!(String.IsNullOrEmpty(text) || String.IsNullOrWhiteSpace(text)))
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument(); doc.LoadHtml(text);
                return System.Web.HttpUtility.HtmlDecode(Common.GetNodeText(doc.DocumentNode));
            }
            else
            {
                return "";
            }
        }

        private static string GetNodeText(HtmlNode node)
        {
            StringBuilder sb = new StringBuilder();

            if (node.Name == "br")
            {
                sb.AppendLine();
            }
            else if (node.ChildNodes.Count > 0)
            {
                foreach (HtmlNode a in node.ChildNodes)
                {
                    sb.Append(Common.GetNodeText(a));
                }
            }
            else
            {
                return node.InnerText;
            }

            return sb.ToString();
        }

        public static string Map_MIME_Type(string filename)
        {
            return "application/octet-stream";
        }

        public static string format_size_string(int size)
        {
            long KB = 1024;
            long MB = 1048576;
            long GB = 1073741824;
            if (size < KB)
            {
                return size.ToString() + " B";
            }
            else if (size > KB & size < MB)
            {
                return Math.Round(Convert.ToDouble(size / KB), 2).ToString() + " KB";
            }
            else if (size > MB & size < GB)
            {
                return Math.Round(Convert.ToDouble(size / MB), 2).ToString() + " MB";
            }
            else if (size > GB)
            {
                return Math.Round(Convert.ToDouble(size / GB), 2).ToString() + " GB";
            }
            else
            {
                return Convert.ToString(size);
            }
        }

        public static int GetBoardMaximumFileSize(string board)
        {
            if (board == "b" || board == "s4s" || board == "r9k") { return 2; }

            if (board == "gd" || board == "hm" || board == "hr" || board == "po" || board == "r" || board == "s" || board == "trv" || board == "tg") { return 8; }

            if (board == "out" || board == "p" || board == "w" || board == "wg") { return 5; }

            if (board == "gif" || board == "soc" || board== "sp" || board == "wsg") { return 4; }

            return 3;
        }
    }
}
