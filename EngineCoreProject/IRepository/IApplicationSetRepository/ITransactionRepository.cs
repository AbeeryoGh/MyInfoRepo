using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using EngineCoreProject.DTOs.FileDto;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IApplicationSetRepository
{
    public interface ITransactionRepository
    {
        Task<List<AppTransaction>> GetAll(int? ApplicationId);
        Task<AppTransaction> GetOne(int id);
        Task<List<int>> DeleteMany(int[] ids);
        Task<int> DeleteOne(int id);
        Task<int> Add(TransactionDto transactionDto,int? addBy);
        Task<int> AddOldVersion(TransactionOldVersionDto transactionOVDto, int addBy);
        Task<UploadedFileMessage> Upload(TransactionFileDto transactionDto);
        Task<int> Update(int id, int userId, TransactionDto transactionDto);
        Task<int> Update(AppTransaction  appTransaction, int userId, TransactionDto transactionDto);

        Task<UploadedFileMessage> UploadTransactionDocument(IFormFile File);
        Task<List<ApplicationPartyView>> getRelatedParties(int transactionId, string lang);

        /*Task<TransactionStatus> GetTransactionStatus(int id, string lang);*/
        Task<TransactionStatus> GetTransactionStatus(int? Tstatus, string TNo, int appId,DateTime? TstartDate , DateTime? TendDate, bool? UnlimitedValidity, string lang);
        Task<TransactionInfo> GetTransactionStatus(int transactionId);
        Task<TransactionInfo> GetTransactionStatus(int transactionId,string transactionNo);

        Task<int> ChangeTransactionStatus(int id, int userId, string status);
        Task<int> ChangeTransactionStatus(int id, int userId, int statusId);
        Task<List<RelatedContentView>> GetTransactionRelatedContents(int id, string lang);

        TransactionDto FromObjectToDto(AppTransaction appTransaction);
        string GenerateTransactionNo();





        //  Task<APIResult> RebuildPDFDocuments(FullUpdate fu, int userId, string lang, int appId, string path);
    }
}
