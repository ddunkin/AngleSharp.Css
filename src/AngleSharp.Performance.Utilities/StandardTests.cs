namespace AngleSharp.Performance
{
    using System;
    using System.Collections.Generic;

    public sealed class StandardTests
    {
        readonly List<ITest<String>> _tests;

        public StandardTests()
        {
            _tests = new List<ITest<String>>();
        }

        public List<ITest<String>> Tests => _tests;

        public StandardTests Include(params String[] values)
        {
            foreach (var value in values)
            {
                _tests.Add(new StandardTest<String>(value));
            }

            return this;
        }
    }

    public sealed class StandardTests<TInput>
    {
        readonly List<ITest<TInput>> _tests;

        public StandardTests()
        {
            _tests = new List<ITest<TInput>>();
        }

        public List<ITest<TInput>> Tests => _tests;

        public StandardTests<TInput> Include(params TInput[] values)
        {
            foreach (var value in values)
            {
                _tests.Add(new StandardTest<TInput>(value));
            }

            return this;
        }
    }
}
