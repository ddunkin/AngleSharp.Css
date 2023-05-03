namespace AngleSharp.Css.RenderTree
{
    using AngleSharp.Css.Dom;

    public interface IRenderElement : IRenderNode
    {
        ICssStyleDeclaration SpecifiedStyle { get; set; }
        ICssStyleDeclaration ComputedStyle { get; set; }
    }
}