using EngineCoreProject.Services;
using EngineCoreProject.DTOs.ActionDtos;
using EngineCoreProject.DTOs.NotificationDtos;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using EngineCoreProject.Models;
using EngineCoreProject.DTOs.ChannelDto;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using EngineCoreProject.IRepository.AdmServices;
using EngineCoreProject.DTOs.AdmService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace EngineCoreProject.Controllers.Actions
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class ActionController : ControllerBase
    {
        private readonly IStageActionsRepository _IStageActionsRepository;

        public ActionController(IStageActionsRepository iStageActionsRepository)
        {
            _IStageActionsRepository = iStageActionsRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetStageActionDetials/{id}")]
        public async Task<ActionResult> GetStageActionDetials([FromRoute] int id, [FromHeader] string lang)
        {
            List<ActionDetialsForNotification> result = await _IStageActionsRepository.GetActionDetialsForNotification(id, lang);
            return this.StatusCode(StatusCodes.Status200OK, result);
        }

    }
}
