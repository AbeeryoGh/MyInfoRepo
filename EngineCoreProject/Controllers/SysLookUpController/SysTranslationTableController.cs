using EngineCoreProject.Services;
using EngineCoreProject.DTOs.SysLookUpDtos;
using EngineCoreProject.IRepository.ISysLookUpRepository;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace EngineCoreProject.Controllers.SysLookUpController
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SysTranslationTableController : ControllerBase
    {
        private readonly ISysTranslationTableRepository _iSysTranslationTableRepository;
        private readonly IGeneralRepository _iGeneralRepository;
        public SysTranslationTableController(ISysTranslationTableRepository iSysTranslationTableRepository, ISysLookUpTypeRepository iSysLookUpTypeRepository, ISysLookUpValueRepository iSysLookUpValueRepository, IGeneralRepository iGeneralRepository)
        {
            _iSysTranslationTableRepository = iSysTranslationTableRepository;   
            _iGeneralRepository = iGeneralRepository;      
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost]
        public async Task<ActionResult<TranslationTableDto>> Post(TranslationTableDto translationTableDto, [FromHeader] string lang)
        {
            if(!await _iGeneralRepository.FindLanguage(translationTableDto.lang)) return this.StatusCode(StatusCodes.Status404NotFound, "this language not found"); ;
            if (!await _iGeneralRepository.FindShortCut(translationTableDto.shortcut)) return this.StatusCode(StatusCodes.Status404NotFound, "this shortcut not found"); ;

            if (await _iSysTranslationTableRepository.FindByShortCutAndLang(translationTableDto.shortcut, translationTableDto.lang)) return this.StatusCode(StatusCodes.Status404NotFound, "this shortcut was translated to "+ translationTableDto.lang+"  ..... you can't add another translation"); ;

            SysTranslation NewSysTranslationTable = new SysTranslation()
            {
                Shortcut = translationTableDto.shortcut,
                Lang = translationTableDto.lang,
                Value = translationTableDto.value
            };
            _iGeneralRepository.Add(NewSysTranslationTable);

            if (await _iGeneralRepository.Save())
            {
                return this.StatusCode(StatusCodes.Status200OK, NewSysTranslationTable);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet]
        public async Task<ActionResult> GetTranslationTableValues( [FromHeader] string lang)
        {
            var result = await _iSysTranslationTableRepository.GetTranslationTables();
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }

            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("{Id}")]
        public async Task<ActionResult> GetTranslationTableById([FromRoute] int Id)
        {
            var result = await _iSysTranslationTableRepository.FindTranslationTableById(Id);
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "Not Found......");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPut("{id}")]
        public async Task<ActionResult<TranslationTableDto>> Put(int id, TranslationTableDto translationTableDto, [FromHeader] string lang)
        {
            SysTranslation sysTranslationTable = await _iSysTranslationTableRepository.FindTranslationTableById(id);

            if (!await _iGeneralRepository.FindLanguage(translationTableDto.lang)) return this.StatusCode(StatusCodes.Status404NotFound, "this language not found");
            if (!await _iGeneralRepository.FindShortCut(translationTableDto.shortcut)) return this.StatusCode(StatusCodes.Status404NotFound, "this shortcut not found");
            sysTranslationTable.Lang = translationTableDto.lang;
            sysTranslationTable.Shortcut = translationTableDto.shortcut;
            sysTranslationTable.Value = translationTableDto.value;           
            _iGeneralRepository.Update(sysTranslationTable);
            if (await _iGeneralRepository.Save())
            {
                return this.StatusCode(StatusCodes.Status200OK, "Updated successfully");
            }

            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id, [FromHeader] string lang)
        {
            SysTranslation sysTranslationTable = await _iSysTranslationTableRepository.FindTranslationTableById(id);
            if(sysTranslationTable==null) return this.StatusCode(StatusCodes.Status404NotFound, "Can't Find......");
            _iGeneralRepository.Delete(sysTranslationTable);
            if (await _iGeneralRepository.Save())
            {
                return this.StatusCode(StatusCodes.Status200OK, "Deleted successfully");
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }
    }
}
