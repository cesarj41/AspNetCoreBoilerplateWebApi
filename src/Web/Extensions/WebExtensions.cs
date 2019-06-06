using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Web.ViewModels;

namespace Web.Extensions
{
    public static class WebExtensions
    {
        public static Task AppendStatusCodeAsync(
            this RedirectContext<CookieAuthenticationOptions> context, int statusCode)
        {
            context.HttpContext.Response.StatusCode = statusCode;
            return Task.CompletedTask;

        }

        public static string[] Errors(this ModelStateDictionary modelState) =>
            modelState.Values
                .SelectMany(values => values.Errors)
                .Select(error => error.ErrorMessage)
                .ToArray();

        public static ApplicationUserViewModel ToViewModel(this ApplicationUser user) =>
            new ApplicationUserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        
        public static HttpResponse Status(this HttpResponse response, int statusCode)
        {
            response.StatusCode = statusCode;
            return response;
        }
        public static Task SendAsync<T>(this HttpResponse response, T result)
        {
            response.ContentType = "application/json";
            var stringifyResult = JsonConvert.SerializeObject(result);
            return response.WriteAsync(stringifyResult);
        }
        
        public static IEnumerable<string> Errors(this IdentityResult result) =>
            result.Errors.Select(error => $"{error.Code}: {error.Description}");
        
        public static string ToString(this IdentityResult result) => result.Errors
            .Select(error => $"{error.Code}: {error.Description}")
            .Aggregate((accum, curr) => $"{accum}{Environment.NewLine}{curr}");

        
        
    }
}