
using EngineCoreProject.Services;
using EngineCoreProject.DTOs.TemplateSetDtos;


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.FileDto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;

namespace EngineCoreProject.Controllers.ApplicationSet
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ApplicationAttachmentsController : ControllerBase
    {
        private readonly IApplicationAttachmentRepository _IApplicationAttachmentRepositiory;

        public ApplicationAttachmentsController(IApplicationAttachmentRepository iApplicationAttachmentRepositiory)
        {
            _IApplicationAttachmentRepositiory = iApplicationAttachmentRepositiory;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> All([FromQuery] int? applicationId)
        {
            string lang = Request.Headers["lang"].ToString().ToLower();
            string Authorization = Request.Headers["Authorization"].ToString().ToLower();
            try
            {
                var result = await _IApplicationAttachmentRepositiory.GetAll(applicationId);
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
        [HttpGet("{id}")]
        public async Task<ActionResult> One([FromRoute] int id, [FromHeader] string lang)
        {
            try
            {
                var result = await _IApplicationAttachmentRepositiory.GetOne(id);
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
        public async Task<IActionResult> Put(int id, [FromBody] ApplicationAttachmentDto templateAttachmentDto, [FromHeader] string lang)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            int UserId = Convert.ToInt32(claim[0].Value);
            int result = await _IApplicationAttachmentRepositiory.Update(id, UserId, templateAttachmentDto);
            return result switch
            {
                -200 => Ok(new { message = Constants.getMessage(lang, "sucsessUpdate") }),
                -204 => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };

        }

       /* [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ApplicationAttachmentDto applicationAttachmentDto, [FromHeader] string lang)
        {
            int result = await _IApplicationAttachmentRepositiory.Add(applicationAttachmentDto);
             return result switch
             {
                 -500 => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
                 _ => Ok(new
                 {
                     message = Constants.getMessage(lang, "sucsessAdd"),
                     id = result
                 })
             };
           // return Ok(await _IApplicationAttachmentRepositiory.Addex(applicationAttachmentDto));/
        }*/

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            int result = await _IApplicationAttachmentRepositiory.DeleteOne(id);
            return result switch
            {
                -200 => Ok(new { message = Constants.getMessage(lang, "sucsessDelete") }),
                -204 => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }

        //-------------------------------------Upload-------------------------------

        //-----------------
        [HttpPost("Upload")]
        public async Task<ActionResult> UploadAttachment(IFormFile File)
        {
            UploadedFileMessage m = await _IApplicationAttachmentRepositiory.UploadApplicationAttachment(File);
            return m.SuccessUpload switch
            {
                false => BadRequest(m),//new { error = Constants.getMessage(lang, "UnknownError") }
                _ => Ok(m)
            };
        }

    }
}
