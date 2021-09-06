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
    public class TransactionFeeController : ControllerBase
    {
        private readonly ITransactionFeeRepository _iTransactionFeeRepository;


        public TransactionFeeController(ITransactionFeeRepository iTransactionFeeRepository)
        {
            _iTransactionFeeRepository = iTransactionFeeRepository;

        }


       [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("TransactionFee")]
        public async Task<ActionResult> AddTransactionFee(TransactionFeePostDto transactionFeePostDto, [FromHeader] string lang)
        {
            var result = await _iTransactionFeeRepository.AddTransactionFee(transactionFeePostDto, lang);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("UpdateTransactionFee")]
        public async Task<ActionResult> UpdateTransactionFee(TransactionFeePostDto transactionFeePostDto, int rowId,  [FromHeader] string lang)
        {
            var result = await _iTransactionFeeRepository.UpdateTransactionFee(transactionFeePostDto, rowId, lang);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            await _iTransactionFeeRepository.DeleteTransactionFee(id, lang);
            return Ok("success");
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetTransactionFees")]
        public async Task<ActionResult> GetTransactionFees()
        {
            var result = await _iTransactionFeeRepository.GetTransactionFees();
            return Ok(result);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("CalculateTransactionFee")]
        public async Task<IActionResult> CalculateTransactionFee(TransactionFeeInput transactionFeeInput, [FromHeader] string lang)
        {
            var obj = await _iTransactionFeeRepository.CalculateTransactionFee(transactionFeeInput, lang);
            return Ok(obj);
        }
    }
}
