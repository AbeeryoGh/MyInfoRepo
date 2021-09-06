using EngineCoreProject.DTOs.UnifiedGateDto;
using EngineCoreProject.DTOs.UnifiedGatePostDto;
using EngineCoreProject.IRepository.IUnifiedGateSubServicesRepository;
using EngineCoreProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;
using Newtonsoft.Json.Linq;
using Service_UAEPassCall;
using SMART_khadamatiSubServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace EngineCoreProject.Controllers.UnifiedGate
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UnifiedGateController : ControllerBase
    {

        private readonly IUnifiedGateSubServicesRepository _IUnifiedGateSubServicesRepository;
        public UnifiedGateController(IUnifiedGateSubServicesRepository iUnifiedGateSubServicesRepository)
        {
            _IUnifiedGateSubServicesRepository = iUnifiedGateSubServicesRepository;
        }

        [HttpPost("statisticsDashboard")]
        public async Task<ActionResult> statisticsDashboard(DashBoardRequestDto dashBoardRequestDto)
        {
            var result = await _IUnifiedGateSubServicesRepository.statisticsDashboard(dashBoardRequestDto);
            switch (result.status)
            {
                case 1:
                    return StatusCode(StatusCodes.Status200OK, result.responseCardDashBoard);
                case 0:
                    return StatusCode(StatusCodes.Status404NotFound, "User Not Found");
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error! Please try again");
            }
        }
        [HttpPost("statisticsTable")]
        public async Task<ActionResult> statisticsTableAsync(DashBoardRequestDto dashBoardRequestDto)
        {
            var result = await _IUnifiedGateSubServicesRepository.statisticsTable(dashBoardRequestDto);
            switch (result.status)
            {
                case 0:
                    return StatusCode(StatusCodes.Status404NotFound, "User Not Found");
                case 1:
                    return StatusCode(StatusCodes.Status200OK, result.responseTableDashBoard);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error! Please try again");
            }
        }

        [HttpGet("GetSubServices")]
        public ActionResult GetSubServices()
        {
            UnifiedGateSubServicesDto SubServicesDescription = _IUnifiedGateSubServicesRepository.GetSubServicesFromUnifiedGate();
            return Ok(SubServicesDescription);
        }

        [HttpGet("GetSubServiceDetials/{id}")]
        public async Task<ActionResult> GetSubServiceDetials(int id)
        {
            var subServicesDescription = _IUnifiedGateSubServicesRepository.GetSubServicesFromUnifiedGate();

            if (subServicesDescription !=null && subServicesDescription.SubServices != null)
            {
                var serviceRequired = subServicesDescription.SubServices.Where(x => x.SubServiceCode == id).FirstOrDefault();
                if (serviceRequired != null)
                {
                    var admService = await _IUnifiedGateSubServicesRepository.GetServiceByUID(id);
                    if (admService != null)
                    {
                        serviceRequired.SubServiceManualAr = admService.GuidFilePathAr;
                        serviceRequired.SubServiceManualEn = admService.GuidFilePathEn;
                    }
                }

                return Ok(serviceRequired);
            }
            return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [HttpPost("GetUserInformation")]
        public ActionResult GetUserInformation(UnifiedGateUserInformationPostDto userInfo)
        {
            UnifiedGateUserInformationDto SubServicesDescription = _IUnifiedGateSubServicesRepository.GetUserInformationUnifiedGate(userInfo);

            if (SubServicesDescription != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, SubServicesDescription);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }
       
        [HttpPost("SignInWithUGate")]
        
        public ActionResult SignInWithUGate(string lang, string EL_Service, string theme)
        {
            string URL = _IUnifiedGateSubServicesRepository.GenerateURLForSignInWithUGate(lang, EL_Service, theme);

            if (URL != null)
            {

                return this.StatusCode(StatusCodes.Status200OK, URL);


            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("RedirectToUGDahBoard")]
        public ActionResult RedirectToUGDahBoard(string lang, string theme)
        {
            string URL = _IUnifiedGateSubServicesRepository.RedirectToUGDahBoard(lang, theme);

            if (URL != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, URL); ;
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [HttpPost("SignOut")]
        public ActionResult SignOut()//string lang, string EL_Eid
        {
            string URL = _IUnifiedGateSubServicesRepository.RemoteSignOut();//lang,EL_Eid
            if (URL != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, URL);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [HttpPost("testAuthrized")]
        public ActionResult testAuthrized()//string lang, string EL_Eid
        {
            bool Authrized = User.Identity.IsAuthenticated;
            return this.StatusCode(StatusCodes.Status200OK, Authrized);
        }
    }
}
