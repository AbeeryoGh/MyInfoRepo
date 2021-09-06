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
    public class TransactionFeeRepository : ITransactionFeeRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;
        ValidatorException _exception;

        public TransactionFeeRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)
        {
           _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _exception = new ValidatorException();
        }

        public async Task<int> AddTransactionFee(TransactionFeePostDto transactionFeePostDto, string lang)
        {
            if (transactionFeePostDto.NameShortcut.Count() == 0)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "missedFeeName"));
                throw _exception;
            }

            if (await _EngineCoreDBContext.TransactionFee.AnyAsync(x => x.SubClass == transactionFeePostDto.SubClass && x.PrimeClass == transactionFeePostDto.PrimeClass))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "duplicatedFeeSubPrimeClass"));
                throw _exception;
            }

            TransactionFee newTransactionFee = transactionFeePostDto.GetEntity();
            newTransactionFee.TransactionNameShortcut = _iGeneralRepository.GenerateShortCut(Constants.TRANSACTION_FEE, Constants.TRANSACTION_NAME_SHORTCUT);
            await _iGeneralRepository.InsertUpdateSingleTranslation(newTransactionFee.TransactionNameShortcut, transactionFeePostDto.NameShortcut);

            await _EngineCoreDBContext.TransactionFee.AddAsync(newTransactionFee);
            await _EngineCoreDBContext.SaveChangesAsync();
            return newTransactionFee.Id;
        }

        public async Task<int> UpdateTransactionFee(TransactionFeePostDto transactionFeePostDto, int rowId, string lang)
        {
            int res = 0;
            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();
            var originalFee =  await _EngineCoreDBContext.TransactionFee.Where(a => a.Id == rowId).FirstOrDefaultAsync();

            if (originalFee == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "FeeNotFound"));
                throw _exception;
            }

            if (transactionFeePostDto.NameShortcut.Count() == 0)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "missedFeeName"));
                throw _exception;
            }

            if (await _EngineCoreDBContext.TransactionFee.AnyAsync(x => x.Id != rowId && x.SubClass == transactionFeePostDto.SubClass && x.PrimeClass == transactionFeePostDto.PrimeClass))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "duplicatedFeeSubPrimeClass"));
                throw _exception;
            }

            originalFee.LimitedValue = transactionFeePostDto.LimitedValue;
            originalFee.MaxLimitedTax = transactionFeePostDto.MaxLimitedTax;
            originalFee.Multiplied = transactionFeePostDto.Multiplied;
            originalFee.Percentage = transactionFeePostDto.Percentage;
            originalFee.Notes = transactionFeePostDto.Note;
            originalFee.PrimeClass = transactionFeePostDto.PrimeClass;
            originalFee.SubClass = transactionFeePostDto.SubClass;
            originalFee.Value = transactionFeePostDto.Value;

            _EngineCoreDBContext.TransactionFee.Update(originalFee);
            await _EngineCoreDBContext.SaveChangesAsync();

            await _iGeneralRepository.InsertUpdateSingleTranslation(originalFee.TransactionNameShortcut, transactionFeePostDto.NameShortcut);
            transaction.Commit();

            res = originalFee.Id;
            return res;
        }

        public async Task<int> DeleteTransactionFee(int id, string lang)
        {
            int res = 0;
            TransactionFee transactionFee = await _EngineCoreDBContext.TransactionFee.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (transactionFee == null)
            {
                return res;
            }

            if (await _EngineCoreDBContext.ServiceFee.AnyAsync(x => x.FeeNo == id))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "joinedRecord"));
                throw _exception;
            }

            using var transaction = _EngineCoreDBContext.Database.BeginTransaction();
            await _iGeneralRepository.DeleteTranslation(transactionFee.TransactionNameShortcut);
            _EngineCoreDBContext.TransactionFee.Remove(transactionFee);
            await _EngineCoreDBContext.SaveChangesAsync();
            await transaction.CommitAsync();
            res = transactionFee.Id;
            return res;
        }

        public async Task<List<TransactionFeeGetDto>> GetTransactionFees()
        {
            var transactionFees = await _EngineCoreDBContext.TransactionFee.ToListAsync();
            List<TransactionFeeGetDto> result = new List<TransactionFeeGetDto>();
            foreach (var row in transactionFees)
            {
                var tab = TransactionFeeGetDto.GetDTO(row);
                tab.FeeName = await _iGeneralRepository.getTranslationsForShortCut(row.TransactionNameShortcut);
                result.Add(tab);
            }
            return result;
        }

        public async Task<List<TransactionFeeOutput>> CalculateTransactionFee(TransactionFeeInput transactionFeeInput, string lang)
        {
            List<TransactionFeeOutput> res = new List<TransactionFeeOutput>();
            var allServiceFee = await _EngineCoreDBContext.ServiceFee.Where(x => x.ServiceNo == transactionFeeInput.ServiceNo).ToListAsync();

            if (allServiceFee == null || allServiceFee.Count == 0)
            {
                return res;
            }

            var allFees = await _EngineCoreDBContext.TransactionFee.ToListAsync();

            List<int> docKind = new List<int>();
            List<int> procKind = new List<int>();

            if (transactionFeeInput.DocumentKind != Constants.DOCUMENT_KIND.ALL)
            {
                docKind.Add((int)transactionFeeInput.DocumentKind);
            }
            else
            {
                docKind.Add((int)Constants.DOCUMENT_KIND.CONTRACTOREDITOR);
                docKind.Add((int)Constants.DOCUMENT_KIND.AGENCY);
            }

            if (transactionFeeInput.ProcessKind != Constants.PROCESS_KIND.ALL)
            {
                procKind.Add((int)transactionFeeInput.ProcessKind);
            }
            else
            {
                procKind.Add((int)Constants.PROCESS_KIND.CONFIRM);
                procKind.Add((int)Constants.PROCESS_KIND.EDIT);
            }

            var ServiceFeesDocProcKind = allServiceFee.Where(x => docKind.Contains(x.DocumentKind) && procKind.Contains(x.ProcessKind)).ToList();

            foreach (var serviceFee in ServiceFeesDocProcKind)
            {
                var fee = allFees.Where(x => x.Id == serviceFee.FeeNo).FirstOrDefault();

                if (fee.LimitedValue)
                {
                    if (transactionFeeInput.Amount >= 0 && fee.LessThan != null)
                    {
                        if (transactionFeeInput.Amount >= fee.LessThan)
                        {
                            continue;
                        }
                    }

                    if (transactionFeeInput.Amount >= 0 && fee.MoreThan != null)
                    {
                        if (transactionFeeInput.Amount <= fee.MoreThan)
                        {
                            continue;
                        }
                    }

                    if (fee.LessThan == null && fee.MoreThan == null)
                    {
                        if (transactionFeeInput.Amount > 0)
                        {
                            continue;
                        }
                    }
                }

                var feeName = await _iGeneralRepository.GetTranslateByShortCut(lang, fee.TransactionNameShortcut);
                double val = fee.Value;

                if (fee.Percentage)
                {
                    val = Math.Round(fee.Value * transactionFeeInput.Amount, 2);
                }

                var quantity = 1;
                if (fee.Multiplied && !fee.PerPage)
                {
                    val = Math.Round(fee.Value * transactionFeeInput.PartiesCount, 2);
                    quantity = transactionFeeInput.PartiesCount;
                }

                if (fee.Multiplied && fee.PerPage)
                {
                    val = Math.Round(fee.Value * transactionFeeInput.PageCount, 2);
                    quantity = transactionFeeInput.PageCount;
                }

                if (fee.MaxLimitedTax != null && fee.MaxLimitedTax > 0)
                {
                    val = Math.Min(val, (int)fee.MaxLimitedTax);
                }

                if (val == 0)
                {
                    continue;
                }

                if (res.Any(x => x.FeeNo == fee.Id))
                {
                    var oldFee = res.Where(x => x.FeeNo == fee.Id).FirstOrDefault();
                    oldFee.FeeValue += val;
                    oldFee.Quantity += quantity;
                }
                else
                {
                    TransactionFeeOutput feeRes = new TransactionFeeOutput()
                    {
                        FeeNo = fee.Id,
                        FeeName = feeName,
                        PrimeClass = fee.PrimeClass,
                        SubCalss = fee.SubClass,
                        FeeValue = val,
                        Quantity = quantity
                    };

                   

                    res.Add(feeRes);
                }
            }

            return res;
        }
    }
}
