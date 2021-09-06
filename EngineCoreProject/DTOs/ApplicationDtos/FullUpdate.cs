using EngineCoreProject.DTOs.NotificationDtos;
using System.Collections.Generic;


namespace EngineCoreProject.DTOs.ApplicationDtos
{
    public class FullUpdate
    {
        public ApplicationSaveInfo    toSave   { get; set; }
        public ApplicationInfoUpdate  toUpdate { get; set; }
        public AttachmentPartyIdsList toDelete { get; set; }
        public List<NotificationLogPostDto> notification{ get; set; }

        

    }
}
