using EngineCoreProject.DTOs.AdmService;
using EngineCoreProject.DTOs.TemplateSetDtos.ModelView;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.AdmServices;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services
{
    public class AdmStageRepository : IAdmStageRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly ISysValueRepository _ISysValueRepository;
        ValidatorException _exception;

        public AdmStageRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository, ISysValueRepository iSysValueRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _ISysValueRepository = iSysValueRepository;
            _exception = new ValidatorException();

        }

        


        public async Task<List<AdmStage>> GetAll()
        {
            List<AdmStage> admStages = await _EngineCoreDBContext.AdmStage.ToListAsync();
            /*var query = _EngineCoreDBContext.AdmService;
            return await query.ToListAsync();*/
            return admStages;
        }

        public async Task<AdmStage> GetOne(int id)
        {
            id = Convert.ToInt32(id);
            AdmStage admStage = await _EngineCoreDBContext.AdmStage.Where(x => x.Id == id).FirstOrDefaultAsync();//.Include(s => s.AdmServiceStage).FirstOrDefaultAsync();

            return admStage;
        }
        public async Task<List<StageNamesDto>> GetStageNames(string lang)
        {
            Task<List<StageNamesDto>> query = null;
            query = (from stg in _EngineCoreDBContext.AdmStage
                     join t in _EngineCoreDBContext.SysTranslation
                         on stg.Shortcut equals t.Shortcut




                     where t.Lang == lang

                     select new StageNamesDto
                     {
                         Id = stg.Id,
                         stagename = t.Value,
                         serviecid=stg.ServiceId,
                         createddate = stg.CreatedDate,
                         lastupdateddate = stg.LastUpdatedDate

                     }).ToListAsync();

            return await query;
        }
       
        public async Task<List<StageNamesDto>> getonestage(int? id, string lang)
        {

            Task<List<StageNamesDto>> query = null;
           
            query = (from stageInfo in (
             (from stg in _EngineCoreDBContext.AdmStage
              join tr in _EngineCoreDBContext.SysTranslation on stg.Shortcut equals tr.Shortcut
              where tr.Lang == lang
              where stg.Id==id
                       select new
                       {
                           orderno=stg.OrderNo,
                           stageid = stg.Id,
                           stg.Shortcut,
                           stg.ServiceId,
                           tr.Lang,
                           stagename = tr.Value,
                           stg.StageTypeId,
                           periodtolate=stg.PeriodForLate,
                           periodtoarchive=stg.PeriodForArchive
                       }))
                     join lv in _EngineCoreDBContext.SysLookupValue on new { stage_type_id = (int)stageInfo.StageTypeId } equals new { stage_type_id = lv.Id }
                     join tr1 in _EngineCoreDBContext.SysTranslation on lv.Shortcut equals tr1.Shortcut
                     where tr1.Lang == lang
                     select new StageNamesDto
                     {
                         orderno= (int)stageInfo.orderno,
                         Id =stageInfo.stageid,
                         shortcut= stageInfo.Shortcut,
                         stagename= stageInfo.stagename,
                         stagetypeid= stageInfo.StageTypeId,
                         stagetypeshortcut = lv.Shortcut,
                         stagetypename=tr1.Value,
                         periodtolate=stageInfo.periodtolate,
                         periodtoarchive=stageInfo.periodtoarchive
                        
                     }).ToListAsync();


            return await query;
        }

        public async Task<int> delete(int id,string lang)
        {
            AdmStage admStage = await GetOne(id);
                if (admStage == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "stageNotFound"));
                throw _exception;
            }
            Application appCurrentSatge = _EngineCoreDBContext.Application.Where(x => x.CurrentStageId == id).FirstOrDefault();
            if (appCurrentSatge != null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "currentAppStage"));
                throw _exception;
            }

            AdmStageAction admStageAction = _EngineCoreDBContext.AdmStageAction.Where(x => x.StageId == id).FirstOrDefault();
            if (admStageAction !=null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "stageAction"));
                throw _exception;
            }

            StageMasterAttachment stageattachment = _EngineCoreDBContext.StageMasterAttachment.Where(x => x.StageId == id).FirstOrDefault();
            if (stageattachment!=null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "stageAttachment"));
                throw _exception;
            }
               
            try
            {
                _iGeneralRepository.Delete(admStage);
                    if (await _iGeneralRepository.Save())
                    return Constants.OK;
            }
            catch (Exception) { return Constants.ERROR; }
            return Constants.ERROR;
        }

        public async Task<List<serviceAllLang>> GetstageAlllang(string shortcut)
        {
            Task<List<serviceAllLang>> query = null;
            query = (from t in _EngineCoreDBContext.SysTranslation
                     where t.Shortcut == shortcut
                     select new serviceAllLang
                     {
                         shortcut = t.Shortcut,
                         lang = t.Lang,
                         tarnslate = t.Value
                     }).ToListAsync();

            return await query;
        }

        public async Task<AdmStage> add(postStageDto postStage)
        {
            var query = _EngineCoreDBContext.AdmService.Where(x => x.Id == postStage.ServiceId).FirstOrDefault();
            if (query == null)
                return null;
            AdmStage admStage = new AdmStage()
            {
                Shortcut =  _iGeneralRepository.GenerateShortCut("stage", "name_shortcut"),
                ServiceId = postStage.ServiceId,
                CreatedDate = DateTime.Now


            };
            _iGeneralRepository.Add(admStage);
            if (await _iGeneralRepository.Save())
                return admStage;
            else
                return admStage;

        }

        public async Task<int> update(int id, updatetStageDto updatetStage)
        {
            AdmStage admStage = await GetOne(id);
            if (admStage != null)
                try
                {
                    admStage.LastUpdatedDate = DateTime.Now;
                    admStage.PeriodForArchive = updatetStage.PeriodForArchive;
                    admStage.PeriodForLate = updatetStage.PeriodForLate;
                    admStage.StageTypeId = updatetStage.StageTypeId;
                    admStage.Shortcut = admStage.Shortcut;
                    admStage.OrderNo = updatetStage.Order;
                    _iGeneralRepository.Update(admStage);

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

//------------------By Yhab---------Attachments------------------------------------------------------
        public async Task<List<AttachmentView>> GetRelatedAttachments(int stageId, string lang)
        {
            string answerYes = "", answerNo = "";
            answerYes = Constants.getMessage(lang, "YES");//await _ISysValueRepository.GetValueByShortcut("Yes", lang);
            answerNo  = Constants.getMessage(lang, "NO"); //await _ISysValueRepository.GetValueByShortcut("No", lang);
            Task<List<AttachmentView>> query = null;
            query = (from sa in _EngineCoreDBContext.StageMasterAttachment
                     join lv in _EngineCoreDBContext.SysLookupValue
                         on sa.MasterAttachmentId equals lv.Id
                     join tr in _EngineCoreDBContext.SysTranslation
                         on lv.Shortcut equals tr.Shortcut

                     where sa.StageId == stageId
                     where tr.Lang == lang
                     select new AttachmentView
                     {
                         AttachmentId = (int)sa.MasterAttachmentId,
                         RelationId = sa.Id,
                         TranslationId = tr.Id,
                         AttachmentName = tr.Value,
                         Required = (bool)sa.Required,
                         RequiredText = sa.Required == true ? answerYes : answerNo,

                     }).ToListAsync();

            return await query;
        }

        public async Task<List<int>> GetPayStagesId()
        {
            var payStageTypeId = await _ISysValueRepository.GetIdByShortcut("SysLookupValue_Shortcut52");
            var payStageTypeIds = await _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId == payStageTypeId).ToListAsync();
            if (payStageTypeIds != null && payStageTypeIds.Count > 0)
            {
                return payStageTypeIds.Select(x => x.Id).ToList();
            }
            return new List<int>();
        }
    }


}
