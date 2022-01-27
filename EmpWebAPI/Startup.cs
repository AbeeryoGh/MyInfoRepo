using EmpRepositoryLayer.RepositoryPattern;
using EmpRepositoryLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmpServiceLayer.EmpServices;

namespace EmpWebAPI
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
      services.AddControllers();
      //add connection string
      services.AddDbContext<ApplicationDbContext>(item => item.UseSqlServer(Configuration.GetConnectionString("EmpDBCon"),
        b => b.MigrationsAssembly("EmpRepositoryLayer")));



      //add services injected
      services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
      services.AddTransient<IEmployeeService, EmployeeService>();
      services.AddTransient<IVacationService, VacationService>();
      services.AddTransient<IETaskService, ETaskService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        //Developement Environment exception handler
        app.UseDeveloperExceptionPage();

      }
      else
      {
        //Non-Developement Environment exception handler
        app.UseExceptionHandler("/error");
      }
      //Non-Developement Environment exception handler
      app.UseStatusCodePages("text/plain", "Status code page, status code: {0}");

      //app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
