using System;

namespace Throttling
{
    public interface ISystemClock
    {
        DateTimeOffset UtcNow { get; }
    }
}