using DinkToPdf.Contracts;
using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.AccountDto;
using EngineCoreProject.DTOs.AdmService;
using EngineCoreProject.DTOs.AdmService.ModelView;
using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.ApplicationDtos.Action;
using EngineCoreProject.DTOs.ApplicationDtos.IdDtos;
using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using EngineCoreProject.DTOs.ApplicationDtos.RelatedContent;
using EngineCoreProject.DTOs.BlockChainDto;
using EngineCoreProject.DTOs.AramexDto;
using EngineCoreProject.DTOs.CalendarDto;
using EngineCoreProject.DTOs.ConfigureWritableDto;
using EngineCoreProject.DTOs.FeesDto;
using EngineCoreProject.DTOs.FileDto;
using EngineCoreProject.DTOs.MeetingDto;
using EngineCoreProject.DTOs.MyApplicationDto;
using EngineCoreProject.DTOs.NotificationDtos;
using EngineCoreProject.DTOs.Payment;
using EngineCoreProject.DTOs.PDFGenerator;
using EngineCoreProject.DTOs.QueueDto;
using EngineCoreProject.DTOs.TemplateSetDtos;
using EngineCoreProject.DTOs.TemplateSetDtos.ModelView;
using EngineCoreProject.DTOs.TransactionFeeDto;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.AdmServices;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.IRepository.IAramex;
using EngineCoreProject.IRepository.ICalendarRepository;
using EngineCoreProject.IRepository.IFilesUploader;
using EngineCoreProject.IRepository.IGeneralSetting;
using EngineCoreProject.IRepository.IMeetingRepository;
using EngineCoreProject.IRepository.INotificationSettingRepository;
using EngineCoreProject.IRepository.IPaymentRepository;
using EngineCoreProject.IRepository.IQueueRepository;
using EngineCoreProject.IRepository.ITransactionFeeRepository;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.IRepository.TemplateSetRepository;
using EngineCoreProject.Models;
using EngineCoreProject.Services.GeneratorServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using static EngineCoreProject.Services.Constants;

namespace EngineCoreProject.Services.ApplicationSet
{
    public class ApplicationRepositiory : IApplicationRepository
    {
        
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _IGeneralRepository;
        private readonly IApplicationAttachmentRepository _IApplicationAttachmentRepository;
        private readonly IApplicationPartyRepository _IApplicationPartyRepository;
        private readonly IApplicationTrackRepository _IApplicationTrackRepository;
        private readonly ITransactionRepository _ITransactionRepository;
        private readonly ISysValueRepository _ISysValueRepository;
        private readonly IQueueRepository _IQueueRepository;
        private readonly IMeetingRepository _IMeetingRepository;
        private readonly IServiceKindRepository _IServiceKindRepository;
        private readonly ICalendarRepository _ICalendarRepository;
        private readonly ISendNotificationRepository _ISendNotificationRepository;
        private readonly INotificationSettingRepository _INotificationSettingRepository;
        private readonly ITemplateRepository _ITemplateRepository;
        private readonly IAdmServiceRepository _IAdmServiceRepository;
        private readonly IUserRepository _IUserRepository;
        private readonly IPaymentRepository _IPaymentRepository;
        private readonly IConfiguration _IConfiguration;
        private readonly IFilesUploaderRepositiory _IFilesUploaderRepository;
        private readonly IAdmStageRepository _IAdmStageRepository;
        private readonly ITransactionFeeRepository _ITransactionFeeRepository;
        private readonly IOptions<FileNaming> _FileNaming;
        private readonly IOptions<Pdfdocumentsetting> _Pdfdocumentsetting;
        private readonly IConverter _IConverter;
        private readonly ILogger<ApplicationRepositiory> _logger;
        private readonly IWorkingTimeRepository _IWorkingTimeRepository;
        private readonly IBlockChain _IBlockChain;

        ValidatorException _exception;





        public ApplicationRepositiory(IBlockChain iBlockChain,IWorkingTimeRepository iWorkingTimeRepository, EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository
            , ISysValueRepository iSysValueRepository, IApplicationAttachmentRepository iApplicationRepositiory,

            IApplicationPartyRepository iApplicationPartyRepository, ITransactionRepository iTransactionRepository,
            IApplicationTrackRepository iApplicationTrackRepository, IQueueRepository iQueueRepository, IMeetingRepository iMeetingRepository, IServiceKindRepository iServiceKindRepository, ICalendarRepository iCalendarRepository
            , ISendNotificationRepository iSendNotificationRepository, INotificationSettingRepository iNotificationSettingRepository, ITemplateRepository iTemplateRepository
            , IAdmServiceRepository iAdmServiceRepository, IUserRepository iUserRepository, IPaymentRepository iPaymentRepository, IConfiguration iconfiguration, IFilesUploaderRepositiory iFilesUploaderRepository, IAdmStageRepository iAdmStageRepository,
            ITransactionFeeRepository iTransactionFeeRepository, IOptions<FileNaming> iFileNaming, IConverter iConverter,  IOptions<Pdfdocumentsetting> pDFDocumentSetting, ILogger<ApplicationRepositiory> logger)

        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _IGeneralRepository = iGeneralRepository;
            _ISysValueRepository = iSysValueRepository;
            _IApplicationAttachmentRepository = iApplicationRepositiory;
            _IApplicationPartyRepository = iApplicationPartyRepository;
            _ITransactionRepository = iTransactionRepository;
            _IApplicationTrackRepository = iApplicationTrackRepository;
            _IQueueRepository = iQueueRepository;
            _IMeetingRepository = iMeetingRepository;
            _IServiceKindRepository = iServiceKindRepository;
            _ICalendarRepository = iCalendarRepository;
            _ISendNotificationRepository = iSendNotificationRepository;
            _INotificationSettingRepository = iNotificationSettingRepository;
            _ITemplateRepository = iTemplateRepository;
            _IAdmServiceRepository = iAdmServiceRepository;
            _IUserRepository = iUserRepository;
            _IPaymentRepository = iPaymentRepository;
            _IConfiguration = iconfiguration;
            _IFilesUploaderRepository = iFilesUploaderRepository;
            _IAdmStageRepository = iAdmStageRepository;
            _ITransactionFeeRepository = iTransactionFeeRepository;
            _Pdfdocumentsetting = pDFDocumentSetting;
            _IWorkingTimeRepository = iWorkingTimeRepository;
            _IBlockChain = iBlockChain;
            _FileNaming = iFileNaming;
            _IConverter = iConverter;
            _logger = logger;
            _exception = new ValidatorException();

        }

        public ApplicationRepositiory(IApplicationPartyRepository iApplicationPartyRepository, ILogger<ApplicationRepositiory> logger,
                                       ITransactionRepository iTransactionRepository)
        {
            _IApplicationPartyRepository = iApplicationPartyRepository;
            _ITransactionRepository = iTransactionRepository;
            _logger = logger;
            _exception = new ValidatorException();
        }

        public ApplicationRepositiory(EngineCoreDBContext EngineCoreDBContext, ILogger<ApplicationRepositiory> logger)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _logger = logger;
            _exception = new ValidatorException();
        }
       
        // constructor for payment business.
        public ApplicationRepositiory(EngineCoreDBContext EngineCoreDBContext, IBlockChain iBlockChain, IGeneralRepository iGeneralRepository, INotificationSettingRepository iNotificationSettingRepository, IFilesUploaderRepositiory iFilesUploaderRepository,
                                      IConfiguration iConfiguration, ITransactionRepository iTransactionRepository, IApplicationPartyRepository iApplicationPartyRepository, IUserRepository iUserRepository,
                                      ISendNotificationRepository iSendNotificationRepository, IAdmServiceRepository iAdmServiceRepository, IPaymentRepository iPaymentRepository,
                                      ISysValueRepository iSysValueRepository, IApplicationTrackRepository iApplicationTrackRepository, ITemplateRepository iTemplateRepository, ILogger<ApplicationRepositiory> logger)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _IGeneralRepository = iGeneralRepository;
            _INotificationSettingRepository = iNotificationSettingRepository;
            _IFilesUploaderRepository = iFilesUploaderRepository;
            _IConfiguration = iConfiguration;
            _ITransactionRepository = iTransactionRepository;
            _IApplicationPartyRepository = iApplicationPartyRepository;
            _IUserRepository = iUserRepository;
            _ISendNotificationRepository = iSendNotificationRepository;
            _IAdmServiceRepository = iAdmServiceRepository;
            _ISysValueRepository = iSysValueRepository;
            _IApplicationTrackRepository = iApplicationTrackRepository;
            _ITemplateRepository = iTemplateRepository;
            _IBlockChain = iBlockChain;
            _IPaymentRepository = iPaymentRepository;
            _logger = logger;
        }

        public Task<List<int>> DeleteMany(int[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<int> DeleteOne(int id)
        {
            Application application = await GetOne(id);
            if (application == null)
                return Constants.NOT_FOUND;
            try
            {
                _IGeneralRepository.Delete(application);
                if (await _IGeneralRepository.Save())
                    return Constants.OK;
            }
            catch (Exception)
            {
                return Constants.ERROR;
            }
            return Constants.ERROR;
        }

        public async Task<List<Application>> GetAll(int? serviceId)
        {
            Task<List<Application>> query = null;
            if (serviceId == null)

                query = _EngineCoreDBContext.Application.ToListAsync();
            else
                query = _EngineCoreDBContext.Application.Where(s => s.ServiceId == serviceId).ToListAsync();
            return await query;
        }

        public async Task<Application> GetOne(int id)
        {
            var query = _EngineCoreDBContext.Application.Where(x => x.Id == id)
                                                     .Include(y => y.AppTransaction)
                                                     .Include(z => z.TargetApplicationApp);
            // .Include(z => z.ApplicationTrack);                                              
            return await query.FirstOrDefaultAsync();
        }



        public async Task<int> Add(ApplicationDto appDto, int? addBy)
        {
            int DraftStageId;
            if (appDto.CurrentStageId < 1 || appDto.CurrentStageId == null)
                DraftStageId = await _IAdmServiceRepository.FirstStage(appDto.ServiceId);
            else
                DraftStageId = (int)appDto.CurrentStageId;

            if (DraftStageId > 0)
                try
                {
                    Application application = new Application
                    {
                        ServiceId = appDto.ServiceId,
                        ApplicationNo = appDto.ApplicationNo,
                        TemplateId = appDto.TemplateId,
                        CurrentStageId = DraftStageId,
                        StateId = appDto.StateId,
                        Note = appDto.Note,
                        AppLastUpdateDate = appDto.AppLastUpdateDate,
                        LastUpdatedDate = DateTime.Now,
                        ApplicationDate = appDto.ApplicationDate,
                        Channel = appDto.Channel,
                        Reason = appDto.Reason,
                        CreatedBy = addBy,
                        LastUpdatedBy = addBy,
                        Delivery = appDto.Delivery,
                    };

                    _IGeneralRepository.Add(application);
                    if (await _IGeneralRepository.Save())
                    {
                        return application.Id;
                    }
                }
                catch (Exception e)
                {
                    var a = e.Message;
                    var d = e.InnerException;
                    return Constants.ERROR;
                }

            return Constants.ERROR;
        }

        public async Task<int> Add(TargetApplicationDto TargetAppDto)
        {
            try
            {
                TargetApplication targetApplication = new TargetApplication
                {
                    AppId = (int)TargetAppDto.AppId,
                    TargetAppId = TargetAppDto.TargetAppId,
                    TargetAppDesc = TargetAppDto.TargetAppDesc,
                    TargetAppDocument = TargetAppDto.TargetAppDocument,
                };

                _IGeneralRepository.Add(targetApplication);
                if (await _IGeneralRepository.Save())
                {
                    return targetApplication.Id;
                }
            }
            catch (Exception e)
            {
                return Constants.ERROR;
            }
            return Constants.ERROR;
        }

        public async Task<int> Update(int id, TargetApplicationDto TargetAppDto)
        {
            TargetApplication targetApplication = await GetOneTargetApplication(id);
            if (targetApplication == null)
                return Constants.NOT_FOUND;
            try
            {
                targetApplication.TargetAppId = TargetAppDto.TargetAppId;
                targetApplication.TargetAppDesc = TargetAppDto.TargetAppDesc;
                targetApplication.TargetAppDocument = TargetAppDto.TargetAppDocument;
                _IGeneralRepository.Update(targetApplication);
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

        public async Task<int> Add(AppRelatedContentDto appRContentDto)
        {
            try
            {
                var r = await _EngineCoreDBContext.AppRelatedContent.Where(x => x.TitleShortcut == appRContentDto.TitleShortcut && x.AppId == appRContentDto.AppId).FirstOrDefaultAsync();
                if (r != null)
                {
                    int id = r.Id;
                    return id;
                }
                AppRelatedContent appRelatedContent = new AppRelatedContent
                {
                    AppId = appRContentDto.AppId,
                    TitleShortcut = appRContentDto.TitleShortcut,
                    Content = appRContentDto.Content,
                    ContentUrl = appRContentDto.ContentUrl
                };

                _IGeneralRepository.Add(appRelatedContent);
                if (await _IGeneralRepository.Save())
                {
                    return appRelatedContent.Id;
                }
            }
            catch (Exception)
            {
                return ERROR;
            }
            return ERROR;
        }


        //------ *** Used in PostAllApplicationData_ *** ------Add new application  | Return appId  OR -500 when fail ----------------------------------------------------------
        public async Task<int> AddToStage(ApplicationDto applicationDto, int stageOrder, int userId)
        {
            int StageId;
            string[] states = new string[] { FORSEND, ONPROGRESS };

            StageId = await _IAdmServiceRepository.FirstStage(applicationDto.ServiceId, stageOrder);
            if (StageId > 0)
                try
                {
                    applicationDto.AppLastUpdateDate = DateTime.Now;
                    applicationDto.CurrentStageId = StageId;
                    applicationDto.StateId = await _ISysValueRepository.GetIdByShortcut(states[stageOrder - 1]);
                    applicationDto.ApplicationNo = "";

                    DateTime? appDate;
                    if (stageOrder == Convert.ToInt32(FIERST_SAVE_STAGE.REVIEW))
                        appDate = DateTime.Now;
                    else
                        appDate = null;
                    applicationDto.ApplicationDate = appDate;
                    int appId = await Add(applicationDto, userId);
                    return appId;
                }
                catch (Exception e)
                {
                    var d = e.Message.ToString();
                    return Constants.ERROR;
                }

            return Constants.ERROR;
        }
        //-----------------------------------------------------------------------------------------
        public async Task<APIResult> Update(int id, int userId, ApplicationDto applicationDto)
        {
            Application application = await GetOne(id);
            APIResult ApiResult = new APIResult();

            // _EngineCoreDBContext.Entry(application).OriginalValues["RowVersion"] = applicationDto.RowVersion;
            // _EngineCoreDBContext.Entry(application).OriginalValues["CurrentStageId"] = applicationDto.CurrentStageId;

            if (application == null)
                return null;
            try
            {
                application.ServiceId = applicationDto.ServiceId;
                application.ApplicationNo = applicationDto.ApplicationNo;
                application.TemplateId = applicationDto.TemplateId;
                application.CurrentStageId = applicationDto.CurrentStageId;
                application.StateId = applicationDto.StateId;
                application.Note = applicationDto.Note;
                application.AppLastUpdateDate = applicationDto.AppLastUpdateDate;
                application.LastUpdatedBy = userId > 0 ? userId : application.LastUpdatedBy;
                application.ApplicationDate = applicationDto.ApplicationDate;
                application.LastUpdatedDate = DateTime.Now;
                application.Reason = applicationDto.Reason;
                //application.Channel = applicationDto.Channel;
                application.Locked = applicationDto.Locked;
                application.LastReader = applicationDto.LastReader;
                application.LastReadDate = applicationDto.LastReadDate;
                application.AppLastUpdateDate = applicationDto.AppLastUpdateDate;
                application.Delivery = applicationDto.Delivery;
                _EngineCoreDBContext.Application.Update(application);
                if (await _EngineCoreDBContext.SaveChangesAsync() >= 0)
                {
                    ApiResult.Id = id;
                    ApiResult.Result = application.RowVersion;
                    ApiResult.Code = Math.Abs(OK);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ApiResult.Message.Add(getMessage("ar", "ConcurrencyError"));

                var msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += " " + ex.InnerException.Message;
                }
                _logger.LogInformation(" Error in Updating the application record for application id  " + id.ToString() + " Error is " +  msg);
            }
            catch (Exception e)
            {
                ApiResult.Message.Add(getMessage("ar", "faildUpdate"));

                var msg = e.Message;
                if (e.InnerException != null)
                {
                    msg += " " + e.InnerException.Message;
                }
                _logger.LogInformation(" Error in Updating the application record for application id  " + id.ToString() + " Error is " + msg);
            }
            return ApiResult;
        }
        public ApplicationWIdDto FromObjectToDto(Application application)
        {
            ApplicationWIdDto appWIdDto = new ApplicationWIdDto();
            appWIdDto.Id = application.Id;
            appWIdDto.ServiceId = application.ServiceId;
            appWIdDto.ApplicationNo = application.ApplicationNo;
            appWIdDto.TemplateId = application.TemplateId;
            appWIdDto.AppLastUpdateDate = application.AppLastUpdateDate;
            appWIdDto.CurrentStageId = application.CurrentStageId;
            appWIdDto.RowVersion = application.RowVersion;
            appWIdDto.StateId = application.StateId;
            appWIdDto.Note = application.Note;
            appWIdDto.Locked = application.Locked;
            appWIdDto.LastReader = application.LastReader;
            appWIdDto.LastReadDate = application.LastReadDate;
            appWIdDto.Channel = application.Channel;
            appWIdDto.ApplicationDate = application.ApplicationDate;
            appWIdDto.Note = application.Note;
            appWIdDto.Reason = application.Reason;
            appWIdDto.Delivery = application.Delivery;

            //--------------------------------

            /* appWIdDto.LastUpdatedBy = application.LastUpdatedBy
             application.Reason = applicationDto.Reason;
             application.Channel = applicationDto.Channel;
             application.Locked = applicationDto.LOCKED;
             application.LastReader = applicationDto.LastReader;
             application.LastReadDate = applicationDto.LastReadDate;
             application.AppLastUpdateDate = applicationDto.AppLastUpdateDate;
             application.ContractValue = applicationDto.ContractValue;*/


            return appWIdDto;
        }
        public async Task<List<StageOfService>> getStageOfService(int appId,/*int first_stage,*/int serviceId, string lang)
        {
            //int id;
            // if (appId==0) id=await _IApplicationRepositiory.FirstStage(serviceId);
            Task<List<StageOfService>> query1_ = null;
            //  List<StageOfService>  query0 =null;
            // if (appId > 0)
            var query0 = ((from app in _EngineCoreDBContext.Application
                           where app.Id == appId
                           select new StageOfService
                           {
                               Id = (int)app.CurrentStageId

                           }));
            /*  else
              {
                  id = await FirstStage(serviceId);
                  query0.Add(

                              new StageOfService
                              {
                                  Id = id
                              });
              }*/

            var query1 = (
                          from stg in _EngineCoreDBContext.AdmStage
                          join t in _EngineCoreDBContext.SysTranslation
                                 on stg.Shortcut equals t.Shortcut
                          join q0 in query0
                          on stg.Id equals q0.Id
                          into result
                          from newTable in result.DefaultIfEmpty()

                          where t.Lang == lang
                          where stg.ServiceId == serviceId
                          orderby stg.OrderNo
                          select new StageOfService
                          {
                              Id = stg.Id,
                              StageTypeId = (int)stg.StageTypeId,
                              Order = (int)stg.OrderNo,
                              Name = t.Value,
                              IsCurrent = newTable.Id != null, //? true : false
                              Icon = stg.Icon
                          });




            query1_ = query1.ToListAsync();
            return await query1_;

            //-------------------------no---------------------------------
            /*Task<List<StageOfService>> query = null;
          query =                               
                (from stg in _EngineCoreDBContext.AdmStage
                  join t in _EngineCoreDBContext.SysTranslation
                    on stg.Shortcut equals t.Shortcut
                     join app in _EngineCoreDBContext.Application
                       on stg.Id equals app.CurrentStageId
                         into result
                          from newTable in result.DefaultIfEmpty()


                 where t.Lang == lang
                where stg.ServiceId == serviceId
             //   where  newTable.Id == 8
                 orderby stg.OrderNo 
                select new StageOfService
                       {   
                           Id = stg.Id,
                           Name = t.Value,
                           IsCurrent= newTable.CurrentStageId != null ? true : false
                       }).ToListAsync();

              return await query;*/
        }

        //----------------------- Action Of Stage----------------------------------------------------------
        public async Task<List<ActionOfStage>> getActions_(int stageId, string lang)
        {
            Task<List<ActionOfStage>> query = null;
            query =
                  (from sa in _EngineCoreDBContext.AdmStageAction
                   join lv in _EngineCoreDBContext.SysLookupValue
                   on sa.ActionId equals lv.Id
                   join t in _EngineCoreDBContext.SysTranslation
                     on lv.Shortcut equals t.Shortcut


                   where t.Lang == lang
                   where sa.StageId == stageId

                   select new ActionOfStage
                   {
                       Id = sa.Id,
                       StageId = stageId,
                       ActionId = (int)sa.ActionId,
                       ActionName = t.Value
                   }).ToListAsync();

            return await query;
        }

        //------------------------------------------------------------------------------------------------

        public async Task<List<ActionOfStage>> getActions(int stageId, string lang)
        {
            int ButtonTypeId = await _ISysValueRepository.GetIdByShortcut("BUTTON");
            Task<List<ActionOfStage>> query = null;

            query = (from ac in _EngineCoreDBContext.AdmAction
                     join
                     sa in _EngineCoreDBContext.AdmStageAction
                     on ac.Id equals sa.ActionId
                     join t in _EngineCoreDBContext.SysTranslation
                     on ac.Shortcut equals t.Shortcut


                     where t.Lang == lang
                     where sa.StageId == stageId
                     where ac.ActionTypeId == ButtonTypeId
                     orderby sa.ShowOrder
                     select new ActionOfStage
                     {
                         Id = sa.Id,
                         StageId = stageId,
                         ActionId = (int)sa.ActionId,
                         ActionName = t.Value,
                         Order = sa.ShowOrder,
                         Enabled = sa.Enabled,
                         Group = sa.Group,
                         executions = (ICollection<SysExecution>)ac.SysExecution.OrderBy(x => x.ExecutionOrder)
                         .Select(c => new SysExecution
                         {
                             Id = c.Id,
                             ExecutionOrder = c.ExecutionOrder,
                             ToExecute = c.ToExecute,
                             Parameter1 = c.ToExecute.StartsWith("T-") ? _ISysValueRepository.GetValueByShortcut(c.Parameter1, lang).Result : c.Parameter1,//c.ToExecute=="ALERT"?  _ISysValueRepository.GetValueByShortcut(c.ToExecute,lang) :
                             Parameter2 = c.Parameter2,
                             Method = c.Method

                         })


                     }).ToListAsync();

            return await query;
        }

        //-----------------------------------------------------------------------------------------------
        public async Task<List<ActionOfStage>> getActionsByUserRole(int stageId, int userId, string lang)
        {
            int ButtonTypeId = await _ISysValueRepository.GetIdByShortcut("BUTTON");
            Task<List<ActionOfStage>> query = null;

            query = (from ac in _EngineCoreDBContext.AdmAction
                     join
                     sa in _EngineCoreDBContext.AdmStageAction
                     on ac.Id equals sa.ActionId
                     join t in _EngineCoreDBContext.SysTranslation
                     on ac.Shortcut equals t.Shortcut


                     where t.Lang == lang
                     where sa.StageId == stageId
                     where ac.ActionTypeId == ButtonTypeId
                     orderby sa.ShowOrder
                     select new ActionOfStage
                     {
                         Id = sa.Id,
                         StageId = stageId,
                         ActionId = (int)sa.ActionId,
                         ActionName = t.Value,
                         Order = sa.ShowOrder,
                         Enabled = sa.Enabled,
                         Group = sa.Group,
                         executions = (ICollection<SysExecution>)ac.SysExecution.OrderBy(x => x.ExecutionOrder)
                         .Select(c => new SysExecution
                         {
                             Id = c.Id,
                             ExecutionOrder = c.ExecutionOrder,
                             ToExecute = c.ToExecute,
                             Parameter1 = c.ToExecute.StartsWith("T-") ? _ISysValueRepository.GetValueByShortcut(c.Parameter1, lang).Result : c.Parameter1,//c.ToExecute=="ALERT"?  _ISysValueRepository.GetValueByShortcut(c.ToExecute,lang) :
                             Parameter2 = c.Parameter2,
                             Method = c.Method

                         })


                     }).ToListAsync();

            return await query;
        }

        //-----------------------------------------------------------------------------------------------
        public async Task<string> getAppState(int? stateId, string lang)
        {

            if (stateId == null)
                return "";
            Task<string> query = null;
            query = (from lv in _EngineCoreDBContext.SysLookupValue
                     join t in _EngineCoreDBContext.SysTranslation
                     on lv.Shortcut equals t.Shortcut


                     where t.Lang == lang
                     where lv.Id == stateId
                     select t.Value
                 ).FirstOrDefaultAsync();

            return await query;


        }

        //------------------------Next Previews Stage-------Return 0 when fail-----------------------------------------------
        public async Task<int> NextPreviewsStage(int serviceId, int currnentStage, int step)
        {
            StageOfService next;
            Task<List<StageOfService>> query = null;
            query = (

               from stg in _EngineCoreDBContext.AdmStage
               where stg.ServiceId == serviceId
               orderby stg.OrderNo
               select new StageOfService
               {
                   Id = stg.Id,
                   Name = "",
                   IsCurrent = false
               }).ToListAsync();

            var list = await query;
            if (step == 1)
                try
                {
                    next = list.SkipWhile(i => i.Id != currnentStage).Skip(1).First();
                    return (int)next.Id;
                }
                catch
                {
                    return 0;
                }
            else
                try
                {
                    next = list.TakeWhile(i => i.Id != currnentStage).LastOrDefault();
                    return (int)next.Id;
                }
                catch
                {
                    return 0;
                }

        }

        //------------------------- Get a stage and next  -------------------------------------------------------------
        public async Task<List<int>> StageAndNext(int? serviceId, int stageOrder, int shift)
        {

            List<int> stages = new List<int>();
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
                  }).ToListAsync();


                try
                {
                    var list = await query;
                    if (list != null)
                    {
                        stages.Add(list[stageOrder - 1].Id);
                        if (stageOrder + shift <= list.Count)
                            stages.Add(list[stageOrder + shift - 1].Id);
                        else
                            stages.Add(0);
                    }
                    return stages;
                }
                catch
                {
                    return stages;
                }
            }
            else
                return stages;

        }



        public async Task<List<ApplicationByStageName>> IncomingApplications__(string lang)
        {
            Task<List<ApplicationByStageName>> query2 = null;

            var query =
                    (from app in _EngineCoreDBContext.Application
                     group app by app.CurrentStageId into g
                     select new ApplicationByStageType
                     {
                         Count = g.Count(),
                         StageTypeId = (int)g.Key
                     });


            var query1 = (
               from stg in _EngineCoreDBContext.AdmStage
               join t in _EngineCoreDBContext.SysTranslation
                            on stg.Shortcut equals t.Shortcut
               join q in query
                            on stg.Id equals q.StageTypeId
                        into result
               from newTable in result.DefaultIfEmpty()

               where t.Lang == lang
               select new ApplicationByStageType
               {
                   Count = newTable.Count,
                   StageTypeId = stg.Id,
                   StageTypeName = t.Value
               });

            query2 = (
                from q1 in query1
                group q1 by q1.StageTypeName into g2
                select new ApplicationByStageName
                {
                    Count = g2.Sum(b => b.Count),
                    StageName = g2.Key
                }

                ).ToListAsync();


            return await query2;
        }


        public async Task<List<ApplicationByStageType>> IncomingApplications(string lang)
        {
            Task<List<ApplicationByStageType>> query1 = null;

            var query =
                    (from app in _EngineCoreDBContext.Application
                     join stg in _EngineCoreDBContext.AdmStage
                       on app.CurrentStageId equals stg.Id
                     group app by stg.StageTypeId into t
                     select new ApplicationByStageType
                     {
                         Count = t.Count(),
                         StageTypeId = (int)t.Key,

                     });


            query1 = (
               from lv in _EngineCoreDBContext.SysLookupValue
               join t in _EngineCoreDBContext.SysTranslation
                      on lv.Shortcut equals t.Shortcut
               join q in query
                       on lv.Id equals q.StageTypeId
               //into result
               // from newTable in result.DefaultIfEmpty()

               where t.Lang == lang
               select new ApplicationByStageType
               {
                   Count = q.Count,
                   StageTypeId = q.StageTypeId,
                   StageTypeName = t.Value

               }).ToListAsync();




            return await query1;
        }


        public async Task<List<ApplicationAttachmentView>> getRelatedAttachments(int appId, string lang)
        {
            Task<List<ApplicationAttachmentView>> query = null;
            query = (from aa in _EngineCoreDBContext.ApplicationAttachment
                     join lv in _EngineCoreDBContext.SysLookupValue
                         on aa.AttachmentId equals lv.Id
                     join tr in _EngineCoreDBContext.SysTranslation
                         on lv.Shortcut equals tr.Shortcut

                     where aa.ApplicationId == appId
                     where tr.Lang == lang
                     select new ApplicationAttachmentView
                     {
                         AttachmentId = (int)aa.AttachmentId,
                         Id = aa.Id,
                         TranslationId = tr.Id,
                         AttachmentName = tr.Value,
                         FileName = aa.FileName,
                         Note = aa.Note

                     }).ToListAsync();

            return await query;
        }

        /*
                public async Task<int> SaveAllApplicationData_(
                                                              ApplicationDto appDto,
                                                              List<ApplicationAttachmentDto> appAttachmentDtos,
                                                              List<ApplicationPartyWithExtraDto> appPartyDtos,
                                                              TransactionDto transactionDto,
                                                              ApplicationTrackDto appTrackDto, int AppId)

                {
                    int id, appId, errorCount = 0, transactionId = -1;
                    List<AppTransaction> transactions = null;
                    //using (var transaction = _EngineCoreDBContext.Database.BeginTransaction())
                    try
                    {

                        if (AppId > 0)
                            appId = AppId;
                        else
                        {
                            if (appDto == null)
                                return Constants.ERROR;
                            appId = await Add(appDto);
                            //if (appId < 0)
                            //  return Constants.ERROR;
                        }

                        if (appAttachmentDtos != null)
                            foreach (var applicationAttachmentDto in appAttachmentDtos)
                            {
                                applicationAttachmentDto.ApplicationId = appId;
                                id = await _IApplicationAttachmentRepository.Add(applicationAttachmentDto);
                                if (id < 0)
                                    --errorCount;
                            }

                        if (transactionDto != null && AppId <= 0)//-----Add transaction just if this is a new post
                        {
                            transactionDto.ApplicationId = appId;
                            transactionId = await _ITransactionRepository.Add(transactionDto);
                        }

                        if (appPartyDtos != null)
                        {
                            if (transactionId == -1)//------------------get transaction id by given appId
                            {
                                transactions = await _ITransactionRepository.GetAll(appId);
                                transactionId = transactions[0].Id;
                            }
                            foreach (var appPartyDto in appPartyDtos)
                            {
                                appPartyDto.TransactionId = transactionId;
                                id = await _IApplicationPartyRepository.Add(appPartyDto);
                            }
                        }

                        if (appTrackDto != null)
                        {
                            appTrackDto.ApplicationId = appId;
                            id = await _IApplicationTrackRepository.Add(appTrackDto);
                        }

                        return appId;
                    }
                    catch (Exception)
                    {
                        //transaction.Rollback();
                        return Constants.ERROR;
                    }
                }
                */

        public async Task<APIResult> PostAllApplicationData_(ApplicationDto appDto,
                                                        List<TargetApplicationDto> appTargetDtos,
                                                        List<ApplicationAttachmentDto> appAttachmentDtos,
                                                        List<ApplicationPartyWithExtraDto> appPartyDtos,
                                                        List<AppRelatedContentDto> appRelatedContentDtos,
                                                        TransactionDto transactionDto,
                                                        ApplicationTrackDto appTrackDto,
                                                        FIERST_SAVE_STAGE toStage,
                                                        int userId, string lang)


        {
            int id, id_, appId, partyUserId,
            transactionId = -1;
            APIResult ApiResult = new APIResult();
            ApiResult.Result = false;
            List<AppTransaction> transactions = null;
            if (appDto == null)
            {
                ApiResult.Message.Add(getMessage(lang, "AppNotProvided"));
                return ApiResult;
            }
            AdmService svce = await _IAdmServiceRepository.GetOne((int)appDto.ServiceId);
            appId = await AddToStage(appDto, (int)toStage, userId);
            if (appId < 0)
            {
                ApiResult.Message.Add(getMessage(lang, "AppAddFail"));
                return ApiResult;
            }

            CreateFolderMessage CFM = CreateAppFolder(appDto.ServiceId.ToString(), appId.ToString());
            if (!CFM.SuccessCreation)
            {
                ApiResult.Result = false;
                ApiResult.Message.Clear();
                ApiResult.Message.Add(getMessage(lang, "AppFolderFail"));
                return ApiResult;
            }

            if (appRelatedContentDtos != null && appRelatedContentDtos.Count > 0)
            {
                foreach (var arc in appRelatedContentDtos)
                {
                    arc.AppId = (int)appId;

                    id = await Add(arc);
                    if (id == ERROR)
                    {
                        ApiResult.Id = ERROR;
                        ApiResult.Message.Add("خطأ في إضافة المحضر"/*getMessage(lang, "AttachmentAddFail")*/);
                        DeleteAppFolder(appDto.ServiceId.ToString(), appId.ToString());
                        return ApiResult;
                    }
                }
            }


            if (svce.Id == 1022 || svce.Id == 1034)
            {
                bool c = appTargetDtos.Any(x => x.TargetAppId != null) || appTargetDtos.Any(x => x.TargetAppDocument != null && x.TargetAppDocument.Length > 0) || (appTargetDtos.Any(x => x.TargetAppDesc != null && x.TargetAppDesc.Length > 10));
                if (!c)
                {
                    ApiResult.Id = -1;
                    ApiResult.Message.Add("يرجى رفع ملف او اختيار محرر أو إضافة معلومات واضحة عن المحرر الطلوب");
                    DeleteAppFolder(appDto.ServiceId.ToString(), appId.ToString());
                    return ApiResult;
                }
            }
            if (appTargetDtos != null && appTargetDtos.Count > 0 && svce.TargetService != null)
            {
                foreach (var dto in appTargetDtos)
                {
                    //if (dto.TargetAppId > 0 || (dto.TargetAppDocument!=null && dto.TargetAppDocument.Length>0)) 
                    //{
                    string document;
                    if (dto.TargetAppId != null)
                    {
                        var t = await GetOne((int)dto.TargetAppId);
                        document = t.AppTransaction.DocumentUrl;
                    }
                    else
                    {
                        document = dto.TargetAppDocument;
                    }
                    TargetApplicationDto tAppDto = new TargetApplicationDto();
                    tAppDto.AppId = appId;
                    tAppDto.TargetAppId = dto.TargetAppId;
                    tAppDto.TargetAppDesc = dto.TargetAppDesc;
                    tAppDto.TargetAppDocument = document;
                    if (tAppDto.TargetAppDocument != null && tAppDto.TargetAppDocument.Length > 0)
                    {
                        string destination = Path.Combine(_IConfiguration["BaseFolder"], appDto.ServiceId.ToString(), appId.ToString(), tAppDto.TargetAppDocument);
                        bool b = MoveAppAttachment(tAppDto.TargetAppDocument, destination);

                        if (b)
                        {
                            tAppDto.TargetAppDocument = destination;
                        }
                    }
                    id = await Add(tAppDto);
                    if (id < 0)
                    {
                        ApiResult.Id = -1;
                        ApiResult.Message.Add(getMessage(lang, "TargetAppNotAdded"));
                        DeleteAppFolder(appDto.ServiceId.ToString(), appId.ToString());
                        return ApiResult;
                    }
                    // } 
                }
            }

            if (appAttachmentDtos != null)
            {
                foreach (var appAttachmentDto in appAttachmentDtos)
                {
                    appAttachmentDto.ApplicationId = appId;
                    appAttachmentDto.CreatedBy = userId;
                    appAttachmentDto.LastUpdatedBy = userId;
                    string destination = Path.Combine(_IConfiguration["BaseFolder"], appDto.ServiceId.ToString(), appId.ToString(), appAttachmentDto.FileName);
                    bool b = MoveAppAttachment(appAttachmentDto.FileName, destination);
                    if (b)
                    {
                        appAttachmentDto.FileName = destination;
                        id = await _IApplicationAttachmentRepository.Add(appAttachmentDto, userId);
                        if (id < 0)
                        {
                            ApiResult.Id = -1;
                            ApiResult.Message.Add(getMessage(lang, "AttachmentAddFail"));
                            DeleteAppFolder(appDto.ServiceId.ToString(), appId.ToString());
                            return ApiResult;
                        }
                    }
                    else
                    {
                        ApiResult.Id = -1;
                        ApiResult.Message.Add(getMessage(lang, "AttachmentFileAddFail"));
                        DeleteAppFolder(appDto.ServiceId.ToString(), appId.ToString());
                        return ApiResult;
                    }
                }
            }
            if (transactionDto != null)
            {
                transactionDto.ApplicationId = appId;
                if (transactionDto.FileName != null && transactionDto.FileName.Length > 0)
                {
                    string destination = Path.Combine(_IConfiguration["BaseFolder"], appDto.ServiceId.ToString(), appId.ToString(), transactionDto.FileName);
                    bool b = MoveAppAttachment(transactionDto.FileName, destination);
                    if (b)
                    {
                        transactionDto.FileName = destination;
                    }
                    else
                    {
                        ApiResult.Id = -1;
                        ApiResult.Message.Add(getMessage(lang, "AttachmentAddFail"));
                        DeleteAppFolder(appDto.ServiceId.ToString(), appId.ToString());
                        return ApiResult;
                    }
                }
                transactionId = await _ITransactionRepository.Add(transactionDto, userId);
                if (transactionId < 0)
                {
                    ApiResult.Id = -1;
                    ApiResult.Message.Add(getMessage(lang, "TransactionAddFail"));
                    DeleteAppFolder(appDto.ServiceId.ToString(), appId.ToString());
                    return ApiResult;
                }
            }
            if (appPartyDtos != null)
            {
                if (transactionId == -1)
                {
                    transactions = await _ITransactionRepository.GetAll(appId);
                    if (transactions != null)
                    {
                        transactionId = transactions[0].Id;
                    }
                    else
                    {
                        ApiResult.Message.Add("لا يوجد معاملة مرتبطة بالطلب");
                        DeleteAppFolder(appDto.ServiceId.ToString(), appId.ToString());
                        return ApiResult;
                    }
                }
                foreach (var applicationPartyDto in appPartyDtos)
                {
                    if (applicationPartyDto.FullName != null && applicationPartyDto.FullName.Trim().Length > 0)
                    {
                        applicationPartyDto.TransactionId = transactionId;
                        ApiResult = await _IApplicationPartyRepository.AddPartyToUser(applicationPartyDto, lang);
                        if (ApiResult.Id < 0)
                        {

                            DeleteAppFolder(appDto.ServiceId.ToString(), appId.ToString());
                            return ApiResult;
                        }
                        partyUserId = ApiResult.Id;
                        if (partyUserId > 0)
                        {
                            applicationPartyDto.PartyId = partyUserId;
                            if (applicationPartyDto.IdAttachment != null && applicationPartyDto.IdAttachment.Length > 0)
                            {
                                string destination = Path.Combine(_IConfiguration["BaseFolder"], appDto.ServiceId.ToString(), appId.ToString(), applicationPartyDto.IdAttachment);
                                MoveAppAttachment(applicationPartyDto.IdAttachment, destination);
                                applicationPartyDto.IdAttachment = destination;
                            }
                            id = await _IApplicationPartyRepository.Add(applicationPartyDto, userId);
                            if (id > 0)
                            {
                                if (applicationPartyDto.PartyExtraAttachment != null)
                                    if (applicationPartyDto.PartyExtraAttachment.Count > 0)
                                        foreach (var PartyExtraAttachment in applicationPartyDto.PartyExtraAttachment)
                                        {
                                            PartyExtraAttachment.ApplicationPartyId = id;
                                            string destination = Path.Combine(_IConfiguration["BaseFolder"], appDto.ServiceId.ToString(), appId.ToString(), PartyExtraAttachment.AttachmentUrl);
                                            MoveAppAttachment(PartyExtraAttachment.AttachmentUrl, destination);
                                            PartyExtraAttachment.AttachmentUrl = destination;

                                            id_ = await _IApplicationPartyRepository.AddExtraAttachment(PartyExtraAttachment);
                                            if (id_ < 0)
                                            {
                                                ApiResult.Id = -1;
                                                ApiResult.Message.Add(getMessage("ar", "ExAttachmentNotAdded"));
                                                DeleteAppFolder(appDto.ServiceId.ToString(), appId.ToString());
                                                return ApiResult;
                                            }
                                        }
                            }
                            else
                            {
                                ApiResult.Id = -1;
                                ApiResult.Message.Add(getMessage("ar", "PartyNotAdded"));
                                DeleteAppFolder(appDto.ServiceId.ToString(), appId.ToString());
                                return ApiResult;
                            }
                        }
                        else
                        {
                            ApiResult.Id = -1;
                            ApiResult.Message.Add(getMessage("ar", "PartyUserNotAdded"));
                            DeleteAppFolder(appDto.ServiceId.ToString(), appId.ToString());
                            return ApiResult;
                        }

                    }
                    else
                    {
                        ApiResult.Id = -1;
                        ApiResult.Message.Add("الرجاء ادخال اسم الطرف");
                        DeleteAppFolder(appDto.ServiceId.ToString(), appId.ToString());
                        return ApiResult;
                    }
                }
            }

            if (appTrackDto != null && toStage == FIERST_SAVE_STAGE.REVIEW)//---------Dont track from Create to Draft 
            {
                List<int> stages = await StageAndNext(appDto.ServiceId, 1, 2);
                appTrackDto.ApplicationId = appId;
                appTrackDto.StageId = stages[0];
                appTrackDto.NextStageId = stages[1];
                appTrackDto.Note = appDto.Note;//"تم إرسال الطلب";
                appTrackDto.NoteKind = (short?)NoteKind.Normal;

                id = await _IApplicationTrackRepository.Add(appTrackDto);
                if (id < 0)
                {
                    ApiResult.Id = -1;
                    ApiResult.Message.Add(getMessage(lang, "AppAppTrackFail"));
                    DeleteAppFolder(appDto.ServiceId.ToString(), appId.ToString());
                    return ApiResult;
                }
            }
            ApiResult.Id = appId;
            ApiResult.Code = Math.Abs(OK);
            ApiResult.Result = true;
            ApiResult.Message.Add(getMessage(lang, "Done"));
            // scope.Complete();
            return ApiResult;
        }

        //--------------------------------------------------One Step Saving-----------------------------------------
        /*     public async Task<int> PostAllApplicationData( ApplicationDto applicationDto,
                                                            List<ApplicationAttachmentDto> applicationAttachmentDtos,
                                                            List<ApplicationPartyWithExtraDto> applicationPartyDtos,
                                                            TransactionDto transactionDto,
                                                            ApplicationTrackDto applicationTrackDto)


             {
                 int id, appId, userId,errorCount = 0, transactionId = -1;
                 List<AppTransaction> transactions = null;

                 if (applicationDto == null)
                     return Constants.ERROR;
                 appId = await Add(applicationDto);
                 if (appId < 0)
                     return Constants.ERROR;
                 //  }

                 if (applicationAttachmentDtos != null)
                     foreach (var applicationAttachmentDto in applicationAttachmentDtos)
                     {
                         applicationAttachmentDto.ApplicationId = appId;
                         id = await _IApplicationAttachmentRepository.Add(applicationAttachmentDto);

                         if (id < 0)
                             --errorCount;
                     }

                 if (transactionDto != null )//-----Add transaction just if this is a new post
                 {
                     transactionDto.ApplicationId = appId;
                     transactionId = await _ITransactionRepository.Add(transactionDto);
                 }

                 if (applicationPartyDtos != null)
                 {
                     if (transactionId == -1)//------------------get transaction id by given appId
                     {
                         transactions = await _ITransactionRepository.GetAll(appId);
                         transactionId = transactions[0].Id;
                     }
                     foreach (var applicationPartyDto in applicationPartyDtos)
                     {
                          applicationPartyDto.TransactionId = transactionId;
                          //--------------Add Party To User ---------------------
                           userId = await _IApplicationPartyRepository.AddPartyToUser(applicationPartyDto);
                         if (userId > 0)
                          {
                            applicationPartyDto.PartyId = userId;
                            id = await _IApplicationPartyRepository.Add(applicationPartyDto);
                            if (id > 0)
                             if (applicationPartyDto.PartyExtraAttachment != null)
                                 if (applicationPartyDto.PartyExtraAttachment.Count > 0)
                                     foreach (var PartyExtraAttachment in applicationPartyDto.PartyExtraAttachment)
                                     {
                                         PartyExtraAttachment.ApplicationPartyId = id;
                                         await _IApplicationPartyRepository.AddExtraAttachment(PartyExtraAttachment);
                                     }

                           }

                     }
                 }

                 if (applicationTrackDto != null)
                 {
                     applicationTrackDto.ApplicationId = appId;
                     id = await _IApplicationTrackRepository.Add(applicationTrackDto);
                     if (id < 0)
                         --errorCount;
                 }
                 return errorCount < 0 ? errorCount : appId;
             }

             */

        //--------------------------------------------------One Step Saving old-----------------------------------------
        /* public async Task<int> SaveAllApplicationDataAppId(ApplicationDto applicationDto,
                                                            List<ApplicationAttachmentDto> applicationAttachmentDtos,
                                                            List<ApplicationPartyWithExtraDto> applicationPartyDtos,
                                                            List<ApplicationPartyExtraAttachmentDto> applicationPartyExtraAttachmentWIds,
                                                            TransactionDto transactionDto,
                                                            ApplicationTrackDto applicationTrackDto, int AppId)


         {
             int id, appId, errorCount = 0, 
                  transactionId = -1;
             List<AppTransaction> transactions = null;

                 if (applicationDto != null)
                     appId = await Update(AppId, applicationDto);


             if (applicationAttachmentDtos != null)
                 foreach (var applicationAttachmentDto in applicationAttachmentDtos)
                 {
                     applicationAttachmentDto.ApplicationId = AppId;
                     id = await _IApplicationAttachmentRepository.Add(applicationAttachmentDto);

                     if (id < 0)
                         --errorCount;
                 }

             if (transactionDto != null && AppId <= 0)//-----Add transaction just if this is a new post
             {
                 transactionDto.ApplicationId = AppId;
                 transactionId = await _ITransactionRepository.Add(transactionDto);
             }

             if (applicationPartyDtos != null)
             {
                 if (transactionId == -1)//------------------get transaction id by given appId
                 {
                     transactions = await _ITransactionRepository.GetAll(AppId);
                     transactionId = transactions[0].Id;
                 }
                 foreach (var applicationPartyDto in applicationPartyDtos)
                 {
                     applicationPartyDto.TransactionId = transactionId;
                     id = await _IApplicationPartyRepository.Add(applicationPartyDto);
                     if (id > 0)
                         if (applicationPartyDto.PartyExtraAttachment != null)
                             if (applicationPartyDto.PartyExtraAttachment.Count > 0)
                                 foreach (var PartyExtraAttachment in applicationPartyDto.PartyExtraAttachment)
                                 {
                                     PartyExtraAttachment.ApplicationPartyId = id;
                                     await _IApplicationPartyRepository.AddExtraAttachment(PartyExtraAttachment);
                                 }


                     // --errorCount;
                 }
             }

             if (applicationTrackDto != null)
             {
                 applicationTrackDto.ApplicationId = AppId;
                 id = await _IApplicationTrackRepository.Add(applicationTrackDto);
                 if (id < 0)
                     --errorCount;
             }

             if (applicationPartyExtraAttachmentWIds != null)
             {
                 foreach (var applicationPartyExtraAttachmentWId in applicationPartyExtraAttachmentWIds)
                 {
                     id = await _IApplicationPartyRepository.AddExtraAttachment(applicationPartyExtraAttachmentWId);
                     if (id < 0)
                         --errorCount;
                 }
             }
             return errorCount < 0 ? errorCount : AppId;
         }*/

        //--------------------------------------------------One Step Saving -----------------------------------------
        public async Task<APIResult> SaveAllApplicationData(ApplicationWIdDto appDto,
                                                      List<TargetApplicationDto> appTargetDtos,
                                                      List<ApplicationAttachmentDto> appAttachmentDtos,
                                                      List<ApplicationPartyWithExtraDto> appPartyDtos,
                                                      List<ApplicationPartyExtraAttachmentDto> appPartyExtraAttachmentWIds,
                                                      List<AppRelatedContentDto> appRelatedContentDtos,
                                                      TransactionDto transactionDto,
                                                      ApplicationTrackDto appTrackDto, int userId, string lang)


        {
            int id, asUserId, transactionId = -1;
            List<AppTransaction> transactions = null;
            APIResult ApiResult = new APIResult();
            if (appDto == null)
            {
                ApiResult.Id = NOT_FOUND;
                ApiResult.Message.Add(getMessage("ar", "AppNotFound"));
                ApiResult.Code = Math.Abs(NOT_FOUND);
                return ApiResult;
            }
            //  using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            //-------ta
            if (appTargetDtos != null && appTargetDtos.Count > 0)//&& svce.TargetService != null
            {
                foreach (var dto in appTargetDtos)
                {
                    if (dto.TargetAppId > 0 || dto.TargetAppDocument != null)
                    {
                        bool b = await IsTargetAppExist((int)appDto.Id, dto.TargetAppId, dto.TargetAppDocument);
                        if (!b)
                        {
                            //------------------New 
                            string document;
                            TargetApplicationDto tAppDto = new TargetApplicationDto();
                            if (dto.TargetAppId != null)
                            {
                                var t = await GetOne((int)dto.TargetAppId);
                                document = t.AppTransaction.DocumentUrl;
                            }
                            else
                            {
                                document = dto.TargetAppDocument;
                            }
                            if (!_IFilesUploaderRepository.FileExist(document))
                            {
                                ApiResult.Message.Add("الوثيقة الخاصة بالطلب الذي تم اختياره مفقودة");
                                ApiResult.Id = -1;
                                return ApiResult;
                            }
                            //------------------/New
                            tAppDto.AppId = appDto.Id;
                            tAppDto.TargetAppId = dto.TargetAppId;
                            tAppDto.TargetAppDesc = dto.TargetAppDesc;
                            tAppDto.TargetAppDocument = document;
                            id = await Add(tAppDto);
                            if (id < 0)
                            {
                                ApiResult.Message.Add(getMessage(lang, "TargetAppNotAdded"));
                                return ApiResult;
                            }
                        }
                    }

                }
            }

            if (appRelatedContentDtos != null && appRelatedContentDtos.Count > 0)
            {
                foreach (var arc in appRelatedContentDtos)
                {
                    arc.AppId = (int)appDto.Id;

                    id = await Add(arc);
                    if (id == ERROR)
                    {
                        ApiResult.Id = ERROR;
                        ApiResult.Message.Add("خطأ في إضافة المحضر"/*getMessage(lang, "AttachmentAddFail")*/);
                        return ApiResult;
                    }
                }
            }

            if (appAttachmentDtos != null && appAttachmentDtos.Count > 0)
            {
                foreach (var appAttachmentDto in appAttachmentDtos)//------------MISSING
                {
                    appAttachmentDto.ApplicationId = appDto.Id;
                    string destination = Path.Combine(_IConfiguration["BaseFolder"], appDto.ServiceId.ToString(), appDto.Id.ToString(), appAttachmentDto.FileName);
                    MoveAppAttachment(appAttachmentDto.FileName, destination);
                    appAttachmentDto.FileName = destination;
                    id = await _IApplicationAttachmentRepository.Add(appAttachmentDto, userId);
                    if (id == ERROR)
                    {
                        ApiResult.Id = ERROR;
                        ApiResult.Message.Add(getMessage(lang, "AttachmentAddFail"));
                        return ApiResult;
                    }
                }
            }

            if (transactionDto != null && appDto.Id <= 0)//-----Add transaction just if this is a new post
            {
                transactionDto.ApplicationId = appDto.Id;
                if (transactionDto.FileName != null && transactionDto.FileName.Length > 0)
                {
                    string destination = Path.Combine(_IConfiguration["BaseFolder"], appDto.ServiceId.ToString(), appDto.Id.ToString(), transactionDto.FileName);
                    MoveAppAttachment(transactionDto.FileName, destination);
                    transactionDto.FileName = destination;
                }
                transactionId = await _ITransactionRepository.Add(transactionDto, userId);
                if (transactionId == ERROR)
                {
                    ApiResult.Id = ERROR;
                    ApiResult.Message.Add(getMessage(lang, "TransactionUpdateFail"));
                    return ApiResult;
                }
            }

            if (appPartyDtos != null && appPartyDtos.Count > 0)
            {
                if (transactionId == -1)
                {
                    transactions = await _ITransactionRepository.GetAll(appDto.Id);
                    transactionId = transactions[0].Id;
                }
                foreach (var appPartyDto in appPartyDtos)
                {
                    if (appPartyDto.FullName != null && appPartyDto.FullName.Trim().Length > 0)
                    {
                        appPartyDto.TransactionId = transactionId;
                        ApiResult = await _IApplicationPartyRepository.AddPartyToUser(appPartyDto, lang);
                        if (ApiResult.Id < 0)
                        {
                            return ApiResult;
                        }

                        asUserId = ApiResult.Id;
                        if (asUserId > 0)
                        {
                            appPartyDto.PartyId = asUserId;
                            if (appPartyDto.IdAttachment != null && appPartyDto.IdAttachment.Length > 0)
                            {
                                string destination = Path.Combine(_IConfiguration["BaseFolder"], appDto.ServiceId.ToString(), appDto.Id.ToString(), appPartyDto.IdAttachment);
                                MoveAppAttachment(appPartyDto.IdAttachment, destination);
                                appPartyDto.IdAttachment = destination;
                            }
                            id = await _IApplicationPartyRepository.Add(appPartyDto, userId);
                            if (id > 0)
                            {
                                if (appPartyDto.PartyExtraAttachment != null)
                                    if (appPartyDto.PartyExtraAttachment.Count > 0)
                                        foreach (var PartyExtraAttachment in appPartyDto.PartyExtraAttachment)
                                        {
                                            PartyExtraAttachment.ApplicationPartyId = id;
                                            string destination = Path.Combine(_IConfiguration["BaseFolder"], appDto.ServiceId.ToString(), appDto.Id.ToString(), PartyExtraAttachment.AttachmentUrl);
                                            MoveAppAttachment(PartyExtraAttachment.AttachmentUrl, destination);
                                            PartyExtraAttachment.AttachmentUrl = destination;
                                            await _IApplicationPartyRepository.AddExtraAttachment(PartyExtraAttachment);
                                        }
                            }
                            else
                            {
                                ApiResult.Id = -1;
                                ApiResult.Message.Add(getMessage(lang, "PartyNotAdded"));
                                return ApiResult;
                            }
                        }
                        else
                        {
                            ApiResult.Id = -1;
                            ApiResult.Message.Add(getMessage(lang, "PartyUserNotAdded"));
                            return ApiResult;
                        }
                    }
                    else
                    {
                        ApiResult.Message.Add("الرجاء ادخال اسم الطرف");
                        return ApiResult;
                    }
                }
            }

            if (appTrackDto != null)
            {
                appTrackDto.ApplicationId = appDto.Id;
                appTrackDto.NoteKind= (short?)NoteKind.Normal;
                id = await _IApplicationTrackRepository.Add(appTrackDto);
                if (id == ERROR)
                {
                    ApiResult.Id = ERROR;
                    ApiResult.Message.Add(getMessage(lang, "AppAppTrackFail"));
                    return ApiResult;
                }
            }

            if (appPartyExtraAttachmentWIds != null && appPartyExtraAttachmentWIds.Count > 0)
            {
                foreach (var appPartyExtraAttWId in appPartyExtraAttachmentWIds)
                {

                    string destination = Path.Combine(_IConfiguration["BaseFolder"], appDto.ServiceId.ToString(), appDto.Id.ToString(), appPartyExtraAttWId.AttachmentUrl);
                    MoveAppAttachment(appPartyExtraAttWId.AttachmentUrl, destination);
                    appPartyExtraAttWId.AttachmentUrl = destination;
                    id = await _IApplicationPartyRepository.AddExtraAttachment(appPartyExtraAttWId);
                    if (id == ERROR)
                    {
                        ApiResult.Id = ERROR;
                        ApiResult.Message.Add(getMessage(lang, "ExAttachmentNotAdded"));
                        return ApiResult;
                    }
                }
            }

            ApiResult.Id = (int)appDto.Id;
            ApiResult.Result = true;
            ApiResult.Code = Math.Abs(OK);
            ApiResult.Message.Add(getMessage(lang, "Done"));
            //scope.Complete();
            return ApiResult;
        }

        //--------------------------------------------One Step Update-----------------------------------------------
        public async Task<APIResult> UpdateAllApplicationData(ApplicationWIdDto appDto,
                                                        List<TargetApplicationWIdDto> appTargetDtos,
                                                        List<ApplicationAttachmentWIdDto> appAttachmentDtos,
                                                        List<ApplicationPartyWIdDto> appPartyDtos,
                                                        List<ApplicationPartyExtraAttachmentWIdDto> appPartyExtraAttachWIds,
                                                        List<AppRelatedContentWIdDto> appRelatedContentDtos,
                                                        TransactionWIdDto transactionDto,
                                                        ApplicationTrackWIdDto appTrackDto,
                                                        int userId, string lang)
        {
            int id;
            APIResult ApiResult = new APIResult();
            if (appDto != null)
            {
                appDto.AppLastUpdateDate = DateTime.Now;
                APIResult UpdateApiResult = await Update((int)appDto.Id, userId, appDto);
                if (UpdateApiResult.Result == null)
                {
                    return UpdateApiResult;
                }
            }
            //  using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            //-------update ta
            if (appTargetDtos != null && appTargetDtos.Count > 0)//&& svce.TargetService != null
            {
                foreach (var dto in appTargetDtos)
                {
                    if (dto.TargetAppId > 0 || dto.TargetAppDocument != null)
                    {
                        TargetApplicationDto tAppDto = new TargetApplicationDto();
                        tAppDto.AppId = appDto.Id;
                        tAppDto.TargetAppId = dto.TargetAppId;
                        tAppDto.TargetAppDesc = dto.TargetAppDesc;
                        tAppDto.TargetAppDocument = dto.TargetAppDocument;
                        id = await Add(tAppDto);
                        if (id < 0)
                        {
                            ApiResult.Message.Add(getMessage(lang, "TargetAppNotAdded"));
                            return ApiResult;
                        }

                    }

                }
            }

            if (appRelatedContentDtos != null && appRelatedContentDtos.Count > 0)
            {
                foreach (var arc in appRelatedContentDtos)
                {
                    //arc.AppId = (int)appDto.Id;

                    ApiResult = await UpdateRContent(arc.Id, arc);
                    if (ApiResult.Id < 0)
                    {
                        ApiResult.Id = ERROR;
                        ApiResult.Message.Add("خطأ في تعديل المحضر");
                        return ApiResult;
                    }
                }
            }

            if (appAttachmentDtos != null && appAttachmentDtos.Count > 0)
            {
                foreach (var applicationAttachmentDto in appAttachmentDtos)
                {
                    if (!applicationAttachmentDto.FileName.StartsWith(_IConfiguration["BaseFolder"]))
                    {
                        string destination = Path.Combine(_IConfiguration["BaseFolder"], appDto.ServiceId.ToString(), appDto.Id.ToString(), applicationAttachmentDto.FileName);
                        MoveAppAttachment(applicationAttachmentDto.FileName, destination);
                        applicationAttachmentDto.FileName = destination;
                    }
                    id = await _IApplicationAttachmentRepository.Update(applicationAttachmentDto.Id, userId, applicationAttachmentDto);
                    if (id == Constants.ERROR)
                    {
                        ApiResult.Message.Add(getMessage(lang, "AttachmentUpdateFail"));
                        return ApiResult;
                    }
                }
            }

            if (appPartyExtraAttachWIds != null && appPartyExtraAttachWIds.Count > 0)
            {
                foreach (var applicationPartyExtraAttachmentWId in appPartyExtraAttachWIds)
                {
                    if (!applicationPartyExtraAttachmentWId.AttachmentUrl.StartsWith(_IConfiguration["BaseFolder"]))
                    {
                        string destination = Path.Combine(_IConfiguration["BaseFolder"], appDto.ServiceId.ToString(), appDto.Id.ToString(), applicationPartyExtraAttachmentWId.AttachmentUrl);
                        MoveAppAttachment(applicationPartyExtraAttachmentWId.AttachmentUrl, destination);
                        applicationPartyExtraAttachmentWId.AttachmentUrl = destination;
                    }
                    id = await _IApplicationPartyRepository.UpdateExtraAttachment(applicationPartyExtraAttachmentWId.Id, applicationPartyExtraAttachmentWId);
                    if (id == Constants.ERROR)
                    {
                        ApiResult.Message.Add(getMessage(lang, "PartyUpdateFail"));
                        return ApiResult;
                    }
                }
            }

            if (transactionDto != null)
            {
                if (transactionDto.FileName != null && transactionDto.FileName.Length > 0)
                    if (!transactionDto.FileName.StartsWith(_IConfiguration["BaseFolder"]))
                    {
                        string destination = Path.Combine(_IConfiguration["BaseFolder"], appDto.ServiceId.ToString(), appDto.Id.ToString(), transactionDto.FileName);
                        MoveAppAttachment(transactionDto.FileName, destination);
                        transactionDto.FileName = destination;
                    }
                id = await _ITransactionRepository.Update(transactionDto.Id, userId, transactionDto);
                if (id == ERROR)
                {
                    ApiResult.Message.Add(getMessage(lang, "TransactionUpdateFail"));
                    return ApiResult;
                }
            }

            if (appPartyDtos != null && appPartyDtos.Count > 0)
            {
                foreach (var applicationPartyDto in appPartyDtos)
                {
                    if (applicationPartyDto.IdAttachment != null && applicationPartyDto.IdAttachment.Length > 0)

                        if (!applicationPartyDto.IdAttachment.StartsWith(_IConfiguration["BaseFolder"]))
                        {
                            string destination = Path.Combine(_IConfiguration["BaseFolder"], appDto.ServiceId.ToString(), appDto.Id.ToString(), applicationPartyDto.IdAttachment);
                            MoveAppAttachment(applicationPartyDto.IdAttachment, destination);
                            applicationPartyDto.IdAttachment = destination;
                        }
                    id = await _IApplicationPartyRepository.Update(applicationPartyDto.Id, userId, applicationPartyDto);
                    if (id == ERROR)
                    {
                        ApiResult.Message.Add(getMessage(lang, "PartyUpdateFail"));
                        return ApiResult;
                    }
                    //if (id == Constants.ERROR)  // --errorCount;
                    /* if (id == Constants.OK)
                         if (applicationPartyDto.PartyExtraAttachment.Count > 0)
                             foreach (var PartyExtraAttachment in applicationPartyDto.PartyExtraAttachment)
                             {
                               await _IApplicationPartyRepository.UpdateExtraAttachment(PartyExtraAttachment.Id, PartyExtraAttachment);
                             }*/
                }
            }

            if (appTrackDto != null)
            {
                id = await _IApplicationTrackRepository.Update(appTrackDto.Id, appTrackDto);
                if (id == ERROR)
                {
                    ApiResult.Message.Add("خطأ في إضافة الملاحظة");
                    return ApiResult;
                }
            }

            ApiResult.Id = (int)appDto.Id;
            ApiResult.Code = 200;
            ApiResult.Result = true;
            // scope.Complete();
            return ApiResult;
        }

        public async Task<Application> GetOneWithAllRelated(int id)
        {
            var query = _EngineCoreDBContext.Application.Where(x => x.Id == id)
                                                     .Include(y => y.ApplicationAttachment)
                                                     .Include(y => y.AppTransaction)
                                                              .ThenInclude(t => t.ApplicationParty)
                                                              .ThenInclude(b => b.ApplicationPartyExtraAttachment)
                                                     .Include(z => z.ApplicationTrack);
            return await query.FirstOrDefaultAsync();
        }

        //------------------------------------ DeleteRelated ApplicationData ----------------------------------------------------
        public async Task<APIResult> DeleteRelatedApplicationData(List<int> attachments, List<int> parties, List<int> extraAttachments)
        {
            APIResult ApiResult = new APIResult();
            int id;
            if (attachments != null && attachments.Count > 0)
                foreach (var attachment in attachments)
                {
                    id = await _IApplicationAttachmentRepository.DeleteOne(attachment);
                    if (id == ERROR)
                    {
                        ApiResult.Message.Add("خطأ في حذف المرفق");
                        return ApiResult;
                    }

                }
            if (parties != null && parties.Count > 0)
                foreach (var party in parties)
                {
                    id = await _IApplicationPartyRepository.DeleteOne(party);
                    if (id == ERROR)
                    {
                        ApiResult.Message.Add("خطأ في حذف الطرف");
                        return ApiResult;
                    }

                }
            if (extraAttachments != null && extraAttachments.Count > 0)
                foreach (var extra in extraAttachments)
                {
                    id = await _IApplicationPartyRepository.DeleteOneExtraAttachment(extra);
                    if (id == ERROR)
                    {
                        ApiResult.Message.Add("خطأ في حذف المرفق الافتراضي");
                        return ApiResult;
                    }

                }
            ApiResult.Code = Math.Abs(OK);
            ApiResult.Result = true;
            ApiResult.Id = Math.Abs(OK);
            return ApiResult;
        }

        //-------*** USED IN : StageForward API ***-----------------Set Stage Forward--------------------------------------------
        public async Task<int> SetStageForward(StagePayload sp, int userId)
        {

            byte[] newRowVersion;
            int nextStage = await NextPreviewsStage((int)sp.application.ServiceId, (int)sp.application.CurrentStageId, 1);
            if (nextStage > 0)
            {
                sp.trackDto.StageId = sp.application.CurrentStageId;
                sp.trackDto.NextStageId = nextStage;
                sp.trackDto.NoteKind= (short?)NoteKind.Normal;
                sp.application.CurrentStageId = nextStage;
                sp.application.StateId = await _ISysValueRepository.GetIdByShortcut(ONPROGRESS);
                sp.application.AppLastUpdateDate = DateTime.Now;
                APIResult ApiResult = new APIResult();
                ApiResult = await Update((int)sp.application.Id, userId, sp.application);
                newRowVersion = ApiResult.Result;
                await _IApplicationTrackRepository.Add(sp.trackDto);
                return nextStage;
            }
            else return Constants.ERROR;
        }

        //------ *** USED IN : StageBackward API *** ---------------Set Stage Backward-------------------------------------------
        public async Task<int> SetStageBackward(StagePayload sp, int userId)
        {
            byte[] newRowVersion;
            int previewsStage = await NextPreviewsStage((int)sp.application.ServiceId, (int)sp.application.CurrentStageId, -1);
            if (previewsStage > 0)
            {
                sp.trackDto.StageId = sp.application.CurrentStageId;
                sp.trackDto.NextStageId = previewsStage;
                sp.trackDto.NoteKind= (short?)NoteKind.Normal;
                sp.application.CurrentStageId = previewsStage;
                sp.application.StateId = await _ISysValueRepository.GetIdByShortcut(ONPROGRESS);
                sp.application.AppLastUpdateDate = DateTime.Now;
                APIResult ApiResult = new APIResult();
                ApiResult = await Update((int)sp.application.Id, userId, sp.application);
                newRowVersion = ApiResult.Result;
                if (sp.trackDto != null)
                await _IApplicationTrackRepository.Add(sp.trackDto);
                return previewsStage;
            }
            else return ERROR;
        }

        //----- *** USED IN : BackToFirstStage API *** -------------BackTo Draft-------------------------------------------------
        public async Task<APIResult> SetToFirstStage(StagePayload stagePayload, int userId)
        {
            int returnedStateId, DraftStageId;
            APIResult ApiResult = new APIResult();
            returnedStateId = await _ISysValueRepository.GetIdByShortcut(RETURNED);
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            if (returnedStateId > 0)
            {
                DraftStageId = await _IAdmServiceRepository.FirstStage(stagePayload.application.ServiceId);
                if (DraftStageId > 0)
                {
                    stagePayload.trackDto.StageId = stagePayload.application.CurrentStageId;
                    stagePayload.trackDto.NextStageId = DraftStageId;
                    stagePayload.trackDto.NoteKind= (short?)NoteKind.Returned;
                    stagePayload.application.StateId = returnedStateId;
                    stagePayload.application.CurrentStageId = DraftStageId;
                    await _IApplicationTrackRepository.Add(stagePayload.trackDto);
                    stagePayload.application.AppLastUpdateDate = DateTime.Now;
                    ApiResult = await Update((int)stagePayload.application.Id, userId, stagePayload.application);

                    if (ApiResult.Result != null)
                    {
                        await EndAppMeetings((int)stagePayload.application.Id);
                        ApiResult.Id = DraftStageId;
                        ApiResult.Message.Add(getMessage("ar", "Done"));
                        ApiResult.Code = Math.Abs(OK);
                        scope.Complete();
                    }

                    return ApiResult;
                }
                else
                {
                    ApiResult.Id = ERROR;
                    ApiResult.Message.Add(getMessage("ar", "SetStageFail"));
                }
            }
            else
            {
                ApiResult.Message.Add(getMessage("ar", "SetStateFail"));
            }
            return ApiResult;
        }



        //----- *** USED IN ChangeState API ***---------------------Change application state-------------------------------------
        public async Task<int> SetToState(StagePayload sp, int userId, string state)
        {

            int StateId;
            StateId = await _ISysValueRepository.GetIdByShortcut(state);
            if (StateId > 0)
            {
                try
                {
                    sp.application.StateId = StateId;
                    sp.application.AppLastUpdateDate = DateTime.Now;
                    await Update((int)sp.application.Id, userId, sp.application);
                    sp.trackDto.NextStageId = sp.application.CurrentStageId;
                    sp.trackDto.NoteKind= (short?)NoteKind.Normal;
                    await _IApplicationTrackRepository.Add(sp.trackDto);
                    return StateId;
                }
                catch
                {
                    return ERROR;
                }
            }
            else
                return ERROR;
        }

        //----- *** USED IN Sign API ***----------------------------Schedule----------------------------------------------------- 
        public async Task<dynamic> Schedule(string appId, int ServiceTypeId, int UserId, int serviceId, string title, DateTime date)
        {
            PickTicket pickTicket = await _IQueueRepository.PickTicket(appId, ServiceTypeId, _IUserRepository.GetUserID(), date, true);
            var sk = await _IServiceKindRepository.GetServiceKindById(ServiceTypeId, "ar");
            var parties = await SignersAndNot(Int32.Parse(appId));
            string Password = null;
            Random rnd = new Random();
            int p = rnd.Next(120502, 989799);
            Password = p.ToString();
            MeetingPostDto meetingPostDto = new MeetingPostDto()
            {
                StartDate = pickTicket.ExpectDateTime,
                EndDate = pickTicket.ExpectDateTime.AddMinutes(sk.EstimatedTimePerProcess),
                Password = Password,
                Status = 0,
                Topic = title,
                TimeZone = "Asia/Dubai",
                OrderNo = appId,
                Description = "مقابلة كاتب العدل",
                PasswordReq = true
            };
            Meeting meeting = await _IMeetingRepository.AddMeeting(meetingPostDto, UserId, "ar");
            var obj = new
            {
                parties = parties.Select(a => a.FullName).ToList(),
                serviceId = serviceId,
                appId = appId,

            };
            var json = new JavaScriptSerializer().Serialize(obj);

            foreach (var party in parties)
            {
                if (party.PartyId != null)
                {
                    CalendarPostDto calendarPostDto = new CalendarPostDto()
                    {
                        UserId = (int)party.PartyId,
                        StartDate = meetingPostDto.StartDate,
                        EndDate = meetingPostDto.EndDate,
                        MeetingId = meeting.Id,
                        Title = title,
                        Description = json,
                        NotifyMe = 0
                    };
                    await _ICalendarRepository.AddCalendar(calendarPostDto, "ar");
                }
            }
            return new
            {
                Id = meeting.Id,
                Password = Password,
                MeetingId = meeting.MeetingId,
                StartDate = meeting.StartDate.Date.ToString("dd/MM/yyyy"),
                StartTime = meeting.StartDate.ToString("hh:mm tt"),
                TicketNo = sk.Symbol + pickTicket.TicketId.ToString(),
            };
        }

        //----- *** ***----------------------------Change sign date ----------------------------------------------------- 
        public async Task<APIResult> UpdateSchedule(string appId, int ServiceTypeId, int serviceId, int actionId, int UserId, DateTime newDate)
        {

            APIResult result = new APIResult();
            List<MeetingGetDto> meetingGetDtos = new List<MeetingGetDto>();
            MeetingPostDto meetingPostDto = null;
            MeetingGetDto meetingGetDto = null;
            PickTicket pickTicket = new PickTicket();
            var sk = await _IServiceKindRepository.GetServiceKindById(ServiceTypeId, "ar");
            // pick a new ticket and update the queue process.
            try
            {
                pickTicket = await _IQueueRepository.PickTicket(appId, ServiceTypeId, _IUserRepository.GetUserID(), newDate, true);
                //  TODO Review if the last meeting is the target.
                meetingGetDtos = await _IMeetingRepository.GetMeetingByOrderNo(appId);
                meetingGetDto = meetingGetDtos.LastOrDefault();
                meetingPostDto = new MeetingPostDto
                {
                    StartDate = pickTicket.ExpectDateTime,
                    EndDate = pickTicket.ExpectDateTime.AddMinutes(sk.EstimatedTimePerProcess),
                    Description = meetingGetDto.Description,
                    Password = meetingGetDto.Password,
                    PasswordReq = meetingGetDto.PasswordReq,
                    TimeZone = meetingGetDto.TimeZone,
                    Topic = meetingGetDto.Topic,
                    OrderNo = meetingGetDto.OrderNo,
                    Status = meetingGetDto.Status
                };
            }
            catch (Exception e)
            {
                result.Message.Add("خطأ في تعديل الموعد");
                return result;

            }

            try
            {
                await _IMeetingRepository.UpdateMeeting(meetingGetDto.Id, meetingPostDto, UserId, "ar");
            }
            catch (Exception e)
            {
                result.Message.Add("خطأ في تعديل الموعد");
                return result;
            }

            // update the calendar for all the participants.

            try
            {
                List<CalendarGetDto> calendarsGetDto = await _ICalendarRepository.GetCalendarByMeetingId(meetingGetDto.Id);
                foreach (var calendarGetDto in calendarsGetDto)
                {
                    CalendarPostDto calendarPostDto = new CalendarPostDto
                    {

                        StartDate = meetingPostDto.StartDate,
                        EndDate = meetingPostDto.EndDate,
                        Description = calendarGetDto.Description,
                        Title = calendarGetDto.Title,
                        UserId = calendarGetDto.UserId,
                        MeetingId = calendarGetDto.MeetingId,
                        NotifyMe = calendarGetDto.NotifyMe
                    };
                    await _ICalendarRepository.UpdateCalendar(calendarGetDto.Id, calendarPostDto, "ar");
                }
            }
            catch (Exception e)
            {
                result.Message.Add("خطأ في تعديل المفكرة");
                return result;

            }

            dynamic schedule =
            new
            {
                meetingId = meetingGetDto.MeetingId,
                serviceKind = sk.Id,
                StartDate = meetingPostDto.StartDate.Date.ToString("dd/MM/yyyy"),
                StartTime = meetingPostDto.StartDate.ToString("hh:mm tt"),
                TicketNo = sk.Symbol + pickTicket.TicketId.ToString(),
                FinishDate = meetingPostDto.EndDate.Date.ToString("dd/MM/yyyy"),
                FinishTime = meetingPostDto.EndDate.ToString("hh:mm tt")
            };

            try
            {
                var actionNotification = await _INotificationSettingRepository.GetNotificationsForAction(actionId);
                List<Receiver> rs = await GetReceiverPartyByAppID(Convert.ToInt32(appId), true);
                List<Receiver> receivers = await AddUserReceiverData(rs);
                var notification = await getScheduleNotifications(actionNotification, receivers, schedule, serviceId, Convert.ToInt32(appId), "ar");
                await _ISendNotificationRepository.DoSend(notification, false);
            }
            catch (Exception e)
            {
                result.Message.Add("خطأ في إرسال الاشعارات");
                return result;

            }


            result.Message.Add("تم تغيير موعد المقابلة بنجاح");
            result.Message.Add($" التاريخ الجديد :{schedule.StartDate} , الوقت المتوقع : {schedule.StartTime}");

            result.Result = schedule;
            result.Id = 1;
            result.Code = 200;
            return result;


        }

        //----- *** In Sign Stage ***--------------------- Get application Schedule info ----------------------------------------------- 
        public async Task<dynamic> GetAppScheduleInfo(string orderNo)
        {
            dynamic result = (from meet in _EngineCoreDBContext.Meeting
                              join queue in _EngineCoreDBContext.QueueProcesses on meet.OrderNo equals queue.ProcessNo
                              join serviceKind in _EngineCoreDBContext.ServiceKind on queue.ServiceKindNo equals serviceKind.Id
                              where meet.OrderNo == orderNo
                              orderby meet.StartDate
                              select new
                              {
                                  meetingId = meet.MeetingId,
                                  password = meet.Password,
                                  serviceKind = serviceKind.Id,
                                  startDate = meet.StartDate.Date.ToString("dd/MM/yyyy"),
                                  startTime = meet.StartDate.ToString("hh:mm tt"),
                                  ticketNo = serviceKind.Symbol + queue.TicketId.ToString(),
                                  finishDate = meet.EndDate.Date.ToString("dd/MM/yyyy"),
                                  finishTime = meet.EndDate.ToString("hh:mm tt")
                              }
                      ).LastOrDefaultAsync();
            return await result;
        }

        // TODO should removed duplicated.
        public async Task<List<NotificationLogPostDto>> BuildNotificationObjectFromResponse(List<NotificationLogPostDto> notifications, List<int> reciverIDs)
        {
            List<NotificationLogPostDto> ToSendNotification = new List<NotificationLogPostDto>();
            var smsChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_SMS_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
            var emailChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_MAIL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();

            if (notifications != null && notifications.Count > 0)
            {
                for (int i = 0; i < reciverIDs.Count; ++i)
                {
                    foreach (NotificationLogPostDto n in notifications)
                    {
                        NotificationLogPostDto notify = new NotificationLogPostDto()
                        {
                            Lang = n.Lang,
                            NotificationBody = n.NotificationBody,
                            NotificationChannelId = n.NotificationChannelId,
                            NotificationTitle = n.NotificationTitle,
                            UserId = reciverIDs[i]
                        };
                        ToSendNotification.Add(notify);
                    }
                }
            }
            return ToSendNotification;
        }


        public async Task<List<NotificationLogPostDto>> BuildNotificationObjectFromResponse(List<NotificationLogPostDto> notifications, List<Receiver> recivers)
        {
            List<NotificationLogPostDto> ToSendNotification = new List<NotificationLogPostDto>();
            if (notifications == null || notifications.Count == 0)
            {
                return ToSendNotification;
            }

            var smsChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_SMS_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
            var emailChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_MAIL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();

            for (int i = 0; i < recivers.Count; ++i)
            {
                foreach (var noti in notifications)
                {
                    var netNoti = noti.ShallowCopy();
                    if (netNoti.NotificationChannelId == smsChannel || netNoti.NotificationChannelId == emailChannel)
                    {
                        netNoti.ToAddress = noti.NotificationChannelId == smsChannel ? recivers[i].Mobile : recivers[i].Email;
                        netNoti.UserId = recivers[i].Id;
                        ToSendNotification.Add(netNoti);
                    }
                }
            }
            return ToSendNotification;
        }

        public async Task<List<NotificationLogPostDto>> BuildNotificationObjectFromResponseByUser(List<NotificationLogPostDto> notifications, List<Receiver> receivers, List<string> notyBody)
        {
            List<NotificationLogPostDto> ToSendNotification = new List<NotificationLogPostDto>();

            if (notifications == null || notifications.Count == 0)
            {
                return ToSendNotification;
            }

            var smsChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_SMS_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
            var emailChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_MAIL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
            string link;
            Dictionary<string, string> ForUrl = new Dictionary<string, string>
            {
                { INTEREVIEW_LINK, "" },
                { INTEREVIEW_LINK_MOB, "" }
            };

            for (int i = 0; i < receivers.Count; ++i)
            {
                foreach (var notify in notifications)
                {
                    var newNoti = notify.ShallowCopy();
                    if (newNoti.NotificationChannelId == smsChannel || newNoti.NotificationChannelId == emailChannel)
                    {
                        newNoti.ToAddress = notify.NotificationChannelId == smsChannel ? receivers[i].Mobile : receivers[i].Email;
                        // ForUrl[INTEREVIEW_LINK_MOB] = notyBody[i];
                        // ForUrl[INTEREVIEW_LINK] = $"<a href='{notyBody[i]}'>{notyBody[i]}</a>";
                        // newNoti.NotificationBody = ReplaceParemeterByValues(ForUrl, newNoti.NotificationBody);
                        if (notyBody.Count > i)
                        {
                            link = newNoti.NotificationChannelId == smsChannel ? notyBody[i] : $"<a href='{notyBody[i]}'>{notyBody[i]}</a>";
                            newNoti.NotificationBody = $"{newNoti.NotificationBody} {receivers[i].Name}  {link}";
                        }
                        newNoti.UserId = receivers[i].Id;
                        ToSendNotification.Add(newNoti);
                    }
                }
            }

            return ToSendNotification;
        }

        public async Task<List<int>> GetPartyByAppID(int appId)
        {
            List<int> partiesId = new List<int>();
            var transactions = await _ITransactionRepository.GetAll(appId);
            if (transactions.Count > 0)
            {
                int transactionId = transactions[0].Id;
                partiesId = await _IApplicationPartyRepository.GetAllIDs(transactionId);
            }
            return partiesId;
        }

        public async Task<List<Receiver>> GetReceiverPartyByAppID(int appId, bool justRequiredParty)
        {
            List<Receiver> receivers = new List<Receiver>();
            var transactions = await _ITransactionRepository.GetAll(appId);
            if (transactions.Count > 0)
            {
                int transactionId = transactions[0].Id;
                receivers = await _IApplicationPartyRepository.GetAllReceivers(transactionId, justRequiredParty);
            }
            return receivers;
        }

        public async Task<int> NotifyAppParties(int appId, List<NotificationLogPostDto> notifications)
        {
            List<Receiver> rs = new List<Receiver>();
            try
            {
                if (notifications != null && notifications.Count > 0)
                {
                    rs = await GetReceiverPartyByAppID(appId, false);
                    List<Receiver> receivers = await AddUserReceiverData(rs);
                    foreach (var n in notifications)
                    {
                        n.ApplicationId = appId;
                    }
                    return await NotifyParties(receivers, notifications);
                }
                else return 0;
            }
            catch
            {
                return Constants.ERROR;
            }
        }

        public async Task<int> NotifyParties(List<int> parties, List<NotificationLogPostDto> notifications)
        {
            try
            {
                if (notifications != null && notifications.Count > 0 && parties.Count > 0)
                {
                    List<NotificationLogPostDto> ToSendNotifications = await BuildNotificationObjectFromResponse(notifications, parties);
                    await _ISendNotificationRepository.DoSend(ToSendNotifications, false);
                    return ToSendNotifications.Count;
                }
                else return 0;
            }
            catch
            {
                return ERROR;
            }
        }
        public async Task<int> NotifyParties(List<Receiver> receivers, List<NotificationLogPostDto> notifications)
        {

            try
            {
                if (notifications != null && notifications.Count > 0 && receivers.Count > 0)
                {
                    List<NotificationLogPostDto> ToSendNotifications = await BuildNotificationObjectFromResponse(notifications, receivers);
                    await _ISendNotificationRepository.DoSend(ToSendNotifications, false);
                    return ToSendNotifications.Count;
                }
                else return 0;
            }
            catch
            {
                return ERROR;
            }
        }

        public async Task<bool> IsSignedApp(int appId)
        {
            var transactions = await _ITransactionRepository.GetAll(appId);
            int transactionId = transactions[0].Id;
            int count = await _IApplicationPartyRepository.NotSignedPartyCount(transactionId);
            return count == 0;
        }
        /*
                public async Task<int>  ESignIt(string Base64Sign, StagePayload stagePayload)
                {
                    var s = _IApplicationPartyRepository.SavePartySignImage(Base64Sign);
                    if (s.Length > 0)
                     {
                        if(await IsSignedApp((int)stagePayload.application.Id))
                         {
                            int u = await SetStageForward(stagePayload);
                         }
                     }
                    return 1;

                }*/

        //-------*** ESign ***---------------ESign an application--------------------------------------------------------
        public async Task<APIResult> ESignIt(int appPartyId, int userId, int appId, string Base64Sign, int signType)
        {
            int i;
            APIResult ApiResult = new APIResult();
            Application a = await GetOne(appId);
            var transactions = await _ITransactionRepository.GetAll(appId);
            int transactionId = transactions[0].Id;
            ApplicationParty appParty = null;
            ApplicationPartyDto appPartyDto = null;
            if (transactionId > 0)
            {
                appParty = await _IApplicationPartyRepository.GetAppParty(transactionId, userId);
                if (appParty != null)
                {
                    if ((bool)appParty.EditableSign)
                    {
                        ApiResult.Result = _IApplicationPartyRepository.SavePartySignImage(Base64Sign);
                        if (ApiResult.Result.Length > 0)
                        {
                            string destination = Path.Combine(_IConfiguration["BaseFolder"], a.ServiceId.ToString(), appId.ToString(), ApiResult.Result);
                            bool m = MoveAppAttachment(ApiResult.Result, destination);
                            if (m)
                            {
                                appPartyDto = ApplicationPartyDto.GetDTO(appParty);
                                appPartyDto.SignDate = DateTime.Now;
                                appPartyDto.SignUrl = destination;
                                appPartyDto.Signed = true;
                                appPartyDto.SignType = signType;
                                appPartyDto.EditableSign = false;
                                i = await _IApplicationPartyRepository.Update(appPartyId, userId, appPartyDto);
                                if (i == OK)
                                {
                                    ApiResult.Id = appPartyId;
                                    ApiResult.Code = 200;
                                }
                                else
                                {
                                    ApiResult.Message.Add(getMessage("ar", "PartyUpdateFail"));
                                }
                            }
                            else
                            {
                                ApiResult.Message.Add(getMessage("ar", "SignFail"));
                            }
                        }
                        else
                        {
                            ApiResult.Message.Add(getMessage("ar", "SignFail"));
                        }
                    }
                    else
                    {
                        if ((bool)appParty.Signed)
                        {
                            ApiResult.Message.Add(getMessage("ar", "alreadySigned"));
                        }
                        else
                        {
                            ApiResult.Message.Add(getMessage("ar", "signNotAllowed"));
                        }

                    }
                }
            }
            return ApiResult;
        }

        public async Task<List<ApplicationPartySignState>> SignersAndNot(int appId)
        {
            var transactions = await _ITransactionRepository.GetAll(appId);
            int transactionId = transactions[0].Id;
            return await _IApplicationPartyRepository.PartySignState(transactionId);
        }

        public async Task<int> MakeItDone(int appId, int userId)
        {
            StagePayload sp = new StagePayload();
            Application app = await GetOne(appId);
            AramexRequests ArApp = await _EngineCoreDBContext.AramexRequests.Where(x => x.ApplicationId == appId).FirstOrDefaultAsync();
            ApplicationTrackDto appTDto = new ApplicationTrackDto();
            ApplicationWIdDto wIdDto = FromObjectToDto(app);
            Dictionary<string, string> ParameterDic = new Dictionary<string, string>();
            string ApplicationBaseUrl = _IConfiguration["ApplicationBaseUrl"];
            string pstatusText = "", pstatusTextAr = "";
            int currentStage = (int)app.CurrentStageId;
            int newStage = await NextPreviewsStage((int)app.ServiceId, (int)app.CurrentStageId, 1);
            if (newStage == 0)
            {
                return ERROR;
            }

            wIdDto.CurrentStageId = newStage;
            appTDto.ApplicationId = appId;
            appTDto.Note = getMessage("ar", "AppDoneSuccessfully");
            appTDto.UserId = userId;
            sp.application = wIdDto;
            sp.trackDto = appTDto;
            sp.trackDto.StageId = currentStage;

            var notification = await _INotificationSettingRepository.GetNotificationsForAction(3039);//7080
            try
            {
                var appPayment = await _IPaymentRepository.GetPaymentsInfoByAppId(appId, "ar");
                pstatusText = appPayment.PaymentStatus == Constants.PAYMENT_STATUS_ENUM.PAID ? getMessage("en", "Success") : getMessage("en", "Fail");
                pstatusTextAr = appPayment.PaymentStatus == Constants.PAYMENT_STATUS_ENUM.PAID ? getMessage("ar", "Success") : getMessage("ar", "Fail");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(" Error in GetPaymentsInfoByAppId during making the application is done " + ex.Message);
            }
            string appUrl = app.Id.ToString();
            string appUrlMob = app.Id.ToString();

            ParameterDic.Add(APPLICATION_NUMBER_MOB, appUrlMob);
            ParameterDic.Add(APPLICATION_NUMBER, appUrl);
            ParameterDic.Add(PAYMENT_STATUS_AR, pstatusTextAr);
            ParameterDic.Add(PAYMENT_STATUS, pstatusText);
            List<Receiver> rs = await GetReceiverPartyByAppID(appId, false);
            List<Receiver> receivers = await AddUserReceiverData(rs);
            List<string> TokenUrls = new List<string>();


            int templateId = (int)app.TemplateId;
            Template temp = await _ITemplateRepository.GetOne(templateId);

            try
            {
                int notDeliveriedStateId = await _ISysValueRepository.GetIdByShortcut("AramexNotDeliveried");
                var documentType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "document_type").FirstOrDefault();
                int documentTypeId = documentType.Id;
                int pOAId = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == documentTypeId)
                             join tr in _EngineCoreDBContext.SysTranslation on lv.Shortcut equals tr.Shortcut
                             where tr.Value.Contains("Power of Attorney")
                             select new { id = lv.Id }
                    ).FirstOrDefault().id;
                if (temp.DocumentTypeId == pOAId)
                {
                    InitialBlockChain(appId);
                }

                if (app.Delivery == true && ArApp == null)
                {
                    int transactionid = (int)_EngineCoreDBContext.AppTransaction.Where(x => x.ApplicationId == app.Id)
                                                                          .Select(z => z.Id).FirstOrDefault();
                    ApplicationParty applicationParty = new ApplicationParty();
                    applicationParty = _EngineCoreDBContext.ApplicationParty.Where(x => x.TransactionId == transactionid && x.IsOwner == true).FirstOrDefault();
                    if (applicationParty == null)
                    {
                        applicationParty = _EngineCoreDBContext.ApplicationParty.Where(x => x.TransactionId == transactionid).FirstOrDefault();
                    }
                    AramexPostDto aramexRequests = new AramexPostDto()
                    {
                        ApplicationId = app.Id,
                        OwnerName = applicationParty.FullName,
                        Email = applicationParty.Email,
                        Mobile = applicationParty.Mobile,
                        StateId = notDeliveriedStateId
                    };
    
                     await _IAdmServiceRepository.AddAramexRequest(aramexRequests);
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation("Aramex & blockChain cose  "+ex.Message);

            }

            try
            {
                int t;
                var svc = await _IAdmServiceRepository.GetOne((int)app.ServiceId);
                switch ((SERVICE_RESULT)svc.ServiceResult)
                {
                    case SERVICE_RESULT.CANCEL:
                        int cancelId = await _ISysValueRepository.GetIdByShortcut(CANCELED);
                        if (app.TargetApplicationApp != null && app.TargetApplicationApp.Count() > 0)
                        {
                            foreach (var ta in app.TargetApplicationApp)
                            {
                                if (ta.TargetAppId != null)
                                {
                                    BlockChainPoa blockChainPoa = _EngineCoreDBContext.BlockChainPoa.Where(x => x.AppId == ta.TargetAppId).FirstOrDefault();
                                    var transactions = await _ITransactionRepository.GetAll(ta.TargetAppId);
                                    t = await _ITransactionRepository.ChangeTransactionStatus(transactions[0].Id, userId, cancelId);
                                    if (blockChainPoa != null)
                                    {
                                        blockChainPoa.IsSysCancelled = true;
                                        _IGeneralRepository.Update(blockChainPoa);
                                        if (await _IGeneralRepository.Save())
                                        {
                                            RevokeDto revokeDto = new RevokeDto()
                                            {

                                                VcID = blockChainPoa.Vcid,
                                                RevocationReason = "Cancelled by owner"
                                            };
                                        
                                            _IBlockChain.RevokevcID(revokeDto,(int) ta.TargetAppId);
                                        }
                                    }
                                }

                               

                            }
                            
                        }

                        break;
                    case SERVICE_RESULT.EXECUTE:
                        int executeId = await _ISysValueRepository.GetIdByShortcut(EXECUTED);
                        if (app.TargetApplicationApp != null && app.TargetApplicationApp.Count() > 0)
                        {
                            foreach (var ta in app.TargetApplicationApp)
                            {
                                if (ta.TargetAppId != null)
                                {
                                    var transactions = await _ITransactionRepository.GetAll(ta.TargetAppId);
                                    t = await _ITransactionRepository.ChangeTransactionStatus(transactions[0].Id, userId, executeId);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }

                int notify = await FillAndSendNotification(notification, receivers, ParameterDic, (int)app.ServiceId, app.Id, true, "ar");

                return await SetToState(sp, userId, DONE);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(" Error in MakeItDone during making the application is done " + ex.Message + " for the app " + appId.ToString());
                return ERROR;
            }
        }

        /* public async Task<partiesInfo> PartyFinalDocument(int appId)
         {
             int transactionId = 0;
             List<string> relatedTransactions = new List<string>();
             var transactions = await _ITransactionRepository.GetAll(appId);
             if (transactions.Count > 0)
             { 
                 transactionId = transactions[0].Id;
             }

             if (transactionId > 0)
             {
                 var app = await GetOne(appId);
                 if(app.TargetApplicationApp.Count>0)
                 {
                     string docUrl;
                     foreach (var tapp in app.TargetApplicationApp)
                     {
                         docUrl = "";
                         if (tapp.TargetAppDocument!=null && tapp.TargetAppDocument.Length>0)
                         {
                          docUrl = tapp.TargetAppDocument;
                          relatedTransactions.Add(docUrl);
                         }
                       // else
                       //  { if (tapp.TargetAppId != null){var trans =await _ITransactionRepository.GetAll(tapp.TargetAppId);
                            //    docUrl = trans[0].DocumentUrl;relatedTransactions.Add(docUrl); }                        }

                     }
                 }
                 TemplateView template = null;
                 if (app.TemplateId != null)
                 {
                  template = await _ITemplateRepository.GetTemplateName((int)app.TemplateId, "ar");
                 }


                 List<ApplicationPartyFinalDocument> query = null;
                 query = await
                       (from ap in _EngineCoreDBContext.ApplicationParty
                        join lv in _EngineCoreDBContext.SysLookupValue
                             on ap.PartyTypeValueId equals lv.Id
                        join t in _EngineCoreDBContext.SysTranslation
                             on lv.Shortcut equals t.Shortcut
                        join c in _EngineCoreDBContext.Country
                             on ap.Nationality equals c.UgId

                        join ex in _EngineCoreDBContext.ApplicationPartyExtraAttachment
                              on ap.Id equals ex.ApplicationPartyId
                                 into result
                        from newTable in result.DefaultIfEmpty()
                        join lv2 in _EngineCoreDBContext.SysLookupValue
                             on newTable.AttachmentId equals lv2.Id
                                 into result_
                        from newTable_ in result_.DefaultIfEmpty()

                        join t2 in _EngineCoreDBContext.SysTranslation
                             on newTable_.Shortcut equals t2.Shortcut
                                into result2
                        from newTable2 in result2.DefaultIfEmpty()
                        join c2 in _EngineCoreDBContext.Country
                             on newTable.CountryOfIssue equals c2.UgId
                                into result3
                        from newTable3 in result3.DefaultIfEmpty()

                        join lv2 in _EngineCoreDBContext.SysLookupValue
                             on newTable.AttachmentId equals lv2.Id
                                into result4
                        from newTable4 in result4.DefaultIfEmpty()

                        where t.Lang == "ar"
                        where (newTable2.Lang == "ar" || newTable2.Lang == null)
                        where ap.TransactionId == transactionId

                        select new ApplicationPartyFinalDocument
                        {
                            Id = ap.Id,
                            FullName = ap.FullName,
                            Mobile = ap.Mobile,
                            Address = ap.Address,
                            Nationality = c.CntCountryAr,
                            PartyType = ap.Description,//t.Value,
                            EmirateId = ap.EmiratesIdNo,
                            EmirateIdUrl = ap.IdAttachment,
                            AttachmentName = newTable.AttachmentId == null ? newTable.AttachmentName : newTable2.Value,
                            AttachmentNo = newTable.Number,
                            AttachmentUrl = newTable.AttachmentUrl,
                            CountryOfIssue = newTable3.CntCountryAr,
                            SignUrl = ap.SignUrl,
                            SignRequired = ap.SignRequired,
                            // IsOwner=ap.IsOwner

                        }).ToListAsync();

                 var a = query.GroupBy(d => new { d.Id, d.FullName, d.Mobile, d.Address, d.Nationality, d.PartyType, d.EmirateId, d.EmirateIdUrl, d.SignUrl, d.SignRequired })
                //.OrderBy(d => d.Key)
                .Select(g => new ApplicationPartyFinalDocumentGrouped
                {
                    Id = g.Key.Id,
                    FullName = g.Key.FullName,
                    Mobile = g.Key.Mobile,
                    Address = g.Key.Address,
                    Nationality = g.Key.Nationality,
                    AttachmentName = g.Key.EmirateId != null ? "هوية امارتية" : null,
                    CountryOfIssue = g.Key.EmirateId != null ? "الامارات العربية المتحدة" : null,
                    AttachmentNo = g.Key.EmirateId,
                    AttachmentUrl = g.Key.EmirateIdUrl,
                    PartyType = g.Key.PartyType,
                    SignUrl = g.Key.SignUrl,
                    SignRequired = g.Key.SignRequired,

                    AttachmentsList = g.Select(a => new Documentation()
                    {
                        AttachmentName = a.AttachmentName,
                        AttachmentNo = a.AttachmentNo,
                        CountryOfIssue = a.CountryOfIssue,
                        AttachmentUrl = a.AttachmentUrl

                    }).ToList()
                }).ToList();

                 DateTime meetingDate = DateTime.Now;
                 var meetings = await _EngineCoreDBContext.Meeting.Where(x => x.OrderNo == app.Id.ToString()).ToListAsync();
                 if (meetings != null && meetings.Count>0)
                 {
                     // TODO get the last meeting for the application.
                     meetingDate = meetings[meetings.Count - 1].StartDate;
                 }

                 var resultObject = new partiesInfo
                 {

                     ServiceResult =  _IAdmServiceRepository.GetOne((int)app.ServiceId).Result.ServiceResult,
                     ApplicationNo =app.ApplicationNo,
                     MeetingDate = meetingDate,
                     TransactionId = transactionId,
                     Title = transactions[0].Title,
                     TransactionNo = transactions[0].TransactionNo,
                     DocumentType = template != null ? template.DocumentType : null,
                     TemplateName = template != null ? template.Title : null,
                     parties = a,
                     transactionText = transactions[0].Content,
                     FileName = transactions[0].FileName,
                     decisionText = transactions[0].DecisionText,
                     relatedContents = await GetAppRelatedContents(appId, "ar"),
                     relatedTransactions= relatedTransactions,
                     notaryInfo = await GetLastUpdaterNotary(appId)
                 };
                 return resultObject;
             }
             else
                 return null;
         }
        */


        public async Task<partiesInfo> PartyFinalDocument(int appId)
        {

            int transactionId = 0;
            partiesInfo info = new partiesInfo();
            var app = await GetOne(appId);
            var service = await _IAdmServiceRepository.GetOne((int)app.ServiceId);
            short ServiceResult = (short)service.ServiceResult;
            if (ServiceResult == 0)
            {
                info.ServiceResult = 0;
                return info;
            }
            List<string> relatedTransactions = new List<string>();
            var transactions = await _ITransactionRepository.GetAll(appId);
            if (transactions.Count > 0)
            {
                transactionId = transactions[0].Id;
            }
            else
            {
                info = null;
                return info;
            }


            if (app.TargetApplicationApp.Count > 0)
            {
                string docUrl;
                //-----------------------Old way-----
                /* foreach (var tapp in app.TargetApplicationApp)
                 {
                     docUrl = "";
                     if (tapp.TargetAppDocument != null && tapp.TargetAppDocument.Length > 0)
                     {
                         docUrl = tapp.TargetAppDocument;
                         relatedTransactions.Add(docUrl);
                     }

                 }*/
                //-----------------------New way-----
                var docUrl_ = app.TargetApplicationApp;
                docUrl = app.TargetApplicationApp.Where(t => t.TargetAppId != null && !string.IsNullOrEmpty(t.TargetAppDocument/*.Trim()*/) && _IFilesUploaderRepository.FileExist(t.TargetAppDocument))
                    .Select(t => t.TargetAppDocument).FirstOrDefault();
                if (docUrl == null)
                {
                    docUrl = app.TargetApplicationApp.Where(t => t.TargetAppDocument != null && t.TargetAppDocument.Trim().Length > 0).Select(t => t.TargetAppDocument).FirstOrDefault();
                }
                if (docUrl != null)
                {
                    relatedTransactions.Add(docUrl);
                }
                //-----------------------------------
            }
            TemplateView template = null;
            if (app.TemplateId != null)
            {
                template = await _ITemplateRepository.GetTemplateName((int)app.TemplateId, "ar");
            }


            List<ApplicationPartyFinalDocument> query = null;
            query = await
                  (from ap in _EngineCoreDBContext.ApplicationParty
                   join lv in _EngineCoreDBContext.SysLookupValue
                        on ap.PartyTypeValueId equals lv.Id
                   join t in _EngineCoreDBContext.SysTranslation
                        on lv.Shortcut equals t.Shortcut
                   join c in _EngineCoreDBContext.Country
                        on ap.Nationality equals c.UgId
                   into result0
                   from newTable0 in result0.DefaultIfEmpty()

                   join ex in _EngineCoreDBContext.ApplicationPartyExtraAttachment
                         on ap.Id equals ex.ApplicationPartyId
                            into result
                   from newTable in result.DefaultIfEmpty()
                   join lv2 in _EngineCoreDBContext.SysLookupValue
                        on newTable.AttachmentId equals lv2.Id
                            into result_
                   from newTable_ in result_.DefaultIfEmpty()

                   join t2 in _EngineCoreDBContext.SysTranslation
                        on newTable_.Shortcut equals t2.Shortcut
                           into result2
                   from newTable2 in result2.DefaultIfEmpty()
                   join c2 in _EngineCoreDBContext.Country
                        on newTable.CountryOfIssue equals c2.UgId
                           into result3
                   from newTable3 in result3.DefaultIfEmpty()

                   join lv2 in _EngineCoreDBContext.SysLookupValue
                        on newTable.AttachmentId equals lv2.Id
                           into result4
                   from newTable4 in result4.DefaultIfEmpty()

                   where t.Lang == "ar"
                   where (newTable2.Lang == "ar" || newTable2.Lang == null)
                   where ap.TransactionId == transactionId

                   select new ApplicationPartyFinalDocument
                   {
                       Id = ap.Id,
                       FullName = ap.FullName,
                       Mobile = ap.Mobile,
                       Address = ap.Address,
                       Nationality = newTable0.CntCountryAr,
                       PartyType = ap.Description,//t.Value,
                       EmirateId = ap.EmiratesIdNo,
                       EmirateIdUrl = ap.IdAttachment,
                       AttachmentName = newTable.AttachmentId == null ? newTable.AttachmentName : newTable2.Value,
                       AttachmentNo = newTable.Number,
                       AttachmentUrl = newTable.AttachmentUrl,
                       CountryOfIssue = newTable3.CntCountryAr,
                       SignUrl = ap.SignUrl,
                       SignRequired = ap.SignRequired,
                       // IsOwner=ap.IsOwner

                   }).ToListAsync();

            var a = query.GroupBy(d => new { d.Id, d.FullName, d.Mobile, d.Address, d.Nationality, d.PartyType, d.EmirateId, d.EmirateIdUrl, d.SignUrl, d.SignRequired })
           //.OrderBy(d => d.Key)
           .Select(g => new ApplicationPartyFinalDocumentGrouped
           {
               Id = g.Key.Id,
               FullName = g.Key.FullName,
               Mobile = g.Key.Mobile,
               Address = g.Key.Address,
               Nationality = g.Key.Nationality,
               AttachmentName = g.Key.EmirateId != null ? "هوية امارتية" : null,
               CountryOfIssue = g.Key.EmirateId != null ? "الامارات العربية المتحدة" : null,
               AttachmentNo = g.Key.EmirateId,
               AttachmentUrl = g.Key.EmirateIdUrl,
               PartyType = g.Key.PartyType,
               SignUrl = g.Key.SignUrl,
               SignRequired = g.Key.SignRequired,

               AttachmentsList = g.Select(a => new Documentation()
               {
                   AttachmentName = a.AttachmentName,
                   AttachmentNo = a.AttachmentNo,
                   CountryOfIssue = a.CountryOfIssue,
                   AttachmentUrl = a.AttachmentUrl

               }).ToList()
           }).ToList();

            DateTime meetingDate = DateTime.Now;
            var meetings = await _EngineCoreDBContext.Meeting.Where(x => x.OrderNo == app.Id.ToString()).ToListAsync();
            if (meetings != null && meetings.Count > 0)
            {
                // TODO get the last meeting for the application.
                meetingDate = meetings[meetings.Count - 1].StartDate;
            }

            var resultObject = new partiesInfo
            {
                ServiceResult = ServiceResult,
                ApplicationNo = app.ApplicationNo,
                MeetingDate = meetingDate,
                TransactionId = transactionId,
                Title = transactions[0].Title,
                TransactionNo = transactions[0].TransactionNo,
                DocumentType = template != null ? template.DocumentType : null,
                TemplateName = template != null ? template.Title : null,
                parties = a,
                transactionText = transactions[0].Content,
                FileName = transactions[0].FileName,
                decisionText = transactions[0].DecisionText,
                relatedContents = await GetAppRelatedContents(appId, "ar"),
                relatedTransactions = relatedTransactions,
                notaryInfo = await GetLastUpdaterNotary(appId)
            };
            return resultObject;
        }

        public async Task<APIResult> EndAppMeetings(int appId)
        {
            APIResult ApiResult = new APIResult();
            List<MeetingGetDto> meetingGetDtos = await _IMeetingRepository.GetMeetingByOrderNo(appId.ToString());

            // using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            foreach (var mid in meetingGetDtos)
            {

                int m = await _IMeetingRepository.SetMeetingStatus(mid.MeetingId, MEETING_STATUS.FINISHED, true, "ar");
                if (m > 0)
                {
                    bool result = await _IQueueRepository.ChangeTicketStatusToDone(_IUserRepository.GetUserID(), appId.ToString());
                    ApiResult.Result = result;
                    if (result)
                    {
                        ApiResult.Code = Math.Abs(OK);
                        ApiResult.Id = appId;
                        ApiResult.Message.Add("تم انهاء المقابلة ");

                    }
                    else
                    {
                        ApiResult.Message.Add("خطأ في تحديد العملية ");
                        return ApiResult;
                    }

                }
                else
                {
                    ApiResult.Message.Add("حدث خطأ في إنهاء المقابلة");
                    return ApiResult;
                }
            }


            //scope.Complete();
            return ApiResult;
        }

        public async Task<List<AppServiceStage>> GetRelatedApplicationsInfo(int appId, string lang)

        {
            List<AppServiceStage> query = null;
            query = await
                  (
                   from ta in _EngineCoreDBContext.TargetApplication
                   join ap in _EngineCoreDBContext.Application
                        on ta.AppId equals ap.Id
                   join srv in _EngineCoreDBContext.AdmService
                        on ap.ServiceId equals srv.Id
                   join t in _EngineCoreDBContext.SysTranslation
                        on srv.Shortcut equals t.Shortcut
                   join st in _EngineCoreDBContext.AdmStage
                        on ap.CurrentStageId equals st.Id
                   join t2 in _EngineCoreDBContext.SysTranslation
                        on st.Shortcut equals t2.Shortcut

                   where ta.TargetAppId == appId
                   where t.Lang == lang
                   where t2.Lang == lang


                   select new AppServiceStage
                   {
                       AppId = (int)ta.AppId,
                       ServiceName = t.Value,
                       StageName = t2.Value,
                       Block_Target = srv.BlockTarget,
                       App_state = (int)ap.StateId

                   }).ToListAsync();
            return query;
        }
        //--------------pd--------

        public async Task<dynamic> GetPreviewStageData(string isNext,int appId, int userId, string lang)

        {

            int ReviewStageTypeId = await _ISysValueRepository.GetIdByShortcut("REVIEW");
            string type = "";
            object reason = null;
            PaymentDetialsForAPPDto paymentDetialsForAPP = new PaymentDetialsForAPPDto();
            bool isVideoStage = false;
            bool isPaymentStage = false;
            List<ActionOfStage> ActionButtons = new List<ActionOfStage>();
            bool blocked = false;
            bool rejected = false;
            bool EnabledSearch = false;
            bool MultipleSearch = false;
            APIResult lockIt = new APIResult();
            string TransactionState;
            int? ownerId;
            List<UserApplication> userApplication = null;
            dynamic feeInfo = null;
            List<string> messages = new List<string>();
            List<string> alerts = new List<string>();
            List<string> msg = new List<string>();
            List<ActionOfStage> RoleActionButtons = new List<ActionOfStage>();
            List<RelatedContentView> related_contents = new List<RelatedContentView>();
            List<TransactionFeeGetDto> fee = new List<TransactionFeeGetDto>();
            bool isParty = false;
            bool isEmp;
            bool asAdmin;
            bool isInspector;


            // TypeList reason = await _ISysValueRepository.GetTypeAll(lang, "service_reason").; //PaymentDto payment=null;

            isInspector = _IUserRepository.IsInspector();
            isEmp = _IUserRepository.IsEmployee();
            asAdmin = isEmp ? false : _IUserRepository.IsAdmin();
            isParty = (!isEmp && !asAdmin) ? await IsParty(appId, userId) : false;
            bool isAuth = isEmp || isParty || asAdmin|| isInspector;
            if (!isAuth)
            {
                msg.Add(getMessage(lang, "AppNotFound"));
                APIResult ApiResult = new APIResult(NOT_FOUND, AppStatus.UNAVAILABLE, Math.Abs(NOT_FOUND), msg);
                return ApiResult;
            }
            Application application = await GetOne(appId);
            if (application == null)
            {
                msg.Add(getMessage(lang, "AppNotFound"));
                APIResult ApiResult = new APIResult(NOT_FOUND, AppStatus.UNAVAILABLE, Math.Abs(NOT_FOUND), msg);
                return ApiResult;
            }
            if (application.Owner != null && application.Owner != userId)
            {
                msg.Add(getMessage(lang, "OwnedApp"));
                APIResult ApiResult = new APIResult(NOT_FOUND, AppStatus.OWNED, Math.Abs(NOT_FOUND), msg);
                return ApiResult;
            }
            byte[] rv = application.RowVersion;

            if (isEmp)
            {
                APIResult isItBookedUp = await IfBookedUp(appId, userId, lang);
                if (isItBookedUp.Result == AppStatus.BOOKEDUP || isItBookedUp.Result == AppStatus.UNAVAILABLE)
                {
                    return isItBookedUp;
                }
               
                else
                {
                    lockIt = await RefreshReadingDate(appId, userId);
                    rv = lockIt.Result;
                }
                AdmService service = await _IAdmServiceRepository.GetOne((int)application.ServiceId);
                if (isEmp && _EngineCoreDBContext.AdmStage.Where(x=>x.StageTypeId==ReviewStageTypeId).Select(z=>z.Id).ToList().Contains((int)application.CurrentStageId))


                {
                    APIResult NotNext = new APIResult();
                    if (isNext == "null" || isNext == "")
                    {
                        NotNext.Message.Add("لقد تجاوزت نظام الدور يرجى كتابة سبب قيامك بذلك");

                        NotNext.Result =NoteKind.ISNEXT;


                        return NotNext;
                    }
                }

                if (application.CurrentStageId == service.ApprovalStage)
                {

                    APIResult signResult = new APIResult();
                    List<AppTransaction> at = await _ITransactionRepository.GetAll(appId);
                    if (at.Count > 0)
                    {
                        signResult = await _IApplicationPartyRepository.IsSignEditByAnotherUser((int)at[0].Id, userId);
                        if (signResult.Result == AppStatus.LOCKED)
                        {
                            return signResult;
                        }
                    }
                }
            }
            int RejectedStateId = await _ISysValueRepository.GetIdByShortcut(REJECTED);
            int AutoRejectedStateId = await _ISysValueRepository.GetIdByShortcut(AUTOCANCEL);
            ServiceNamesDto serviceName = await _IAdmServiceRepository.GetOnename((int)application.ServiceId, lang);
            List<ApplicationPartyView> Parties = await _ITransactionRepository.getRelatedParties(application.AppTransaction.Id, lang);
            int tid = application.TemplateId > 0 ? (int)application.TemplateId : -1;
            TemplateView template = await _ITemplateRepository.GetTemplateName(tid, lang);
            var Stages = await getStageOfService(appId, (int)serviceName.Id, lang);

            var relatedApps = await GetRelatedApplicationsInfo(appId, lang);
            dynamic Schedule = null;

            if (isEmp || application.CurrentStageId == Stages[Stages.Count() - 1].Id)
            {
                related_contents = await GetAppRelatedContents(appId, lang);
            }

            if (isEmp || asAdmin)
            {
                if (related_contents == null || related_contents.Count == 0)
                {
                    if (tid > 0)
                    {
                        var template_related_contents = await _ITemplateRepository.GetRelatedContents(tid, application.CurrentStageId, lang);
                        List<string> rc = related_contents.Select(x => x.TitleSortcut).ToList();
                        var filtered_related_contents = template_related_contents.Where(x => !rc.Contains(x.TitleSortcut)).ToList();
                        related_contents = related_contents.Concat(filtered_related_contents).ToList();
                    }
                    if (related_contents == null || related_contents.Count == 0)
                    {
                        related_contents = await _IAdmServiceRepository.GetRelatedContents((int)application.ServiceId, lang);
                    }
                }

            }


            if (serviceName.TargetService != null /*&& application.TargetApplicationApp.Count > 0*/ && application.CurrentStageId <= serviceName.ApprovalStage)
            {
                //if (/*application.TargetApplicationApp.Count<1 ||*/ application.TargetApplicationApp.All(t => t.TargetAppDocument == null))
                if (/*application.TargetApplicationApp.Count<1 ||*/ application.TargetApplicationApp.All(t => t.TargetAppId == null))
                {
                    if (isEmp || asAdmin)
                    {
                        EnabledSearch = true;
                        MultipleSearch = serviceName.ExternalFileRequired == Convert.ToInt16(EXTERNAL_FILE_REQUIRED.MULTI) || serviceName.ExternalFileRequired == Convert.ToInt16(EXTERNAL_FILE_REQUIRED.OPTIONAL) ? true : false;
                        //ownerId = Parties.Where(x => x.IsOwner == true).Select(x => x.PartyId).FirstOrDefault();
                    }
                    else
                    {
                        ownerId = userId;
                        //}
                        if (ownerId != null)
                            //  {
                            userApplication = await GetUserTransaction((int)serviceName.TargetService, (int)ownerId, lang);
                    }

                }
            }



            var actionRole = await _IUserRepository.GetUserActionsPermissions(userId);

            if (relatedApps != null && relatedApps.Count > 0)
            {
                foreach (var rapp in relatedApps)
                {

                    if (await isBlocking(rapp.App_state, (bool)rapp.Block_Target))
                    {
                        blocked = true;
                        messages.Add("يوجد طلب متعلق بهذا الطلب يجب انجازه قبل المتابعة");
                        messages.Add($"الخدمة : {rapp.ServiceName}");
                        messages.Add($"رقم الطلب : {rapp.AppId}");
                        break;
                    }
                }

            }
            rejected = application.StateId == RejectedStateId || application.StateId == AutoRejectedStateId;
            if (rejected && (isEmp || asAdmin))
            {
                messages.Add(getMessage(lang, "rejectedApp"));
            }
            ActionButtons = await getActions((int)application.CurrentStageId, lang);
            if (!rejected && !blocked)
            {

                RoleActionButtons = ActionButtons.Where(x => actionRole.Contains(x.ActionId) && x.Group != FIX_BUTTON).ToList();
                isVideoStage = ActionButtons.Any(g => g.Group == VIDEO_BTN);
                isPaymentStage = ActionButtons.Any(g => g.Group == PAY_BTN);
            }
            else
            {
                if (!blocked)
                    RoleActionButtons = ActionButtons.Where(x => actionRole.Contains(x.ActionId) && x.Group == FIX_BUTTON).ToList();

            }

            if (isVideoStage)
            {
                Schedule = await GetAppScheduleInfo(appId.ToString());
            }

            try
            {
                paymentDetialsForAPP = await _IPaymentRepository.GetPaymentsInfoByAppId(appId, lang);
            }
            catch
            {
                paymentDetialsForAPP.PaymentStatus = Constants.PAYMENT_STATUS_ENUM.NOPAYMENT;
            }


            if (application.CurrentStageId == serviceName.ApprovalStage && (isEmp || asAdmin))
            {
                fee = await _ITransactionFeeRepository.GetTransactionFees();
                byte? dType = template.Type;

                TransactionFeeInput transactionFeeInput = new TransactionFeeInput()
                {
                    ServiceNo = (int)application.ServiceId,
                    DocumentKind = (dType == (byte)DOCUMENT_KIND.CONTRACTOREDITOR) ? DOCUMENT_KIND.CONTRACTOREDITOR : DOCUMENT_KIND.AGENCY,
                    ProcessKind = PROCESS_KIND.CONFIRM,
                    PartiesCount = Parties.Where(x => x.SignRequired == true && x.PartyTypeId != TRANSLATOR).Count(),
                    PageCount = 0,
                    Amount = (application.AppTransaction.ContractValue == null) ? 0 : (double)application.AppTransaction.ContractValue

                };

                List<TransactionFeeOutput> calculatedFee = new List<TransactionFeeOutput>();
                try
                {
                    calculatedFee = await _ITransactionFeeRepository.CalculateTransactionFee(transactionFeeInput, lang);
                }
                catch
                {
                    calculatedFee = null;
                }

                feeInfo = new
                {
                    transactionFeeInput = transactionFeeInput,
                    fee = fee,
                    calculatedFee = calculatedFee
                };
            }
            /*    var ButtonsGroup = RoleActionButtons.GroupBy(p => p.Group, (k, c) => new{Group = k,Buttons = c.Select(p => new {p.ActionId,p.ActionName,p.Enabled,p.executions}).ToList() }).ToList();*/

            /*APIResult reCheckisItLoacked = await ifLocked(appId, userId, lang);
            if (reCheckisItLoacked.Result)
            {
                return reCheckisItLoacked;
            }*/
            if (isEmp)
            {
                var app = await GetOne(appId);
                byte[] newRowVersion = app.RowVersion;
                APIResult isItBookedUp_ = await IfBookedUp(appId, userId, lang);
                if (rv != newRowVersion || isItBookedUp_.Result == AppStatus.BOOKEDUP)
                {
                    APIResult reCheckisItLoacked = new APIResult();
                    reCheckisItLoacked.Result = true;
                    reCheckisItLoacked.Message.Add("تم التعديل على الطلب أو حجزه من قبل كاتب عدل آخر");
                    reCheckisItLoacked.Message.Add("حاول مرة أخرى");
                    return reCheckisItLoacked;
                }
            }
            var state = await getAppState(application.StateId, lang);
            TransactionState = (application.AppTransaction.TransactionStatus != null && application.AppTransaction.DocumentUrl != null) ? await getAppState(application.AppTransaction.TransactionStatus, lang) : null;
            var attachments = await getRelatedAttachments(appId, lang);
            var appObjections = await GetAppObjection(appId);
            var result = new
            {
                template = template,
                serviceName = serviceName,
                Stages = Stages,
                State = state,
                Buttons = RoleActionButtons,
                Application = application,
                Attachments = attachments,
                Parties = Parties,
                Track = await _IApplicationTrackRepository.GetAllWithUser(appId),
                Payment = paymentDetialsForAPP,
                Schedule = Schedule,
                reason = reason,
                relatedApps = relatedApps,
                userApplication = userApplication,
                feeInfo = feeInfo,
                related_contents = related_contents.Count > 0 ? related_contents : null,
                messages = messages,
                alerts = alerts,
                EnabledSearch = EnabledSearch,
                MultipleSearch = MultipleSearch,
                TransactionState = TransactionState,
                AppObjections = appObjections
            };
            return result;
        }
        //-------------------------Light pd-------------
        public async Task<dynamic> GetPreviewStageDataLight(int appId, int userId, string lang)
        {
            List<string> msg = new List<string>();
            bool isEmp = _IUserRepository.IsEmployee();
            if (!true)
            {
                msg.Add(getMessage(lang, "AppNotFound"));
                APIResult ApiResult = new APIResult(NOT_FOUND, AppStatus.UNAVAILABLE, Math.Abs(NOT_FOUND), msg);
                return ApiResult;
            }
            Application application = await GetOne(appId);
            if (application == null)
            {
                msg.Add(getMessage(lang, "AppNotFound"));
                APIResult ApiResult = new APIResult(NOT_FOUND, AppStatus.UNAVAILABLE, Math.Abs(NOT_FOUND), msg);
                return ApiResult;
            }
            //AdmService service = await _IAdmServiceRepository.GetOne((int)application.ServiceId);
            ServiceNamesDto serviceName = await _IAdmServiceRepository.GetOnename((int)application.ServiceId, lang);
            List<ApplicationPartyView> Parties = await _ITransactionRepository.getRelatedParties(application.AppTransaction.Id, lang);
            int tid = application.TemplateId > 0 ? (int)application.TemplateId : -1;
            TemplateView template = await _ITemplateRepository.GetTemplateName(tid, lang);
            var Stages = await getStageOfService(appId, (int)serviceName.Id, lang);
            var relatedApps = await GetRelatedApplicationsInfo(appId, lang);
            var state = await getAppState(application.StateId, lang);
            var attachments = await getRelatedAttachments(appId, lang);
            var result = new
            {
                template = template,
                serviceName = serviceName,
                Stages = Stages,
                State = state,
                Application = application,
                Attachments = attachments,
                Parties = Parties,
                relatedApps = relatedApps,
            };
            return result;
        }

        // IN Preview Approval---------------------
        public async Task<APIResult> FullUpdateWithForwardNoti(FullUpdate fu, int userId, string lang, int actionId)
        {
            APIResult ApiResult = new APIResult();
            string ApplicationBaseUrl = _IConfiguration["ApplicationBaseUrl"];
            Dictionary<string, string> ParameterDic = new Dictionary<string, string>();

            int newStage = await NextPreviewsStage((int)fu.toUpdate.applicationDto.ServiceId, (int)fu.toUpdate.applicationDto.CurrentStageId, 1);
            fu.toUpdate.applicationDto.StateId = await _ISysValueRepository.GetIdByShortcut(ONPROGRESS);
            fu.toSave.applicationTrackDto.StageId = fu.toUpdate.applicationDto.CurrentStageId;
            fu.toUpdate.applicationDto.CurrentStageId = newStage;
            fu.toUpdate.applicationDto.AppLastUpdateDate = DateTime.Now;
            fu.toSave.applicationTrackDto.ApplicationId = fu.toUpdate.applicationDto.Id;
            fu.toSave.applicationTrackDto.StageId = fu.toSave.applicationDto.CurrentStageId;
            fu.toSave.applicationTrackDto.NextStageId = newStage;
            //fu.toSave.applicationTrackDto.Note = fu.toSave.applicationDto.Note.Length > 0 ? fu.toSave.applicationDto.Note : "تم إرسال الطلب";

            bool signed = await IsSignedApp((int)fu.toUpdate.applicationDto.Id);
            if (!signed)
            {
                ApiResult.Id = -1;
                ApiResult.Result = false;
                ApiResult.Code = 500;
                ApiResult.Message.Add(getMessage(lang, "notSigned"));
                return ApiResult;
            }

            EXTERNAL_FILE_REQUIRED external = (EXTERNAL_FILE_REQUIRED)await _EngineCoreDBContext.AdmService.Where(x => x.Id == fu.toUpdate.applicationDto.ServiceId).Select(x => x.ExternalFileRequired).FirstOrDefaultAsync();
            if (external == EXTERNAL_FILE_REQUIRED.ONE || external == EXTERNAL_FILE_REQUIRED.MULTI)
            {
                bool b = false;
                APIResult c = await isCertifiedTargetApp((int)fu.toUpdate.applicationDto.Id, true);
                if (fu.toSave.targetApplicationDtos != null)
                {
                    b = fu.toSave.targetApplicationDtos.Any(x => x.TargetAppId != null);
                }
                if (!b && !c.Result)
                {
                    ApiResult.Id = -1;
                    ApiResult.Result = false;
                    ApiResult.Code = 200;
                    ApiResult.Message.Add("  تتطلب هذه الخدمة وجود محرر سابق ");
                    ApiResult.Message.Add("يرجى تحديد المحرر المطلوب من خلال البحث واختيار النتيجة الصحيحة ");
                    return ApiResult;
                }
            }
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {

                ApiResult = await SendData(fu, userId, lang);
                if (!ApiResult.Result)
                {
                    return ApiResult;
                }

                if (fu.toSave.calculatedFeesDto != null)
                {
                    ApiResult = await AddFees(fu.toSave.calculatedFeesDto, (int)fu.toUpdate.applicationDto.Id, actionId, userId, lang);
                    if (!ApiResult.Result)
                    {
                        return ApiResult;
                    }
                }
                APIResult endMeetingResult = await EndAppMeetings((int)fu.toUpdate.applicationDto.Id);
                ParameterDic.Add(APPLICATION_NUMBER_MOB, fu.toUpdate.applicationDto.Id.ToString());
                ParameterDic.Add(APPLICATION_NUMBER, fu.toUpdate.applicationDto.Id.ToString());
                List<Receiver> rs = await GetReceiverPartyByAppID((int)fu.toUpdate.applicationDto.Id, false);
                List<Receiver> receivers = await AddUserReceiverData(rs);
                List<string> TokenUrls = new List<string>();
                int notify = await FillAndSendNotification(fu.notification, receivers, ParameterDic, (int)fu.toUpdate.applicationDto.ServiceId, (int)fu.toUpdate.applicationDto.Id, true, lang);

                ApiResult.Code = Math.Abs(OK);
                ApiResult.Result = true;
                ApiResult.Id = (int)fu.toUpdate.applicationDto.Id;
                ApiResult.Message.Add(getMessage("ar", "Done"));
                scope.Complete();
                return ApiResult;
            }
            catch (Exception e)
            {
                ApiResult.Id = -1;
                ApiResult.Message.Add("خطأ في التحديث");
                _logger.LogInformation($"Approval  meeting:{fu.toUpdate.applicationDto.Id}***{e.Message.ToString()}***{e.InnerException}**end Approval  meeting**");
                return ApiResult;
            }
        }
        //----------------In send Draft-------------------------
        public async Task<APIResult> FullUpdateWithForward(FullUpdate fu, int actionId, int userId, string lang)
        {

            APIResult ApiResult = new APIResult();
            var app = await GetOne((int)fu.toUpdate.applicationDto.Id);
            //int currnent_state = (int)fu.toUpdate.applicationDto.StateId;
            int returned_state = await _ISysValueRepository.GetIdByShortcut(RETURNED);

            int newStage = await NextPreviewsStage((int)fu.toUpdate.applicationDto.ServiceId, (int)fu.toUpdate.applicationDto.CurrentStageId, 1);
            fu.toSave.applicationTrackDto.StageId = fu.toUpdate.applicationDto.CurrentStageId;
            fu.toSave.applicationTrackDto.Note = fu.toUpdate.applicationDto.Note;
            fu.toUpdate.applicationDto.StateId = await _ISysValueRepository.GetIdByShortcut(ONPROGRESS);
            fu.toUpdate.applicationDto.CurrentStageId = newStage;

            fu.toSave.applicationTrackDto.ApplicationId = fu.toUpdate.applicationDto.Id;
            fu.toSave.applicationTrackDto.NextStageId = newStage;
            fu.toUpdate.applicationDto.AppLastUpdateDate = DateTime.Now;

            if (app.StateId != returned_state)
            {
                fu.toUpdate.applicationDto.ApplicationDate = DateTime.Now;
                fu.toUpdate.applicationDto.ApplicationNo = APPLICATION_NO_PREFIX + DateTime.Now.Year.ToString() + "_" + fu.toUpdate.applicationDto.Id.ToString().PadLeft(7, '0');
            }
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            ApiResult = await SendData(fu, userId, lang);
            if (!ApiResult.Result)
            {
                return ApiResult;
            }

            var notification = await _INotificationSettingRepository.GetNotificationsForAction(actionId);
            List<int> party = new List<int>();
            party.Add(userId);

            foreach (var n in notification)
            {
                n.ApplicationId = (int)fu.toUpdate.applicationDto.Id;
            }
            int notiResponse = await NotifyParties(party, notification);
            scope.Complete();
            ApiResult.Id = (int)fu.toUpdate.applicationDto.Id;
            ApiResult.Result = true;
            ApiResult.Code = Math.Abs(OK);
            ApiResult.Message.Clear();
            ApiResult.Message.Add(getMessage("ar", "Done"));
            return ApiResult;

        }
        //-------in Save Draft----------------------
        public async Task<APIResult> FullUpdateAndStay(FullUpdate fu, int userId, string state, string lang)
        {

            APIResult ApiResult = null;
            int StateId = await _ISysValueRepository.GetIdByShortcut(state);
            fu.toUpdate.applicationDto.StateId = StateId;
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            ApiResult = await SendData(fu, userId, lang);
            if (!ApiResult.Result)
            {
                return ApiResult;
            }
            scope.Complete();
            ApiResult.Id = (int)fu.toUpdate.applicationDto.Id;
            ApiResult.Code = Math.Abs(OK);
            ApiResult.Result = true;
            ApiResult.Message.Add(getMessage("ar", "Done"));
            return ApiResult;

        }
        //-----RETURN------------
        public async Task<APIResult> BackToFirstStageNoti(FullUpdate fu, int userId, string lang)
        {
            APIResult ApiResult = new APIResult();
            StagePayload sp = new StagePayload();
            sp.application = fu.toUpdate.applicationDto;
            sp.trackDto = fu.toSave.applicationTrackDto;

            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                ApiResult = await SetToFirstStage(sp, userId);
                var transactions = await _ITransactionRepository.GetAll((int)fu.toUpdate.applicationDto.Id);
                if (transactions.Count > 0)
                {
                    int transactionId = transactions[0].Id;
                    APIResult clearResult = await _IApplicationPartyRepository.ClearPartiesSignInfo(transactionId, userId);
                    if (clearResult.Id < 0)
                    {
                        return clearResult;
                    }
                }

                string MeetingBaseUrl = _IConfiguration["MeetingBaseUrl"], s;
                List<Receiver> rs = await GetReceiverPartyByAppID((int)fu.toUpdate.applicationDto.Id, false);
                List<Receiver> receivers = await AddUserReceiverData(rs);
                Dictionary<string, string> ParameterDic = new Dictionary<string, string>();
                ParameterDic.Add(APPLICATION_NUMBER_MOB, fu.toUpdate.applicationDto.Id.ToString());
                ParameterDic.Add(APPLICATION_NUMBER, fu.toUpdate.applicationDto.Id.ToString());
                int notify = await FillAndSendNotification(fu.notification, receivers, ParameterDic, (int)fu.toUpdate.applicationDto.ServiceId, (int)fu.toUpdate.applicationDto.Id, true, lang);

                ApiResult.Id = (int)fu.toUpdate.applicationDto.Id;
                ApiResult.Result = true;
                ApiResult.Code = 200;
                ApiResult.Message.Clear();
                ApiResult.Message.Add(getMessage("ar", "Done"));
                scope.Complete();
            }
            catch
            {
                ApiResult.Message.Add("خطأ في إرجاع الطلب ");
            }

            return ApiResult;
        }
        //-----  in Approval  Review--------------
        public async Task<APIResult> SigningNotiForward(FullUpdate fu, int actionId, int userId, string lang)
        {
            APIResult ApiResult = new APIResult();
            try
            {
                int newStage = await NextPreviewsStage((int)fu.toUpdate.applicationDto.ServiceId, (int)fu.toUpdate.applicationDto.CurrentStageId, 1);
                fu.toUpdate.applicationDto.StateId = await _ISysValueRepository.GetIdByShortcut(ONPROGRESS);
                fu.toSave.applicationTrackDto.StageId = fu.toUpdate.applicationDto.CurrentStageId;
                fu.toUpdate.applicationDto.CurrentStageId = newStage;
                fu.toUpdate.applicationDto.AppLastUpdateDate = DateTime.Now;
                fu.toUpdate.applicationDto.ApplicationNo = APPLICATION_NO_PREFIX + DateTime.Now.Year.ToString() + "_" + fu.toUpdate.applicationDto.Id.ToString().PadLeft(7, '0');
                fu.toSave.applicationTrackDto.ApplicationId = fu.toUpdate.applicationDto.Id;
                fu.toSave.applicationTrackDto.NextStageId = newStage;
                var responseNotification = fu.notification;

                using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                ApiResult = await SendData(fu, userId, lang);
                if (!ApiResult.Result)
                {
                    return ApiResult;
                }

                ServiceNamesDto serviceName = await _IAdmServiceRepository.GetOnename((int)fu.toUpdate.applicationDto.ServiceId, "ar");
                int kind = (int)serviceName.ServiceKind;
                List<Receiver> rs = await GetReceiverPartyByAppID((int)fu.toUpdate.applicationDto.Id, true);
                List<Receiver> receivers = await AddUserReceiverData(rs);
                dynamic schedule = await Schedule(fu.toUpdate.applicationDto.Id.ToString(), kind, fu.toSave.applicationTrackDto.UserId, (int)fu.toUpdate.applicationDto.ServiceId, serviceName.serviceName, DateTime.Now);

                // if(schedule!=null)  { 

                //--- replaced by next rows-- var notification = await _INotificationSettingRepository.GetNotificationsForAction(actionId);


                var notification = await getScheduleNotifications(responseNotification, receivers, schedule, (int)fu.toUpdate.applicationDto.ServiceId, (int)fu.toUpdate.applicationDto.Id, lang);
                await _ISendNotificationRepository.DoSend(notification, false);
                scope.Complete();

                ApiResult.Id = (int)fu.toUpdate.applicationDto.Id;
                ApiResult.Code = Math.Abs(OK);
                ApiResult.Result = true;
                ApiResult.Message.Add(getMessage(lang, "Done"));
                return ApiResult;

            }
            catch (Exception e)
            {
                ApiResult.Id = -1;
                ApiResult.Message.Add("لم تتم العملية بنجاح");
                _logger.LogInformation($"Approval  Review: {fu.toUpdate.applicationDto.Id}**{e.Message.ToString()}**{e.InnerException}**end Approval  Review*");
                return ApiResult;
            }
        }
        //-------------reject
        public async Task<APIResult> ChangeAppStateWithNoti(FullUpdate fu, string to, int actionId, int userId, string lang)
        {
            StagePayload sp = new StagePayload();
            sp.application = fu.toUpdate.applicationDto;
            sp.trackDto = fu.toSave.applicationTrackDto;
            APIResult ApiResult = new APIResult();
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            int i = await SetToState(sp, userId, to);
            if (to == REJECTED)
            {
                await EndAppMeetings((int)sp.application.Id);
            }
            if (i > 0)
            {

                List<Receiver> rs = await GetReceiverPartyByAppID((int)fu.toUpdate.applicationDto.Id, false);
                List<Receiver> receivers = await AddUserReceiverData(rs);
                int notify = await FillAndSendNotification(fu.notification, receivers, ParameterDic, (int)fu.toUpdate.applicationDto.ServiceId, (int)fu.toUpdate.applicationDto.Id, true, lang);

                ApiResult.Id = i;
                ApiResult.Result = true;
                ApiResult.Code = Math.Abs(OK);
                ApiResult.Message.Add(getMessage(lang, "Done"));
                scope.Complete();
            }

            else
            {
                ApiResult.Id = i;
                ApiResult.Message.Add(getMessage(lang, "ChangeStateFail"));
            }
            return ApiResult;
        }

        public async Task<APIResult> PostAllWithNotification(FullUpdate fu, FIERST_SAVE_STAGE toStage, int actionId, int userId, string lang)
        {

            string appUrl, appUrlMob;
            string AppBaseUrl = _IConfiguration["ApplicationBaseUrl"];
            ApplicationDto applicationDto = fu.toSave.applicationDto;
            List<ApplicationAttachmentDto> appAttachmentDtos = fu.toSave.applicationAttachmentDtos;
            List<ApplicationPartyWithExtraDto> appPartyDtos = fu.toSave.applicationPartyDtos;
            List<AppRelatedContentDto> appRelatedContentDtos = fu.toSave.relatedContentDtos;
            TransactionDto transactionDto = fu.toSave.transactionDto;
            ApplicationTrackDto appTrackDto = fu.toSave.applicationTrackDto;
            List<TargetApplicationDto> targetApplicationDtos = fu.toSave.targetApplicationDtos;
            // fu.toSave.applicationDto.ApplicationNo = APPLICATION_NO_PREFIX + DateTime.Now.Year.ToString() + "_" + fu.toUpdate.applicationDto.Id.ToString().PadLeft(7, '0');
            appTrackDto.Note = fu.toSave.applicationDto.Note.Length > 0 ? fu.toSave.applicationDto.Note : "تم إرسال الطلب";

            //  List<NotificationLogPostDto> notification = fullUpdate.notification;




            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            APIResult ApiResult = new APIResult();
            if (toStage == FIERST_SAVE_STAGE.REVIEW)
            {
                applicationDto.AppLastUpdateDate = DateTime.Now;
                //applicationDto.
            }

            try
            {
                ApiResult = await PostAllApplicationData_(applicationDto, targetApplicationDtos, appAttachmentDtos, appPartyDtos, appRelatedContentDtos, transactionDto, appTrackDto, toStage, userId, lang);
                string s = new JavaScriptSerializer().Serialize(fu.toSave);//JsonConvert.SerializeObject(fu.toSave);
            }
            catch (Exception e)
            {
                string s = JsonConvert.SerializeObject(fu);
                _logger.LogInformation("***new application****" + e.Message + " " + e.InnerException + " ** " + s);
                ApiResult.Message.Add(e.Message);
                return ApiResult;
            }

            if (ApiResult.Id > 0)
            {

                if (toStage == FIERST_SAVE_STAGE.REVIEW)
                {
                    List<Receiver> rs = new List<Receiver>();
                    foreach (ApplicationPartyWithExtraDto p in appPartyDtos)
                    {
                        if (p.PartyId != null  /*&& (bool)p.IsOwner*/)
                        {

                            Receiver receiver = new Receiver();
                            receiver.Id = (int)p.PartyId;
                            receiver.Mobile = p.Mobile;
                            receiver.Email = p.Email;
                            receiver.Name = p.FullName;
                            rs.Add(receiver);
                        }
                    }
                    List<Receiver> receivers = await AddUserReceiverData(rs);
                    try
                    {
                        var notification = await _INotificationSettingRepository.GetNotificationsForAction(actionId);


                        Dictionary<string, string> ParameterDic = new Dictionary<string, string>();
                        // appUrl = $"<a href='{AppBaseUrl}{fu.toSave.applicationDto.ServiceId}/{ ApiResult.Id.ToString()}'>{ApiResult.Id.ToString()}</a>";
                        //appUrlMob = $"{AppBaseUrl}{fu.toSave.applicationDto.ServiceId}/{ ApiResult.Id.ToString()}";
                        appUrl = ApiResult.Id.ToString();
                        appUrlMob = ApiResult.Id.ToString();
                        ParameterDic.Add(APPLICATION_NUMBER_MOB, appUrlMob);
                        ParameterDic.Add(APPLICATION_NUMBER, appUrl);
                        int notify = await FillAndSendNotification(notification, receivers, ParameterDic, (int)fu.toSave.applicationDto.ServiceId, ApiResult.Id, false, lang);
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation("***notification****" + e.Message + " " + e.InnerException);
                    }
                    scope.Complete();
                }
                else
                {
                    scope.Complete();
                }
            }
            return ApiResult;
        }

        public async Task<APIResult> PostAll(FullUpdate fu, FIERST_SAVE_STAGE toStage, int userId, string lang)
        {
            APIResult ApiResult;
            ApplicationDto applicationDto = fu.toSave.applicationDto;
            List<TargetApplicationDto> appTargetDto = fu.toSave.targetApplicationDtos;
            List<ApplicationAttachmentDto> appAttachmentDtos = fu.toSave.applicationAttachmentDtos;
            List<ApplicationPartyWithExtraDto> appPartyDtos = fu.toSave.applicationPartyDtos;
            List<AppRelatedContentDto> appRelatedContentDtos = fu.toSave.relatedContentDtos;
            TransactionDto transactionDto = fu.toSave.transactionDto;
            ApplicationTrackDto appTrackDto = fu.toSave.applicationTrackDto;
            appTrackDto.Note = "تم إرسال الطلب";
            // List<NotificationLogPostDto> notification = fu.notification;
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            ApiResult = await PostAllApplicationData_(applicationDto, appTargetDto, appAttachmentDtos, appPartyDtos, appRelatedContentDtos, transactionDto, appTrackDto, toStage, userId, lang);
            if (ApiResult.Id > 0)
            {
                scope.Complete();
            }
            return ApiResult;
        }
        //---------- The Approval stage -------------
        public async Task<APIResult> StageForward(FullUpdate fu, int actionId, int userId, string lang)
        {
            APIResult ApiResult = new APIResult();
            StagePayload sp = new StagePayload();
            sp.application = fu.toUpdate.applicationDto;
            sp.application.AppLastUpdateDate = DateTime.Now;
            sp.trackDto = fu.toSave.applicationTrackDto;

            if (!IsSignedApp((int)fu.toUpdate.applicationDto.Id).Result)
            {
                ApiResult.Message.Add(getMessage(lang, "notSigned"));
                return ApiResult;
            }
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var result = await SetStageForward(sp, userId);
            if (fu.toUpdate.transactionDto != null)
            {
                await _ITransactionRepository.Update(fu.toUpdate.transactionDto.Id, userId, fu.toUpdate.transactionDto);
            }
            if (fu.toUpdate.relatedContentDtos != null && fu.toUpdate.relatedContentDtos.Count > 0)
            {
                foreach (var rd in fu.toUpdate.relatedContentDtos)
                {
                    await UpdateRContent(rd.Id, rd);
                }

            }
            if (fu.toSave.calculatedFeesDto != null)
            {
                ApiResult = await AddFees(fu.toSave.calculatedFeesDto, (int)fu.toUpdate.applicationDto.Id, actionId, userId, lang);



                if (!ApiResult.Result)
                {
                    return ApiResult;
                }
            }
            APIResult endMeetingResult = await EndAppMeetings((int)fu.toUpdate.applicationDto.Id);

            Dictionary<string, string> ParameterDic = new Dictionary<string, string>();
            string appUrl = fu.toUpdate.applicationDto.Id.ToString();
            string appUrlMob = fu.toUpdate.applicationDto.Id.ToString();
            ParameterDic.Add(APPLICATION_NUMBER_MOB, appUrlMob);
            ParameterDic.Add(APPLICATION_NUMBER, appUrl);

            List<Receiver> rs = await GetReceiverPartyByAppID((int)fu.toUpdate.applicationDto.Id, false);
            List<Receiver> receivers = await AddUserReceiverData(rs);
            int notify = await FillAndSendNotification(fu.notification, receivers, ParameterDic, (int)fu.toUpdate.applicationDto.ServiceId, (int)fu.toUpdate.applicationDto.Id, true, lang);

            scope.Complete();

            ApiResult = new APIResult();
            ApiResult.Id = result;
            ApiResult.Message.Clear();
            ApiResult.Message.Add(getMessage(lang, "Done"));
            return ApiResult;
        }


        //---------- The  -------------
        public async Task<APIResult> StageBackward(FullUpdate fu, int actionId, int userId, string lang)
        {
            APIResult ApiResult = new APIResult();
            StagePayload sp = new StagePayload();
            sp.application = fu.toUpdate.applicationDto;
            sp.application.AppLastUpdateDate = DateTime.Now;
            sp.trackDto = fu.toSave.applicationTrackDto;

            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var result = await SetStageBackward(sp, userId);
            if (fu.toUpdate.transactionDto != null)
            {
                await _ITransactionRepository.Update(fu.toUpdate.transactionDto.Id, userId, fu.toUpdate.transactionDto);
            }


            scope.Complete();

            ApiResult = new APIResult();
            ApiResult.Id = (int)sp.application.Id;
            ApiResult.Message.Clear();
            ApiResult.Message.Add(getMessage(lang, "Done"));
            return ApiResult;
        }

        public async Task<bool> IsParty(int AppId, int userId)
        {
            var userList = await GetPartyByAppID(AppId);
            return (userList.IndexOf(userId) > -1);
        }

        public async Task<List<UserApplication>> GetUserTransaction(int serviceId, int userId, string lang)
        {
            int StateId = await _ISysValueRepository.GetIdByShortcut(DONE);
            Task<List<UserApplication>> query = null;
            query = (
                    from ap in _EngineCoreDBContext.Application
                    join trs in _EngineCoreDBContext.AppTransaction
                         on ap.Id equals trs.ApplicationId
                    join pty in _EngineCoreDBContext.ApplicationParty
                         on trs.Id equals pty.TransactionId
                    join tem in _EngineCoreDBContext.Template
                         on ap.TemplateId equals tem.Id
                    join t in _EngineCoreDBContext.SysTranslation
                         on tem.TitleShortcut equals t.Shortcut

                    where ap.ServiceId == serviceId
                    where pty.PartyId == userId
                    where trs.TransactionNo != null
                    // where pty.IsOwner == true
                    /*
                     
                     query = (
                    from ap in _EngineCoreDBContext.Application
                    join trs in _EngineCoreDBContext.AppTransaction
                         on ap.Id equals trs.ApplicationId
                    join pty in _EngineCoreDBContext.ApplicationParty
                         on trs.Id equals pty.TransactionId
                    join tem in _EngineCoreDBContext.Template
                         on ap.TemplateId equals tem.Id
                         into result
                    from newTable in result.DefaultIfEmpty()

                    join t in _EngineCoreDBContext.SysTranslation
                         on newTable.TitleShortcut equals t.Shortcut
                     */



                    where t.Lang == lang
                    select new UserApplication
                    {
                        ApplicationId = ap.Id,
                        TransactionId = trs.Id,
                        Template = t.Value,
                        Owner = (bool)pty.IsOwner ? getMessage(lang, "Owner") : getMessage(lang, "Party"),
                        ApplicationDate = (DateTime)ap.ApplicationDate,
                        ExpireDate = (DateTime)trs.TransactionEndDate,
                        StartDate = (DateTime)trs.TransactionStartDate,
                        //Enabled =ap.StateId== StateId,
                        Enabled = _ITransactionRepository.GetTransactionStatus((int)trs.TransactionStatus, trs.TransactionNo, ap.Id, (DateTime)trs.TransactionStartDate, (DateTime)trs.TransactionEndDate, (bool)trs.UnlimitedValidity, lang).Result.IsValid,
                        Status = _ITransactionRepository.GetTransactionStatus((int)trs.TransactionStatus, trs.TransactionNo, ap.Id, (DateTime)trs.TransactionStartDate, (DateTime)trs.TransactionEndDate, (bool)trs.UnlimitedValidity, lang).Result.Status
                    }
                   ).ToListAsync();


            return await query;
        }

        public async Task<List<UserApplication>> GetUserApplication(int serviceId, int userId, string lang, List<int> AcceptableStageOrder, List<string> AcceptableState)
        {

            //  int StateId = await _ISysValueRepository.GetIdByShortcut("DONE");
            List<int> StagesIds = await _IAdmServiceRepository.GetStagesIds(serviceId);
            List<int> AcceptableStages = new List<int>();
            foreach (int i in AcceptableStageOrder)
            {
                int n = i - 1;
                if (n < StagesIds.Count)
                    AcceptableStages.Add(StagesIds[i - 1]);
            }
            Task<List<UserApplication>> query = null;
            query = (
                    from ap in _EngineCoreDBContext.Application
                    join trs in _EngineCoreDBContext.AppTransaction
                         on ap.Id equals trs.ApplicationId
                    join pty in _EngineCoreDBContext.ApplicationParty
                         on trs.Id equals pty.TransactionId
                    join tem in _EngineCoreDBContext.Template
                         on ap.TemplateId equals tem.Id
                    join t in _EngineCoreDBContext.SysTranslation
                         on tem.TitleShortcut equals t.Shortcut

                    where ap.ServiceId == serviceId
                    where pty.PartyId == userId
                    // where pty.IsOwner == true

                    where t.Lang == lang
                    select new UserApplication
                    {
                        ApplicationId = ap.Id,
                        TransactionId = trs.Id,
                        Template = t.Value,
                        Owner = (bool)pty.IsOwner ? getMessage(lang, "Owner") : getMessage(lang, "Party"),
                        ApplicationDate = (DateTime)ap.ApplicationDate,
                        ExpireDate = (DateTime)trs.TransactionEndDate,
                        StartDate = (DateTime)trs.TransactionStartDate,
                        //Enabled =ap.StateId== StateId,
                        Enabled = AcceptableStages.Contains((int)ap.CurrentStageId),
                        Status = _ITransactionRepository.GetTransactionStatus((int)trs.TransactionStatus, trs.TransactionNo, ap.Id, (DateTime)trs.TransactionStartDate, (DateTime)trs.TransactionEndDate, (bool)trs.UnlimitedValidity, lang).Result.Status
                    }
                   ).ToListAsync();


            return await query;
        }


        public async Task<byte[]> ChangeLockStatus(int appId, int userId, ApplicationDto applicationDto, bool locked)
        {
            byte[] newRowVirsion;
            applicationDto.Locked = locked;
            try
            {
                APIResult ApiResult = new APIResult();
                ApiResult = await Update(appId, userId, applicationDto);
                newRowVirsion = ApiResult.Result;
                if (newRowVirsion == null)
                    return applicationDto.RowVersion;
                else
                    return newRowVirsion;
            }
            catch
            {
                return applicationDto.RowVersion;
            }

        }


        public async Task<byte[]> GetRowVersion(int id)
        {
            var query = await _EngineCoreDBContext.Application.Where(x => x.Id == id).Select(x => x.RowVersion).FirstOrDefaultAsync();
            return query;
        }

        public CreateFolderMessage CreateAppFolder(string serviceId, string appId)
        {
            string baseFolderName = _IConfiguration["BaseFolder"];
            CreateFolderMessage CFM = _IFilesUploaderRepository.CreateFolder(Path.Combine(baseFolderName, serviceId, appId));
            if (CFM.SuccessCreation)
            {
                string attFolder = _IConfiguration["ApplicationFileFolder"];
                CFM = _IFilesUploaderRepository.CreateFolder(Path.Combine(baseFolderName, serviceId, appId, attFolder));
                if (CFM.SuccessCreation)
                {
                    attFolder = _IConfiguration["PartyFileFolder"];
                    CFM = _IFilesUploaderRepository.CreateFolder(Path.Combine(baseFolderName, serviceId, appId, attFolder));
                    if (CFM.SuccessCreation)
                    {
                        attFolder = _IConfiguration["TransactionFolder"];
                        CFM = _IFilesUploaderRepository.CreateFolder(Path.Combine(baseFolderName, serviceId, appId, attFolder));
                    }
                }
            }

            return CFM;
        }

        public void DeleteAppFolder(string serviceId, string appId)
        {
            string baseFolderName = _IConfiguration["BaseFolder"];
            _IFilesUploaderRepository.DeleteFolder(Path.Combine(baseFolderName, serviceId, appId));

        }

        public bool MoveAppAttachment(string source, string destination)
        {
            //string baseFolderName = _IConfiguration["BaseFolder"];
            //string realDestination = Path.Combine(baseFolderName, destination);
            return _IFilesUploaderRepository.CopyFile(source, destination);
        }


        public async Task<dynamic> GetNotificationsByAction(int actionId, string lang, int serviceId, int stageId, int appId)
        {
            var s = await _IAdmServiceRepository.GetOne((int)serviceId);
            Dictionary<string, string> ParameterDic = new Dictionary<string, string>();
            try
            {
                //dynamic decision = null;
                /*if (s.ApprovalStage == stageId)decision = new {lable = getMessage(lang, "Approval"), decision_text = s.ApprovalText};*/

                var notification = await _INotificationSettingRepository.GetNotificationsForAction(actionId);

                string ApplicationBaseUrl = _IConfiguration["ApplicationBaseUrl"];
                //string appUrl = $"<a href='{ApplicationBaseUrl}/{appId}'>{appId}</a>";
                //string appUrlMob = $"{ApplicationBaseUrl}/{appId}";
                string appUrl = appId.ToString();
                string appUrlMob = appId.ToString();
                ParameterDic.Add(APPLICATION_NUMBER_MOB, appUrlMob);
                ParameterDic.Add(APPLICATION_NUMBER, appUrl);

                if (notification[0].NotificationBody.Contains("@Expected"))
                {
                    PickTicket pickTicket = await _IQueueRepository.PickTicket(appId.ToString(), 8, _IUserRepository.GetUserID(), DateTime.Now, false);
                    ParameterDic.Add(EXPECTED_DATE, pickTicket.ExpectDateTime.ToString("dd/MM/yyyy"));
                    ParameterDic.Add(EXPECTED_TIME, pickTicket.ExpectDateTime.ToString("hh:mm tt"));
                }


                foreach (var n in notification)
                {
                    n.NotificationBody = ReplaceParemeterByValues(ParameterDic, n.NotificationBody);
                }

                return new
                {
                    notification = notification,
                    channel = await _ISysValueRepository.GetTypeAll(lang, "notification_channel"),
                    // decision = decision
                };
            }
            catch (Exception)
            {
                return ERROR;
            }
        }

        public async Task<dynamic> GetInitialData(int serviceId, string lang, int userId)
        {
            List<TypeList> DocumentTypes = null;
            List<TemplateView> Templates = null;
            List<UserApplication> userApplication = null;
            List<TypeList> reasons = null;
            /*List<int> svs = new List<int>();
            svs.Add(1030);
            svs.Add(1035);
            svs.Add(1034);
            svs.Add(3107);
            svs.Add(1022); svs.Add(1025);
            if (!svs.Contains(serviceId))
            {
                APIResult ApiResult = new APIResult();
                ApiResult.Message.Add(getMessage(lang, "NAvailableService"));
                return ApiResult;
            }*/
            ServiceNamesDto serviceNames = await _IAdmServiceRepository.GetOnename(serviceId, lang);

            if (serviceNames == null)
            {
                APIResult ApiResult = new APIResult();
                ApiResult.Message.Add(getMessage(lang, "ServiceNotFound"));
                return ApiResult;
            }

            //--------------Target Service--------------------

            // AdmService TargetServices= await _IAdmServiceRepositiory.GetOne(serviceId);
            // List<int> a = TargetServices.TargetServiceService.ToList().Select(x => x.TargetServiceId).ToList();

            //-------------------------------------------------

            if ((bool)serviceNames.Templated)
            {
                DocumentTypes = await _ISysValueRepository.GetTypeAll(lang, "document_type");
                Templates = await _ITemplateRepository.GetTemplateNames(0, lang);

                //if (serviceNames.DocumentTypeId!=null)
                // {

                var query = await _EngineCoreDBContext.AdmServiceDocumentType.Where(s => s.ServiceId == serviceId).Select(x => x.DocumentTypeId).ToListAsync();
                if (query != null && query.Count > 0)
                {
                    DocumentTypes = DocumentTypes.Where(x => query.Contains(x.Id)).ToList();

                }
                //DocumentTypes = DocumentTypes.Where(x => x.Id == serviceNames.DocumentTypeId).ToList();
                // }
                /*  else
                  if (serviceNames.TemplateId != null)
                 {
                     Templates = Templates.Where(y => y.Id == serviceNames.TemplateId).ToList();
                     DocumentTypes = DocumentTypes.Where(x => x.Id == Templates.First().DocumentTypeId).ToList();
                 }*/

            }

            else
            {
                int targetService;
                if (serviceNames.TargetService != null)
                {
                    targetService = (int)serviceNames.TargetService;

                    if (serviceNames.ShowApplication != null && (bool)serviceNames.ShowApplication)
                    {
                        //if((bool)serviceNames.ShowApplication)
                        //  { 
                        List<int> l = new List<int>();
                        l.Add(2);
                        l.Add(3);
                        userApplication = await GetUserApplication(targetService, userId, lang, l, null);
                        //}
                    }
                    else
                    {
                        if (serviceNames.ShowTransaction != null)
                        {
                            if ((bool)serviceNames.ShowTransaction)
                            {
                                userApplication = await GetUserTransaction(targetService, userId, lang);
                            }
                        }

                    }
                }

            }

            /* if ((bool)serviceNames.HasReason)
             {
                 reasons = await _ISysValueRepository.GetTypeAll(lang, "service_reason");
             }*/

            int firstStage = await _IAdmServiceRepository.FirstStage(serviceId, 0);
            var actionRole = await _IUserRepository.GetUserActionsPermissions(userId);
            List<ActionOfStage> ActionButtons = await getActions(firstStage, lang);
            var RoleActionButtons = ActionButtons.Where(x => actionRole.Contains(x.ActionId)).ToList();
            var result = new
            {
                Service = serviceNames,
                Stages = await getStageOfService(0, (int)serviceNames.Id, lang),
                ActionButton = RoleActionButtons,
                Nationality = await _ISysValueRepository.GetAllCountry(lang),
                DocumentTypes = DocumentTypes,
                Templates = Templates,
                UserApplication = userApplication,
                StageAttachments = await _IAdmStageRepository.GetRelatedAttachments(firstStage, lang),
                reasons = reasons
            };
            return result;

        }

        public async Task<dynamic> GetRequiredData(string lang)
        {
            var result = new
            {
                PartyType = await _ISysValueRepository.GetTypeAll(lang, "party_type"),
                AttachmentType = await _ISysValueRepository.GetTypeAll(lang, "attachment_type"),
                MaritalStatus = await _ISysValueRepository.GetTypeAll(lang, "marital_status"),
                Nationality = await _ISysValueRepository.GetAllCountry(lang),
                Gender = await _ISysValueRepository.GetTypeAll(lang, "gender"),
            };
            return result;
        }

        public async Task<List<Receiver>> AddUserReceiverData(List<Receiver> receivers)
        {
            List<Receiver> rs = new List<Receiver>();
            rs.AddRange(receivers);
            foreach (Receiver r in receivers)
            {
                UserDto user = await _IUserRepository.GetOne((int)r.Id);
                if (user != null)
                {
                    if (user.Email != r.Email || user.PhoneNumber != r.Mobile)
                    {
                        Receiver receiver2 = new Receiver();
                        receiver2.Id = (int)r.Id;
                        receiver2.Mobile = user.PhoneNumber != r.Mobile ? user.PhoneNumber : null;
                        receiver2.Email = user.Email != r.Email ? user.Email : null;
                        receiver2.Name = r.Name;
                        rs.Add(receiver2);
                    }
                }
            }
            return rs;

        }

        public async Task<APIResult> RefreshReadingDate(int id, int userId)
        {
            APIResult ApiResult = new APIResult();
            var app = await GetOne(id);
            ApplicationDto applicationDto = FromObjectToDto(app);
            applicationDto.LastReadDate = DateTime.Now;
            applicationDto.LastReader = userId;
            applicationDto.Locked = true;
            ApiResult = await Update(id, -1, applicationDto);
            return ApiResult;
        }

        public async Task<APIResult> IfBookedUp(int appId, int userId, string lang)
        {
            APIResult ApiResult = new APIResult();
            var app = await _EngineCoreDBContext.Application.Where(x => x.Id == appId).FirstOrDefaultAsync();
            if (app != null)
            {
                //    if (app.Locked != true){ApiResult.Id = 1;ApiResult.Result = AVAILABLE;return ApiResult;}
                if (app.LastReader == userId || app.LastReadDate == null)
                {
                    ApiResult.Id = 1;
                    ApiResult.Result = AppStatus.AVAILABLE;
                    return ApiResult;
                }
                bool b = DateTime.Now.Subtract((DateTime)app.LastReadDate).TotalSeconds < LOCK_SECONDS_TIME;
                if (b)
                {
                    // string userName = await _EngineCoreDBContext.User.Where(u => u.Id == app.LastReader).Select(x => x.FullName).FirstOrDefaultAsync();
                    ApiResult.Result = AppStatus.BOOKEDUP;
                    ApiResult.Id = -1;
                    ApiResult.Message.Add(getMessage(lang, "bookedApp"));
                    //ApiResult.Message.Add(userName);
                    return ApiResult;
                }
                else
                {
                    ApiResult.Id = 1;
                    ApiResult.Result = AppStatus.AVAILABLE;
                    return ApiResult;
                }

            }
            else
            {
                ApiResult.Result = AppStatus.UNAVAILABLE;
                ApiResult.Message.Add(getMessage(lang, "AppNotFound"));
                return ApiResult;

            }
        }

        public async Task<APIResult> UpdateRContent(int id, AppRelatedContentDto appRelatedContent)
        {
            AppRelatedContent content = await GetOneRelatedContent(id);
            APIResult ApiResult = new APIResult();

            if (content == null)
                return null;
            try
            {
                content.AppId = appRelatedContent.AppId;
                content.TitleShortcut = appRelatedContent.TitleShortcut;
                content.Content = appRelatedContent.Content;
                content.ContentUrl = appRelatedContent.ContentUrl;

                _IGeneralRepository.Update(content);
                if (await _IGeneralRepository.Save())
                {
                    ApiResult.Id = id;
                    ApiResult.Result = true;
                    ApiResult.Code = Math.Abs(OK);
                }
            }

            catch (Exception e)
            {
                ApiResult.Message.Add("خطأ في تعديل المحضر");
                var a = e.Message;
                var t = e.InnerException;
            }
            return ApiResult;
        }

        public async Task<AppRelatedContent> GetOneRelatedContent(int id)
        {
            var query = _EngineCoreDBContext.AppRelatedContent.Where(x => x.Id == id);
            return await query.FirstOrDefaultAsync();
        }
        public async Task<TargetApplication> GetOneTargetApplication(int id)
        {
            var query = _EngineCoreDBContext.TargetApplication.Where(x => x.Id == id);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<RelatedContentView>> GetAppRelatedContents(int id, string lang)
        {
            Task<List<RelatedContentView>> query = null;
            query = (
                     from arc in _EngineCoreDBContext.AppRelatedContent
                     join tr in _EngineCoreDBContext.SysTranslation
                         on arc.TitleShortcut equals tr.Shortcut

                     where arc.AppId == id
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



        async Task<bool> isBlocking(int stateId, bool block_target)
        {
            if (!block_target)
            {
                return false;
            }
            int doneStateId;
            doneStateId = await _ISysValueRepository.GetIdByShortcut(ONPROGRESS);
            return (stateId == doneStateId);
        }


        async Task<APIResult> SendData(FullUpdate fu, int userId, string lang)
        {
            APIResult ApiResult = new APIResult();
            ApiResult.Result = false;
            try
            {
                List<AppTransaction> t = await _ITransactionRepository.GetAll(fu.toUpdate.applicationDto.Id);

                int appTransactionId = t[0].Id;
                if (fu.toUpdate.transactionDto != null)
                {
                    fu.toUpdate.transactionDto.Id = appTransactionId;
                }
            }
            catch
            {
                ApiResult.Message.Add("حصل خطأ في البيانات");
                return ApiResult;
            }


            if (fu.toSave != null)
            {
                ApiResult = await SaveAllApplicationData(fu.toSave.applicationDto, fu.toSave.targetApplicationDtos, fu.toSave.applicationAttachmentDtos, fu.toSave.applicationPartyDtos, fu.toSave.applicationPartyExtraAttachmentWIds, fu.toSave.relatedContentDtos, fu.toSave.transactionDto, fu.toSave.applicationTrackDto, userId, lang);
                if (ApiResult.Id < 0)
                {
                    ApiResult.Message.Add(getMessage(lang, "AppAddFail"));
                    return ApiResult;
                }
            }
            if (fu.toUpdate != null)
            {
                ApiResult = await UpdateAllApplicationData(fu.toUpdate.applicationDto, fu.toUpdate.targetApplicationDtos, fu.toUpdate.applicationAttachmentDtos, fu.toUpdate.applicationPartyDtos, fu.toUpdate.applicationPartyExtraAttachmentWIds, fu.toUpdate.relatedContentDtos, fu.toUpdate.transactionDto, fu.toUpdate.applicationTrackDto, userId, lang);
                if (ApiResult.Id < 0)
                {
                    ApiResult.Message.Add(getMessage(lang, "AppUpdateFail"));
                    return ApiResult;
                }
            }
            if (fu.toDelete != null)
            {
                ApiResult = await DeleteRelatedApplicationData(fu.toDelete.Attachments, fu.toDelete.Parties, fu.toDelete.ExtraAttachments);
                if (ApiResult.Id < 0)
                {
                    ApiResult.Message.Add(getMessage(lang, "faildDelete"));
                    return ApiResult;
                }
            }
            ApiResult.Code = 200;
            ApiResult.Result = true;
            ApiResult.Id = 1;
            return ApiResult;
        }


        async Task<APIResult> AddFees(List<TransactionFeeOutput> t, int appId, int actionId, int userId, string lang)
        {

            APIResult result = new APIResult();
            if (t != null && t.Count > 0)
            {
                PaymentDetailsDto payDto = new PaymentDetailsDto()
                {
                    ActionId = actionId,
                    ApplicationId = appId,
                    UserId = userId,
                    FeeList = new List<FeesDto>()
                };

                foreach (var feeDto in t)
                {
                    FeesDto fee = new FeesDto()
                    {
                        ServiceMainCode = feeDto.PrimeClass,
                        ServiceSubCode = feeDto.SubCalss,
                        Quantity = feeDto.Quantity,
                        Price = feeDto.FeeValue,
                    };
                    payDto.FeeList.Add(fee);
                }
                try
                {
                    await _IPaymentRepository.AddApplicationFeesAsync(payDto, lang);
                    result.Id = 1;
                    result.Result = true;
                    result.Code = 200;
                    return result;
                }

                catch (ValidatorException e)
                {
                    result.Result = false;
                    result.Message = e.AttributeMessages;
                    return result;
                }

                catch (Exception e)
                {
                    result.Result = false;
                    result.Message.Add(e.Message);
                    return result;
                }
            }
            else
            {
                result.Id = 1;
                result.Result = true;
                result.Code = 200;
                return result;
            }
        }





        public async Task<sendMOADetailsMOJResponse> AddApplicationBashr(SendAppMOADetails_MOJ sendAppMOADetails_MOJ)
        {

            sendMOADetailsMOJResponse result = new sendMOADetailsMOJResponse
            {
                EODBTrackingNumber = sendAppMOADetails_MOJ.EODBTrackingNumber
            };

            if (sendAppMOADetails_MOJ.EODBTrackingNumber == null)
            {
                result.responseCode = "ERR102";
                result.responseDescription = " Invalid/Missing Parameter, EODBTrackingNumber is empty.";
                return result;
            }

            string newAppNo = APPLICATION_NO_G2G_PREFIX + sendAppMOADetails_MOJ.EODBTrackingNumber.Trim();
            if (await _EngineCoreDBContext.Application.AnyAsync(x => x.ApplicationNo == newAppNo))
            {
                result.responseCode = "ERR102";
                result.responseDescription = " Invalid/Missing Parameter, EODBTrackingNumber existed before.";
                return result;
            }

            if (sendAppMOADetails_MOJ.PaymentInformation.voucherReferenceNo == null)
            {
                sendAppMOADetails_MOJ.PaymentInformation.voucherReferenceNo = "no VoucherReferenceNo available";
            }

            if (sendAppMOADetails_MOJ.CompanyInformation.tradeNameEn == null || sendAppMOADetails_MOJ.CompanyInformation.tradeNameAr == null)
            {
                result.responseCode = "ERR102";
                result.responseDescription = " Invalid/Missing Parameter, TradeName is empty";
                return result;
            }

            if (sendAppMOADetails_MOJ.PaymentInformation.paymentMode == null || sendAppMOADetails_MOJ.PaymentInformation.totalFees == null || sendAppMOADetails_MOJ.PaymentInformation.paymentReferenceNumber == null)
            {
                result.responseCode = "ERR102";
                result.responseDescription = " Invalid/Missing Parameter, PaymentMode and TotalFees and PaymentReferenceNumber are mandatory";
                return result;
            }

            if (sendAppMOADetails_MOJ.MoaSignedCopy == null)
            {
                result.responseCode = "ERR102";
                result.responseDescription = " Invalid/Missing Parameter, MoaSignedCopy is empty";
                return result;
            }

            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var channel = await _IGeneralRepository.getShortCutId("G2G");
            if (channel == null || channel.id == 0)
            {
                result.responseCode = "ERR101";
                result.responseDescription = "No channel defined for G2G services.";
                return result;
            }

            var partyId = await _ISysValueRepository.GetIdByShortcut("G2GPARTY");
            if (partyId == -1)
            {
                result.responseCode = "ERR101";
                result.responseDescription = "No  defined type for G2G services.";
                return result;
            }

            var service = await _EngineCoreDBContext.AdmService.Where(x => x.UgId == Constants.UnifiedGateEditorConfirmServiceID).FirstOrDefaultAsync();
            if (service == null || service.Id == 0)
            {
                result.responseCode = "ERR101";
                result.responseDescription = "No service defined to accept applications from G2G services.";
                return result;
            }

            int currentStageId = 0;

            if (service != null)
            {
                string DoneTranslate = await _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stageDoneEN) && x.Shortcut.ToLower().Contains("syslookupvalue"))
                                                                        .Select(y => y.Shortcut).FirstOrDefaultAsync();
                int stageDone = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == DoneTranslate).Select(y => y.Id).FirstOrDefaultAsync();
                currentStageId = await _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId == stageDone && x.ServiceId == service.Id).Select(y => y.Id).FirstOrDefaultAsync();
            }

            if (currentStageId == 0)
            {
                result.responseCode = "ERR101";
                result.responseDescription = "No stages defined to accept applications from G2G services as done applications.";
                return result;
            }

            int StateId = await _ISysValueRepository.GetIdByShortcut(DONE);
            if (StateId == -1)
            {
                result.responseCode = "ERR101";
                result.responseDescription = "No Performed stage defined to accept applications from G2G services as done applications.";
                return result;
            }



            // 1. Add application.
            Application application = new Application
            {
                ApplicationNo = newAppNo,
                ApplicationDate = DateTime.Now,
                Note = " From G2G: Company Information: arName: " + sendAppMOADetails_MOJ.CompanyInformation.tradeNameAr + ", EnName: " + sendAppMOADetails_MOJ.CompanyInformation.tradeNameEn
                                                  + ", Duration: " + sendAppMOADetails_MOJ.CompanyInformation.companyDuration + ", CapitalAmount: " + sendAppMOADetails_MOJ.CompanyInformation.capitalAmount
                                                  + ", ShareValue: " + sendAppMOADetails_MOJ.CompanyInformation.shareValue + ", NoOfShares: " + sendAppMOADetails_MOJ.CompanyInformation.noOfShares + ", EODBTrackingNumber: " + sendAppMOADetails_MOJ.EODBTrackingNumber,
                Locked = false,
                Channel = channel.id,
                ServiceId = service.Id,
                StateId = await _ISysValueRepository.GetIdByShortcut(DONE),
                AppLastUpdateDate = DateTime.Now,
                LastReadDate = DateTime.Now,
                CurrentStageId = currentStageId,
                LastUpdatedDate = DateTime.Now
            };

            await _EngineCoreDBContext.Application.AddAsync(application);
            if (await _EngineCoreDBContext.SaveChangesAsync() <= 0)
            {
                result.responseCode = "ERR101";
                result.responseDescription = "Un-Expected Error";
                return result;
            }


            CreateFolderMessage CFM = CreateAppFolder(application.ServiceId.ToString(), application.Id.ToString());
            if (!CFM.SuccessCreation)
            {
                result.responseCode = "ERR101";
                result.responseDescription = "Failed in creating a container for the application.";
                return result;
            }

            byte[] bytes = sendAppMOADetails_MOJ.MoaSignedCopy;

            //Image image;   // in case receive as image.
            // using (MemoryStream ms = new MemoryStream(bytes))
            //  {
            //    image = Image.FromStream(ms);
            //  }
            // string destination = Path.Combine(_IConfiguration["BaseFolder"], service.Id.ToString(), application.Id.ToString(), sendAppMOADetails_MOJ.CompanyInformation.TradeNameEn + ".pdf");
            // image.Save(Path.Combine(_IFilesUploaderRepository.GetRootPath(), destination));

            var fileName = sendAppMOADetails_MOJ.EODBTrackingNumber.Trim() + ".pdf";
            var destPathWithName = Path.Combine(_IConfiguration["BaseFolder"], service.Id.ToString(), application.Id.ToString(), "transactions", fileName);
            try
            {
                string destination = Path.Combine(_IFilesUploaderRepository.GetRootPath(), destPathWithName);
                await System.IO.File.WriteAllBytesAsync(destination, bytes);
            }
            catch (Exception ex)
            {
                result.responseCode = "ERR101";
                result.responseDescription = "Failed in adding the moaSignedCopy to Enotary system " + ex.Message;
                _IFilesUploaderRepository.DeleteFolder(Path.Combine(_IConfiguration["BaseFolder"], service.Id.ToString(), application.Id.ToString()));
                return result;
            }

            // 2. Add transaction.
            TransactionDto transactionDto = new TransactionDto
            {
                ApplicationId = application.Id,
                UnlimitedValidity = true,
                TransactionCreatedDate = DateTime.Now,
                DocumentUrl = destPathWithName,
                FileName = fileName,
                Content = null,
                DecisionText = null,
                QrCode = null,

                TransactionNo = Constants.TRANSACTION_NO_G2G_PREFIX + DateTime.Now.Year.ToString() + "_" + sendAppMOADetails_MOJ.EODBTrackingNumber.Trim(),
                TransactionStatus = StateId
            };

            int transactionId = await _ITransactionRepository.Add(transactionDto, null);
            if (transactionId == Constants.ERROR)
            {
                result.responseCode = "ERR101";
                result.responseDescription = "Un-Expected Error in adding transaction application";
                _IFilesUploaderRepository.DeleteFolder(Path.Combine(_IConfiguration["BaseFolder"], service.Id.ToString(), application.Id.ToString()));
                return result;
            }

            // 3. add parties.
            int firstUserID = 0;
            foreach (var part in sendAppMOADetails_MOJ.Records)
            {
                /* TODO unused user information
                var unstoredInfo = part.FullNameEn.Trim() + " " + part.PassportNumber.Trim() +
                                   part.NationalityCountryCode.Trim() + part.NationalityCountryDescAr.Trim() +
                                   part.NationalityCountryDescEn.Trim() + part.CityCode.Trim() +
                                   part.CityDescAr.Trim() + part.CityDescEn.Trim() +
                                   part.EmirateCode.Trim() + part.EmirateDescAr.Trim() + part.EmirateDescEn.Trim() +
                                   part.FlatNumber.Trim() + part.StreetName.Trim() + part.AreaDescAr.Trim() +
                                   part.AreaDescEn.Trim() + part.DateOfBirth.Trim() + part.CapitalSharePercentage.Trim() +
                                   part.BuildingName.Trim() + part.ProfitSharePercentage.Trim() + part.IsManager.Trim() +
                                   part.IsAuthorizedSignatory.Trim() + part.IsManager.Trim();
                 */

                if (part.fullNameAr == null || part.emiratesID == null || part.emailID == null || part.dateOfBirth == null)
                {
                    result.responseCode = "ERR102";
                    result.responseDescription = " Invalid/Missing Parameter, FullNameAr, EmiratesID, EmailID, DateOfBirth are mandatory.";
                    return result;
                }

                try
                {
                    var addr = new System.Net.Mail.MailAddress(part.emailID);
                    if (addr.Address == part.emailID)
                    {
                        // valid email do nothing.
                    }
                }
                catch
                {
                    result.responseCode = "ERR102";
                    result.responseDescription = " Invalid/Missing Parameter, Invalid EmailID" + part.emailID;
                    return result;
                }

                ApplicationPartyDto partyDto = new ApplicationPartyDto
                {
                    TransactionId = transactionId,
                    FullName = part.fullNameAr.Trim(),
                    EmiratesIdNo = part.emiratesID.Trim(),
                    Email = part.emailID.Trim(),
                    BirthDate = Convert.ToDateTime(part.dateOfBirth.Trim()),
                    Mobile = part.mobileNumber,
                    PartyTypeId = partyId,
                    IsOwner = false,

                    // TODO Nationality to lookup on country after knowing what the NationalityCountryCode means?
                    //  Nationality = part.NationalityCountryCode,                   
                    // MaritalStatus,
                    // Gender,
                };

                var apiResult = await _IApplicationPartyRepository.AddPartyToUser(partyDto, "en");
                if (apiResult.Id == 0 || apiResult.Result == false)
                {
                    result.responseCode = "ERR101";
                    var errMessage = apiResult.Message.Select(x => x.ToString() + "  ");
                    result.responseDescription = "Failed in adding the user to Enotary system" + errMessage;
                    _IFilesUploaderRepository.DeleteFolder(Path.Combine(_IConfiguration["BaseFolder"], service.Id.ToString(), application.Id.ToString()));
                    return result;
                }

                partyDto.PartyId = apiResult.Id;
                if (firstUserID == 0)
                {
                    firstUserID = apiResult.Id;
                }


                var res = await _IApplicationPartyRepository.Add(partyDto, null);
                if (res == Constants.ERROR)
                {
                    result.responseCode = "ERR101";
                    result.responseDescription = String.Format("Failed in joining the user {0} to the application", part.fullNameEn);
                    _IFilesUploaderRepository.DeleteFolder(Path.Combine(_IConfiguration["BaseFolder"], service.Id.ToString(), application.Id.ToString()));
                    return result;
                }
            }

            // add application payment with details.
            try
            {
                List<Models.PaymentDetails> payDetails = new List<PaymentDetails>();
                foreach (var paidFee in sendAppMOADetails_MOJ.FeesRecords)
                {
                    Models.PaymentDetails feesDto = new Models.PaymentDetails
                    {
                        ServiceMainCode = "145307",
                        ServiceSubCode = paidFee.serviceCode,
                        Price = Convert.ToDouble(paidFee.serviceFees),
                        Quantity = 1
                    };
                    payDetails.Add(feesDto);
                }

                Models.Payment payment = new Models.Payment
                {
                    //  PaymentDate = Convert.ToDateTime(sendAppMOADetails_MOJ.PaymentInformation.paymentDate.Trim()),
                    // TotalAmount = Convert.ToDouble(sendAppMOADetails_MOJ.PaymentInformation.totalFees.Trim()),
                    PaymentDate = sendAppMOADetails_MOJ.PaymentInformation.paymentDate,
                    TotalAmount = sendAppMOADetails_MOJ.PaymentInformation.totalFees,
                    Status = Constants.SuccessfulCodeStatus,
                    ServiceId = service.Id,
                    ReceiptNo = "PAY_RECEIPTNO_G2G" + _IGeneralRepository.GetNextSecForPayment(),
                    //TransactionResponseDate = Convert.ToDateTime(sendAppMOADetails_MOJ.PaymentInformation.paymentDate.Trim()),
                    TransactionResponseDate = sendAppMOADetails_MOJ.PaymentInformation.paymentDate,
                    //NumOfCopies = 0,
                    PaymentSource = "G2G",
                    PaymentType = "G2G",
                    ApplicationId = application.Id,
                    InvoiceNo = "G2G_PAY" + application.Id + sendAppMOADetails_MOJ.PaymentInformation.paymentReferenceNumber.Trim(),
                    LastUpdatedDate = DateTime.Now,
                    PaymentDetails = payDetails,
                    PaymentMethodType = sendAppMOADetails_MOJ.PaymentInformation.paymentMode.Trim(),
                    PaymentStatus = Constants.SuccessfullPaymentStatus,
                    StatusMessage = "payed by G2G, PaymentStatus no is " + sendAppMOADetails_MOJ.PaymentInformation.voucherReferenceNo.Trim(),
                    CreatedDate = DateTime.Now
                };

                await _EngineCoreDBContext.Payment.AddAsync(payment);
                if (await _EngineCoreDBContext.SaveChangesAsync() == 0)
                {
                    result.responseCode = "ERR101";
                    result.responseDescription = "Failed in adding the payment with details to Enotary system";
                    _IFilesUploaderRepository.DeleteFolder(Path.Combine(_IConfiguration["BaseFolder"], service.Id.ToString(), application.Id.ToString()));
                }
            }
            catch (Exception ex)
            {
                result.responseCode = "ERR101";
                result.responseDescription = "Un-Expected Error Failed in adding payment" + ex.Message;
                _IFilesUploaderRepository.DeleteFolder(Path.Combine(_IConfiguration["BaseFolder"], service.Id.ToString(), application.Id.ToString()));
                return result;
            }


            // result.VoucherReferenceNo = sendAppMOADetails_MOJ.PaymentInformation.voucherReferenceNo.Trim();
            result.responseCode = "SUC100";
            result.responseDescription = "Success";

            scope.Complete();
            return result;
        }



        async Task<List<NotificationLogPostDto>> getScheduleNotifications(List<NotificationLogPostDto> responseNotification, List<Receiver> receivers, dynamic schedule, int serviceId, int appId, string lang)
        {
            string MeetingBaseUrl = _IConfiguration["MeetingBaseUrl"], s;
            string ApplicationBaseUrl = _IConfiguration["ApplicationBaseUrl"];
            string appUrl = $"<a href='{ApplicationBaseUrl}/{appId.ToString()}'>{appId.ToString()}</a>";
            string appUrlMob = $"{ApplicationBaseUrl}/{appId.ToString()}";
            Dictionary<string, string> ParameterDic = new Dictionary<string, string>();

            ParameterDic.Add(EXPECTED_TIME, schedule.StartTime);
            ParameterDic.Add(EXPECTED_DATE, schedule.StartDate);
            ParameterDic.Add(APPLICATION_NUMBER_MOB, appUrlMob);
            ParameterDic.Add(APPLICATION_NUMBER, appUrl);
            List<string> TokenUrls = new List<string>();
            foreach (var n in responseNotification)
            {
                n.ApplicationId = appId;
                n.NotificationBody = ReplaceParemeterByValues(ParameterDic, n.NotificationBody);
            }
            foreach (Receiver r in receivers)
            {
                s = await _ISendNotificationRepository.GenerateUrlToken(r.Id, serviceId, appId, lang);
                s = $"{MeetingBaseUrl}{s}";
                TokenUrls.Add(s);
            }
            List<NotificationLogPostDto> notification = await BuildNotificationObjectFromResponseByUser(responseNotification, receivers, TokenUrls);
            return notification;
        }
        async Task<bool> IsTargetAppExist(int appId, int? targetAppId, string targetAppDocument)
        {
            var count = await _EngineCoreDBContext.TargetApplication.Where(x => x.AppId == appId && x.TargetAppId == targetAppId /*&& x.TargetAppDocument== targetAppDocument*/).ToListAsync();
            if (count.Count > 0)
                return true;
            else
                return false;
        }

        public async Task<List<UserApplication>> GetUserTransaction(ICollection<TargetService> targetServices, int userId, string lang)
        {
            List<int?> targetServiceIds = targetServices.Select(s => s.TargetServiceId).ToList();
            List<int?> targetDocuments = targetServices.Select(s => s.TargetDocumentTypeId).ToList();
            int StateId = await _ISysValueRepository.GetIdByShortcut(DONE);
            Task<List<UserApplication>> query = null;
            query = (
                    from ap in _EngineCoreDBContext.Application
                    join trs in _EngineCoreDBContext.AppTransaction
                         on ap.Id equals trs.ApplicationId
                    join pty in _EngineCoreDBContext.ApplicationParty
                         on trs.Id equals pty.TransactionId
                    join tem in _EngineCoreDBContext.Template
                         on ap.TemplateId equals tem.Id
                         into result
                    from newTable in result.DefaultIfEmpty()

                    join t in _EngineCoreDBContext.SysTranslation
                         on newTable.TitleShortcut equals t.Shortcut

                    where targetServiceIds.Contains(ap.ServiceId) && targetDocuments.Contains(newTable.DocumentTypeId)
                    where pty.PartyId == userId
                    where trs.TransactionNo != null
                    // where pty.IsOwner == true

                    where t.Lang == lang
                    select new UserApplication
                    {
                        ApplicationId = (int)ap.Id,
                        TransactionId = trs.Id,
                        Template = t.Value,
                        Owner = (bool)pty.IsOwner ? getMessage(lang, "Owner") : getMessage(lang, "Party"),
                        ApplicationDate = (DateTime)ap.ApplicationDate,
                        ExpireDate = (DateTime)trs.TransactionEndDate,
                        StartDate = (DateTime)trs.TransactionStartDate,
                        //Enabled =ap.StateId== StateId,
                        Enabled = _ITransactionRepository.GetTransactionStatus((int)trs.TransactionStatus, trs.TransactionNo, ap.Id, (DateTime)trs.TransactionStartDate, (DateTime)trs.TransactionEndDate, (bool)trs.UnlimitedValidity, lang).Result.IsValid,
                        Status = _ITransactionRepository.GetTransactionStatus((int)trs.TransactionStatus, trs.TransactionNo, ap.Id, (DateTime)trs.TransactionStartDate, (DateTime)trs.TransactionEndDate, (bool)trs.UnlimitedValidity, lang).Result.Status
                    }
                   ).ToListAsync();


            return await query;
        }



        public async Task<NotaryInfo> GetLastUpdaterNotary(int appId)
        {
            int userId = 0;
            bool notaryFound = false;
            NotaryInfo notary = new NotaryInfo();
            var app = await _EngineCoreDBContext.Application.Where(x => x.Id == appId).Include(z => z.ApplicationTrack).FirstOrDefaultAsync();
            app.ApplicationTrack = app.ApplicationTrack.OrderByDescending(x => x.CreatedDate).ToList();
            foreach (ApplicationTrack track in app.ApplicationTrack)
            {
                userId = (int)track.UserId;
                if (await _IUserRepository.IsEmployee(userId))
                {
                    notaryFound = true;
                    break;
                }

            }
            if (userId > 0 && notaryFound)
            {
                UserDto user = await _IUserRepository.GetOne(userId);
                notary.Id = user.Id;
                notary.FullName = user.FullName;
                notary.Address = user.Address;
                notary.SignUrl = user.Sign;
            }

            return notary;
        }

        public async Task<APIResult> freeApplication(int appId, int userId)
        {
            APIResult ApiResult = new APIResult();
            var app = await GetOne(appId);
            ApplicationDto applicationDto = FromObjectToDto(app);
            applicationDto.Locked = false;
            ApiResult = await Update(appId, userId, applicationDto);

            // Reset Queue and MeetingForApplication
            await _IQueueRepository.ChangeTicketStatusBackToPendingByProcessNo(_IUserRepository.GetUserID(), appId.ToString());
            await _IMeetingRepository.ChangeMeetingStatusBackToPendingByAppId(appId.ToString(), "en");

            return ApiResult;
        }

        public async Task<APIResult> NotifyWithTokenLink(int appId, int serviceId, int actionId, string lang)
        {
            APIResult ApiResult = new APIResult();
            string MeetingBaseUrl = _IConfiguration["MeetingBaseUrl"], s;
            try
            {
                List<Receiver> rs = await GetReceiverPartyByAppID(appId, true);
                List<Receiver> receivers = await AddUserReceiverData(rs);
                if (receivers.Count < 1)
                {
                    ApiResult.Message.Add("لايوجد أطراف مطلوبين للتوقيع, يرجى تحديد طرف واحد على الأقل");
                    return ApiResult;

                }
                var actionNotification = await _INotificationSettingRepository.GetNotificationsForAction(actionId);
                List<string> TokenUrls = new List<string>();
                Dictionary<string, string> ParameterDic = new Dictionary<string, string>();
                ParameterDic.Add(APPLICATION_NUMBER_MOB, appId.ToString());
                ParameterDic.Add(APPLICATION_NUMBER, appId.ToString());
                foreach (var n in actionNotification)
                {
                    n.ApplicationId = appId;
                    n.NotificationBody = ReplaceParemeterByValues(ParameterDic, n.NotificationBody);
                }
                foreach (Receiver r in receivers)
                {
                    s = await _ISendNotificationRepository.GenerateUrlToken(r.Id, serviceId, appId, lang);
                    s = $"{MeetingBaseUrl}{s}";
                    TokenUrls.Add(s);
                }
                List<NotificationLogPostDto> notification = await BuildNotificationObjectFromResponseByUser(actionNotification, receivers, TokenUrls);
                await _ISendNotificationRepository.DoSend(notification, false);
                ApiResult.Id = appId;
                ApiResult.Result = true;
                ApiResult.Code = 200;
                ApiResult.Message.Add(getMessage("ar", "OkNotifyParties"));

            }
            catch
            {
                ApiResult.Message.Add(getMessage("ar", "NoNotifyParties"));
            }
            return ApiResult;
        }

        public async Task<APIResult> RebuildPDFDocuments(FullUpdate fu, int userId, string lang)
        {
            APIResult result = new APIResult();
            NotaryInfo n = await GetLastUpdaterNotary((int)fu.toUpdate.applicationDto.Id);
            if (n.Id != userId)
            {
                result.Id = ERROR;
                result.Message.Add(Constants.getMessage(lang, "notAuthoraizedNotary"));
                return result;
            }

            var paymentStatus = await _IPaymentRepository.GetPaymentsInfoByAppId((int)fu.toUpdate.applicationDto.Id, lang);
            if (paymentStatus.PaymentStatus != PAYMENT_STATUS_ENUM.PAID)
            {
                result.Id = ERROR;
                result.Message.Add(Constants.getMessage(lang, "notPaidRegenerateAPP") + paymentStatus.PaymentStatusName);
                return result;
            }

            /* if(fu.toUpdate.applicationDto!=null)
            {
             if (DateTime.Now.Subtract((DateTime)fu.toUpdate.transactionDto.TransactionCreatedDate).TotalDays > TRANSACTION_EDIT_DAY)
              {
                result.Id = ERROR;
                result.Message.Add("لا يمكن تعديل الوثيقة بعد مرور يومين على انجازها");
                return result;

              };
            }*/


            GeneratorRepository _IGenerator = new GeneratorRepository(_EngineCoreDBContext, this, _IPaymentRepository, _IGeneralRepository, _FileNaming, _IConverter, _Pdfdocumentsetting, _IFilesUploaderRepository);

            // string tpath = Path.Combine(_IConfiguration["BaseFolder"], fu.toUpdate.applicationDto.ServiceId.ToString(), fu.toUpdate.applicationDto.Id.ToString(), _IConfiguration["TransactionFolder"]);
            string tpath = _IConfiguration["TransactionFolder"];
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {

                // move from pay stage to done if paid.
                var payStagesIds = await _IAdmStageRepository.GetPayStagesId();
                var application = await _EngineCoreDBContext.Application.Where(x => x.Id == (int)fu.toUpdate.applicationDto.Id).FirstOrDefaultAsync();
                if (payStagesIds.Contains((int)application.CurrentStageId))
                {
                    if (await MakeItDone((int)fu.toUpdate.applicationDto.Id, userId) == Constants.ERROR)
                    {
                        _exception.AttributeMessages.Add(String.Format("failed in changing the application {0} to performed, please ask the admin to fix.", fu.toUpdate.applicationDto.Id.ToString()));
                        _logger.LogInformation(" Error in makeItDone for application no " + fu.toUpdate.applicationDto.Id.ToString());
                        throw _exception;
                    }
                }
                else
                {
                    _logger.LogInformation(" Warning: attempt to RebuildPDFDocuments  " + fu.toUpdate.applicationDto.Id.ToString() + " current stage for the app is not at pay but at " + ((int)application.CurrentStageId).ToString());
                }

                fu.toUpdate.applicationDto.StateId = await _ISysValueRepository.GetIdByShortcut(DONE);
                string note = $" إعادة توليد الوثيقة :{fu.toSave.applicationTrackDto.Note}";
                result = await SendData(fu, userId, lang);
                if (result.Id > 0)
                {
                    var transactions = await _ITransactionRepository.GetAll((int)fu.toUpdate.applicationDto.Id);
                    if (transactions.Count > 0)
                    {

                        int transactionId = transactions[0].Id;

                        if (transactions[0].TransactionNo != null && transactions[0].TransactionNo.Length > 0 && transactions[0].DocumentUrl != null && transactions[0].DocumentUrl.Length > 0)
                        {
                            TransactionOldVersionDto dto = new TransactionOldVersionDto
                            {
                                TransactionId = transactions[0].Id,
                                TransactionCreatedDate = transactions[0].TransactionCreatedDate != null ? (DateTime)transactions[0].TransactionCreatedDate : System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                                DocumentUrl = transactions[0].DocumentUrl,
                                TransactionNo = transactions[0].TransactionNo,
                                Note = fu.toSave.applicationTrackDto.Note                              
                            };
                            int i = await _ITransactionRepository.AddOldVersion(dto, userId);
                        }

                        TransactionDto transactionDto = _ITransactionRepository.FromObjectToDto(transactions[0]);
                        transactionDto.LastUpdatedBy = userId;
                        transactionDto.TransactionNo = _ITransactionRepository.GenerateTransactionNo();
                        transactionDto.QrCode = null;

                        int u = await _ITransactionRepository.Update(transactions[0], userId, transactionDto);
                        if (u == OK)
                        {
                            AutoCreatePdfPaths paths = await _IGenerator.autoCreatePDFAsync(lang, (int)fu.toUpdate.applicationDto.Id, tpath);
                            if (paths.TransactionDoc != null && paths.TransactionDoc.Length > 0)
                            {
                                transactions = await _ITransactionRepository.GetAll((int)fu.toUpdate.applicationDto.Id);
                                string destination = Path.Combine(_IConfiguration["BaseFolder"], fu.toUpdate.applicationDto.ServiceId.ToString(), fu.toUpdate.applicationDto.Id.ToString(), paths.TransactionDoc);
                                bool m = MoveAppAttachment(paths.TransactionDoc, destination);
                                transactionDto.DocumentUrl = destination;
                                transactionDto.QrCode = transactions[0].Qrcode;
                                transactionDto.TransactionCreatedDate = DateTime.Now;
                                await _ITransactionRepository.Update(transactions[0], userId, transactionDto);
                            }

                            if (paths.RecordIdPaths.Count > 0)
                            {
                                foreach (KeyValuePair<int, string> entry in paths.RecordIdPaths)
                                {
                                    AppRelatedContent a = await GetOneRelatedContent(entry.Key);
                                    AppRelatedContentDto appdto = new AppRelatedContentDto
                                    {
                                        AppId = a.AppId,
                                        Content = a.Content,
                                        TitleShortcut = a.TitleShortcut,
                                        ContentUrl = entry.Value
                                    };

                                    await UpdateRContent(entry.Key, appdto);
                                }
                            }
                        }
                        else
                        {
                            result.Id = ERROR;
                            result.Result = false;
                            result.Message.Add("خطأ في تعديل الوثيقة");
                            _logger.LogInformation(" Error in generate application no " + fu.toUpdate.applicationDto.Id.ToString() + " by enotary id " + userId.ToString());
                            return result;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result.Id = ERROR;
                result.Result = false;
                result.Code = 500;
                result.Message.Add("خطأ في إنشاء الوثيقة");
                _logger.LogInformation(" Error in generate application no " + fu.toUpdate.applicationDto.Id.ToString() + e.Message + " " + e.InnerException);
                result.Message.Add(e.Message.ToString());
                return result;

            }


            //------------------

            List<Receiver> rs = await GetReceiverPartyByAppID((int)fu.toUpdate.applicationDto.Id, false);
            List<Receiver> receivers = await AddUserReceiverData(rs);
            Dictionary<string, string> ParameterDic = new Dictionary<string, string>();
            ParameterDic.Add(APPLICATION_NUMBER_MOB, fu.toUpdate.applicationDto.Id.ToString());
            ParameterDic.Add(APPLICATION_NUMBER, fu.toUpdate.applicationDto.Id.ToString());
            int notify = await FillAndSendNotification(fu.notification, receivers, ParameterDic, (int)fu.toUpdate.applicationDto.ServiceId, (int)fu.toUpdate.applicationDto.Id, true, lang);
            scope.Complete();
            //------------------


            result.Id = (int)fu.toUpdate.applicationDto.Id;
            result.Result = true;
            result.Code = 200;
            result.Message.Add("تمت العملية بنجاح");
            return result;

        }




        public async Task<APIResult> BuildPDFDocuments(int appId)
        {
            APIResult result = new APIResult();
            GeneratorRepository _IGenerator = new GeneratorRepository(_EngineCoreDBContext, this, _IPaymentRepository, _IGeneralRepository, _FileNaming, _IConverter, _Pdfdocumentsetting, _IFilesUploaderRepository);
            string tpath = _IConfiguration["TransactionFolder"];
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var app = await GetOne(appId);
                var transactions = await _ITransactionRepository.GetAll(appId);
                if (transactions.Count > 0)
                {
                    //if (transactions[0].TransactionNo != null && transactions[0].TransactionNo.Length > 0 && (transactions[0].DocumentUrl == null || transactions[0].DocumentUrl.Length == 0))
                    if (String.IsNullOrEmpty(transactions[0].DocumentUrl))
                    {
                        if (String.IsNullOrEmpty(transactions[0].TransactionNo))
                        {
                            transactions[0].TransactionNo = _ITransactionRepository.GenerateTransactionNo();
                            _EngineCoreDBContext.AppTransaction.Update(transactions[0]);
                            if (await _EngineCoreDBContext.SaveChangesAsync() == 0)
                            {
                                result.Id = ERROR;
                                result.Result = false;
                                result.Code = 500;
                                result.Message.Add("خطأ في حفط رقم المعاملة");
                                return result;
                            }
                        }

                        AutoCreatePdfPaths paths = await _IGenerator.autoCreatePDFAsync("ar", appId, tpath);
                        if (paths.TransactionDoc != null && paths.TransactionDoc.Length > 0)
                        {
                            string destination = Path.Combine(_IConfiguration["BaseFolder"], app.ServiceId.ToString(), appId.ToString(), paths.TransactionDoc);
                            bool m = MoveAppAttachment(paths.TransactionDoc, destination);
                            transactions[0].DocumentUrl = destination;
                            transactions[0].TransactionCreatedDate = DateTime.Now;

                            _EngineCoreDBContext.AppTransaction.Update(transactions[0]);
                            if (await _EngineCoreDBContext.SaveChangesAsync() == 0)
                            {
                                result.Id = ERROR;
                                result.Result = false;
                                result.Code = 500;
                                result.Message.Add("خطأ في إنشاء الوثيقة");
                                return result;
                            }
                        }
                    }
                    else
                    {
                        result.Id = ERROR;
                        result.Result = false;
                        result.Code = 500;
                        result.Message.Add("الملف موجود مسبقا");
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                result.Id = ERROR;
                result.Result = false;
                result.Code = 500;
                result.Message.Add("خطأ في إنشاء الوثيقة");
                result.Message.Add(e.Message.ToString());
                return result;

            }
            scope.Complete();
            result.Id = appId;
            result.Result = true;
            result.Code = 200;
            result.Message.Add("تمت العملية بنجاح");
            return result;
        }

        async Task<int> FillAndSendNotification(List<NotificationLogPostDto> notifications, List<Receiver> receivers, Dictionary<string, string> Parameters, int serviceId, int appId, bool addAppLink, string lang)
        {
            string MeetingBaseUrl = _IConfiguration["MeetingBaseUrl"], userToken;
            List<string> TokenUrls = new List<string>();
            foreach (var n in notifications)
            {
                n.ApplicationId = appId;
                n.NotificationBody = ReplaceParemeterByValues(Parameters, n.NotificationBody);
            }
            if (addAppLink)
            {
                foreach (Receiver r in receivers)
                {
                    userToken = await _ISendNotificationRepository.GenerateUrlToken(r.Id, serviceId, appId, lang);
                    userToken = $"{MeetingBaseUrl}{userToken}";
                    TokenUrls.Add(userToken);
                }
            }
            notifications = await BuildNotificationObjectFromResponseByUser(notifications, receivers, TokenUrls);

            try
            {

                await _ISendNotificationRepository.DoSend(notifications, false);
                return OK;
            }
            catch
            {
                return ERROR;
            }
        }



        //-------*** : BackToStage ----------------------

        public async Task<APIResult> SetToStage(StagePayload sp, int stageId, int userId, string state)
        {
            int returnedStateId;
            APIResult ApiResult = new APIResult();
            returnedStateId = await _ISysValueRepository.GetIdByShortcut(state);//RETURNED
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            if (returnedStateId > 0)
            {

                if (stageId > 0)
                {
                    sp.trackDto.StageId = sp.application.CurrentStageId;
                    sp.trackDto.NextStageId = stageId;
                    sp.trackDto.NoteKind= (short?)NoteKind.Returned;
                    sp.application.StateId = returnedStateId;
                    sp.application.CurrentStageId = stageId;
                    await _IApplicationTrackRepository.Add(sp.trackDto);
                    sp.application.AppLastUpdateDate = DateTime.Now;
                    ApiResult = await Update((int)sp.application.Id, userId, sp.application);

                    if (ApiResult.Result != null)
                    {
                        await EndAppMeetings((int)sp.application.Id);
                        ApiResult.Id = stageId;
                        ApiResult.Message.Add(getMessage("ar", "Done"));
                        ApiResult.Code = Math.Abs(OK);
                        scope.Complete();
                    }
                    return ApiResult;
                }
                else
                {
                    ApiResult.Id = ERROR;
                    ApiResult.Message.Add(getMessage("ar", "SetStageFail"));
                }
            }
            else
            {
                ApiResult.Message.Add(getMessage("ar", "SetStateFail"));
            }
            return ApiResult;
        }
        public async Task<APIResult> BackToApprovalStageNoti(FullUpdate fu, int userId, string lang)
        {
            APIResult ApiResult = new APIResult();
            StagePayload sp = new StagePayload();
            sp.application = fu.toUpdate.applicationDto;
            sp.trackDto = fu.toSave.applicationTrackDto;


            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var service = await _IAdmServiceRepository.GetOne((int)fu.toUpdate.applicationDto.ServiceId);
                int approvalStageId = (int)service.ApprovalStage;
                ApiResult = await SetToStage(sp, approvalStageId, userId, ONPROGRESS);

                List<AppTransaction> transactions = await _ITransactionRepository.GetAll(fu.toUpdate.applicationDto.Id);
                var t = await _ITransactionRepository.ChangeTransactionStatus(transactions[0].Id, userId, CANCELED);
                List<Receiver> rs = await GetReceiverPartyByAppID((int)fu.toUpdate.applicationDto.Id, false);
                List<Receiver> receivers = await AddUserReceiverData(rs);
                Dictionary<string, string> ParameterDic = new Dictionary<string, string>();
                ParameterDic.Add(APPLICATION_NUMBER_MOB, fu.toUpdate.applicationDto.Id.ToString());
                ParameterDic.Add(APPLICATION_NUMBER, fu.toUpdate.applicationDto.Id.ToString());
                int notify = await FillAndSendNotification(fu.notification, receivers, ParameterDic, (int)fu.toUpdate.applicationDto.ServiceId, (int)fu.toUpdate.applicationDto.Id, true, lang);

                ApiResult.Id = (int)fu.toUpdate.applicationDto.Id;
                ApiResult.Result = true;
                ApiResult.Code = 200;
                ApiResult.Message.Clear();
                ApiResult.Message.Add(getMessage("ar", "Done"));
                scope.Complete();
            }
            catch
            {
                ApiResult.Message.Add("خطأ في إرجاع الطلب ");
            }

            return ApiResult;
        }

        public async Task<APIResult> ClearRelatedPartiesSignInfo(ApplicationTrackDto appTrackDto, int userId, string lang)
        {
            APIResult result = new APIResult();
            appTrackDto.Note = "مسح بيانات التوقيع :" + appTrackDto.Note;
            appTrackDto.NoteKind= (short?)NoteKind.ClearSign;
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                int id = await _IApplicationTrackRepository.Add(appTrackDto);
                if (id < 0)
                {
                    result.Message.Add("خطأ في إضافة الملاحظات");
                    return result;
                }

                List<AppTransaction> appTransaction = await _ITransactionRepository.GetAll(appTrackDto.ApplicationId);
                int transactionId = appTransaction[0].Id;
                result = await _IApplicationPartyRepository.ClearPartiesSignInfo(transactionId, userId);
                if (result.Id < 0)
                {
                    return result;
                }
                scope.Complete();
            }
            catch
            {
                result.Id = -1;
                result.Message.Add("خطأ في مسح بيانات التوقيع");
            }
            return result;
        }

        public async Task<APIResult> OwnApplication(int appId, int userId, string lang)
        {
            APIResult result = new APIResult();
            Application app = await GetOne(appId);
            if (app == null)
            {
                result.Message.Add("الطلب غير موجود !");
                return result;
            }
            app.Owner = userId;
            _EngineCoreDBContext.Update(app);
            if (await _IGeneralRepository.Save())
            {
                result.Result = true;
                result.Id = userId;
                result.Code = 200;
            }
            else
            {
                result.Message.Add("خطأ في حجزالطلب");
                result.Result = false;
                result.Code = 500;
            }
            return result;
        }

        public async Task<APIResult> ReleaseApplication(int appId, int userId, string lang)
        {
            APIResult result = new APIResult();
            Application app = await GetOne(appId);
            if (app == null)
            {
                result.Message.Add("الطلب غير موجود !");
                return result;
            }
            if (app.Owner == null)
            {
                result.Message.Add("الطلب غير محجوز !");
                return result;
            }
            if (app.Owner != userId)
            {
                result.Message.Add("عملية غير مسموحة");
                return result;
            }

            app.Owner = null;
            _EngineCoreDBContext.Update(app);
            if (await _IGeneralRepository.Save())
            {
                result.Result = true;
                result.Id = userId;
                result.Code = 200;
            }
            else
            {
                result.Message.Add("خطأ في تحرير الطلب ");
                result.Result = false;

            }
            return result;
        }

        public async Task<List<UserApplication>> Search(SearchObject searchObject, string lang)
        {
            int StateId = await _ISysValueRepository.GetIdByShortcut(DONE);
            Task<List<UserApplication>> query = null;
            query = (
                    from ap in _EngineCoreDBContext.Application
                    join trs in _EngineCoreDBContext.AppTransaction
                         on ap.Id equals trs.ApplicationId
                    join pty in _EngineCoreDBContext.ApplicationParty
                         on trs.Id equals pty.TransactionId
                    join tem in _EngineCoreDBContext.Template
                         on ap.TemplateId equals tem.Id
                    join t in _EngineCoreDBContext.SysTranslation
                         on tem.TitleShortcut equals t.Shortcut


                    where trs.TransactionNo != null
                    where
                          (ap.ApplicationNo.Contains(searchObject.ApplicationNo) || trs.TransactionNo.Contains(searchObject.ApplicationNo) || searchObject.ApplicationNo == null)
                    /*(ap.ApplicationNo.Contains(searchObject.ApplicationNo) || searchObject.ApplicationNo == null)
                        ||(trs.TransactionNo.Contains(searchObject.ApplicationNo)|| searchObject.ApplicationNo == null)*/
                       && (pty.FullName.Contains(searchObject.FullName) || searchObject.FullName == null)
                       && (pty.Email == searchObject.Email || searchObject.Email == null)
                       //&& (trs.TransactionCreatedDate == searchObject.TransactionDate || searchObject.TransactionDate == null)

                       && t.Lang == lang
                    // && pty.IsOwner==true
                    orderby ap.Id

                    select new UserApplication
                    {
                        FullName = pty.FullName,
                        ApplicationId = ap.Id,
                        ApplicationNo = ap.ApplicationNo,
                        TransactionId = trs.Id,
                        TransactionNo = trs.TransactionNo,
                        DocumentUrl = trs.DocumentUrl,
                        Template = t.Value,
                        Owner = (bool)pty.IsOwner ? getMessage("ar", "Owner") : getMessage("ar", "Party"),
                        ApplicationDate = (DateTime)ap.ApplicationDate,
                        ExpireDate = (DateTime)trs.TransactionEndDate,
                        StartDate = (DateTime)trs.TransactionStartDate,
                        //Enabled =ap.StateId== StateId,
                        Enabled = _ITransactionRepository.GetTransactionStatus((int)trs.TransactionStatus, trs.TransactionNo, ap.Id, (DateTime)trs.TransactionStartDate, (DateTime)trs.TransactionEndDate, (bool)trs.UnlimitedValidity, lang).Result.IsValid,
                        Status = _ITransactionRepository.GetTransactionStatus((int)trs.TransactionStatus, trs.TransactionNo, ap.Id, (DateTime)trs.TransactionStartDate, (DateTime)trs.TransactionEndDate, (bool)trs.UnlimitedValidity, lang).Result.Status,
                        Existing = _IFilesUploaderRepository.FileExist(trs.DocumentUrl)
                    }
                   ).ToListAsync();

            var result = await query;
            /*  return result;*/
            var grouped = result.GroupBy(x => x.ApplicationId, (key, g) => g.OrderBy(e => e.Owner).First()).ToList();
            var a = (List<UserApplication>)grouped;
            return (List<UserApplication>)grouped;
        }

        public async Task<APIResult> NotifyLateAppsPartyies(List<ServiceApplication> serviceApplications)
        {

            int ActionToReject = _EngineCoreDBContext.AdmAction.Where(x => x.Shortcut.Contains("RejectLateApps")).Select(z => z.Id).FirstOrDefault();

            APIResult result = new APIResult();
            int partyCount = 0;

            List<Receiver> receivers = new List<Receiver>();
            Dictionary<string, string> ParameterDic = new Dictionary<string, string>
            {
                { APPLICATION_NUMBER_MOB, "" },
                { APPLICATION_NUMBER, "" }
            };

            List<NotificationLogPostDto> notifications = new List<NotificationLogPostDto>();



            foreach (ServiceApplication app in serviceApplications)
            {
                notifications = await _INotificationSettingRepository.GetNotificationsForAction(ActionToReject);
                receivers = await GetReceiverPartyByAppID(app.ApplicationId, true);
                partyCount += receivers.Count;
                ParameterDic[APPLICATION_NUMBER] = app.ApplicationId.ToString();
                ParameterDic[APPLICATION_NUMBER_MOB] = app.ApplicationId.ToString();
                int noti = await FillAndSendNotification(notifications, receivers, ParameterDic, app.ServiceId, app.ApplicationId, false, "ar");
            }
            result.Id = serviceApplications.Count;
            result.Result = partyCount;
            result.Message.Add($"عدد الطلبات المتأخرة : {serviceApplications.Count}");
            result.Message.Add($"عدد الأطراف المخطرين  : {partyCount}");

            return result;
        }

        public async Task<APIResult> isCertifiedTargetApp(int app_id, bool required_target)
        {

            Application app = await GetOne(app_id);
            if (app.TargetApplicationApp.Count == 0)
            {
                if (!required_target)
                {
                    APIResult certified = new APIResult(app_id, true, 200, null);
                    return certified;
                }
                else
                {
                    APIResult certified = new APIResult();
                    certified.Result = false;
                    certified.Message.Add("يرجى اختيار محرر من النظام ");
                    return certified;

                }
            }

            APIResult result = new APIResult();
            result.Result = app.TargetApplicationApp.Any(t => t.TargetAppId != null);
            if (!result.Result)
            {
                result.Message.Add("يرجى اختيار محرر من النظام ");
            }
            return result;
        }



        public async Task<int> AddAppObjectionParty(int customerUserId, int appId, string reason, string lang)
        {
            // add application objection.  TODO
            if (!await _EngineCoreDBContext.User.AnyAsync(x => x.Id == customerUserId))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }


            if (!await _EngineCoreDBContext.Application.AnyAsync(x => x.Id == appId))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "ApplicationNotFound"));
                throw _exception;
            }

            var transactions = await _ITransactionRepository.GetAll(appId);
            if (transactions.Count < 1)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "ApplicationNotFound") + " no transaction for this application");
                throw _exception;
            }

            int transactionId = transactions[0].Id;

            var partyInfo = await _EngineCoreDBContext.ApplicationParty.Where(x => x.PartyId == customerUserId && x.TransactionId == transactions[0].Id).FirstOrDefaultAsync();
            if (partyInfo == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "partyNotFound"));
                throw _exception;
            }


            var oldAppObjections = await _EngineCoreDBContext.ApplicationObjection.Where(x => x.ApplicationId == appId).ToListAsync();
            if (oldAppObjections != null && oldAppObjections.Count > 0)
            {
                int objectionNotesMaxCount = Constants.APPLICATION_OBJECTION_NOTES_MAX_COUNT;
                if (_IConfiguration["ObjectionApplicationNotesCount"] == null)
                {
                    _logger.LogInformation("Warning!!! ObjectionApplicationNotesCount is missing");
                }
                else
                {
                    bool success = int.TryParse(_IConfiguration["ObjectionApplicationNotesCount"], out int settingCount);
                    if (!success || settingCount < 1)
                    {
                        _logger.LogInformation("Warning ObjectionApplicationNotesCount is invalid number or < 1 ");
                    }
                    else
                    {
                        objectionNotesMaxCount = settingCount;
                    }
                }

                if (oldAppObjections.Count >= objectionNotesMaxCount)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "AppNotesExceeded"));
                    throw _exception;
                }
            }

            if (transactions[0].TransactionCreatedDate != null)
            {
                await _IWorkingTimeRepository.InitialaizeWorkingDic((DateTime)transactions[0].TransactionCreatedDate, DateTime.Now);
                var periodAppInMinutes = await _IWorkingTimeRepository.GetWorkingMinutesBetweenTwoDates((DateTime)transactions[0].TransactionCreatedDate, DateTime.Now);
                int objectionPeriodInMinutes = Constants.APPLICATION_OBJECTION_PERIOD_INMINUTES_DEFAULT;

                if (_IConfiguration["ObjectionApplicationPeriodInMinutes"] == null)
                {
                    _logger.LogInformation("Warning!!! ObjectionApplicationPeriodInMinutes is missing");
                }
                else
                {
                    bool success = int.TryParse(_IConfiguration["ObjectionApplicationPeriodInMinutes"], out int settingPeriod);
                    if (!success || settingPeriod < 1)
                    {
                        _logger.LogInformation("Warning ObjectionApplicationPeriodInMinutes is invalid number or < 1 minute");
                    }
                    else
                    {
                        objectionPeriodInMinutes = settingPeriod;
                    }
                }

                if (periodAppInMinutes > objectionPeriodInMinutes)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "AppNotesPeriodExceeded"));
                    throw _exception;
                }
            }
           

            AppObjectionDto appObjectionDto = new AppObjectionDto
            {
                ApplicationId = appId,
                FullName = partyInfo.FullName,
                Phone = partyInfo.Mobile,
                Reason = reason,
                Address = partyInfo.Address,
                Birthday = partyInfo.BirthDate,
                City = partyInfo.City,
                Email = partyInfo.Email,
                EmiratesId = partyInfo.EmiratesIdNo,
                Note = "party obj" + partyInfo.Id.ToString()
            };

            var res = await AddAppObjection(appObjectionDto, lang);

            // add Internal notary notification.  TODO
            var lastNotary = await GetLastUpdaterNotary(appId);
            if (lastNotary.Id > 0)
            {
                if (!await _IUserRepository.IsEmployee(lastNotary.Id))
                {
                    _exception.AttributeMessages.Add("no enotary for this application ask the admin.");
                    throw _exception;
                }
            }
            else
            {
                _exception.AttributeMessages.Add("no enotary for this application ask the admin..");
                throw _exception;
            }

            int internalChannel = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == Constants.NOTIFICATION_INTERNAL_CHANNEL).Select(x => x.Id).FirstOrDefaultAsync();
            if (internalChannel == 0)
            {
                _exception.AttributeMessages.Add("no configuration for internal channel ask the admin.");
                throw _exception;
            }

            NotificationLogPostDto notificationLogPostDto = new NotificationLogPostDto
            {
                ApplicationId = appId,
                CreatedDate = DateTime.Now,
                NotificationChannelId = internalChannel,
                NotificationBody = reason,
                NotificationTitle = Constants.APPLICATION_OBJECTION_TITLE + appId.ToString(),
                ToAddress = lastNotary.Id.ToString(),
                Lang = lang == "en" ? "en" : "ar",
                UserId = customerUserId
            };

            List<NotificationLogPostDto> notificationLogPostDtos = new List<NotificationLogPostDto>
            {
                notificationLogPostDto
            };

            await _ISendNotificationRepository.DoSend(notificationLogPostDtos, false);

            return res;
        }


        public async Task<int> AddAppObjection(AppObjectionDto appObjectionDto, string lang)
        {
            var application = await _EngineCoreDBContext.Application.Where(x => x.Id == appObjectionDto.ApplicationId).FirstOrDefaultAsync();

            if (application == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "ApplicationNotFound"));
                throw _exception;
            }

            if (_IConfiguration["BaseFolder"] == null || _IConfiguration["ObjectionFolder"] == null)
            {
                _exception.AttributeMessages.Add(" missed configuration for application or objection folder setting.");
                throw _exception;
            }

            if (appObjectionDto.Email != null && appObjectionDto.Email.Length > 0)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(appObjectionDto.Email);
                }
                catch
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "InvalidEmailFormat"));
                    throw _exception;
                }
            }


            if (appObjectionDto.FullName.Length < 6)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "MissedFullName"));
                throw _exception;
            }

            if (appObjectionDto.Reason.Length < 10)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "MissedAppReason"));
                throw _exception;
            }

            if (appObjectionDto.Reason.Length > 200)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "AppReasonAtMost"));
                throw _exception;
            }


            if (appObjectionDto.Attachments != null && appObjectionDto.Attachments.Count > 0)
            {
                foreach (var att in appObjectionDto.Attachments)
                {
                    if (!_IFilesUploaderRepository.FileExist(att))
                    {
                        _exception.AttributeMessages.Add(Constants.getMessage(lang, "FileNotFound") + "  " + att);
                        throw _exception;
                    }
                }
            }


            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {

                ApplicationObjection applicationObjection = new ApplicationObjection
                {
                    ApplicationId = appObjectionDto.ApplicationId,
                    Address = appObjectionDto.Address,
                    Birthday = appObjectionDto.Birthday,
                    City = appObjectionDto.City,
                    Email = appObjectionDto.Email,
                    EmaraId = appObjectionDto.EmaraId,
                    EmiratesId = appObjectionDto.EmiratesId,
                    FullName = appObjectionDto.FullName,
                    Gender = appObjectionDto.Gender,
                    Nationality = Convert.ToInt32(appObjectionDto.Nationality),
                    Note = appObjectionDto.Note,
                    Phone = appObjectionDto.Phone,
                    Reason = appObjectionDto.Reason,
                    CreatedDate = DateTime.Now
                };

                await _EngineCoreDBContext.ApplicationObjection.AddAsync(applicationObjection);
                await _EngineCoreDBContext.SaveChangesAsync();

                if (appObjectionDto.Attachments != null && appObjectionDto.Attachments.Count > 0)
                {
                    if (!_IFilesUploaderRepository.FolderExist(Path.Combine(_IConfiguration["BaseFolder"], application.ServiceId.ToString())))
                    {
                        _IFilesUploaderRepository.CreateFolder(Path.Combine(_IConfiguration["BaseFolder"], application.ServiceId.ToString()));
                    }

                    if (!_IFilesUploaderRepository.FolderExist(Path.Combine(_IConfiguration["BaseFolder"], application.ServiceId.ToString(), application.Id.ToString())))
                    {
                        _IFilesUploaderRepository.CreateFolder(Path.Combine(_IConfiguration["BaseFolder"], application.ServiceId.ToString(), application.Id.ToString()));
                    }

                    string destination = Path.Combine(_IConfiguration["BaseFolder"], application.ServiceId.ToString(), application.Id.ToString(), _IConfiguration["ObjectionFolder"]);
                    if (!_IFilesUploaderRepository.FolderExist(destination))
                    {
                        _IFilesUploaderRepository.CreateFolder(destination);
                    }

                    List<ApplicationObjectionAttachment> applicationObjectionAttachments = new List<ApplicationObjectionAttachment>();
                    foreach (var attach in appObjectionDto.Attachments)
                    {
                        var fileName = Path.GetFileName(attach);

                        if (!_IFilesUploaderRepository.MoveFile(attach, Path.Combine(destination, fileName)))
                        {
                            _exception.AttributeMessages.Add(" Fail in moving the file from the temp to the application files.");
                            throw _exception;
                        }

                        ApplicationObjectionAttachment applicationObjectionAttachment = new ApplicationObjectionAttachment
                        {
                            ObjectionId = applicationObjection.Id,
                            Attachment = Path.Combine(destination, fileName)
                        };

                        applicationObjectionAttachments.Add(applicationObjectionAttachment);
                    }


                    await _EngineCoreDBContext.ApplicationObjectionAttachment.AddRangeAsync(applicationObjectionAttachments);
                    await _EngineCoreDBContext.SaveChangesAsync();
                }


                scope.Complete();



            }

            catch (Exception ex)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "WrongObjection") + "  " + ex.Message);
                throw _exception;
            }
            return 1;

        }




        public async Task<List<LateAppsDto>> GetLateApps(string lessDate)
        {
            SysLookupType sysLookupType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "stage_type").FirstOrDefault();
            int lookupTypeId = sysLookupType.Id;

            var stageTypeTranslation = // get All stage type translation 
                (from sys_lookup_value in _EngineCoreDBContext.SysLookupValue
                 join sys_translation in _EngineCoreDBContext.SysTranslation on sys_lookup_value.Shortcut equals sys_translation.Shortcut
                 where
                   sys_lookup_value.LookupTypeId == lookupTypeId &&
                   sys_translation.Lang == "ar"

                 select new
                 {
                     sys_translation.Value,
                     sys_translation.Lang,
                     lvshortcut = sys_lookup_value.Shortcut,
                     sys_lookup_value.LookupTypeId,
                     sys_lookup_value.Id
                 });

            List<int> StagesHours = await _EngineCoreDBContext.AdmStage.Select(x => (int)x.PeriodForLate).Distinct().ToListAsync();
            Dictionary<int, DateTime> lateDate = await _IWorkingTimeRepository.GetDeadline(StagesHours.Distinct().ToList());


            int AutoCancelId = await _ISysValueRepository.GetIdByShortcut("AutoCancel");
            int RejectedStateId = await _ISysValueRepository.GetIdByShortcut("REJECTED");
            int MeetingStageId = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == lookupTypeId)
                                  join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stageMeetingAR))
                                  on lv.Shortcut equals tr.Shortcut
                                  select new { StageId = lv.Id }).Select(x => x.StageId).FirstOrDefault();
            int ReturnedStateId = await _ISysValueRepository.GetIdByShortcut("RETURNED");
            int PaymentStageId = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == lookupTypeId)
                                  join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stagePaymentAR))
                                  on lv.Shortcut equals tr.Shortcut
                                  select new { StageId = lv.Id }).Select(x => x.StageId).FirstOrDefault();

            var late = (from app in _EngineCoreDBContext.Application.Where(x => x.StateId != RejectedStateId && x.StateId != AutoCancelId)
                        join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId == MeetingStageId || x.StageTypeId == PaymentStageId)
                        on app.CurrentStageId equals stg.Id
                        join srv in _EngineCoreDBContext.AdmService on app.ServiceId equals srv.Id
                        join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == "ar") on srv.Shortcut equals tr.Shortcut
                        join temp in _EngineCoreDBContext.Template on app.TemplateId equals temp.Id
                        join tr1 in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == "ar") on temp.TitleShortcut equals tr1.Shortcut

                        where app.AppLastUpdateDate <= Convert.ToDateTime(lessDate)
                        select new LateAppsDto
                        {
                            LastUpdateDate = (DateTime)app.AppLastUpdateDate,
                            StageTypeId = (int)stg.StageTypeId,
                            StateId = (int)app.StateId,
                            ApplicationId = app.Id,
                            ServiceId = (int)app.ServiceId,
                            ApplicationNo = app.ApplicationNo,
                            ServiceName = tr.Value,
                            TemplateName = tr1.Value,
                            stageName = stageTypeTranslation.Where(x => x.Id == stg.StageTypeId).Select(z => z.Value).FirstOrDefault(),
                            islate = app.AppLastUpdateDate <= lateDate[(int)stg.PeriodForLate] ? true : false
                        }).ToList();

            var LateMeetPay = late.Where(x => x.islate == true).ToList();

            var lateDraft = (from app in _EngineCoreDBContext.Application.Where(x => x.StateId != RejectedStateId && x.StateId == ReturnedStateId && x.StateId != AutoCancelId)
                             join stg in _EngineCoreDBContext.AdmStage
                             on app.CurrentStageId equals stg.Id
                             join srv in _EngineCoreDBContext.AdmService on app.ServiceId equals srv.Id
                             join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == "ar") on srv.Shortcut equals tr.Shortcut
                             join temp in _EngineCoreDBContext.Template on app.TemplateId equals temp.Id
                             join tr1 in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == "ar") on temp.TitleShortcut equals tr1.Shortcut

                             where app.AppLastUpdateDate <= Convert.ToDateTime(lessDate)
                             select new LateAppsDto
                             {
                                 LastUpdateDate = (DateTime)app.AppLastUpdateDate,
                                 StageTypeId = (int)stg.StageTypeId,
                                 StateId = (int)app.StateId,
                                 ApplicationId = app.Id,
                                 ServiceId = (int)app.ServiceId,
                                 ApplicationNo = app.ApplicationNo,
                                 ServiceName = tr.Value,
                                 TemplateName = tr1.Value,
                                 stageName = stageTypeTranslation.Where(x => x.Id == stg.StageTypeId).Select(z => z.Value).FirstOrDefault(),
                                 islate = app.AppLastUpdateDate <= lateDate[(int)stg.PeriodForLate] ? true : false
                             }).ToList();

            var LateDraft = lateDraft.Where(x => x.islate == true).ToList();
            List<LateAppsDto> list = LateDraft.Union(LateMeetPay).ToList();


            return list;
        }
        public async Task<APIResult> RejectApps(string lessDate)
        {
            APIResult result = new APIResult();
            List<LateAppsDto> list = new List<LateAppsDto>();
            list = await GetLateApps(lessDate);
            if (list == null)
            {
                result.Message.Add("No Applications to reject");
                return result;
            }
            List<ServiceApplication> serviceApplications = new List<ServiceApplication>();

            foreach (var app in list)
            {
                ServiceApplication serviceApplication = new ServiceApplication();
                serviceApplication.ApplicationId = app.ApplicationId;
                serviceApplication.ServiceId = app.ServiceId;
                serviceApplications.Add(serviceApplication);

            }





            int AutoCancelled = await _ISysValueRepository.GetIdByShortcut("AutoCancel");
            if (AutoCancelled == -1)
            {
                result.Message.Add("Invalid rejected ID, no configuration for rejected Id, ask the admin to fix.");
                return result;
            }

            int? adminId = await _EngineCoreDBContext.User.Where(x => x.UserName.ToLower().Contains("notaryadmin")).Select(z => z.Id).FirstOrDefaultAsync();


            if (adminId == null)
            {
                result.Message.Add("Invalid admin ID, no configuration for rejected Id, ask the admin to fix.");
                return result;
            }


            var applicationsToRejectList = serviceApplications.Select(y => y.ApplicationId).ToList();

            var applicationsToReject = await _EngineCoreDBContext.Application.Where(x => applicationsToRejectList.Contains(x.Id)).ToListAsync();

            List<ApplicationTrack> applicationsTrack = new List<ApplicationTrack>();

            foreach (var app in applicationsToReject)
            {
                app.StateId = AutoCancelled;
                ApplicationTrack applicationTrack = new ApplicationTrack
                {
                    CreatedBy = adminId,
                    StageId = app.CurrentStageId,
                    UserId = adminId,
                    ApplicationId = app.Id,
                    CreatedDate = DateTime.Now,
                    Note = "تم الغاء الطلب لعدم استكماله من قبلكم ضمن الفترة المسموح بها",
                    NoteKind= (short?)NoteKind.AutoCancelled

            };


                applicationsTrack.Add(applicationTrack);
            }

            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            _EngineCoreDBContext.Application.UpdateRange(applicationsToReject);
            await _EngineCoreDBContext.SaveChangesAsync();

            await _EngineCoreDBContext.ApplicationTrack.AddRangeAsync(applicationsTrack);
            await _EngineCoreDBContext.SaveChangesAsync();

            result = await NotifyLateAppsPartyies(serviceApplications);
            scope.Complete();



            return result;
        }

        public async Task<List<LateAppsDto>> GetNotLateApps()
        {
            int AutoCancelId = await _ISysValueRepository.GetIdByShortcut("AutoCancel");

            SysLookupType sysLookupType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "stage_type").FirstOrDefault();
            int lookupTypeId = sysLookupType.Id;

            var stageTypeTranslation = // get All stage type translation 
                (from sys_lookup_value in _EngineCoreDBContext.SysLookupValue
                 join sys_translation in _EngineCoreDBContext.SysTranslation on sys_lookup_value.Shortcut equals sys_translation.Shortcut
                 where
                   sys_lookup_value.LookupTypeId == lookupTypeId &&
                   sys_translation.Lang == "ar"

                 select new
                 {
                     sys_translation.Value,
                     sys_translation.Lang,
                     lvshortcut = sys_lookup_value.Shortcut,
                     sys_lookup_value.LookupTypeId,
                     sys_lookup_value.Id
                 });

            List<int> StagesHours = await _EngineCoreDBContext.AdmStage.Select(x => (int)x.PeriodForLate).Distinct().ToListAsync();
            Dictionary<int, DateTime> lateDate = await _IWorkingTimeRepository.GetDeadline(StagesHours.Distinct().ToList());



            int RejectedStateId = await _ISysValueRepository.GetIdByShortcut("REJECTED");
            int MeetingStageId = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == lookupTypeId)
                                  join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stageMeetingAR))
                                  on lv.Shortcut equals tr.Shortcut
                                  select new { StageId = lv.Id }).Select(x => x.StageId).FirstOrDefault();
            int ReturnedStateId = await _ISysValueRepository.GetIdByShortcut("RETURNED");
            int PaymentStageId = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == lookupTypeId)
                                  join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stagePaymentAR))
                                  on lv.Shortcut equals tr.Shortcut
                                  select new { StageId = lv.Id }).Select(x => x.StageId).FirstOrDefault();

            var late = (from app in _EngineCoreDBContext.Application.Where(x => x.StateId != RejectedStateId && x.StateId != AutoCancelId)
                        join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId == MeetingStageId || x.StageTypeId == PaymentStageId)
                        on app.CurrentStageId equals stg.Id
                        join srv in _EngineCoreDBContext.AdmService on app.ServiceId equals srv.Id
                        join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == "ar") on srv.Shortcut equals tr.Shortcut
                        join temp in _EngineCoreDBContext.Template on app.TemplateId equals temp.Id
                        join tr1 in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == "ar") on temp.TitleShortcut equals tr1.Shortcut


                        select new LateAppsDto
                        {
                            LastUpdateDate = (DateTime)app.AppLastUpdateDate,
                            StageTypeId = (int)stg.StageTypeId,
                            StateId = (int)app.StateId,
                            ApplicationId = app.Id,
                            ServiceId = (int)app.ServiceId,
                            ApplicationNo = app.ApplicationNo,
                            ServiceName = tr.Value,
                            TemplateName = tr1.Value,
                            stageName = stageTypeTranslation.Where(x => x.Id == stg.StageTypeId).Select(z => z.Value).FirstOrDefault(),
                            islate = app.AppLastUpdateDate <= lateDate[(int)stg.PeriodForLate] ? true : false
                        }).ToList();

            var LateMeetPay = late.Where(x => x.islate == false).ToList();

            var lateDraft = (from app in _EngineCoreDBContext.Application.Where(x => x.StateId != RejectedStateId && x.StateId == ReturnedStateId && x.StateId != AutoCancelId)
                             join stg in _EngineCoreDBContext.AdmStage
                             on app.CurrentStageId equals stg.Id
                             join srv in _EngineCoreDBContext.AdmService on app.ServiceId equals srv.Id
                             join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == "ar") on srv.Shortcut equals tr.Shortcut
                             join temp in _EngineCoreDBContext.Template on app.TemplateId equals temp.Id
                             join tr1 in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == "ar") on temp.TitleShortcut equals tr1.Shortcut


                             select new LateAppsDto
                             {
                                 LastUpdateDate = (DateTime)app.AppLastUpdateDate,
                                 StageTypeId = (int)stg.StageTypeId,
                                 ApplicationId = app.Id,
                                 StateId = (int)app.StateId,
                                 ServiceId = (int)app.ServiceId,
                                 ApplicationNo = app.ApplicationNo,
                                 ServiceName = tr.Value,
                                 TemplateName = tr1.Value,
                                 stageName = stageTypeTranslation.Where(x => x.Id == stg.StageTypeId).Select(z => z.Value).FirstOrDefault(),
                                 islate = app.AppLastUpdateDate <= lateDate[(int)stg.PeriodForLate] ? true : false
                             }).ToList();

            var LateDraft = lateDraft.Where(x => x.islate == false).ToList();
            List<LateAppsDto> list = LateDraft.Union(LateMeetPay).ToList();


            return list;
        }

        public async Task<APIResult> DailyNotify()
        {
            APIResult result = new APIResult();
            try
            {

                // select throw exception if the configuration is missed.
                int actionToReject = _EngineCoreDBContext.AdmAction.Where(x => x.Shortcut.Contains("RejectLateApps")).Select(z => z.Id).FirstOrDefault();
                int actionNotifyToReturned = _EngineCoreDBContext.AdmAction.Where(x => x.Shortcut.Contains("NotifyReturnedApps")).Select(z => z.Id).FirstOrDefault();
                int actionNotifyToMeeting = _EngineCoreDBContext.AdmAction.Where(x => x.Shortcut.Contains("NotifyMeetingApps")).Select(z => z.Id).FirstOrDefault();
                int actionNotifyToPayment = _EngineCoreDBContext.AdmAction.Where(x => x.Shortcut.Contains("NotifyPaymentApps")).Select(z => z.Id).FirstOrDefault();


                List<LateAppsDto> list = new List<LateAppsDto>();
                SysLookupType sysLookupType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "stage_type").FirstOrDefault();
                int lookupTypeId = sysLookupType.Id;
                int meetingStageId = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == lookupTypeId)
                                      join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stageMeetingAR))
                                      on lv.Shortcut equals tr.Shortcut
                                      select new { StageId = lv.Id }).Select(x => x.StageId).FirstOrDefault();

                int returnedStateId = await _ISysValueRepository.GetIdByShortcut("RETURNED");
                int paymentStageId = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == lookupTypeId)
                                      join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stagePaymentAR))
                                      on lv.Shortcut equals tr.Shortcut
                                      select new { StageId = lv.Id }).Select(x => x.StageId).FirstOrDefault();

                list = await GetNotLateApps();

                if (list == null)
                {
                    string msg = " No Applications to Notify at " + DateTime.Now.ToString();
                    _logger.LogInformation(msg);
                    result.Message.Add(msg);
                    return result;
                }

                List<ServiceApplication> serviceApplications = new List<ServiceApplication>();

                foreach (var app in list)
                {
                    ServiceApplication serviceApplication = new ServiceApplication
                    {
                        ApplicationId = app.ApplicationId,
                        ServiceId = app.ServiceId,
                        StateId = app.StateId,
                        StageTyeId = app.StageTypeId
                    };
                    serviceApplications.Add(serviceApplication);
                }


                int partyCount = 0;

                List<Receiver> receivers = new List<Receiver>();
                Dictionary<string, string> ParameterDic = new Dictionary<string, string>
                {   { APPLICATION_NUMBER_MOB, "" }, { APPLICATION_NUMBER, "" }
                };

                var notificationsToReturned = await _INotificationSettingRepository.GetNotificationsForAction(actionNotifyToReturned);
                var notificationsToMeeting = await _INotificationSettingRepository.GetNotificationsForAction(actionNotifyToMeeting);
                var notificationsToPayment = await _INotificationSettingRepository.GetNotificationsForAction(actionNotifyToPayment);

                foreach (var app in serviceApplications)
                {
                    if (app.StateId == returnedStateId)
                    {
                        List<NotificationLogPostDto> notifications = new List<NotificationLogPostDto>();
                        foreach (var notify in notificationsToReturned)
                        {
                            notifications.Add(notify.ShallowCopy());
                        }

                        receivers = await GetReceiverPartyByAppID(app.ApplicationId, true);
                        partyCount += receivers.Count;
                        ParameterDic[APPLICATION_NUMBER] = app.ApplicationId.ToString();
                        ParameterDic[APPLICATION_NUMBER_MOB] = app.ApplicationId.ToString();
                        int noti = await FillAndSendNotification(notifications, receivers, ParameterDic, app.ServiceId, app.ApplicationId, true, "ar");
                    }
                    else if (app.StageTyeId == meetingStageId)
                    {
                        List<NotificationLogPostDto> notifications = new List<NotificationLogPostDto>();
                        foreach (var notify in notificationsToMeeting)
                        {
                            notifications.Add(notify.ShallowCopy());
                        }

                        receivers = await GetReceiverPartyByAppID(app.ApplicationId, true);
                        partyCount += receivers.Count;
                        ParameterDic[APPLICATION_NUMBER] = app.ApplicationId.ToString();
                        ParameterDic[APPLICATION_NUMBER_MOB] = app.ApplicationId.ToString();
                        int noti = await FillAndSendNotification(notifications, receivers, ParameterDic, app.ServiceId, app.ApplicationId, true, "ar");
                    }
                    else if (app.StageTyeId == paymentStageId)
                    {
                        List<NotificationLogPostDto> notifications = new List<NotificationLogPostDto>();
                        foreach (var notify in notificationsToPayment)
                        {
                            notifications.Add(notify.ShallowCopy());
                        }

                        receivers = await GetReceiverPartyByAppID(app.ApplicationId, true);
                        partyCount += receivers.Count;
                        ParameterDic[APPLICATION_NUMBER] = app.ApplicationId.ToString();
                        ParameterDic[APPLICATION_NUMBER_MOB] = app.ApplicationId.ToString();
                        int noti = await FillAndSendNotification(notifications, receivers, ParameterDic, app.ServiceId, app.ApplicationId, true, "ar");
                    }
                }

                result.Id = serviceApplications.Count;
                result.Result = partyCount;
                result.Message.Add($" Application Count  : {serviceApplications.Count}");
                result.Message.Add($"Notified parties count : {partyCount}");

                return result;
            }
            catch (Exception e)
            {
                string msg = " Error Exception in DailyNotify " + e.Message + " inner exception is " + e.InnerException;
                _logger.LogInformation(msg);
                result.Result = 0;
                result.Message.Add(msg);
                return result;
            }

        }


        public bool InitialBlockChain(int appid)
        {
            BlockChainPoa app = _EngineCoreDBContext.BlockChainPoa.Where(x => x.AppId == appid).FirstOrDefault();
            if (app == null)
            {
                BlockChainPoa blockChainPoa = new BlockChainPoa()
                {
                    AppId = appid,
                    CreatedDate = DateTime.Now,
                    IsSysCancelled = false,
                    IsUgCancelled = false,
                    IsSentUg = false
                };
                _IGeneralRepository.Add(blockChainPoa);

                return _IGeneralRepository.Save().Result;
            }
            else
                return false;
        }
            

        public async Task<List<AppObjectionDto>> GetAppObjection(int appId)
        {
           List<AppObjectionDto> applicationObjections = new List<AppObjectionDto>();
            var query = (from app in _EngineCoreDBContext.ApplicationObjection.Where(x => x.ApplicationId == appId)
                                          join cn in _EngineCoreDBContext.Country on app.Nationality equals cn.UgId
                                         //join att in _EngineCoreDBContext.ApplicationObjectionAttachment on app.Id equals att.ObjectionId
                                     select new AppObjectionDto
                                     {
                                         Email = app.Email,
                                         EmaraId = (int)app.EmaraId,
                                         EmiratesId = app.EmiratesId,
                                         Phone = app.Phone,
                                         Gender=app.Gender,
                                         Nationality=cn.CntCountryAr,
                                         FullName = app.FullName,
                                         Address = app.Address,
                                         City = app.City,
                                         Reason = app.Reason,
                                         Note = app.Note,
                                         Birthday = app.Birthday,

                                         Attachments = _EngineCoreDBContext.ApplicationObjectionAttachment.Where(x => x.ObjectionId == app.Id)
                                                       .Select(z=>z.Attachment)
                                                       .ToList(),

                                     });
            applicationObjections =await  query.ToListAsync();

            return applicationObjections;
        }


      public async  Task<APIResult> AddNoteTrack(ApplicationTrackDto applicationTrackDto)


        {
            APIResult result = new APIResult();
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                int id = await _IApplicationTrackRepository.Add(applicationTrackDto);
                if (id < 0)
                {
                    result.Message.Add("خطأ في إضافة الملاحظات");
                    return result;
                }


                scope.Complete();
            }
            catch
            {
                result.Id = -1;
                result.Message.Add("خطأ في إضافة الملاحظات  ");
            }
            result.Id = 1;
            result.Result = true;
            result.Code = 200;
            return result;
        }

        public async Task<APIResult> GetDoneNoPay(int appId, int userId, int? timeLimit, List<string> timeoutMsgs, string lang)
        {
            APIResult result = new APIResult();
            if (timeLimit != null)
            {
                DateTime? startDate = await _EngineCoreDBContext.ApplicationTrack.Where(x => x.ApplicationId == appId).OrderByDescending(t => t.CreatedDate).Select(x => x.CreatedDate).FirstOrDefaultAsync();
                double diff = DateTime.Now.Subtract((DateTime)startDate).TotalDays;
                if (diff < timeLimit)
                {
                    result.Message.AddRange(timeoutMsgs);
                    result.Message.Add($"يوم/أيام {timeLimit - (int)diff}  متبقي على الأقل ");
                    return result;
                }
            }

            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            if (await MakeItDone(appId, userId) == ERROR)
            {
                result.Message.Add("خطأ في نقل الطلب للمرحلة التالية");
                return result;
            }
            var transactions = await _ITransactionRepository.GetAll(appId);
            if (transactions.Count > 0)
            {
                string path = _IConfiguration["TransactionFolder"];
                int transactionId = transactions[0].Id;
                if (!String.IsNullOrEmpty(transactions[0].TransactionNo))
                {
                    TransactionOldVersionDto dto = new TransactionOldVersionDto
                    {
                        TransactionId = transactions[0].Id,
                        TransactionCreatedDate = transactions[0].TransactionCreatedDate != null ? (DateTime)transactions[0].TransactionCreatedDate : System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                        DocumentUrl = transactions[0].DocumentUrl == null ? "" : transactions[0].DocumentUrl,
                        TransactionNo = transactions[0].TransactionNo
                    };

                    int i = await _ITransactionRepository.AddOldVersion(dto, userId);
                }

                TransactionDto transactionDto = _ITransactionRepository.FromObjectToDto(transactions[0]);
                transactionDto.TransactionNo = _ITransactionRepository.GenerateTransactionNo();
                if (await _ITransactionRepository.Update(transactionId, userId, transactionDto) == ERROR)
                {
                    result.Message.Add("خطأ في تعديل المعاملة");
                    return result;

                }

                try
                {
                    GeneratorRepository generator = new GeneratorRepository(_EngineCoreDBContext, this, _IPaymentRepository, _IGeneralRepository, _FileNaming, _IConverter, _Pdfdocumentsetting, _IFilesUploaderRepository);
                    AutoCreatePdfPaths paths = await generator.autoCreatePDFAsync("en", appId, path);
                    if (paths.TransactionDoc != null && paths.TransactionDoc.Length > 0)
                    {
                        var app = await _EngineCoreDBContext.Application.Where(x => x.Id == appId).FirstOrDefaultAsync();
                        string destination = Path.Combine(_IConfiguration["BaseFolder"], app.ServiceId.ToString(), appId.ToString(), paths.TransactionDoc);
                        bool m = MoveAppAttachment(paths.TransactionDoc, destination);
                        transactionDto.DocumentUrl = destination;
                        transactionDto.TransactionCreatedDate = DateTime.Now;
                        await _ITransactionRepository.Update(transactionId, userId, transactionDto);
                    }

                    if (paths.RecordIdPaths.Count > 0)
                    {
                        foreach (KeyValuePair<int, string> entry in paths.RecordIdPaths)
                        {
                            AppRelatedContentDto appdto = new AppRelatedContentDto();
                            AppRelatedContent a = await GetOneRelatedContent(entry.Key);
                            appdto.AppId = a.AppId;
                            appdto.Content = a.Content;
                            appdto.TitleShortcut = a.TitleShortcut;
                            appdto.ContentUrl = entry.Value;
                            await UpdateRContent(entry.Key, appdto);
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.Message.Add("خطأ في إنشاء الوثيقة");
                    return result;
                }
            }
            else
            {
                return result;
            }

            result.Id = 1;
            result.Message.Add(getMessage(lang, "Done"));
            scope.Complete();
            return result;

        }
    }
}
