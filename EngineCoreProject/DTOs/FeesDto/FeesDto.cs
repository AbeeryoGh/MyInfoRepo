using System.Collections.Generic;

namespace EngineCoreProject.DTOs.FeesDto
{
    public class PaymentDetailsDto
    {
        public int ApplicationId { set; get; }
        public int UserId { set; get; }
        public int ActionId { set; get; }
        public  List<FeesDto> FeeList { set; get; }
    }

    public class FeesDto
    {      
        public string ServiceMainCode { set; get; }
        public string ServiceSubCode { set; get; }
        public int Quantity { set; get; }
        public double? Price { set; get; }
    }

    public class GetFees
    {
        public int Id { set; get; }
        public string ServiceMainCode { set; get; }
        public string ServiceSubCode { set; get; }
        public int Quantity { set; get; }
        public double? Price { set; get; }
        public double? AmountWithFees { set; get; }
        public double? AmountWithoutFees { set; get; }
        public double? OwnerFees { set; get; }
        public string FeeName { set; get; }
    }
}
