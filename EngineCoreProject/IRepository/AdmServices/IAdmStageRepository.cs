using EngineCoreProject.DTOs.AdmService;
using EngineCoreProject.DTOs.TemplateSetDtos.ModelView;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.AdmServices
{
    public interface IAdmStageRepository
    {
        Task<List<AdmStage>> GetAll();
        Task<AdmStage> GetOne(int id);
        Task<List<StageNamesDto>> GetStageNames(string lang);
        Task<List<StageNamesDto>> getonestage(int? id, string lang);
        Task<int> delete(int id,string lang);
        Task<List<serviceAllLang>> GetstageAlllang(string shortcut);
        Task<AdmStage> add(postStageDto postStage);
        Task<int> update(int id, updatetStageDto updatetStage);
        //------By yhab------------
        Task<List<AttachmentView>> GetRelatedAttachments(int templateId, string lang);
        Task<List<int>> GetPayStagesId();


    }
}
