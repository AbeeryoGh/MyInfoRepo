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
    public class TemplateAttachmentRepository : ITemplateAttachmentRepository
    {
        private readonly EngineCoreDBContext   _EngineCoreDBContext;
        private readonly IGeneralRepository _IGeneralRepository;


        public TemplateAttachmentRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _IGeneralRepository = iGeneralRepository;
        }


        public async Task<List<TemplateAttachment>> GetAll(int? templateId)
        {
            Task<List<TemplateAttachment>> query = null;
            if (templateId == null)
                query = _EngineCoreDBContext.TemplateAttachment.ToListAsync();
            else
                query = _EngineCoreDBContext.TemplateAttachment.Where(s => s.TemplateId == templateId).ToListAsync();
            return await query;
        }

        public async Task<TemplateAttachment> GetOne(int id)
        {
            id = Convert.ToInt32(id);
            var query = _EngineCoreDBContext.TemplateAttachment.Where(x => x.Id == id);
            return await query.FirstOrDefaultAsync();
        }
        public async Task<int> Add(TemplateAttachmentDto templateAttachmentDto)
        {
            try
            {
                TemplateAttachment templateAttachment = new TemplateAttachment
                {
                    TemplateId = templateAttachmentDto.TemplateId,
                    AttachmentId = templateAttachmentDto.AttachmentId,
                    Required = templateAttachmentDto.Required,
                    //CreatedDate = DateTime.Now,
                };

                _IGeneralRepository.Add(templateAttachment);
                if (await _IGeneralRepository.Save())
                {
                    return templateAttachment.Id;
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
                TemplateAttachment templateAttachment = await _EngineCoreDBContext.TemplateAttachment.Where(x => x.Id == ids[i]).FirstOrDefaultAsync();
                if (templateAttachment != null)
                {
                    try
                    {
                        _IGeneralRepository.Delete(templateAttachment);
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
            TemplateAttachment templateAttachment = await GetOne(id);
            if (templateAttachment == null)
                return Constants.NOT_FOUND;
            try
            {
                _IGeneralRepository.Delete(templateAttachment);
                if (await _IGeneralRepository.Save())
                    return Constants.OK;
            }
            catch (Exception) { return Constants.ERROR; }
            return Constants.ERROR;
        }
        public async Task<int> Update(int id, TemplateAttachmentDto templateDto)
        {
            TemplateAttachment templateAttachment = await GetOne(id);
            if (templateAttachment == null)
                return Constants.NOT_FOUND;
            try
            {
                templateAttachment.TemplateId = templateDto.TemplateId;
                templateAttachment.AttachmentId = templateDto.AttachmentId;
                templateAttachment.Required = templateDto.Required;
               
                _IGeneralRepository.Update(templateAttachment);
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
