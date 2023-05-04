namespace AngleSharp.Css.Values
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Represents an number value.
    /// </summary>
    public struct Number : IEquatable<Number>, IComparable<Number>, ICssPrimitiveValue
    {

        #region Fields

        private readonly Double _value;

        #endregion

        #region ctor

        /// <summary>
        /// Creates a new number value.
        /// </summary>
        /// <param name="value">The value of the number.</param>
        public Number(Double value)
        {
            _value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the CSS text representation.
        /// </summary>
        public String CssText => _value.CssStringify();

        /// <summary>
        /// Gets the value of the number.
        /// </summary>
        public Double Value => _value;

        #endregion

        #region Comparison

        /// <summary>
        /// Compares the magnitude of two numbers.
        /// </summary>
        public static Boolean operator >=(Number a, Number b)
        {
            var result = a.CompareTo(b);
            return result == 0 || result == 1;
        }

        /// <summary>
        /// Compares the magnitude of two numbers.
        /// </summary>
        public static Boolean operator >(Number a, Number b) => a.CompareTo(b) == 1;

        /// <summary>
        /// Compares the magnitude of two numbers.
        /// </summary>
        public static Boolean operator <=(Number a, Number b)
        {
            var result = a.CompareTo(b);
            return result == 0 || result == -1;
        }

        /// <summary>
        /// Compares the magnitude of two numbers.
        /// </summary>
        public static Boolean operator <(Number a, Number b) => a.CompareTo(b) == -1;

        /// <summary>
        /// Compares the current length against the given one.
        /// </summary>
        /// <param name="other">The length to compare to.</param>
        /// <returns>The result of the comparison.</returns>
        public Int32 CompareTo(Number other) => _value.CompareTo(other._value);

        #endregion

        #region Methods

        /// <summary>
        /// Tries to convert the given string to a Number.
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <param name="result">The reference to the result.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public static Boolean TryParse(String s, out Number result)
        {
            if (Double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            {
                result = new Number(value);
                return true;
            }

            result = default;
            return false;
        }

        #endregion

        #region Equality

        /// <summary>
        /// Checks the equality of the two given numbers.
        /// </summary>
        /// <param name="a">The left number.</param>
        /// <param name="b">The right number.</param>
        /// <returns>True if both numbers are equal, otherwise false.</returns>
        public static Boolean operator ==(Number a, Number b) => a.Equals(b);

        /// <summary>
        /// Checks the inequality of the two given numbers.
        /// </summary>
        /// <param name="a">The left number.</param>
        /// <param name="b">The right number.</param>
        /// <returns>True if both numbers are not equal, otherwise false.</returns>
        public static Boolean operator !=(Number a, Number b) => !a.Equals(b);

        /// <summary>
        /// Checks if both numbers are actually equal.
        /// </summary>
        /// <param name="other">The other number to compare to.</param>
        /// <returns>True if both numbers are equal, otherwise false.</returns>
        public Boolean Equals(Number other) =>
            (_value == other._value || (Double.IsNaN(_value) && Double.IsNaN(other._value))) &&
            (_value == 0.0);

        /// <summary>
        /// Tests if another object is equal to this object.
        /// </summary>
        /// <param name="obj">The object to test with.</param>
        /// <returns>True if the two objects are equal, otherwise false.</returns>
        public override Boolean Equals(Object obj)
        {
            var other = obj as Number?;

            if (other != null)
            {
                return Equals(other.Value);
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code that defines the current number.
        /// </summary>
        /// <returns>The integer value of the hashcode.</returns>
        public override Int32 GetHashCode() => _value.GetHashCode();

        #endregion
    }
}
