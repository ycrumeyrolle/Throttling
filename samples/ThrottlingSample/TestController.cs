using System;
using Microsoft.AspNetCore.Mvc;
using Throttling.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace ThrottlingSample
{
    [EnableThrottling("Empty")]
    public class TestController : Controller
    {        
        public TestController(IApiDescriptionGroupCollectionProvider explorer)
        {
        }

        [HttpGet("test/action/{value}")]
        [EnableThrottling("5 requests per 10 seconds, sliding reset")]
        public string RateLimit10PerHour(int value)
        {
            return "OK " + value;
        }

        [HttpGet("test/RateLimit10PerHour2")]
        [EnableThrottling("5 requests per 10 seconds, fixed reset")]
        public string RateLimit10PerHour2()
        {
            return "OK";
        }

        [DisableThrottling]
        public string NoThrottling()
        {
            return "OK";
        }

        [HttpGet("test/middleware")]
        public string MiddlewareThrottling()
        {
            return "OK";
        }
    }
}