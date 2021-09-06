using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.MyApplicationDto;
using EngineCoreProject.IRepository.IMyApplications;
using EngineCoreProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;


namespace EngineCoreProject.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MyApplicationController :ControllerBase
    {
        private readonly IMyApplicationRepository _IMyApplicationRepository;
        public MyApplicationController(IMyApplicationRepository iMyApplicationRepository)
        {
            _IMyApplicationRepository = iMyApplicationRepository;
            

        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("SearchPages")]
        public async Task<ActionResult> searchpage([FromHeader] string lang, searchDto searchDto, int currentpage,int perpage)
        {
            try
            {
                var result = await _IMyApplicationRepository.SearchUserPages(lang, searchDto, currentpage,perpage);
                return Ok(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error");
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("SearchByStage")]
        public async Task<ActionResult> searchBystage([FromHeader] string lang, searchDto searchDto, int currentpage, int perpage)
        {
            try
            {
                var result = await _IMyApplicationRepository.SearchUserByStage(lang, searchDto, currentpage, perpage);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error  " + ex.Message);
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("SearchAllApp")]
        public async Task<ActionResult> SearchAllAppStage([FromHeader] string lang, searchDto searchDto, int currentpage, int perpage)
        {
            try
            {
                var result = await _IMyApplicationRepository.SearchEverything(lang, searchDto, currentpage, perpage);
                return Ok(result);
            }
            catch (Exception ex )
            {  
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error   " + ex.Message);
            }
        }
       



        

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("userAppByStage")]
        public async Task<ActionResult> userAppByStage([FromHeader] string lang,searchDto searchDto,int currentpage,int perpage)
        {
            //string lang_ = Request.Headers["lang"].ToString().ToLower();
            try
            {
                var result = await _IMyApplicationRepository.userAppsBystage(searchDto, lang,currentpage,perpage);
                return Ok(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("userAllApps")]
        public async Task<ActionResult> userApp( [FromHeader] string lang, searchDto searchDto, int currentPage, int perPage)
        {
            try
            {
                var result = await _IMyApplicationRepository.userAllApps(searchDto, lang,currentPage,perPage);
                return Ok(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error");
            }
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("GetStageId")]
        public async Task<ActionResult> GetID([FromHeader] string lang, string value)
        {
            try
            {
                var result = await _IMyApplicationRepository.getShortCutStageByTranslate( lang, value );
                return Ok(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error");
            }
        }


        [HttpPost("VerifyTransaction")]
        public async Task<ActionResult> VerifyTransaction(VerifyDto verifyDto,string lang)
        {
             var result =  _IMyApplicationRepository.VerifyTransaction(verifyDto,lang);
                return Ok(result);
          
        }


        
    }
}
                                                                