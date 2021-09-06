using EngineCoreProject.Services;
using EngineCoreProject.DTOs.TemplateSetDtos;
using EngineCoreProject.IRepository.TemplateSetRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using EngineCoreProject.DTOs.SysLookUpDtos;
using System.Collections.Generic;
using EngineCoreProject.IRepository.ITemplateSetRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using System.Security.Claims;
using System.Linq;
using EngineCoreProject.Models;
using EngineCoreProject.IRepository.AdmServices;
using EngineCoreProject.DTOs.ApplicationDtos.ModelView;

namespace EngineCoreProject.Controllers.TemplateSet
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TemplatesController : ControllerBase
    {
        private readonly ITemplateRepository _ITemplateRepository;
        private readonly ITermRepository _ITermRepository;
        private readonly IGeneralRepository _IGeneralRepository;
        private readonly IApplicationRepository _IApplicationRepository;
        private readonly IAdmServiceRepository _IAdmServiceRepository;


        public TemplatesController(ITemplateRepository iTemplateRepository,
        IGeneralRepository iGeneralRepository, ITermRepository iITermRepository, IApplicationRepository iApplicationRepository,
        IAdmServiceRepository iAdmServiceRepository)
        {
            _ITemplateRepository = iTemplateRepository;
            _IGeneralRepository  = iGeneralRepository;
            _ITermRepository     = iITermRepository;
            _IApplicationRepository = iApplicationRepository;
            _IAdmServiceRepository = iAdmServiceRepository;

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> All([FromQuery] int? documentTypeId, [FromQuery] string lang)
        {
            try
            {
                var result = await _ITemplateRepository.GetTemplateNames(documentTypeId, lang);
                return Ok(result);
            }
            catch
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,//e.Message.ToString()
                                       new { message = Constants.getMessage(lang, "UnknownError") }
                                       );
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [Route("AddShortCut")]
        [HttpPost]
        public async Task<ActionResult> AddShortCut([FromBody] TableField tableField, [FromHeader] string lang)
        {
            string shortcut;
            shortcut = _IGeneralRepository.GenerateShortCut(tableField.TableName, tableField.FieldName);
            TemplateDto templateDto = new TemplateDto();
            templateDto.TitleShortcut = shortcut;
            templateDto.DocumentTypeId = tableField.ParentId;

            int result = await _ITemplateRepository.Add(templateDto);
            return result switch
            {
                -500 => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
                _ => Ok(new
                {
                    message = Constants.getMessage(lang, "sucsessAdd"),
                    id = result,
                    shortcut = shortcut
                })
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [Route("InsertTranslation")]
        [HttpPost]
        public async Task<ActionResult> InsertTranslation([FromBody] List<TranslationDto> translationDtos)
        {
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            List<string> messagesList = new List<string>();
            AddNewTransResult addNewTransResult;
            foreach (var translationDto in translationDtos)
            {
                addNewTransResult = await _IGeneralRepository.insertTranslation(translationDto.Shortcut, translationDto.Value, translationDto.Lang);
                foreach (var message in addNewTransResult.messages)
                    messagesList.Add(message);
            }

            return Ok(messagesList);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("{TemplateId}/Attachments")]
        [HttpGet]
        public async Task<ActionResult> Attachments([FromRoute] int TemplateId, [FromQuery] string lang)
        {
            string lang_ = Request.Headers["lang"].ToString().ToLower();
            string Authorization = Request.Headers["Authorization"].ToString().ToLower();
            try
            {
                var result = await _ITemplateRepository.GetRelatedAttachments(TemplateId, lang);
                return Ok(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,//e.Message.ToString()
                new { message = Constants.getMessage(lang, "UnknownError") }
                        );
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("{TemplateId}/Parties")]
        [HttpGet]
        public async Task<ActionResult> Parties([FromRoute] int TemplateId, [FromQuery] string lang)
        {
            try
            {
                var result = await _ITemplateRepository.GetRelatedParties(TemplateId, lang);
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
        public async Task<ActionResult> One([FromRoute] int id, [FromQuery] string lang)
        {
            Response.Headers.Add("lang", lang);
            try
            {
                var result = await _ITemplateRepository.GetTemplateName(id, lang);
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
        public async Task<IActionResult> Put(int id, [FromBody] TemplateDto templateDto, [FromHeader] string lang)
        {
            int result = await _ITemplateRepository.Update(id, templateDto);
            return result switch
            {
                -200 => Ok(new { message = Constants.getMessage(lang, "sucsessUpdate") }),
                -204 => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TemplateDto templateDto, [FromHeader] string lang)
        {
            int result = await _ITemplateRepository.Add(templateDto);
            return result switch
            {
                -500 => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
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
            int result = await _ITemplateRepository.DeleteOne(id,lang);
            return result switch
            {
                -200 => Ok(new { message = Constants.getMessage(lang, "sucsessDelete") }),
                -204 => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete]
        public async Task<ActionResult> DeleteMany(int[] ids, [FromHeader] string lang)
        {
            List<int> result = await _ITemplateRepository.DeleteMany(ids);
            return result.Count switch
            {
                0 => Ok(new { message = Constants.getMessage(lang, "sucsessDelete") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "faildDelete") + " " + string.Join(",", result) })
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("details/{id}")]
        public async Task<ActionResult> Details([FromRoute] int id, [FromQuery] int serviceId, [FromQuery] string lang)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            var t = await _ITemplateRepository.GetTemplateName(id, lang);
            List<UserApplication> relatedData = null;
            AdmService admService = await _IAdmServiceRepository.GetOne(serviceId);
            int DraftStageId = await _IAdmServiceRepository.FirstStage(serviceId);
            var relatedContent = await _ITemplateRepository.GetRelatedContents(id, DraftStageId, lang);
            bool TransactionRelatedData=false;
            //int targetService = admService.TargetServiceService.Select(x => x.ServiceId).FirstOrDefault();
            int targetService;
            //if (admService.TargetServiceService != null && admService.TargetServiceService.Count>0)
            if (admService.TargetService != null)
            {  
              targetService = (int)admService.TargetService;

            if (t.ShowApplication)
            {
                TransactionRelatedData = false;
                List<int> l = new List<int>();
                l.Add(2);
                l.Add(3);
                relatedData = await _IApplicationRepository.GetUserApplication(targetService, UserId, lang, l, null);
            }
            else
            {
                if (t.ShowTransaction)
                {
                   TransactionRelatedData = true;
                  if (admService.TargetServiceService.Count == 1 && admService.TargetServiceService.ToList()[0].TargetDocumentTypeId == null)
                     {
                        relatedData = await _IApplicationRepository.GetUserTransaction((int)admService.TargetServiceService.ToList()[0].TargetServiceId, UserId, lang);
                     }
                   else 
                     {
                       relatedData = await _IApplicationRepository.GetUserTransaction(admService.TargetServiceService, UserId, lang);
                     }
                     
                }

            }
        }
            var result = new
            {
                Delivery=admService.Delivery,
                Template = t,
                attachments = await _ITemplateRepository.GetRelatedAttachments(id, lang),
                parties = await _ITemplateRepository.GetRelatedParties(id, lang),
                terms = await _ITermRepository.GetAll(id),
                tableTitle= TransactionRelatedData?Constants.getMessage(lang, "pickTransaction"): Constants.getMessage(lang, "pickApp"),
                relatedData = relatedData,
                TransactionRelatedData = TransactionRelatedData,
                relatedContent = relatedContent,


            };
            return Ok(result);

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Translations")]
        public async Task<ActionResult> getTranslations([FromQuery] string code)
        {
            var result = await _ITemplateRepository.GetTitleTranslations(code);
            return Ok(result);
        }
    }
}
