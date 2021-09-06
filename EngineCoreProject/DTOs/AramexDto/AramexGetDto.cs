using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AramexDto
{
    public class AramexGetDto
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? StateId { get; set; }
        public string OwnerName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string TemplateName { get; set; }
        public int TemplateId { get; set; }
        public string StateName { get; set; }
    }
}
