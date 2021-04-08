using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Project1.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Project1
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
            services.AddDbContext<JobTestDB>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("JobTestDB")));

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<JobTestDB>();
                context.Database.EnsureCreated();
                List<Category> testCat = context.Categories.ToList();
                if (testCat.Count == 0)
                {
                    byte[] Light = File.ReadAllBytes("./Icons/light.ico");
                    byte[] Medium = File.ReadAllBytes("./Icons/medium.ico");
                    byte[] Heavy = File.ReadAllBytes("./Icons/heavy.ico");
                    var LightC = new Category { Name = "Light", Min = 0, Max = 500, PhotoFile = Light, ImageMimeType = "image/x-icon" };
                    var MediumC = new Category { Name = "Medium", Min = 500, Max = 2500, PhotoFile = Medium, ImageMimeType = "image/x-icon" };
                    var HeavyC = new Category { Name = "Heavy", Min = 2500, Max = 10000, PhotoFile = Heavy, ImageMimeType = "image/x-icon" };
                    context.Categories.Add(LightC);
                    context.SaveChanges();
                    context.Categories.Add(MediumC);
                    context.SaveChanges();
                    context.Categories.Add(HeavyC);
                    context.SaveChanges();
                }
            }
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
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Vehicles}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            }); 
        }
    }
}
