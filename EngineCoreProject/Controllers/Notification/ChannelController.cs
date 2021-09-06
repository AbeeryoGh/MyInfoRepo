using EngineCoreProject.Services;
using EngineCoreProject.IRepository.IChannelRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using EngineCoreProject.DTOs.ChannelDto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace EngineCoreProject.Controllers.ChannelControler
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ChannelController : ControllerBase
    {
        private readonly IChannelRepository _iChannelRepository;
        private readonly IGeneralRepository _iGeneralRepository;
        private IConfiguration _configuration;

        public ChannelController(IChannelRepository iChannelRepository, IGeneralRepository iGeneralRepository, IConfiguration configuration)
        {
            _iChannelRepository = iChannelRepository;
            _iGeneralRepository = iGeneralRepository;
            _configuration = configuration;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet]
        public ActionResult GetChannelNames([FromHeader] string lang)
        {
            var result = _iChannelRepository.GetChannelsName(lang);
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }

            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetChannelMailFirstConfig")]
        public ActionResult ChannelMailFirstSetting()
        {
            var result = _iChannelRepository.GetChannelMailFirstConfig();
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }

            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("TestEmailFirstSettingConnection")]
        public ActionResult TestEmailFirstSettingConnection(ChannelMailFirstSetting channelMailFirstSetting, [FromHeader] string lang)
        {
            try
            {
                _iChannelRepository.TestEmailFirstSettingConnection(channelMailFirstSetting, lang);
                return this.StatusCode(StatusCodes.Status200OK, "successfully setting");
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("AddChannelMailFirstConfig")]
        public ActionResult AddChannelMailFirstConfig(ChannelMailFirstSetting channelMailFirstSetting)
        {
            _iChannelRepository.AddChannelMailFirstSetting(channelMailFirstSetting);
            var result = _iChannelRepository.GetChannelMailFirstConfig();
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }

            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }
    }
}
