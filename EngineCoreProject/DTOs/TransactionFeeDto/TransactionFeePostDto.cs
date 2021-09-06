using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.TransactionFeeDto
{
    public class TransactionFeePostDto
    {
        public Dictionary<string, string> NameShortcut { get; set; }
        [Required]
        public float Value { get; set; }
        [Required]
        public string SubClass { get; set; }
        [Required]
        public string PrimeClass { get; set; }
        [Required]
        public bool Multiplied { get; set; }
        [Required]
        public bool Percentage { get; set; }
        [Required]
        public bool PerPage { get; set; }
        [Required]
        public int MaxLimitedTax { get; set; }
        [Required]
        public bool LimitedValue { get; set; }
        public int? MoreThan { get; set; }
        public int? LessThan { get; set; }

        public string EntityCodeEpos { get; set; }
        public string EntityGlCodeEpos { get; set; }
        public string ServiceCodeEpos { get; set; }
        public string ServiceGlCodeEpos { get; set; }
        public string MappingFmisCodeEpos { get; set; }

        public string Note { get; set; }

        public TransactionFeePostDto()
        {

        }

        public TransactionFee GetEntity()
        {
            TransactionFee transactionFees = new TransactionFee
            {
                Value = Value,
                MaxLimitedTax = MaxLimitedTax,
                LimitedValue = LimitedValue,
                Multiplied = Multiplied,
                Percentage = Percentage,
                PerPage = PerPage,
                EntityCodeEpos = EntityCodeEpos,
                EntityGlCodeEpos = EntityGlCodeEpos,
                ServiceCodeEpos = ServiceCodeEpos,
                ServiceGlCodeEpos = ServiceGlCodeEpos,
                MappingFmisCodeEpos = MappingFmisCodeEpos,             
                PrimeClass = PrimeClass,
                SubClass = SubClass,
                LessThan = LessThan,
                MoreThan = MoreThan,
                Notes = Note
            };

            return transactionFees;
        }

    }
}
