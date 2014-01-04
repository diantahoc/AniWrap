﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AniWrap.Helpers;

namespace AniWrap.DataTypes
{
   public class ThreadContainer
    {
       private Dictionary<int, Reply> _childs;

       public ThreadContainer(Thread instance)
        {
            _childs = new Dictionary<int, Reply>();
            this.Instance = instance;
            this.Title = ThreadHelper.Guess_Post_Title(instance);
        }

       /// <summary>
       /// OP Post.
       /// </summary>
       public Thread Instance { get ; private set; }

       public void AddReply(GenericPost reply) 
        {
                if (_childs.ContainsKey(reply.ID))
                {
                    //?? what do
                    return;
                }
                else
                {
                    Reply r = new Reply(this.Instance, reply);

                    CommentToken[] tokens = ThreadHelper.TokenizeComment(reply.Comment);

                    foreach (CommentToken token in tokens)
                    {
                        if (token.Type == CommentToken.TokenType.Quote)
                        {
                            int id = Convert.ToInt32(token.TokenData);
                            if (id == this.Instance.ID) //if quoting op
                            {
                                this.Instance.MarkAsQuotedBy(r.ID);
                            }
                            else if (id == r.ID) //if quoting self
                            {
                                r.MarkAsQuotedBy(id);
                            }
                            else
                            {
                                Reply qq = GetPost(Convert.ToInt32(token.TokenData));
                                if (qq != null) { qq.MarkAsQuotedBy(r.ID); }
                            }
                        }
                    }

                    this._childs.Add(reply.ID, r);
                }
        }

       public Reply GetPost(int id) 
        {
           if (_childs.ContainsKey(id))      
           {   
               return _childs[id];   
           }  
           else 
           { 
               return null; 
           }
        }

       public void RemovePost(int id)
        {
           if (_childs.ContainsKey(id))
           {
               _childs.Remove(id);
           }
        }

       public Reply[] Replies { get { return _childs.Values.ToArray(); } }

       public string Title { get; private set; }

    }
}