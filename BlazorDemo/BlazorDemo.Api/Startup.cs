using System;
using AutoMapper;
using BlazorDemo.Common.Extensions;
using BlazorDemo.Common.Models;
using BlazorDemo.Common.Models.Account;
using BlazorDemo.Common.Models.Account.MappingProfiles;
using BlazorDemo.Common.Models.Admin.MappingProfiles;
using BlazorDemo.Common.Security;
using BlazorDemo.Common.Services;
using BlazorDemo.Common.Services.Backend.Account;
using BlazorDemo.Common.Services.Backend.Account.Interfaces;
using BlazorDemo.Common.Services.Backend.Admin;
using BlazorDemo.Common.Services.Backend.Admin.Interfaces;
using BlazorDemo.Common.Services.Backend.EmployeeManagement;
using BlazorDemo.Common.Services.Backend.EmployeeManagement.Interfaces;
using BlazorDemo.Common.Services.Common;
using BlazorDemo.Common.Services.Common.Interfaces;
using BlazorDemo.Common.Services.Frontend.Account;
using BlazorDemo.Common.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using JsonConverter = BlazorDemo.Common.Converters.JsonConverter;

namespace BlazorDemo.Api
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
            ConfigUtils.DBCS = Configuration.GetConnectionString("DBCS");
            services.AddControllers().AddNewtonsoftJson(o => o.SerializerSettings.SetSettings(JsonConverter.JSerializer().GetSettings()));
            services.AddDbContextPool<AppDbContext>(o => o.UseSqlServer(ConfigUtils.DBCS, b => b.MigrationsAssembly("BlazorDemo.Api")));
            services.AddScoped<IPasswordHasher<User>, SHA3withECDSAPasswordHasher<User>>();
            services.AddIdentity<User, IdentityRole<Guid>>(o =>
                {
                    o.SignIn.RequireConfirmedEmail = true;
                    o.Lockout.MaxFailedAccessAttempts = 5;
                    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    o.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmationTokenProvider";
                    o.Password.RequiredUniqueChars = 5;
                })
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<CustomEmailConfirmationTokenProvider<User>>("CustomEmailConfirmationTokenProvider");;
            services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromMinutes(15));
            services.AddAuthentication()
                .AddGoogle(o =>
                {
                    o.ClientId = Configuration.GetSection("GoogleApi").GetSection("ClientId").Value;
                    o.ClientSecret = Configuration.GetSection("GoogleApi").GetSection("ClientSecret").Value;
                })
                .AddFacebook(o =>
                {
                    o.AppId = Configuration.GetSection("FacebookApi").GetSection("AppId").Value;
                    o.AppSecret = Configuration.GetSection("FacebookApi").GetSection("AppSecret").Value;
                })                
                .AddTwitter(o =>
                {
                    o.ConsumerKey = Configuration.GetSection("TwitterApi").GetSection("ConsumerKey").Value;
                    o.ConsumerSecret = Configuration.GetSection("TwitterApi").GetSection("ConsumerSecret").Value;
                    o.RetrieveUserDetails = true;
                });
            services.AddScoped<IEmailSenderManager, EmailSenderManager>();
            services.AddScoped<IAccountManager, AccountManager>();
            services.AddScoped<IAdminManager, AdminManager>();
            services.AddAutoMapper(typeof(UserMappingProfile), typeof(AdminMappingProfile), typeof(RoleMappingProfile), typeof(ClaimMappingProfile));
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider sp)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            ServiceLocator.Initialize(sp.GetService<IServiceProviderProxy>());
        }
    }
}
