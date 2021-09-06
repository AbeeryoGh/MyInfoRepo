using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using EngineCoreProject.DTOs.FileDto;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.IRepository.IFilesUploader;
using EngineCoreProject.IRepository.TemplateSetRepository;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.ApplicationSet
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _IGeneralRepository;
        private readonly IFilesUploaderRepositiory _IFilesUploaderRepository;
        private readonly ISysValueRepository _ISysValueRepository;

        public TransactionRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository, IFilesUploaderRepositiory iFilesUploaderRepository, ISysValueRepository iSysValueRepository, ITemplateRepository iTemplateRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _IGeneralRepository = iGeneralRepository;
            _IFilesUploaderRepository = iFilesUploaderRepository;
            _ISysValueRepository = iSysValueRepository;

        }
        //-----------------------------Without uploading file-----------

        public async Task<int> Add(TransactionDto tDto, int? addBy)
        {
            try
            {
                AppTransaction transaction = new AppTransaction
                {
                    ApplicationId = tDto.ApplicationId,
                    Content = tDto.Content,
                    FileName = tDto.FileName,
                    TransactionEndDate = tDto.TransactionEndDate,
                    TransactionStartDate = tDto.TransactionStartDate,
                    UnlimitedValidity = tDto.UnlimitedValidity,
                    DocumentUrl = tDto.DocumentUrl,
                    DecisionText = tDto.DecisionText,
                    TransactionStatus = tDto.TransactionStatus,
                    Qrcode = tDto.QrCode,
                    TransactionNo = tDto.TransactionNo,
                    CreatedDate = DateTime.Now,
                    LastUpdatedDate = DateTime.Now,
                    Title = tDto.Title,
                    ContractValue = tDto.ContractValue,
                    CreatedBy = addBy
                };

                _IGeneralRepository.Add(transaction);
                /*if (await _IGeneralRepository.Save())
                {
                    return transaction.Id;
                }*/
                await _IGeneralRepository.Save();
                return transaction.Id;
            }
            catch (Exception e)
            {
                return Constants.ERROR;
            }

            return Constants.ERROR;
        }

        //----------------------------- With uploading file ------------

        public async Task<UploadedFileMessage> Upload(TransactionFileDto transactionDto)
        {
            UploadedFileMessage f;
            string target = "transactions/";
            f = await _IFilesUploaderRepository.UploadFile(transactionDto.File, target);
            if (f.SuccessUpload)
                try
                {
                    AppTransaction transaction = new AppTransaction
                    {
                        ApplicationId = transactionDto.ApplicationId,
                        FileName = f.FileName
                    };

                    _IGeneralRepository.Add(transaction);
                    if (await _IGeneralRepository.Save())
                    {
                        f.Id = transaction.Id;
                        return f;
                    }

                }
                catch (Exception)
                {
                    f.Id = Constants.ERROR;
                    return f;
                }

            f.Id = Constants.ERROR;
            return f;

        }

        //--------------------------------------------------------------
        public Task<List<int>> DeleteMany(int[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<int> DeleteOne(int id)
        {
            AppTransaction transaction = await GetOne(id);
            if (transaction == null)
                return Constants.NOT_FOUND;
            try
            {
                _IGeneralRepository.Delete(transaction);
                if (await _IGeneralRepository.Save())
                    return Constants.OK;
            }
            catch (Exception)
            {
                return Constants.ERROR;
            }
            return Constants.ERROR;
        }

        public async Task<List<AppTransaction>> GetAll(int? ApplicationId)
        {
            Task<List<AppTransaction>> query = null;
            if (ApplicationId == null)

                query = _EngineCoreDBContext.AppTransaction.ToListAsync();
            else
                query = _EngineCoreDBContext.AppTransaction.Where(s => s.ApplicationId == ApplicationId).ToListAsync();
            return await query;
        }

        public async Task<AppTransaction> GetOne(int id)
        {
            var query = _EngineCoreDBContext.AppTransaction.Where(x => x.Id == id).Include(x=>x.TransactionOldVersion);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> Update(int id, int userId, TransactionDto transactionDto)
        {
            AppTransaction transaction = await GetOne(id);
            if (transaction == null)
                return Constants.NOT_FOUND;
            try
            {
                transaction.ApplicationId = transactionDto.ApplicationId;
                transaction.Content = transactionDto.Content;
                transaction.DocumentUrl = transactionDto.DocumentUrl;
                transaction.FileName = transactionDto.FileName;
                transaction.TransactionNo = transactionDto.TransactionNo;
                transaction.DecisionText = transactionDto.DecisionText;
                transaction.TransactionEndDate = transactionDto.TransactionEndDate;
                transaction.TransactionStartDate = transactionDto.TransactionStartDate;
                transaction.UnlimitedValidity = transactionDto.UnlimitedValidity;
                transaction.TransactionStatus = transactionDto.TransactionStatus;
                transaction.TransactionCreatedDate = transactionDto.TransactionCreatedDate;
                transaction.Qrcode = transactionDto.QrCode;
                transaction.Title = transactionDto.Title;
                transaction.ContractValue = transactionDto.ContractValue;
                transaction.LastUpdatedDate = DateTime.Now;
                transaction.LastUpdatedBy = userId;
                _IGeneralRepository.Update(transaction);
                if (await _IGeneralRepository.Save())
                {
                    return Constants.OK;
                }
            }
            catch (Exception e)
            {
                var a = e.Message.ToString();
                a = e.InnerException.Message.ToString();

                return Constants.ERROR;
            }
            return Constants.ERROR;
        }

        public async Task<UploadedFileMessage> UploadTransactionDocument(IFormFile File)
        {
            UploadedFileMessage m;
            string target = "TransactionFolder";
            m = await _IFilesUploaderRepository.UploadFile(File, target, "Portable document format");
            return m;
        }


        public async Task<List<ApplicationPartyView>> getRelatedParties(int transactionId, string lang)
        {
            string answerYes = "",
                   answerNo = "";
            answerYes = Constants.getMessage(lang, "YES"); //await _ISysValueRepository.GetValueByShortcut("Yes", lang);
            answerNo = Constants.getMessage(lang, "NO");//await _ISysValueRepository.GetValueByShortcut("No" , lang);

            Task<List<ApplicationPartyView>> query = null;
            query = (from ap in _EngineCoreDBContext.ApplicationParty
                     join lv in _EngineCoreDBContext.SysLookupValue
                         on ap.PartyTypeValueId equals lv.Id
                     join tr in _EngineCoreDBContext.SysTranslation
                         on lv.Shortcut equals tr.Shortcut
                     where ap.TransactionId == transactionId
                     where tr.Lang == lang
                     select new ApplicationPartyView
                     {
                         Id = ap.Id,
                         PartyId = ap.PartyId,
                         PartyTypeId = ap.PartyTypeValueId,
                         PartyTypeName = tr.Value,
                         IsOwnerText = ap.IsOwner == true ? answerYes : answerNo,
                         FullName = ap.FullName,
                         Mobile = ap.Mobile,
                         EmiratesIdNo = ap.EmiratesIdNo,
                         IsOwner = ap.IsOwner,
                         TransactionId = ap.TransactionId,
                         Email = ap.Email,
                         Nationality = (int)ap.Nationality,
                         BirthDate = ap.BirthDate,
                         MaritalStatus = (int)ap.MaritalStatus,
                         Gender = (int)ap.Gender,
                         IdExpirationDate = ap.IdExpirationDate,
                         IdAttachment = ap.IdAttachment,
                         UnifiedNumber = ap.UnifiedNumber,
                         SignRequired = ap.SignRequired,
                         SignRequiredText = (bool)ap.SignRequired ? answerYes : answerNo,
                         Signed = ap.Signed,
                         SignedText = (bool)ap.Signed ? answerYes : answerNo,
                         SignType = ap.SignType,
                         SignDate = ap.SignDate,
                         SignUrl = ap.SignUrl,
                         EditableSign = ap.EditableSign,
                         Address = ap.Address,
                         Description = ap.Description,
                         City=ap.City,
                         Emirate=ap.Emirate,
                         AlternativeEmail=ap.AlternativeEmail,
                         PartyExtraAttachment = ap.ApplicationPartyExtraAttachment,

                     }).ToListAsync();

            return await query;
        }

        /* public async Task<TransactionStatus> GetTransactionStatus(int id,string lang)
         {
             AppTransaction at = await GetOne(id);
             TransactionStatus ts = new TransactionStatus();
             ts.StatusId = (int)at.TransactionStatus;
             int CanceledId = await _ISysValueRepository.GetIdByShortcut("CANCELED");
             if (at.TransactionStatus != CanceledId)
              { 
                 if (at.UnlimitedValidity == false)
                  {
                     if ( DateTime.Now > at.TransactionEndDate)//DateTime.Now < at.TransactionStartDate ||
                     {
                         ts.IsValid  = false;
                         ts.Status   = await _ISysValueRepository.GetValueByShortcut("EXPIRED", lang);
                     }
                     else
                     {
                         ts.IsValid = true;
                         ts.Status = await _ISysValueRepository.GetValueByShortcut("VALID", lang);
                     }
                  }
                 else
                  {
                     ts.IsValid = true;
                     ts.Status = await _ISysValueRepository.GetValueByShortcut("VALID", lang);
                  }
                }
             else
              {
                 ts.IsValid = false;
                 ts.Status = await _ISysValueRepository.GetValueByShortcut("CANCELED", lang);
              }


             return ts;
         }*/

        public async Task<TransactionStatus> GetTransactionStatus(int? Tstatus, string TNo, int appId, DateTime? TstartDate, DateTime? TendDate, bool? UnlimitedValidity, string lang)
        {
            TransactionStatus ts = new TransactionStatus();
            if (TNo == null || TNo.Length == 0)
            {
                ts.IsValid = false;
                ts.Status = "غير منجزة بعد";//await _ISysValueRepository.GetValueByShortcut("CANCELED", lang);
                return ts;
            }
            int CanceledId = await _ISysValueRepository.GetIdByShortcut("CANCELED");
            int ExecutedId = await _ISysValueRepository.GetIdByShortcut("EXECUTED");

            if (Tstatus != CanceledId)
            {
                if (UnlimitedValidity == false)
                {
                    if (DateTime.Now > TendDate)//DateTime.Now < TstartDate || 
                    {
                        ts.IsValid = false;
                        ts.Status = await _ISysValueRepository.GetValueByShortcut("EXPIRED", lang);
                    }
                    else
                    {
                        ts.IsValid = true;
                        ts.Status = await _ISysValueRepository.GetValueByShortcut("VALID", lang);
                    }
                }
                else
                {
                    ts.IsValid = true;
                    ts.Status = await _ISysValueRepository.GetValueByShortcut("VALID", lang);
                }
                if(Tstatus== ExecutedId)
                {
                  ts.Status = ts.Status+" | " + await _ISysValueRepository.GetValueByShortcut("EXECUTED", lang);
                }
            }
            else
            {
                ts.IsValid = false;
                ts.Status = await _ISysValueRepository.GetValueByShortcut("CANCELED", lang);
            }
            return ts;
        }

        public async Task<int> ChangeTransactionStatus(int id, int userId, string status)
        {
            int StatusId = await _ISysValueRepository.GetIdByShortcut(status);
            AppTransaction t = await GetOne(id);
            TransactionDto tDto = TransactionDto.GetDto(t);
            tDto.TransactionStatus = StatusId;
            return await Update(id,userId, tDto);
        }
        public async Task<int> ChangeTransactionStatus(int id, int userId, int statusId)
        {
            AppTransaction t = await GetOne(id);
            TransactionDto tDto = TransactionDto.GetDto(t);
            tDto.TransactionStatus = statusId;
            return await Update(id,userId, tDto);
        }

        public async Task<TransactionInfo> GetTransactionStatus(int transactionId)
        {
            TransactionInfo result = new TransactionInfo();
            AppTransaction at = await GetOne(transactionId);
            if(at==null)
            {
                result = null;
            }
            else 
            {
            TransactionStatus status = await GetTransactionStatus(at.TransactionStatus, at.TransactionNo, at.Id, at.TransactionStartDate, at.TransactionEndDate, at.UnlimitedValidity, "ar");
            result.IsValid = status.IsValid;
            result.Status = status.Status;
            result.DocumentUrl = at.DocumentUrl;
            result.TransactionNo = at.TransactionNo;
            result.StartDate = at.TransactionStartDate;
            result.EndDate = at.TransactionEndDate;
            result.CreatedDate = at.TransactionCreatedDate;
            
            if (at.TransactionOldVersion.Count > 0) 
                {
                    result.MultiVersion = true;
                    result.Messages = new List<string>();
                    result.Messages.Add ( "تم إصدار أكثر من نسخة من هذه الوثيقة ");
                    result.Messages.Add($"{at.TransactionNo}: رقم الوثيقة الصحيحة هو  ");
                    result.OldVersion = new List<TransactionOldVersionDto>();
                    foreach (var oldVersion in at.TransactionOldVersion)
                    {
                        TransactionOldVersionDto dto = new TransactionOldVersionDto();
                        dto.DocumentUrl = oldVersion.DocumentUrl;
                        dto.Note = oldVersion.Note;
                        dto.TransactionId = oldVersion.TransactionId;
                        dto.TransactionNo = oldVersion.TransactionNo;
                        dto.TransactionCreatedDate = oldVersion.TransactionCreatedDate;
                        result.OldVersion.Add(dto);
                    }
                }

                // List<RelatedContentView> rc = await GetTransactionRelatedContents(transactionId, "ar");
                // result.RelatedDataUrlsDocumentUrl = rc; 

            }
            return result;
        }

        public async Task<List<RelatedContentView>> GetTransactionRelatedContents(int tid, string lang)
        {
            int? appId = _EngineCoreDBContext.AppTransaction.Where(t => t.Id == tid).Select(x => x.ApplicationId).FirstOrDefault();
            Task<List<RelatedContentView>> query = null;
            query = (

                    //  from appt in _EngineCoreDBContext.AppTransaction join
                    from arc in _EngineCoreDBContext.AppRelatedContent
                        ///on appt.ApplicationId equals arc.AppId
                    join tr in _EngineCoreDBContext.SysTranslation
                        on arc.TitleShortcut equals tr.Shortcut

                    where arc.AppId == appId
                    where tr.Lang == lang
                    select new RelatedContentView
                    {
                        Id = arc.Id,
                        TitleSortcut = tr.Shortcut,
                        Title = tr.Value,
                        Content = arc.Content,
                        ContentUrl = arc.ContentUrl

                    }).ToListAsync();

            return await query;
        }

        public TransactionDto FromObjectToDto(AppTransaction appt)
        {
            TransactionDto transactionDto = new TransactionDto()
            {
                ApplicationId = appt.ApplicationId,
                Content = appt.Content,
                FileName = appt.FileName,
                DecisionText = appt.DecisionText,
                DocumentUrl = appt.DocumentUrl,
                TransactionStatus = appt.TransactionStatus,
                UnlimitedValidity = appt.UnlimitedValidity,
                TransactionEndDate = appt.TransactionEndDate,
                TransactionStartDate = appt.TransactionStartDate,
                Title = appt.Title,
                ContractValue = appt.ContractValue,
                QrCode = appt.Qrcode,
                LastUpdatedBy = appt.LastUpdatedBy,
                TransactionNo = appt.TransactionNo,
                LastUpdatedDate= appt.LastUpdatedDate,
                TransactionCreatedDate= appt.TransactionCreatedDate  


        };
            return transactionDto;
        }


        public  string GenerateTransactionNo()
        {
            var p = new SqlParameter("@result", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };
             _EngineCoreDBContext.Database.ExecuteSqlRaw("set @result = next value for transaction_seq", p);
            int sequenceNum = (int)p.Value;
            string TransactionNo = Constants.TRANSACTION_NO_PREFIX + DateTime.Now.Year.ToString() + "_" + sequenceNum.ToString().PadLeft(7, '0');
            return TransactionNo;
        }

        public async Task<int> AddOldVersion(TransactionOldVersionDto TOVDto, int addBy)
        {
            try
            {
                TransactionOldVersion transactionOV = new TransactionOldVersion
                {
                    TransactionId= TOVDto.TransactionId,
                   // OldTransactionId=TOVDto.OldTransactionId,
                    TransactionCreatedDate= TOVDto.TransactionCreatedDate,
                    DocumentUrl= TOVDto.DocumentUrl,
                    TransactionNo = TOVDto.TransactionNo,
                    Note= TOVDto.Note,
                    CreatedDate = DateTime.Now,
                    LastUpdatedDate = DateTime.Now,
                    CreatedBy = addBy
                };

                _IGeneralRepository.Add(transactionOV);
                if (await _IGeneralRepository.Save())
                {
                    return transactionOV.Id;
                }
            }
            catch (Exception E)
            {
                return Constants.ERROR;
            }

            return Constants.ERROR;
        }

        public async Task<int> Update(AppTransaction appT, int userId, TransactionDto tDto)
        {

            try
            {
                appT.ApplicationId = tDto.ApplicationId;
                appT.Content = tDto.Content;
                appT.DocumentUrl = tDto.DocumentUrl;
                appT.FileName = tDto.FileName;
                appT.TransactionNo = tDto.TransactionNo;
                appT.DecisionText = tDto.DecisionText;
                appT.TransactionEndDate = tDto.TransactionEndDate;
                appT.TransactionStartDate = tDto.TransactionStartDate;
                appT.UnlimitedValidity = tDto.UnlimitedValidity;
                appT.TransactionStatus = tDto.TransactionStatus;
                appT.TransactionCreatedDate = tDto.TransactionCreatedDate;
                appT.Qrcode = tDto.QrCode;
                appT.Title = tDto.Title;
                appT.ContractValue = tDto.ContractValue;
                appT.LastUpdatedDate = DateTime.Now;
                appT.LastUpdatedBy = userId;
                _IGeneralRepository.Update(appT);
                if (await _IGeneralRepository.Save())
                {
                    return Constants.OK;
                }
            }
            catch (Exception e)
            {
                var a = e.Message.ToString();
                a = e.InnerException.Message.ToString();

                return Constants.ERROR;
            }
            return Constants.ERROR;
        }

        public async Task<TransactionInfo> GetTransactionStatus(int transactionId, string transactionNo)
        {
            TransactionInfo result = new TransactionInfo();
           // if(transactionId < 400)
            AppTransaction at = await GetOne(transactionId);
            if (at == null)
            {
                result = null;
            }
            else
            {
                TransactionStatus status = await GetTransactionStatus(at.TransactionStatus, at.TransactionNo, at.Id, at.TransactionStartDate, at.TransactionEndDate, at.UnlimitedValidity, "ar");
                result.IsValid = status.IsValid;
                result.Status = status.Status;
                result.DocumentUrl = at.DocumentUrl;
                result.TransactionNo = at.TransactionNo;
                List<RelatedContentView> rc = await GetTransactionRelatedContents(transactionId, "ar");
                result.RelatedDataUrlsDocumentUrl = rc;
            }
            return result;
        }
    }
}
