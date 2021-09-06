using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.TransactionFeeDto
{
    public class TransactionFeeGetDto
    {
        public int Id { get; set; }
        public Dictionary<string, string> FeeName { get; set; }
        public double Value { get; set; }
        public string SubClass { get; set; }
        public string PrimeClass { get; set; }
        public int? LessThan { get; set; }
        public int? MoreThan { get; set; }
        public bool Multiplied { get; set; }
        public bool Percentage { get; set; }
        public bool? PerPage { get; set; }
        public bool LimitedValue { get; set; }
        public int? MaxLimitedTax { get; set; }
        public string Note { get; set; }

        public TransactionFeeGetDto()
        {

        }

        public static TransactionFeeGetDto GetDTO(TransactionFee transactionFee)
        {
            TransactionFeeGetDto dto = new TransactionFeeGetDto
            {
                Id = transactionFee.Id,
                Value = transactionFee.Value,
                LimitedValue = transactionFee.LimitedValue,
                MaxLimitedTax = transactionFee.MaxLimitedTax,
                Multiplied = transactionFee.Multiplied,
                PrimeClass = transactionFee.PrimeClass,
                SubClass = transactionFee.SubClass,
                LessThan = transactionFee.LessThan,
                MoreThan = transactionFee.MoreThan,
                Percentage = transactionFee.Percentage,
                PerPage = transactionFee.PerPage,
                Note = transactionFee.Notes
            };

            return dto;
        }
    }
}
