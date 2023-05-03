#nullable enable
namespace AngleSharp.Css.RenderTree
{
    using AngleSharp.Css.Dom;
    using AngleSharp.Dom;
    using System.Collections.Generic;
    using System.Linq;

    class ElementRenderNode : IRenderElement
    {
        public ElementRenderNode(INode reference, IEnumerable<IRenderNode> children, ICssStyleDeclaration specifiedStyle, ICssStyleDeclaration computedStyle)
        {
            Ref = reference;
            Children = children;
            SpecifiedStyle = specifiedStyle;
            ComputedStyle = computedStyle;
        }

        public INode Ref { get; }

        public IEnumerable<IRenderNode> Children { get; }

        public ICssStyleDeclaration SpecifiedStyle { get; }

        public ICssStyleDeclaration ComputedStyle { get; }
    }
}
