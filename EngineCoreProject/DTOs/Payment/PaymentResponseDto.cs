using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.Payment
{
    public class PaymentResponseDto
    {
        [Required]
        public PaymentResponseDtoList[] paymentResponseDtoList { get; set; }
    }
}

public class PaymentResponseDtoList
{
    [Required]
    public string key { get; set; }
    [Required]
    public string[] value { get; set; }
}
