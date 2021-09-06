using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.TransactionFeeDto
{
    public class ServiceFeePostDto
    {
        [Required]
        public int ServiceNo { get; set; }
        [Required]
        public int FeeNo { get; set; }
        [Required]
        public bool Required { get; set; }

        public ServiceFeePostDto()
        {

        }

        public ServiceFee GetEntity()
        {
            ServiceFee serviceFees = new ServiceFee
            {
                FeeNo = FeeNo,
                ServiceNo = ServiceNo,
                Required =Required
                
            };

            return serviceFees;
        }

    }
}
