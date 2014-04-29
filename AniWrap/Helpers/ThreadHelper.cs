using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Web;
using AniWrap.DataTypes;

namespace AniWrap.Helpers
{
    public static class ThreadHelper
    {
        public static string Guess_Post_Title(GenericPost t)
        {
            if (String.IsNullOrEmpty(t.Subject))
            {
                if (String.IsNullOrEmpty(t.Comment))
                {
                    return t.ID.ToString();
                }
                else
                {
                    string comment = "";

                    HtmlAgilityPack.HtmlDocument d = new HtmlAgilityPack.HtmlDocument();
                    d.LoadHtml(t.Comment);

                    comment = HttpUtility.HtmlDecode(d.DocumentNode.InnerText);
                    if (comment.Length > 25)
                    {
                        return comment.Remove(24) + "...";
                    }
                    else
                    {
                        return comment;
                    }

                }
            }
            else
            {
                return HttpUtility.HtmlDecode(t.Subject);
            }
        }
    }
}
