using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.AccountDto;
using EngineCoreProject.DTOs.UnifiedGateDto;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.Models;
using EngineCoreProject.Services;
using EngineCoreProject.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EngineCoreProject.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]

    public class EmployeeSettingController : ControllerBase
    {

        private readonly IEmployeeSettingRepository _iEmployeeSettingRepository;
        private readonly IUserRepository _IUserRepository;
        public EmployeeSettingController(IEmployeeSettingRepository iEmployeeSettingRepository, IUserRepository iUserRepository)
        {
            _iEmployeeSettingRepository = iEmployeeSettingRepository;
            _IUserRepository = iUserRepository;
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AddUpdateEmployeeSetting")]
        public async Task<ActionResult> UpdateEmployeeSetting(EmployeeSettingPostDto employeeSettingPostDto, [FromHeader] string lang)
        {
            var result = await _iEmployeeSettingRepository.AddUpdatEmployeeSetting(_IUserRepository.GetUserID(), employeeSettingPostDto, lang);
            if (result != 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("GetEmployeeSetting")]
        public async Task<ActionResult> GetEmployeeSetting([FromHeader] string lang)
        {
            var result = await _iEmployeeSettingRepository.GetEmployeeSetting(_IUserRepository.GetUserID(), lang);
            return Ok(result);
        }

    }

}
