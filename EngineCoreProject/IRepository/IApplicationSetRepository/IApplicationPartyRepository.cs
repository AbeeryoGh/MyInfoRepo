using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using EngineCoreProject.DTOs.FileDto;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IApplicationSetRepository
{
    public interface IApplicationPartyRepository
    {
        Task<List<ApplicationParty>> GetAll(int? transactionId);
        Task<ApplicationParty> GetAppParty(int transactionId, int partyIdAsUser);
        Task<List<int>> GetAllIDs(int TransactionId);

        Task<List<Receiver>> GetAllReceivers(int TransactionId,bool justRequiredParty);
        Task<ApplicationParty> GetOne(int id);
        Task<List<int>> DeleteMany(int[] ids);
        Task<int> DeleteOne(int id);
        Task<APIResult> DeleteParty(int id);
        Task<int> AddWithAttachment(ApplicationPartyWithAttachmentsDto applicationPartyDto, string targetFolder);
        Task<int> Add(ApplicationPartyDto applicationPartyDto,int? addBy);
        Task<int> Update(int id,int userId , ApplicationPartyDto applicationPartyDto);
        Task<UploadedFileMessage> UploadPartyAttachment(IFormFile File);
        Task<int> AddExtraAttachment(ApplicationPartyExtraAttachmentDto partyExtraAttachmentDto);
        Task<List<ApplicationPartyExtraAttachment>> GetAllExtraAttachment(int? ApplicationPartyId);
        Task<ApplicationPartyExtraAttachment> GetOneExtraAttachment(int id);
        // Task<List<int>> DeleteManyExtraAttachment(int[] ids);
        Task<int> DeleteOneExtraAttachment(int id);
        Task<int> UpdateExtraAttachment(int id, ApplicationPartyExtraAttachmentDto applicationPartyExtraAttachment);
        Task<APIResult> AddPartyToUser(ApplicationPartyDto applicationPartyDto, string lang);
        Task<List<ApplicationPartySignState>> PartySignState(int transactionId);
        Task<APIResult> IsSignEditByAnotherUser(int transactionId,int userId);
        Task<APIResult> ClearPartySignInfo(int partyIdint ,int userId);
        Task<APIResult> ClearPartiesSignInfo(int transactionId, int userId);
        Task<int> NotSignedPartyCount(int transactionId);
        string SavePartySignImage(string Base64String);
        Task<APIResult> SwitchPartySignStatus(int id,int userId);

        Task<APIResult> SwitchPartySignRequired(int id, int userId);

        Task<int> FixAPPParty();
    }
    
}
