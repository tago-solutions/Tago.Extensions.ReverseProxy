using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Tago.Extensions.ReverseProxy.Settings;

namespace Tago.Infra.Proxy
{

    public class Startup
    {
        private ReverseProxySettings settings = new ReverseProxySettings();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            /***************************************************/
            var pb = services.AddProxy(Configuration, "ProxySettings");
            /***************************************************/

            services.AddJwtSigner(opts =>
            {
                opts.Configure(Configuration.GetSection("JwtSigner"));
            });
            //services.AddJwt(opts => {
            //    opts.Configure(Configuration.GetSection("JwtSettings"));
            //});

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               //.AddJwt(opts => opts.Configure(Configuration.GetSection("JwtSigner")))
               //.AddCustomAuth("test")
               ;

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
                c.SwaggerDoc("auth", new OpenApiInfo { Title = "WebApi", Version = "auth" });
            });
            //services.AddSwaggerGenNewtonsoftSupport();

            //services.AddRestClient();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(builder =>
            {
                builder.SetIsOriginAllowed(isOriginAllowed: (origin) => { return true; }).
                AllowCredentials().
                AllowAnyMethod().
                AllowAnyHeader();

            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger(c =>
            {
                //c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                //c.RoutePrefix = "api";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1");
                c.SwaggerEndpoint("/swagger/auth/swagger.json", "Auth v1");
            }
            );

            app.UseRouting();
            //app.UseCookiePolicy();

            IConfigurationSection s = Configuration.GetSection("ProxySettings");
            s.Bind(settings);

            app.RunProxy(
                hooks =>
                {

                    hooks.OnStatusResult(401, async (response, request, resender) =>
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            //request.Headers.Remove("Authorization");
                            return await resender.ResendAsync(request);
                        }
                        else
                        {
                            return response;
                        }
                    });


                    hooks.SetCertificatesAsync = async (ctx, ep) =>
                    {
                        return null;
                    };

                    //hooks.BeforeSend = (ctx, request) =>
                    //{
                    //    //var f = ctx.ServiceProvider.GetService<IHttpClientHandlerFactory>();

                    //    //request.Headers.TryAddWithoutValidation("user_name", ctx?.HttpContext.User?.Identity?.Name);
                    //    //return ctx;
                    //};

                    hooks.BeforeResponse = (ctx, routePath, setting) =>
                    {
                        //ctx.HttpContext.Response.Cookies.Append("justbeforeresponse", "yes", new CookieOptions
                        //{
                        //    Path = routePath
                        //});                        
                    };
                });


            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }



    }
}
