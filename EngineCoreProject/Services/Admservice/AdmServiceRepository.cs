using EngineCoreProject.DTOs.AdmService;
using EngineCoreProject.DTOs.AdmService.ModelView;
using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using EngineCoreProject.DTOs.AramexDto;
using EngineCoreProject.DTOs.FileDto;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.AdmServices;
using EngineCoreProject.IRepository.IFilesUploader;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services
{
    public class AdmServiceRepository : IAdmServiceRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly IUserRepository _IUserRepository;
        private readonly IFilesUploaderRepositiory _IFilesUploaderRepository;
        private readonly ISysValueRepository _ISysValueRepository;
        private readonly IConfiguration _IConfiguration;
        private readonly ILogger<AdmServiceRepository> _logger;
        ValidatorException _exception;

        public AdmServiceRepository(IUserRepository iUserRepository, ISysValueRepository iSysValueRepository,
                                    EngineCoreDBContext EngineCoreDBContext, IConfiguration iConfiguration,
                                    ILogger<AdmServiceRepository> ilogger,
                                    IGeneralRepository iGeneralRepository, IFilesUploaderRepositiory filesUploaderRepositiory)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _IFilesUploaderRepository = filesUploaderRepositiory;
            _ISysValueRepository = iSysValueRepository;
            _exception = new ValidatorException();
            _IUserRepository = iUserRepository;
            _logger = ilogger;
            _IConfiguration = iConfiguration;
        }

        public async Task<List<AdmService>> GetAll()
        {
            List<AdmService> admServices = await _EngineCoreDBContext.AdmService.ToListAsync();
            /*var query = _EngineCoreDBContext.AdmService;
            return await query.ToListAsync();*/
            return admServices;
        }
        public async Task<AdmService> GetOne(int id)
        {
            AdmService admService = await _EngineCoreDBContext.AdmService.Where(x => x.Id == id).Include(x => x.TargetServiceService/*.Select(x => new { TargetServiceId = x.TargetServiceId }).ToList()*/).FirstOrDefaultAsync();
            return admService;
        }
        public async Task<ServiceNamesDto> GetOnename(int id, string lang)
        {
            id = Convert.ToInt32(id);
            /* AdmService admService = await _EngineCoreDBContext.AdmService.Where(x => x.Id == id).FirstOrDefaultAsync();//.Include(s=>s.AdmStage).FirstOrDefaultAsync();
             return admService;*/
            Task<ServiceNamesDto> query = null;
            query = (from srv in _EngineCoreDBContext.AdmService
                     join t in _EngineCoreDBContext.SysTranslation
                         on srv.Shortcut equals t.Shortcut
                     join rd in _EngineCoreDBContext.RelatedData
                         on srv.Id equals rd.ServiceId
                         into nt
                     from newTable in nt.DefaultIfEmpty()

                     where t.Lang == lang
                     where srv.Id == id

                     select new ServiceNamesDto
                     {
                         Id = srv.Id,
                         shortcut = srv.Shortcut,
                         serviceName = t.Value,
                         ServiceKind = srv.ServiceKindNo,
                         Icon = srv.Icon,
                         fee = srv.Fee,
                         orderNo = srv.Order,
                         UgId = srv.UgId,
                         createdDate = srv.CreatedDate,
                         lastupdatedDate = srv.LastUpdatedDate,
                         TargetService = srv.TargetService,
                         DefaultUser = srv.DefaultUser,
                         Templated = srv.Templated,
                         HasReason = srv.HasReason,
                         ApprovalStage = srv.ApprovalStage,
                         TemplateId = srv.TemplateId,
                         DocumentTypeId = srv.DocumentTypeId,
                         ShowApplication = newTable.ShowApplication,
                         ShowTransaction = newTable.ShowTransaction,
                         LimitedTime = srv.LimitedTime,
                         HasDocument = srv.HasDocument,
                         ExternalFileRequired = srv.ExternalFileRequired,
                         GuidFilePathAr = srv.GuidFilePathAr,
                         GuidFilePathEn = srv.GuidFilePathEn,
                         Delivery=srv.Delivery

                     }).FirstOrDefaultAsync();

            return await query;
        }

        public async Task<List<ServiceNamesDto>> GetserviceNAmes(string lang)
        {
            Task<List<ServiceNamesDto>> query = null;
            query = (from srv in _EngineCoreDBContext.AdmService
                     join t in _EngineCoreDBContext.SysTranslation
                         on srv.Shortcut equals t.Shortcut

                     orderby srv.Order

                     where srv.RecStatus != "disable"
                     where t.Lang == lang

                     select new ServiceNamesDto
                     {
                         Id = srv.Id,
                         shortcut = srv.Shortcut,
                         serviceName = t.Value,
                         UgId = srv.UgId,
                         createdDate = srv.CreatedDate,
                         lastupdatedDate = srv.LastUpdatedDate,
                         fee = srv.Fee,
                         Icon = srv.Icon,
                         orderNo = srv.Order,
                         recordStatus = srv.RecStatus,
                         HasDocument = srv.HasDocument,
                         GuidFilePathAr = srv.GuidFilePathAr,
                         GuidFilePathEn = srv.GuidFilePathEn
                     }).ToListAsync();

            return await query;
        }

        public async Task<List<servicestagesDto>> getsatgesofservice(int id, string lang)
        {
            Task<List<servicestagesDto>> query = null;
            query = (from srv in _EngineCoreDBContext.AdmService
                     join stg in _EngineCoreDBContext.AdmStage on new { id = srv.Id } equals new { id = (int)stg.ServiceId }
                     join tr in _EngineCoreDBContext.SysTranslation on stg.Shortcut equals tr.Shortcut
                     where
                       stg.ServiceId == id &&
                       tr.Lang == lang
                     orderby stg.OrderNo
                     select new servicestagesDto
                     {
                         serviceid = srv.Id,
                         stageshortcut = stg.Shortcut,
                         stageid = (int?)stg.Id,
                         stagename = tr.Value,

                     }
                ).ToListAsync();
            return await query;
        }

        public async Task<List<serviceAllLang>> GetserviceAlllang(string shortcut)
        {
            Task<List<serviceAllLang>> query = null;
            query = (from t in _EngineCoreDBContext.SysTranslation
                     where t.Shortcut == shortcut
                     select new serviceAllLang
                     {
                         shortcut = t.Shortcut,
                         lang = t.Lang,
                         tarnslate = t.Value
                     }).ToListAsync();

            return await query;
        }

        public async Task<UploadedFileMessage> Upload(IFormFile File)
        {
            UploadedFileMessage f;
            string target = "ServiceIconFolder";
            f = await _IFilesUploaderRepository.UploadFile(File, target, "images");
            return f;
        }

        public async Task<object> ChangeServiceManual(int serviceId, IFormFile File, string lang)
        {
            var admservice = await GetOne(serviceId);
            string res = "";

            if (admservice == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "serviceNotFound"));
                throw _exception;
            }

            if (_IConfiguration["ServiceManualFolder"] == null || _IFilesUploaderRepository.GetRootPath() == null)
            {
                _exception.AttributeMessages.Add(" Missed configuration for Service Manual ask the admin to fix.");
                throw _exception;
            }

            if (lang != "ar" && lang != "en")
            {
                _exception.AttributeMessages.Add(" Wrong lang! it should be en or ar.");
                throw _exception;
            }

            if (!_IFilesUploaderRepository.FolderExist(Path.Combine(_IFilesUploaderRepository.GetRootPath(), _IConfiguration["ServiceManualFolder"])))
            {
                _exception.AttributeMessages.Add(" Missing folder " + Path.Combine(_IFilesUploaderRepository.GetRootPath(), _IConfiguration["ServiceManualFolder"]));
                throw _exception;
            }

            var upload = new UploadedFileMessage();
            try
            {
                upload = await _IFilesUploaderRepository.UploadFile(File, "ServiceManualFolder", "Portable document format");
            }
            catch (Exception ex)
            {
                _exception.AttributeMessages.Add(ex.Message);
                throw _exception;
            }

            if (upload.SuccessUpload)
            {
                if (lang == "ar" && admservice.GuidFilePathAr != null && admservice.GuidFilePathAr.Length > 0)
                {
                    // remove old ar file.
                    string oldFile = Path.Combine(_IFilesUploaderRepository.GetRootPath(), _IConfiguration["ServiceManualFolder"], admservice.GuidFilePathAr);
                    if (!_IFilesUploaderRepository.RemoveFile(oldFile))
                    {
                        _logger.LogInformation(" Warning: there is unnecessary file at " + oldFile);
                    }                 
                }

                if (lang == "en" && admservice.GuidFilePathEn != null && admservice.GuidFilePathEn.Length > 0)
                {
                    // remove old en file.
                    string oldFile = Path.Combine(_IFilesUploaderRepository.GetRootPath(), _IConfiguration["ServiceManualFolder"], admservice.GuidFilePathEn);
                    if (!_IFilesUploaderRepository.RemoveFile(oldFile))
                    {
                        _logger.LogInformation(" Warning: there is unnecessary file at " + oldFile);
                    }               
                }

                try
                {
                    if (lang == "en")
                    {
                        admservice.GuidFilePathEn = Path.Combine(_IConfiguration["ServiceManualFolder"], upload.FileName);
                    }
                    else
                    {
                        admservice.GuidFilePathAr = Path.Combine(_IConfiguration["ServiceManualFolder"], upload.FileName);
                    }

                    res = Path.Combine(_IConfiguration["ServiceManualFolder"], upload.FileName);
                    _EngineCoreDBContext.AdmService.Update(admservice);
                    await _EngineCoreDBContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _exception.AttributeMessages.Add(ex.Message);
                    throw _exception;
                }
            }
            else
            {
                _exception.AttributeMessages.Add(upload.Message);
                throw _exception;
            }
            var final = new
            {

                result = res,
               

            };

         

            return final;
        }


        public async Task<AdmService> add()
        {
            AdmService admService = new AdmService()

            {

                Shortcut = _iGeneralRepository.GenerateShortCut("service", "name_shortcut"),    //_IGeneralRepository.gernatre()
                CreatedDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now,

            };

            _iGeneralRepository.Add(admService);


            if (await _iGeneralRepository.Save())
            {

                return admService;
            }
            return admService;
        }

        public async Task<int> delete(int id, string lang)
        {
            AdmService admservice = await GetOne(id);
            if (admservice == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "serviceNotFound"));
                throw _exception;
            }
            AdmStage admStage = _EngineCoreDBContext.AdmStage.Where(x => x.ServiceId == id).FirstOrDefault();
            if (admStage != null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "servicesStage"));
                throw _exception;
            }
            // admservice.RecStatus = "disable";


            _iGeneralRepository.Delete(admservice);
            if (await _iGeneralRepository.Save())
            {
                return Constants.OK;
            }
            return Constants.ERROR;
        }

        public async Task<int> GetKindNo(int id)
        {
            return (int)await _EngineCoreDBContext.AdmService.Where(x => x.Id == id).Select(x => x.ServiceKindNo).FirstOrDefaultAsync();
        }
        //-----------------by yhab-----------------------------------
        public async Task<string> GetApprovalText(int id)
        {
            string text = await _EngineCoreDBContext.AdmService.Where(x => x.Id == id).Select(x => x.ApprovalText).FirstOrDefaultAsync();

            return text;
        }
        //-----------------by yhab-----------------------------------
        public async Task<List<int>> GetStagesIds(int id)
        {
            return await _EngineCoreDBContext.AdmStage.Where(x => x.ServiceId == id).OrderBy(x => x.OrderNo).Select(x => x.Id).ToListAsync();
        }

        public async Task<List<RelatedContentView>> GetRelatedContents(int serviceId, string lang)
        {
            Task<List<RelatedContentView>> query = null;
            query = (from sv in _EngineCoreDBContext.AdmService
                     join rc in _EngineCoreDBContext.RelatedContent
                     on sv.Id equals rc.ServiceId
                     join tr in _EngineCoreDBContext.SysTranslation
                         on rc.TitleShortcut equals tr.Shortcut

                     where sv.Id == serviceId
                     where tr.Lang == lang
                     select new RelatedContentView
                     {

                         TitleSortcut = tr.Shortcut,
                         Title = tr.Value,
                         Content = rc.Content,

                     }).ToListAsync();

            return await query;
        }



        public async Task<List<int>> GetReviewStagesId()
        {
            int stageId;
            List<int> reviewStagesId = new List<int>();
            List<int> servicesIDs = await _EngineCoreDBContext.AdmService.Select(x => x.Id).ToListAsync();
            foreach (int sId in servicesIDs)
            {
                stageId = await FirstStage(sId, 2);
                if (stageId > 0)
                {
                    reviewStagesId.Add(stageId);
                }
            }
            return reviewStagesId;
        }

        public async Task<List<int>> GetInterviewStagesId()
        {
            List<int> interviewStagesId = new List<int>();
            List<int?> approvalStageIDs = await _EngineCoreDBContext.AdmService.Select(x => x.ApprovalStage).ToListAsync();
            foreach (int? sId in approvalStageIDs)
            {

                if (sId != null)
                {
                    interviewStagesId.Add((int)sId);
                }
            }
            return interviewStagesId;
        }


        //-------------------------Get first stage ID of a service | Return 0 when fail OR Not found---------------------------------------------------------------------
        public async Task<int> FirstStage(int? serviceId, int stageOrder = 1)
        {
            // Task<StageOfService> query = null;
            Task<List<StageOfService>> query = null;
            if (serviceId > 0)
            {
                query = (
                  from stg in _EngineCoreDBContext.AdmStage
                  where stg.ServiceId == serviceId
                  orderby stg.OrderNo
                  select new StageOfService
                  {
                      Id = stg.Id,
                      Name = "",
                      IsCurrent = false
                  }).ToListAsync();   //FirstOrDefaultAsync();
                try
                {
                    var list = await query;
                    if (list != null)
                        return (int)list[stageOrder].Id;
                    else return 0;
                }
                catch
                {
                    return 0;
                }
            }
            else return 0;

        }

        public async Task<int> DeleteAppById(int AppId)
        {
            Application application = _EngineCoreDBContext.Application.Where(x => x.Id == AppId).FirstOrDefault();
            if (application == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage("en", "APPNF"));
                throw _exception;
            }

            int ApplicationId = _EngineCoreDBContext.Application.Where(x => x.Id == AppId).Select(y => y.Id).FirstOrDefault();
            int TransactionId = _EngineCoreDBContext.AppTransaction.Where(x => x.ApplicationId == AppId).Select(y => y.Id).FirstOrDefault();
            int MeetingId = _EngineCoreDBContext.Meeting.Where(x => x.OrderNo == AppId.ToString()).Select(y => y.Id).FirstOrDefault();
            int PaymentId = _EngineCoreDBContext.Payment.Where(x => x.ApplicationId == AppId).Select(y => y.Id).FirstOrDefault();

            TargetApplication targetApplication = _EngineCoreDBContext.TargetApplication.Where(x => x.AppId == AppId).FirstOrDefault();
            if (targetApplication != null)
            {
                _iGeneralRepository.Delete(targetApplication);
                await _iGeneralRepository.Save();
            }
            TargetApplication targetApplication2 = _EngineCoreDBContext.TargetApplication.Where(x => x.TargetAppId == AppId).FirstOrDefault();
            if (targetApplication != null)
            {
                _iGeneralRepository.Delete(targetApplication2);
                await _iGeneralRepository.Save();
            }

            QueueProcesses queueProcesses = _EngineCoreDBContext.QueueProcesses.Where(x => x.ProcessNo == ApplicationId.ToString()).FirstOrDefault();
            if (queueProcesses != null)
            {
                _iGeneralRepository.Delete(queueProcesses);
                await _iGeneralRepository.Save();
            }

            List<TransactionOldVersion> transactionOldVersions = _EngineCoreDBContext.TransactionOldVersion.Where(x => x.TransactionId == TransactionId).ToList();
            _EngineCoreDBContext.TransactionOldVersion.RemoveRange(transactionOldVersions);
            await _iGeneralRepository.Save();

            List<ApplicationParty> applicationParties = _EngineCoreDBContext.ApplicationParty.Where(x => x.TransactionId == TransactionId).ToList();
            _EngineCoreDBContext.ApplicationParty.RemoveRange(applicationParties);
            await _iGeneralRepository.Save();

            List<MeetingLogging> meetings = _EngineCoreDBContext.MeetingLogging.Where(x => x.MeetingId == MeetingId).ToList();
            _EngineCoreDBContext.MeetingLogging.RemoveRange(meetings);
            await _iGeneralRepository.Save();


            List<Calendar> calendars = _EngineCoreDBContext.Calendar.Where(x => x.MeetingId == MeetingId).ToList();
            _EngineCoreDBContext.Calendar.RemoveRange(calendars);
            await _iGeneralRepository.Save();

            Meeting meeting = _EngineCoreDBContext.Meeting.Where(x => x.OrderNo == AppId.ToString()).FirstOrDefault();
            if (meeting != null)
            {
                _iGeneralRepository.Delete(meeting);
                await _iGeneralRepository.Save();
            }

            List<PaymentDetails> paymentDetails = _EngineCoreDBContext.PaymentDetails.Where(x => x.PaymentId == PaymentId).ToList();
            _EngineCoreDBContext.PaymentDetails.RemoveRange(paymentDetails);
            await _iGeneralRepository.Save();

            Models.Payment payment = _EngineCoreDBContext.Payment.Where(x => x.ApplicationId == AppId).FirstOrDefault();
            if (payment != null)
            {
                _iGeneralRepository.Delete(payment);
                await _iGeneralRepository.Save();
            }

            _iGeneralRepository.Delete(application);
            if (await _iGeneralRepository.Save())

                return Constants.OK;
            else
                return Constants.ERROR;




        }

        public async Task<List<int>> GetDoneStagesId()
        {
            int DoneStageTypeId = await _ISysValueRepository.GetIdByShortcut("SysLookupValue_Shortcut677");
            int stageId;
            List<int> doneStagesId = new List<int>();
            doneStagesId = await _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId == DoneStageTypeId).Select(z => z.Id).ToListAsync();
            return doneStagesId;
        }

        public string LastNotary(int AppId)
        {



            string NotaryName = "";
            List<int> employees = new List<int>(_IUserRepository.GetEmployees().Result.Keys);
            var AppTrack = _EngineCoreDBContext.ApplicationTrack.Where(x => x.ApplicationId == AppId
         && employees.Contains((int)x.UserId)).OrderByDescending(z => z.CreatedDate).FirstOrDefault();

            if (AppTrack == null)
                NotaryName = "";
            else
            {
                int Notaryid = (int)AppTrack.UserId;
                NotaryName = _EngineCoreDBContext.User.Where(x => x.Id == Notaryid).Select(z => z.FullName).FirstOrDefault();

            }

            return NotaryName;

        }

        public async Task<bool> AddAramexRequest(AramexPostDto aramexPostDto)
        {

            AramexRequests aramexRequests = new AramexRequests
            {
                ApplicationId = aramexPostDto.ApplicationId,
                StateId = aramexPostDto.StateId,
                CreatedDate = DateTime.Now,
                OwnerName = aramexPostDto.OwnerName,
                Mobile = aramexPostDto.Mobile,
                Email = aramexPostDto.Email
            };

            await _EngineCoreDBContext.AramexRequests.AddAsync(aramexRequests);
            if (await _EngineCoreDBContext.SaveChangesAsync() >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
