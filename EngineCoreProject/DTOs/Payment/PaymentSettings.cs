using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Payment
{
    public class PaymentSettings
    {
        [Required]
        public string URL { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public string paymentType { get; set; }
        [Required]
        public string returnURL { get; set; }
        [Required]
        public string EntityID { get; set; }
        [Required]
        public string serviceId { get; set; }
        [Required]
        public string userId { get; set; }


        [Required]
        public string serviceCode { get; set; }
        [Required]
        public string secretKey { get; set; }  //

        [Required]
        public string PreInvoiceNo { get; set; }
        public string redirectToURL { get; set; }  //redirectToURL


    }
}
