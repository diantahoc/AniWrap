using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniWrap
{
    public static class Common
    {
        public static string imageLink = @"http://i.4cdn.org/#/src/$";
        public static string thumbLink = @"http://t.4cdn.org/#/thumb/$s.jpg";

        public static DateTime ParseUTC_Stamp(int timestamp)
        {
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(timestamp);
            return dateTime;
        }

        public static string MD5(string s)
        {
            using (System.Security.Cryptography.MD5CryptoServiceProvider md5s = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(s);
                string result = ByteArrayToString(md5s.ComputeHash(bytes));
                return result;
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
