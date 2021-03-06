using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NationalBooks.Data;
using NationalBooks.Data.Interfaces;
using NationalBooks.Data.Services;

namespace NationalBooks.MVC
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;

            });

            services.AddSingleton<CosmosDBConnector>(serviceProvider =>
            {
                var endpointUri = Configuration["CosmosDBEndpoint"];
                var accessKey = Configuration["CosmosDBAccessKey"];
                var databaseName = Configuration["CosmosDBDatabase"];
                var cookieCollection = Configuration["CosmosDBBookCollectionName"];
                var orderCollection = Configuration["CosmosDBOrderCollectionName"];

                return new CosmosDBConnector(
                    endpointUri, accessKey, databaseName, cookieCollection, orderCollection);
            });

            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IBookService, BookService>();
            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
