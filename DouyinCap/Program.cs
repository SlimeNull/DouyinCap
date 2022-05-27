using CommandLine;
using Esprima.Ast;
using PuppeteerSharp;
using RestSharp;

namespace DouyinCap
{
    class Program
    {
        static int signIndex = 0;
        static string[] signs = { "-", "\\", "|", "/" };
        static void Fetcher_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            if (signIndex >= signs.Length)
                signIndex = 0;

            Console.WriteLine($"Browser downloading... [{e.ProgressPercentage}%] {signs[signIndex]}");
            signIndex++;
        }
        static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<StartupOptions>(args)
                .WithParsed(MainAction);
        }

        static void MainAction(StartupOptions options)
        {
            async Task MainActionAsync()
            {
                using var fetcher = new BrowserFetcher();
                fetcher.DownloadProgressChanged += Fetcher_DownloadProgressChanged;
                await fetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

                Console.WriteLine($"Starting browser...");
                using Browser browser = await Puppeteer.LaunchAsync(new LaunchOptions()
                {
                    Headless = !options.ShowBrowser,
                });

                AppDomain.CurrentDomain.ProcessExit += (s, e) =>
                {
                    browser?.Dispose();
                };

                RestClient? client = null;
                long roomId = options.RoomId;
                string liveHomeAddr = $"https://live.douyin.com/";
                string liveRoomAddr = $"https://live.douyin.com/{roomId}";

                if (options.PostAddress != null)
                    client = new RestClient(options.PostAddress);

                Console.WriteLine($"Loading page...");
                using Page page = (await browser.PagesAsync())[0];
                //await page.SetRequestInterceptionAsync(true);
                //page.Request += Page_Request;

                await page.GoToAsync(liveHomeAddr);
                await page.GoToAsync(liveRoomAddr);
                string? lastDataId = null;

                while (true)
                {
                    string query = QueryHelper.AllChatMessagesAfter(lastDataId);
                    ElementHandle[]? items = await page.QuerySelectorAllAsync(query);

                    if (items.Length == 0)
                    {
                        string msgQuery = QueryHelper.AllChatMessages();
                        ElementHandle[]? msgs = await page.QuerySelectorAllAsync(msgQuery);

                        if (msgs.Length == 0)
                            lastDataId = null;

                        continue;
                    }

                    foreach (var item in items)
                    {
                        var nameNode = await item.QuerySelectorAsync(".tfObciRM");
                        var valueNode = await item.QuerySelectorAsync(".Wz8LGswb");

                        if (nameNode != null && valueNode != null)
                        {
                            string name = await nameNode.GetInnerTextAsync();
                            string value = await valueNode.GetInnerTextAsync();

                            name = name.TrimEnd(':', '：');

                            Console.WriteLine($"{name}: {value}");

                            if (client != null)
                            {
                                var request = new RestRequest()
                                    .AddJsonBody(new
                                    {
                                        Name = name,
                                        Value = value,
                                    });
                                client.Post(request);
                            }
                        }
                    }

                    if (items.Length > 0)
                        lastDataId = await items[^1].GetAttributeAsync("data-id");
                }
            }

            MainActionAsync().Wait();
        }

        class StartupOptions
        {
            [Option("show-browser", Default = false, HelpText = "Show browser window.")]
            public bool ShowBrowser { get; set; }


            [Option("post-addr", Default = null, HelpText = "Specify a http server to post chat message data.")]
            public string? PostAddress { get; set; }


            [Value(0, MetaName = "room-id", HelpText = "Live room Id", Required = true)]
            public long RoomId { get; set; }
        }
    }
}