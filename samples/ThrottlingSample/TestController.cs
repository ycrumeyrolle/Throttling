using System;
using Microsoft.AspNet.Mvc;
using Throttling.Mvc;
using Microsoft.AspNet.Mvc.ApiExplorer;

namespace ThrottlingSample
{
    public class TestController : Controller
    {        

        public TestController(IApiDescriptionGroupCollectionProvider explorer)
        {

        }

        [HttpGet("test/action/{value}")]
        [Throttle("5 requests per 10 seconds, sliding reset")]
        public string Action1(int value)
        {
            return "OK " + value;
        }

        [HttpGet("test/action2")]
        [Throttle("5 requests per 10 seconds, fixed reset")]
        public string Action2()
        {
            return "OK";
        }

        public string NoThrottling()
        {
            return "OK";
        }

        [HttpGet("test/MutliThrottling")]
        [AuthenticatedUserThrottle(10, 10)]
        [AuthenticatedUserThrottle(100, 60)]
        [IPThrottle(10, 10)]
        public string MutliThrottling()
        {
            return "OK";
        }
    }
}