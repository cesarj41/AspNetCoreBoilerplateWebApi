using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Extensions;
using Web.Middlewares;
using Web.Policies;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabase(Configuration["DbConnection"]);            
            services.AddHttpContext();
            services.AddSwagger();
            //services.AddAutoMapperConfig();
            services.AddCookieAuthentication();
            //services.AddMediator();
            services.AddApplicationLayerLogging();
            services.AddMvc(config => {
                config.AddApplicationPolicies();
            })
            .ConfigureOptionsForApiBehavior()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseSwaggerTools();
            }
            else
            {
                app.UseErrorHandler();
                app.UseCustomStatusCode();
            }
            
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseSwaggerTools();
            app.UseLoggerEnricher();
            app.UseMvc();
        }
    }
}
