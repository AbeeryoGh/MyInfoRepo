using EngineCoreProject.DTOs.AdmService;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.AdmServices;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.Admservice
{
    public class StageMasterAttachmentRepository : IStageMasterAttachmentRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly ISysValueRepository _ISysValueRepository;

        public StageMasterAttachmentRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository, ISysValueRepository iSysValueRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _ISysValueRepository = iSysValueRepository;

        }
        public async Task<List<StageMasterAttachment>> getall()
        {
            List<StageMasterAttachment> stageMasterAttachments = await _EngineCoreDBContext.StageMasterAttachment.ToListAsync();
            return stageMasterAttachments;
        }

        public async Task<StageMasterAttachment> getone(int id)
        {
            var query = await _EngineCoreDBContext.StageMasterAttachment.Where(x => x.Id == id).FirstOrDefaultAsync();
            return query;
        }
        

     
        public async Task<List<StageAttachmentDto>> getstageattach(int id, string lang)
        {
            string answerYes = "", answerNo = "";
            answerYes = await _ISysValueRepository.GetValueByShortcut("Yes", lang);
            answerNo = await _ISysValueRepository.GetValueByShortcut("No", lang);
            Task<List<StageAttachmentDto>> query = null;
            query = (from ta in _EngineCoreDBContext.StageMasterAttachment
                     join lv in _EngineCoreDBContext.SysLookupValue
                         on ta.MasterAttachmentId equals lv.Id
                     join tr in _EngineCoreDBContext.SysTranslation
                         on lv.Shortcut equals tr.Shortcut

                     where ta.StageId == id
                     where tr.Lang == lang
                     select new StageAttachmentDto
                     {
                         stageId=id,
                         AttachmentId = ta.MasterAttachmentId,
                         RelationId = ta.Id,
                         TranslationId = tr.Id,
                         AttachmentName = tr.Value,
                         Required = (bool)ta.Required,
                         RequiredText = ta.Required == true ? answerYes : answerNo,                                 

                     }).ToListAsync();

            return await query;
        }

        public async Task<int> delete(int id)
        {
            StageMasterAttachment stageMasterAttachment = await getone(id);
            if (stageMasterAttachment == null)
                return Constants.NOT_FOUND;
            try
            {
                _iGeneralRepository.Delete(stageMasterAttachment);
                if (await _iGeneralRepository.Save())
                    return Constants.OK;
            }
            catch (Exception) { return Constants.ERROR; }
            return Constants.ERROR;


        }

       public async Task<List<int>> DeleteMany(int[] ids)
        {
            List<int> FailedDeletedList = new List<int>();
            for (int i = 0; i < ids.Length; i++)
            {
                StageMasterAttachment stageMasterAttachment = await _EngineCoreDBContext.StageMasterAttachment.Where(x => x.Id == ids[i]).FirstOrDefaultAsync();
                if (stageMasterAttachment != null)
                {
                    try
                    {
                        _iGeneralRepository.Delete(stageMasterAttachment);
                        await _iGeneralRepository.Save();
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

        public async Task<int> update(int id,updateStageAttachDto stageAttachDto)
        {
                StageMasterAttachment stageMasterAttachment = await getone(id);
                if (stageMasterAttachment == null)
                    return Constants.NOT_FOUND;
                try
                {

                    stageMasterAttachment.MasterAttachmentId = stageAttachDto.attachid;
                    stageMasterAttachment.Required = stageAttachDto.required;

                    // template.LastUpdatedDate = DateTime.Now;

                    _iGeneralRepository.Update(stageMasterAttachment);
                    if (await _iGeneralRepository.Save())
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

        public async Task<List<StageMasterAttachment>> add(List<postStageAttachmentDto> postStageAttachments)
        {
            List<StageMasterAttachment> stageMasterAttachmentslist = new List<StageMasterAttachment>();
            foreach (postStageAttachmentDto stageattach in postStageAttachments)
            {

                StageMasterAttachment stageMasterAttachments = new StageMasterAttachment();
                stageMasterAttachments.StageId = stageattach.stageId;
                stageMasterAttachments.MasterAttachmentId = stageattach.attachId;
                stageMasterAttachments.Required = stageattach.required;
                stageMasterAttachments.CreatedDate = DateTime.Now;

                _iGeneralRepository.Add(stageMasterAttachments);
                stageMasterAttachmentslist.Add(stageMasterAttachments);
                if (!await _iGeneralRepository.Save())
                    return stageMasterAttachmentslist;
            }

            return stageMasterAttachmentslist;
        }
    }
    }
    

