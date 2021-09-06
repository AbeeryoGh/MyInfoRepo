using EngineCoreProject.IRepository.IGeneralSetting;
using EngineCoreProject.Services;
using EngineCoreProject.DTOs.GeneralSettingDto;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


using System.Linq;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using System;
using EngineCoreProject.Services.GeneralSetting;

namespace EngineCoreProject.Controllers.DayOff
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class DayOffController : ControllerBase
    {

        private readonly IGlobalDayOffRepository _iGlobalDayOffRepository;
        public DayOffController(IGlobalDayOffRepository iGlobalDayOffRepository)
        {
            _iGlobalDayOffRepository = iGlobalDayOffRepository;
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost]
        public async Task<ActionResult> AddDayOff([FromBody] GlobalDayOffPostDto dayOffDto, [FromHeader] string lang)
        {
            var result = await _iGlobalDayOffRepository.AddDayOff(dayOffDto, lang);
            if (result != 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }

        [HttpPut("throwExceptionTest")]
        public IActionResult ThrowException (bool throwException)
        {
            _iGlobalDayOffRepository.ThrowExceptionTest(throwException);
            return Ok();
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(GlobalDayOffPostDto dayOffPostDto, int id, [FromHeader] string lang)
        {
            var result = await _iGlobalDayOffRepository.UpdateDayOff(dayOffPostDto, lang, id);
            if (result != 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            var result = await _iGlobalDayOffRepository.DeleteDayOff(id);
            if (result != 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet]
        public async Task<ActionResult> Get([FromHeader] string lang)
        {
            var result = await _iGlobalDayOffRepository.GetDaysOff(lang);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            else return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id, [FromHeader] string lang)
        {
            var result = await _iGlobalDayOffRepository.GetDayOff(id, lang);
            if (result != null && result.Id != 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }

    }
}
