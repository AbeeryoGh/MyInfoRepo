using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AccountDto
{

    public class EmployeeSettingDefaultPostDto
    {
        public int UserId { get; set; }
        public string EntityCode { get; set; }
        public string TerminalId { get; set; }
        public string SessionToken { get; set; }
        public string Tokens { get; set; }
        public DateTime? ExpiredSessionToken { get; set; }
        public string Channel { get; set; }
        public string LocationCode { get; set; }
        public string TransactionReference { get; set; }
        public string SourceReference { get; set; }
        public int EnotaryId { get; set; }

        public int? EnotaryLocationId { get; set; }
        public string EnotaryLocationName { get; set; }

        public string ActiveDirectoryAccount { get; set; }

        public bool? DefaultShowCam { get; set; }
        public bool? DefaultMuteVoice { get; set; }
        public bool? DefaultViewCards { get; set; }

    }


    public class EmployeeSettingGetDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string EntityCode { get; set; }
        public string TerminalId { get; set; }
        public string SessionToken { get; set; }
        public string Tokens { get; set; }
        public DateTime? ExpiredSessionToken { get; set; }
        public string Channel { get; set; }
        public string LocationCode { get; set; }
        public string TransactionReference { get; set; }
        public string SourceReference { get; set; }
        public int EnotaryId { get; set; }

        public int? EnotaryLocationId { get; set; }
        public string EnotaryLocationName { get; set; }

        public string ActiveDirectoryAccount { get; set; }
        public bool? DefaultShowCam { get; set; }
        public bool? DefaultMuteVoice { get; set; }
        public bool? DefaultViewCards { get; set; }


        public static EmployeeSettingGetDto GetDto(EmployeeSetting employeeSetting)
        {
            EmployeeSettingGetDto EmployeeSettingGetDto = new EmployeeSettingGetDto
            {
                ActiveDirectoryAccount = employeeSetting.ActiveDirectoryAccount,
                EnotaryId = employeeSetting.EnotaryId,
                Channel = employeeSetting.Channel,
                DefaultMuteVoice = employeeSetting.DefaultMuteVoice,
                DefaultShowCam = employeeSetting.DefaultShowCam,
                DefaultViewCards = employeeSetting.DefaultViewCards,
                EntityCode = employeeSetting.EntityCode,
                ExpiredSessionToken = employeeSetting.ExpiredSessionToken,
                LocationCode = employeeSetting.LocationCode,
                TerminalId = employeeSetting.TerminalId,
                SessionToken = employeeSetting.SessionToken,
                SourceReference = employeeSetting.SourceReference,
                Tokens = employeeSetting.Tokens,
                TransactionReference = employeeSetting.TransactionReference,
                UserId = employeeSetting.UserId,
                Id = employeeSetting.Id,
                EnotaryLocationId = employeeSetting.NotaryLocationId

            };
            return EmployeeSettingGetDto;
        }
    }
}
