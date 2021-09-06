using EngineCoreProject.DTOs.TemplateSetDtos;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.ITemplateSetRepository
{
    public interface ITermRepository
    {

        Task<List<Term>> GetAll(int? templateId);
        Task<Term> GetOne(int id);
        Task<List<int>> DeleteMany(int[] ids);
        Task<int> DeleteOne(int id);
        Task<int> Add(TermDto termDto);
        Task<int> Update(int id, TermDto termDto);
    }
}
