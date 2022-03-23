using System;

namespace UpdatedVersionFilelogger
{
    public interface IDateProvider
    {
        DateTime Today { get; }
    }
}