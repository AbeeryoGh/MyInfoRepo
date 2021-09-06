using EngineCoreProject.Services;
using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.SysLookUpDtos;
using EngineCoreProject.DTOs.TemplateSetDtos;
using EngineCoreProject.IRepository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace EngineCoreProject.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SysValuesController : ControllerBase
    {
        private readonly ISysValueRepository _ISysValueRepository;

        public SysValuesController(ISysValueRepository iSysValueRepository)
        {
            _ISysValueRepository = iSysValueRepository;

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("PostRecord")]
        public async Task<ActionResult> AddRecord([FromBody] TableField idValue)
        {
            string result = null;
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            result = await _ISysValueRepository.AddRecord(idValue.Id, 
                                                          idValue.TableName,
                                                          idValue.FieldName,
                                                          idValue.ParentId,
                                                          idValue.ParentFieldName);

            return result switch
            {
                null => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
                _ => Ok(new
                {
                    message = Constants.getMessage(lang, "sucsessAdd"),
                    shortcut = result
                })
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [Route("PostTranslation")]
        [HttpPost]
        public async Task<ActionResult> AddTranslation([FromBody] List<TranslationDto> translationDtos)
        {
            List<int> l = new List<int>();
            foreach (var translationDto in translationDtos)
            {
                string result = await _ISysValueRepository.AddTranslation(translationDto);
                l.Add(Int32.Parse(result));
            }

            return Ok(l);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{type}")]
        public async Task<List<TypeList>> GetTypeAll([FromQuery] string lang, [FromRoute] string type)
        {
            return await _ISysValueRepository.GetTypeAll(lang, type);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("Shortcut/{shortcut}")]
        public async Task<int> GetShortcutId([FromRoute] string shortcut)
        {
            return await _ISysValueRepository.GetIdByShortcut(shortcut);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("Countries")]
        public async Task<List<CountryDto>> GetCountries([FromQuery] string lang)
        {
            return await _ISysValueRepository.GetAllCountry(lang);
        }
    }
}
