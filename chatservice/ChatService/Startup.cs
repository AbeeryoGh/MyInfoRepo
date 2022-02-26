using ChatService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalRChat.Hubs;
using System.Collections;
using System.Collections.Generic;

namespace ChatService
{
  public class Startup
  {
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllers();
      services.AddSignalR();

      services.AddCors(options =>
      {
        options.AddDefaultPolicy(builder =>
                  {
                builder.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
              });
      });

      services.AddSingleton<IDictionary<string, UserConnection>>(opts => new Dictionary<string, UserConnection>());
      services.AddDbContext<ChatDBContext>(item => item.UseSqlServer(Configuration.GetConnectionString("DBCon")));
      
      //services.AddCors(c =>
      //{
      //  c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
      //});
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      //app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseCors();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapHub<ChatHub>("/chat");
        endpoints.MapControllers();
      });
     
    }
  }
}
