using System;
using Microsoft.AspNet.Mvc;
using Throttling.Mvc;
using Microsoft.AspNet.Mvc.Description;

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
        [Throttling("10 requests per 10 seconds, fixed reset")]
        public string Fixed()
        {   
            return "OK";
        }

        public string NoThrottling()
        {
            return "OK";
        }

        [HttpGet("MutliThrottling")]
        [UserThrottling(10, 10)]
        [UserThrottling(100, 60)]
        [IPThrottling(10, 10)]
        public string MutliThrottling()
        {
            return "OK";
        }
    }
}