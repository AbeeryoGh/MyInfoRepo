using EngineCoreProject.DTOs.TabDto;
using EngineCoreProject.IRepository.ITabRepository;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.TabController
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class TabController : ControllerBase
    {
        private readonly ITabRepository _iTabRepository;
        private readonly IUserRepository _iuserRepository;
        public TabController(ITabRepository iTabRepository, IUserRepository iuserRepository)
        {
            _iTabRepository = iTabRepository;
            _iuserRepository = iuserRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("AddTab")]
        public async Task<ActionResult> AddTab([FromBody] TabPostDto tabDto, [FromHeader] string lang)
        {
            var result = await _iTabRepository.AddTab(tabDto);
            return result != 0
                ? StatusCode(StatusCodes.Status200OK, result)
                : StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPut("updateTabById")]
        public async Task<ActionResult> Update([FromBody] TabPostDto tabDto, int id, [FromHeader] string lang)
        {
                var result = await _iTabRepository.UpdateTab(tabDto, id);
            return result != 0
                ? StatusCode(StatusCodes.Status200OK, result)
                : StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            var result = await _iTabRepository.DeleteTab(id, lang);
            return result != 0
                ? StatusCode(StatusCodes.Status200OK, result)
                : StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> Get([FromHeader] string lang)
        {
            var result = await _iTabRepository.GetTabs(lang);
            return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetMyTabs")]
        public async Task<ActionResult> GetMyTabs([FromHeader] string lang)
        {
            var result = await _iTabRepository.GetMyTabs(_iuserRepository.GetUserID(), lang);
            return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        // TODO should be only for admin.
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("UpdateIconTab")]
        public async Task<ActionResult> UpdateIconTab([FromForm] FromFileDto file, int rowId)
        {
            var result = await _iTabRepository.UpdateIconTab(file.IconImage, rowId);
            return result != 0 ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        // TODO should be only for admin.
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("UpdateIconStringTab")]
        public async Task<ActionResult> UpdateIconStringTab(string iconString, int rowId)
        {
            var result = await _iTabRepository.UpdateIconStringTab(iconString, rowId);
            return result != 0 ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }
    }
}
