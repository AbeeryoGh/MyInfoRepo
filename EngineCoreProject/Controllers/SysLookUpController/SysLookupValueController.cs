using EngineCoreProject.Services;
using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.SysLookUpDtos;
using EngineCoreProject.IRepository;
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
    public class SysLookupValueController : ControllerBase
    {
        private readonly ISysLookUpValueRepository _iSysLookUpValueRepository;
        private readonly IGeneralRepository _iGeneralRepository;

        public SysLookupValueController(ISysLookUpValueRepository iSysLookUpValueRepository, IGeneralRepository iGeneralRepository)
        {
            _iSysLookUpValueRepository = iSysLookUpValueRepository;
            _iGeneralRepository = iGeneralRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost]
        public async Task<ActionResult> Post(TranslationValueDto translationValueDto)
        {
            if (!await FindByTypeIdAsync(translationValueDto.TypeId)) return this.StatusCode(StatusCodes.Status404NotFound, "Cann't Find This Type");

            SysLookupValue NewValue = new SysLookupValue();
            NewValue.Shortcut = _iGeneralRepository.GenerateShortCut("SysLookupValue", "Shortcut"); //translationValueDto.shortcut;
            NewValue.LookupTypeId = translationValueDto.TypeId;
            _iGeneralRepository.Add(NewValue);
            if (await _iGeneralRepository.Save())
            {
                return this.StatusCode(StatusCodes.Status200OK, new
                {
                    Id = NewValue.Id,
                    TranslateValue = NewValue.Shortcut,
                    TranslateTypeId = Convert.ToInt32(NewValue.LookupTypeId)
                });
            }

            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPut("{id}")]
        public async Task<ActionResult<TranslationValueDto>> Put(int id, TranslationValueDto translationValueDto, [FromHeader] string lang)
        {
            if (!await FindByIdAsync(id)) return this.StatusCode(StatusCodes.Status404NotFound, "Not Found .....");
            SysLookupValue lookupValue = await _iSysLookUpValueRepository.FindTranslationValueById(id);

            if (!await FindByTypeIdAsync(translationValueDto.TypeId)) return this.StatusCode(StatusCodes.Status404NotFound, "Can't Find This Type .....");
            lookupValue.LookupTypeId = translationValueDto.TypeId;
            _iGeneralRepository.Update(lookupValue);
            if (await _iGeneralRepository.Save())
            {
                return this.StatusCode(StatusCodes.Status200OK, "updated successfully");
            }

            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetTranslationValues")]
        public async Task<ActionResult> GetTranslationValues([FromHeader] string lang)
        {
            var result = await _iSysLookUpValueRepository.GetTranslationValues(lang);
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetAllTranslationValues")]
        public async Task<ActionResult> GetAllTranslationValues()
        {
            var result = await _iSysLookUpValueRepository.GetAllTranslationValues( );
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetTranslationValueById([FromRoute] int id, [FromHeader] string lang)
        {
            var result = await _iSysLookUpValueRepository.FindTranslationValueByIdWithTrans(id,lang);
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id, [FromHeader] string lang)
        {
            SysLookupValue lookupValue = await _iSysLookUpValueRepository.FindTranslationValueById(id);
            if (lookupValue == null) return this.StatusCode(StatusCodes.Status200OK, "Not Found.....");
            _iGeneralRepository.Delete(lookupValue);
            if (await _iGeneralRepository.Save())
            {
                return this.StatusCode(StatusCodes.Status200OK, "deleted successfully");
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        private async Task<bool> FindByValueAsync(string value)
        {
            bool found = false;
            SysLookupValue lookupValue = await _iSysLookUpValueRepository.FindTranslationValueByValue(value);
            if (lookupValue != null) found = true;
            return found;
        }

        private async Task<bool> FindByIdAsync(int id)
        {
            bool found = false;
            SysLookupValue lookupValue = await _iSysLookUpValueRepository.FindTranslationValueById(id);
            if (lookupValue != null) found = true;
            return found;
        }

        private async Task<bool> FindByTypeIdAsync(int TypeId)
        {
            bool found = false;
            SysLookupType lookupValue = await _iSysLookUpValueRepository.FindTranslationValueBytypeId(TypeId);
            if (lookupValue != null) found = true;
            return found;
        }
    }
}
