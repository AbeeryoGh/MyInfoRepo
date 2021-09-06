using EngineCoreProject.DTOs.AdmService;
using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using EngineCoreProject.DTOs.AramexDto;
using EngineCoreProject.DTOs.FileDto;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.AdmServices
{
    public interface IAdmServiceRepository
    {
        Task<List<AdmService>> GetAll();
        Task<AdmService> GetOne(int id);
        Task<ServiceNamesDto> GetOnename(int id, string lang);
        Task<int> GetKindNo(int id);
        Task<List<ServiceNamesDto>> GetserviceNAmes(string lang);
        Task<AdmService> add();
        Task<List<servicestagesDto>> getsatgesofservice(int id, string lang);
        Task<List<serviceAllLang>> GetserviceAlllang(string shortcut);
        // Task<UploadedFileMessage> UploadserviceIcon(int id, IFormFile file);
        Task<UploadedFileMessage> Upload(IFormFile File);
        Task<object> ChangeServiceManual(int serviceId, IFormFile File, string lang);
        Task<int> delete(int id, string lang);
        Task<string> GetApprovalText(int id);
        Task<List<int>> GetStagesIds(int id);
        Task<List<RelatedContentView>> GetRelatedContents(int serviceId, string lang);
        //  Task<ServiceNamesDto> GetServiceSpecifications(int id);
        Task<List<int>> GetDoneStagesId();
        Task<List<int>> GetReviewStagesId();
        Task<List<int>> GetInterviewStagesId();
        Task<int> FirstStage(int? serviceId, int stageOrder = 1);
        Task<int> DeleteAppById(int AppId);
        string LastNotary(int AppId);
        Task<bool> AddAramexRequest(AramexPostDto aramexPostDto);
    }
}
