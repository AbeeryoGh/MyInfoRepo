using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmpWebAPI.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class ErrorController : ControllerBase
  {
    //[HttpGet]
    //[Route("/error-local-development")]
    //public IActionResult ErrorLocalDevelopment([FromServices] IWebHostEnvironment webHostEnvironment)
    //{
    //  //if (webHostEnvironment.EnvironmentName != "Development")
    //  //{
    //  //  throw new InvalidOperationException(
    //  //      "This shouldn't be invoked in non-development environments.");
    //  //}

    //  var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

    //  return Problem(
    //      detail: context.Error.StackTrace,
    //      title: context.Error.Message,
    //      statusCode : context.Error.HResult);
    //}

    [HttpGet]
    [Route("/error")]
    public IActionResult Error([FromServices] IWebHostEnvironment webHostEnvironment)
    {
      var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
      return Problem(
          detail: context.Error.StackTrace,
          title: context.Error.Message,
          statusCode: context.Error.HResult);
    }

  }

}
