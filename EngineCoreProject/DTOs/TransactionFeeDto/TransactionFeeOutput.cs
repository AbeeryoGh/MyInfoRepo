using EngineCoreProject.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.TransactionFeeDto
{
    public class TransactionFeeOutput
    {
        public int FeeNo { get; set; }
        public string FeeName { get; set; }
        public int Quantity { get; set; }
        public double FeeValue { get; set; }
        public string SubCalss { get; set; }
        public string PrimeClass { get; set; }
    }
}
