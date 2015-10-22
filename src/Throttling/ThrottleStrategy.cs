namespace Throttling
{
    public class ThrottleStrategy
    {
        public ThrottlePolicy Policy { get; set; }

        public string RouteTemplate { get; set; }
    }
}