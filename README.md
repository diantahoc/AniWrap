AniWrap
=======

.NET API Wrapper for the popular anime site.

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

            Console.WriteLine(gp.Comment);

            Console.WriteLine();
        }
```

<<<<<<< HEAD
![Example demo](https://github.com/diantahoc/AniWrap/raw/master/test.png "Example demo")

=======
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

            AniWrap.AniWrap.ReportStatus rs = api_wrapper.ReportPost("g", 39305145, AniWrap.AniWrap.ReportReason.CommercialSpam, sv);

            Console.WriteLine(rs.ToString());

            Console.ReadLine();
        }
```

Output:

![Post report demo](https://github.com/diantahoc/AniWrap/raw/master/misc/report_test.png "Post report demo")
>>>>>>> Added post reporting support.
