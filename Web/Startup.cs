using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Web.Domain.Interfaces;
using Web.Domain.Services;
using Web.Infrastructure.Contexts;

namespace Web
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
      services.AddControllersWithViews()
        .AddNewtonsoftJson(options =>
        {
          options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        });

      services.AddAuthentication(options =>
        {
          options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
          options.SaveToken = true;
          options.RequireHttpsMetadata = false;
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Configuration["Jwt:Issuer"],
            ValidAudience = Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
            ClockSkew = TimeSpan.Zero
          };
        });

      services.AddSingleton<IDatabaseContext, DatabaseContext>();
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      services.AddScoped<IPasswordService, PasswordService>();
      services.AddScoped<IMessageService, MessageService>();
      services.AddScoped<IJwtService, JwtService>();
      services.AddScoped<IDatatableService, DatatableService>();
      services.AddScoped<IHelperService, HelperService>();
      services.AddScoped<IUserService, UserService>();
      services.AddTransient<IEmailService, EmailService>();
      services.AddScoped<IApproveService, ApproveService>();
      services.AddScoped<IItemService, ItemService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler(a => a.Run(async context =>
        {
          var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
          var exception = exceptionHandlerPathFeature.Error;

          var result = JsonConvert.SerializeObject(new { Error = exception.Message });
          context.Response.ContentType = "application/json";
          await context.Response.WriteAsync(result);
        }));
      }

      app.Use((context, next) =>
      {
        string token = "";

        if (context.Request.Cookies.ContainsKey(Configuration["Jwt:Name"]) == true)
        {
          token = context.Request.Cookies[Configuration["Jwt:Name"]];
          context.Request.Headers.Add("Authorization", "Bearer " + token);
        }

        return next();
      });

      app.UseStaticFiles();

      app.UseRouting();

      app.UseStatusCodePagesWithRedirects("/Home/Error?code={0}");

      app.UseAuthentication();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}");
      });
    }
  }
}
