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
        [Throttling("5 requests per 10 seconds, sliding reset")]
        public string Action1(int value)
        {
            return "OK " + value;
        }

        [HttpGet("test/action2")]
        [Throttling("5 requests per 10 seconds, fixed reset")]
        public string Action2()
        {
            return "OK";
        }

        public string NoThrottling()
        {
            return "OK";
        }

        [HttpGet("test/MutliThrottling")]
        [AuthenticatedUserThrottling(10, 10)]
        [AuthenticatedUserThrottling(100, 60)]
        [IPThrottling(10, 10)]
        public string MutliThrottling()
        {
            return "OK";
        }
    }
}