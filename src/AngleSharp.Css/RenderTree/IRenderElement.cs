#nullable enable

namespace AngleSharp.Css.RenderTree
{
    using AngleSharp.Css.Dom;
    using AngleSharp.Dom;

    public interface IRenderElement : IRenderNode
    {
        IElement Ref { get; }
        ICssStyleDeclaration SpecifiedStyle { get; }
        ICssStyleDeclaration ComputedStyle { get; }
    }
}