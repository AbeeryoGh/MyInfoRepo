using EngineCoreProject.DTOs.TransactionFeeDto;
using EngineCoreProject.IRepository.ITransactionFeeRepository;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.TransactionFeeService
{
    public class ServiceFeeRepository : IServiceFeeRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;
        ValidatorException _exception;

        public ServiceFeeRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)
        {
           _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _exception = new ValidatorException();
        }

        public async Task<int> AddServiceFee(ServiceFeePostDto serviceFeePostDto, Constants.DOCUMENT_KIND docKind, Constants.PROCESS_KIND processKind, string lang)
        {
            await ValidateServiceFee(serviceFeePostDto, docKind, processKind, 0, lang);

            ServiceFee newServiceFee = serviceFeePostDto.GetEntity();
            newServiceFee.DocumentKind = (int)docKind;
            newServiceFee.ProcessKind = (int)processKind;

            await _EngineCoreDBContext.ServiceFee.AddAsync(newServiceFee);
            await _EngineCoreDBContext.SaveChangesAsync();
            return newServiceFee.Id;
        }

        public async Task<int> UpdateServiceFee(ServiceFeePostDto serviceFeePostDto, int rowId, Constants.DOCUMENT_KIND docKind, Constants.PROCESS_KIND processKind, string lang)
        {
            int res = 0;
            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();
            var originalServiceFee = await _EngineCoreDBContext.ServiceFee.Where(a => a.Id == rowId).FirstOrDefaultAsync();

            if (originalServiceFee == null)
            {
                return res;
            }

            await ValidateServiceFee(serviceFeePostDto, docKind, processKind, rowId, lang);

            originalServiceFee.ServiceNo = serviceFeePostDto.ServiceNo;
            originalServiceFee.FeeNo = serviceFeePostDto.FeeNo;
            originalServiceFee.DocumentKind = (int)docKind;
            originalServiceFee.ProcessKind = (int)processKind;
            originalServiceFee.Required = serviceFeePostDto.Required;

            _EngineCoreDBContext.ServiceFee.Update(originalServiceFee);
            await _EngineCoreDBContext.SaveChangesAsync();

            transaction.Commit();

            res = originalServiceFee.Id;
            return res;
        }

        public async Task<int> DeleteServiceFee(int id)
        {
            int res = 0;
            ServiceFee serviceFee = await _EngineCoreDBContext.ServiceFee.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (serviceFee == null)
            {
                return res;
            }

            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();
            _EngineCoreDBContext.ServiceFee.Remove(serviceFee);
            await _EngineCoreDBContext.SaveChangesAsync();
            await transaction.CommitAsync();
            res = serviceFee.Id;
            return res;
        }

        public async Task<List<ServiceFeeGetDto>> GetServiceFeesByServiceId(int serviceId)
        {
            var servicesFees = await _EngineCoreDBContext.ServiceFee.Where(x => x.ServiceNo == serviceId).ToListAsync();
            List<ServiceFeeGetDto> result = new List<ServiceFeeGetDto>();
            foreach (var row in servicesFees)
            {         
                var tab = ServiceFeeGetDto.GetDTO(row);
                result.Add(tab);         
            }
            return result;
        }

        public async Task<List<ServiceFeeGetDto>> GetServiceFees()
        {
            var servicesFees = await _EngineCoreDBContext.ServiceFee.ToListAsync();
            List<ServiceFeeGetDto> result = new List<ServiceFeeGetDto>();
            foreach (var row in servicesFees)
            {
                var tab = ServiceFeeGetDto.GetDTO(row);
                tab.ServiceName = await _iGeneralRepository.getTranslationsForShortCut(_EngineCoreDBContext.AdmService.Where(x => x.Id == row.ServiceNo).Select(y => y.Shortcut).FirstOrDefault());
                tab.FeeName = await _iGeneralRepository.getTranslationsForShortCut(_EngineCoreDBContext.TransactionFee.Where(x => x.Id == row.FeeNo).Select(y => y.TransactionNameShortcut).FirstOrDefault());
                result.Add(tab);
            }
            return result;
        }

        private async Task ValidateServiceFee(ServiceFeePostDto serviceFeePostDto, Constants.DOCUMENT_KIND docKind, Constants.PROCESS_KIND processKind, int updatedRec, string lang)
        {
            if (docKind != Constants.DOCUMENT_KIND.AGENCY  && docKind != Constants.DOCUMENT_KIND.CONTRACTOREDITOR)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "DocumentKindNotFound"));
                throw _exception;
            }

            if (processKind != Constants.PROCESS_KIND.EDIT && processKind != Constants.PROCESS_KIND.CONFIRM)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "ProcessKindNotFound"));
                throw _exception;
            }

            if (! await _EngineCoreDBContext.AdmService.AnyAsync(x => x.Id == serviceFeePostDto.ServiceNo))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "ServiceNotFound"));
                throw _exception;
            }

            if (! await _EngineCoreDBContext.TransactionFee.AnyAsync(x => x.Id == serviceFeePostDto.FeeNo))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "FeeNotFound"));
                throw _exception;
            }

            var oldServiceFees = await _EngineCoreDBContext.ServiceFee.Where(x => x.ServiceNo == serviceFeePostDto.ServiceNo && x.Id != updatedRec).ToListAsync();

            if (oldServiceFees != null)
            {
                var oldFee = oldServiceFees.Where(x => x.FeeNo == serviceFeePostDto.FeeNo);

                if (oldFee.Any(x => x.DocumentKind == (int)docKind && x.ProcessKind == (int)processKind))
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "ConflictDocumentKind"));
                    throw _exception;
                }
            }
        }
    }
}
