using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using EngineCoreProject.DTOs.ApplicationDtos.RelatedContent;
using EngineCoreProject.DTOs.SysLookUpDtos;
using EngineCoreProject.DTOs.TemplateSetDtos;
using EngineCoreProject.DTOs.TemplateSetDtos.ModelView;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.TemplateSetRepository;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.TemplateSetService
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly EngineCoreDBContext    _EngineCoreDBContext;
        private readonly IGeneralRepository  _IGeneralRepository;
        private readonly ISysValueRepository _ISysValueRepository;
        ValidatorException _exception;


        public  TemplateRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository, ISysValueRepository iSysValueRepository)
        {
            _EngineCoreDBContext    = EngineCoreDBContext;
            _IGeneralRepository  = iGeneralRepository;
            _ISysValueRepository = iSysValueRepository;
            _exception = new ValidatorException();
        }
           
     /*public async Task<List<TemplateAttachmentView>> getAttachments(int templateId,string lang)
        {
            var template_id = new SqlParameter("@TEMPLATE_ID", templateId);
            var _lang =   new SqlParameter("@LANG", lang);
            var result = await _EngineCoreDBContext.TemplateAttachmentView.FromSqlRaw("Exec dbo.getTemplateAttachments @TEMPLATE_ID , @LANG", template_id, _lang)
                                                          .ToListAsync();

            return result;
        }*/

       /* public async Task<List<TemplatePartyView>> getParties(int templateId, string lang)
        {
            var template_id = new SqlParameter("@TEMPLATE_ID", templateId);
            var _lang = new SqlParameter("@LANG", lang);
            var result = await _EngineCoreDBContext.TemplatePartyView.FromSqlRaw("Exec dbo.getTemplateParties @TEMPLATE_ID , @LANG", template_id, _lang)
                                                                  .ToListAsync();

            return result;
        }*/

       /* public async Task<SysyLanguage> getTemplatesWithTerms(int templateId, string lang)
        {
            
            var query = _EngineCoreDBContext.SysyLanguage.Where(x => x.Id == templateId)
                                                           //  .Include(x => x.Term)
                                                  //.Include(x => x.MasterAttachment)
                                                  // .Include(x => x.TemplateParty)

                                                  ;
            //.Include(x => x.Term);
            return await query.FirstOrDefaultAsync();
        }*/



public async Task<List<Template>> GetAll(int? documentTypeId)
         {

             Task<List<Template>> query = null;
             if (documentTypeId == null)

                 query = _EngineCoreDBContext.Template.ToListAsync();
             else
                 query = _EngineCoreDBContext.Template.Where(s => s.DocumentTypeId == documentTypeId).ToListAsync();
             return await query;

                //   catch(Exception e){
                //    return BadRequest( new { error = e.error });               }
         }

//---------------------------Templates names--------------------------------------------------
public async Task<List<TemplateView>> GetTemplateNames(int? documentTypeId,string lang)
   {
      Task<List<TemplateView>> query1_ = null;


          var  query = (
                       from tmp in _EngineCoreDBContext.Template
                       join t in _EngineCoreDBContext.SysTranslation
                            on tmp.TitleShortcut equals t.Shortcut
                       join lv in _EngineCoreDBContext.SysLookupValue
                            on tmp.DocumentTypeId equals lv.Id
                       join rd in _EngineCoreDBContext.RelatedData
                             on tmp.Id equals rd.TemplateId
                             into nt
                      from newTable in nt.DefaultIfEmpty()
                       where t.Lang == lang
                               where documentTypeId > 0 ? tmp.DocumentTypeId == documentTypeId : true

                       select new TemplateView
                      {
                         Id = tmp.Id,
                         DocumentTypeId= lv.Id,
                         Title = t.Value,
                         DocumentTypeLoukup = lv.Shortcut,                        
                         TitleShortcut = t.Shortcut,
                         CreatedDate = tmp.CreatedDate,
                         LastUpdatedDate = tmp.LastUpdatedDate,
                           ShowApplication = newTable.ShowApplication,
                           ShowTransaction = newTable.ShowTransaction,
                           Type= tmp.Type
                       });//.ToListAsync();*/

           var query2 =(
                         from trans2 in _EngineCoreDBContext.SysTranslation
                         join q1 in query
                         on trans2.Shortcut equals q1.DocumentTypeLoukup

                         where trans2.Lang == lang
                         select new TemplateView
                          {
                           Id = q1.Id,
                           DocumentTypeId = q1.DocumentTypeId,
                           DocumentType = trans2.Value,
                           Title =q1.Title,
                           TitleShortcut = q1.TitleShortcut,
                           CreatedDate = q1.CreatedDate,
                           LastUpdatedDate = q1.LastUpdatedDate,
                           ShowApplication = q1.ShowApplication,
                           ShowTransaction = q1.ShowTransaction,
                           Type = q1.Type
                         }) ;

            /*var query2 = (
                          from 
                            q1 in query 
                          join
                           trans2 in _EngineCoreDBContext.SysTranslation
                          on q1.DocumentTypeLoukup equals  trans2.Shortcut into r
                          from ed in r.DefaultIfEmpty()
                          
                          where ed.Lang == lang
                          select new TemplateView
                          {
                              Id = ed.Id,
                              DocumentType = ed.Value,
                              Title = q1.Title,
                              TitleShortcut = q1.TitleShortcut,
                              CreatedDate = q1.CreatedDate,
                              LastUpdatedDate = q1.LastUpdatedDate
                          });*/
           
            query1_ = query2.ToListAsync();
            return await query1_;
       }

//---------------------------Attachments------------------------------------------------------
 public async Task<List<AttachmentView>> GetRelatedAttachments(int templateId, string lang)
        {
            string answerYes = "", answerNo = "";
            answerYes = await _ISysValueRepository.GetValueByShortcut("Yes", lang);
            answerNo =  await _ISysValueRepository.GetValueByShortcut("No", lang);
            Task<List<AttachmentView>> query = null;
            query = (from ta in _EngineCoreDBContext.TemplateAttachment
                     join lv in _EngineCoreDBContext.SysLookupValue
                         on ta.AttachmentId equals lv.Id
                     join tr in _EngineCoreDBContext.SysTranslation
                         on lv.Shortcut equals tr.Shortcut

                     where ta.TemplateId == templateId
                     where tr.Lang == lang
                     select new AttachmentView
                     {
                         AttachmentId= ta.AttachmentId,
                         RelationId = ta.Id,
                         TranslationId=tr.Id,
                         AttachmentName = tr.Value,
                         Required= (bool)ta.Required,
                         RequiredText = ta.Required == true ? answerYes : answerNo,

                     }).ToListAsync();

            return await query;
        }

//---------------------------Parties----------------------------------------------------------
  public async Task<List<PartyView>> GetRelatedParties(int templateId, string lang)
        {
            string answerYes="", answerNo="";
            answerYes = await _ISysValueRepository.GetValueByShortcut("Yes", lang);
            
            answerNo =  await _ISysValueRepository.GetValueByShortcut("No", lang);
           
            Task<List<PartyView>> query = null;
            query = (from tp in _EngineCoreDBContext.TemplateParty
                     join lv in _EngineCoreDBContext.SysLookupValue
                         on tp.PartyId equals lv.Id
                     join tr in _EngineCoreDBContext.SysTranslation
                         on lv.Shortcut equals tr.Shortcut

                     where tp.TemplateId == templateId
                     where tr.Lang == lang
                     orderby lv.Order
                     select new PartyView
                     {
                         PartyTypeId = tp.PartyId,
                         RelationId = tp.Id,
                         TranslationId = tr.Id,
                         PartyName = tr.Value,
                         Required= (bool)tp.Required,
                         RequiredText = tp.Required==true ? answerYes: answerNo,
                         SignRequired= (bool)tp.SignRequired,
                         SignRequiredText =tp.SignRequired == true ? answerYes : answerNo
                     }).ToListAsync();

            return await query;
        }

//---------------------------Template---------------------------------------------------------
 public async Task<Template> GetOne(int id)
      {
         id = Convert.ToInt32(id);
          var query = _EngineCoreDBContext.Template.Where(x => x.Id == id)
                                                       .Include(x => x.Term)
                                                       .Include(x => x.TemplateAttachment).ThenInclude(y => y.Attachment)
                                                       .Include(x => x.TemplateParty).ThenInclude(z => z.Party);
          return await query.FirstOrDefaultAsync();

            
        }

//--------------------------------------------------------------------------------------------
 public async Task<List<int>> DeleteMany(int[] ids)
       {
            List<int> FailedDeletedList = new List<int>();
            for (int i = 0; i < ids.Length; i++)
            {
                Template template = await _EngineCoreDBContext.Template.Where(x => x.Id == ids[i]).FirstOrDefaultAsync();
                if (template != null)
                {
                    try
                    {
                        _IGeneralRepository.Delete(template);
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

//--------------------------------------------------------------------------------------------
        public async Task<int> DeleteOne(int id, string lang)
        {

            Template template = await GetOne(id);
            if (template == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "templateNotfound"));
                throw _exception;
            }
            Application appTemplate = _EngineCoreDBContext.Application.Where(x => x.TemplateId == id).FirstOrDefault();
            if (appTemplate != null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "applicationTemplate"));
                throw _exception;
            }

            TemplateParty tempParty = _EngineCoreDBContext.TemplateParty.Where(x => x.TemplateId == id).FirstOrDefault();
            if (tempParty != null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "templateProperties"));
                throw _exception;
            }

            TemplateAttachment tempattach = _EngineCoreDBContext.TemplateAttachment.Where(x => x.TemplateId == id).FirstOrDefault();
            if (tempattach != null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "templateProperties"));
                throw _exception;
            }

            Term tempterm = _EngineCoreDBContext.Term.Where(x => x.TemplateId == id).FirstOrDefault();
            if (tempterm != null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "templateProperties"));
                throw _exception;
            }
            try
            {
                        _IGeneralRepository.Delete(template);
                        if (await _IGeneralRepository.Save())
                            return Constants.OK;
                    }
                    catch (Exception) { return Constants.ERROR; }
                    return Constants.ERROR;
                }

//--------------------------------------------------------------------------------------------
 public async Task<int> Add(TemplateDto templateDto)
   {
       try
          {
                /* var shortcut = new SqlParameter("@SHORTCUT", SqlDbType.VarChar);
                 shortcut.Direction = ParameterDirection.Output;
                 shortcut.Size = 25;

                 var lang = new SqlParameter("@LANG", "ar");
                 var value = new SqlParameter("@VALUE", templateDto.Title);
                 var template_id = new SqlParameter("@DOCTYPE_ID", docTypeId);
                 await _EngineCoreDBContext.Database.ExecuteSqlRawAsync("Exec dbo.AddTemplate @TYPE , @LANG, @VALUE,@DOCTYPE_ID, @SHORTCUT out",
                                                                      type, lang, value, docType_id, shortcut);*/
                //-------------------------
                Template template = new Template
                {

                    DocumentTypeId = templateDto.DocumentTypeId,
                    TitleShortcut = templateDto.TitleShortcut,
                    Type = templateDto.Type,
                    CreatedDate = DateTime.Now,
                };

                  _IGeneralRepository.Add(template);
                  if (await _IGeneralRepository.Save())
                  {
                      return template.Id;
                  }
              }
              catch (Exception)
              {
                  return Constants.ERROR;
              }

              return Constants.ERROR;
          }

 public async Task<int> Update(int id, TemplateDto templateDto)
            {
                Template template = await GetOne(id);
                if (template == null)
                    return Constants.NOT_FOUND;
                try
                {

                    template.DocumentTypeId = templateDto.DocumentTypeId;
                    template.TitleShortcut = templateDto.TitleShortcut;
                    template.Type = templateDto.Type;
                    template.LastUpdatedDate = DateTime.Now;

                _IGeneralRepository.Update(template);
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

public async Task<TemplateView> GetTemplateName(int id, string lang)
        {
             /*Task<TemplateView> query = null;
           query = (from tmp in _EngineCoreDBContext.Template
                     join t in _EngineCoreDBContext.SysTranslation
                         on tmp.TitleShortcut equals t.Shortcut
                     where t.Lang == lang
                     where tmp.Id == id
                     select new TemplateView
                     {
                         Id = tmp.Id,
                         Title = t.Value,
                         TitleShortcut = t.Shortcut
                     }).FirstOrDefaultAsync();

            return await query;*/
            Task<TemplateView> query1_ = null;

            if (id > 0)
            {
                var query = (
                             from tmp in _EngineCoreDBContext.Template
                             join t in _EngineCoreDBContext.SysTranslation
                                  on tmp.TitleShortcut equals t.Shortcut
                             join lv in _EngineCoreDBContext.SysLookupValue
                                  on tmp.DocumentTypeId equals lv.Id
                             join rd in _EngineCoreDBContext.RelatedData
                                  on tmp.Id equals rd.TemplateId
                             into nt from newTable in nt.DefaultIfEmpty()
                             where t.Lang == lang
                             where tmp.Id == id

                             select new TemplateView
                             {
                                 Id = tmp.Id,
                                 DocumentTypeId = (int)tmp.DocumentTypeId,
                                 DocumentTypeLoukup = lv.Shortcut,
                                 Title = t.Value,
                                 TitleShortcut = t.Shortcut,
                                 CreatedDate = tmp.CreatedDate,
                                 LastUpdatedDate = tmp.LastUpdatedDate,
                                 ShowApplication = newTable.ShowApplication,
                                 ShowTransaction = newTable.ShowTransaction,
                                 Type=tmp.Type

                             });//.ToListAsync();

                var query2 = (
                            from trans2 in _EngineCoreDBContext.SysTranslation
                            join q1 in query
                            on trans2.Shortcut equals q1.DocumentTypeLoukup
                            where trans2.Lang == lang
                            select new TemplateView
                            {
                                Id = q1.Id,
                                DocumentType = trans2.Value,
                                DocumentTypeId = (int)q1.DocumentTypeId,
                                Title = q1.Title,
                                TitleShortcut = q1.TitleShortcut,
                                CreatedDate = q1.CreatedDate,
                                LastUpdatedDate = q1.LastUpdatedDate,
                                ShowApplication = q1.ShowApplication,
                                ShowTransaction = q1.ShowTransaction,
                                Type = q1.Type
                            });
                query1_ = query2.FirstOrDefaultAsync();
                return await query1_;
            }
            else return null;
        }
        public async Task<TemplateView> GetJustTemplateName(int id, string lang)
        {
          Task<TemplateView> query = null;
          query = (from tmp in _EngineCoreDBContext.Template
                    join t in _EngineCoreDBContext.SysTranslation
                        on tmp.TitleShortcut equals t.Shortcut
                    where t.Lang == lang
                    where tmp.Id == id
                    select new TemplateView
                    {
                        Id = tmp.Id,
                        Title = t.Value,
                        TitleShortcut = t.Shortcut,
                        
                    }).FirstOrDefaultAsync();

           return await query;
           
           
        }

        public async Task<List<TranslationDto>> GetTitleTranslations(string shortcut)
        {
            Task<List<TranslationDto>> query = null;
            query = (from t in _EngineCoreDBContext.SysTranslation
                     where t.Shortcut == shortcut
                     select new TranslationDto
                     {
                         Shortcut= t.Shortcut,
                         Lang = t.Lang,
                         Value = t.Value
                     }).ToListAsync();

            return await query;
        }

        public async Task<List<RelatedContentView>> GetRelatedContents(int templateId, int? stageId, string lang)
        {
            
            Task<List<RelatedContentView>> query = null;
            query = (from tm in _EngineCoreDBContext.Template
                     join rc in _EngineCoreDBContext.RelatedContent
                     on tm.Id equals rc.TemplateId
                     /*join lv in _EngineCoreDBContext.SysLookupValue
                         on rc.TitleShortcut equals lv.Shortcut*/
                     join tr in _EngineCoreDBContext.SysTranslation
                         on rc.TitleShortcut equals tr.Shortcut

                     where tm.Id == templateId
                     where rc.StageId== stageId
                     where tr.Lang == lang
                     select new RelatedContentView
                     {
                         TitleSortcut=tr.Shortcut,
                         Title = tr.Value,
                         Content = rc.Content,

                     }).ToListAsync();

            return await query;
        }

        public async Task<List<RelatedContentView>> GetRelatedContents(int templateId, string lang)
        {

            Task<List<RelatedContentView>> query = null;
            query = (from tm in _EngineCoreDBContext.Template
                     join rc in _EngineCoreDBContext.RelatedContent
                     on tm.Id equals rc.TemplateId
                     join tr in _EngineCoreDBContext.SysTranslation
                         on rc.TitleShortcut equals tr.Shortcut

                     where tm.Id == templateId
                     where tr.Lang == lang
                     select new RelatedContentView
                     {
                         TitleSortcut = tr.Shortcut,
                         Title = tr.Value,
                         Content = rc.Content,

                     }).ToListAsync();

            return await query;
        }



        public async Task<RelatedContent> GetOneRelatedContent(int id)
        {
            var query = _EngineCoreDBContext.RelatedContent.Where(x => x.Id == id);
            return await query.FirstOrDefaultAsync();


        }

        public async Task<int> AddRelatedContent(RelatedContentDto relatedContentDto)
        {
            RelatedContent relatedContent = new RelatedContent
            {
                TemplateId = relatedContentDto.TemplateId,
                TitleShortcut = relatedContentDto.TitleShortcut,
                Content = relatedContentDto.Content,
                ServiceId= relatedContentDto.ServiceId,
                StageId = relatedContentDto.StageId,
                IsOutput=true                
            };
            try
            {
                _IGeneralRepository.Add(relatedContent);
                if (await _IGeneralRepository.Save())
                {
                    return relatedContent.Id;
                }
            }
            catch (Exception)
            {
                return Constants.ERROR;
            }

              return Constants.ERROR; 
        }

        public async Task<int> Update(int id, RelatedContentDto relatedContentDto)
        {
            RelatedContent relatedContent = await GetOneRelatedContent(id);
            if (relatedContent == null)
                return Constants.NOT_FOUND;
            try
            {
                relatedContent.TemplateId = relatedContentDto.TemplateId;
                relatedContent.TitleShortcut = relatedContentDto.TitleShortcut;
                relatedContent.Content = relatedContentDto.Content;
                relatedContent.ServiceId = relatedContentDto.ServiceId;
                relatedContent.StageId = relatedContentDto.StageId;
                relatedContent.IsOutput = relatedContentDto.IsOutput;
                _IGeneralRepository.Update(relatedContent);
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
