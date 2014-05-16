using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using System.Net;

namespace AniWrap
{
    public class PostSender : IDisposable
    {
        private BackgroundWorker bg_r;
        private BackgroundWorker bg_t;

        private string url_template = "https://sys.4chan.org/%/post";

        public PostSender()
        {
            bg_r = new BackgroundWorker();
            bg_r.DoWork += new DoWorkEventHandler(bg_r_DoWork);
            bg_r.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_r_RunWorkerCompleted);

            bg_t = new BackgroundWorker();
            bg_t.DoWork += new DoWorkEventHandler(bg_t_DoWork);
            bg_t.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_t_RunWorkerCompleted);
        }



        private void bg_r_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument.GetType() == typeof(object[]))
            {
                object[] data = (object[])e.Argument;
                try
                {
                    PostSenderResponse psr = SendReply((string)data[0], (int)data[1], (PostSenderData)data[2], (SolvedCaptcha)data[3]);

                    e.Result = psr;
                    return;
                }
                catch (Exception ex)
                {
                    e.Result = ex;
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void bg_r_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                if (SendRequestFailed != null)
                {
                    SendRequestFailed((Exception)(e.Result), SendType.Reply);
                }
            }
            else
            {
                if (SendCompleted != null)
                {
                    SendCompleted((PostSenderResponse)e.Result, SendType.Reply);
                }
            }
        }

        private void bg_t_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument.GetType() == typeof(object[]))
            {
                object[] data = (object[])e.Argument;
                try
                {
                    PostSenderResponse psr = MakeThread((string)data[0], (PostSenderData)data[1], (SolvedCaptcha)data[2]);

                    e.Result = psr;
                    return;
                }
                catch (Exception ex)
                {
                    e.Result = ex;
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void bg_t_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                if (SendRequestFailed != null)
                {
                    SendRequestFailed((Exception)(e.Result), SendType.Thread);
                }
            }
            else
            {
                if (SendCompleted != null)
                {
                    SendCompleted((PostSenderResponse)e.Result, SendType.Thread);
                }
            }
        }

        public PostSenderResponse SendReply(string board, int threadID, PostSenderData data, SolvedCaptcha captcha)
        {
            if (String.IsNullOrEmpty(board))
            {
                throw new ArgumentNullException("Board cannot be null");
            }

            if (threadID <= 0)
            {
                throw new ArgumentException("Invalid thread ID");
            }

            if (data != null)
            {

                if (String.IsNullOrEmpty(data.Comment) && String.IsNullOrEmpty(data.FileName))
                {
                    throw new Exception("Blank posts are not allowed");
                }

                if (data.Comment.Length > 1500)
                {
                    throw new Exception("Comment is too long");
                }

                if (!data.Is4chanPass && (captcha == null)) { throw new ArgumentNullException("Captcha is null"); }

                if (!data.Is4chanPass && (captcha != null))
                {
                    if (string.IsNullOrEmpty(captcha.ResponseField)) { throw new ArgumentNullException("The captcha is unsolved"); }
                }
            }
            else
            {
                throw new ArgumentNullException("Post data is null");
            }

            return work(data, board, true, threadID, captcha);
        }

        public PostSenderResponse MakeThread(string board, PostSenderData data, SolvedCaptcha captcha)
        {
            if (String.IsNullOrEmpty(board))
            {
                throw new ArgumentNullException("Board cannot be null");
            }

            if (data != null)
            {
                if (!String.IsNullOrEmpty(data.FilePath))
                {
                    if (!System.IO.File.Exists(data.FilePath))
                    {
                        throw new ArgumentException("The provided file path lead to a non-existing file");
                    }
                }

                if (data.Comment.Length > 1500)
                {
                    throw new Exception("Comment is too long");
                }

                if (!data.Is4chanPass && (captcha == null)) { throw new ArgumentNullException("Captcha is null"); }

                if (!data.Is4chanPass && (captcha != null))
                {
                    if (string.IsNullOrEmpty(captcha.ResponseField)) { throw new ArgumentNullException("The captcha is unsolved"); }
                }
            }
            else
            {
                throw new ArgumentNullException("Post data is null");
            }

            return work(data, board, false, 0, captcha);
        }

        private PostSenderResponse work(PostSenderData data, string board, bool isreply, int tid, SolvedCaptcha captcha)
        {
            PostSenderResponse resp = null;

            //To hold all the form variables
            NameValueCollection values = new NameValueCollection();

            FileStream fs = null;

            List<UploadFile> files = null;

            // 'upfile' is the name of the the uploaded file
            if (!(String.IsNullOrEmpty(data.FileName) | String.IsNullOrEmpty(data.FilePath)))
            {
                fs = new FileStream(data.FilePath, FileMode.Open);
                files = new List<UploadFile>();
                files.Add(
                    new UploadFile
                    {
                        Name = "upfile",
                        Filename = data.FileName,
                        ContentType = Common.Map_MIME_Type(data.FileName),
                        Stream = fs
                    }
                    );
            }
            else
            {
                files = new List<UploadFile>();
                files.Add(
                    new UploadFile
                    {
                        Name = "upfile",
                        Filename = "",
                        ContentType = "application/octet-stream",
                        Stream = Stream.Null
                    }
                    );
            }

            values.Add("MAX_FILE_SIZE", Convert.ToString(Common.GetBoardMaximumFileSize(board) * 1024 * 1024));

            values.Add("mode", "regist");

            if (isreply)
            {
                values.Add("resto", tid.ToString());
            }

            values.Add("name", data.Name);
            values.Add("email", data.Email);
            values.Add("sub", data.Subject);
            values.Add("com", data.Comment);
            values.Add("pwd", data.PostPassword);

            //Check if the user is using a 4chan pass (is_pass is a boolean).
            //moot remove the captcha when a pass is availble, so recaptcha fields are not needed.
            if (!data.Is4chanPass)
            {
                values.Add("recaptcha_response_field", captcha.ResponseField);
                values.Add("recaptcha_challenge_field", captcha.ChallengeField);
            }

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url_template.Replace("%", board));
            request.Method = "POST";
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
            request.Referer = "http://boards.4chan.org/%".Replace("%", board);


            //This is the part we encode data using "multipart/form-data" technique 
            //RFC 1867 : http://tools.ietf.org/html/rfc1867#section-6

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "--" + boundary;
            using (Stream requestStream = request.GetRequestStream())
            {
                // Write the values
                foreach (string name in values.Keys)
                {
                    byte[] buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}", name, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(values[name] + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                // Write the files

                foreach (UploadFile file in files)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", file.Name, file.Filename, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Type: {0}{1}{1}", file.ContentType, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    file.Stream.CopyTo(requestStream);
                    buffer = Encoding.ASCII.GetBytes(Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }
                byte[] boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
                requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);
            }

            //response text

            string response_text;
            using (WebResponse response = request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (MemoryStream stream = new MemoryStream())
            {
                responseStream.CopyTo(stream);
                response_text = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }

            if (fs != null) { fs.Close(); }

            if (!String.IsNullOrEmpty(response_text))
            {
                resp = new PostSenderResponse(response_text);
            }
            return resp;
        }

        public void MakeThreadAsyc(string board, PostSenderData data, SolvedCaptcha captcha)
        {
            if (bg_t.IsBusy)
            {
                throw new Exception("Thread maker is busy!");
            }
            else
            {
                bg_t.RunWorkerAsync(new object[] { board, data, captcha });
            }
        }

        public void SendReplyAsyc(string board, int threadID, PostSenderData data, SolvedCaptcha captcha)
        {
            if (bg_r.IsBusy)
            {
                throw new Exception("Reply sender is busy!");
            }
            else
            {
                bg_r.RunWorkerAsync(new object[] { board, threadID, data, captcha });
            }
        }

        public delegate void SendRequestCompleteEvent(PostSenderResponse psr, SendType t);

        public event SendRequestCompleteEvent SendCompleted;

        public delegate void SendRequestFailedEvent(Exception ex, SendType t);

        public event SendRequestFailedEvent SendRequestFailed;

        public enum SendType { Reply, Thread }

        public void Dispose()
        {
            bg_r.Dispose();
            bg_t.Dispose();
        }

    }

    public class UploadFile
    {
        public UploadFile()
        {
            ContentType = "application/octet-stream";
        }
        public string Name { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public Stream Stream { get; set; }
    }

    public class PostSenderData
    {
        public PostSenderData()
        {
            this.Is4chanPass = false;
        }

        public string Name { get; set; }

        public string Subject { get; set; }

        public string Email { get; set; }

        public string Comment { get; set; }

        /// <summary>
        /// 4chan pass
        /// </summary>
        public bool Is4chanPass { get; set; }

        public string PostPassword { get; set; }

        public string FilePath { get; set; }

        public string FileName { get; set; }
    }

    public class PostSenderResponse
    {
        public PostSenderResponse(string response_data)
        {
            this.Status = get_status(response_data);

            if (this.Status == ResponseStatus.Success)
            {
                try
                {
                    this.ResponseBody = get_new_reply_id(response_data); //should be in this format: [threadID:newReplyID]
                }
                catch (Exception)
                {
                    this.ResponseBody = response_data;
                }
            }
            else
            {
                this.ResponseBody = response_data;
            }
        }

        public enum ResponseStatus
        {
            Success,
            Banned,
            PermaBanned,
            RangeBanned,
            CorruptedImage,
            Muted,
            Flood,
            SpamFilter,
            CaptchaError,
            DuplicateFile,
            Limit,
            InvalidThreadID,
            SubjectRequired,
            NoBoard,
            FileRequired,
            Warned,
            Unknown
        }

        public ResponseStatus Status { get; private set; }
        public string ResponseBody { get; private set; }

        private ResponseStatus get_status(string str)
        {
            if (str.IndexOf("Post successful") != -1)
            {
                return ResponseStatus.Success;
            }

            if (str.IndexOf("Flood") != -1)
            {
                return ResponseStatus.Flood;
            }
            if (str.IndexOf("Duplicate") != -1)
            {
                return ResponseStatus.DuplicateFile;
            }
            if (str.IndexOf("banned") != -1)
            {
                if (str.IndexOf("will not expire") != -1)
                    return ResponseStatus.PermaBanned;
                else
                    return ResponseStatus.Banned;
            }
            if (str.IndexOf("verification") != -1 || str.IndexOf("expired") != -1 || str.IndexOf("You forgot to solve the CAPTCHA") != -1)
            {
                return ResponseStatus.CaptchaError;
            }
            if (str.IndexOf("board doesn") != -1)
            {
                return ResponseStatus.NoBoard;
            }
            if (str.IndexOf("ISP") != -1)
            {
                return ResponseStatus.RangeBanned;
            }
            if (str.IndexOf("Max limit of") != -1)
            {
                return ResponseStatus.Limit;
            }
            if (str.IndexOf("Thread specified") != -1)
            {
                return ResponseStatus.InvalidThreadID;
            }
            if (str.IndexOf("appears corrupt") != -1)
            {
                return ResponseStatus.CorruptedImage;
            }
            if (str.IndexOf("before posting") != -1)
            {
                return ResponseStatus.Muted;
            }
            if (str.IndexOf("require a subject") != -1)
            {
                return ResponseStatus.SubjectRequired;
            }
            if (str.IndexOf("spam") != -1)
            {
                return ResponseStatus.SpamFilter;
            }
            if (str.IndexOf("No file selected") != -1)
            {
                return ResponseStatus.FileRequired;
            }

            if (str.Contains("issued a warning"))
            {
                return ResponseStatus.Warned;
            }

            return ResponseStatus.Unknown;
        }

        private string get_new_reply_id(string response_data)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(response_data);

            HtmlAgilityPack.HtmlNode d = doc.DocumentNode;

            string data = "";

            foreach (HtmlAgilityPack.HtmlNode n1 in d.ChildNodes)
            {
                if (n1.Name == "body")
                {
                    foreach (HtmlAgilityPack.HtmlNode n2 in n1.ChildNodes)
                    {
                        if (n2.Name == "#comment")
                        {
                            data = n2.InnerText;
                        }
                    }
                }
            }

            //"<!-- thread:38545194,no:38545729 -->"

            if (data != "")
            {
                data = data.Replace("<!--", "").Replace("-->", "").Trim();

                string[] dataa = data.Split(',');

                //   int tid = Convert.ToInt32(dataa[0].Split(':')[1]);

                // int replyid = Convert.ToInt32(dataa[1].Split(':')[1]);

                return String.Format("{0}:{1}", dataa[0].Split(':')[1], dataa[1].Split(':')[1]);
            }
            else
            {
                return response_data;
            }
        }

    }
}
