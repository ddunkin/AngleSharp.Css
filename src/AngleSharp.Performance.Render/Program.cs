namespace AngleSharp.Performance.Render
{
    using AngleSharp.Css;
    using AngleSharp.Dom;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(String[] args)
        {
            var configuration = Configuration.Default
                .WithDefaultLoader()
                .WithCss()
                .WithRenderDevice(new DefaultRenderDevice { DeviceWidth = 1280, DeviceHeight = 720, ViewPortWidth = 1280, ViewPortHeight = 720 });
            var browsingContext = new BrowsingContext(configuration);

            var document =
                await browsingContext.OpenAsync(
                    "https://en.wikipedia.org/w/index.php?title=Presidency_of_Donald_Trump&oldid=1159225311");
            await document.WaitForReadyAsync();

            var documents = new StandardTests<IDocument>()
                .Include(document);


            var renderers = new List<ITestee<IDocument>>
            {
                new AngleSharpRenderer(),
            };

            var testsuite = new TestSuite<IDocument>(renderers, documents.Tests, new Output())
            {
                NumberOfRepeats = 5,
                NumberOfReRuns = 1,
            };

            testsuite.Run();
        }
    }
}
