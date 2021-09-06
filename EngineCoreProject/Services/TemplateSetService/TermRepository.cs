using EngineCoreProject.Services;
using EngineCoreProject.DTOs.TemplateSetDtos;
using EngineCoreProject.IRepository.ITemplateSetRepository;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.TemplateSetService
{
    public class TermRepository : ITermRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _IGeneralRepository;


        public TermRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _IGeneralRepository = iGeneralRepository;
        }

        public async Task<List<Term>> GetAll(int? templateId)
        {
            /*var query = _EngineCoreDBContext.Term.ToListAsync();
            return await query;*/
            Task<List<Term>> query = null;
            if (templateId == null)
                query = _EngineCoreDBContext.Term.ToListAsync();
            else
                query = _EngineCoreDBContext.Term.Where(s => s.TemplateId == templateId).ToListAsync();
            return await query;
        }

        public async Task<Term> GetOne(int id)
        {
            id = Convert.ToInt32(id);
            var query = _EngineCoreDBContext.Term.Where(x => x.Id == id);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<int>> DeleteMany(int[] ids)
        {
            List<int> FailedDeletedList = new List<int>();
            for (int i = 0; i < ids.Length; i++)
            {
                Term term = await _EngineCoreDBContext.Term.Where(x => x.Id == ids[i]).FirstOrDefaultAsync();
                if (term != null)
                {
                    try
                    {
                        _IGeneralRepository.Delete(term);
                         await _IGeneralRepository.Save();
                    }
                    catch (Exception)
                       {
                        FailedDeletedList.Add(ids[i]);
                       }
                }
                else
                    FailedDeletedList.Add(ids[i]);
            }
            return FailedDeletedList;
        }

        public async Task<int> DeleteOne(int id)
        {
            Term term = await GetOne(id);
            if (term == null)
                return Constants.NOT_FOUND;
            try
            {
                _IGeneralRepository.Delete(term);
                if (await _IGeneralRepository.Save())
                    return Constants.OK;
            }
            catch (Exception)
            {
                return Constants.ERROR;
            }
            return Constants.ERROR;

        }

        public async Task<int> Add(TermDto termDto)
        {
            try
            {
                Term term = new Term
                {
                    Title       = termDto.Title,
                    Content     = termDto.Content,
                    TemplateId  = termDto.TemplateId,
                    CreatedDate = DateTime.Now,
                   
                };

                _IGeneralRepository.Add(term);
                if (await _IGeneralRepository.Save())
                {
                    return term.Id;
                }
            }
            catch (Exception)
            {
                return Constants.ERROR;
            }

            return Constants.ERROR;
        }

        public async Task<int> Update(int id, [FromBody] TermDto termDto)
        {
            Term term = await GetOne(id);
            if (term == null)
                return Constants.NOT_FOUND;
            try
            {
                term.Content    = termDto.Content;
                term.Title      = termDto.Title;
                term.TemplateId = termDto.TemplateId;
                

                _IGeneralRepository.Update(term);
                if (await _IGeneralRepository.Save())
                {
                    return Constants.OK;
                }
            }
            catch (Exception)
            {
                return Constants.ERROR;
            }
            return Constants.ERROR;
        }
    }
}
