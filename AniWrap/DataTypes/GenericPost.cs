using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AniWrap.Helpers;
using System.Web;

namespace AniWrap.DataTypes
{
    public class GenericPost
    {
        public GenericPost()
        {
            this.Capcode = CapcodeEnum.None;
        }

        public int ID { get; set; }

        public DateTime Time { get; set; }

        public string Comment { get; set; }

        public string CommentText
        {
            get
            {
                return Common.DecodeHTML(this.Comment);
            }
        }

        public string Subject { get; set; }

        public string Trip { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Board { get; set; }

        public PostFile File;

        public CapcodeEnum Capcode { get; set; }

        public string CountryFlag { get; set; }
        public string CountryName { get; set; }

        public enum PostType { FourChan, Fuuka, FoolFuuka }

        public enum CapcodeEnum { Admin, Mod, Developer, None }

        public enum ThreadTag
        {
            Other,
            Game,
            Loop,
            Japanese,
            Anime,
            Porn,
            Hentai,
            NoTag,
            Unknown
        }

        public ThreadTag Tag { get; set; }
    }
}
