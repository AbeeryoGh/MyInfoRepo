using EngineCoreProject.Services;
using EngineCoreProject.DTOs.TemplateSetDtos;
using EngineCoreProject.IRepository.ITemplateSetRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace EngineCoreProject.Controllers.TemplateSet
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TermsController : ControllerBase
    {
        private readonly ITermRepository _ITermRepository;
        private readonly IGeneralRepository _IGeneralRepository;

        public TermsController(ITermRepository iTermRepository, IGeneralRepository iGeneralRepository)
        {
            _ITermRepository = iTermRepository;
            _IGeneralRepository = iGeneralRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet]
        public async Task<ActionResult> All([FromQuery] int? templateId, [FromHeader] string lang)
        {
            try
            {
                var result = await _ITermRepository.GetAll(templateId);
                return Ok(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,//e.Message.ToString()
                new { message = Constants.getMessage(lang, "UnknownError") }
                        );
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("{id}")]
        public async Task<ActionResult> One([FromRoute] int id, [FromHeader] string lang)
        {
            try
            {
                var result = await _ITermRepository.GetOne(id);
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
        public async Task<IActionResult> Put(int id, [FromBody] TermDto termDto, [FromHeader] string lang)
        {
            int result = await _ITermRepository.Update(id, termDto);
            return result switch
            {
                Constants.OK => Ok(new { message = Constants.getMessage(lang, "sucsessUpdate") }),
                Constants.NOT_FOUND => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TermDto termDto, [FromHeader] string lang)
        {
            int result = await _ITermRepository.Add(termDto);
            return result switch
            {
                Constants.ERROR => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
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
            int result = await _ITermRepository.DeleteOne(id);
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
            List<int> result = await _ITermRepository.DeleteMany(ids);
            return result.Count switch
            {
                0 => Ok(new { message = Constants.getMessage(lang, "sucsessDelete") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "faildDelete") + " " + string.Join(",", result) })
            };
        }
    }
}
