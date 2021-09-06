using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.TransactionFeeDto
{
    public class ServiceFeeGetDto
    {
        public int Id { get; set; }
        public int ServiceNo { get; set; }
        public Dictionary<string, string> ServiceName { get; set; }
        public int FeeNo { get; set; }
        public Dictionary<string, string> FeeName { get; set; }
        public int DocumentKind { get; set; }
        public int ProcessKind { get; set; }
        public bool? Required { get; set; }

        public static ServiceFeeGetDto GetDTO(ServiceFee serviceFee)
        {
            ServiceFeeGetDto res = new ServiceFeeGetDto
            {
                Id = serviceFee.Id,
                ServiceNo = serviceFee.ServiceNo,
                FeeNo = serviceFee.FeeNo,
                Required = serviceFee.Required,
                DocumentKind = serviceFee.DocumentKind,
                ProcessKind = serviceFee.ProcessKind
            };

            return res;
        }
    }
}
