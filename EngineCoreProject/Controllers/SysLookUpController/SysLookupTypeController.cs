using EngineCoreProject.Services;

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
    public class SysLookupTypeController : ControllerBase
    {
        private readonly ISysLookUpTypeRepository _iSysLookUpTypeRepository;
        private readonly IGeneralRepository _iGeneralRepository;

        public SysLookupTypeController(ISysLookUpTypeRepository iSysLookUpTypeRepository, IGeneralRepository iGeneralRepository)
        {
            _iSysLookUpTypeRepository = iSysLookUpTypeRepository;
            _iGeneralRepository = iGeneralRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost]
        public async Task<ActionResult<TranslationTypeDtoGet>> Post(TranslationTypeDto translationTypeDto, [FromHeader] string lang)
        {
            if (await FindByValueAsync(translationTypeDto.shortcut)) return this.StatusCode(StatusCodes.Status404NotFound, "repeated value.....");

            SysLookupType NewType = new SysLookupType();
            NewType.Value = translationTypeDto.shortcut;
            _iGeneralRepository.Add(NewType);

            if (await _iGeneralRepository.Save())
            {

                return this.StatusCode(StatusCodes.Status200OK, new TranslationTypeDtoGet
                {
                    shortcut = NewType.Value,
                    TypeID = NewType.Id
                });
            }

            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPut("{id}")]
        public async Task<ActionResult<TranslationTypeDto>> Put(int id, TranslationTypeDto translationTypeDto, [FromHeader] string lang)
        {
            if (!await FindByIdAsync(id)) return this.StatusCode(StatusCodes.Status404NotFound, "Not Found .....");
            if (await FindByValueAsync(translationTypeDto.shortcut)) return this.StatusCode(StatusCodes.Status200OK, "repeated value.....");

            SysLookupType lookupType = await _iSysLookUpTypeRepository.FindTranslationTypeById(id);
            lookupType.Value = translationTypeDto.shortcut;
            _iGeneralRepository.Update(lookupType);
            if (await _iGeneralRepository.Save())
            {
                return this.StatusCode(StatusCodes.Status200OK, "updated successfully");
            }

            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetTranslationTypes")]
        public async Task<ActionResult> GetTranslationTypes([FromHeader] string lang)
        {
            var result = await _iSysLookUpTypeRepository.GetTranslationTypes(lang);
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetAllTranslationTypes")]
        public async Task<ActionResult> GetAllTranslationTypes()
        {
            var result = await _iSysLookUpTypeRepository.GetAllTranslationTypes();
            if (result != null)
            {

                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetTranslationTypeById([FromRoute] int id, [FromHeader] string lang)
        {
            var result = await _iSysLookUpTypeRepository.GetTranslationTypesByID(id, lang);
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
            SysLookupType lookupType = await _iSysLookUpTypeRepository.FindTranslationTypeById(id);
            if (lookupType == null) return this.StatusCode(StatusCodes.Status200OK, "Not Found.....");
            _iGeneralRepository.Delete(lookupType);
            if (await _iGeneralRepository.Save())
            {
                return this.StatusCode(StatusCodes.Status200OK, "deleted successfully");
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        private async Task<bool> FindByValueAsync(string value)
        {
            bool found = false;
            SysLookupType lookupType = await _iSysLookUpTypeRepository.FindTranslationTypeByType(value);
            if (lookupType != null) found = true;
            return found;
        }

        private async Task<bool> FindByIdAsync(int id)
        {
            bool found = false;
            SysLookupType lookupType = await _iSysLookUpTypeRepository.FindTranslationTypeById(id);
            if (lookupType != null) found = true;
            return found;
        }
    }
}
