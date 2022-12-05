using projectman.Areas.Man.Controllers;
using projectman.Data;
using projectman.Repositories;
using CSHelper.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using projectman.Util;

namespace projectman
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        public IConfiguration _config { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            services.AddDbContext<DBContext>(options =>
                options.UseSqlServer(_config.GetConnectionString("DefaultConnection")));

            services.AddScoped<ILookupRepository, LookupRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITransaction, Transaction>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<ILDAPClient, LDAPClient>();

            services.AddAuthentication(ManDefaults.AuthenticationScheme)
                .AddCookie(ManDefaults.AuthenticationScheme, o =>
                {
                    o.LoginPath = new PathString("/Home/Login");
                    o.LogoutPath = new PathString("/Home/Logout");
                    o.AccessDeniedPath = new PathString("/Home/AccessDenied");
                }
                );

            services.AddAuthorization(options =>
            {
                options.AddPolicy("backstage", new AuthorizationPolicyBuilder(ManDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());

                /*
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                */
            });

            services.AddHttpContextAccessor();

            services.AddControllersWithViews(o =>
            {
                o.ModelBinderProviders.Insert(0, new EnumFlagTypeModelBinderProvider());
            })
#if DEBUG
                .AddRazorRuntimeCompilation()
#endif
                ;
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                
                endpoints.MapControllerRoute(
                    name: "area",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}").RequireAuthorization("backstage");
                
                endpoints.MapAreaControllerRoute(
                    name: "Man",
                    areaName: "man",
                    pattern: "{controller=Home}/{action=Login}").RequireAuthorization("backstage");
                
            });
        }
    }
}
