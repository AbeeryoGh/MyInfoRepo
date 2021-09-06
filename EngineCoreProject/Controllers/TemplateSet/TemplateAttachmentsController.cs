using EngineCoreProject.IRepository.ITemplateSetRepository;
using EngineCoreProject.Services;
using EngineCoreProject.DTOs.TemplateSetDtos;


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace EngineCoreProject.Controllers.TemplateSet
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TemplateAttachmentsController : ControllerBase
    {
        private readonly ITemplateAttachmentRepository _ITemplateAttachmentRepository;
        private readonly IGeneralRepository _IGeneralRepository;
        public TemplateAttachmentsController(ITemplateAttachmentRepository iTemplateAttachmentRepository,
                                             IGeneralRepository iGeneralRepository)
        {
            _ITemplateAttachmentRepository = iTemplateAttachmentRepository;
            _IGeneralRepository = iGeneralRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> All([FromQuery] int? templateId, [FromHeader] string lang)
        {
            try
            {
                var result = await _ITemplateAttachmentRepository.GetAll(templateId);
                return Ok(result);
            }
            catch
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,//e.Message.ToString()
                new { message = Constants.getMessage(lang, "UnknownError") }
                      );
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task<ActionResult> One([FromRoute] int id, [FromHeader] string lang)
        {
            try
            {
                var result = await _ITemplateAttachmentRepository.GetOne(id);
                if (result != null)
                    return Ok(result);
                else
                    return NotFound(new { message = Constants.getMessage(lang, "zeroResult") });
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                      new { message = Constants.getMessage(lang, "UnknownError") });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TemplateAttachmentDto templateAttachmentDto, [FromHeader] string lang)
        {
            int result = await _ITemplateAttachmentRepository.Update(id, templateAttachmentDto);
            return result switch
            {
                -200 => Ok(new { message = Constants.getMessage(lang, "sucsessUpdate") }),
                -204 => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TemplateAttachmentDto templateAttachmentDto, [FromHeader] string lang)
        {
            int result = await _ITemplateAttachmentRepository.Add(templateAttachmentDto);
            return result switch
            {
                -500 => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
                _ => Ok(new
                {
                    message = Constants.getMessage(lang, "sucsessAdd"),
                    id = result
                })
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            int result = await _ITemplateAttachmentRepository.DeleteOne(id);
            return result switch
            {
                -200 => Ok(new { message = Constants.getMessage(lang, "sucsessDelete") }),
                -204 => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete]
        public async Task<ActionResult> DeleteMany(int[] ids, [FromHeader] string lang)
        {
            List<int> result = await _ITemplateAttachmentRepository.DeleteMany(ids);
            return result.Count switch
            {
                0 => Ok(new { message = Constants.getMessage(lang, "sucsessDelete") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "faildDelete") + " " + string.Join(",", result) })
            };
        }
    }
}
