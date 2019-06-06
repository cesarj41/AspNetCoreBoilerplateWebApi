using Application.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Web.Extensions;
using Web.ViewModels;

namespace Web.Middlewares
{
    public static class MiddlewareRegistrationExtensions
    {
        public static void UseCustomStatusCode(this IApplicationBuilder app) =>
            app.UseStatusCodePages(async context => {
                string requestPath = context.HttpContext.Request.Path.Value;
                string httpMethod = context.HttpContext.Request.Method;
                string problem = "Invalid request";
                int statusCode = context.HttpContext.Response.StatusCode;

                if (statusCode == 401) problem = "Unauthorized request";
                else if (statusCode == 403) problem = "Forbidden request";
                else if (statusCode == 404) problem = "Resource not found";
                else if (statusCode == 405) problem = "Invalid request, method not allowed";
                else if (statusCode == 406) problem = "Invalid request, not acceptable";

                var errorDetails = new ErrorDetails(problem);

                Log.Warning(
                    "Request for path: {path}, method: {httpMethod} was invalid, returned status code: {status}, result: {@result}",
                    requestPath,
                    httpMethod,
                    statusCode,
                    errorDetails
                );
                await context.HttpContext.Response.SendAsync(errorDetails);
            });
            
        
        public static void UseErrorHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp => errorApp.Run(async context => {
                ErrorDetails errorDetails = null;
                int statusCode = 500;
                object actionName = "";
                var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = errorFeature.Error;

                context.Items.TryGetValue("actionName", out actionName);
                if (exception is BaseException baseException)
                {
                    statusCode = baseException.StatusCode;
                    errorDetails = new ErrorDetails(baseException.Errors);
                    Log.Warning(
                        "An Exception occured ! instance: {instance}, exception: {@exception}",
                        errorDetails.instance,
                        exception
                    );

                    Log.Warning(
                        "Request for path: {path}, action: {action} was invalid, returned status code: {status}, result: {@result}",
                        context.Request.Path,
                        actionName as string,
                        statusCode,
                        errorDetails
                    );
                }
                else
                {
                    errorDetails = new ErrorDetails("Internal server error");
                    Log.Error(
                        "An unexpected error occurred! instance: {instance}, exception: {@exception}",
                        errorDetails.instance,
                        exception
                    );
                    Log.Warning(
                        "An error ocurred proccessing request for path: {path}, action: {action}, returned status code: {status}, result: {@result}",
                        context.Request.Path,
                        actionName as string,
                        statusCode,
                        errorDetails
                    );
                }

                await context.Response.Status(statusCode).SendAsync(errorDetails);
            }));
        }
        public static void UseLoggerEnricher(this IApplicationBuilder app) => 
            app.UseMiddleware<LoggerEnricherMiddleware>();
        public static void UseSwaggerTools(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My App");
            });
        }

        public static void UseCustomForwardedHeaders(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
        }

    }
}