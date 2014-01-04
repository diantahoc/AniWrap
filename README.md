AniWrap
=======

.NET API Wrapper for the popular anime site.


Example usage
------

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

- Since AniWrap cache API requests, you can specify your own cache directory in the constructor

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

![Example demo](https://github.com/diantahoc/AniWrap/raw/master/test.png "Example demo")

