using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;

namespace Macuxi
{
    public class Startup
    {

        private const string GitHubClientId = "GitHubClientId";
        private const string GitHubClientSecret = "GitHubClientSecret";
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {                
            services.AddControllers();     
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie()
                .AddGitHub(options =>
                {
                    //options.CallbackPath = new PathString("/signin-github");
                    options.ClientId = Configuration[GitHubClientId];
                    options.ClientSecret = Configuration[GitHubClientSecret];
                    options.Scope.Add("user:email");
                });
            services.AddAuthorization();
            services.AddSignalR().AddAzureSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {                                
            app.UseAuthentication();   
            app.UseFileServer();   
            app.UseRouting();                                       
            app.UseAuthorization();
            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute("default", "/", defaults: new { controller = "Auth", action = "login"} );
                routes.MapHub<Chat>("/chat");
            });                                                          
        }       
    }
}
