using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModels;

namespace Throttling.Mvc
{
    public class ThrottlingAttribute : Attribute, IActionModelConvention
    {
        private readonly string _policyName;

        public ThrottlingAttribute(string policyName)
        {
            _policyName = policyName;
        }

        public void Apply(ActionModel model)
        {
            model.Filters.Add(new ThrottlingAuthorizationFilterFactory(_policyName));
        }
    }
}