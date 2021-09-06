using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AccountDto
{
 
    public class EmployeeSettingPostDto
    {
        public int EnotaryId { get; set; }
        public string EposUserId { get; set; }
        public string EntityCode { get; set; }
        public string TerminalId { get; set; }
        public string SessionToken { get; set; }
        public string Tokens { get; set; }
        public DateTime? ExpiredSessionToken { get; set; }
        public string Channel { get; set; }
        public string LocationCode { get; set; }
        public int NotaryLocationId { get; set; }
        public string TransactionReference { get; set; }
        public string SourceReference { get; set; }
        public string ActiveDirectoryAccount { get; set; }
        public bool? DefaultShowCam { get; set; }
        public bool? DefaultMuteVoice { get; set; }
        public bool? DefaultViewCards { get; set; }


        public EmployeeSetting GetEntity()
        {
            EmployeeSetting employeeSetting = new EmployeeSetting
            {
                UserId = EposUserId,
                ActiveDirectoryAccount = ActiveDirectoryAccount,
                Channel = Channel,
                DefaultMuteVoice = DefaultMuteVoice,
                DefaultShowCam = DefaultMuteVoice,
                DefaultViewCards = DefaultViewCards,
                EntityCode = EntityCode,
                ExpiredSessionToken = ExpiredSessionToken,
                LocationCode = LocationCode,
                SessionToken = SessionToken,
                SourceReference = SourceReference,
                TerminalId = TerminalId,
                Tokens = Tokens,
                TransactionReference = TransactionReference,
                NotaryLocationId = NotaryLocationId,
                EnotaryId = EnotaryId

            };

            return employeeSetting;
        }
    }
}
