using EngineCoreProject.Services;
using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.SysLookUpDtos;
using EngineCoreProject.IRepository.ISysLookUpRepository;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EngineCoreProject.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private readonly ISysLookUpValueRepository _iSysLookUpValueRepository;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        public GeneralController(EngineCoreDBContext EngineCoreDBContext, ISysLookUpValueRepository iSysLookUpValueRepository, IGeneralRepository iGeneralRepository)
        {
            _iSysLookUpValueRepository = iSysLookUpValueRepository;
            _iGeneralRepository = iGeneralRepository;
            _EngineCoreDBContext = EngineCoreDBContext;
        }

       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("MyDevice")]
        public async Task<ActionResult> myDevice()
        {
            var userAgent = Request.Headers["User-Agent"];
            int description = _iGeneralRepository.AppDevice(userAgent);
            return Ok(description);

        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("[Action]")]
        public async Task<ActionResult> insertTranslation(string shortcut, string value, [FromHeader] string lang)
        {
            var result = await _iGeneralRepository.insertTranslation(shortcut, value, lang);
            if (result != null)
            {

                //return this.StatusCode(StatusCodes.Status200OK,result);
                return Ok(result);

            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("getTranslationsForShortcut")]
        public async Task<ActionResult> getTranslationsForShortcut(string shortcut)
        {
            var result = await _iGeneralRepository.getTranslationsForShortCut(shortcut);
            if (result != null)
            {
                return Ok(result);
            }

            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("[Action]")]
        public async Task<ActionResult> insertListTranslation(List<TranslationDto> TranslationList)
        {
            var result = await _iGeneralRepository.insertListTranslationAsync(TranslationList);
            if (result != null)
            {
                return Ok(result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("[Action]")]
        public async Task<ActionResult> updateTranslation(string language, string value, string shortCut)
        {
            var result = await _iGeneralRepository.updateTranslation(language, value, shortCut);
            if (result != null)
            {
                return Ok(result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("AllTranslation")]
        public async Task<ActionResult> GetAllTranslations(string shortcut)
        {
            var result = await _iGeneralRepository.GetAllTranslation(shortcut);
            if (result != null)
            {
                return Ok(result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("ConvertFromHTMLTpPDF")]
        public ActionResult ConvertFromHTMLTpPDF([FromBody] string HTML)
        {
            string FileName = _iGeneralRepository.ConvertFromHTMLTpPDF(HTML);
            if (FileName != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, FileName);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("SecureHashGenerationHMACSHA256")]
        public ActionResult SecureHashGenerationHMACSHA256([FromHeader] string StringForHash, [FromHeader] string Key)
        {
            string secureHash = _iGeneralRepository.SecureHashGenerationHMACSHA256(StringForHash, Key);
            if (secureHash != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, secureHash);
            }

            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }


        [HttpPost("GenerateURLWithQueryString")]
        public ActionResult GenerateURLWithQueryString(string URL,[FromBody]Dictionary<string,string> QueryStrings)
        {
            string secureHash = _iGeneralRepository.GenerateURL(QueryStrings, URL);
            
                return this.StatusCode(StatusCodes.Status200OK, secureHash);
            
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("getShortCutID")]
        public async Task<ActionResult> getShortCutID(string shortcut)
        {
            var result = await _iGeneralRepository.getShortCutId(shortcut);
            if (result != null)
            {
                return Ok(result);
            }

            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }
    }
}
