using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestUpload.Profile;
using TestUpload.Repository.SQL;
using TestUpload.Service;
using TestUpload.Securities;
using TestUpload.Repository;

namespace TestUpload
{
    public class Startup
    {
        string Connstring;
        public Startup(IConfiguration configuration)
        {
            Connstring = configuration.GetConnectionString("Default");
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
            });

            services.AddControllersWithViews();
            services.AddDbContext<DataContext>(options =>
        options.UseMySQL(Connstring));

            #region Repository
            services.AddScoped<LoginRepository>();
            services.AddScoped<UserRepository>();
            services.AddScoped<ChangepassRepository>();
            services.AddScoped<HistoryRepository>();
            services.AddScoped<ErrorLogRepository>();
            services.AddScoped<FileUploadRepository>();
            services.AddScoped<FileStorageRepository>();
            services.AddScoped<SessionRepository>();
            services.AddScoped<FileTotalRepository>();
            #endregion

            #region Services
            services.AddScoped<IChangepasswordService, ChangepasswordService>();
            services.AddScoped<IuserService, userService>();           
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IhistoryLogService, historyLogService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<ISessionServices, SessionServices>();
            services.AddScoped<IFileTotalServices, FileTotalServices>();
            #endregion
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IpAddress>();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(6);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddDistributedMemoryCache();
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            
        }
    }
}
