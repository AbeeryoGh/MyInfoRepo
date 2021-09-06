using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.ApplicationSet
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ApplicationTracksController : ControllerBase
    {
        private readonly IApplicationTrackRepository _IApplicationTrackRepository;

        public ApplicationTracksController(IApplicationTrackRepository iApplicationTrackRepository)
        {
            _IApplicationTrackRepository = iApplicationTrackRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> All([FromQuery] int? applicationId, [FromHeader] string lang)
        {
            try
            {
                var result = await _IApplicationTrackRepository.GetAll(applicationId);
                return Ok(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,//e.Message.ToString()
                new { message = Constants.getMessage(lang, "UnknownError") });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task<ActionResult> One([FromRoute] int id, [FromHeader] string lang)
        {
            try
            {
                var result = await _IApplicationTrackRepository.GetOne(id);
                if (result != null)
                    return Ok(result);
                else
                    return NotFound(new { message = Constants.getMessage(lang, "zeroResult") });
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                      new { message = Constants.getMessage(lang, "UnknownError") });
            }
        }

        //-------------------------------------------------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ApplicationTrackDto applicationTrackDto, [FromHeader] string lang)
        {
            int result = await _IApplicationTrackRepository.Update(id, applicationTrackDto);
            return result switch
            {
                Constants.OK => Ok(new { message = Constants.getMessage(lang, "sucsessUpdate") }),
                Constants.NOT_FOUND => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }

        //------------------------------Post Just Record-------------------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ApplicationTrackDto applicationTrackDto, [FromHeader] string lang)
        {
            int result = await _IApplicationTrackRepository.Add(applicationTrackDto);
            return result switch
            {
                Constants.ERROR => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
                _ => Ok(new
                {
                    message = Constants.getMessage(lang, "sucsessAdd"),
                    id = result
                })
            };
        }

        //------------------------------------------------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            int result = await _IApplicationTrackRepository.DeleteOne(id);
            return result switch
            {
                Constants.OK => Ok(new { message = Constants.getMessage(lang, "sucsessDelete") }),
                Constants.NOT_FOUND => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }
    }
}
