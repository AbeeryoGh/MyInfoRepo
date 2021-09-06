using EngineCoreProject.DTOs.AdmService;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.AdmServices;
using EngineCoreProject.Models;
using EngineCoreProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.Service
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class StageActionController : ControllerBase
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IStageActionsRepository _IStageActionRepository;
        private readonly IGeneralRepository _IGeneralRepository;
        private readonly ISysValueRepository _ISysValueRepository;

        public StageActionController(IStageActionsRepository iStageActionRepository,
        IGeneralRepository iGeneralRepository, EngineCoreDBContext EngineCoreDBContext, ISysValueRepository iSysValueRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _IStageActionRepository = iStageActionRepository;
            _IGeneralRepository = iGeneralRepository;
            _ISysValueRepository = iSysValueRepository;
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetAllActionType")]
        public async Task<IActionResult> GetAllActionTypeAsync([FromHeader] string lang)
        {
            var ActionTyp = await _ISysValueRepository.GetTypeAll(lang, "action_type");
            return Ok(ActionTyp);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost]
        public async Task<ActionResult> Post(postActionDto stageActionDto)
        {
            AdmStageAction admStageActions = await _IStageActionRepository.Add(stageActionDto);
            if (admStageActions != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, "Actions Add Successfully To Stage");
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred .... ");
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetStagesAction")]
        public async Task<ActionResult> GetStagesAction(int id, [FromHeader] string lang)
        {
            var result = await _IStageActionRepository.GetStageAction(id, lang);
            if (result != null)
            {

                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            int result = await _IStageActionRepository.Delete(id);
            return result switch
            {
                Constants.OK => Ok(new { message = Constants.getMessage(lang, "sucsessDelete") }),
                Constants.NOT_FOUND => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("getActionRole")]
        public async Task<ActionResult> Role([FromBody] List<int> actions, [FromQuery] string lang)
        {
            return Ok(await _IStageActionRepository.GetActionstoRole(actions, lang));
        }
    }
}
