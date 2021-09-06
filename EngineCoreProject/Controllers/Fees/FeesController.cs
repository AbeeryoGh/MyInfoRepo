using EngineCoreProject.DTOs.FeesDto;

using EngineCoreProject.IRepository.IPaymentRepository;
using EngineCoreProject.Models;
using EngineCoreProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.Fees
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeesController : ControllerBase
    {      
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _IGeneralRepository;
        private readonly IPaymentRepository _IPaymentRepository;
        public FeesController( IGeneralRepository iGeneralRepository, EngineCoreDBContext EngineCoreDBContext, IPaymentRepository iPaymentRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;          
            _IGeneralRepository = iGeneralRepository;
            _IPaymentRepository = iPaymentRepository;
        }

        [HttpGet("GetFees")]
        public async Task<ActionResult> GetFees()
        {
            var result = await _IPaymentRepository.GetApplicationFeesAsync();
            if (result != null)
            {

                return this.StatusCode(StatusCodes.Status200OK, result);

            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred....");
        }


        [HttpGet("test")]
        public ActionResult test()
        {
            List<string> fruits = new List<string> {"a","b","c","d" };

            var result = fruits.Where(x => x.Contains("g")).FirstOrDefault();
                return this.StatusCode(StatusCodes.Status200OK, result);

            
        }


        [HttpGet("GetFeesById")]
        public async Task<ActionResult> GetFeesById(int id, [FromHeader] string lang)
        {
            var result = await _IPaymentRepository.GetApplicationFeesByIdAsync(id, lang);
            if (result != null)
            {

                return this.StatusCode(StatusCodes.Status200OK, result);

            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred....");
        }



        [HttpGet("GetFeesByAppId")]
        public async Task<ActionResult> GetFeesByAppId(int AppId)
        {
            var result = await _IPaymentRepository.GetApplicationFeesByAppIdAsync(AppId);
            if (result != null)
            {

                return this.StatusCode(StatusCodes.Status200OK, result);

            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred....");
        }


        [HttpPost]
        public async Task<ActionResult> AddApplicationFeesAsync(PaymentDetailsDto PaymentDetailsDto, [FromHeader] string lang)
        {
            await _IPaymentRepository.AddApplicationFeesAsync(PaymentDetailsDto, lang);   
            return this.StatusCode(StatusCodes.Status200OK,"Fees were added ...");        
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteApplicationFeesAsync(int id)
        {
            bool result = await _IPaymentRepository.DeleteApplicationFeesAsync(id);
            if (result)
            {

                return this.StatusCode(StatusCodes.Status200OK, result);

            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred....");
        }

        /* TODO delete payment !! need more validation
        [HttpDelete("DeleteFeesByAppId")]
        public async Task<ActionResult> DeleteFeesByAppId(int AppId)
        {
            bool result = await _IPaymentRepository.DeleteApplicationFeesByAppIdAsync(AppId);
            if (result)
            {

                return this.StatusCode(StatusCodes.Status200OK, result);

            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred....");
        }
        */

    }
}
