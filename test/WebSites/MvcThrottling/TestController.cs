using Microsoft.AspNetCore.Mvc;
using Throttling.Mvc;

namespace MvcThrottling
{
    [Route("{apikey}/test")]
    [Route("{apikey}/test2")]
    public class TestController : Controller
    {
        [HttpGet("RateLimit10PerHour/{value}")]
        [EnableThrottling("10 requests per hour, fixed reset")]
        public string RateLimit10PerHour(int value)
        {
            return "RateLimit10PerHour : " + value;
        }
        
        [HttpGet("RateLimit10PerHour2/{value}")]
        [EnableThrottling("10 requests per hour, fixed reset")]
        public string RateLimit10PerHour2(int value)
        {
            return "RateLimit10PerHour : " + value;
        }

        [HttpGet("Quota160BPerHourByIP/{value}")]
        [EnableThrottling("160 bytes per hour by IP")]
        public string Quota160BPerHourByIP(int value)
        {
            return "0123456789012345";
        }

        [HttpGet("Quota160BPerHourByApiKey/{value}")]
        [EnableThrottling("160 bytes per hour by API key")]
        public string Quota160BPerHourByApiKey(int value)
        {
            return "0123456789012345";
        }

        [HttpGet("Sliding/{value}")]
        [EnableThrottling("10 requests per hour, sliding reset")]
        public string Sliding(int value)
        {
            return "Value : " + value;
        }

        [HttpGet("Sliding2/{value}")]
        [EnableThrottling("10 requests per hour, sliding reset")]
        public string Sliding2(int value)
        {
            return "Value2 : " + value;
        }

        [HttpGet("Fixed")]
        [EnableThrottling("10 requests per hour, fixed reset")]
        public string Fixed()
        {
            return "OK";
        }

        [HttpGet("Bandwidth")]
        [EnableThrottling("Bandwidth")]
        public string Bandwidth()
        {
            return "OK";
        }

        public string NoThrottling()
        {
            return "OK";
        }        
    }
}