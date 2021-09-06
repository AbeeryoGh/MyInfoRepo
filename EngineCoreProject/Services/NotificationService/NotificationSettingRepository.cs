using EngineCoreProject.Services;
using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.NotificationDtos;
using EngineCoreProject.IRepository.INotificationSettingRepository;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EngineCoreProject.DTOs.ActionDtos;
using EngineCoreProject.DTOs.AdmService;
using System.IO;

namespace EngineCoreProject.Services.NotificationService
{
    public class NotificationSettingRepository : INotificationSettingRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;

        public NotificationSettingRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
        }

        public async Task<int> AddNotificationTemplateWithDetails(NotificationTemplateWithDetailsPostDto notificationTemplatePostDto, string lang)
        {
            // TODO validation for template and template details, channels id's, check if passed en, Arabic names are distinct.
            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();
            NotificationTemplate notifyTemp = new NotificationTemplate
            {
                NotificationNameShortcut = _iGeneralRepository.GenerateShortCut(Constants.NOTIFICATION_TEMPLATE, Constants.NOTIFICATION_TEMPLATE_NAME_SHORTCUT)
            };

            _EngineCoreDBContext.NotificationTemplate.Add(notifyTemp);
            await _EngineCoreDBContext.SaveChangesAsync();
            await _iGeneralRepository.InsertUpdateSingleTranslation(notifyTemp.NotificationNameShortcut, notificationTemplatePostDto.NotificationTemplateShortCutLangValue);

            // add template details.
            foreach (var notificationTemplateDetailDto in notificationTemplatePostDto.NotificationTemplatedDetails)
            {
                NotificationTemplateDetail notificationTemplateDetail = new NotificationTemplateDetail
                {
                    NotificationTemplateId = notifyTemp.Id,
                    NotificationChannelId = notificationTemplateDetailDto.NotificationChannelId,
                    ChangeAble = notificationTemplateDetailDto.ChangeAble,
                    TitleShortcut = _iGeneralRepository.GenerateShortCut(Constants.NOTIFICATION_TEMPLATE_DETAIL, Constants.NOTIFICATION_TEMPLATE_DETAIL_TITLE_SHORTCUT),
                    BodyShortcut = _iGeneralRepository.GenerateShortCut(Constants.NOTIFICATION_TEMPLATE_DETAIL, Constants.NOTIFICATION_TEMPLATE_DETAIL_BODY_SHORTCUT)
                };

                _EngineCoreDBContext.NotificationTemplateDetail.Add(notificationTemplateDetail);
                await _EngineCoreDBContext.SaveChangesAsync();
                await _iGeneralRepository.InsertUpdateSingleTranslation(notificationTemplateDetail.TitleShortcut, notificationTemplateDetailDto.TitleShortCutLangValue);
                await _iGeneralRepository.InsertUpdateSingleTranslation(notificationTemplateDetail.BodyShortcut, notificationTemplateDetailDto.BodyShortCutLangValue);
            }

            await transaction.CommitAsync();
            return notifyTemp.Id;
        }

        public async Task<List<int>> AddNotificationAction(NotificationActionPostDto notificationActionPostDto, int notificationTemplateId)
        {
            // TODO validate for adding valid details for the notification id and NotificationActionPostDto.
            List<int> res = new List<int>();
            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();

            foreach (var actionId in notificationActionPostDto.ActionListId)
            {
                NotificationAction notificationAction = new NotificationAction
                {
                    ActionId = actionId,
                    NotificationTemplateId = notificationTemplateId
                };

                _EngineCoreDBContext.NotificationAction.Add(notificationAction);
                await _EngineCoreDBContext.SaveChangesAsync();
                res.Add(notificationAction.Id);
            }
            await transaction.CommitAsync();
            return res;
        }

        public async Task<int> AddNotificationTemplatesToOneAction(NotificationTemplatesActionPostDto notificationTemplatesAction)
        {
            int res = 0;
            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();

            List<NotificationAction> newNotifyAction = new List<NotificationAction>();

            var oldNotificationsAction = await _EngineCoreDBContext.NotificationAction.Where(x => x.ActionId == notificationTemplatesAction.ActionId).ToListAsync();
            if (oldNotificationsAction != null || oldNotificationsAction.Count > 0)
            {
                _EngineCoreDBContext.NotificationAction.RemoveRange(oldNotificationsAction);
                await _EngineCoreDBContext.SaveChangesAsync();
            }

            foreach (var notificationTemplateId in notificationTemplatesAction.NotificationTemplateIds)
            {
                if (_EngineCoreDBContext.NotificationTemplate.Any(x => x.Id == notificationTemplateId) && !_EngineCoreDBContext.NotificationAction.Any(x => x.ActionId == notificationTemplatesAction.ActionId && x.NotificationTemplateId == notificationTemplateId) && _EngineCoreDBContext.AdmAction.Any(x => x.Id == notificationTemplatesAction.ActionId))
                {
                    NotificationAction notifyAct = new NotificationAction
                    {
                        ActionId = notificationTemplatesAction.ActionId,
                        NotificationTemplateId = notificationTemplateId
                    };
                    newNotifyAction.Add(notifyAct);
                }

            }

            _EngineCoreDBContext.NotificationAction.AddRange(newNotifyAction);
            res = await _EngineCoreDBContext.SaveChangesAsync();
            await transaction.CommitAsync();
            return res;
        }

        public async Task<bool> DeleteNotificationTemplate(int notificationTemplateId)
        {
            NotificationTemplate notificationTemplate = await _EngineCoreDBContext.NotificationTemplate.Where(x => x.Id == notificationTemplateId).FirstOrDefaultAsync();
            if (notificationTemplate == null)
            {
                return false;
            }

            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();

            List<NotificationAction> notificationActions = await _EngineCoreDBContext.NotificationAction.Where(x => x.NotificationTemplateId == notificationTemplateId).ToListAsync();
            if (notificationActions.Count > 0)
            {
                _EngineCoreDBContext.NotificationAction.RemoveRange(notificationActions);
                _EngineCoreDBContext.SaveChanges();
            }

            List<NotificationTemplateDetail> notificationTemplateDetails = await _EngineCoreDBContext.NotificationTemplateDetail.Where(x => x.NotificationTemplateId == notificationTemplateId).ToListAsync();
            if (notificationTemplateDetails.Count > 0)
            {
                foreach (var tran in notificationTemplateDetails)
                {
                    await _iGeneralRepository.DeleteTranslation(tran.TitleShortcut);
                    await _iGeneralRepository.DeleteTranslation(tran.BodyShortcut);
                }

                _EngineCoreDBContext.NotificationTemplateDetail.RemoveRange(notificationTemplateDetails);
                _EngineCoreDBContext.SaveChanges();
            }

            await _iGeneralRepository.DeleteTranslation(notificationTemplate.NotificationNameShortcut);
            _EngineCoreDBContext.NotificationTemplate.Remove(notificationTemplate);
            _EngineCoreDBContext.SaveChanges();

            await transaction.CommitAsync();
            return true;
        }

        public async Task<NotificationTemplateWithDetailsGetDto> GetAllNotificationDetails(int notifyTemplateId)
        {
            NotificationTemplateWithDetailsGetDto res = new NotificationTemplateWithDetailsGetDto();
            NotificationTemplate NotificationTemplate = await _EngineCoreDBContext.NotificationTemplate.Where(x => x.Id == notifyTemplateId).FirstOrDefaultAsync();
            if (NotificationTemplate == null)
            {
                return res;
            }

            res.NotificationTemplateShortCutLangValue = await _iGeneralRepository.getTranslationsForShortCut(NotificationTemplate.NotificationNameShortcut);
            res.NotificationTemplateId = notifyTemplateId;

            List<NotificationTemplateDetail> notifyDetails = await _EngineCoreDBContext.NotificationTemplateDetail.Where(x => x.NotificationTemplateId == notifyTemplateId).ToListAsync();

            foreach (var notifyDetail in notifyDetails)
            {
                NotificationTemplateDetailGetDto notifyDetailDto = new NotificationTemplateDetailGetDto
                {
                    BodyShortCutLangValue = await _iGeneralRepository.getTranslationsForShortCut(notifyDetail.BodyShortcut),
                    TitleShortCutLangValue = await _iGeneralRepository.getTranslationsForShortCut(notifyDetail.TitleShortcut),
                    ChangeAble = notifyDetail.ChangeAble,
                    NotificationChannelId = notifyDetail.NotificationChannelId
                };

                var channelShortCut = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Id == notifyDetail.NotificationChannelId).Select(x => x.Shortcut).FirstOrDefaultAsync();
                if (channelShortCut != null)
                {
                    notifyDetailDto.ChannelShortCutLangValue = await _iGeneralRepository.getTranslationsForShortCut(channelShortCut);
                }
                res.NotificationTemplateDetails.Add(notifyDetailDto);
            }
            return res;
        }

        public async Task<List<NotificationTemplateGetDto>> GetAllNotificationTemplates()
        {
            List<NotificationTemplateGetDto> res = new List<NotificationTemplateGetDto>();
            var NotificationTemplates = await _EngineCoreDBContext.NotificationTemplate.ToListAsync();
            if (NotificationTemplates == null)
            {
                return res;
            }

            foreach (var notifyTemp in NotificationTemplates)
            {
                NotificationTemplateGetDto notifyTempDto = new NotificationTemplateGetDto
                {
                    NotificationTemplateId = notifyTemp.Id,
                    NotificationTemplateShortCutLangValue = await _iGeneralRepository.getTranslationsForShortCut(notifyTemp.NotificationNameShortcut)
                };
                res.Add(notifyTempDto);
            }
            return res;
        }

        public async Task<int> EditNotificationTemplateDetials(NotificationTemplateWithDetailsPostDto notificationTemplateDetails, int templateId)
        {
            NotificationTemplate notifyTemplate = await _EngineCoreDBContext.NotificationTemplate.Where(x => x.Id == templateId).FirstOrDefaultAsync();
            if (notifyTemplate == null)
            {
                return 0;
            }

            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();
            await _iGeneralRepository.InsertUpdateSingleTranslation(notifyTemplate.NotificationNameShortcut, notificationTemplateDetails.NotificationTemplateShortCutLangValue);

            // replace details.
            var oldDetails = await _EngineCoreDBContext.NotificationTemplateDetail.Where(x => x.NotificationTemplateId == templateId).ToListAsync();
            if (oldDetails != null || oldDetails.Count > 0)
            {
                _EngineCoreDBContext.NotificationTemplateDetail.RemoveRange(oldDetails);
                foreach (var tran in oldDetails)
                {
                    await _iGeneralRepository.DeleteTranslation(tran.TitleShortcut);
                    await _iGeneralRepository.DeleteTranslation(tran.BodyShortcut);
                }
            }
            foreach (var notificationTemplateDetailDto in notificationTemplateDetails.NotificationTemplatedDetails)
            {
                NotificationTemplateDetail notificationTemplateDetail = new NotificationTemplateDetail
                {
                    NotificationTemplateId = templateId,
                    NotificationChannelId = notificationTemplateDetailDto.NotificationChannelId,
                    ChangeAble = notificationTemplateDetailDto.ChangeAble,
                    TitleShortcut = _iGeneralRepository.GenerateShortCut(Constants.NOTIFICATION_TEMPLATE_DETAIL, Constants.NOTIFICATION_TEMPLATE_DETAIL_TITLE_SHORTCUT),
                    BodyShortcut = _iGeneralRepository.GenerateShortCut(Constants.NOTIFICATION_TEMPLATE_DETAIL, Constants.NOTIFICATION_TEMPLATE_DETAIL_BODY_SHORTCUT)
                };

                _EngineCoreDBContext.NotificationTemplateDetail.Add(notificationTemplateDetail);
                await _EngineCoreDBContext.SaveChangesAsync();

                await _iGeneralRepository.InsertUpdateSingleTranslation(notificationTemplateDetail.TitleShortcut, notificationTemplateDetailDto.TitleShortCutLangValue);
                await _iGeneralRepository.InsertUpdateSingleTranslation(notificationTemplateDetail.BodyShortcut, notificationTemplateDetailDto.BodyShortCutLangValue);
            }

            await transaction.CommitAsync();
            return templateId;
        }

        public Dictionary<string, string> GetParameterList()
        {
            return Constants.ParameterDic;
        }

        public async Task<List<NotificationLogPostDto>> GetNotificationsForAction(int actionId)
        {
            List<NotificationLogPostDto> res = new List<NotificationLogPostDto>();
            List<NotificationTemplateDetailsForOneAction> notificationTemplates = await GetNotificationsDetailsForAction(actionId);
            foreach (var notificationTemplate in notificationTemplates)
            {
                List<NotificationTemplateWithDetailsGetDto> notificationTemplateDetails = notificationTemplate.NotificationTemplateDetails;
                foreach (var notificationTemplateDetail in notificationTemplateDetails)
                {
                    List<NotificationTemplateDetailGetDto> notificationDetails = notificationTemplateDetail.NotificationTemplateDetails;
                    foreach (var notify in notificationDetails)
                    {
                        foreach (KeyValuePair<string, string> entry in notify.TitleShortCutLangValue)
                        {
                            NotificationLogPostDto notification = new NotificationLogPostDto
                            {
                                Lang = entry.Key,
                                NotificationTitle = notify.TitleShortCutLangValue[entry.Key],
                                NotificationBody  = notify.BodyShortCutLangValue[entry.Key],
                                NotificationChannelId = notify.NotificationChannelId
                            };
                            res.Add(notification);
                        }
                    }
                }
            }
            return res;
        }

        private async Task<List<NotificationTemplateDetailsForOneAction>> GetNotificationsDetailsForAction(int actionId)
        {
            List<NotificationTemplateDetailsForOneAction> res = await _EngineCoreDBContext.NotificationAction.
                  Include(x => x.NotificationTemplate).
                  ThenInclude(x => x.NotificationTemplateDetail).
                  Where(x => x.ActionId == actionId).
                  Select(x => new NotificationTemplateDetailsForOneAction
                  {
                      ActionId = x.ActionId,
                      NotificationTemplateDetails = new List<NotificationTemplateWithDetailsGetDto>
                      { new NotificationTemplateWithDetailsGetDto
                           {
                              NotificationTemplateId = x.NotificationTemplateId, NotificationTemplateShortCutLangValue =  _iGeneralRepository.getTranslationsForShortCut(x.NotificationTemplate.NotificationNameShortcut).Result,
                              NotificationTemplateDetails =
                                (from y in x.NotificationTemplate.NotificationTemplateDetail select
                                 new NotificationTemplateDetailGetDto {  NotificationChannelId = y.NotificationChannelId ,
                                                                         BodyShortCutLangValue = _iGeneralRepository.getTranslationsForShortCut(y.BodyShortcut).Result,
                                                                         TitleShortCutLangValue = _iGeneralRepository.getTranslationsForShortCut(y.TitleShortcut).Result,
                                                                         ChangeAble = y.ChangeAble,
                                                                         ChannelShortCutLangValue =  _iGeneralRepository.getTranslationsForShortCut(_EngineCoreDBContext.SysLookupValue.Where(x => x.Id == y.NotificationChannelId).Select(z => z.Shortcut).FirstOrDefault()).Result
                      }).ToList()

                  }
                  }
                  }
                  ).ToListAsync();
            return res;

        }




        public async Task GetAllNotificationDetailsLans(string lang)
        {
            string body = "body: ";
            string title = "address: ";

            if (lang == "ar")
            {
                body = " :النص ";
                title = " :العنوان ";
            }

            var NotificationTemplates = await _EngineCoreDBContext.NotificationTemplate.ToListAsync();

            foreach (var template in NotificationTemplates)
            {

                var tempName = await _iGeneralRepository.GetTranslateByShortCut(lang, template.NotificationNameShortcut);
                File.AppendAllText("Result.txt", template.Id + "   " + tempName + Environment.NewLine);
                List<NotificationTemplateDetail> notifyDetails = await _EngineCoreDBContext.NotificationTemplateDetail.Where(x => x.NotificationTemplateId == template.Id).ToListAsync();
                foreach (var notifyDetail in notifyDetails)
                {
                    var notTitle = await _iGeneralRepository.GetTranslateByShortCut(lang, notifyDetail.TitleShortcut);
                    var notBody = await _iGeneralRepository.GetTranslateByShortCut(lang, notifyDetail.BodyShortcut);

                    var channelShortCut = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Id == notifyDetail.NotificationChannelId).Select(x => x.Shortcut).FirstOrDefaultAsync();
                    var chan = await _iGeneralRepository.GetTranslateByShortCut(lang, channelShortCut);

                    File.AppendAllText("Result.txt", "   " + chan + " :  " + title + notTitle + " , " + body + notBody + Environment.NewLine);
                }

                File.AppendAllText("Result.txt", "  --------------------------------- -------------------  -----------------------------   " + Environment.NewLine);

            }

  
        }
    }
}

