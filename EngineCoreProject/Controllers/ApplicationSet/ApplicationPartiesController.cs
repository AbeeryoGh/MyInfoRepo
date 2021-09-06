using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.FileDto;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.ApplicationSet
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ApplicationPartiesController : ControllerBase
    {
        private readonly IApplicationPartyRepository _IApplicationPartyRepositiory;

        public ApplicationPartiesController(IApplicationPartyRepository iApplicationPartyRepositiory)
        {
            _IApplicationPartyRepositiory = iApplicationPartyRepositiory;


        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> All([FromQuery] int? transactionId, [FromHeader] string lang)
        {
            try
            {
                var result = await _IApplicationPartyRepositiory.GetAll(transactionId);
                return Ok(result);
            }
            catch
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,//e.Message.ToString()
                new { message = Constants.getMessage(lang, "UnknownError") }
                      );
            }

        }

        //----------------------------------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Transaction/{transactionId}")]
        public async Task<ActionResult> AllIDs([FromRoute] int transactionId, [FromHeader] string lang)
        {
            try
            {
                var result = await _IApplicationPartyRepositiory.GetAllIDs(transactionId);
                return Ok(result);
            }
            catch
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,//e.Message.ToString()
                new { message = Constants.getMessage(lang, "UnknownError") }
                      );
            }

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("party")]
        public async Task<ActionResult> party([FromQuery] int id)
        {
            var a = await _IApplicationPartyRepositiory.GetAllIDs(id);
            return Ok(a);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task<ActionResult> One([FromRoute] int id, [FromHeader] string lang)
        {
            try
            {
                var result = await _IApplicationPartyRepositiory.GetOne(id);
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ApplicationPartyDto applicationPartyDto, [FromHeader] string lang)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            int result = await _IApplicationPartyRepositiory.Update(id, UserId, applicationPartyDto);
            return result switch
            {
                Constants.OK => Ok(new { message = Constants.getMessage(lang, "sucsessUpdate") }),
                Constants.NOT_FOUND => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }

        /*  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
          [HttpPost]
          public async Task<ActionResult> Post([FromBody] ApplicationPartyDto applicationPartyDto, [FromHeader] string lang)
          {
              int result = await _IApplicationPartyRepositiory.Add(applicationPartyDto);
              return result switch
              {
                  Constants.ERROR => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
                  _ => Ok(new
                  {
                      message = Constants.getMessage(lang, "sucsessAdd"),
                      id = result
                  })
              };
          }*/

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            APIResult result = await _IApplicationPartyRepositiory.DeleteParty(id);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Upload")]
        public async Task<ActionResult> UploadAttachment(IFormFile File, [FromHeader] string lang)
        {
            UploadedFileMessage m = await _IApplicationPartyRepositiory.UploadPartyAttachment(File);
            return m.SuccessUpload switch
            {
                false => BadRequest(m),//new { error = Constants.getMessage(lang, "UnknownError") }
                _ => Ok(m)
            };
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AddPartyUser")]
        public async Task<ActionResult> AddPartyUser(ApplicationPartyDto applicationPartyDto, [FromHeader] string lang)
        {
            var m = await _IApplicationPartyRepositiory.AddPartyToUser(applicationPartyDto, lang);
            return
               Ok(m);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("SwithUserSignState")]
        public async Task<ActionResult> SwithUserSignState(int id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            APIResult ApiResult = await _IApplicationPartyRepositiory.SwitchPartySignStatus(id, UserId);
            return
               Ok(ApiResult);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("SwithUserSignRequired")]
        public async Task<ActionResult> SwithUserSignRequired(int id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            APIResult ApiResult = await _IApplicationPartyRepositiory.SwitchPartySignRequired(id, UserId);
            return
               Ok(ApiResult);
        }

        //  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("FixAPPParty")]
        public async Task<int> FixAPPParty()
        {
            var ApiResult = await _IApplicationPartyRepositiory.FixAPPParty();
            return ApiResult;
        }
    }
}
