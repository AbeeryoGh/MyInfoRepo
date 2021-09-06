using EngineCoreProject.DTOs.TransactionFeeDto;
using EngineCoreProject.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.ITransactionFeeRepository
{
    public interface IServiceFeeRepository
    {
        Task<int> AddServiceFee(ServiceFeePostDto serviceFeePostDto, Constants.DOCUMENT_KIND docKind, Constants.PROCESS_KIND processKind, string lang);
        Task<int> UpdateServiceFee(ServiceFeePostDto serviceFeePostDto, int rowId, Constants.DOCUMENT_KIND docKind, Constants.PROCESS_KIND processKind, string lang);
        Task<int> DeleteServiceFee(int id);
        Task<List<ServiceFeeGetDto>> GetServiceFeesByServiceId(int serviceId);
        Task<List<ServiceFeeGetDto>> GetServiceFees();
    }
}
