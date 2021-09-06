using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Payment
{
    public class GetPaymentStatusDto
    {
        [Required]
        public string PurchaseId { set; get; }
        
    }
}
