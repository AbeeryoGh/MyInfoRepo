using EngineCoreProject.DTOs.NotificationDtos;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AdmService
{
    public class ActionDetialsForNotification
    {

        //ActionId=adm_stage_action.ActionId,
        //ActionShortcut=notification_action.Action.Shortcut,
        //ActionTranslate = _iGeneralRepository.getTranslateByShortCut(lang, notification_action.Action.Shortcut),
        //action_type_id = (int?) notification_action.Action.ActionTypeId,
        //NotificationDetialsId= notification_action.NotificationDetialsId,
        //NotificationTempleteShortCut= notification_action.NotificationDetials.NotificationName,
        //NotificationTempleteTranslation = _iGeneralRepository.getTranslateByShortCut(lang, notification_action.NotificationDetials.NotificationName),
        //action_type = _iGeneralRepository.getTranslateByIdFromLookUpValueId(lang, (int)notification_action.Action.ActionTypeId)

        public int ActionId { get; set; }
        public int RecId { get; set; }
        public int StageId { get; set; }
        public string ActionShortcut { get; set; }
        public string ActionTranslate { get; set; }
        public int action_type_id { get; set; }
        public List<ClassNotification>  NotificationsForAction { get; set; }//roleId
        public string action_type_translation { get; set; }//
        public List<ClassRole> Roles { get; set; }//roleId

    }
}


public class ClassRole
{
    public int? id { get; set; }
    public string RoleShortCut { get; set; }//AdmStageActionRoleId
    public int? AdmStageActionRoleId { get; set; }
    public string RoleTranslate { get; set; }//



}

public class ClassNotification
{
    public int? idNotificationAction { get; set; }
    public string NotificationTempleteShortCut { get; set; }
    public string NotificationTempleteTranslation { get; set; }
  



}
