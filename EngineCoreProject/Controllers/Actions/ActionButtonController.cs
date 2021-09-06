using EngineCoreProject.DTOs.ActionButton;
using EngineCoreProject.IRepository.IActionButtonRepository;
using EngineCoreProject.Models;
using EngineCoreProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.ActionButton
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ActionButtonController : ControllerBase
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IActionButtonRepository _IActionButtonRepository;
        private readonly IGeneralRepository _IGeneralRepository;

        public ActionButtonController(IActionButtonRepository iActionButtonRepository,
        IGeneralRepository iGeneralRepository, EngineCoreDBContext EngineCoreDBContext)
        {

            _EngineCoreDBContext = EngineCoreDBContext;
            _IActionButtonRepository = iActionButtonRepository;
            _IGeneralRepository = iGeneralRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetAllaction")]
        public async Task<IActionResult> GetAllaction([FromHeader] string lang)
        {
            Response.Headers.Add("lang", lang);
            var result = await _IActionButtonRepository.getallActions(lang);
            if (result != null)
            {

                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult> Post()
        {
            AdmAction admAction = await _IActionButtonRepository.add();
            if (admAction != null)
                return this.StatusCode(StatusCodes.Status200OK, new { admAction.Id, admAction.Shortcut });
            else
                return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, int actiontypeid, [FromHeader] string lang)
        {
            int result = await _IActionButtonRepository.update(id, actiontypeid);
            return result switch
            {
                -200 => Ok(new { message = Constants.getMessage(lang, "sucsessUpdate") }),
                -204 => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }
    }

}
