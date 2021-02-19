using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIDConnect.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication; 
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc.Authorization; 
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace OpenIDConnect
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddAuthorizationCore();
            services.AddHttpClient();

            services.AddSingleton<WeatherForecastService>();

            services.AddScoped<TokenProvider>();
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                    .AddCookie()
                    .AddOpenIdConnect("oidc", options =>
                    {
                        options.Authority = "https://demo.identityserver.io/";
                        options.ClientId = "interactive.confidential.short"; // 75 seconds
                        options.ClientSecret = "secret";
                        // because we are using the regular web app flow, our initial request is for an authorization 
                        // code; when we request our tokens using this code, we will receive the ID Token we need for 
                        // authentication.
                        options.ResponseType = OpenIdConnectResponseType.Code;
                        options.SaveTokens = true;
                        options.GetClaimsFromUserInfoEndpoint = true;
                        options.UseTokenLifetime = false;
                        // the scope parameter includes three values, the requested OIDC scopes:
                        // openid(to indicate that the application intends to use OIDC to verify the user's identity)
                        // profile(to get name, nickname, and picture)
                        // email(to get email and email_verified)
                        options.Scope.Add("openid");
                        options.Scope.Add("profile");
                        options.Scope.Add("email");
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            NameClaimType = "name"
                        };
                      
                        options.Events = new OpenIdConnectEvents
                        {
                            OnAccessDenied = context =>
                            {
                                context.HandleResponse();
                                context.Response.Redirect("/");
                                return Task.CompletedTask;
                            }
                        };
                    });
        }

   

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
