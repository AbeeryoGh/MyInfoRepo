using EngineCoreProject.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.TransactionFeeDto
{
    public class TransactionFeeInput
    {
        [Required]
        public int ServiceNo { get; set; }
        [Required]
        public Constants.DOCUMENT_KIND DocumentKind { get; set; }
        [Required]
        public Constants.PROCESS_KIND ProcessKind { get; set; }
        public int PartiesCount { get; set; }  // parties count.
        public int PageCount { get; set; }  // Pages count.
        public double Amount { get; set; }  // contract value.

    }
}
