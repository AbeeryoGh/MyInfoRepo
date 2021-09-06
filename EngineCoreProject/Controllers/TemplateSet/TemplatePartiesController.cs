using EngineCoreProject.Services;
using EngineCoreProject.DTOs.TemplateSetDtos;
using EngineCoreProject.IRepository.ITemplateSetRepository;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace EngineCoreProject.Controllers.TemplateSet
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TemplatePartiesController : ControllerBase
    {
        private readonly ITemplatePartyRepository _ITemplatePartyRepository;
        private readonly IGeneralRepository _IGeneralRepository;
        public TemplatePartiesController(ITemplatePartyRepository iTemplatePartyRepository,
                                             IGeneralRepository iGeneralRepository)
        {
            _ITemplatePartyRepository = iTemplatePartyRepository;
            _IGeneralRepository = iGeneralRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> All([FromQuery] int? templateId, [FromHeader] string lang)
        {
            try
            {
                var result = await _ITemplatePartyRepository.GetAll(templateId);
                return Ok(result);
            }
            catch
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                new { message = Constants.getMessage(lang, "UnknownError") }
                      );
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task<ActionResult> One([FromRoute] int id)
        {
            string lang = Request.Headers["lang"].ToString().ToLower();
            string Authorization = Request.Headers["Authorization"].ToString();
            Response.Headers.Add("lang", lang);
            try
            {
                var result = await _ITemplatePartyRepository.GetOne(id);
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
        public async Task<IActionResult> Put(int id, [FromBody] TemplatePartyDto templatePartyDto)
        {
            string lang = Request.Headers["lang"].ToString().ToLower();
            int result = await _ITemplatePartyRepository.Update(id, templatePartyDto);
            return result switch
            {
                -200 => Ok(new { message = Constants.getMessage(lang, "sucsessUpdate") }),
                -204 => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TemplatePartyDto templatePartyDto, [FromHeader] string lang)
        {
            int result = await _ITemplatePartyRepository.Add(templatePartyDto);
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
            int result = await _ITemplatePartyRepository.DeleteOne(id);
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
            List<int> result = await _ITemplatePartyRepository.DeleteMany(ids);
            return result.Count switch
            {
                0 => Ok(new { message = Constants.getMessage(lang, "sucsessDelete") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "faildDelete") + " " + string.Join(",", result) })
            };
        }

    }
}
