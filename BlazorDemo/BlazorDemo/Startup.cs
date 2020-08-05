using System;
using AutoMapper;
using BlazorDemo.Common.Models.Account.MappingProfiles;
using BlazorDemo.Common.Models.Admin.MappingProfiles;
using BlazorDemo.Common.Models.EmployeeManagement.MappingProfiles;
using BlazorDemo.Common.Services.Common;
using BlazorDemo.Common.Services.Common.Interfaces;
using BlazorDemo.Common.Services.Frontend;
using BlazorDemo.Common.Services.Frontend.Account;
using BlazorDemo.Common.Services.Frontend.Account.Interfaces;
using BlazorDemo.Common.Services.Frontend.Admin;
using BlazorDemo.Common.Services.Frontend.Admin.Interfaces;
using BlazorDemo.Common.Services.Frontend.EmployeeManagement;
using BlazorDemo.Common.Services.Frontend.EmployeeManagement.Interfaces;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static BlazorDemo.Common.Utils.ConfigUtils;

namespace BlazorDemo
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IServiceProviderProxy, HttpContextServiceProviderProxy>();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredLocalStorage();
            services.AddAutoMapper(typeof(UserMappingProfile), typeof(AdminMappingProfile), typeof(RoleMappingProfile), typeof(ClaimMappingProfile));
            services.AddScoped<IRefreshService, RefreshService>();
            services.AddScoped<IParametersService, ParametersService>();
            services.AddHttpClient<IAccountService, AccountService>(client => client.BaseAddress = new Uri(ApiBaseUrl));
            services.AddHttpClient<IAdminService, AdminService>(client => client.BaseAddress = new Uri(ApiBaseUrl));
            services.AddHttpClient<IEmployeeService, EmployeeService>(client => client.BaseAddress = new Uri(ApiBaseUrl));
            services.AddHttpClient<IDepartmentService, DepartmentService>(client => client.BaseAddress = new Uri(ApiBaseUrl));
            services.AddScoped<AuthenticationStateProvider, UserAuthenticationStateProvider>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider sp)
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
            ServiceLocator.Initialize(sp.GetService<IServiceProviderProxy>());
        }
    }
}
