namespace Throttling
{
    public interface IThrottlePolicyBuilder
    {
        ThrottlePolicy Build(ThrottleOptions options);
    }
}