using EngineCoreProject.DTOs.EPOSMachineDto;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IEPOSMachineRepository
{
    public interface IEPOSMachine
    {
        public Task<ResLoginDto> MerchantLoginAsync(string lang, int UserId);
        public Task<ResQueryPriceDto> QueryServicePriceAsync(ReqQueryPriceDto ReqQueryPriceDto, int notaryId, string lang);
        public Task<EmployeeSetting> GetEposSetting(string lang, int UserId);
        public Task UpdateEposSetting(string lang, EPOSMachineSettingDto EPOSMachineSettingDto);
        public Task<ResQueryURN> CheckUniqueReferenceNumberStatus(string lang, int NotaryId, string UniqueReferenceNumber);

        public Task<ResLoginDto> QueryTokenAsync(string lang);
    }
}
