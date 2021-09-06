using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Payment
{
    public class AttributeToGenerateUrlDto
    {//string _languageCode, string _price, string _quantity
      
        public string LanguageCode { get; set; }
        [Required]
      
        public string quantity { get; set; }
        [Required]
        
        
        
        public string price { get; set; }//
        
        
    }
}
