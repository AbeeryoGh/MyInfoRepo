using EngineCoreProject.DTOs.AdmService;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.AdmServices
{
    public interface IStageActionsRepository
    {
       
       Task<AdmStageAction> GetStageById(int id);
       Task<List<StageActionDto>> GetStageAction(int id, string lang);
        Task<int> Delete(int id);
        Task<List<ActionDetialsForNotification>> GetActionDetialsForNotification(int id,string lang);
        Task<AdmStageAction> Add(postActionDto postAction);
        Task<dynamic> GetActionstoRole(List<int> actionIds, string lang);
    }
}
