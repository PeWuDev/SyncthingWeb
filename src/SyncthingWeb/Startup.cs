using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentScheduler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SyncthingWeb.Areas.Devices.Permissions;
using SyncthingWeb.Areas.Share.Providers;
using SyncthingWeb.Attributes;
using SyncthingWeb.Authorization;
using SyncthingWeb.Bus;
using SyncthingWeb.Commands;
using SyncthingWeb.Commands.Implementation.Events;
using SyncthingWeb.Data;
using SyncthingWeb.Exceptions;
using SyncthingWeb.Models;
using SyncthingWeb.Modules;
using SyncthingWeb.Permissions;
using SyncthingWeb.Services;
using SyncthingWeb.Syncthing;
using SyncthingWebUI.Areas.Folders.Module;
using SyncthingWebUI.Areas.Users.Permissions;
using SyncthingWebUI.Authorization;

namespace SyncthingWeb
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets(Assembly.GetEntryAssembly());
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }


        public static IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var sqlProvider = Configuration.GetValue<string>("DatabaseProvider").ToLowerInvariant();

                //TODO factory or sth - separate it!
                switch (sqlProvider)
                {
                    case "mssql":
                        options.UseSqlServer(Configuration.GetConnectionString("DefaultMSSQLConnection"));
                        break;
                    case "sqlite":
                        options.UseSqlite(Configuration.GetConnectionString("DefaultSQLiteConnection"));
                        break;
                    default:
                        throw new NotSupportedException("Provided type is not supported.");
                }

            });
          
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc(opt =>
            {
                opt.Filters.Add(typeof(SetupRequiredAttribute));
                opt.Filters.Add(typeof(GlobalExceptionFilter));
            });

            services.AddDistributedMemoryCache();
            services.AddSession();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();



            var builder = new ContainerBuilder();

            // Register dependencies, populate the services from
            // the collection, and build the container. If you want
            // to dispose of the container at the end of the app,
            // be sure to keep a reference to it as a property or field.
            //builder.RegisterType<MyType>().As<IMyType>();
            builder.Populate(services);

            builder.RegisterModule<CacheModule>();
            builder.RegisterModule<NotificationsModule>();
            builder.RegisterModule<CommandsModule>();
            builder.RegisterModule<EventBusModule>();
            builder.RegisterModule<SyncthingModule>();
            builder.RegisterModule<AuthorizationModule>();
            builder.RegisterModule<FoldersModule>();

            ConfigureEvents(builder);

            ApplicationContainer = builder.Build();

            using (var scope = ApplicationContainer.BeginLifetimeScope())
            using (var ctx = scope.Resolve<ApplicationDbContext>())
            {
                ctx.Database.Migrate();
            }

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment() || this.Configuration["DevelopmentErrors"] == "true")
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });


            JobManager.Start();
        }

        public void ConfigureEvents(ContainerBuilder builder)
        {
            builder.RegisterType<ShareProvider>()
             .As<IEventHandler<IShareCollector>>()
             .InstancePerDependency()
             .PropertiesAutowired();

            builder.RegisterType<DefaultPermissionResolver>()
                .As<IEventHandler<AddedUserRoleEvent>>()
                .InstancePerDependency()
                .PropertiesAutowired();

            builder.RegisterType<DefaultPermissionResolver>()
                .As<IEventHandler<RemovedUserRoleEvent>>()
                .InstancePerDependency()
                .PropertiesAutowired();

            builder.RegisterType<DevicePermissions>()
                .As<IEventHandler<IPermissionCollector>>()
                .InstancePerDependency()
                .PropertiesAutowired();

            builder.RegisterType<RolePermissions>()
                .As<IEventHandler<IPermissionCollector>>()
                .InstancePerDependency()
                .PropertiesAutowired();

            builder.RegisterType<UserPermissions>()
                .As<IEventHandler<IPermissionCollector>>()
                .InstancePerDependency()
                .PropertiesAutowired();
        }
    }
}
