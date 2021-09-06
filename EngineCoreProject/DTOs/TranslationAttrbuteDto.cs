using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs
{
    public class TranslationAttributeDto
    {

        //[Required]
        //public bool FoundShortCut { get; set; }
       
        public string tableName { get; set; }
       
        public string columnName { get; set; }
       

        public string shortcut { get; set; }
        [Required]

        public string value { get; set; }
        [Required]

        public string currentLanguage { get; set; }



    }
}
