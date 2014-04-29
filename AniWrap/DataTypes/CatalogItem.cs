using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AniWrap.DataTypes
{
    public class CatalogItem : GenericPost
    {
        public CatalogItem() { }

        public CatalogItem(GenericPost base_data)
        {
            base.Board = base_data.Board;
            base.Capcode = base_data.Capcode;
            base.Comment = base_data.Comment;
            base.CountryFlag = base_data.CountryFlag;
            base.CountryName = base_data.CountryName;
            base.Email = base_data.Email;
            base.File = base_data.File;
            base.Name = base_data.Name;
            base.ID = base_data.ID;
            base.Subject = base_data.Subject;
            base.Tag = base_data.Tag;
            base.Time = base_data.Time;
            base.Trip = base_data.Trip;
        }

        public int image_replies;
        public int text_replies;

        public int page_number;

        public int TotalReplies { get { return image_replies + text_replies; } }
        public bool IsClosed { get; set; }
        public bool IsSticky { get; set; }
        public GenericPost[] trails;
    }
}
