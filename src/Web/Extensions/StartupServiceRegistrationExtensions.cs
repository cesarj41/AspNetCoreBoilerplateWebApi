using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Threading.Tasks;
using Application.Interfaces;
using AutoMapper;
using Infrastructure.Data.ApplicationDb;
using Infrastructure.Identity;
using Infrastructure.Logging;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using Web.ViewModels;

namespace Web.Extensions
{
    public static class StartupServiceRegistrationExtensions
    {
        // public static void AddAutoMapperConfig(this IServiceCollection services) =>
        //     services.AddAutoMapper(new Assembly[] {
        //         typeof(AccountProfile).GetTypeInfo().Assembly
        //     });
        
        // public static void AddMediator(this IServiceCollection services) => 
        //     services.AddMediatR(typeof(CreateAccountCommand));

        public static void AddApplicationLayerLogging(this IServiceCollection services)
        {
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
        }
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }
            
        public static void AddDatabase(this IServiceCollection services, string url)
        {
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(url);
            });

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>(sp => 
                sp.GetRequiredService<ApplicationDbContext>());
        }

        public static void AddHttpContext(this IServiceCollection services) => 
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        

        public static void AddCookieAuthentication(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            
            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;                
            });

            services.ConfigureApplicationCookie(config => {
                config.Cookie.Name = "my_app_auth";
                config.Cookie.SameSite = SameSiteMode.Strict;
                config.LoginPath = new PathString("/api/accounts/login");
                config.AccessDeniedPath = new PathString("/api/accounts/login");
                config.Events.OnRedirectToLogin = context => context.AppendStatusCodeAsync(401);
                config.Events.OnRedirectToAccessDenied = context => context.AppendStatusCodeAsync(403);
                config.Events.OnRedirectToReturnUrl = context => context.AppendStatusCodeAsync(201);
            });
                
        }

        public static IMvcBuilder ConfigureOptionsForApiBehavior(this IMvcBuilder addMvc)
        {
            addMvc.ConfigureApiBehaviorOptions(options => {
                options.InvalidModelStateResponseFactory = context => 
                    new BadRequestObjectResult(new ErrorDetails(
                        context.ModelState.Errors()
                    )
                ); 
            });
            return addMvc;
        }
        
    }
}