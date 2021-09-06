using EngineCoreProject.DTOs.EPOSMachineDto;
using EngineCoreProject.DTOs.SysLookUpDtos;
using EngineCoreProject.IRepository.IEPOSMachineRepository;
using EngineCoreProject.IRepository.ISysLookUpRepository;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.Models;
using EngineCoreProject.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.EPOSMachine
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class EPOSMachineController : ControllerBase
    {
        private readonly IEPOSMachine _iEPOSMachine;
        private readonly ISysLookUpValueRepository _iSysLookUpValueRepository;
        private readonly IUserRepository _IUserRepository;
      

        public EPOSMachineController(ISysLookUpValueRepository iSysLookUpValueRepository, IEPOSMachine iEPOSMachine, IUserRepository iUserRepository)
        {
            _iEPOSMachine = iEPOSMachine;
            _iSysLookUpValueRepository = iSysLookUpValueRepository;
            _IUserRepository = iUserRepository;
        }

        [HttpPost("MerchantLogin")]
        [AllowAnonymous]
        public async Task<ActionResult> MerchantLoginAsync([FromHeader] string lang, int UserId)
        {
            ResLoginDto result = await _iEPOSMachine.MerchantLoginAsync(lang, UserId);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("QueryServicePrice")]
        public async Task<ActionResult> QueryServicePriceAsync([FromHeader] string lang, ReqQueryPriceDto reqQueryPriceDto)
        {
            ResQueryPriceDto result = await _iEPOSMachine.QueryServicePriceAsync(reqQueryPriceDto, _IUserRepository.GetUserID(), lang);
            return Ok(result);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetCards")]
        public async Task<ActionResult> GetCards([FromHeader] string lang)
        {
            EPOSCardsEmployeeToken ePOSCardsEmployeeToken = new EPOSCardsEmployeeToken();
            string type = "CardsType";
            var tokenSetting = await _iEPOSMachine.QueryTokenAsync(lang);
            ePOSCardsEmployeeToken.Cards = await _iSysLookUpValueRepository.GetTranslationValuesForType(lang, type);
            ePOSCardsEmployeeToken.EposToken = tokenSetting.sessionToken;
            ePOSCardsEmployeeToken.ExpiredTokenTime = (DateTime)tokenSetting.tokenExpiryDate;

            return Ok(ePOSCardsEmployeeToken);
        }
    }
}
