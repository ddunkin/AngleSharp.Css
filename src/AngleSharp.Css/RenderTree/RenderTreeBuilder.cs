#nullable enable
namespace AngleSharp.Css.RenderTree
{
    using AngleSharp.Css.Dom;
    using AngleSharp.Css.Values;
    using AngleSharp.Dom;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class RenderTreeBuilder
    {
        private readonly IBrowsingContext _context;
        private readonly IWindow _window;
        private readonly IEnumerable<ICssStyleSheet> _defaultSheets;
        private readonly IRenderDevice _device;

        public RenderTreeBuilder(IWindow window, IRenderDevice? device = null)
        {
            var ctx = window.Document.Context;
            var defaultStyleSheetProvider = ctx.GetServices<ICssDefaultStyleSheetProvider>();
            _context = ctx;
            _device = device ?? ctx.GetService<IRenderDevice>() ?? throw new ArgumentNullException(nameof(device));
            _defaultSheets = defaultStyleSheetProvider.Select(m => m.Default).Where(m => m != null);
            _window = window;
        }

        public IRenderNode RenderDocument()
        {
            var document = _window.Document;
            var currentSheets = document.GetStyleSheets().OfType<ICssStyleSheet>();
            var stylesheets = _defaultSheets.Concat(currentSheets).ToList();
            var collection = new StyleCollection(stylesheets, _device);
            var rootStyle = collection.ComputeCascadedStyle(document.DocumentElement);
            var rootFontValue = rootStyle.GetProperty(PropertyNames.FontSize)?.RawValue;
            var rootFontSize = rootFontValue switch
            {
                Length length => length.Value,
                Constant<Length> constant => constant.Value.Value,
                _ => 16
            };
            return RenderElement(rootFontSize, document.DocumentElement, collection);
        }

        private ElementRenderNode RenderElement(double rootFontSize, IElement reference, StyleCollection collection, ICssStyleDeclaration? parent = null)
        {
            var style = collection.ComputeCascadedStyle(reference);
            var computedStyle = Compute(rootFontSize, style, parent);
            if (parent != null)
            {
                computedStyle.UpdateDeclarations(parent);
            }
            var children = new List<IRenderNode>();

            foreach (var child in reference.ChildNodes)
            {
                if (child is IText text)
                {
                    children.Add(RenderText(text));
                }
                else if (child is IElement element)
                {
                    children.Add(RenderElement(rootFontSize, element, collection, computedStyle));
                }
            }

            // compute unitless line-height after rendering children
            if (computedStyle.GetProperty(PropertyNames.LineHeight).RawValue is Length { Type: Length.Unit.None } unitlessLineHeight)
            {
                var fontSize = computedStyle.GetProperty(PropertyNames.FontSize).RawValue is Length { Type: Length.Unit.Px } fontSizeLength ? fontSizeLength.Value : rootFontSize;
                var pixelValue = unitlessLineHeight.Value * fontSize;
                var computedLineHeight = new Length(pixelValue, Length.Unit.Px);

                // create a new property because SetProperty would change the parent value
                var lineHeightProperty = _context.CreateProperty(PropertyNames.LineHeight);
                lineHeightProperty.RawValue = computedLineHeight;
                computedStyle.SetDeclarations(new[] { lineHeightProperty });
            }

            var node = new ElementRenderNode(reference, children, style, computedStyle);
            foreach (var child in children)
            {
                if (child is ElementRenderNode elementChild)
                    elementChild.Parent = node;
                else if (child is TextRenderNode textChild)
                    textChild.Parent = node;
                else
                    throw new InvalidOperationException();
            }
            return node;
        }

        private IRenderNode RenderText(IText text) => new TextRenderNode(text);

        private CssStyleDeclaration Compute(Double rootFontSize, ICssStyleDeclaration style, ICssStyleDeclaration? parentStyle)
        {
            var computedStyle = new CssStyleDeclaration(_context);
            var parentFontSize = ((Length?) parentStyle?.GetProperty(PropertyNames.FontSize)?.RawValue)?.ToPixel(_device) ?? rootFontSize;
            var fontSize = parentFontSize;
            // compute font-size first because other properties may depend on it
            if (style.GetProperty(PropertyNames.FontSize) is { RawValue: not null } fontSizeProperty)
            {
                fontSize = GetFontSizeInPixels(fontSizeProperty.RawValue);
            }
            var declarations = style.Select(property =>
            {
                var name = property.Name;
                var value = property.RawValue;
                if (name == PropertyNames.FontSize)
                {
                    // font-size was already computed
                    value = new Length(fontSize, Length.Unit.Px);
                }
                else if (value is Length { IsAbsolute: true, Type: not Length.Unit.Px } absoluteLength)
                {
                    value = new Length(absoluteLength.ToPixel(_device), Length.Unit.Px);
                }
                else if (value is Length { Type: Length.Unit.Percent } percentLength)
                {
                    if (name == PropertyNames.VerticalAlign || name == PropertyNames.LineHeight)
                    {
                        var pixelValue = percentLength.Value / 100 * fontSize;
                        value = new Length(pixelValue, Length.Unit.Px);
                    }
                    else
                    {
                        // TODO: compute for other properties that should be absolute
                    }
                }
                else if (value is Length { IsRelative: true, Type: not Length.Unit.None } relativeLength)
                {
                    var pixelValue = relativeLength.Type switch
                    {
                        Length.Unit.Em => relativeLength.Value * fontSize,
                        Length.Unit.Rem => relativeLength.Value * rootFontSize,
                        _ => relativeLength.ToPixel(_device),
                    };
                    value = new Length(pixelValue, Length.Unit.Px);
                }

                return new CssProperty(name, property.Converter, property.Flags, value, property.IsImportant);
            });
            computedStyle.SetDeclarations(declarations);

            return computedStyle;

            Double GetFontSizeInPixels(ICssValue value) =>
			    value switch
			    {
				    Constant<Length> constLength when constLength.CssText == CssKeywords.XxSmall => 9D/16 * rootFontSize,
				    Constant<Length> constLength when constLength.CssText == CssKeywords.XSmall => 10D/16 * rootFontSize,
				    Constant<Length> constLength when constLength.CssText == CssKeywords.Small => 13D/16 * rootFontSize,
				    Constant<Length> constLength when constLength.CssText == CssKeywords.Medium => 16D/16 * rootFontSize,
				    Constant<Length> constLength when constLength.CssText == CssKeywords.Large => 18D/16 * rootFontSize,
				    Constant<Length> constLength when constLength.CssText == CssKeywords.XLarge => 24D/16 * rootFontSize,
				    Constant<Length> constLength when constLength.CssText == CssKeywords.XxLarge => 32D/16 * rootFontSize,
				    Constant<Length> constLength when constLength.CssText == CssKeywords.XxxLarge => 48D/16 * rootFontSize,
				    Constant<Length> constLength when constLength.CssText == CssKeywords.Smaller => ComputeRelativeFontSize(constLength),
				    Constant<Length> constLength when constLength.CssText == CssKeywords.Larger => ComputeRelativeFontSize(constLength),
				    Length { Type: Length.Unit.Px } length => length.Value,
				    Length { IsAbsolute: true } length => length.ToPixel(_device),
				    Length { Type: Length.Unit.Vh or Length.Unit.Vw or Length.Unit.Vmax or Length.Unit.Vmin } length => length.ToPixel(_device),
				    Length { IsRelative: true } length => ComputeRelativeFontSize(length),
                    ICssSpecialValue specialValue when specialValue.CssText == CssKeywords.Inherit || specialValue.CssText == CssKeywords.Unset => parentFontSize,
                    ICssSpecialValue specialValue when specialValue.CssText == CssKeywords.Initial => rootFontSize,
				    _ => throw new InvalidOperationException("Font size must be a length"),
			    };

		    Double ComputeRelativeFontSize(ICssValue value)
            {
                var ancestorValue = parentStyle?.GetProperty(PropertyNames.FontSize)?.RawValue;
                var ancestorPixels = ancestorValue switch
			    {
				    Length { IsAbsolute: true } ancestorLength => ancestorLength.ToPixel(_device),
                    null => rootFontSize,
                    _ => throw new InvalidOperationException(),
			    };
			    // set a minimum size of 9px for relative sizes
			    return Math.Max(9, value switch
			    {
				    Constant<Length> constLength when constLength.CssText == CssKeywords.Smaller => ancestorPixels / 1.2,
				    Constant<Length> constLength when constLength.CssText == CssKeywords.Larger => ancestorPixels * 1.2,
				    Length { Type: Length.Unit.Rem } length => length.Value * rootFontSize,
				    Length { Type: Length.Unit.Em } length => length.Value * ancestorPixels,
				    Length { Type: Length.Unit.Percent } length => length.Value / 100 * ancestorPixels,
				    _ => throw new InvalidOperationException(),
			    });
		    }
        }
    }
}
