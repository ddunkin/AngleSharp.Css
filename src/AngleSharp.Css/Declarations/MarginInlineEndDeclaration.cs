namespace AngleSharp.Css.Declarations
{
    using System;
    using Dom;
    using static ValueConverters;

    public class MarginInlineEndDeclaration
    {
        public static String Name = PropertyNames.MarginInlineEnd;

        public static String[] Shorthands = new[]
        {
            PropertyNames.MarginInline,
        };

        public static IValueConverter Converter = AutoLengthOrPercentConverter;

        public static ICssValue InitialValue = InitialValues.MarginInlineEndDecl;

        public static PropertyFlags Flags = PropertyFlags.Unitless | PropertyFlags.Animatable;
    }
}
