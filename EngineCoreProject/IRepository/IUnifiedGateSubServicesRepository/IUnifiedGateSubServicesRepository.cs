
using EngineCoreProject.DTOs.AccountDto;
using EngineCoreProject.DTOs.UnifiedGateDto;
using EngineCoreProject.DTOs.UnifiedGatePostDto;
using EngineCoreProject.Models;

using PaymentServicePro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IUnifiedGateSubServicesRepository
{
    public interface IUnifiedGateSubServicesRepository
    {
        UnifiedGateSubServicesDto GetSubServicesFromUnifiedGate();
        UnifiedGateUserInformationDto GetUserInformationUnifiedGate(UnifiedGateUserInformationPostDto userInfo);
        Task<PaymentGetStatusResponse> GetPaymentStatusAsync(string pruchaseId, string secureHash, int entityId);
        string GenerateURLForSignInWithUGate(string lang, string EL_Service, string theme);
        Task<AdmService> GetServiceByUID(int ugId);
        Task<string> AccessAccountOnUnifiedGate(string lang, string EL_Eid);
        string RemoteSignOut();//string lang, string EL_Eid
        Task<LogInResultDto> remotelogin(RemoteLoginDto remoteLoginDto, string lang);
        Task<UserResultDto> UpdateOrInsertUserInfoFromUnifiedGate(UnifiedGateUserInformationPostDto userInfo, string lang);
        string RedirectToUGDahBoard(string Lang, string theme);
        Task<responseCardDashBoardDto> statisticsDashboard(DashBoardRequestDto dashBoardRequestDto);
        Task<responseTableDashBoardDto> statisticsTable(DashBoardRequestDto dashBoardRequestDto);
    }
}
