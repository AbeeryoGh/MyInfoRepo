using EngineCoreProject.Services;
using EngineCoreProject.DTOs.NotificationDtos;
using EngineCoreProject.IRepository.INotificationSettingRepository;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EngineCoreProject.DTOs.ActionDtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace EngineCoreProject.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class NotificationSettingController : ControllerBase
    {
        private readonly INotificationSettingRepository _iNotificationSettingRepository;
        private readonly IGeneralRepository _iGeneralRepository;

        public NotificationSettingController(INotificationSettingRepository iNotificationRepository, IGeneralRepository iGeneralRepository)
        {
            _iNotificationSettingRepository = iNotificationRepository;
            _iGeneralRepository = iGeneralRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("AddNotificationTemplate")]
        public async Task<ActionResult> AddNotificationTemplate([FromBody] NotificationTemplateWithDetailsPostDto notificationTemplatePostDto, [FromHeader] string lang)
        {
            var result = await _iNotificationSettingRepository.AddNotificationTemplateWithDetails(notificationTemplatePostDto, lang);

            if (result != 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("AddNotificationActions")]
        public async Task<ActionResult> AddNotificationActions([FromBody] NotificationActionPostDto notificationActionPostDto, int notificationId, [FromHeader] string lang)
        {
            var result = await _iNotificationSettingRepository.AddNotificationAction(notificationActionPostDto, notificationId);

            if (result != null)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("AddNotificationsToOneAction")]
        public async Task<ActionResult> AddNotificationsToOneAction(NotificationTemplatesActionPostDto notificationTemplatesAction, [FromHeader] string lang)
        {
            var result = await _iNotificationSettingRepository.AddNotificationTemplatesToOneAction(notificationTemplatesAction);

            if (result != 0)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }

            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "failedAdd"));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("{templateId}")]
        public async Task<ActionResult> DeleteTemplateId(int templateId, [FromHeader] string lang)
        {

            var result = await _iNotificationSettingRepository.DeleteNotificationTemplate(templateId);
            if (result == true)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            else return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
        }

       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("{notificationId}")]
        public async Task<ActionResult> GetNotificationTemplateDetails(int notificationId)
        {
            var result = await _iNotificationSettingRepository.GetAllNotificationDetails(notificationId);

            if (result != null)
            {

                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet()]
        public async Task<ActionResult> GetAllNotificationTemplates()
        {
            var result = await _iNotificationSettingRepository.GetAllNotificationTemplates();
            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPut("{notificationTemplateId}")]
        public async Task<ActionResult> Put(NotificationTemplateWithDetailsPostDto notificationTemplateWithDetailsPostDto, int notificationTemplateId)
        {
            var result = await _iNotificationSettingRepository.EditNotificationTemplateDetials(notificationTemplateWithDetailsPostDto, notificationTemplateId);

            if (result != 0)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("parameterList")]
        public ActionResult GetparameterList()
        {
            var result =  _iNotificationSettingRepository.GetParameterList();

            if (result != null)
            {

                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetNotificationsDetailsForAction")]
        public async Task<ActionResult> GetNotificationsForAction(int actionId)
        {
            var result = await _iNotificationSettingRepository.GetNotificationsForAction(actionId);

            if (result != null)
            {
                return this.StatusCode(StatusCodes.Status200OK, result);
            }
            else return this.StatusCode(StatusCodes.Status404NotFound, "error occurred");
        }


        [HttpGet("GetAllNotificationDetailsLans")]
        public async Task GetAllNotificationDetailsLans(string lang)
        {
            await _iNotificationSettingRepository.GetAllNotificationDetailsLans(lang);;
        }

    }
}
