using EngineCoreProject.DTOs.AdmService;
using EngineCoreProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.AdmServices
{
    public interface IStageMasterAttachmentRepository
    {
        Task<List<StageMasterAttachment>> getall();
        Task<StageMasterAttachment> getone(int id);
        Task<List<StageAttachmentDto>> getstageattach(int id, string lang);
        Task<int> delete(int id);
        Task<List<int>> DeleteMany(int[] ids);
        Task<int> update(int id, updateStageAttachDto stageAttachDto);
        Task<List<StageMasterAttachment>> add(List<postStageAttachmentDto> postStageAttachments);



    }
}
