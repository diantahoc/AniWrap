using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniWrap.DataTypes
{
    public class CatalogItem
    {
        public int ID;
        public DateTime time;
        public string comment;
        public string subject;
        public string trip;
        public string name;
        public string email;
        public string board;
        public int image_replies;
        public int text_replies;
        public int page_number;
        public PostFile file;
        public int TotalReplies { get { return image_replies + text_replies; } }
        public GenericPost[] trails;
    }
}
