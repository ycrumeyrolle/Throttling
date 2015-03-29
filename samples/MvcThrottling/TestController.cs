using System;
using Microsoft.AspNet.Mvc;
using Throttling.Mvc;

namespace MvcThrottling
{
    public class TestController : Controller
    {
        [Throttling("Test")]
        public string Action1()
        {
            return "OK";
        }

        public string Action2()
        {
            return "OK";
        }
    }
}