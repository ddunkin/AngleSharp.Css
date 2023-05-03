#nullable enable
namespace AngleSharp.Css.RenderTree
{
    using AngleSharp.Dom;
    using System.Collections.Generic;
    using System.Linq;

    class TextRenderNode : IRenderNode
    {
        public TextRenderNode(INode reference)
        {
            Ref = reference;
        }

        public INode Ref { get; }

        public IEnumerable<IRenderNode> Children => Enumerable.Empty<IRenderNode>();
    }
}
