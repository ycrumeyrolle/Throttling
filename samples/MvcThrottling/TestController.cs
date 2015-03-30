using System;
using Microsoft.AspNet.Mvc;
using Throttling.Mvc;

namespace MvcThrottling
{
    public class TestController : Controller
    {
        [HttpGet("test/action/{value}")]
        [Throttling("5 requests per 10 seconds, sliding reset")]
        public string Action1(int value)
        {
            return "OK " + value;
        }

        [Throttling("5 requests per 10 seconds, fixed reset")]
        public string Action2()
        {
            return "OK";
        }

        public string NoThrottling()
        {
            return "OK";
        }
    }
}