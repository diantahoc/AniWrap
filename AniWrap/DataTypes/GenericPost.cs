using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AniWrap.Helpers;

namespace AniWrap.DataTypes
{
   public class GenericPost
    {
       public GenericPost() 
       {
           this.Capcode = CapcodeEnum.None;
       }

       public int ID { get; set; }

       public DateTime Time;

       public string Comment { get; set; }

       public string Subject { get; set; }
       public string Trip { get; set; }
       public string Name { get; set; }
       public string Email { get; set; }
       public string Board { get; set; }

       public PostFile File;

       public CapcodeEnum Capcode { get; set; }

       public string country_flag { get; set; }
       public string country_name { get; set; }

       public enum CapcodeEnum { Admin, Mod, Developer, None }

       public CommentToken[] CommentTokens { get { return ThreadHelper.TokenizeComment(this.Comment); } }

       private List<int> _my_quoters = new List<int>();

       public void MarkAsQuotedBy(int id)
       {
           if (!_my_quoters.Contains(id))
           {
               _my_quoters.Add(id);
           }
       }

       public int[] QuotedBy { get { return _my_quoters.ToArray(); } }

    }
}
