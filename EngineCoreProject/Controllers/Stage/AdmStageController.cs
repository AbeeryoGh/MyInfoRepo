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
    public class AdmStageController : ControllerBase
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IAdmStageRepository _IAdmStageRepository;
        private readonly IGeneralRepository _IGeneralRepository;
        private readonly ISysValueRepository _ISysValueRepository;

        public AdmStageController(IAdmStageRepository iStageRepository,
        IGeneralRepository iGeneralRepository, EngineCoreDBContext EngineCoreDBContext, ISysValueRepository iSysValueRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _IAdmStageRepository = iStageRepository;
            _IGeneralRepository = iGeneralRepository;
            _ISysValueRepository = iSysValueRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetAllstagetype")]
        public async Task<IActionResult> GetAllstagetypeAsync(string lang)
        {
            var StageType = await _ISysValueRepository.GetTypeAll(lang, "stage_type");
            return Ok(StageType);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet]
        public async Task<ActionResult> GetStages([FromHeader] string lang)
        {
            var result = await _IAdmStageRepository.GetStageNames(lang);
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("{Id}")]
        public async Task<ActionResult> GetStageById(int Id, [FromHeader] string lang)
        {           
            var result = await _IAdmStageRepository.getonestage(Id, lang);
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost]
        public async Task<ActionResult> Post(postStageDto stageDto)
        {
            AdmStage admStage = await _IAdmStageRepository.add(stageDto);
             if (admStage!=null)
             {
                 return this.StatusCode(StatusCodes.Status200OK, new {admStage.ServiceId,admStage.Id,admStage.Shortcut}
                 );
             }
             else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred .... No Service Found");         
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, updatetStageDto stageDto, [FromHeader] string lang)
        {
            int result = await _IAdmStageRepository.update(id, stageDto);
            return result switch
            {
                -200 => Ok(new { message = Constants.getMessage(lang, "sucsessUpdate") }),
                -204 => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            int result = await _IAdmStageRepository.delete(id,lang);
            return result switch
            {
                Constants.OK => Ok(new { message = Constants.getMessage(lang, "sucsessDelete") }),
                Constants.NOT_FOUND => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }
    }
}
