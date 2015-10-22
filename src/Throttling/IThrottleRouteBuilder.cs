namespace Throttling
{
    public interface IThrottleRouteBuilder
    {
        void Add(ThrottleRoute route);
        IThrottleRouter Build();
    }
}