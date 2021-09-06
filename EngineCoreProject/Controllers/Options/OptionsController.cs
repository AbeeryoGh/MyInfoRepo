using EngineCoreProject.DTOs.ConfigureWritableDto;
using EngineCoreProject.IRepository.IConfigureWritable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.Options
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptionsController : Controller
    {
        //private readonly IWritableOptions<Person> _writableLocations;
        //public OptionsController(IWritableOptions<Person> writableLocations)
        //{
        //    _writableLocations = writableLocations;
        //}
        //[HttpPost]
        ////Update Locations:Name
        //public IActionResult Change(string value)
        //{
        //    _writableLocations.Update(opt => {
        //        opt.p1 = value;
        //    });
        //    return Ok("OK");
        //}
    }
}
