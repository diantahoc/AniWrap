AniWrap
=======

.NET API Wrapper for the popular anime site.

Writing features:
- Post sending.
- Post reporting.
- Post removal.

Reading features:
- Catalog fetching.
- Thread fetching
- Pages (index) fetching.

Please note that the writing features are not official, so thay migt not work in case someting in the site is changed.

Read API
--------

- Quick guide

```csharp
        static void Main(string[] args)
        {
            AniWrap.AniWrap api_wrapper = new AniWrap.AniWrap();
            
            //Download /g/ catalog
            api_wrapper.GetCatalog("g");

            //Download /a/ first page
            api_wrapper.GetPage("a", 0);

            //Download thread number 1234 from /gd/
            api_wrapper.GetThreadData("gd", 1234);
        }
```

- Since AniWrap cache API requests, you can specify your own cache directory in the constructor:

```csharp
AniWrap.AniWrap api_wrapper = new AniWrap.AniWrap(@"C:\");
```

- An example program that print thread data to a console

```csharp
        static void Main(string[] args)
        {
            Console.Title = "AniWrap test";

            AniWrap.AniWrap api_wrapper = new AniWrap.AniWrap();

            AniWrap.DataTypes.ThreadContainer tc = api_wrapper.GetThreadData("g", 39282313);

            print_post(tc.Instance); // print op post

            foreach (AniWrap.DataTypes.Reply reply in tc.Replies) 
            {
                print_post(reply);
            }

            Console.Read();
        }

        private static void print_post(AniWrap.DataTypes.GenericPost gp) 
        {
            Console.WriteLine(String.Format("Name: {0} , Time {1}, PostNo. {2}", gp.Name, gp.Time.ToShortDateString(), gp.ID));
            Console.WriteLine("----------------------");

            Console.WriteLine(gp.CommentText);

            Console.WriteLine();
        }
```

Output:

![Example demo](https://github.com/diantahoc/AniWrap/raw/master/misc/api_test.png "Example demo")


Write API
----------

- Post reporting

AniWrap support post reporting.

The following is a sample example on how to report a post:

```csharp
        static void Main(string[] args)
        {
            Console.Title = "AniWrap report test";

            AniWrap.AniWrap api_wrapper = new AniWrap.AniWrap();

            AniWrap.CaptchaChallenge cc = api_wrapper.GetCaptchaChallenge();

            string desktop_path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            cc.CaptchaImage.Save(Path.Combine(desktop_path, "challenge.jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);

            Console.WriteLine("Please solve the challenge:");

            string resp = Console.ReadLine();

            AniWrap.SolvedCaptcha sv = new AniWrap.SolvedCaptcha(cc.ChallengeField, resp);

            AniWrap.ReportStatus rs = api_wrapper.ReportPost("g", 39305145, AniWrap.AniWrap.ReportReason.CommercialSpam, sv);

            Console.WriteLine(rs.ToString());

            Console.ReadLine();
        }
```

Output:

![Post report demo](https://github.com/diantahoc/AniWrap/raw/master/misc/report_test.png "Post report demo")


- Post sending

AniWrap support sending replies and making new threads.

The following is an example showing how to reply to a thread.

```csharp
        static void Main(string[] args)
        {
            Console.Title = "AniWrap reply test";

            AniWrap.AniWrap api_wrapper = new AniWrap.AniWrap();

            AniWrap.PostSender post_sender = new AniWrap.PostSender();

            AniWrap.CaptchaChallenge cc = api_wrapper.GetCaptchaChallenge();

            string desktop_path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            cc.CaptchaImage.Save(Path.Combine(desktop_path, "challenge.jpg"), System.Drawing.Imaging.ImageFormat.Jpeg);

            Console.WriteLine("Please solve the challenge:");

            string captcha_resp = Console.ReadLine();

            AniWrap.SolvedCaptcha sv = new AniWrap.SolvedCaptcha(cc.ChallengeField, captcha_resp);

            AniWrap.PostSenderData data = new AniWrap.PostSenderData()
            {
                Name = "AniWrap Library",

                Comment = "This is test",

                PostPassword = "123456"
            };

            AniWrap.PostSenderResponse psr = post_sender.SendReply("g", 39306716, data, sv);

            Console.WriteLine(String.Format("Response status: {0}", psr.Status.ToString()));

            Console.ReadLine();
        }
```

Output:

![Reply demo](https://github.com/diantahoc/AniWrap/raw/master/misc/reply_test.png "Reply demo")


- Post removal

To remove a post, do the following:

```csharp
//Board, Thread ID, Post ID, Password, File Only
AniWrap.AniWrap.DeletePost("g", 39800493, 39802345, "123456", false);
```

