using EngineCoreProject.DTOs.TransactionFeeDto;
using EngineCoreProject.IRepository.ITransactionFeeRepository;
using EngineCoreProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.TransactionFeeController
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ServiceFeeController : ControllerBase
    {
        private readonly IServiceFeeRepository _iServiceFeeRepository;


        public ServiceFeeController(IServiceFeeRepository iServiceFeeRepository)
        {
            _iServiceFeeRepository = iServiceFeeRepository;

        }


       [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("AddServiceFee")]
        public async Task<ActionResult> AddServiceFee(ServiceFeePostDto serviceFeePostDto, Constants.DOCUMENT_KIND docKind, Constants.PROCESS_KIND processKind, [FromHeader] string lang)
        {
            var result = await _iServiceFeeRepository.AddServiceFee(serviceFeePostDto, docKind, processKind, lang);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("UpdateServiceFee")]
        public async Task<ActionResult> UpdateTransactionFee(ServiceFeePostDto serviceFeePostDto, int rowId, Constants.DOCUMENT_KIND docKind, Constants.PROCESS_KIND processKind, [FromHeader] string lang)
        {
            var result = await _iServiceFeeRepository.UpdateServiceFee(serviceFeePostDto, rowId, docKind, processKind, lang);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            await _iServiceFeeRepository.DeleteServiceFee(id);
            return Ok("success");
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetServiceFees")]
        public async Task<ActionResult> GetTransactionFees(int servicId)
        {
            var result = await _iServiceFeeRepository.GetServiceFeesByServiceId(servicId);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetAllServiceFees")]
        public async Task<ActionResult> GetServiceFees()
        {
            var result = await _iServiceFeeRepository.GetServiceFees();
            return Ok(result);
        }
    }
}
