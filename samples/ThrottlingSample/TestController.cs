using System;
using Microsoft.AspNet.Mvc;
using Throttling.Mvc;
using Microsoft.AspNet.Mvc.ApiExplorer;

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
        public string Action1(int value)
        {
            return "OK " + value;
        }

        [HttpGet("test/action2")]
        [EnableThrottling("5 requests per 10 seconds, fixed reset")]
        public string Action2()
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