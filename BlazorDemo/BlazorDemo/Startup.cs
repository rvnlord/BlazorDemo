using System;
using AutoMapper;
using BlazorDemo.Models;
using BlazorDemo.Services;
using CommonLibrary.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlazorDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddAutoMapper(typeof(EmployeeMappingProfile));
            services.AddScoped<IRefreshService, RefreshService>();
            services.AddScoped<IParametersService, ParametersService>();
            services.AddHttpClient<IEmployeeService, EmployeeService>(client => client.BaseAddress = new Uri("http://localhost:4658"));
            services.AddHttpClient<IDepartmentService, DepartmentService>(client => client.BaseAddress = new Uri("http://localhost:4658"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
