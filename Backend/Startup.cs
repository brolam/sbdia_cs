using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Backend.BackgroundServices;

namespace Backend
{
  public class Startup
  {
    private readonly IConfiguration configuration;
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
      this.configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      string defaultConnection = configuration.GetConnectionString("DefaultConnection");

      services.AddDbContext<AppDbContext>(options =>
          options.UseSqlite(defaultConnection)
      );

      services.AddDefaultIdentity<Owner>()
          .AddEntityFrameworkStores<AppDbContext>();

      services.AddIdentityServer()
          .AddApiAuthorization<Owner, AppDbContext>();

      services.AddAuthentication()
          .AddIdentityServerJwt();

      services.AddControllersWithViews();
      services.AddRazorPages();

      // In production, the React files will be served from this directory
      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "ClientApp/build";
      });

      services.AddHostedService<SensorLogBatchBackgroundService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      UpdateDatabase(app);
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
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

      app.UseAuthentication();
      app.UseIdentityServer();
      app.UseAuthorization();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller}/{action=Index}/{id?}");
        endpoints.MapRazorPages();
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

    private static void UpdateDatabase(IApplicationBuilder app)
    {
      using (var serviceScope = app.ApplicationServices
          .GetRequiredService<IServiceScopeFactory>()
          .CreateScope())
      {
        using (var context = serviceScope.ServiceProvider.GetService<AppDbContext>())
        {
          context.Database.Migrate();
        }
      }
    }
  }
}
