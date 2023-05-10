using AngleSharp.Xml.Parser;

namespace AngleSharp.Css.Tests.Extensions
{
    using AngleSharp.Css.Dom;
    using AngleSharp.Css.RenderTree;
    using AngleSharp.Dom;
    using NUnit.Framework;
    using System.Text;
    using System.Threading.Tasks;
    using static CssConstructionFunctions;

    [TestFixture]
    public class AnalysisWindowTests
    {
        [Test]
        public void GetComputedStyleTrivialInitialScenario()
        {
            var source = "<!doctype html><head><style>p > span { color: blue; } span.bold { font-weight: bold; }</style></head><body><div><p><span class='bold'>Bold text";
            var document = ParseDocument(source);
            var window = document.DefaultView;
            Assert.IsNotNull(document);

            var element = document.QuerySelector("span.bold");
            Assert.IsNotNull(element);

            Assert.AreEqual("span", element.LocalName);
            Assert.AreEqual("bold", element.ClassName);

            var style = window.GetComputedStyle(element);
            Assert.IsNotNull(style);
            Assert.AreEqual(2, style.Length);
        }

        [Test]
        public void GetComputedStyleImportantHigherNoInheritance()
        {
            var source = new StringBuilder("<!doctype html> ");

            var styles = new StringBuilder("<head><style>");
            styles.Append("p {text-align: center;}");
            styles.Append("p > span { color: blue; }");
            styles.Append("p > span { color: red; }");
            styles.Append("span.bold { font-weight: bold !important; }");
            styles.Append("span.bold { font-weight: lighter; }");

            styles.Append("#prioOne { color: black; }");
            styles.Append("div {color: green; }");
            styles.Append("</style></head>");

            var body = new StringBuilder("<body>");
            body.Append("<div><p><span class='bold'>Bold text</span></p></div>");
            body.Append("<div id='prioOne'>prioOne</div>");
            body.Append("</body>");

            source.Append(styles);
            source.Append(body);

            var document = ParseDocument(source.ToString());
            Assert.IsNotNull(document);
            var window = document.DefaultView;

            // checks for element with text bold text
            var element = document.QuerySelector("span.bold");
            Assert.IsNotNull(element);
            Assert.AreEqual("span", element.LocalName);
            Assert.AreEqual("bold", element.ClassName);

            var computedStyle = window.GetComputedStyle(element);
            Assert.AreEqual("rgba(255, 0, 0, 1)", computedStyle.GetColor());
            Assert.AreEqual("bold", computedStyle.GetFontWeight());
            Assert.AreEqual(3, computedStyle.Length);
        }

        [Test]
        public void GetComputedStyleHigherMatchingPrio()
        {
            var source = new StringBuilder("<!doctype html> ");

            var styles = new StringBuilder("<head><style>");
            styles.Append("p {text-align: center;}");
            styles.Append("p > span { color: blue; }");
            styles.Append("p > span { color: red; }");
            styles.Append("span.bold { font-weight: bold !important; }");
            styles.Append("span.bold { font-weight: lighter; }");

            styles.Append("#prioOne { color: black; }");
            styles.Append("div {color: green; }");
            styles.Append("</style></head>");

            var body = new StringBuilder("<body>");
            body.Append("<div><p><span class='bold'>Bold text</span></p></div>");
            body.Append("<div id='prioOne'>prioOne</div>");
            body.Append("</body>");

            source.Append(styles);
            source.Append(body);

            var document = ParseDocument(source.ToString());
            Assert.IsNotNull(document);
            var window = document.DefaultView;

            // checks for element with text prioOne
            var prioOne = document.QuerySelector("#prioOne");
            Assert.IsNotNull(prioOne);
            Assert.AreEqual("div", prioOne.LocalName);
            Assert.AreEqual("prioOne", prioOne.Id);

            var computePrioOneStyle = window.GetComputedStyle(prioOne);
            Assert.AreEqual("rgba(0, 0, 0, 1)", computePrioOneStyle.GetColor());
        }

        [Test]
        public void GetComputedStyleUseAndPreferInlineStyle()
        {
            var source = new StringBuilder("<!doctype html> ");

            var styles = new StringBuilder("<head><style>");
            styles.Append("p > span { color: blue; }");
            styles.Append("</style></head>");

            var body = new StringBuilder("<body>");
            body.Append("<div><p><span style='color: red'>Bold text</span></p></div>");
            body.Append("</body>");

            source.Append(styles);
            source.Append(body);

            var document = ParseDocument(source.ToString());
            Assert.IsNotNull(document);
            var window = document.DefaultView;

            // checks for element with text bold text
            var element = document.QuerySelector("p > span");
            Assert.IsNotNull(element);
            Assert.AreEqual("span", element.LocalName);

            var computedStyle = window.GetComputedStyle(element);
            Assert.AreEqual("rgba(255, 0, 0, 1)", computedStyle.GetColor());
            Assert.AreEqual(1, computedStyle.Length);
        }

        [Test]
        public void GetComputedStyleComplexScenario()
        {
            var sourceCode = @"<!doctype html>
<head>
<style>
p > span { color: blue; }
span.bold { font-weight: bold; }
</style>
<style>
p { font-size: 12pt; }
em { font-style: italic !important; }
.red { margin: 5px; }
</style>
<style>
#text { font-style: normal; margin: 0; }
</style>
</head>
<body>
<div><p><span class=bold>Bold <em style='color: red' class=red id=text>text</em>";

            var document = ParseDocument(sourceCode);
            Assert.IsNotNull(document);
            var window = document.DefaultView;

            var element = document.QuerySelector("#text");
            Assert.IsNotNull(element);

            Assert.AreEqual("em", element.LocalName);
            Assert.AreEqual("red", element.ClassName);
            Assert.IsNotNull(element.GetAttribute("style"));
            Assert.AreEqual("text", element.TextContent);

            var style = window.GetComputedStyle(element);
            Assert.IsNotNull(style);
            Assert.AreEqual(8, style.Length);

            Assert.AreEqual("0", style.GetMargin());
            Assert.AreEqual("rgba(255, 0, 0, 1)", style.GetColor());
            Assert.AreEqual("bold", style.GetFontWeight());
            Assert.AreEqual("italic", style.GetFontStyle());
            Assert.AreEqual("12pt", style.GetFontSize());
        }

        [Test]
        public void GetComputedStylePseudoInitialScenarioSingleColon()
        {
            var sourceCode = "<!doctype html><head><style>p > span::after { color: blue; } span.bold { font-weight: bold; }</style></head><body><div><p><span class='bold'>Bold text";

            var document = ParseDocument(sourceCode);
            Assert.IsNotNull(document);
            var window = document.DefaultView;

            var element = document.QuerySelector("span.bold");
            Assert.IsNotNull(element);

            Assert.AreEqual("span", element.LocalName);
            Assert.AreEqual("bold", element.ClassName);

            var style = window.GetComputedStyle(element, ":after");
            Assert.IsNotNull(style);
            Assert.AreEqual(2, style.Length);
        }

        [Test]
        public void GetComputedStylePseudoInitialScenarioDoubleColon()
        {
            var sourceCode = "<!doctype html><head><style>p > span::after { color: blue; } span.bold { font-weight: bold; }</style></head><body><div><p><span class='bold'>Bold text";

            var document = ParseDocument(sourceCode);
            Assert.IsNotNull(document);
            var window = document.DefaultView;

            var element = document.QuerySelector("span.bold");
            Assert.IsNotNull(element);

            Assert.AreEqual("span", element.LocalName);
            Assert.AreEqual("bold", element.ClassName);

            var style = window.GetComputedStyle(element, "::after");
            Assert.IsNotNull(style);
            Assert.AreEqual(2, style.Length);
        }

        [Test]
        public void GetComputedStyleMixedTrivialAndPseudoScenario()
        {
            var sourceCode = "<!doctype html><head><style>p > span { color: blue; } span.bold { font-weight: bold; } span.bold::before { color: red; content: 'Important!'; }</style></head><body><div><p><span class='bold'>Bold text";

            var document = ParseDocument(sourceCode);
            Assert.IsNotNull(document);
            var window = document.DefaultView;

            var element = document.QuerySelector("span.bold");
            Assert.IsNotNull(element);

            Assert.AreEqual("span", element.LocalName);
            Assert.AreEqual("bold", element.ClassName);

            var styleNormal = window.GetComputedStyle(element);
            Assert.IsNotNull(styleNormal);
            Assert.AreEqual(2, styleNormal.Length);

            var stylePseudo = window.GetComputedStyle(element, ":before");
            Assert.IsNotNull(stylePseudo);
            Assert.AreEqual(3, stylePseudo.Length);
        }

        [Test]
        public void GetCascadedValueOfTextTransformFromGlobalStyle()
        {
            var sourceCode = "<!doctype html><style>body { text-transform: uppercase }</style><div><p><span>Bold text";

            var document = ParseDocument(sourceCode);
            Assert.IsNotNull(document);
            var window = document.DefaultView;

            var element = document.QuerySelector("span");
            Assert.IsNotNull(element);

            var styleNormal = window.GetComputedStyle(element);
            Assert.IsNotNull(styleNormal);
            Assert.AreEqual("uppercase", styleNormal.GetTextTransform());
        }

        [Test]
        public void GetCascadedValueOfTextTransformFromElementStyle()
        {
            var sourceCode = "<!doctype html><div style=\"text-transform: uppercase\"><p><span>Bold text";

            var document = ParseDocument(sourceCode);
            Assert.IsNotNull(document);
            var window = document.DefaultView;

            var element = document.QuerySelector("span");
            Assert.IsNotNull(element);

            var styleNormal = window.GetComputedStyle(element);
            Assert.IsNotNull(styleNormal);
            Assert.AreEqual("uppercase", styleNormal.GetTextTransform());
        }

        [Test]
        public async Task NullSelectorStillWorks_Issue52()
        {
            var sheet = ParseStyleSheet("a {}");
            var document = await sheet.Context.OpenAsync(res => res.Content("<body></body>"));
            sheet.Add(new CssStyleRule(sheet));
            var sc = new StyleCollection(new[] { sheet }, new DefaultRenderDevice());
            var decl = sc.ComputeCascadedStyle(document.Body);
            Assert.IsNotNull(decl);
        }

        [Test]
        public void RenderComplexScenario()
        {
            var sourceCode = @"<!doctype html>
<head>
<style>
p > span { color: blue; }
span.bold { font-weight: bold; }
</style>
<style>
p { font-size: 12pt; }
em { font-style: italic !important; }
.red { margin: 5px; }
.big { font-size: larger; }
.huge { font-size: 5em; }
</style>
<style>
#text { font-style: normal; margin: 0; }
#box { height: 1em; width: 1em; background-color: red; }
</style>
</head>
<body>
<div><p><span class=bold>Bold <em style='color: red' class=red id=text>text</em></span></p></div>
<div class=big id=big1>big1<div id=big2>big2</div></div>
<div class=big><div class=big id=big3>big3</div></div>
<div class=huge><div id=box></div><span id=text2>text</span></div>";

            var document = ParseDocument(sourceCode);
            Assert.IsNotNull(document);
            var window = document.DefaultView;

            var renderRoot = window.Render();
            Assert.IsNotNull(renderRoot);

            var textElement = renderRoot.QuerySelector("#text");
            Assert.IsNotNull(textElement);

            var textElementComputedStyle = textElement.ComputedStyle;

            Assert.AreEqual(8, textElementComputedStyle.Length);

            Assert.AreEqual("0", textElementComputedStyle.GetMargin());
            Assert.AreEqual("rgba(255, 0, 0, 1)", textElementComputedStyle.GetColor());
            Assert.AreEqual("bold", textElementComputedStyle.GetFontWeight());
            Assert.AreEqual("italic", textElementComputedStyle.GetFontStyle());
            Assert.AreEqual("16px", textElementComputedStyle.GetFontSize());

            Assert.AreEqual("larger", renderRoot.QuerySelector("#big1").SpecifiedStyle.GetFontSize());
            Assert.AreEqual("19.2px", renderRoot.QuerySelector("#big1").ComputedStyle.GetFontSize());
            Assert.AreEqual("", renderRoot.QuerySelector("#big2").SpecifiedStyle.GetFontSize());
            Assert.AreEqual("19.2px", renderRoot.QuerySelector("#big2").ComputedStyle.GetFontSize());
            Assert.AreEqual("larger", renderRoot.QuerySelector("#big3").SpecifiedStyle.GetFontSize());
            Assert.AreEqual("23.04px", renderRoot.QuerySelector("#big3").ComputedStyle.GetFontSize());

            Assert.AreEqual("1em", renderRoot.QuerySelector("#box").SpecifiedStyle.GetWidth());
            Assert.AreEqual("80px", renderRoot.QuerySelector("#box").ComputedStyle.GetWidth());
            Assert.AreEqual("1em", renderRoot.QuerySelector("#box").SpecifiedStyle.GetHeight());
            Assert.AreEqual("80px", renderRoot.QuerySelector("#box").ComputedStyle.GetHeight());
        }

        [Test]
        public void RenderLineHeight()
        {
            var sourceCode = @"<!doctype html>
<head>
<style>
.xl { font-size: x-large; }
</style>
</head>
<body>
<div><span id=text1>text</span> <span class=xl id=text2>text</span></div>
<div style='line-height: 1.5'><span id=text3>text</span> <span class=xl id=text4>text</span></div>
<div style='line-height: 1.5em'><span class=xl id=text5>text</span></div>
<div style='line-height: normal'><span id=text6>text</span></div>
<div style='font-size: 110%; line-height: 125%'><span id=text7>text</span></div>";

            var document = ParseDocument(sourceCode);
            Assert.IsNotNull(document);
            var window = document.DefaultView;

            var renderRoot = window.Render();
            Assert.IsNotNull(renderRoot);

            Assert.Multiple(() =>
            {
                Assert.AreEqual("", renderRoot.QuerySelector("#text1").SpecifiedStyle.GetLineHeight());
                Assert.AreEqual("", renderRoot.QuerySelector("#text1").ComputedStyle.GetLineHeight());

                Assert.AreEqual("", renderRoot.QuerySelector("#text2").SpecifiedStyle.GetLineHeight());
                Assert.AreEqual("", renderRoot.QuerySelector("#text2").ComputedStyle.GetLineHeight());

                Assert.AreEqual("", renderRoot.QuerySelector("#text3").SpecifiedStyle.GetLineHeight());
                Assert.AreEqual("24px", renderRoot.QuerySelector("#text3").ComputedStyle.GetLineHeight());

                Assert.AreEqual("", renderRoot.QuerySelector("#text4").SpecifiedStyle.GetLineHeight());
                Assert.AreEqual("36px", renderRoot.QuerySelector("#text4").ComputedStyle.GetLineHeight());

                Assert.AreEqual("24px", renderRoot.QuerySelector("#text5").ComputedStyle.GetLineHeight());

                Assert.AreEqual("normal", renderRoot.QuerySelector("#text6").ComputedStyle.GetLineHeight());

                Assert.AreEqual("22px", renderRoot.QuerySelector("#text7").ComputedStyle.GetLineHeight());
            });
        }

        [Test]
        public void RenderFontSize()
        {
            var sourceCode = @"<!doctype html>
<head>
<style>
.larger { font-size: 150%; }
.xl { font-size: x-large; }
</style>
</head>
<body>
<div><span id=text1>text</span> <span class=xl id=text2>text</span></div>
<div class=larger>
<span id=text3>text</span>
<span class=larger id=text4>text</span>
<span style='font-size: 1.5rem' id=text5>text</span>
<span style='font-size: inherit' id=text6>text</span>
<span style='font-size: unset' id=text7>text</span>
<span style='font-size: initial' id=text8>text</span>
</div>";

            var document = ParseDocument(sourceCode);
            Assert.IsNotNull(document);
            var window = document.DefaultView;

            var renderRoot = window.Render();
            Assert.IsNotNull(renderRoot);

            Assert.Multiple(() =>
            {
                Assert.AreEqual("", renderRoot.QuerySelector("#text1").ComputedStyle.GetFontSize());
                Assert.AreEqual("24px", renderRoot.QuerySelector("#text2").ComputedStyle.GetFontSize());
                Assert.AreEqual("24px", renderRoot.QuerySelector("#text3").ComputedStyle.GetFontSize());
                Assert.AreEqual("36px", renderRoot.QuerySelector("#text4").ComputedStyle.GetFontSize());
                Assert.AreEqual("24px", renderRoot.QuerySelector("#text5").ComputedStyle.GetFontSize());
                Assert.AreEqual("24px", renderRoot.QuerySelector("#text6").ComputedStyle.GetFontSize());
                Assert.AreEqual("24px", renderRoot.QuerySelector("#text7").ComputedStyle.GetFontSize());
                Assert.AreEqual("16px", renderRoot.QuerySelector("#text8").ComputedStyle.GetFontSize());
            });
        }

        [Test]
        public void ComputeXhtmlElementStyle()
        {
            var sourceCode = "<!DOCTYPE html><html xmlns=\"http://www.w3.org/1999/xhtml\"><body><div style=\"color: red\">red text</div></body></html>";

            var parser = new XmlParser(new(), BrowsingContext.New(Configuration.Default.WithCss().WithRenderDevice()));
            var document = parser.ParseDocument(sourceCode);
            Assert.IsNotNull(document);
            var window = document.DefaultView;

            var element = document.QuerySelector("div");
            Assert.IsNotNull(element);

            var style = window.GetComputedStyle(element);
            Assert.IsNotNull(style);
            Assert.AreEqual("rgba(255, 0, 0, 1)", style.GetColor());
        }
    }
}
