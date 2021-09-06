using EngineCoreProject.DTOs.ActionRole;
using EngineCoreProject.DTOs.RoleDto;
//using EngineCoreProject.IRepository.ActionRole;
using EngineCoreProject.IRepository.IRoleRepository;
using EngineCoreProject.Models;
using EngineCoreProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.RoleController
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _IGeneralRepository;
        private readonly IRoleRepository _IRoleRepository;

        public RoleController(IRoleRepository roleRepository, IGeneralRepository iGeneralRepository, EngineCoreDBContext EngineCoreDBContext)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _IGeneralRepository = iGeneralRepository;
            _IRoleRepository = roleRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet]
        public async Task<ActionResult> Get([FromHeader] string lang)
        {
            var result = await _IRoleRepository.GetAllRoles(lang);
            if (result != null)
            {

                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetRolePermissions")]
        public async Task<ActionResult> GetRolePermissions(int roleId, [FromHeader] string lang)
        {
            var result = await _IRoleRepository.GetRolePermissions(roleId, lang);
            if (result != null)
            {

                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("AddUpdatePermissionsToRole")]
        public async Task<ActionResult> AddUpdatePermissionsToRole(RolePermissionsPostDTO rolePermissionsDTO, [FromHeader] string lang)
        {
            var result = await _IRoleRepository.AddUpdatePermissionsToRole(rolePermissionsDTO, lang);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("CreateRoleAsync")]
        public async Task<ActionResult> CreateRoleAsync(RolePostDto postRoleDto, [FromHeader] string lang)
        {
            var result = await _IRoleRepository.CreateRoleAsync(postRoleDto, lang);
            return Ok(result);
        }

        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("CreateHardCodedRoleAsync")]
        public async Task<ActionResult> CreateHardCodedRoleAsync(string postRoleDto)
        {
            var result = await _IRoleRepository.CreateHardCodedRoleAsync(postRoleDto);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPut("UpdateRole")]
        public async Task<ActionResult> UpdateRole(RolePostDto postRoleDto, int roleId, [FromHeader] string lang)
        {
            var result = await _IRoleRepository.UpdateRole(postRoleDto, roleId, lang);
           return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("DeleteRoleAsync")]
        public async Task<ActionResult> DeleteRoleAsync(int roleId, [FromHeader] string lang)
        {
            var result = await _IRoleRepository.DeleteRoleAsync(roleId, lang);
            return Ok(result); 
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("AddActionToRoles")]
        public async Task<ActionResult> AddActionToRoles(int actionId, List<int> roles, [FromHeader] string lang)
        {
            var result = await _IRoleRepository.AddUpdateActionToRoles(actionId, roles);
            if (result == IdentityResult.Success)
            {
                return Ok(new { message = Constants.getMessage(lang, "sucsessAdd") });
            }
            else
            {
                return BadRequest(new { error = Constants.getMessage(lang, "UnknownError") });
            }
        }
    }
}
