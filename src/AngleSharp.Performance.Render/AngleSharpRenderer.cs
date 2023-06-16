namespace AngleSharp.Performance.Render
{
    using AngleSharp;
    using AngleSharp.Dom;
    using System;

    class AngleSharpRenderer : ITestee<IDocument>
    {
        public String Name => "AngleSharp";

        public Type Library => typeof(BrowsingContext);

        public void Run(IDocument document)
        {
            var window = document.DefaultView;
            window.Render();
        }
    }
}
