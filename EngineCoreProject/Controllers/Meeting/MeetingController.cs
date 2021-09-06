using EngineCoreProject.IRepository.IMeetingRepository;
using EngineCoreProject.Services;
using EngineCoreProject.DTOs.MeetingDto;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using EngineCoreProject.IRepository.IUserRepository;

namespace EngineCoreProject.Controllers.Meeting
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MeetingController : ControllerBase
    {

        private readonly IMeetingRepository _IMeetingRepository;
        private readonly IUserRepository _iUserRepository;
        public MeetingController(IMeetingRepository iMeetingRepository, IUserRepository iUserRepository)
        {
            _IMeetingRepository = iMeetingRepository;
            _iUserRepository = iUserRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult> AddMeeting([FromBody] MeetingPostDto meetingDto, [FromHeader] string lang)
        {
            var result = await _IMeetingRepository.AddMeeting(meetingDto, _iUserRepository.GetUserID(), lang);
            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            return StatusCode(StatusCodes.Status404NotFound, "error accrued");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> Get([FromHeader] string lang)
        {
            var result = await _IMeetingRepository.GetMeetings(_iUserRepository.GetUserID(), lang);
            return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(MeetingPostDto MeetingPostDto, int id, [FromHeader] string lang)
        {
            var result = await _IMeetingRepository.UpdateMeeting(id, MeetingPostDto, _iUserRepository.GetUserID(), lang);
            return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id, [FromHeader] string lang)
        {
            var result = await _IMeetingRepository.GetMeetingById(id, lang);
            return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{meetingid}")]
        public async Task<ActionResult> GetMeeting(string meetingId, [FromHeader] string lang)
        {
            var result = await _IMeetingRepository.GetMeetingByMeetingId(meetingId, lang);
            return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }


        [HttpGet("GetMeetingInfo")]
        public async Task<ActionResult> GetMeetingInfo(string meetingId, string password, [FromHeader] string lang)
        {
            var result = await _IMeetingRepository.GetMeetingByMeetingIdAndPassword(meetingId, password, lang);
            return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("LogInToMeeting")]
        public async Task<ActionResult> LogInToMeeting(string meetingNo)
        {
            var result = await _IMeetingRepository.LogInToMeeting(meetingNo);
            return Ok(result);
        }

        [HttpGet("GetMeetingForOrderNo")]
        public async Task<ActionResult> GetMeetingForOrderNo(string OrderNo)
        {
            var result = await _IMeetingRepository.GetMeetingByOrderNo(OrderNo);
            return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [HttpGet("IsAttendedByAppNo")]
        public async Task<ActionResult> IsAttended(int meetingId)
        {
            var result = await _IMeetingRepository.IsAttendedByAppNo(meetingId);
            return Ok(result);
        }

        [HttpGet("hasPassword")]
        public async Task<bool> MeetingHasPassword(string meetingId)
        {
            return await _IMeetingRepository.MeetingHasPassword(meetingId);
        }

        [HttpPost("anonymJWT")]
        public async Task<IActionResult> anonymJWT(string meetingId, string userName, string meetingPassword, [FromHeader] string lang)
        {
            object obj = await _IMeetingRepository.MeetingJWT(meetingId, null, userName, lang, meetingPassword);
            return Ok(obj);
        }


        [HttpPost("GetMeetingLogger")]
        public async Task<IActionResult> GetMeetingLogger()
        {
            await _IMeetingRepository.GetMeetingLogger();
            return Ok(true);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("moderatorJWT")]
        public async Task<IActionResult> moderatorJWT(string meetingId, [FromHeader] string lang)
        {
            object obj = await _IMeetingRepository.MeetingJWT(meetingId, _iUserRepository.GetUserID(), _iUserRepository.GetUserName(), lang, null);
            return Ok(obj);
        }
    }
}

