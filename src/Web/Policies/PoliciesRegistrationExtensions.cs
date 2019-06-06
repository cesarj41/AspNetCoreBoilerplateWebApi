using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Web.Policies
{
    public static class PoliciesRegistrationExtensions
    {
        public static void AddApplicationPolicies(this MvcOptions config)
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            config.Filters.Add(new AuthorizeFilter(policy));  
        }          
        
    }
}