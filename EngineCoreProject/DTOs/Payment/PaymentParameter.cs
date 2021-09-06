using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Payment
{
    public class PaymentParameter
    {
        [Required]
        public string description { get; set; }
        [Required]
        public string id { get; set; }
        [Required]
        public string LanguageCode { get; set; }
        [Required]
        public string paymentType { get; set; }
        [Required]
        public string quantity { get; set; }
        [Required]
        public string returnURL { get; set; }
        [Required]
        public string serviceCode { get; set; }

        [Required]
        public string EntityID { get; set; }
        [Required]
        public string price { get; set; }//
        [Required]
        public string secureHash { get; set; }//
    }
}
