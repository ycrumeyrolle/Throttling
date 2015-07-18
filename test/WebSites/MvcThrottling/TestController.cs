﻿using Microsoft.AspNet.Mvc;
using Throttling.Mvc;

namespace MvcThrottling
{
    [Route("{apikey}/test")]
    [Route("{apikey}/test2")]
    public class TestController : Controller
    {
        [HttpGet("Action1/{value}")]
        [EnableThrottling("10 requests per hour, fixed reset")]
        public string Action1(int value)
        {
            return "Action1 : " + value;
        }
        
        [HttpGet("Action2/{value}")]
        [EnableThrottling("10 requests per hour, fixed reset")]
        public string Action2(int value)
        {
            return "Action1 : " + value;
        }

        [HttpGet("Action3/{value}")]
        [EnableThrottling("160 bytes per hour by IP")]
        public string Action3(int value)
        {
            return "0123456789012345";
        }

        [HttpGet("Action4/{value}")]
        [EnableThrottling("160 bytes per hour by API key")]
        public string Action4(int value)
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