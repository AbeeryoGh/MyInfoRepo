using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using EngineCoreProject.IRepository.ICalendarRepository;
using EngineCoreProject.DTOs.CalendarDto;
using EngineCoreProject.Services;
using EngineCoreProject.IRepository.IUserRepository;

namespace EngineCoreProject.Controllers.Calendar
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {

        private readonly ICalendarRepository _iCalendarRepository;
        private readonly IUserRepository _iUserRepository;

        public CalendarController(ICalendarRepository iCalendarRepository, IUserRepository iUserRepository)
        {
            _iCalendarRepository = iCalendarRepository;
            _iUserRepository = iUserRepository;
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult> AddCalendar([FromBody] CalendarPostDto calendarDto, [FromHeader] string lang)
        {
            var result = await _iCalendarRepository.AddCalendar(calendarDto, lang);
            if (result != 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, "error accrued");
        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet()]
        public async Task<ActionResult> Get()
        {
            var result = await _iCalendarRepository.GetCalendar(_iUserRepository.GetUserID());
            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(CalendarPostDto calendarPostDto, int id, [FromHeader] string lang)
        {
            var result = await _iCalendarRepository.UpdateCalendar(id, calendarPostDto, lang);
            if (result != 0)
            {

                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            var result = await _iCalendarRepository.DeleteCalendar(id);
            if (result != 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }


    }

}
