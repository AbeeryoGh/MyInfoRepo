using EngineCoreProject.DTOs.TemplateSetDtos;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.ITemplateSetRepository
{
    public interface ITemplatePartyRepository
    {
        Task<List<TemplateParty>> GetAll(int? templateId);
      
        Task<TemplateParty> GetOne(int id);
        Task<List<int>> DeleteMany(int[] ids);
        Task<int> DeleteOne(int id);
        Task<int> Add(TemplatePartyDto templateDto);
        Task<int> Update(int id, TemplatePartyDto templateDto);
    }
}
