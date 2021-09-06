using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.TemplateSetDtos
{
    public class TableField
    {
        public int    Id { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public int    ParentId { get; set; }
        public string ParentFieldName { get; set; }
       


    }
}
