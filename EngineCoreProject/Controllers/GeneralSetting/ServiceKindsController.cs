using EngineCoreProject.DTOs.GeneralSettingDto;
using EngineCoreProject.IRepository.IGeneralSetting;
using EngineCoreProject.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.ServiceKinds
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class ServiceKindsController : ControllerBase
    {
        private readonly IServiceKindRepository _iServiceKindRepository;
        public ServiceKindsController(IServiceKindRepository iServiceKindRepository)
        {
            _iServiceKindRepository = iServiceKindRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost]
        public async Task<ActionResult> AddServiceKind([FromBody] ServiceKindPostDto serviceKindDto, [FromHeader] string lang)
        {
            var result = await _iServiceKindRepository.AddServiceKind(serviceKindDto, lang);
            if (result != 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateServiceKind(ServiceKindPostDto serviceKindPostDto, int id, [FromHeader] string lang)
        {
            var result = await _iServiceKindRepository.UpdateServiceKind(serviceKindPostDto, id, lang);
            if (result != 0)
            {

                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteServiceKind(int id, [FromHeader] string lang)
        {
            var result = await _iServiceKindRepository.DeleteServiceKind(id);
            if (result != 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet]
        public async Task<ActionResult> GetServiceKinds([FromHeader] string lang)
        {
            var result = await _iServiceKindRepository.GetServiceKinds(lang);
            if (result != null)
            {

                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetServiceKindById(int id, [FromHeader] string lang)
        {
            var result = await _iServiceKindRepository.GetServiceKindById(id, lang);
            if (result != null && result.Id !=0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, Constants.getMessage(lang, "zeroResult")));
        }
    }
}
