using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Web.Extensions;

namespace Web.Filters
{
    public class Logger : Attribute, IActionFilter, IResourceFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            string actionName = context.ActionDescriptor.DisplayName;
            context.HttpContext.Items.Add("actionName", actionName);
            Log.Information(
                "Proccessing request for path: {path}, action: {action}, params: {@arguments}",
                context.HttpContext.Request.Path.Value,
                actionName, 
                context.ActionArguments
            );
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            if (!context.ModelState.IsValid)
            {
                Log.Information(
                    "Proccessing request for path: {path}, action: {action}, params: {@arguments}",
                    context.HttpContext.Request.Path.Value,
                    context.ActionDescriptor.DisplayName,
                    context.RouteData.Values
                );
                Log.Warning(
                    "Model state validation failed with errors: {@errors}",
                    context.ModelState.Errors()
                );

                Log.Warning(
                    "Request for path: {path}, action: {action} was invalid, returned status code: {status}, result: {@result}",
                    context.HttpContext.Request.Path.Value,
                    context.ActionDescriptor.DisplayName,
                    context.HttpContext.Response.StatusCode,
                    context.Result
                );
            }
            else
            {
                Log.Information(
                    "Request for path: {path}, action: {action} finished proccessing , returned status code: {status}, result: {@result}", 
                    context.HttpContext.Request.Path.Value,
                    context.ActionDescriptor.DisplayName,
                    context.HttpContext.Response.StatusCode,
                    context.Result
                );
            }
        }

        public void OnResourceExecuting(ResourceExecutingContext context) {}
        public void OnActionExecuted(ActionExecutedContext context) {}
    }
}