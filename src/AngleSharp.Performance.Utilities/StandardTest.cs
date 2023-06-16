namespace AngleSharp.Performance
{
    using System;

    sealed class StandardTest<TInput> : ITest<TInput>
    {
        public StandardTest(TInput value)
            : this(value.ToString(), value)
        {
        }

        public StandardTest(String name, TInput source)
        {
            Name = name;
            Source = source;
        }

        public String Name
        {
            get;
            private set;
        }

        public TInput Source
        {
            get;
            private set;
        }
    }
}
