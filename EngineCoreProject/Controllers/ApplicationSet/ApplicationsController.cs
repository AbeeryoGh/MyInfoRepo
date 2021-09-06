using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.AdmService;
using EngineCoreProject.DTOs.AdmService.ModelView;
using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.TemplateSetDtos.ModelView;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.AdmServices;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.IRepository.IGeneratorRepository;
using EngineCoreProject.IRepository.INotificationSettingRepository;
using EngineCoreProject.IRepository.IPaymentRepository;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.IRepository.TemplateSetRepository;
using EngineCoreProject.Models;
using EngineCoreProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static EngineCoreProject.Services.Constants;


namespace EngineCoreProject.Controllers.ApplicationSet
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationRepository  _IApplicationRepositiory;
        private readonly IAdmServiceRepository   _IAdmServiceRepository;
        private readonly ISysValueRepository     _ISysValueRepository;
        private readonly ITemplateRepository     _ITemplateRepository;
        private readonly ITransactionRepository  _ITransactionRepository;
        private readonly IApplicationTrackRepository    _IApplicationTrackRepository;
        private readonly INotificationSettingRepository _INotificationSettingRepository;
        private readonly IUserRepository    _IUserRepository;
        private readonly IPaymentRepository _IPaymentRepository;
        private readonly IGeneralRepository _IGeneralRepository;
        private readonly IGenerator _IGenerator;
        private readonly IConfiguration _IConfiguration;
        private readonly EngineCoreDBContext _EngineCoreDBContext;

        public ApplicationsController(IApplicationRepository iApplicationRepositiory, IAdmServiceRepository iAdmServiceRepositiory, 
              ISysValueRepository iSysValueRepository, ITemplateRepository iTemplateRepository, ITransactionRepository iTransactionRepository,
              IApplicationTrackRepository iApplicationTrackRepository, INotificationSettingRepository  iNotificationSettingRepository,
              IUserRepository iUserRepository, IPaymentRepository iPaymentRepository,  IGeneralRepository iGeneralRepository, IGenerator iGenerator, IConfiguration iConfiguration)
        {
            _IApplicationRepositiory = iApplicationRepositiory;
            _IAdmServiceRepository  = iAdmServiceRepositiory;
            _ISysValueRepository     = iSysValueRepository;
            _ITemplateRepository     = iTemplateRepository;
            _ITransactionRepository  = iTransactionRepository;
            _IApplicationTrackRepository    = iApplicationTrackRepository;
            _INotificationSettingRepository = iNotificationSettingRepository;
            _IUserRepository    = iUserRepository;
            _IPaymentRepository = iPaymentRepository;
            _IGeneralRepository = iGeneralRepository;
            _IGenerator = iGenerator;
            _IConfiguration=iConfiguration;
    }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> All([FromQuery] int? serviceId, [FromHeader] string lang)
        {
            try
            {
                var result = await _IApplicationRepositiory.GetAll(serviceId);
                return Ok(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                new { message = getMessage(lang, "UnknownError") });
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task<ActionResult> One([FromRoute] int id, [FromHeader] string lang)
        {
            try
            {
                var result = await _IApplicationRepositiory.GetOne(id);
                if (result != null)
                    return Ok(result);
                else
                    return NotFound(new { message = getMessage(lang, "zeroResult") });
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                      new { message = getMessage(lang, "UnknownError") });
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("StagesWithCurrent/{appId}")]
        public async Task<ActionResult> CurrentStage([FromRoute] int appId, [FromQuery] int serviceId, [FromQuery] string lang)
        {

            try
            {
                var result = await _IApplicationRepositiory.getStageOfService(appId, serviceId, lang);
                return Ok(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                      new { message = Constants.getMessage(lang, "UnknownError") });
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("StageActions/{stageId}")]
        public async Task<ActionResult> Actions([FromRoute] int stageId, [FromQuery] string lang)
        {
            try
            {
                var result = await _IApplicationRepositiory.getActions(stageId, lang);
                return Ok(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                      new { message = Constants.getMessage(lang, "UnknownError") });
            }
        }

      
      
        //-------*** Approval / Sign | Approve ***-----------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("StageForward")]
        public async Task<IActionResult> SetStageForward([FromBody] FullUpdate fu, [FromQuery] int actionId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userId = Convert.ToInt32(claim[0].Value);
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            APIResult result = await _IApplicationRepositiory.StageForward(fu, actionId,  userId, lang);
            return Ok(result);        

        }

        //-------*** Approval / Sign | Backword ***-----------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("StageBackward")]
        public async Task<IActionResult> SetStageBackward([FromBody] FullUpdate fu, [FromQuery] int actionId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int userId = Convert.ToInt32(claim[0].Value);
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            APIResult result = await _IApplicationRepositiory.StageBackward(fu, actionId, userId, lang);
            return Ok(result);
        }

        /*   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
           [HttpPut("StageForward/Signing")]
           public async Task<IActionResult> SetStageForwardSigning([FromBody] FullUpdate fullUpdate)
           {
               var identity = HttpContext.User.Identity as ClaimsIdentity;
               IList<Claim> claim = identity.Claims.ToList();
               int UserId = Convert.ToInt32(claim[0].Value);
               StagePayload stagePayload = new StagePayload();
               stagePayload.application = fullUpdate.toUpdate.applicationDto;
               stagePayload.trackDto    = fullUpdate.toSave.applicationTrackDto;
               string lang = Request.Headers["lang"].ToString().ToLower();
               Response.Headers.Add("lang", lang);
               try
               {

                   var result = await _IApplicationRepositiory.SetStageForward(stagePayload, UserId);
                   int kind   = await _IAdmServiceRepository.GetKindNo((int)fullUpdate.toUpdate.applicationDto.ServiceId);
                   List<int> parties = await _IApplicationRepositiory.GetPartyByAppID((int)fullUpdate.toUpdate.applicationDto.Id);
                   ServiceNamesDto serviceName = await _IAdmServiceRepository.GetOnename((int)fullUpdate.toUpdate.applicationDto.ServiceId, lang);

                   DateTime schedule = await _IApplicationRepositiory.Schedule(fullUpdate.toUpdate.applicationDto.Id.ToString(), kind, fullUpdate.toSave.applicationTrackDto.UserId, (int)fullUpdate.toUpdate.applicationDto.ServiceId, serviceName.serviceName, DateTime.Now);

                   return result switch
                   {
                       OK => Ok(fullUpdate.toUpdate.applicationDto.Id),
                       _ => BadRequest(ERROR),
                   };
               }
               catch
               {
                   return BadRequest(ERROR);
               }
           }*/

        //-------------------Stage Backward------------------------------------------

      /*  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("StageBackward_")]
        public async Task<IActionResult> SetStageBackward([FromBody] FullUpdate fullUpdate)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            StagePayload stagePayload = new StagePayload();
            stagePayload.application = fullUpdate.toUpdate.applicationDto;
            stagePayload.trackDto    = fullUpdate.toSave.applicationTrackDto;
            var result = await _IApplicationRepositiory.SetStageBackward(stagePayload,UserId);
            return result switch
            {
                  OK => Ok(fullUpdate.toUpdate.applicationDto.Id),
                _ => BadRequest(ERROR),
            };


        }
*/
        //-----------*** Return ***------------ Back To First Stage notification- -----------------------------

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("BackToFirstStageNoti")]
        public async Task<IActionResult> BackToFirstStageNoti([FromBody] FullUpdate fullUpdate)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            APIResult result = await _IApplicationRepositiory.BackToFirstStageNoti(fullUpdate, UserId, lang);
            return Ok(result);
        }

        //-----------***  ***------------ Back To approval Stage notification- -----------------------------

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("BackToApprovalStageNoti")]
        public async Task<IActionResult> BackToApprovalStageNoti([FromBody] FullUpdate fullUpdate)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            APIResult result = await _IApplicationRepositiory.BackToApprovalStageNoti(fullUpdate, UserId, lang);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ApplicationDto applicationDto)
        {
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            APIResult ApiResult = await _IApplicationRepositiory.Update(id, UserId, applicationDto);        
            return ApiResult.Result switch
            {
                null=> BadRequest(new { error = getMessage(lang, "UnknownError") }),
                _ => Ok(new { message = getMessage(lang, "sucsessUpdate") }),
            };
        }

        /*  [HttpPost]
          public async Task<ActionResult> Post([FromBody] ApplicationDto applicationDto)
          {
              string lang = Request.Headers["lang"].ToString().ToLower();
              Response.Headers.Add("lang", lang);
              int result = await _IApplicationRepositiory.Add(applicationDto);
              return result switch
              {
                  ERROR => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
                  _ => Ok(new
                  {
                      message = getMessage(lang, "sucsessAdd"),
                      id = result
                  })
              };
          }*/

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            int result = await _IApplicationRepositiory.DeleteOne(id);
            return result switch
            {
                OK => Ok(new { message = Constants.getMessage(lang, "sucsessDelete") }),
                NOT_FOUND => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }

        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("InitialData/{serviceId}")]
        public async Task<ActionResult> GetInitialData([FromRoute] int serviceId, [FromQuery] string lang)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            var result = await _IApplicationRepositiory.GetInitialData(serviceId, lang, UserId);
            return Ok(result);
        }

      //  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Data")]
        public async Task<ActionResult> GetRequiredData(string lang)
        {
            var result = await _IApplicationRepositiory.GetRequiredData(lang);
            return Ok(result);
        }
      
        //-------------------save as draft
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AddEntireAppInfo/{toStage}")]
        public async Task<ActionResult> PostAll([FromBody] FullUpdate fullUpdate, [FromRoute] FIERST_SAVE_STAGE toStage)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            fullUpdate.toSave.applicationDto.Channel = _IGeneralRepository.AppDevice(Request.Headers["User-Agent"]);
            APIResult ApiResult = await _IApplicationRepositiory.PostAll(fullUpdate, toStage, UserId, lang);
            return Ok(ApiResult);
        }
        
        //--------------- *** Send / Creation *****  ------------
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AddEntireAppInfoNotif/{toStage}")]
        public async Task<ActionResult> PostAllWithNotification([FromBody] FullUpdate fullUpdate, [FromRoute] FIERST_SAVE_STAGE toStage, [FromQuery] int actionId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            if(await _IUserRepository.IsEmployee(UserId))
            {
                fullUpdate.toSave.applicationDto.Channel= await _ISysValueRepository.GetIdByShortcut("Notary");
            }
            else
            {
              fullUpdate.toSave.applicationDto.Channel = _IGeneralRepository.AppDevice(Request.Headers["User-Agent"]);
            }
            APIResult ApiResult = await _IApplicationRepositiory.PostAllWithNotification(fullUpdate, toStage, actionId, UserId, lang);
            return Ok(ApiResult);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("PreviewData/{appId}")]    
        public async Task<ActionResult> GetPreviewStageData([FromRoute] int appId, [FromQuery] string lang,string isNext)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value) ;


            var result = await _IApplicationRepositiory.GetPreviewStageData(isNext,appId, UserId, lang);


            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("PreviewDataLight/{appId}")]
        public async Task<ActionResult> GetPreviewStageDataLight([FromRoute] int appId, [FromQuery] string lang)
        {
           // var identity = HttpContext.User.Identity as ClaimsIdentity;
           // IList<Claim> claim = identity.Claims.ToList();
            //int UserId = Convert.ToInt32(claim[0].Value);
            var result = await _IApplicationRepositiory.GetPreviewStageDataLight(appId, 6076, lang);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("PreviewDatan/{appId}")]
        public async Task<ActionResult> GetPreviewStageDatan([FromRoute] int appId, [FromQuery] string lang,string isNext)
        {

            var result = await _IApplicationRepositiory.GetPreviewStageData(isNext,appId, 6057, lang);

            return result switch
            {
                Constants.NOT_FOUND => NotFound(),
                _ => Ok(result),
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("EditableData/{appId}")]
        public async Task<ActionResult> GetEditableData([FromRoute] int appId, [FromQuery] string lang)
        {
            int tid;
            var application = await _IApplicationRepositiory.GetOneWithAllRelated(appId);
            tid = application.TemplateId > 0 ? (int)application.TemplateId : -1;
            ServiceNamesDto serviceName = await _IAdmServiceRepository.GetOnename((int)application.ServiceId, lang);
            TemplateView template = await _ITemplateRepository.GetTemplateName(tid, lang);
            List<StageOfService> Stages = await _IApplicationRepositiory.getStageOfService(appId, (int)serviceName.Id, lang);
            var result = new
            {
                template = template,
                serviceName = serviceName,
                stages = Stages,
                application = application,
                ActionButtons = await _IApplicationRepositiory.getActions((int)application.CurrentStageId, lang),
            };
            if (application != null)
                return Ok(result);
            else
                return NotFound(result);
        }

        //---------- ****** Send / Draft ******------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AppFullUpdate/Forward")]
        public async Task<ActionResult> FullUpdateWithForward([FromBody] FullUpdate fullUpdate, [FromQuery] int actionId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            APIResult result = await _IApplicationRepositiory.FullUpdateWithForward(fullUpdate, actionId, UserId, lang);//_IUserRepository.GetUserID()
            return Ok(result);

        }
        //---------- ****** Approval / Review ******---------------------

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AppFullUpdate/SigningNotiForward")]
        public async Task<ActionResult> FullUpdateWithSigningNotiForward([FromBody] FullUpdate fullUpdate,[FromQuery] int actionId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            APIResult result = await _IApplicationRepositiory.SigningNotiForward(fullUpdate, actionId, UserId, lang);
            return Ok(result);
        }
        //---------- ****** Approval / Interview ******-----------

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AppFullUpdateNoti/Forward")]
        public async Task<ActionResult> FullUpdateWithForwardNoti([FromBody] FullUpdate fullUpdate,[FromQuery] int actionId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            APIResult result=await _IApplicationRepositiory.FullUpdateWithForwardNoti(fullUpdate,UserId, lang, actionId);
            return Ok(result);
  
        }

        //--------- ****** Save Draft ****** ------------ 
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AppFullUpdate")]
        public async Task<ActionResult> FullUpdateAndStay([FromBody] FullUpdate fullUpdate)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            APIResult result = await _IApplicationRepositiory.FullUpdateAndStay(fullUpdate,UserId, FORSEND, lang);
            return Ok(result);

        }
        //--------- ****** Save IN OTHER STAGES ****** ------------ 

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AppFullUpdateOnProgress")]
        public async Task<ActionResult> FullUpdateAndStayOnProgress([FromBody] FullUpdate fullUpdate)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            APIResult result = await _IApplicationRepositiory.FullUpdateAndStay(fullUpdate, UserId, ONPROGRESS, lang);
            return Ok(result);

        }
        //------------****** Change State / Refuse ******-------------

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("ChangeStateNoti")]
        public async Task<ActionResult> ChangeAppStateWithNoti([FromBody] FullUpdate fullUpdate, [FromQuery] string to, [FromQuery] int actionId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            APIResult result = await _IApplicationRepositiory.ChangeAppStateWithNoti(fullUpdate,to, actionId, UserId, lang);
            return Ok(result);         
        }

        //-------------------------- Update Meeting Date  ----------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("UpdateMeetingDate")]
        public async Task<ActionResult> UpdateMeetingDate([FromQuery] string orderNo, [FromQuery] DateTime toDate, [FromQuery] int serviceId, [FromQuery] int actionId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            string lang = Request.Headers["lang"].ToString().ToLower();

            APIResult sc = await _IApplicationRepositiory.UpdateSchedule(orderNo, 8,   serviceId,  actionId,UserId/*_IUserRepository.GetUserID()*/, toDate);
            return Ok(sc);
          

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("ESign")]
        public async Task<ActionResult> ESignIt(ESignPayload eSignPayload)
        {
            try
            {
              APIResult ApiResult = await _IApplicationRepositiory.ESignIt(eSignPayload.appPartyId, eSignPayload.userId, eSignPayload.appId, eSignPayload.Base64Sign,eSignPayload.signType);
              return Ok(ApiResult);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
       
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("notification/action/{actionId}")]
        public async Task<ActionResult> GetNotificationByAction([FromRoute] int actionId, [FromQuery] string lang, [FromQuery] int serviceId, [FromQuery] int stageId, [FromQuery] int appId)
        {
            var a= await _IApplicationRepositiory.GetNotificationsByAction(actionId, lang, serviceId,stageId,appId);
            return Ok(a);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetSignatureStatus/{appId}")]
        public async Task<ActionResult> GetSignatureStatus([FromRoute] int appId)
        {
          var result=await _IApplicationRepositiory.SignersAndNot(appId);
          return Ok(result);           
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("NotifyRequiredParties")]
        public async Task<ActionResult> NotifyParties([FromQuery] int appId,int serviceId, int actionId)
        {
            string lang = Request.Headers["lang"].ToString().ToLower();
            if (lang == null || lang.Length < 2)
                lang = "ar";
            APIResult result = await _IApplicationRepositiory.NotifyWithTokenLink(appId, serviceId, actionId, lang);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("RebuildAppDocuments")]
        public async Task<ActionResult> RebuildAppDocuments([FromBody] FullUpdate fu)
        {
            string lang = Request.Headers["lang"].ToString().ToLower();
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            APIResult result = await _IApplicationRepositiory.RebuildPDFDocuments(fu, UserId, lang);     
            return Ok(result);
        }

        /*
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("RebuildAppDocuments_")]
        public async Task<ActionResult> RebuildAppDocuments_([FromBody] FullUpdate fu)
        {
           
            APIResult result = await _IApplicationRepositiory.RebuildPDFDocuments(fu, 6076, "ar");
            return Ok(result);
        }*/

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("BuildAppDocuments/{appId}")]
        public async Task<ActionResult> BuildAppDocuments(int appId)
        {
            /*string lang = Request.Headers["lang"].ToString().ToLower();
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);*/
            APIResult result = await _IApplicationRepositiory.BuildPDFDocuments(appId);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("OwnApplication")]
        public async Task<ActionResult> OwnApplication([FromQuery] int appId)
        {
            string lang = Request.Headers["lang"].ToString().ToLower();
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            APIResult result = await _IApplicationRepositiory.OwnApplication(appId, UserId, lang);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("ReleaseApplication")]
        public async Task<ActionResult> ReleaseApplication([FromQuery]int appId)
        {
            string lang = Request.Headers["lang"].ToString().ToLower();
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            APIResult result = await _IApplicationRepositiory.ReleaseApplication(appId, UserId, lang);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("SearchTransaction")]
        public async Task<ActionResult> SearchTransaction([FromBody] SearchObject searchObject)
        {
            string lang = Request.Headers["lang"].ToString().ToLower();
            /*var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);*/
            var result = await _IApplicationRepositiory.Search(searchObject, lang);
            return Ok(result);
        }
        //-----------------------------for test ------------------------- 

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("userApplication")]
        public async Task<ActionResult> ua( [FromQuery] int serviceId, [FromQuery]  int userId, [FromQuery] string lang)
        {
            List<int> l = new List<int>();
            l.Add(1);
            l.Add(2);
            var a = await _IApplicationRepositiory.GetUserApplication( serviceId, userId,lang,l,null);
            return Ok(a);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("relatedApplication")]
        public async Task<ActionResult> ra( [FromQuery] int appId, [FromQuery] string lang)
        {

            var a = await _IApplicationRepositiory.GetRelatedApplicationsInfo( appId,lang) ;
            return Ok(a);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("party")]
        public async Task<ActionResult> party([FromQuery] int id)
        {
            var a = await _IApplicationRepositiory.GetPartyByAppID(id);
            return Ok(a);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("isparty")]
        public async Task<ActionResult> party([FromQuery] int id, [FromQuery] int userId)
        {
            var a = await _IApplicationRepositiory.IsParty(id,userId);
            return Ok(a);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("refreshReadingData/{appId}")]
        public async Task<ActionResult> read( int appId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            var a = await _IApplicationRepositiory.RefreshReadingDate(appId, UserId);
            return Ok(a);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("freeApplication/{appId}")]
        public async Task<ActionResult> freeApp( int appId)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            var a = await _IApplicationRepositiory.freeApplication(appId, UserId);
            return Ok(a);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("ClearSignInfo")]
        public async Task<ActionResult> ClearSignInfo([FromBody] ApplicationTrackDto appTrackDto)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            var a = await _IApplicationRepositiory.ClearRelatedPartiesSignInfo(appTrackDto, UserId, "ar");
            return Ok(a);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("makedone/{appId}")]
        public async Task<ActionResult> done(int appId, [FromQuery] int userId)
        {
            var a = await _IApplicationRepositiory.MakeItDone(appId, userId);
            return Ok(a);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("lastnotary")]
        public async Task<ActionResult> ln( [FromQuery] int userId)
        {
            var a = await _IApplicationRepositiory.GetLastUpdaterNotary(userId);
            return Ok(a);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("final")]
        public async Task<ActionResult> f( [FromQuery] int appId)
        {
            var a = await _IApplicationRepositiory.PartyFinalDocument(appId);
            return Ok(a);
        }

        //[HttpPost("NotifyLatePartyies")]
        //public async Task<ActionResult> Notifylate(List<ServiceApplication> serviceApplications, string messagebody)
        //{
        //    var a = await _IApplicationRepositiory.NotifyLateInterviewPartyies(serviceApplications,  messagebody);
        //    return Ok(a);
        //}

        [HttpPost("AddAppObjection")]
        public async Task<ActionResult> AddAppObjection(AppObjectionDto appObjectionDto, [FromHeader] string lang)
        {
            var a = await _IApplicationRepositiory.AddAppObjection(appObjectionDto, lang);
            return Ok(a);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AddAppObjectionParty")]
        public async Task<ActionResult> AddAppObjectionParty([FromBody] ApplicationObjectionDTO applicationObjectionDTO, [FromHeader] string lang)
        {
            var a = await _IApplicationRepositiory.AddAppObjectionParty(_IUserRepository.GetUserID(), applicationObjectionDTO.ApplicationId, applicationObjectionDTO.Reason, lang);
            return Ok(a);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("GetLateApps")]
        public async Task<ActionResult> getLateAppinMeet(string lessDate)
        {
            var result = await _IApplicationRepositiory.GetLateApps(lessDate);
            return Ok(result);

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("GetNotLateApps")]
        public async Task<ActionResult> getNotLateAppinMeet()
        {
            var result = await _IApplicationRepositiory.GetNotLateApps();
            return Ok(result);

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("RejectApps")]
        public async Task<ActionResult> BacktoDraft(string lessDate)
        {
            var result = await _IApplicationRepositiory.RejectApps(lessDate);
            return Ok(result);

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("DailyNotify")]
        public async Task<ActionResult> DailyNotify()
        {
            var result = await _IApplicationRepositiory.DailyNotify();
            return Ok(result);

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AddNoteAppTrack")]
        public async Task<ActionResult> AddNoteAppTrack([FromBody] ApplicationTrackDto appTrackDto)
        {
            


            var a = await _IApplicationRepositiory.AddNoteTrack(appTrackDto);
            return Ok(a);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("DoneWithNoPay/{appId}")]
        public async Task<ActionResult> DoneWithNoPay([FromRoute] int appId)
        {
            int timeToEnd = Convert.ToInt32( _IConfiguration["ADVTime"]);
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            List<string> msgs = new List<string>();
            msgs.Add("لم تنته المدة المخصصة للاعتراض");
            var result = await _IApplicationRepositiory.GetDoneNoPay(appId, UserId, timeToEnd, msgs, "ar");
            return Ok(result);

        }
    }

}


