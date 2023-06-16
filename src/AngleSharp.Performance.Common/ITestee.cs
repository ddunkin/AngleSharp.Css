namespace AngleSharp.Performance
{
    using System;

    public interface ITestee<TInput>
    {
        String Name { get; }

        Type Library { get; }

        void Run(TInput argument);
    }
}
