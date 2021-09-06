using EngineCoreProject.DTOs.FileDto;
using EngineCoreProject.IRepository.AdmServices;
using EngineCoreProject.IRepository.IFilesUploader;
using EngineCoreProject.Models;
using EngineCoreProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static EngineCoreProject.Services.Constants;
namespace EngineCoreProject.Controllers.Service
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IAdmServiceRepository _IAdmServiceRepository;
        private readonly IGeneralRepository _IGeneralRepository;
        private readonly IFilesUploaderRepositiory _IFilesUploaderRepository;
      

        public ServiceController(IAdmServiceRepository iServiceRepository,
        IGeneralRepository iGeneralRepository, EngineCoreDBContext EngineCoreDBContext, IFilesUploaderRepositiory filesUploaderRepositiory)
        {

            _EngineCoreDBContext = EngineCoreDBContext;
            _IAdmServiceRepository = iServiceRepository;
            _IGeneralRepository = iGeneralRepository;
            _IFilesUploaderRepository = filesUploaderRepositiory;
            
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost]
        public async Task<ActionResult> Post()
        {
            AdmService admService = await _IAdmServiceRepository.add();
            if (admService != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, new { admService.Id, admService.Shortcut });
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        // For anonymous users.
        //  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> GetServices([FromHeader] string lang)
        {
            var result = await _IAdmServiceRepository.GetserviceNAmes(lang);
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{Id}")]
        public async Task<ActionResult> GetServiceById(int Id, [FromHeader] string lang)
        {
            var result = await _IAdmServiceRepository.GetOnename(Id, lang);
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            int result = await _IAdmServiceRepository.delete(id,lang);
            return result switch
            {
                Constants.OK => Ok(new { message = Constants.getMessage(lang, "sucsessDelete") }),
                Constants.NOT_FOUND => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("serviceStages")]
        public async Task<ActionResult> GetServicestages(int id, [FromHeader] string lang)
        {
            var result = await _IAdmServiceRepository.getsatgesofservice(id, lang);
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }

            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("ChangeServiceManual")]
        public async Task<IActionResult> ChangeServiceManual(int serviceId, IFormFile file, string lang)
        {
            var obj = await _IAdmServiceRepository.ChangeServiceManual(serviceId, file, lang);
            return Ok(obj);
        }

            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPut("UploadIcon")]
        public async Task<IActionResult> Put(int id, IFormFile file)
        {
            UploadedFileMessage f = await _IAdmServiceRepository.Upload(file);
            if (f.SuccessUpload != true)
                return this.StatusCode(StatusCodes.Status404NotFound, "No Icon was Uploaded...." + f.Message);
            AdmService admService = await _IAdmServiceRepository.GetOne(id);
            try
            {
                if (admService != null)

                {
                    admService.LastUpdatedDate = DateTime.Now;
                    //admService.Shortcut = serviceDto.NameShortcut;
                    admService.Icon = f.FileUrl;
                    _IGeneralRepository.Update(admService);
                    if (await _IGeneralRepository.Save())
                    {
                        return this.StatusCode(StatusCodes.Status200OK, "Service Updated");
                    }
                }
                else
                { return this.StatusCode(StatusCodes.Status404NotFound, "No Service Found ...."); }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
            }
            return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Translation")]
        public async Task<ActionResult> GetServicestrans(string shortcut)
        {
            var result = await _IGeneralRepository.GetAllTranslation(shortcut);
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }


        [HttpGet("stage")]
        public async Task<ActionResult> reviewstage()
        {
            var a = await _IAdmServiceRepository.GetReviewStagesId();
            return Ok(a);
        } 
        
        [HttpGet("getOne")]
        public async Task<ActionResult> getOne([FromQuery] int id)
        {
            AdmService admService = await _IAdmServiceRepository.GetOne(id);
            return Ok(admService);
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("DeleteAppById")]
        public async Task<ActionResult> DeleteAppById(int id)
        {
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            int result = await _IAdmServiceRepository.DeleteAppById(id);
            return result switch
            {
                OK => Ok(new { message = Constants.getMessage(lang, "sucsessDelete") }),
                NOT_FOUND => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };


        }


    }
}
