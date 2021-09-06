using EngineCoreProject.DTOs.AdmService;
using EngineCoreProject.DTOs.AdmService.ModelView;
using EngineCoreProject.DTOs.NotificationDtos;
using EngineCoreProject.DTOs.RoleDto;
using EngineCoreProject.IRepository.AdmServices;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.Admservice
{
    public class StageActionRepository : IStageActionsRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;

        public StageActionRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
        }

        public async Task<AdmStageAction> GetStageById(int id)
        {
            var query = await _EngineCoreDBContext.AdmStageAction.Where(x => x.Id == id).FirstOrDefaultAsync();
            return query;
        }

        public async Task<List<StageActionDto>> GetStageAction(int id, string lang)
        {
            Task<List<StageActionDto>> query = null;
            query = (from sa in _EngineCoreDBContext.AdmStageAction
                     join act in _EngineCoreDBContext.AdmAction on new { action_id = (int)sa.ActionId } equals new { action_id = act.Id }
                     join ta in _EngineCoreDBContext.SysTranslation on act.Shortcut equals ta.Shortcut
                     join lv in _EngineCoreDBContext.SysLookupValue on new { action_type_id = (int)act.ActionTypeId } equals new { action_type_id = lv.Id }
                     join ta1 in _EngineCoreDBContext.SysTranslation on lv.Shortcut equals ta1.Shortcut
                     where sa.StageId == id
                     where ta.Lang == lang
                     where ta1.Lang == lang
                     select new StageActionDto
                     {
                         recId = sa.Id,
                         stageId = sa.StageId,
                         actionId = (int?)sa.ActionId,
                         ActionName = act.Shortcut,
                         actiontypeid = (int?)act.ActionTypeId,
                         lang = ta.Lang,
                         value = ta.Value,
                         actiontype = lv.Shortcut,
                         typename = ta1.Value,

                     }).ToListAsync();
            return await query;
        }

        public async Task<int> Delete(int id)
        {
            AdmStageAction stageAction = await GetStageById(id);
            if (stageAction == null)
                return Constants.NOT_FOUND;
            try
            {
                _iGeneralRepository.Delete(stageAction);
                if (await _iGeneralRepository.Save())
                    return Constants.OK;
            }
            catch (Exception) { return Constants.ERROR; }
            return Constants.ERROR;
        }

        public async Task<List<ActionDetialsForNotification>> GetActionDetialsForNotification(int id, string lang)
        {
            var adm_stage_action_with_action = await (from adm_stage_action in _EngineCoreDBContext.AdmStageAction
                                                from adm_action in _EngineCoreDBContext.AdmAction.Where(x => x.Id == adm_stage_action.ActionId)
                                                select new ActionDetialsForNotification
                                                {
                                                    ActionId = adm_action.Id,
                                                    ActionShortcut = adm_action.Shortcut,
                                                    action_type_id = (int)adm_action.ActionTypeId,
                                                    RecId = adm_stage_action.Id,
                                                    StageId = (int)adm_stage_action.StageId,
                                                    ActionTranslate =  _EngineCoreDBContext.SysTranslation.Where(x=>x.Shortcut== adm_action.Shortcut && x.Lang==lang).Select(y=>y.Value).FirstOrDefault(), // _iGeneralRepository.GetTranslateByShortCut(lang, adm_action.Shortcut).Result,
                                                    action_type_translation = _iGeneralRepository.getTranslateByIdFromLookUpValueId(lang, (int)adm_action.ActionTypeId),
                                                    NotificationsForAction = (from notification_action in _EngineCoreDBContext.NotificationAction.Where(y => y.ActionId == adm_action.Id)
                                                                              from notification_templete in _EngineCoreDBContext.NotificationTemplate.Where(z => z.Id == notification_action.NotificationTemplateId)
                                                                              select new ClassNotification
                                                                              {
                                                                                  idNotificationAction = notification_action.Id,
                                                                                  NotificationTempleteShortCut = notification_templete.NotificationNameShortcut,
                                                                                  NotificationTempleteTranslation = _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang && x.Shortcut == notification_templete.NotificationNameShortcut).Select(y => y.Value).FirstOrDefault()
                                                                              }
                                                    ).ToList() 
                                                    ,
                                                    Roles = (from adm_stage_action_role in _EngineCoreDBContext.RoleClaim.Where(y => y.ClaimType == CustomClaimTypes.Action && y.ClaimValue == adm_action.Id.ToString())
                                                             from role in _EngineCoreDBContext.Role.Where(m => m.Id == adm_stage_action_role.RoleId)
                                                             select new ClassRole
                                                             {
                                                                 id = role.Id,
                                                                 AdmStageActionRoleId = adm_stage_action_role.Id,                 
                                                                 RoleTranslate = _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang && x.Shortcut == role.Name).Select(y => y.Value).FirstOrDefault()
                                                             }
                                                    )
                                                   .ToList()
                                                }).ToListAsync();


            return adm_stage_action_with_action.Where(x => x.StageId == id).ToList();
        }

        public async Task<AdmStageAction> Add(postActionDto postAction)
        {
            AdmStageAction admStageAction = new AdmStageAction()
            {
                StageId = postAction.stageid,
                ActionId = postAction.actionid,
                CreatedDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now

            };
            _iGeneralRepository.Add(admStageAction);
            if (await _iGeneralRepository.Save())
            {
                return admStageAction;
            }
            else return admStageAction;
        }

        public async Task<dynamic> GetActionstoRole(List<int> actionIds, string lang)
        {
            List<ActionForRole> query = null;
            query = await (from ac in _EngineCoreDBContext.AdmAction
                           join
                           sa in _EngineCoreDBContext.AdmStageAction
                           on ac.Id equals sa.ActionId
                           join t in _EngineCoreDBContext.SysTranslation
                           on ac.Shortcut equals t.Shortcut
                           join s in _EngineCoreDBContext.AdmStage on
                            sa.StageId equals s.Id
                           join sr in _EngineCoreDBContext.AdmService
                           on s.ServiceId equals sr.Id
                           join t2 in _EngineCoreDBContext.SysTranslation
                           on sr.Shortcut equals t2.Shortcut
                           join t3 in _EngineCoreDBContext.SysTranslation
                            on s.Shortcut equals t3.Shortcut

                           where t.Lang  == lang
                           where t2.Lang == lang
                           where t3.Lang == lang

                           orderby sa.ShowOrder
                           select new ActionForRole
                           {
                               StageId = sa.Id,
                               ActionId = (int)sa.ActionId,
                               ActionName = t.Value,
                               ServiceName = t2.Value,
                               StageName = t3.Value,
                               HasAccess = actionIds.Contains((int)sa.ActionId)
                           }).ToListAsync();

            var grouped = query.GroupBy(d => new { d.ServiceName })

               .Select(g => new 
               {
                   Services = g.Key.ServiceName,
                   Stages = g.Select(a => new
                   {
                       ActionId = a.ActionId,
                       ActionName = a.ActionName,
                       StageName = a.StageName,
                       HasAccess = a.HasAccess

                   }).ToList().GroupBy(x => new { x.StageName })
                   .Select(x => new {
                       StageName = x.Key.StageName,
                       Actions = x.Select(y => new {
                           ActionID = y.ActionId,
                           ActionName = y.ActionName,
                           HasAccess = y.HasAccess
                       }).ToList()
                   })
               }).ToList();

            return grouped;
        }
    }
}
