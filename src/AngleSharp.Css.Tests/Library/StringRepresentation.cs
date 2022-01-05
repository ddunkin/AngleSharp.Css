namespace AngleSharp.Css.Tests.Library
{
    using AngleSharp.Css.Parser;
    using AngleSharp.Css.Values;
    using NUnit.Framework;
    using System.IO;

    [TestFixture]
    public class StringRepresentationTests
    {
        [Test]
        public void PrettyStyleFormatterStringifyShouldWork_Issue41()
        {
            var text = "@media (min-width: 800px) { .ad_column { width: 728px; height: 90px } }";
            var parser = new CssParser();
            var document = parser.ParseStyleSheet(text);

            using (var stringWriter = new StringWriter())
            {
                document.ToCss(stringWriter, new PrettyStyleFormatter());
                Assert.AreEqual("@media (min-width: 800px) { \n\t.ad_column {\n\t\twidth: 728px;\n\t\theight: 90px;\n\t}\n}", stringWriter.ToString());
            }
        }

        [Test]
        public void SimpleColorWorksWithHexOutput_Issue96()
        {
            var color = new Color(65, 12, 48);
            Color.UseHex = true;
            var text = color.CssText;
            Color.UseHex = false;
            Assert.AreEqual("#410C30", text);
        }

        [Test]
        public void TransparentColorDoesNotWorkWithHexOutput_Issue96()
        {
            var color = new Color(65, 12, 48, 10);
            Color.UseHex = true;
            var text = color.CssText;
            Color.UseHex = false;
            Assert.AreEqual("rgba(65, 12, 48, 0.04)", text);
        }
    }
}
