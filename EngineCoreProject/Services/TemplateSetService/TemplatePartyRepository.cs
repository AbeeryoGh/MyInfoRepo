using EngineCoreProject.Services;
using EngineCoreProject.DTOs.TemplateSetDtos;
using EngineCoreProject.IRepository.ITemplateSetRepository;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.TemplateSetService
{
    public class TemplatePartyRepository : ITemplatePartyRepository
    {
        private readonly EngineCoreDBContext   _EngineCoreDBContext;
        private readonly IGeneralRepository _IGeneralRepository;


        public TemplatePartyRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _IGeneralRepository = iGeneralRepository;
        }

        public async Task<List<TemplateParty>> GetAll(int? templateId)
        {
            Task<List<TemplateParty>> query = null;
            if (templateId == null)
                query = _EngineCoreDBContext.TemplateParty.ToListAsync();
            else
                query = _EngineCoreDBContext.TemplateParty.Where(s => s.TemplateId == templateId).ToListAsync();
            return await query;
        }

        public async Task<TemplateParty> GetOne(int id)
        {
            id = Convert.ToInt32(id);
            var query = _EngineCoreDBContext.TemplateParty.Where(x => x.Id == id);
            return await query.FirstOrDefaultAsync();
        }



        public async Task<int> DeleteOne(int id)
        {
            TemplateParty templateParty = await GetOne(id);
            if (templateParty == null)
                return Constants.NOT_FOUND;
            try
            {
                _IGeneralRepository.Delete(templateParty);
                if (await _IGeneralRepository.Save())
                    return Constants.OK;
            }
            catch (Exception) { return Constants.ERROR; }
            return Constants.ERROR;
        }

        public async Task<int> Add(TemplatePartyDto templatePartyDto)
        {
            try
            {
                TemplateParty templateParty = new TemplateParty
                {
                    TemplateId = templatePartyDto.TemplateId,
                    PartyId = templatePartyDto.PartyId,
                    Required = templatePartyDto.Required,
                    SignRequired= templatePartyDto.SignRequired
                };

                _IGeneralRepository.Add(templateParty);
                if (await _IGeneralRepository.Save())
                {
                    return templateParty.Id;
                }
            }
            catch (Exception )
            {
                return Constants.ERROR;
                
            }

            return Constants.ERROR;
        }

        public async Task<int> Update(int id, TemplatePartyDto templatePartyDto)
        {
            TemplateParty templateParty = await GetOne(id);
            if (templateParty == null)
                return Constants.NOT_FOUND;
            try
            {
                templateParty.TemplateId = templatePartyDto.TemplateId;
                templateParty.PartyId = templatePartyDto.PartyId;
                templateParty.Required = templatePartyDto.Required;
                templateParty.SignRequired = templatePartyDto.SignRequired;

                _IGeneralRepository.Update(templateParty);
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

        public async Task<List<int>> DeleteMany(int[] ids)
        {
            List<int> FailedDeletedList = new List<int>();
            for (int i = 0; i < ids.Length; i++)
            {
                TemplateParty templateParty = await _EngineCoreDBContext.TemplateParty.Where(x => x.Id == ids[i]).FirstOrDefaultAsync();
                if (templateParty != null)
                {
                    
                        try
                        {
                            _IGeneralRepository.Delete(templateParty);
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
    }
}
