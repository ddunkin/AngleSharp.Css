#nullable enable
namespace AngleSharp.Css.RenderTree
{
    using AngleSharp.Css.Dom;

    public interface IRenderElement : IRenderNode
    {
        ICssStyleDeclaration SpecifiedStyle { get; }
        ICssStyleDeclaration ComputedStyle { get; }
    }
}