
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
    public class StageAttachmentController : ControllerBase
    {
        private readonly IStageMasterAttachmentRepository _IStageAttachmentRepository;
        private readonly IGeneralRepository _IGeneralRepository;
        private readonly ISysValueRepository _ISysValueRepository;

        public StageAttachmentController(IStageMasterAttachmentRepository iStageAttachRepository,
        IGeneralRepository iGeneralRepository, ISysValueRepository iSysValueRepository)
        {
            _IStageAttachmentRepository = iStageAttachRepository;
            _IGeneralRepository = iGeneralRepository;
            _ISysValueRepository = iSysValueRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost]
        public async Task<ActionResult> Post(List<postStageAttachmentDto> stageAttachDto)
        {
            List<StageMasterAttachment> stageMasterAttachments = await _IStageAttachmentRepository.add(stageAttachDto);

            if (stageMasterAttachments != null)
                return this.StatusCode(StatusCodes.Status200OK, stageMasterAttachments);
            else
                return this.StatusCode(StatusCodes.Status404NotFound, "No Attachment Saved to Stage");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetStagesAttachment")]
        public async Task<ActionResult> GetStagesAttachment(int id, [FromHeader] string lang)
        {
            var result = await _IStageAttachmentRepository.getstageattach(id, lang);
            if (result != null)
            {

                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetAllattachment")]
        public async Task<IActionResult> GetAllattachmentAsync([FromHeader] string lang)
        {
            var AttachmentType = await _ISysValueRepository.GetTypeAll(lang, "attachment_type");
            return Ok(AttachmentType);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            int result = await _IStageAttachmentRepository.delete(id);

            return result switch
            {
                Constants.OK => Ok(new { message = Constants.getMessage(lang, "sucsessDelete") }),
                Constants.NOT_FOUND => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete]
        public async Task<ActionResult> DeleteMany(int[] ids, [FromHeader] string lang)
        {
            List<int> result = await _IStageAttachmentRepository.DeleteMany(ids);
            return result.Count switch
            {
                0 => Ok(new { message = Constants.getMessage(lang, "sucsessDelete") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "faildDelete") + " " + string.Join(",", result) })
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, updateStageAttachDto stageAttachDto, [FromHeader] string lang)
        {
            int result = await _IStageAttachmentRepository.update(id, stageAttachDto);
            return result switch
            {
                -200 => Ok(new { message = Constants.getMessage(lang, "sucsessUpdate") }),
                -204 => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };

        }
    }
}
