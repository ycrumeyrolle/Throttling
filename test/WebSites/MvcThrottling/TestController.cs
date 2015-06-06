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
        [Throttling("10 requests per hour, sliding reset")]
        public string Sliding(int value)
        {
            return "Value : " + value; 
        }

        [HttpGet("Sliding2/{value}")]
        [Throttling("10 requests per hour, sliding reset")]
        public string Sliding2(int value)
        {
            return "Value2 : " + value;
        }

        [HttpGet("Fixed")]
        [Throttling("10 requests per hour, fixed reset")]
        public string Fixed()
        {
            return "OK";
        }

        [HttpGet("Bandwidth")]
        [Throttling("Bandwidth")]
        public string Bandwidth()
        {
            return "OK";
        }

        public string NoThrottling()
        {
            return "OK";
        }

        [HttpGet("MutliThrottling")]
        [AuthenticatedUserThrottling(10, 10)]
        [AuthenticatedUserThrottling(100, 60)]
        [IPThrottling(10, 10)]
        public string MutliThrottling()
        {
            return "OK";
        }
    }
}