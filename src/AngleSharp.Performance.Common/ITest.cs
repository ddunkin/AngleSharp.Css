namespace AngleSharp.Performance
{
    using System;

    public interface ITest<TInput>
    {
        String Name { get; }

        TInput Source { get; }
    }
}
