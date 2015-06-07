using System;
using Microsoft.AspNet.Mvc;
using Throttling.Mvc;
using Microsoft.AspNet.Mvc.ApiExplorer;

namespace MvcThrottling
{
    public class TestController : Controller
    {        

        public TestController(IApiDescriptionGroupCollectionProvider explorer)
        {

        }

        [HttpGet("Sliding/{value}")]
        [Throttle("10 requests per hour, sliding reset")]
        public string Sliding(int value)
        {
            return "Value : " + value; 
        }

        [HttpGet("Sliding2/{value}")]
        [Throttle("10 requests per hour, sliding reset")]
        public string Sliding2(int value)
        {
            return "Value2 : " + value;
        }

        [HttpGet("Fixed")]
        [Throttle("10 requests per hour, fixed reset")]
        public string Fixed()
        {
            return "OK";
        }

        [HttpGet("Bandwidth")]
        [Throttle("Bandwidth")]
        public string Bandwidth()
        {
            return "OK";
        }

        public string NoThrottling()
        {
            return "OK";
        }

        [HttpGet("MutliThrottling")]
        [AuthenticatedUserThrottle(10, 10)]
        [AuthenticatedUserThrottle(100, 60)]
        [IPThrottle(10, 10)]
        public string MutliThrottling()
        {
            return "OK";
        }
    }
}