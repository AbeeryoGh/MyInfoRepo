using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.ApplicationDtos.RelatedContent;
using EngineCoreProject.DTOs.EPOSMachineDto;
using EngineCoreProject.DTOs.Payment;
using EngineCoreProject.DTOs.PDFGenerator;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.IRepository.IGeneratorRepository;
using EngineCoreProject.IRepository.IPaymentRepository;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.Models;
using EngineCoreProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.Payment
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentSettings _paymentSettings;
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IPaymentRepository _PaymentRepository;
        private readonly IGeneralRepository _IGeneralRepository;
        private readonly IApplicationRepository _IApplicationRepository;
        private readonly IUserRepository _IUserRepository;
        private readonly IGenerator _iGenerator;
        private readonly ITransactionRepository _ITransactionRepository;
       
        private readonly IConfiguration _IConfiguration;

        public PaymentController( ITransactionRepository iTransactionRepository, IGenerator iGenerator, IApplicationRepository iApplicationRepository, IGeneralRepository iGeneralRepository, EngineCoreDBContext EngineCoreDBContext, IPaymentRepository paymentRepository, IOptions<PaymentSettings> paymentSettings, IUserRepository iUserRepository, IConfiguration iConfiguration)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _paymentSettings = paymentSettings.Value;
            _IGeneralRepository = iGeneralRepository;
            _PaymentRepository = paymentRepository;
            _IApplicationRepository = iApplicationRepository;
            _IUserRepository = iUserRepository;
            _iGenerator = iGenerator;
            _ITransactionRepository = iTransactionRepository;
            _IConfiguration = iConfiguration;
        }

        [HttpGet("UpdatePaymentTableCrons")]
        public async Task<ActionResult> UpdatePaymentTableCrons()
        {
            await _PaymentRepository.UpdatePaymentTableCrons();
            return this.StatusCode(StatusCodes.Status200OK, "ok");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetMyPayment")]
        public async Task<ActionResult> GetMyPayment(int UserId, [FromHeader] string lang)
        {
            var result = await _PaymentRepository.GetMyPayment(UserId, lang);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("GenerateURL")]
        public async Task<ActionResult> GenerateURL(int applicationId, [FromHeader] string lang, int actionId)
        {
            var userAgent = Request.Headers["User-Agent"];
            string description = _IGeneralRepository.GetDecviceInfo(userAgent);
            string result = await _PaymentRepository.GenerateURL(applicationId, lang, description, _IUserRepository.GetUserID(), actionId);
            return this.StatusCode(StatusCodes.Status200OK, result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetPaymentsBetweenTwoDates")]
        public async Task<ActionResult> GetPaymentsBetweenTwoDates(DateTime startDate, DateTime endDate, string paymentType)
        {
            var result = await _PaymentRepository.GetPaymentsBetweenTwoDates(startDate, endDate, paymentType);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetPaymentDetailsById")]
        public async Task<ActionResult> GetPaymentDetailsById(int paymentId)
        {
            var result = await _PaymentRepository.GetPaymentDetailsById(paymentId);
            return Ok(result);
        }


        [HttpPost("returnValue")]
        public async Task<ActionResult> returnValueAsync()
        {
            string lang = Request.Headers["lang"].ToString().ToLower();
            if (lang == "")
            {
                lang = "en";
            }

            List<PaymentResponseDtoList> myDeserializedClass = JsonConvert.DeserializeObject<List<PaymentResponseDtoList>>(JsonConvert.SerializeObject(Request.Form.ToList()));

            string URLString = await _PaymentRepository.AutoPay(myDeserializedClass, lang);
            return Redirect(URLString);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("GetPaymentStatus")]
        public async Task<ActionResult> GetPaymentStatusAsync([FromBody] GetPaymentStatusDto getPaymentStatusDto)
        {
            var result = await _PaymentRepository.GetPaymentStatus(getPaymentStatusDto);
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("GetPaymentInfo")]
        public async Task<ActionResult> GetPaymentInfo(int PaymentId, [FromHeader] string lang)
        {
            var result = await _PaymentRepository.GetPaymentInfo(PaymentId, lang);
            return Ok(result);
        }
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("GetPaymentsInfoByAppId")]
        public async Task<ActionResult> GetPaymentsInfoByAppId(int appId, [FromHeader] string lang)
        {
            var result = await _PaymentRepository.GetPaymentsInfoByAppId(appId, lang);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("getResponseHash")]
        public ActionResult getResponseHash([FromBody] List<PaymentResponseDtoList> myDeserializedClass)
        {
            var result = _PaymentRepository.getResponseHash(myDeserializedClass);
            return Ok(result);
        }  

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("ManualPay")]
        public async Task<ActionResult> ManualPay(ManualPaymentDetialsDto manualPaymentDetials, [FromHeader] string lang)
        {
            var obj = await _PaymentRepository.ManualPay(manualPaymentDetials, lang);
            return Ok(obj);
        }



        [HttpPost("EposPayStatus")]
        public async Task<ActionResult> EposPayStatus([FromBody] ApplicationID AppId, [FromHeader] string lang)
        {
            ResQueryURNDto result = await _PaymentRepository.QueryURNAsync(AppId.AppId, lang);
            return Ok(result);
        }
    }
}
