using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.AccountDto;
using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using EngineCoreProject.DTOs.FileDto;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.IRepository.IFilesUploader;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace EngineCoreProject.Services.ApplicationSet
{
    public class ApplicationPartyRepositiory : IApplicationPartyRepository
    {

        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _IGeneralRepository;
        private readonly IFilesUploaderRepositiory _IFilesUploaderRepository;
        private readonly IUserRepository _IUserRepository;


        public ApplicationPartyRepositiory(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository, IFilesUploaderRepositiory iFilesUploaderRepository, IUserRepository iUserRepositiory)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _IGeneralRepository = iGeneralRepository;
            _IFilesUploaderRepository = iFilesUploaderRepository;
            _IUserRepository = iUserRepositiory;

        }


        public async Task<List<ApplicationParty>> GetAll(int? TransactionId)
        {
            Task<List<ApplicationParty>> query = null;
            if (TransactionId == null)
                query = _EngineCoreDBContext.ApplicationParty.ToListAsync();
            else
                query = _EngineCoreDBContext.ApplicationParty.Where(s => s.TransactionId == TransactionId).ToListAsync();
            return await query;
        }
        //-----------------------------------------Get All IDs----------------------------------------
        public async Task<List<int>> GetAllIDs(int TransactionId)
        {
            Task<List<int?>> query = null;
            List<int> IDs = new List<int>();
            query = _EngineCoreDBContext.ApplicationParty.Where(s => s.TransactionId == TransactionId).Select(a => a.PartyId).ToListAsync();

            var l = await query;
            for (int i = 0; i < l.Count; ++i)
                if (l[i] != null)
                    IDs.Add((int)l[i]);
            return IDs;
        }
        //----------------------------------------------NNNN------------------------------------
        public async Task<List<Receiver>> GetAllReceivers(int TransactionId, bool justRequiredParty)
        {
            Task<List<Receiver>> recivers = null;
            List<int> IDs = new List<int>();
            if (justRequiredParty)
                recivers = _EngineCoreDBContext.ApplicationParty.Where(s => s.TransactionId == TransactionId && s.SignRequired == true)
                                                             .Select(a => new Receiver()
                                                             {
                                                                 Id = (int)a.PartyId,
                                                                 Mobile = a.Mobile,
                                                                 Email = (a.AlternativeEmail != null && a.AlternativeEmail.Trim().Length > 0) ? a.AlternativeEmail : a.Email,
                                                                 Name = a.FullName
                                                             }).ToListAsync();
            else
                recivers = _EngineCoreDBContext.ApplicationParty.Where(s => s.TransactionId == TransactionId)
                                                             .Select(a => new Receiver()
                                                             {
                                                                 Id = (int)a.PartyId,
                                                                 Mobile = a.Mobile,
                                                                 Email = (a.AlternativeEmail != null && a.AlternativeEmail.Trim().Length > 0) ? a.AlternativeEmail : a.Email,
                                                                 Name = a.FullName
                                                             }).ToListAsync();

            return await recivers;
        }



        //--------------------------------------------------------------------------------------------
        public async Task<ApplicationParty> GetOne(int id)
        {
            id = Convert.ToInt32(id);
            var query = _EngineCoreDBContext.ApplicationParty.Where(x => x.Id == id).Include(y => y.ApplicationPartyExtraAttachment);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> FixAPPParty()
        {
            int count = 0;
            var query = await _EngineCoreDBContext.ApplicationParty.Where(x => x.Email == null || x.Email == "" || x.Email == "0").ToListAsync();
            foreach (var nullEmail in query)
            {
                var app = await _EngineCoreDBContext.ApplicationParty.Where(x => x.TransactionId == nullEmail.TransactionId).ToListAsync();
                if (app.Count == 2)
                {
                    var valEmail = app.Where(x => x.Email != null && x.Email != "" && x.Email != "0").FirstOrDefault();
                    if (valEmail != null)
                    {
                        nullEmail.Email = valEmail.Email;
                        _EngineCoreDBContext.ApplicationParty.Update(nullEmail);
                        await _EngineCoreDBContext.SaveChangesAsync();
                        count++;
                    }
                }
            }
            return count;
        }

        public async Task<int> AddWithAttachment(ApplicationPartyWithAttachmentsDto appPartyDto, string target)
        {
            try
            {
                UploadedFileMessage idAttachment = await _IFilesUploaderRepository.UploadFile(appPartyDto.IdAttachmentFile, target);
                UploadedFileMessage visaAttachment = await _IFilesUploaderRepository.UploadFile(appPartyDto.VisaAttachmentFile, target);
                UploadedFileMessage passportAttachment = await _IFilesUploaderRepository.UploadFile(appPartyDto.PassportAttachmentFile, target);
                if ((idAttachment.SuccessUpload) && (visaAttachment.SuccessUpload) && (passportAttachment.SuccessUpload))
                {

                    ApplicationParty applicationParty = new ApplicationParty
                    {
                        TransactionId = appPartyDto.TransactionId,
                        PartyId = appPartyDto.PartyId,
                        IsOwner = appPartyDto.IsOwner,
                        PartyTypeValueId = appPartyDto.PartyTypeValueId,
                        FullName = appPartyDto.FullName,
                        Mobile = appPartyDto.Mobile,
                        Email = appPartyDto.Email,
                        Nationality = appPartyDto.Nationality,
                        BirthDate = appPartyDto.BirthDate,
                        MaritalStatus = appPartyDto.MaritalStatus,
                        Gender = appPartyDto.Gender,
                        EmiratesIdNo = appPartyDto.EmiratesIdNo,
                        IdExpirationDate = appPartyDto.IdExpirationDate,
                        IdAttachment = idAttachment.FileName,
                        UnifiedNumber = appPartyDto.UnifiedNumber,

                    };

                    _IGeneralRepository.Add(applicationParty);
                    if (await _IGeneralRepository.Save())
                    {
                        return applicationParty.Id;
                    }
                }
                else return Constants.ERROR;

            }
            catch (Exception)
            {
                return Constants.ERROR;
            }

            return Constants.ERROR;
        }

        //--------------------------------------Add With out File----------------------------------------
        public async Task<int> Add(ApplicationPartyDto appPartyDto, int? addBy)
        {
            try
            {

                ApplicationParty applicationParty = new ApplicationParty
                {
                    TransactionId = appPartyDto.TransactionId,
                    PartyId = appPartyDto.PartyId,
                    IsOwner = appPartyDto.IsOwner,
                    PartyTypeValueId = appPartyDto.PartyTypeId,
                    FullName = appPartyDto.FullName,
                    Mobile = appPartyDto.Mobile,
                    Email = appPartyDto.Email,
                    Nationality = appPartyDto.Nationality == 0 ? null : appPartyDto.Nationality,
                    BirthDate = appPartyDto.BirthDate,
                    MaritalStatus = appPartyDto.MaritalStatus == 0 ? null : appPartyDto.MaritalStatus,
                    Gender = appPartyDto.Gender == 0 ? null : appPartyDto.Gender,
                    EmiratesIdNo = appPartyDto.EmiratesIdNo,
                    IdExpirationDate = appPartyDto.IdExpirationDate,
                    IdAttachment = appPartyDto.IdAttachment,
                    UnifiedNumber = appPartyDto.UnifiedNumber,
                    SignRequired = appPartyDto.SignRequired,
                    Signed = appPartyDto.Signed,
                    SignType = appPartyDto.SignType,
                    SignDate = appPartyDto.SignDate,
                    SignUrl = appPartyDto.SignUrl,
                    EditableSign = false,
                    Address = appPartyDto.Address,
                    CreatedBy = addBy,
                    LastUpdatedBy = addBy,
                    CreatedDate = DateTime.Now,
                    LastUpdatedDate = DateTime.Now,
                    Description = appPartyDto.Description,
                    Emirate = appPartyDto.Emirate,
                    AlternativeEmail = appPartyDto.AlternativeEmail,
                    City = appPartyDto.City,
                    NotaryId = appPartyDto.NotaryId




                };

                _IGeneralRepository.Add(applicationParty);
                if (await _IGeneralRepository.Save())
                {
                    return applicationParty.Id;
                }


            }
            catch (Exception e)
            {
                var r = e.InnerException.ToString();
                return Constants.ERROR;
            }

            return Constants.ERROR;
        }

        //--------------------------------------------------------------------------------------------------
        public Task<List<int>> DeleteMany(int[] ids)
        {
            throw new NotImplementedException();
        }

        //-----------------------------------------------Delete One-------------------------------------------
        public async Task<int> DeleteOne(int id)
        {
            ApplicationParty applicationParty = await GetOne(id);
            if (applicationParty == null)
                return Constants.NOT_FOUND;
            try
            {
                _IGeneralRepository.Delete(applicationParty);
                if (await _IGeneralRepository.Save())
                    // try
                    // {
                    //   if (File.Exists(Path.Combine(rootFolder, applicationParty.IdAttachment)))   
                    //   File.Delete(Path.Combine(rootFolder, authorsFile)); 
                    return Constants.OK;
                //  }                 

                //catch (IOException )
                //  {
                //   return Constants.ERROR;
                //  }



            }
            catch (Exception)
            {
                return Constants.ERROR;
            }
            return Constants.ERROR;
        }

        public async Task<APIResult> DeleteParty(int id)
        {
            ApplicationParty applicationParty = await GetOne(id);
            List<string> msgs = new List<string>();
            if (applicationParty == null)
            {
                APIResult result = new APIResult(Constants.NOT_FOUND, false, 204, null);
                result.Message.Add("الطرف غير موجود");
                return result;
            }
            try
            {
                List<ApplicationPartyExtraAttachment> extras = await GetAllExtraAttachment(id);
                using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                foreach (var extra in extras)
                {
                    await DeleteOneExtraAttachment(extra.Id);
                }
                _IGeneralRepository.Delete(applicationParty);
                if (await _IGeneralRepository.Save())
                {
                    APIResult result = new APIResult(id, true, 200, msgs);
                    result.Message.Add("تم حذف الطرف");
                    scope.Complete();
                    return result;

                }

            }
            catch (Exception)
            {
                APIResult result = new APIResult(Constants.ERROR, false, 500, msgs);
                result.Message.Add("خطأ في الحذف");
                return result;

            }
            APIResult result_ = new APIResult(Constants.ERROR, false, 500, msgs);
            result_.Message.Add("خطأ في الحذف");
            return result_;

        }



        //------------------------------------------------Update-----------------------------------------------        
        public async Task<int> Update(int id, int userId, ApplicationPartyDto appPartyDto)
        {
            ApplicationParty appParty = await GetOne(id);
            if (appParty == null)
                return Constants.NOT_FOUND;
            try
            {
                appParty.PartyId = appPartyDto.PartyId;
                appParty.TransactionId = appPartyDto.TransactionId;
                appParty.IsOwner = appPartyDto.IsOwner;
                appParty.PartyTypeValueId = appPartyDto.PartyTypeId;
                appParty.FullName = appPartyDto.FullName;
                appParty.Mobile = appPartyDto.Mobile;
                appParty.Email = appPartyDto.Email;
                appParty.Nationality = appPartyDto.Nationality == 0 ? null : appPartyDto.Nationality;
                appParty.BirthDate = appPartyDto.BirthDate;
                appParty.MaritalStatus = appPartyDto.MaritalStatus == 0 ? null : appPartyDto.MaritalStatus;
                appParty.Gender = appPartyDto.Gender == 0 ? null : appPartyDto.Gender;
                appParty.EmiratesIdNo = appPartyDto.EmiratesIdNo;
                appParty.IdExpirationDate = appPartyDto.IdExpirationDate;
                appParty.IdAttachment = appPartyDto.IdAttachment;
                appParty.UnifiedNumber = appPartyDto.UnifiedNumber;
                appParty.SignRequired = appPartyDto.SignRequired;
                appParty.Signed = appPartyDto.Signed;
                appParty.SignType = appPartyDto.SignType;
                appParty.SignDate = appPartyDto.SignDate;
                appParty.SignUrl = appPartyDto.SignUrl;
                appParty.EditableSign = appPartyDto.EditableSign;
                appParty.Description = appPartyDto.Description;
                appParty.Address = appPartyDto.Address;
                appParty.AlternativeEmail = appPartyDto.AlternativeEmail;
                appParty.Emirate = appPartyDto.Emirate;
                appParty.City = appPartyDto.City;
                appParty.LastUpdatedBy = userId;
                appParty.LastUpdatedDate = DateTime.Now;
                appParty.NotaryId = appPartyDto.NotaryId;

                _IGeneralRepository.Update(appParty);
                if (await _IGeneralRepository.Save())
                {
                    return Constants.OK;
                }
            }
            catch (Exception)
            {
                return Constants.ERROR;
            }
            return Constants.ERROR;
        }
        //------------------------updateBY object-----------------
        /*public async Task<int> Update(ApplicationParty appParty, int userId, ApplicationPartyDto appPartyDto)
        {

            try
            {
                appParty.PartyId = appPartyDto.PartyId;
                appParty.TransactionId = appPartyDto.TransactionId;
                appParty.IsOwner = appPartyDto.IsOwner;
                appParty.PartyTypeValueId = appPartyDto.PartyTypeId;
                appParty.FullName = appPartyDto.FullName;
                appParty.Mobile = appPartyDto.Mobile;
                appParty.Email = appPartyDto.Email;
                appParty.Nationality = appPartyDto.Nationality == 0 ? null : appPartyDto.Nationality;
                appParty.BirthDate = appPartyDto.BirthDate;
                appParty.MaritalStatus = appPartyDto.MaritalStatus == 0 ? null : appPartyDto.MaritalStatus;
                appParty.Gender = appPartyDto.Gender == 0 ? null : appPartyDto.Gender;
                appParty.EmiratesIdNo = appPartyDto.EmiratesIdNo;
                appParty.IdExpirationDate = appPartyDto.IdExpirationDate;
                appParty.IdAttachment = appPartyDto.IdAttachment;
                appParty.UnifiedNumber = appPartyDto.UnifiedNumber;
                appParty.SignRequired = appPartyDto.SignRequired;
                appParty.Signed = appPartyDto.Signed;
                appParty.SignType = appPartyDto.SignType;
                appParty.SignDate = appPartyDto.SignDate;
                appParty.SignUrl = appPartyDto.SignUrl;
                appParty.EditableSign = appPartyDto.EditableSign;
                appParty.Description = appPartyDto.Description;
                appParty.Address = appPartyDto.Address;
                appParty.AlternativeEmail = appPartyDto.AlternativeEmail;
                appParty.Emirate = appPartyDto.Emirate;
                appParty.City = appPartyDto.City;
                appParty.LastUpdatedBy = userId;
                appParty.LastUpdatedDate = DateTime.Now;
                appParty.NotaryId = appPartyDto.NotaryId;

                _IGeneralRepository.Update(appParty);
                if (await _IGeneralRepository.Save())
                {
                    return Constants.OK;
                }
            }
            catch (Exception)
            {
                return Constants.ERROR;
            }
            return Constants.ERROR;
        }*/



        //--------------------------------------Upload Party Attachment-----------------------------------------
        public async Task<UploadedFileMessage> UploadPartyAttachment(IFormFile File)
        {
            UploadedFileMessage f;
            string target = "PartyFileFolder";
            f = await _IFilesUploaderRepository.UploadFile(File, target);
            return f;
        }

        //--------------------------------------eXtra attachments-----------------------------------------------
        public async Task<int> AddExtraAttachment(ApplicationPartyExtraAttachmentDto partyExtraAttachmentDto)
        {
            try
            {
                ApplicationPartyExtraAttachment partyExtraAttachment = new ApplicationPartyExtraAttachment
                {
                    ApplicationPartyId = (int)partyExtraAttachmentDto.ApplicationPartyId,
                    AttachmentUrl = partyExtraAttachmentDto.AttachmentUrl,
                    AttachmentId = partyExtraAttachmentDto.AttachmentId,
                    AttachmentName = partyExtraAttachmentDto.AttachmentName,
                    CountryOfIssue = partyExtraAttachmentDto.CountryOfIssue,
                    ExpirationDate = partyExtraAttachmentDto.ExpirationDate,
                    Number = partyExtraAttachmentDto.Number
                };

                _IGeneralRepository.Add(partyExtraAttachment);
                if (await _IGeneralRepository.Save())
                {
                    return partyExtraAttachment.Id;
                }

            }
            catch (Exception)
            {
                return Constants.ERROR;
            }

            return Constants.ERROR;
        }

        public async Task<List<ApplicationPartyExtraAttachment>> GetAllExtraAttachment(int? ApplicationPartyId)
        {
            Task<List<ApplicationPartyExtraAttachment>> query = null;
            if (ApplicationPartyId == null)
                query = _EngineCoreDBContext.ApplicationPartyExtraAttachment.ToListAsync();
            else
                query = _EngineCoreDBContext.ApplicationPartyExtraAttachment.Where(s => s.ApplicationPartyId == ApplicationPartyId).ToListAsync();
            return await query;
        }

        public async Task<ApplicationPartyExtraAttachment> GetOneExtraAttachment(int id)
        {
            id = Convert.ToInt32(id);
            var query = _EngineCoreDBContext.ApplicationPartyExtraAttachment.Where(x => x.Id == id);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> DeleteOneExtraAttachment(int id)
        {
            ApplicationPartyExtraAttachment partyExtraAttachment = await GetOneExtraAttachment(id);
            if (partyExtraAttachment == null)
                return Constants.NOT_FOUND;
            try
            {
                _IGeneralRepository.Delete(partyExtraAttachment);
                if (await _IGeneralRepository.Save())

                    return Constants.OK;
            }
            catch (Exception)
            {
                return Constants.ERROR;
            }
            return Constants.ERROR;
        }

        public async Task<int> UpdateExtraAttachment(int id, ApplicationPartyExtraAttachmentDto applicationPartyExtraAttachment)
        {
            ApplicationPartyExtraAttachment applicationPartyExtra = await GetOneExtraAttachment(id);
            if (applicationPartyExtra == null)
                return Constants.NOT_FOUND;
            try
            {
                applicationPartyExtra.ApplicationPartyId = (int)applicationPartyExtraAttachment.ApplicationPartyId;
                applicationPartyExtra.AttachmentUrl = applicationPartyExtraAttachment.AttachmentUrl;
                applicationPartyExtra.AttachmentName = applicationPartyExtraAttachment.AttachmentName;
                applicationPartyExtra.AttachmentId = applicationPartyExtraAttachment.AttachmentId;
                applicationPartyExtra.CountryOfIssue = applicationPartyExtraAttachment.CountryOfIssue;
                applicationPartyExtra.ExpirationDate = applicationPartyExtraAttachment.ExpirationDate;
                applicationPartyExtra.Number = applicationPartyExtraAttachment.Number;

                _IGeneralRepository.Update(applicationPartyExtra);
                if (await _IGeneralRepository.Save())
                {
                    return Constants.OK;
                }
            }
            catch (Exception)
            {
                return Constants.ERROR;
            }
            return Constants.ERROR;
        }


        //-------------------------------Return UserId  OR ERROR ----------------------------     
        public async Task<APIResult> AddPartyToUser(ApplicationPartyDto applicationPartyDto, string lang)
        {
            string eId = applicationPartyDto.EmiratesIdNo;
            string userEmail = applicationPartyDto.Email;
            APIResult ApiResult = new APIResult();
            if (applicationPartyDto.EmiratesIdNo != null && applicationPartyDto.EmiratesIdNo.Length > 0)
            {
                if (await _IUserRepository.IsExist(applicationPartyDto.EmiratesIdNo))
                {
                    ApiResult.Id = _EngineCoreDBContext.User.Where(x => x.EmiratesId == applicationPartyDto.EmiratesIdNo).FirstOrDefault().Id;
                    ApiResult.Code = 200;
                    ApiResult.Result = true;
                    return ApiResult;
                }
            }

            // new Emarit id, or no Emarit id available.
            if (applicationPartyDto.Email != null && applicationPartyDto.Email.Length > 0)
            {
                var existMail = await _EngineCoreDBContext.User.Where(x => x.Email == applicationPartyDto.Email.Trim()).FirstOrDefaultAsync();
                if (existMail != null)
                {
                    if (existMail.EmiratesId.StartsWith("inv") && applicationPartyDto.EmiratesIdNo == null)
                    {
                        ApiResult.Id = existMail.Id;
                        ApiResult.Code = 200;
                        ApiResult.Result = true;
                        return ApiResult;
                    }
                    else
                    {
                        string Email = Constants.INVALID_EMAIL_PREFIX + _IGeneralRepository.GetNewValueBySec() + Constants.INVALID_EMAIL_SUFFIX;
                        userEmail = Email;
                    }
                }
            }
            else
            {
                string Email = Constants.INVALID_EMAIL_PREFIX + _IGeneralRepository.GetNewValueBySec() + Constants.INVALID_EMAIL_SUFFIX;
                userEmail = Email;
            }



            var visitorRole = await _EngineCoreDBContext.Role.Where(x => x.Name == Constants.DefaultVisitorPolicy).FirstOrDefaultAsync();
            if (visitorRole == null)
            {
                ApiResult.Result = false;
                ApiResult.Id = -1;
                ApiResult.Result = Constants.ERROR;
                ApiResult.Message.Clear();
                ApiResult.Message.Add($"{applicationPartyDto.Email} {Constants.getMessage(lang, "FailedAddedUser")}");
                return ApiResult;
            }

            List<int> newUserRoles = new List<int>
            {
                visitorRole.Id
            };

            UserResultDto a;
            //int at = applicationPartyDto.Email.IndexOf("@");

            // generate random password.
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            var rnd = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());

            // generate random Emirates Id when user has not.
            if (applicationPartyDto.EmiratesIdNo == null || applicationPartyDto.EmiratesIdNo == "")
            {
                eId = "invalid_" + new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
            }

            UserPostDto user = new UserPostDto()
            {
                UserName = userEmail,
                FullName = applicationPartyDto.FullName,
                Email = userEmail,
                UserRoles = newUserRoles,
                PhoneNumber = applicationPartyDto.Mobile,
                PasswordHash = "P@ssw0rd_" + rnd,
                EmiratesId = eId,
                BirthdayDate = applicationPartyDto.BirthDate
            };

            try
            {
                a = await _IUserRepository.CreateUser(user, null, true, lang, false);
                ApiResult.Id = a.user.Id;
                ApiResult.Code = 200;
                ApiResult.Result = true;
                return ApiResult;
            }
            catch (Exception e)
            {
                ApiResult.Result = Constants.ERROR;
                ApiResult.Result = false;
                ApiResult.Id = -1;
                ApiResult.Message.Clear();
                string message;
                if (e != null)
                {
                    message = $"{e.Message} {applicationPartyDto.Email}";
                }
                else
                {
                    message = $"{applicationPartyDto.Email} {Constants.getMessage(lang, "FailedAddedUser")}";
                }


                ApiResult.Message.Add(message);
                return ApiResult;
            }
        }

        //-------------------------------Return number of not signed parties / 0  if all party has signed ---------------------------- 
        public async Task<int> NotSignedPartyCount(int transactionId)
        {
            int count = await _EngineCoreDBContext.ApplicationParty
                              .Where(s => s.TransactionId == transactionId && (s.SignRequired == true && String.IsNullOrEmpty(s.SignUrl)))
                              .Select(a => a.PartyId)
                              .CountAsync();
            return count;
        }



        public string SavePartySignImage(string Base64String)
        {
            string target = "pfiles";
            var s = _IFilesUploaderRepository.FromBase64ToImage(Base64String, target);
            return s;

        }

        public async Task<List<ApplicationPartySignState>> PartySignState(int transactionId)
        {
            List<ApplicationPartySignState> list = await _EngineCoreDBContext.ApplicationParty
                           .Where(s => s.TransactionId == transactionId)
                           .Select(a => new ApplicationPartySignState()
                           {
                               Id = a.Id,
                               PartyId = a.PartyId,
                               IsOwner = a.IsOwner,
                               PartyTypeId = a.PartyTypeValueId,
                               FullName = a.FullName,
                               SignRequired = a.SignRequired,
                               Signed = a.Signed,
                               SignType = a.SignType,
                               SignDate = a.SignDate,
                               SignUrl = a.SignUrl,
                               EditableSign = a.EditableSign,
                               NotaryId = a.NotaryId


                           }).ToListAsync();

            return list;
        }

        public async Task<ApplicationParty> GetAppParty(int transactionId, int partyIdAsUser)
        {
            var query = _EngineCoreDBContext.ApplicationParty.Where(s => s.TransactionId == transactionId &&
            s.PartyId == partyIdAsUser).FirstOrDefaultAsync();
            return await query;
        }

        public async Task<APIResult> SwitchPartySignStatus(int id, int userId)
        {
            APIResult ApiResult = new APIResult();
            ApiResult.Id = -1;
            ApiResult.Code = 500;
            ApiResult.Message = new List<string>();
            ApiResult.Result = "";

            ApplicationParty ap = await GetOne(id);
            ApplicationPartyDto apDto = ApplicationPartyDto.GetDTO(ap);

            apDto.NotaryId = userId;

            apDto.EditableSign = apDto.EditableSign == true ? false : true;

            if ((bool)apDto.EditableSign)
            {
                apDto.Signed = false;
                apDto.SignUrl = null;
                apDto.SignType = null;
                apDto.SignDate = null;
            }
            int result = await Update(id, userId, apDto);
            if (result == Constants.OK)
            {
                ApiResult.Id = id;
                ApiResult.Code = 200;
                ApiResult.Message.Add(Constants.getMessage("ar", "sucsessUpdate"));
                ApiResult.Result = apDto.EditableSign;
            }
            else
                ApiResult.Message.Add(Constants.getMessage("ar", "faildUpdate"));
            return ApiResult;


        }

        public async Task<APIResult> SwitchPartySignRequired(int id, int userId)
        {
            APIResult ApiResult = new APIResult();
            ApiResult.Id = -1;
            ApiResult.Code = 500;
            ApiResult.Message = new List<string>();
            ApiResult.Result = "";

            ApplicationParty ap = await GetOne(id);
            if (ap == null)
            {
                ApiResult.Message.Add(Constants.getMessage("ar", "partyNotFound"));
                return ApiResult;
            }
            ApplicationPartyDto apDto = ApplicationPartyDto.GetDTO(ap);
            apDto.SignRequired = apDto.SignRequired == true ? false : true;
            int result = await Update(id, userId, apDto);
            if (result == Constants.OK)
            {
                ApiResult.Id = id;
                ApiResult.Code = 200;
                ApiResult.Result = apDto.SignRequired;
                //ApiResult.Message.Add(Constants.getMessage("ar", "sucsessUpdate"));

            }
            else
                ApiResult.Message.Add(Constants.getMessage("ar", "faildUpdate"));
            return ApiResult;
        }

        public async Task<APIResult> IsSignEditByAnotherUser(int transactionId, int userId)
        {

            
            APIResult result = new APIResult();
            List <ApplicationPartySignState> state= await PartySignState(transactionId);
            bool b = state.Any(x=>x.NotaryId!= null && x.NotaryId!= userId);
            if(b)
            {
                int s =state.Count(u => u.Signed==true);
                int a= state.Count(u => u.EditableSign == true && u.Signed==false);
                result.Result = Constants.AppStatus.LOCKED;
                result.Message.Add("تم العمل على هذا الطلب من قبل كاتب عدل آخر");
                result.Message.Add("هل تريد الاستمرار في معالجة الطلب؟");
                if (s > 0) 
                { 
                    result.Message.Add(".ملاحظة: في حال الموافقة سيتم مسح بيانات التواقيع الخاصة بأطراف المعاملة");
                    result.Message.Add($"عدد الأطراف الموقعين {s}");
                }
            }
             

           /* APIResult result = new APIResult();
            List<ApplicationPartySignState> state = await PartySignState(transactionId);
            bool b = state.Any(x => x.NotaryId != null && x.NotaryId != userId);
            if (b)
            {
                //int s =state.Count(u => u.Signed==true);
                int s = state.Count(u => !String.IsNullOrEmpty(u.SignUrl));

                if (s > 0)
                {
                    int a = state.Count(u => u.EditableSign == true && u.Signed == false);
                    result.Result = Constants.LOCKED;
                    result.Message.Add("تم العمل على هذا الطلب من قبل كاتب عدل آخر");
                    result.Message.Add("هل تريد الاستمرار في معالجة الطلب؟");
                    result.Message.Add(".ملاحظة: في حال الموافقة سيتم مسح بيانات التواقيع الخاصة بأطراف المعاملة");
                    result.Message.Add($"عدد الأطراف الموقعين {s}");
                }
            }*/

            else
            {
                result.Result = Constants.AppStatus.AVAILABLE;
                result.Id = 1;
            }
            return result;
        }


        public async Task<APIResult> ClearPartySignInfo(int partyId, int userId)
        {
            APIResult result = new APIResult();
            ApplicationParty appParty = await GetOne(partyId);
            appParty.SignDate = null;
            appParty.Signed = false;
            appParty.EditableSign = false;
            appParty.SignType = null;
            appParty.SignUrl = null;
            appParty.NotaryId = null;
            appParty.LastUpdatedBy = userId;
            appParty.LastUpdatedDate = DateTime.Now;
            _EngineCoreDBContext.Update(appParty);
            if (await _EngineCoreDBContext.SaveChangesAsync() >= 0)
            {
                result.Result = true;
                result.Id = partyId;
                result.Code = 200;
            }
            else
            {
                result.Message.Add("خطأ في مسح بيانات التوقيع");
                result.Result = false;
            }
            return result;
        }
        public async Task<APIResult> ClearPartiesSignInfo(int transactionId, int userId)
        {
            APIResult result = new APIResult();
            List<int> list = await _EngineCoreDBContext.ApplicationParty
                           .Where(s => s.TransactionId == transactionId)
                           .Select(a => a.Id).ToListAsync();

            foreach (int i in list)
            {
                result = await ClearPartySignInfo(i, userId);
                if (result.Id < 0)
                {
                    return result;
                }
            }

            result.Id = 1;
            result.Result = true;
            result.Code = 200;
            return result;
        }
    }
}
