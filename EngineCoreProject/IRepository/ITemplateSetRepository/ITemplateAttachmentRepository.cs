using EngineCoreProject.DTOs.TemplateSetDtos;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.ITemplateSetRepository
{
    public interface ITemplateAttachmentRepository
    {
        Task<List<TemplateAttachment>> GetAll(int? templateId);
        Task<TemplateAttachment> GetOne(int id);
        Task<List<int>> DeleteMany(int[] ids);
        Task<int> DeleteOne(int id);
        Task<int> Add(TemplateAttachmentDto templateDto);
        Task<int> Update(int id, TemplateAttachmentDto templateDto);
    }
}
