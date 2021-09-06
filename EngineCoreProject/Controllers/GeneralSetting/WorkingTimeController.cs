using EngineCoreProject.DTOs.GeneralSettingDto;
using EngineCoreProject.IRepository.IGeneralSetting;
using EngineCoreProject.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace EngineCoreProject.Controllers.WorkingTime
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class WorkingTimeController : ControllerBase
    {
        private readonly IWorkingTimeRepository _iWorkingTimeRepository;
        public WorkingTimeController(IWorkingTimeRepository iWorkingTimeRepository)
        {
            _iWorkingTimeRepository = iWorkingTimeRepository;
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost]
        public async Task<ActionResult> AddWorkingTime([FromBody] WorkingTimePostDto workingDto, [FromHeader] string lang)
        {
            var result = await _iWorkingTimeRepository.AddWorkingTime(workingDto, lang);
            if (result != 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(WorkingTimePostDto workingTimePostDto, int id, [FromHeader] string lang)
        {
            var result = await _iWorkingTimeRepository.UpdateWorkingTime(workingTimePostDto, id, lang);
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
            var result = await _iWorkingTimeRepository.DeleteWorkingTime(id, lang);
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
            var result = await _iWorkingTimeRepository.GetWorkingHours(lang);
            if (result != null)
            {

                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetWorkingHoursBetweenTwoDates")]
        public async Task<ActionResult> Get(DateTime sDateTime, DateTime eDateTime)
        {
            var result = await _iWorkingTimeRepository.GetWorkingMinutesBetweenTwoDates(sDateTime, eDateTime);
            return StatusCode(StatusCodes.Status200OK, result);
        }


        [HttpGet("GetDeadline")]
        public async Task<ActionResult> GetDeadline()
        {
            List<int> ddd = new List<int> { 20, 30, 20, 20 };
            var result = await _iWorkingTimeRepository.GetDeadline(ddd);
            return StatusCode(StatusCodes.Status200OK, result);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id, [FromHeader] string lang)
        {
            var result = await _iWorkingTimeRepository.GetWorkTimeId(id, lang);
            if (result != null && result.Id != 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("WorkingDates")]
        public async Task<ActionResult> Get(DateTime untilDate, [FromHeader] string lang)
        {
            var result = await _iWorkingTimeRepository.GetWorkingDates(untilDate);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }
    }
}
