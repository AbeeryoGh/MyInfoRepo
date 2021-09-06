using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using EngineCoreProject.DTOs.ApplicationDtos.RelatedContent;
using EngineCoreProject.DTOs.SysLookUpDtos;
using EngineCoreProject.DTOs.TemplateSetDtos;
using EngineCoreProject.DTOs.TemplateSetDtos.ModelView;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.TemplateSetRepository
{
    public interface ITemplateRepository
    {

         Task<List<Template>>       GetAll(int? documentTypeId);
         Task<List<TemplateView>>   GetTemplateNames(int? documentTypeId, string lang);
         Task<TemplateView>         GetTemplateName(int Id, string lang);
         Task<TemplateView>         GetJustTemplateName(int Id, string lang);
         Task<List<AttachmentView>> GetRelatedAttachments(int templateId, string lang);
         Task<List<RelatedContentView>> GetRelatedContents(int templateId,int? stageId, string lang);
         Task<List<RelatedContentView>> GetRelatedContents(int templateId, string lang);

         Task<List<PartyView>> GetRelatedParties(int templateId, string lang);
         Task<List<TranslationDto>> GetTitleTranslations(string shortcut);
         Task<Template>  GetOne(int id);
         Task<List<int>> DeleteMany(int[] ids);
         Task<int>       DeleteOne(int id,string lang);
         Task<int>       Add(TemplateDto templateDto);
         Task<int>       Update(int id, TemplateDto templateDto);
        Task<int> AddRelatedContent(RelatedContentDto relatedContentDto);

       


        /* Task<List<TemplateAttachmentView>>  getAttachments(int templateId,string lang);
         Task<List<TemplatePartyView>>       getParties(int templateId, string lang);

        Task<int> addParty(TemplatePartyDto templatePartyDto);
        Task<int> addAttachment(TemplateAttachmentDto templateAttachmentDto);*/

        // Task<SysyLanguage> getTemplatesWithTerms(int templateId, string lang);


    }
}
