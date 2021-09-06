using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using EngineCoreProject.DTOs.MyApplicationDto;
using EngineCoreProject.IRepository.IMyApplications;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.Models;
using EngineCoreProject.Services.GeneralSetting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.IQueueRepository;
using EngineCoreProject.DTOs.QueueDto;
using EngineCoreProject.IRepository.IMeetingRepository;
using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.IRepository.AdmServices;
using EngineCoreProject.DTOs;
using EngineCoreProject.IRepository.IGeneralSetting;
using System.Transactions;
using EngineCoreProject.DTOs.NotificationDtos;
using EngineCoreProject.DTOs.AdmService;

namespace EngineCoreProject.Services.MyApplication
{
    public class MyApplicationRepository : IMyApplicationRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _IGeneralRepository;
        private readonly IGlobalDayOffRepository _iGlobalDayOffRepository;
        private readonly IUserRepository _iUserRepository;
        private readonly IApplicationRepository _iApplicationRepository;
        private readonly ISysValueRepository _ISysValueRepository;
        private readonly IQueueRepository _IQueueRepository;
        private readonly IMeetingRepository _IMeetingRepository;
        private readonly IAdmServiceRepository _IAdmServiceRepository;
        private readonly IWorkingTimeRepository _IWorkingTimeRepository;
        
        ValidatorException _exception;


        public MyApplicationRepository(IWorkingTimeRepository iWorkingTimeRepository,IAdmServiceRepository iAdmServiceRepository ,IMeetingRepository iMeetingRepository, IQueueRepository iQueueRepository ,ISysValueRepository iSysValueRepository, IApplicationRepository iApplicationRepository, IUserRepository iUserRepository, EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository, IGlobalDayOffRepository iGlobalDayOffRepository)


        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _IGeneralRepository = iGeneralRepository;
            _iGlobalDayOffRepository = iGlobalDayOffRepository;
            _iUserRepository = iUserRepository;
            _iApplicationRepository = iApplicationRepository;
            _ISysValueRepository = iSysValueRepository;
            _IQueueRepository = iQueueRepository;
            _IMeetingRepository = iMeetingRepository;
            _IAdmServiceRepository = iAdmServiceRepository;
            _IWorkingTimeRepository = iWorkingTimeRepository;
            
            _exception = new ValidatorException();

        }

        public async Task<List<AllapplicationDto>> FirstPage(string lang)
        {
            Task<List<AllapplicationDto>> query = null;  //AllapplicationDto

            return await query;



        }



        public async Task<ApplicationCountDto> appPages(searchDto searchDto, string lang, int currentpage, int perpage)
        {
            SysLookupType sysLookupType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "stage_type").FirstOrDefault();
            int lookupTypeId = sysLookupType.Id;
            int RejectedStageId = await _ISysValueRepository.GetIdByShortcut("REJECTEDSTAGE");


            var stagetypetranslation = // get All stage type translation in system lookup type table
                (from sys_lookup_value in _EngineCoreDBContext.SysLookupValue
                 join sys_translation in _EngineCoreDBContext.SysTranslation on sys_lookup_value.Shortcut equals sys_translation.Shortcut
                 where
                   sys_lookup_value.LookupTypeId == lookupTypeId &&
                   sys_translation.Lang == lang


                 select new
                 {
                     sys_translation.Value,
                     sys_translation.Lang,
                     lvshortcut = sys_lookup_value.Shortcut,
                     sys_lookup_value.LookupTypeId,
                     sys_lookup_value.Id
                 });
            string draftShortcut = _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stageDraftAR) || x.Value.Contains(Constants.stageDraftEN))
                                                                   .Select(y => y.Shortcut).FirstOrDefault();
            int draftId = _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == draftShortcut).Select(y => y.Id).FirstOrDefault();

            var isemployee = (from appt in _EngineCoreDBContext.ApplicationTrack
                              join ur in _EngineCoreDBContext.UserRole on appt.UserId equals ur.UserId
                              where ur.RoleId == 17
                              select new
                              {
                                  appid = appt.ApplicationId,
                                  userid = appt.UserId,
                                  created_date = appt.CreatedDate
                              });



            var GroupappTrak = (
                from appt in isemployee
                group appt by appt.appid into t



                select new
                {
                    appid = t.Key,
                    created_date = t.Max(x => x.created_date)
                });
            var apptrak = (from appt in _EngineCoreDBContext.ApplicationTrack
                           join gt in GroupappTrak on new { colA = appt.ApplicationId, colB = appt.CreatedDate } equals new { colA = gt.appid, colB = gt.created_date }
                           select new
                           {
                               appid = appt.ApplicationId,
                               userid = appt.UserId
                           }

                );
            int userid = _iUserRepository.GetUserID();
            ApplicationCountDto applicationCountDto = new ApplicationCountDto();


            if (searchDto.onlyMyApps)
            {
                var AppStageService = // get service name and stage for each application and translate Service shortcut
                    (from app in _EngineCoreDBContext.Application
                     join pt in apptrak.Where(x=>x.userid==userid)
                     on app.Id equals pt.appid
                     join srv in _EngineCoreDBContext.AdmService
                     on app.ServiceId equals srv.Id
                     join stg in _EngineCoreDBContext.AdmStage//.Where(x => x.StageTypeId != draftId)
                     on app.CurrentStageId equals stg.Id
                     join tr in _EngineCoreDBContext.SysTranslation
                     on srv.Shortcut equals tr.Shortcut

                     where tr.Lang == lang
                     select new AllapplicationDto
                     {
                         percent = 0,
                         lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                         periodLate = (int)stg.PeriodForLate,
                         appStatusId = app.StateId,
                         appid = app.Id,
                         stagetypeid = stg.StageTypeId,
                         serviceid = srv.Id,
                         servicename = tr.Value,
                         fee = srv.Fee,
                         templateid = app.TemplateId,
                         appstartdate = app.CreatedDate,
                         stagetypename = (stagetypetranslation.Where(x => x.Id == stg.StageTypeId).Select(y => y.Value).FirstOrDefault())
                     }
                    );//.ToListAsync(); 


                //we need to get All name of template for All applications 
                //if template id ==null then template name is null by left join in query
                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id into g
                                     from q in g.DefaultIfEmpty()
                                     select new AllapplicationDto
                                     {
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         fee = q1.fee,
                                         templateid = q.Id,
                                         appstartdate = q1.appstartdate,
                                         templatename = q.TitleShortcut,
                                         doctypeid = q.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid,
                                         stagetypename=q1.stagetypename


                                     });//.ToListAsync();

                //then we get translations of template name 
                //if template name ==null then template name translation is null by left join in query
                var querytemplatetrans = (from q1 in templateQuery
                                          join tm in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang) on q1.templatename equals tm.Shortcut into g
                                          from q in g.DefaultIfEmpty()
                                          select new AllapplicationDto
                                          {
                                              percent = q1.percent,
                                              lastUpdateStage = q1.lastUpdateStage,
                                              periodLate = q1.periodLate,
                                              appStatusId = q1.appStatusId,
                                              appid = q1.appid,
                                              serviceid = q1.serviceid,
                                              servicename = q1.servicename,
                                              fee = q1.fee,
                                              templateid = q.Id,
                                              appstartdate = q1.appstartdate,
                                              templatename = q.Value,
                                              doctypeid = q1.doctypeid,
                                              stagetypeid = q1.stagetypeid,
                                              stagetypename=q1.stagetypename


                                          });//.ToListAsync();

                //now i need to get document name fro every application
                // if documentid==null then document name=null by left join in query
                var documentNameQuery = (
                       from qtl in querytemplatetrans
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id into g
                       from q in g.DefaultIfEmpty()


                       select new AllapplicationDto
                       {
                           percent = qtl.percent,
                           lastUpdateStage = qtl.lastUpdateStage,
                           periodLate = qtl.periodLate,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           fee = qtl.fee,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           documentname = q.Shortcut,
                           stagetypeid = qtl.stagetypeid,
                           stagetypename=qtl.stagetypename

                       }
                       );

                var querydoctrans = (
                    from q3 in documentNameQuery
                    join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang)
                    on q3.documentname equals tr.Shortcut into g
                    from q in g.DefaultIfEmpty()
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q.Value,
                        stagetypeid = q3.stagetypeid,
                        stagetypename=q3.stagetypename

                    }

                    );


                //get applications status translation
                var Appstatustranslate = (
                    from q3 in querydoctrans
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    join tr5 in _EngineCoreDBContext.SysTranslation
                    on lv.Shortcut equals tr5.Shortcut
                    where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        stagetypeid = q3.stagetypeid,
                        stagetypename=q3.stagetypename

                    }
                    );
                //we need to know owner of All applications
                var isownerQuery = (
                    from par in _EngineCoreDBContext.ApplicationParty
                    where par.IsOwner == true
                    select new
                    {
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile
                    });

                // get transaction for owner applications
                var userTransactionsQuery = (
                     from app in apptrak //_EngineCoreDBContext.Application
                     join tranc in _EngineCoreDBContext.AppTransaction on app.appid equals tranc.ApplicationId
                     join q in isownerQuery on tranc.Id equals q.trancId into g

                     from q in g.DefaultIfEmpty()


                     select new AppPartyDto
                     {
                         appid = app.appid,
                         fullname = q.fullname,
                         email = q.email,
                         mobile = q.mobile
                     });//.ToListAsync();


                //get result of all applications
                var result = (
                     from q4 in userTransactionsQuery
                     join q5 in Appstatustranslate on q4.appid equals q5.appid

                     orderby q5.appstartdate
                     select new Apps
                     {
                         percent = q5.percent,
                         lastUpdateStage = q5.lastUpdateStage,
                         periodLate = q5.periodLate,
                         appStatusName = q5.appStatusName,
                         appStatusId = q5.appStatusId,
                         appid = q4.appid,
                         serviceid = q5.serviceid,
                         servicename = q5.servicename,
                         fee = q5.fee,
                         appstartdate = q5.appstartdate,
                         templateid = q5.templateid,
                         templatename = q5.templatename,
                         doctypeid = q5.doctypeid,
                         documentname = q5.documentname,
                         fullname = q4.fullname,
                         email = q4.email,
                         mobile = q4.mobile,
                         stagetypeid = q5.stagetypeid,
                         stagetypename=q5.stagetypename,

                     }
                     );

                int skipValue = perpage * (currentpage - 1);
                int perpageValue = perpage;
                int count = result.Count();
                applicationCountDto.Applications = await result.OrderBy(x => x.lastUpdateStage).Skip(skipValue)
                                                               .Take(perpageValue)
                                                               .ToListAsync();
                applicationCountDto.count = count;
                // applicationCountDto.AppBySatge = finalResult.ToList();}
            }
           else if (!searchDto.onlyMyApps)
            {
                var AppStageService = // get service name and stage for each application and translate Service shortcut
                    (from app in _EngineCoreDBContext.Application
                     join srv in _EngineCoreDBContext.AdmService
                     on app.ServiceId equals srv.Id
                     join stg in _EngineCoreDBContext.AdmStage//.Where(x => x.StageTypeId != draftId)
                     on app.CurrentStageId equals stg.Id
                     join tr in _EngineCoreDBContext.SysTranslation
                     on srv.Shortcut equals tr.Shortcut

                     where tr.Lang == lang
                     select new AllapplicationDto
                     {
                         percent = 0,
                         lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                         periodLate = (int)stg.PeriodForLate,
                         appStatusId = app.StateId,
                         appid = app.Id,
                         stagetypeid = stg.StageTypeId,
                         serviceid = srv.Id,
                         servicename = tr.Value,
                         fee = srv.Fee,
                         templateid = app.TemplateId,
                         appstartdate = app.CreatedDate,
                         stagetypename = (stagetypetranslation.Where(x => x.Id == stg.StageTypeId).Select(y => y.Value).FirstOrDefault())
                     }
                    );//.ToListAsync(); 


                //we need to get All name of template for All applications 
                //if template id ==null then template name is null by left join in query
                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id into g
                                     from q in g.DefaultIfEmpty()
                                     select new AllapplicationDto
                                     {
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         fee = q1.fee,
                                         templateid = q.Id,
                                         appstartdate = q1.appstartdate,
                                         templatename = q.TitleShortcut,
                                         doctypeid = q.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid,
                                         stagetypename=q1.stagetypename


                                     });//.ToListAsync();

                //then we get translations of template name 
                //if template name ==null then template name translation is null by left join in query
                var querytemplatetrans = (from q1 in templateQuery
                                          join tm in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang) on q1.templatename equals tm.Shortcut into g
                                          from q in g.DefaultIfEmpty()
                                          select new AllapplicationDto
                                          {
                                              percent = q1.percent,
                                              lastUpdateStage = q1.lastUpdateStage,
                                              periodLate = q1.periodLate,
                                              appStatusId = q1.appStatusId,
                                              appid = q1.appid,
                                              serviceid = q1.serviceid,
                                              servicename = q1.servicename,
                                              fee = q1.fee,
                                              templateid = q.Id,
                                              appstartdate = q1.appstartdate,
                                              templatename = q.Value,
                                              doctypeid = q1.doctypeid,
                                              stagetypeid = q1.stagetypeid,
                                              stagetypename=q1.stagetypename


                                          });//.ToListAsync();

                //now i need to get document name fro every application
                // if documentid==null then document name=null by left join in query
                var documentNameQuery = (
                       from qtl in querytemplatetrans
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id into g
                       from q in g.DefaultIfEmpty()


                       select new AllapplicationDto
                       {
                           percent = qtl.percent,
                           lastUpdateStage = qtl.lastUpdateStage,
                           periodLate = qtl.periodLate,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           fee = qtl.fee,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           documentname = q.Shortcut,
                           stagetypeid = qtl.stagetypeid,
                           stagetypename=qtl.stagetypename,
                       }
                       );

                var querydoctrans = (
                    from q3 in documentNameQuery
                    join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang)
                    on q3.documentname equals tr.Shortcut into g
                    from q in g.DefaultIfEmpty()
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q.Value,
                        stagetypeid = q3.stagetypeid,
                        stagetypename=q3.stagetypename
                    }

                    );


                //get applications status translation
                var Appstatustranslate = (
                    from q3 in querydoctrans
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    join tr5 in _EngineCoreDBContext.SysTranslation
                    on lv.Shortcut equals tr5.Shortcut
                    where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        stagetypeid = q3.stagetypeid,
                       stagetypename=q3.stagetypename
                    }
                    );
                //we need to know owner of All applications
                var isownerQuery = (
                    from par in _EngineCoreDBContext.ApplicationParty
                    where par.IsOwner == true
                    select new
                    {
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile
                    });

                // get transaction for owner applications
                var userTransactionsQuery = (
                     from app in _EngineCoreDBContext.Application
                     join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                     join q in isownerQuery on tranc.Id equals q.trancId into g

                     from q in g.DefaultIfEmpty()


                     select new AppPartyDto
                     {
                         appid = app.Id,
                         fullname = q.fullname,
                         email = q.email,
                         mobile = q.mobile
                     });//.ToListAsync();


                //get result of all applications
                var result = (
                     from q4 in userTransactionsQuery
                     join q5 in Appstatustranslate on q4.appid equals q5.appid

                     orderby q5.appstartdate
                     select new Apps
                     {
                         percent = q5.percent,
                         lastUpdateStage = q5.lastUpdateStage,
                         periodLate = q5.periodLate,
                         appStatusName = q5.appStatusName,
                         appStatusId = q5.appStatusId,
                         appid = q4.appid,
                         serviceid = q5.serviceid,
                         servicename = q5.servicename,
                         fee = q5.fee,
                         appstartdate = q5.appstartdate,
                         templateid = q5.templateid,
                         templatename = q5.templatename,
                         doctypeid = q5.doctypeid,
                         documentname = q5.documentname,
                         fullname = q4.fullname,
                         email = q4.email,
                         mobile = q4.mobile,
                         stagetypeid = q5.stagetypeid,
                         stagetypename=q5.stagetypename

                     }
                     );

                int skipValue = perpage * (currentpage - 1);
                int perpageValue = perpage;
                int count = result.Count();
                applicationCountDto.Applications = await result.OrderBy(x => x.stagetypeid).Skip(skipValue)
                                                               .Take(perpageValue)
                                                               .ToListAsync();
                applicationCountDto.count = count;
                // applicationCountDto.AppBySatge = finalResult.ToList();}
            }
            return applicationCountDto;



        }


        public async Task<ApplicationCountDto> AllAppByStageType(searchDto searchDto, string lang, int currentpage, int perpage)
        {
            ApplicationCountDto applicationCountDto = new ApplicationCountDto();
            if (searchDto.onlyMyApps)
            {

                applicationCountDto = await NotaryCountApps(lang);
            }

            else
            {
                int AutoCancelledStageId = await _ISysValueRepository.GetIdByShortcut("SysCancelled");
                int DoneStageTypeId = await _ISysValueRepository.GetIdByShortcut("SysLookupValue_Shortcut677");
                string drtafShortcut = _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stageDraftAR)).Select(y => y.Shortcut).FirstOrDefault();
                int draftId = _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == drtafShortcut).Select(y => y.Id).FirstOrDefault();
                int ReturnedStateId = await _ISysValueRepository.GetIdByShortcut("RETURNED");
                int RejectedStateId = await _ISysValueRepository.GetIdByShortcut("REJECTED");
                int RejectedStageTypeId = await _ISysValueRepository.GetIdByShortcut("REJECTEDSTAGE");
                int AutoCancelId= await _ISysValueRepository.GetIdByShortcut("AutoCancel");



                List<int> StagesHours = await _EngineCoreDBContext.AdmStage.Select(x => (int)x.PeriodForLate).Distinct().ToListAsync();
                Dictionary<int, DateTime> lateDate = await _IWorkingTimeRepository.GetDeadline(StagesHours.Distinct().ToList());

                var AppStageService =
                    (from app in _EngineCoreDBContext.Application.Where(x => x.StateId != RejectedStateId && x.Id>0 && x.StateId!=AutoCancelId)          
                    
                     join stg in _EngineCoreDBContext.AdmStage
                     on app.CurrentStageId equals stg.Id
                     select new AllapplicationDto
                     {
                         
                         lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                         periodLate = (int)stg.PeriodForLate,
                         appStatusId = app.StateId,
                         appid = app.Id,
                         stagetypeid = stg.StageTypeId,
                         appstartdate = app.CreatedDate
                     });

                SysLookupType sysLookupType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "stage_type").FirstOrDefault();
                int lookupTypeId = sysLookupType.Id;


                var countAppinStage = // number of Application in each Stage
                            (from app in AppStageService

                             group app by app.stagetypeid into t
                             select new ApplicationByStageType
                             {
                                 Count = t.Count(),
                                 StageTypeId = (int)t.Key,


                             });

                var stageTypeTranslation = // get All stage type translation 
                    (from sys_lookup_value in _EngineCoreDBContext.SysLookupValue//.Where(x => !x.Shortcut.Contains(drtafShortcut))
                     join sys_translation in _EngineCoreDBContext.SysTranslation on sys_lookup_value.Shortcut equals sys_translation.Shortcut
                     where
                       sys_lookup_value.LookupTypeId == lookupTypeId &&
                       sys_translation.Lang == lang

                     select new
                     {
                         sys_translation.Value,
                         sys_translation.Lang,
                         lvshortcut = sys_lookup_value.Shortcut,
                         sys_lookup_value.LookupTypeId,
                         sys_lookup_value.Id
                     });

                var currentAppstagetype =// translate current stage of applications
                    (
                    from lv in _EngineCoreDBContext.SysLookupValue
                    join t in _EngineCoreDBContext.SysTranslation
                           on lv.Shortcut equals t.Shortcut
                    join q in countAppinStage
                            on lv.Id equals q.StageTypeId


                    where t.Lang == lang
                    select new ApplicationByStageType
                    {
                        Count = q.Count,
                        StageTypeId = q.StageTypeId,
                        StageTypeName = t.Value


                    });//.ToListAsync();

                var groupStageType = // left join to get All stage and number of applications in each stage  
                      (from q2 in stageTypeTranslation
                       join q1 in currentAppstagetype on q2.Id equals q1.StageTypeId into g
                       from q1 in g.DefaultIfEmpty()
                       select new ApplicationByStageType
                       {
                           StageTypeName = q2.Value,
                           StageTypeId = q2.Id,
                           Count = q1.Count,

                       });
                var finalResult = (
                    from q10 in groupStageType
                    orderby q10.StageTypeId
                    select new ApplicationByStage
                    {
                        StageTypeId = q10.StageTypeId,
                        StageTypeName = q10.StageTypeName,
                        Count = q10.Count,
                        Appstage = (
                            from q in AppStageService
                            where q.stagetypeid == q10.StageTypeId && q.stagetypeid!=DoneStageTypeId
                            select new Apps
                            {
                                //lastUpdateStage = q.lastUpdateStage,
                                //periodLate = q.periodLate,
                                //appStatusName = q.appStatusName,
                                appStatusId = q.appStatusId,
                              
                                stagetypeid = q.stagetypeid,
                                islate =(q.stagetypeid==DoneStageTypeId||q.stagetypeid==draftId||q.appStatusId==RejectedStateId||q.appStatusId==AutoCancelId)?false: q.lastUpdateStage >= lateDate[q.periodLate] ? false : true,
                                //islate =workingTimeRepository.IsOrderLate(q.lastUpdateStage, DateTime.Now, q.periodLate * 60).Result

                            }).ToList()

                    }
                       );
                applicationCountDto.autoCancelledStageId = AutoCancelledStageId;
                applicationCountDto.RejectedStageTypeId = RejectedStageTypeId;
                applicationCountDto.AutoCancelledId = AutoCancelId;
                applicationCountDto.autoCancelledCount = _EngineCoreDBContext.Application.Where(x =>x.StateId == AutoCancelId).Count();
                applicationCountDto.rejectedCount = _EngineCoreDBContext.Application.Where(x=>x.StateId==RejectedStateId ).Count();
                applicationCountDto.returnedCount = AppStageService.Where(x => x.stagetypeid == draftId && x.appStatusId == ReturnedStateId).Count();
                applicationCountDto.AppBySatge = await finalResult.ToListAsync();

            }

            return applicationCountDto;

        }










        public async Task<ApplicationCountDto> SearchPages(string lang, searchDto searchDto, int currentpage, int perpage)
        {

            string draftShortcut = _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stageDraftAR) || x.Value.Contains(Constants.stageDraftEN))
                                                                  .Select(y => y.Shortcut).FirstOrDefault();
            int draftId = _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == draftShortcut).Select(y => y.Id).FirstOrDefault();

            var isemployee = (from appt in _EngineCoreDBContext.ApplicationTrack
                              join ur in _EngineCoreDBContext.UserRole on appt.UserId equals ur.UserId
                              where ur.RoleId == 17
                              select new
                              {
                                  appid = appt.ApplicationId,
                                  userid = appt.UserId,
                                  created_date = appt.CreatedDate
                              });



            var GroupappTrak = (
                from appt in isemployee
                group appt by appt.appid into t



                select new
                {
                    appid = t.Key,
                    created_date = t.Max(x => x.created_date)
                });
            var apptrak = (from appt in _EngineCoreDBContext.ApplicationTrack
                           join gt in GroupappTrak on new { colA = appt.ApplicationId, colB = appt.CreatedDate } equals new { colA = gt.appid, colB = gt.created_date }
                           select new
                           {
                               appid = appt.ApplicationId,
                               userid = appt.UserId
                           }

                );
            int userid = _iUserRepository.GetUserID();
            ApplicationCountDto applicationCountDto = new ApplicationCountDto();
            if (searchDto.onlyMyApps)
            {
                var AppStageService = // get service name and stage for each application and translate Service shortcut
                   (from app in _EngineCoreDBContext.Application
                    join pt in apptrak.Where(x=>x.userid==userid)
                    on app.Id equals pt.appid
                    join srv in _EngineCoreDBContext.AdmService
                    on app.ServiceId equals srv.Id
                    join stg in _EngineCoreDBContext.AdmStage//.Where(x => x.StageTypeId != draftId)
                    on app.CurrentStageId equals stg.Id
                    join tr in _EngineCoreDBContext.SysTranslation
                    on srv.Shortcut equals tr.Shortcut

                    where tr.Lang == lang
                    select new AllapplicationDto
                    {
                        percent =0,
                        lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                        periodLate = (int)stg.PeriodForLate,
                        appStatusId = app.StateId,
                        appid = app.Id,
                        stagetypeid = stg.StageTypeId,
                        serviceid = srv.Id,
                        servicename = tr.Value,
                        fee = srv.Fee,
                        templateid = app.TemplateId,
                        appstartdate = app.CreatedDate
                    }
                   );//.ToListAsync(); 

                //we need to get All name of template for All applications 
                //if template id ==null then template name is null by left join in query
                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id into g
                                     from q in g.DefaultIfEmpty()
                                     select new AllapplicationDto
                                     {
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         fee = q1.fee,
                                         templateid = q.Id,
                                         appstartdate = q1.appstartdate,
                                         templatename = q.TitleShortcut,
                                         doctypeid = q.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid


                                     });//.ToListAsync();
                                        //then we get translations of template name 
                                        //if template name ==null then template name translation is null by left join in query
                var querytemplatetrans = (from q1 in templateQuery
                                          join tm in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang) on q1.templatename equals tm.Shortcut into g
                                          from q in g.DefaultIfEmpty()
                                          select new AllapplicationDto
                                          {
                                              percent = q1.percent,
                                              lastUpdateStage = q1.lastUpdateStage,
                                              periodLate = q1.periodLate,
                                              appStatusId = q1.appStatusId,
                                              appid = q1.appid,
                                              serviceid = q1.serviceid,
                                              servicename = q1.servicename,
                                              fee = q1.fee,
                                              templateid = q.Id,
                                              appstartdate = q1.appstartdate,
                                              templatename = q.Value,
                                              doctypeid = q1.doctypeid,
                                              stagetypeid = q1.stagetypeid


                                          });//.ToListAsync();

                //now i need to get document name fro every app
                // if documentid==null then document name=null by left join in query
                var documentNameQuery = (
                       from qtl in querytemplatetrans
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id into g
                       from q in g.DefaultIfEmpty()


                       select new AllapplicationDto
                       {
                           percent = qtl.percent,
                           lastUpdateStage = qtl.lastUpdateStage,
                           periodLate = qtl.periodLate,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           fee = qtl.fee,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           documentname = q.Shortcut,
                           stagetypeid = qtl.stagetypeid

                       }
                       );
                //get translations of document name 
                //if  document name=null then document name translation is null by left join in query
                var querydoctrans = (
                    from q3 in documentNameQuery
                    join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang)
                    on q3.documentname equals tr.Shortcut into g
                    from q in g.DefaultIfEmpty()
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q.Value,
                        stagetypeid = q3.stagetypeid

                    }

                    );

                //get applications status translation
                var Appstatustranslate = (
                    from q3 in querydoctrans
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    join tr5 in _EngineCoreDBContext.SysTranslation
                    on lv.Shortcut equals tr5.Shortcut
                    where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        stagetypeid = q3.stagetypeid

                    });

                var extraquery = (from par in _EngineCoreDBContext.ApplicationParty
                                  join ex in _EngineCoreDBContext.ApplicationPartyExtraAttachment on par.Id equals ex.ApplicationPartyId into g
                                  from ex in g.DefaultIfEmpty()
                                  select new
                                  {
                                      id = par.Id,
                                      trancid = par.TransactionId,
                                      partyfullname = par.FullName,
                                      partyemail = par.Email,
                                      partyphone = par.Mobile,
                                      partyemirateid = par.EmiratesIdNo,
                                      partydocid = ex.Number

                                  });

                //we need to know owner of All applications
                var isownerQuery = (
                    from par in _EngineCoreDBContext.ApplicationParty
                    where par.IsOwner == true
                    select new
                    {
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile,

                    });

                var allparty = (
                    from exq in extraquery
                    join iso in isownerQuery on exq.trancid equals iso.trancId into g

                    from iso in g.DefaultIfEmpty()
                    select new
                    {
                        trancid = exq.trancid,
                        partyfullname = exq.partyfullname,
                        partyemail = exq.partyemail,
                        partyphone = exq.partyphone,
                        partyemirateid = exq.partyemirateid,
                        partyexdocid = exq.partydocid,
                        ownername = iso.fullname,
                        owneremail = iso.email,
                        ownerphone = iso.mobile,

                    }
                    );
                
                // get transaction for owner applications
                var userTransactionsQuery = (
                     from q in allparty
                     join tranc in _EngineCoreDBContext.AppTransaction on q.trancid equals tranc.Id
                     join app in apptrak.Where(x => x.userid == userid)//join app in _EngineCoreDBContext.Application//.Where(x => x.StateId == ReturnedStateId)

                     on tranc.ApplicationId equals app.appid into g

                     from app in g.DefaultIfEmpty()


                     select new AppPartyDto
                     {
                         TransactionId=tranc.TransactionNo,
                         appid = app.appid,
                         fullname = q.ownername,
                         email = q.owneremail,
                         mobile = q.ownerphone,
                         extradocid = q.partyexdocid,
                         paremirateid = q.partyemirateid,
                         paremail = q.partyemail,
                         parfullname = q.partyfullname,
                         parmobile = q.partyphone
                     });//.ToListAsync();

                //we need to know owner of All applications
                //var isownerQuery = (
                //    from par in _EngineCoreDBContext.ApplicationParty
                //    where par.IsOwner == true
                //    select new
                //    {
                //        trancId = par.TransactionId,
                //        fullname = par.FullName,
                //        email = par.Email,
                //        mobile = par.Mobile
                //    });
                // get transaction for owner applications
                //var userTransactionsQuery = (
                //     from app in apptrak //_EngineCoreDBContext.Application
                //     join tranc in _EngineCoreDBContext.AppTransaction on app.appid equals tranc.ApplicationId
                //     join q in isownerQuery on tranc.Id equals q.trancId into g

                //     from q in g.DefaultIfEmpty()


                //     select new AppPartyDto
                //     {
                //         appid = app.appid,
                //         fullname = q.fullname,
                //         email = q.email,
                //         mobile = q.mobile
                //     });//.ToListAsync();

                //apply search for All Applications
                var searchQuery = (
                    from q4 in userTransactionsQuery
                    join q5 in Appstatustranslate on q4.appid equals q5.appid

                    orderby q5.lastUpdateStage

                    where (q4.appid.ToString().Contains(searchDto.Id) || searchDto.Id == null) &&
                             (q4.parfullname.Contains(searchDto.Name) || searchDto.Name == null) &&
                             (q4.parmobile.Contains(searchDto.Phone) || searchDto.Phone == null) &&
                             (q4.paremail.Contains(searchDto.Email) || searchDto.Email == null) &&
                             // (q5.stagetypeid.ToString().Contains(searchDto.stagetypeid) || searchDto.stagetypeid == null) &&
                             (q4.paremirateid.Contains(searchDto.EmirateId) || q4.extradocid.Contains(searchDto.EmirateId) || searchDto.EmirateId == null)&&
                             (q4.TransactionId.Contains(searchDto.TransactionID)||searchDto.TransactionID==null)
                    //where (q4.appid.ToString().Contains(searchDto.Id) || searchDto.Id == null) &&
                    //       (q4.fullname.Contains(searchDto.Name) || searchDto.Name == null) &&
                    //       (q4.mobile.Contains(searchDto.Phone) || searchDto.Phone == null) &&
                    //       (q4.email.Contains(searchDto.Email) || searchDto.Email == null)
                    select new Apps
                    {
                        percent = q5.percent,
                        lastUpdateStage = q5.lastUpdateStage,
                        periodLate = q5.periodLate,
                        appStatusName = q5.appStatusName,
                        appStatusId = q5.appStatusId,
                        appid = q4.appid,
                        serviceid = q5.serviceid,
                        servicename = q5.servicename,
                        fee = q5.fee,
                        appstartdate = q5.appstartdate,
                        templateid = q5.templateid,
                        templatename = q5.templatename,
                        doctypeid = q5.doctypeid,
                        documentname = q5.documentname,
                        fullname = q4.fullname,
                        email = q4.email,
                        mobile = q4.mobile,
                        stagetypeid = q5.stagetypeid


                    }
                    );

           

             
              
                int count = searchQuery.Distinct().Count();

                //applicationCountDto.count = count;
                applicationCountDto.Applications = searchQuery.Distinct().OrderBy(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();
                //applicationCountDto.AppBySatge = finallResult.ToList();
                //applicationCountDto.Applications = searchQuery.GroupBy(z => z.appid).Select(p => p.FirstOrDefault()).ToList();//.OrderByDescending(x => x.appstartdate).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();
              //  int count = searchQuery.GroupBy(z => z.appid).Select(p => p.FirstOrDefault()).Count();
                applicationCountDto.count = count;
            }
            else if (!searchDto.onlyMyApps)
            {
                var AppStageService = // get service name and stage for each application and translate Service shortcut
                      (from app in _EngineCoreDBContext.Application.Where(x => (x.ServiceId.ToString().Contains(searchDto.serviceNo) || searchDto.serviceNo == null))
                       join srv in _EngineCoreDBContext.AdmService
                       on app.ServiceId equals srv.Id
                       join stg in _EngineCoreDBContext.AdmStage//.Where(x => x.StageTypeId != draftId)
                       on app.CurrentStageId equals stg.Id
                       join tr in _EngineCoreDBContext.SysTranslation
                       on srv.Shortcut equals tr.Shortcut

                       where tr.Lang == lang
                       select new AllapplicationDto
                       {
                           percent = 0,//(float)100 / (_EngineCoreDBContext.AdmStage.Where(x => x.ServiceId == app.ServiceId).Count() - 1) * (float)stg.OrderNo,
                        lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                           periodLate = (int)stg.PeriodForLate,
                           appStatusId = app.StateId,
                           appid = app.Id,
                           stagetypeid = stg.StageTypeId,
                           serviceid = srv.Id,
                           servicename = tr.Value,
                           fee = srv.Fee,
                           templateid = app.TemplateId,
                           appstartdate = app.CreatedDate,
                          // stagetypename = (stagetypetranslation.Where(x => x.Id == stg.StageTypeId).Select(y => y.Value).FirstOrDefault())
                       }
                      );//.ToListAsync(); 

                //we need to get All name of template for All applications 
                //if template id ==null then template name is null by left join in query
                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id into g
                                     from q in g.DefaultIfEmpty()
                                     select new AllapplicationDto
                                     {
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         fee = q1.fee,
                                         templateid = q.Id,
                                         appstartdate = q1.appstartdate,
                                         templatename = q.TitleShortcut,
                                         doctypeid = q.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid,
                                         stagetypename = q1.stagetypename

                                     });//.ToListAsync();
                                        //then we get translations of template name 
                                        //if template name ==null then template name translation is null by left join in query
                var querytemplatetrans = (from q1 in templateQuery
                                          join tm in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang) on q1.templatename equals tm.Shortcut into g
                                          from q in g.DefaultIfEmpty()
                                          select new AllapplicationDto
                                          {
                                              percent = q1.percent,
                                              lastUpdateStage = q1.lastUpdateStage,
                                              periodLate = q1.periodLate,
                                              appStatusId = q1.appStatusId,
                                              appid = q1.appid,
                                              serviceid = q1.serviceid,
                                              servicename = q1.servicename,
                                              fee = q1.fee,
                                              templateid = q.Id,
                                              appstartdate = q1.appstartdate,
                                              templatename = q.Value,
                                              doctypeid = q1.doctypeid,
                                              stagetypeid = q1.stagetypeid,
                                              stagetypename = q1.stagetypename

                                          });//.ToListAsync();

                //now i need to get document name fro every app
                // if documentid==null then document name=null by left join in query
                var documentNameQuery = (
                       from qtl in querytemplatetrans
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id into g
                       from q in g.DefaultIfEmpty()


                       select new AllapplicationDto
                       {
                           percent = qtl.percent,
                           lastUpdateStage = qtl.lastUpdateStage,
                           periodLate = qtl.periodLate,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           fee = qtl.fee,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           documentname = q.Shortcut,
                           stagetypeid = qtl.stagetypeid,
                           stagetypename = qtl.stagetypename,
                         //  UserName = employees.Contains(apptrak.Where(y => y.appid == qtl.appid).Select(z => z.userid).FirstOrDefault().Value) ?
                                         //    _EngineCoreDBContext.User.Where(x => x.Id == apptrak.Where(y => y.appid == qtl.appid).Select(z => z.userid).FirstOrDefault()).Select(p => p.FullName).FirstOrDefault() : null

                       }
                       );
                //get translations of document name 
                //if  document name=null then document name translation is null by left join in query
                var querydoctrans = (
                    from q3 in documentNameQuery
                    join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang)
                    on q3.documentname equals tr.Shortcut into g
                    from q in g.DefaultIfEmpty()
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q.Value,
                        stagetypeid = q3.stagetypeid,
                        stagetypename = q3.stagetypename,
                        UserName = q3.UserName
                    }

                    );

                //get applications status translation
                var Appstatustranslate = (
                    from q3 in querydoctrans
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    join tr5 in _EngineCoreDBContext.SysTranslation
                    on lv.Shortcut equals tr5.Shortcut
                    where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        stagetypeid = q3.stagetypeid,
                        stagetypename = q3.stagetypename,
                        UserName = q3.UserName

                    });

                //get all parties with there emirates id and extra attachment if found or not
                var extraquery = (from par in _EngineCoreDBContext.ApplicationParty
                                  join ex in _EngineCoreDBContext.ApplicationPartyExtraAttachment on par.Id equals ex.ApplicationPartyId into g
                                  from ex in g.DefaultIfEmpty()
                                  select new
                                  {
                                      id = par.Id,
                                      trancid = par.TransactionId,
                                      partyfullname = par.FullName,
                                      partyemail = par.Email,
                                      partyphone = par.Mobile,
                                      partyemirateid = par.EmiratesIdNo,
                                      partydocid = ex.Number

                                  });

                //we need to know owner of All applications
                var isownerQuery = (
                    from par in _EngineCoreDBContext.ApplicationParty
                    where par.IsOwner == true
                    select new
                    {
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile,

                    });

                var allparty = (
                    from exq in extraquery
                    join iso in isownerQuery on exq.trancid equals iso.trancId into g

                    from iso in g.DefaultIfEmpty()
                    select new
                    {
                        trancid = exq.trancid,
                        partyfullname = exq.partyfullname,
                        partyemail = exq.partyemail,
                        partyphone = exq.partyphone,
                        partyemirateid = exq.partyemirateid,
                        partyexdocid = exq.partydocid,
                        ownername = iso.fullname,
                        owneremail = iso.email,
                        ownerphone = iso.mobile,

                    }
                    );

                // get transaction for owner applications
                var userTransactionsQuery = (
                     from q in allparty
                     join tranc in _EngineCoreDBContext.AppTransaction on q.trancid equals tranc.Id
                     join app in _EngineCoreDBContext.Application//.Where(x => x.StateId != RejectedStateId)

                       on tranc.ApplicationId equals app.Id into g

                     from app in g.DefaultIfEmpty()


                     select new AppPartyDto
                     {
                         TransactionId=tranc.TransactionNo,
                         appid = app.Id,
                         fullname = q.ownername,
                         email = q.owneremail,
                         mobile = q.ownerphone,
                         extradocid = q.partyexdocid,
                         paremirateid = q.partyemirateid,
                         paremail = q.partyemail,
                         parfullname = q.partyfullname,
                         parmobile = q.partyphone
                     });//.ToListAsync();

                //apply search for All Applications
                var searchQuery = (
                    from q4 in userTransactionsQuery
                    join q5 in Appstatustranslate on q4.appid equals q5.appid

                    //orderby q5.appstartdate

                    where (q4.appid.ToString().Contains(searchDto.Id) || searchDto.Id == null) &&
                               (q4.parfullname.Contains(searchDto.Name) || searchDto.Name == null) &&
                               (q4.parmobile.Contains(searchDto.Phone) || searchDto.Phone == null) &&
                               (q4.paremail.Contains(searchDto.Email) || searchDto.Email == null) &&
                               // (q5.stagetypeid.ToString().Contains(searchDto.stagetypeid) || searchDto.stagetypeid == null) &&
                               (q4.paremirateid.Contains(searchDto.EmirateId) || q4.extradocid.Contains(searchDto.EmirateId) || searchDto.EmirateId == null)&&
                               (q4.TransactionId.Contains(searchDto.TransactionID)||searchDto.TransactionID==null)
                    select new Apps
                    {
                        percent = q5.percent,
                        lastUpdateStage = q5.lastUpdateStage,
                        periodLate = q5.periodLate,
                        appStatusName = q5.appStatusName,
                        appStatusId = q5.appStatusId,
                        appid = q4.appid,
                        serviceid = q5.serviceid,
                        servicename = q5.servicename,
                        fee = q5.fee,
                        appstartdate = q5.appstartdate,
                        templateid = q5.templateid,
                        templatename = q5.templatename,
                        doctypeid = q5.doctypeid,
                        documentname = q5.documentname,
                        fullname = q4.fullname,
                        email = q4.email,
                        mobile = q4.mobile,
                        stagetypeid = q5.stagetypeid,
                        stagetypename = q5.stagetypename,
                        UserName = q5.UserName,
                      //  islate = (q5.stagetypename.Contains(Constants.stageDoneEN) || (q5.stagetypename.Contains(Constants.stageDoneAR))) ? false : workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result,
                     //   islocked = _iApplicationRepository.ifLocked((int)q4.appid, userid, lang).Result.Id == -1


                        // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                    }
                    ).ToList();


                var searchLateQuery = (from q5 in searchQuery

                                           //orderby q5.appstartdate

                                       where (q5.islate)
                                       select new Apps
                                       {
                                           percent = q5.percent,
                                           lastUpdateStage = q5.lastUpdateStage,
                                           periodLate = q5.periodLate,
                                           appStatusName = q5.appStatusName,
                                           appStatusId = q5.appStatusId,
                                           appid = q5.appid,
                                           serviceid = q5.serviceid,
                                           servicename = q5.servicename,
                                           fee = q5.fee,
                                           appstartdate = q5.appstartdate,
                                           templateid = q5.templateid,
                                           templatename = q5.templatename,
                                           doctypeid = q5.doctypeid,
                                           documentname = q5.documentname,
                                           fullname = q5.fullname,
                                           email = q5.email,
                                           mobile = q5.mobile,
                                           UserName = q5.UserName,
                                           stagetypeid = q5.stagetypeid,
                                           islate = q5.islate,//workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result
                                           islocked = q5.islocked
                                           // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                                       }
                   );

                if (searchDto.isLate)
                {

                    applicationCountDto.Applications = searchLateQuery.GroupBy(z => z.appid).Select(p => p.FirstOrDefault()).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();
                    int count = applicationCountDto.Applications.Count();// if islate click  on card select this
                    applicationCountDto.count = count;
                }
                else
                {
                    applicationCountDto.Applications = searchQuery.GroupBy(z => z.appid).Select(p => p.FirstOrDefault()).OrderByDescending(x => x.appstartdate).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();
                    int count = searchQuery.GroupBy(z => z.appid).Select(p => p.FirstOrDefault()).Count();
                    applicationCountDto.count = count;
                }
            
        }
            return applicationCountDto;
        }

        public async Task<ApplicationCountDto>  userAppsBystage(searchDto searchDto, string lang, int currentpage, int perpage)
        {
            ApplicationCountDto applicationCountDto = new ApplicationCountDto();
            if (_iUserRepository.IsAdmin() || _iUserRepository.IsEmployee()|| _iUserRepository.IsInspector())
            {
                applicationCountDto = await AllAppByStageType(searchDto, lang, currentpage, perpage);

            }
            else
            {
                int userid = _iUserRepository.GetUserID();
                DateTime startDate = _EngineCoreDBContext.Application.Min(x => x.AppLastUpdateDate).Value;
                DateTime EDate = _EngineCoreDBContext.Application.Max(x => x.AppLastUpdateDate).Value.AddMinutes(_EngineCoreDBContext.AdmStage.Max(x => x.PeriodForLate).Value * 60);

                WorkingTimeRepository workingTimeRepository = new WorkingTimeRepository(_EngineCoreDBContext, _IGeneralRepository, _iGlobalDayOffRepository);
                await workingTimeRepository.InitialaizeWorkingDic(startDate.Date, EDate.Date);

                // this query get stage of apps with translate stage name
                var AppStageService = // get service name and stage for each app
                (from app in _EngineCoreDBContext.Application
                     //join srv in _EngineCoreDBContext.AdmService
                     //on app.ServiceId equals srv.Id
                 join stg in _EngineCoreDBContext.AdmStage
                 on app.CurrentStageId equals stg.Id
                 //join tr in _EngineCoreDBContext.SysTranslation
                 //on srv.Shortcut equals tr.Shortcut

                 //where tr.Lang == lang
                 select new AllapplicationDto
                 {

                     lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                     periodLate = (int)stg.PeriodForLate,
                     // appStatusId = app.StateId,
                     appid = app.Id,
                     stagetypeid = stg.StageTypeId,
                     //serviceid = srv.Id,
                     // servicename = tr.Value,
                     //fee = srv.Fee,
                     //templateid = app.TemplateId,
                     appstartdate = app.CreatedDate
                 }
                );


                // we need to know transactions of user
                var userTransactions = (
                    from par in _EngineCoreDBContext.ApplicationParty
                    where par.PartyId == userid
                    select new
                    {
                        trancId = (int)par.TransactionId,
                        // transactions of user
                    });

                var userTransactionsApp = (
                    from ut in userTransactions
                    join tranc in _EngineCoreDBContext.AppTransaction on ut.trancId equals tranc.Id

                    select new
                    {
                        appid = tranc.ApplicationId
                    });

                var AlluserApps = (
                    from uta in userTransactionsApp
                    join app in AppStageService on uta.appid equals app.appid
                    select new AllapplicationDto
                    {
                        lastUpdateStage = app.lastUpdateStage,
                        periodLate = app.periodLate,
                        appid = app.appid,
                        appstartdate = app.appstartdate,
                        stagetypeid = app.stagetypeid
                    });



                SysLookupType sysLookupType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "stage_type").FirstOrDefault();
                int lookupTypeId = sysLookupType.Id;


                var countAppsinStage = // number Apps in each Stage
                            (from app in AlluserApps

                             group app by app.stagetypeid into t
                             select new ApplicationByStageType
                             {
                                 Count = t.Count(),
                                 StageTypeId = (int)t.Key,


                             });

                var StageTypetranslation = // get All stage translation 
                    (from sys_lookup_value in _EngineCoreDBContext.SysLookupValue
                     join sys_translation in _EngineCoreDBContext.SysTranslation on sys_lookup_value.Shortcut equals sys_translation.Shortcut
                     where
                       sys_lookup_value.LookupTypeId == lookupTypeId &&
                       sys_translation.Lang == lang
                     select new
                     {
                         sys_translation.Value,
                         sys_translation.Lang,
                         lvshortcut = sys_lookup_value.Shortcut,
                         sys_lookup_value.LookupTypeId,
                         sys_lookup_value.Id
                     });

                var query9 =// translate current stage of apps
                    (
                    from lv in _EngineCoreDBContext.SysLookupValue
                    join t in _EngineCoreDBContext.SysTranslation
                           on lv.Shortcut equals t.Shortcut
                    join q in countAppsinStage
                            on lv.Id equals q.StageTypeId


                    where t.Lang == lang
                    select new ApplicationByStageType
                    {
                        Count = q.Count,
                        StageTypeId = q.StageTypeId,
                        StageTypeName = t.Value


                    });//.ToListAsync();

                var groupStageType = // left join to get All stage and number of apps in each stage  
                      (from q2 in StageTypetranslation
                       join q1 in query9 on q2.Id equals q1.StageTypeId into g
                       from q1 in g.DefaultIfEmpty()
                       select new ApplicationByStageType
                       {
                           StageTypeName = q2.Value,
                           StageTypeId = q2.Id,
                           Count = q1.Count,

                       });
                var finallResult = (
                    from q10 in groupStageType
                    select new ApplicationByStage
                    {
                        StageTypeId = q10.StageTypeId,
                        StageTypeName = q10.StageTypeName,
                        Count = q10.Count,
                        Appstage = (
                        from q in AlluserApps
                        where q.stagetypeid == q10.StageTypeId
                        select new Apps
                        {
                            lastUpdateStage = q.lastUpdateStage,
                            periodLate = q.periodLate,
                            appStatusName = q.appStatusName,
                            appStatusId = q.appStatusId,
                            appid = q.appid,
                            serviceid = q.serviceid,
                            servicename = q.servicename,
                            fee = q.fee,
                            appstartdate = q.appstartdate,
                            templateid = q.templateid,
                            templatename = q.templatename,
                            doctypeid = q.doctypeid,
                            documentname = q.documentname,
                            fullname = q.fullname,
                            email = q.email,
                            mobile = q.mobile,
                            stagetypeid = q.stagetypeid,
                            islate = (q10.StageTypeName.Contains(Constants.stageDoneAR) || q10.StageTypeName.Contains(Constants.stageDoneEN) ? false : workingTimeRepository.IsOrderLate(q.lastUpdateStage, DateTime.Now, q.periodLate * 60).Result)

                        }).ToList()//.Skip(perpage * (currentpage - 1)).Take(perpage).ToList()

                    }
                       );

                //applicationCountDto.count = count;
                //applicationCountDto.Applications = await result.Skip(perpage * (currentpage - 1)).Take(perpage).ToListAsync();
                applicationCountDto.AppBySatge = finallResult.ToList();
            }

            return applicationCountDto;



        }
        public async Task<ApplicationCountDto> userAllApps(searchDto searchDto, string lang, int currentpage, int perpage)
        {
            ApplicationCountDto applicationCountDto = new ApplicationCountDto();
            if (_iUserRepository.IsAdmin() || _iUserRepository.IsEmployee())
            {
                applicationCountDto = await appPages(searchDto, lang, currentpage, perpage);

            }
            else
            {
                int userid = _iUserRepository.GetUserID();
                DateTime startDate = _EngineCoreDBContext.Application.Min(x => x.AppLastUpdateDate).Value;
                DateTime EDate = _EngineCoreDBContext.Application.Max(x => x.AppLastUpdateDate).Value.AddMinutes(_EngineCoreDBContext.AdmStage.Max(x => x.PeriodForLate).Value * 60);

                WorkingTimeRepository workingTimeRepository = new WorkingTimeRepository(_EngineCoreDBContext, _IGeneralRepository, _iGlobalDayOffRepository);
                await workingTimeRepository.InitialaizeWorkingDic(startDate.Date, EDate.Date);

                // this query get stage of apps with translate stage name
                var AppStageService = // get service name and stage for each app
                (from app in _EngineCoreDBContext.Application
                 join srv in _EngineCoreDBContext.AdmService
                 on app.ServiceId equals srv.Id
                 join stg in _EngineCoreDBContext.AdmStage
                 on app.CurrentStageId equals stg.Id
                 join tr in _EngineCoreDBContext.SysTranslation
                 on srv.Shortcut equals tr.Shortcut

                 where tr.Lang == lang
                 select new AllapplicationDto
                 {
                     percent =0,
                     lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                     periodLate = (int)stg.PeriodForLate,
                     appStatusId = app.StateId,
                     appid = app.Id,
                     stagetypeid = stg.StageTypeId,
                     serviceid = srv.Id,
                     servicename = tr.Value,
                     fee = srv.Fee,
                     templateid = app.TemplateId,
                     appstartdate = app.CreatedDate
                 }
                );//.ToListAsync(); 


                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id into g
                                     from q in g.DefaultIfEmpty()
                                     select new AllapplicationDto
                                     {
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         fee = q1.fee,
                                         templateid = q.Id,
                                         appstartdate = q1.appstartdate,
                                         templatename = q.TitleShortcut,
                                         doctypeid = q.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid


                                     });//.ToListAsync();
                //get translation for template names
                var querytemplatetrans = (from q1 in templateQuery
                                          join tm in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang) on q1.templatename equals tm.Shortcut into g
                                          from q in g.DefaultIfEmpty()
                                          select new AllapplicationDto
                                          {
                                              percent = q1.percent,
                                              lastUpdateStage = q1.lastUpdateStage,
                                              periodLate = q1.periodLate,
                                              appStatusId = q1.appStatusId,
                                              appid = q1.appid,
                                              serviceid = q1.serviceid,
                                              servicename = q1.servicename,
                                              fee = q1.fee,
                                              templateid = q.Id,
                                              appstartdate = q1.appstartdate,
                                              templatename = q.Value,
                                              doctypeid = q1.doctypeid,
                                              stagetypeid = q1.stagetypeid


                                          });//.ToListAsync();
                //get document shortcut 
                //if document name is null so its shortcut is null by left join in query
                var documentNameQuery = (
                       from qtl in querytemplatetrans
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id into g
                       from q in g.DefaultIfEmpty()


                       select new AllapplicationDto
                       {
                           percent = qtl.percent,
                           lastUpdateStage = qtl.lastUpdateStage,
                           periodLate = qtl.periodLate,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           fee = qtl.fee,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           documentname = q.Shortcut,
                           stagetypeid = qtl.stagetypeid

                       }
                       );
                //get translation of document names
                //if document name is null so its translation is null by left join in query
                var querydoctrans = (
                    from q3 in documentNameQuery
                    join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang)
                    on q3.documentname equals tr.Shortcut into g
                    from q in g.DefaultIfEmpty()
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q.Value,
                        stagetypeid = q3.stagetypeid

                    }

                    );

                //get translation for all Applications status
                var appStatusTranslation = (
                    from q3 in querydoctrans
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    join tr5 in _EngineCoreDBContext.SysTranslation
                    on lv.Shortcut equals tr5.Shortcut
                    where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        stagetypeid = q3.stagetypeid

                    }
                    );
                // we need to know transactions of user
                var userTransactions = (
                    from par in _EngineCoreDBContext.ApplicationParty
                    where par.PartyId == userid
                    select new
                    {
                        trancId = (int)par.TransactionId,// transactions of user
                    });
                int count = userTransactions.Count();

                // now i need to know who is owner App 
                var isowner = (
                    from par in _EngineCoreDBContext.ApplicationParty
                    where par.IsOwner == true
                    select new
                    {
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile

                    }
                    );

                //by left join i get apps for every user and i get owner for these Apps
                var userAppwithOwner = (from iq in userTransactions
                                        join q in isowner
                                        on iq.trancId equals q.trancId into g
                                        from q in g.DefaultIfEmpty()
                                        select new
                                        {
                                            trancid = iq.trancId,
                                            fullname = q.fullname,
                                            email = q.email,
                                            mobile = q.mobile
                                        });
                //get info of user Apps Owner
                var ownerInfo = (
                     from app in _EngineCoreDBContext.Application
                     join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                     join q in userAppwithOwner on tranc.Id equals q.trancid
                     select new AppPartyDto
                     {
                         appid = app.Id,
                         fullname = q.fullname,
                         email = q.email,
                         mobile = q.mobile
                     });

                var result = (
                  from q4 in ownerInfo
                  join q5 in appStatusTranslation on q4.appid equals q5.appid


                  select new Apps
                  {
                      percent = q5.percent,
                      lastUpdateStage = q5.lastUpdateStage,
                      periodLate = q5.periodLate,
                      appStatusName = q5.appStatusName,
                      appStatusId = q5.appStatusId,
                      appid = q4.appid,
                      serviceid = q5.serviceid,
                      servicename = q5.servicename,
                      fee = q5.fee,
                      appstartdate = q5.appstartdate,
                      templateid = q5.templateid,
                      templatename = q5.templatename,
                      doctypeid = q5.doctypeid,
                      documentname = q5.documentname,
                      fullname = q4.fullname,
                      email = q4.email,
                      mobile = q4.mobile,
                      stagetypeid = q5.stagetypeid


                  }
                  );

                //SysLookupType sysLookupType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "stage_type").FirstOrDefault();
                //int lookupTypeId = sysLookupType.Id;


                //var countAppsinStage = // number Apps in each Stage
                //            (from app in result

                //             group app by app.stagetypeid into t
                //             select new ApplicationByStageType
                //             {
                //                 Count = t.Count(),
                //                 StageTypeId = (int)t.Key,


                //             });

                //var StageTypetranslation = // get All stage translation 
                //    (from sys_lookup_value in _EngineCoreDBContext.SysLookupValue
                //     join sys_translation in _EngineCoreDBContext.SysTranslation on sys_lookup_value.Shortcut equals sys_translation.Shortcut
                //     where
                //       sys_lookup_value.LookupTypeId == lookupTypeId &&
                //       sys_translation.Lang == lang
                //     select new
                //     {
                //         sys_translation.Value,
                //         sys_translation.Lang,
                //         lvshortcut = sys_lookup_value.Shortcut,
                //         sys_lookup_value.LookupTypeId,
                //         sys_lookup_value.Id
                //     });

                //var query9 =// translate current stage of apps
                //    (
                //    from lv in _EngineCoreDBContext.SysLookupValue
                //    join t in _EngineCoreDBContext.SysTranslation
                //           on lv.Shortcut equals t.Shortcut
                //    join q in countAppsinStage
                //            on lv.Id equals q.StageTypeId


                //    where t.Lang == lang
                //    select new ApplicationByStageType
                //    {
                //        Count = q.Count,
                //        StageTypeId = q.StageTypeId,
                //        StageTypeName = t.Value


                //    });//.ToListAsync();

                //var groupStageType = // left join to get All stage and number of apps in each stage  
                //      (from q2 in StageTypetranslation
                //       join q1 in query9 on q2.Id equals q1.StageTypeId into g
                //       from q1 in g.DefaultIfEmpty()
                //       select new ApplicationByStageType
                //       {
                //           StageTypeName = q2.Value,
                //           StageTypeId = q2.Id,
                //           Count = q1.Count,

                //       });
                //var finallResult = (
                //    from q10 in groupStageType
                //    select new ApplicationByStage
                //    {
                //        StageTypeId = q10.StageTypeId,
                //        StageTypeName = q10.StageTypeName,
                //        Count = q10.Count,
                //        Appstage = (
                //        from q in result
                //        where q.stagetypeid == q10.StageTypeId
                //        select new Apps
                //        {
                //            lastUpdateStage = q.lastUpdateStage,
                //            periodLate = q.periodLate,
                //            appStatusName = q.appStatusName,
                //            appStatusId = q.appStatusId,
                //            appid = q.appid,
                //            serviceid = q.serviceid,
                //            servicename = q.servicename,
                //            fee = q.fee,
                //            appstartdate = q.appstartdate,
                //            templateid = q.templateid,
                //            templatename = q.templatename,
                //            doctypeid = q.doctypeid,
                //            documentname = q.documentname,
                //            fullname = q.fullname,
                //            email = q.email,
                //            mobile = q.mobile,
                //            stagetypeid = q.stagetypeid,
                //            islate = (q10.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false : workingTimeRepository.IsOrderLate(q.lastUpdateStage, DateTime.Now, q.periodLate * 60).Result)

                //        }).ToList()

                //    }
                //       );

                applicationCountDto.count = count;
                applicationCountDto.Applications = await result.OrderBy(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToListAsync();
                //  applicationCountDto.AppBySatge = finallResult.ToList();
            }

            return applicationCountDto;
        }

        public async Task<ApplicationCountDto> SearchUserPages(string lang, searchDto searchDto, int currentpage, int perpage)
        {
            ApplicationCountDto applicationCountDto = new ApplicationCountDto();
            if (_iUserRepository.IsAdmin() || _iUserRepository.IsEmployee())
            {
                applicationCountDto = await SearchPages(lang, searchDto, currentpage, perpage);

            }
            else
            {
                int userid = _iUserRepository.GetUserID();
                DateTime startDate = _EngineCoreDBContext.Application.Min(x => x.AppLastUpdateDate).Value;
                DateTime EDate = _EngineCoreDBContext.Application.Max(x => x.AppLastUpdateDate).Value.AddMinutes(_EngineCoreDBContext.AdmStage.Max(x => x.PeriodForLate).Value * 60);

                WorkingTimeRepository workingTimeRepository = new WorkingTimeRepository(_EngineCoreDBContext, _IGeneralRepository, _iGlobalDayOffRepository);
                await workingTimeRepository.InitialaizeWorkingDic(startDate.Date, EDate.Date);

                // this query get stage of apps with translate stage name
                var appStageService = // get service name and stage for each app
                (from app in _EngineCoreDBContext.Application
                 join srv in _EngineCoreDBContext.AdmService
                 on app.ServiceId equals srv.Id
                 join stg in _EngineCoreDBContext.AdmStage
                 on app.CurrentStageId equals stg.Id
                 join tr in _EngineCoreDBContext.SysTranslation
                 on srv.Shortcut equals tr.Shortcut

                 where tr.Lang == lang
                 select new AllapplicationDto
                 {
                     percent =0,
                     lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                     periodLate = (int)stg.PeriodForLate,
                     appStatusId = app.StateId,
                     appid = app.Id,
                     stagetypeid = stg.StageTypeId,
                     serviceid = srv.Id,
                     servicename = tr.Value,
                     fee = srv.Fee,
                     templateid = app.TemplateId,
                     appstartdate = app.CreatedDate
                 }
                );//.ToListAsync(); 


                var templateQuery = (from q1 in appStageService
                                     join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id into g
                                     from q in g.DefaultIfEmpty()
                                     select new AllapplicationDto
                                     {
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         fee = q1.fee,
                                         templateid = q.Id,
                                         appstartdate = q1.appstartdate,
                                         templatename = q.TitleShortcut,
                                         doctypeid = q.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid


                                     });//.ToListAsync();

                var querytemplatetrans = (from q1 in templateQuery
                                          join tm in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang) on q1.templatename equals tm.Shortcut into g
                                          from q in g.DefaultIfEmpty()
                                          select new AllapplicationDto
                                          {
                                              percent = q1.percent,
                                              lastUpdateStage = q1.lastUpdateStage,
                                              periodLate = q1.periodLate,
                                              appStatusId = q1.appStatusId,
                                              appid = q1.appid,
                                              serviceid = q1.serviceid,
                                              servicename = q1.servicename,
                                              fee = q1.fee,
                                              templateid = q.Id,
                                              appstartdate = q1.appstartdate,
                                              templatename = q.Value,
                                              doctypeid = q1.doctypeid,
                                              stagetypeid = q1.stagetypeid


                                          });//.ToListAsync();

                var documentNamesQuery = (
                       from qtl in querytemplatetrans
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id into g
                       from q in g.DefaultIfEmpty()


                       select new AllapplicationDto
                       {
                           percent = qtl.percent,
                           lastUpdateStage = qtl.lastUpdateStage,
                           periodLate = qtl.periodLate,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           fee = qtl.fee,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           documentname = q.Shortcut,
                           stagetypeid = qtl.stagetypeid

                       }
                       );

                var querydoctrans = (
                    from q3 in documentNamesQuery
                    join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang)
                    on q3.documentname equals tr.Shortcut into g
                    from q in g.DefaultIfEmpty()
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q.Value,
                        stagetypeid = q3.stagetypeid

                    }

                    );


                var appstatustranslation = (
                    from q3 in querydoctrans
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    join tr5 in _EngineCoreDBContext.SysTranslation
                    on lv.Shortcut equals tr5.Shortcut
                    where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        stagetypeid = q3.stagetypeid

                    });
                // we need to know transactions of user
                var userTransactions = (
                    from par in _EngineCoreDBContext.ApplicationParty
                    where par.PartyId == userid
                    select new
                    {
                        trancId = (int)par.TransactionId,// transactions of user
                    });
                int count = userTransactions.Count();

                // now i need to know who is owner App 
                var isowner = (
                    from par in _EngineCoreDBContext.ApplicationParty
                    where par.IsOwner == true
                    select new
                    {
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile

                    }
                    );

                //by left join i get apps for every user and i get owner for these Apps
                var userAppwithOwner = (from iq in userTransactions
                                        join q in isowner
                                        on iq.trancId equals q.trancId into g
                                        from q in g.DefaultIfEmpty()
                                        select new
                                        {
                                            trancid = iq.trancId,
                                            fullname = q.fullname,
                                            email = q.email,
                                            mobile = q.mobile
                                        });
                //get info of user Apps Owner
                var ownerInfo = (
                     from app in _EngineCoreDBContext.Application
                     join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                     join q in userAppwithOwner on tranc.Id equals q.trancid
                     select new AppPartyDto
                     {
                         appid = app.Id,
                         fullname = q.fullname,
                         email = q.email,
                         mobile = q.mobile
                     });

                var result = (
                  from q4 in ownerInfo
                  join q5 in appstatustranslation on q4.appid equals q5.appid

                  where (q4.appid.ToString().Contains(searchDto.Id) || searchDto.Id == null) &&
                      (q4.fullname.Contains(searchDto.Name) || searchDto.Name == null) &&
                      (q4.email.Contains(searchDto.Email) || searchDto.Email == null) &&
                      (q4.mobile.Contains(searchDto.Phone) || searchDto.Phone == null)

                  select new Apps
                  {
                      percent = q5.percent,
                      lastUpdateStage = q5.lastUpdateStage,
                      periodLate = q5.periodLate,
                      appStatusName = q5.appStatusName,
                      appStatusId = q5.appStatusId,
                      appid = q4.appid,
                      serviceid = q5.serviceid,
                      servicename = q5.servicename,
                      fee = q5.fee,
                      appstartdate = q5.appstartdate,
                      templateid = q5.templateid,
                      templatename = q5.templatename,
                      doctypeid = q5.doctypeid,
                      documentname = q5.documentname,
                      fullname = q4.fullname,
                      email = q4.email,
                      mobile = q4.mobile,
                      stagetypeid = q5.stagetypeid


                  }
                  );

                //SysLookupType sysLookupType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "stage_type").FirstOrDefault();
                //int lookupTypeId = sysLookupType.Id;


                //var countAppinStage = // number Apps in each Stage
                //            (from app in result

                //             group app by app.stagetypeid into t
                //             select new ApplicationByStageType
                //             {
                //                 Count = t.Count(),
                //                 StageTypeId = (int)t.Key,


                //             });

                //var stageTypeTranslation = // get All stage translation 
                //    (from sys_lookup_value in _EngineCoreDBContext.SysLookupValue
                //     join sys_translation in _EngineCoreDBContext.SysTranslation on sys_lookup_value.Shortcut equals sys_translation.Shortcut
                //     where
                //       sys_lookup_value.LookupTypeId == lookupTypeId &&
                //       sys_translation.Lang == lang
                //     select new
                //     {
                //         sys_translation.Value,
                //         sys_translation.Lang,
                //         lvshortcut = sys_lookup_value.Shortcut,
                //         sys_lookup_value.LookupTypeId,
                //         sys_lookup_value.Id
                //     });

                //var currentAppstagetype =// translate current stage of apps
                //    (
                //    from lv in _EngineCoreDBContext.SysLookupValue
                //    join t in _EngineCoreDBContext.SysTranslation
                //           on lv.Shortcut equals t.Shortcut
                //    join q in countAppinStage
                //            on lv.Id equals q.StageTypeId


                //    where t.Lang == lang
                //    select new ApplicationByStageType
                //    {
                //        Count = q.Count,
                //        StageTypeId = q.StageTypeId,
                //        StageTypeName = t.Value


                //    });//.ToListAsync();

                //var groupstagetype = // left join to get All stage and number of apps in each stage  
                //      (from q2 in stageTypeTranslation
                //       join q1 in currentAppstagetype on q2.Id equals q1.StageTypeId into g
                //       from q1 in g.DefaultIfEmpty()
                //       select new ApplicationByStageType
                //       {
                //           StageTypeName = q2.Value,
                //           StageTypeId = q2.Id,
                //           Count = q1.Count,

                //       });
                //var finallResult = (
                //    from q10 in groupstagetype
                //    select new ApplicationByStage
                //    {
                //        StageTypeId = q10.StageTypeId,
                //        StageTypeName = q10.StageTypeName,
                //        Count = q10.Count,
                //        Appstage = (
                //        from q in result
                //        where q.stagetypeid == q10.StageTypeId
                //        select new Apps
                //        {
                //            lastUpdateStage = q.lastUpdateStage,
                //            periodLate = q.periodLate,
                //            appStatusName = q.appStatusName,
                //            appStatusId = q.appStatusId,
                //            appid = q.appid,
                //            serviceid = q.serviceid,
                //            servicename = q.servicename,
                //            fee = q.fee,
                //            appstartdate = q.appstartdate,
                //            templateid = q.templateid,
                //            templatename = q.templatename,
                //            doctypeid = q.doctypeid,
                //            documentname = q.documentname,
                //            fullname = q.fullname,
                //            email = q.email,
                //            mobile = q.mobile,
                //            stagetypeid = q.stagetypeid,
                //            islate = (q10.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false : workingTimeRepository.IsOrderLate(q.lastUpdateStage, DateTime.Now, q.periodLate * 60).Result)

                //        }).ToList()

                //    }
                //       );

                applicationCountDto.count = count;
                applicationCountDto.Applications = await result.OrderBy(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToListAsync();
                //   applicationCountDto.AppBySatge = finallResult.ToList();
            }

            return applicationCountDto;
        }

        public async Task<ApplicationCountDto> SearchByStage(string lang, searchDto searchDto, int currentpage, int perpage)
        {
            int AutoCancelledStageId = await _ISysValueRepository.GetIdByShortcut("SysCancelled");
            int AutoCancelId = await _ISysValueRepository.GetIdByShortcut("AutoCancel");
            int RejectedStateId = await _ISysValueRepository.GetIdByShortcut("REJECTED");
            int ReturnedStateId = await _ISysValueRepository.GetIdByShortcut("RETURNED");
            int DoneStateId = await _ISysValueRepository.GetIdByShortcut("DONE");
            int ReviewStageTypeId = await _ISysValueRepository.GetIdByShortcut("REVIEW");
            string draftshorcut = _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stageDraftAR) || x.Value.ToLower()
                                                          .Contains(Constants.stageDraftEN)).Select(y => y.Shortcut).FirstOrDefault();
            int draftId = _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == draftshorcut).Select(y => y.Id).FirstOrDefault();
            List<int> employees = new List<int>(_iUserRepository.GetEmployees().Result.Keys);
            int PayStageTypeId = await _ISysValueRepository.GetIdByShortcut("SysLookupValue_Shortcut52");
            int DoneStageTypeId = await _ISysValueRepository.GetIdByShortcut("SysLookupValue_Shortcut677");
            int MeetingStageId = await getShortCutStageByTranslate("en", Constants.stageMeetingEN);
            SysLookupType sysLookupType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "stage_type").FirstOrDefault();
            int lookupTypeId = sysLookupType.Id;
            int RejectedStageId = await _ISysValueRepository.GetIdByShortcut("REJECTEDSTAGE");

           
           
            var stagetypetranslation = // get All stage type translation in system lookup type table
                (from sys_lookup_value in _EngineCoreDBContext.SysLookupValue
                 join sys_translation in _EngineCoreDBContext.SysTranslation on sys_lookup_value.Shortcut equals sys_translation.Shortcut
                 where
                   sys_lookup_value.LookupTypeId == lookupTypeId &&
                   sys_translation.Lang == lang


                 select new
                 {
                     sys_translation.Value,
                     sys_translation.Lang,
                     lvshortcut = sys_lookup_value.Shortcut,
                     sys_lookup_value.LookupTypeId,
                     sys_lookup_value.Id,
                     percent=sys_lookup_value.Order
                 });






                
            //int userid = _iUserRepository.GetUserID();
            ApplicationCountDto applicationCountDto = new ApplicationCountDto();

            if (searchDto.onlyMyApps)
            {
                applicationCountDto = await NotarySearchByStage(lang, searchDto, currentpage, perpage);
            }
            
            else
            if (searchDto.stagetypeid == draftId.ToString())
            {

              

       var isemployee = (from appt in _EngineCoreDBContext.ApplicationTrack
                                  join ur in _EngineCoreDBContext.UserRole on appt.UserId equals ur.UserId
                                  where ur.RoleId == 17
                                  select new
                                  {
                                      appid = appt.ApplicationId,
                                      userid = appt.UserId,
                                      created_date = appt.CreatedDate
                                  });



                var GroupappTrak = (
                    from appt in isemployee
                    group appt by appt.appid into t




                    select new
                    {
                        appid = t.Key,
                        created_date = t.Max(x => x.created_date)
                    });
                var apptrak = (from appt in _EngineCoreDBContext.ApplicationTrack
                               join gt in GroupappTrak on new { colA = appt.ApplicationId, colB = appt.CreatedDate } equals new { colA = gt.appid, colB = gt.created_date }
                               join us in _EngineCoreDBContext.User on appt.UserId equals us.Id
                               select new
                               {
                                   username = us.FullName,
                                   appid = appt.ApplicationId,
                                   userid = appt.UserId
                               });
                var AppStageService = // get service name and stage for each application and translate Service shortcut
                   (from app in _EngineCoreDBContext.Application.Where(x=>x.Id>0 && x.StateId == ReturnedStateId)
                    join srv in _EngineCoreDBContext.AdmService
                    on app.ServiceId equals srv.Id
                    join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId == draftId)
                    on app.CurrentStageId equals stg.Id
                    join tr in _EngineCoreDBContext.SysTranslation
                    on srv.Shortcut equals tr.Shortcut

                    where tr.Lang == lang
                    select new AllapplicationDto
                    {
                        LastDateReader=(DateTime)app.LastReadDate,
                        percent = 0,
                        lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                        periodLate = (int)stg.PeriodForLate,
                        appStatusId = app.StateId,
                        appid = app.Id,
                        stagetypeid = stg.StageTypeId,
                        serviceid = srv.Id,
                        servicename = tr.Value,
                        fee = srv.Fee,
                        templateid = app.TemplateId,
                        appstartdate = app.CreatedDate,
                        stagetypename = (stagetypetranslation.Where(x => x.Id == stg.StageTypeId).Select(y => y.Value).FirstOrDefault())
                    }
                   );//.ToListAsync(); 

                //we need to get All name of template for All applications 
                //if template id ==null then template name is null by left join in query
                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template
                                     on q1.templateid equals tm.Id 
                                     join tr in _EngineCoreDBContext.SysTranslation
                                     on tm.TitleShortcut equals tr.Shortcut
                                     where tr.Lang==lang
                                     select new AllapplicationDto
                                     {
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         fee = q1.fee,
                                         templateid = q1.templateid,
                                         appstartdate = q1.appstartdate,
                                         templatename = tr.Value,
                                         doctypeid = tm.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid,
                                         stagetypename = q1.stagetypename,
                                         UserName = employees.Contains(apptrak.Where(y => y.appid == q1.appid).Select(z => z.userid).FirstOrDefault().Value) ?
                                              _EngineCoreDBContext.User.Where(x => x.Id == apptrak.Where(y => y.appid == q1.appid).Select(z => z.userid).FirstOrDefault()).Select(p => p.FullName).FirstOrDefault() : null


                                     });

                //now i need to get document name fro every app
                // if documentid==null then document name=null by left join in query
                var documentNameQuery = (
                       from qtl in templateQuery
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id //into g
                       //from q in g.DefaultIfEmpty()
                       join tr in _EngineCoreDBContext.SysTranslation 
                       on lv.Shortcut equals tr.Shortcut
                       where tr.Lang==lang

                       select new AllapplicationDto
                       {
                           percent = qtl.percent,
                           lastUpdateStage = qtl.lastUpdateStage,
                           periodLate = qtl.periodLate,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           fee = qtl.fee,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           documentname = tr.Value,
                           stagetypeid = qtl.stagetypeid,
                           stagetypename = qtl.stagetypename,
                           UserName = qtl.UserName
                       }
                       );
              
                //get applications status translation
                var Appstatustranslate = (
                    from q3 in documentNameQuery
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    join tr5 in _EngineCoreDBContext.SysTranslation
                    on lv.Shortcut equals tr5.Shortcut
                    where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        stagetypeid = q3.stagetypeid,
                        stagetypename = q3.stagetypename,
                        UserName = q3.UserName

                    });
                //we need to know owner of All applications
                var isownerQuery = (
                    from tranc in _EngineCoreDBContext.AppTransaction
                    join par in _EngineCoreDBContext.ApplicationParty on tranc.Id equals par.TransactionId
                    where par.IsOwner == true
                    select new
                    {
                        appid=tranc.ApplicationId,
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile
                    });
             

                //apply search for All Applications
                var searchQuery = (
                    from q4 in isownerQuery
                    join q5 in Appstatustranslate on q4.appid equals q5.appid

                  
                    select new Apps
                    {
                        percent = q5.percent,
                        lastUpdateStage = q5.lastUpdateStage,
                        periodLate = q5.periodLate,
                        appStatusName = q5.appStatusName,
                        appStatusId = q5.appStatusId,
                        appid = q4.appid,
                        serviceid = q5.serviceid,
                        servicename = q5.servicename,
                        fee = q5.fee,
                        appstartdate = q5.appstartdate,
                        templateid = q5.templateid,
                        templatename = q5.templatename,
                        doctypeid = q5.doctypeid,
                        documentname = q5.documentname,
                        fullname = q4.fullname,
                        email = q4.email,
                        mobile = q4.mobile,
                        stagetypeid = q5.stagetypeid,
                        stagetypename = q5.stagetypename,
                        UserName = q5.UserName
                    
                    }
                    ).ToList();



                    int count = searchQuery.Count();
                    applicationCountDto.count = count; // if late not clicked on card select this 
                    applicationCountDto.Applications = searchQuery.OrderBy(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();
                

            }
            else if(searchDto.stagetypeid==DoneStageTypeId.ToString())
            {
              

                var AppStageService = // get service name and stage for each application and translate Service shortcut
                   (from app in _EngineCoreDBContext.Application.Where(x => x.Id > 0 && x.StateId != RejectedStateId && x.StateId!=AutoCancelId)
                    join srv in _EngineCoreDBContext.AdmService
                    on app.ServiceId equals srv.Id
                    join stg in _EngineCoreDBContext.AdmStage.Where(x=>x.StageTypeId==DoneStageTypeId)
                    on app.CurrentStageId equals stg.Id
                    join tr in _EngineCoreDBContext.SysTranslation
                    on srv.Shortcut equals tr.Shortcut

                    where tr.Lang == lang
                    select new AllapplicationDto
                    {
                        LastDateReader=(DateTime)app.LastReadDate,
                        percent = 100,
                        lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                        periodLate = (int)stg.PeriodForLate,
                        appStatusId = app.StateId,
                        appid = app.Id,
                        stagetypeid = stg.StageTypeId,
                        serviceid = srv.Id,
                        servicename = tr.Value,
                        fee = srv.Fee,
                        templateid = app.TemplateId,
                        appstartdate = app.CreatedDate,
                        stagetypename = (stagetypetranslation.Where(x => x.Id == stg.StageTypeId).Select(y => y.Value).FirstOrDefault())
                    }
                   );


                //we need to get All name of template for All applications 
                //if template id ==null then template name is null by left join in query
                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template
                                     on q1.templateid equals tm.Id // into g
                                   //  from q in g.DefaultIfEmpty()
                                     join tr in _EngineCoreDBContext.SysTranslation 
                                     on tm.TitleShortcut equals tr.Shortcut
                                     where tr.Lang==lang
                                     select new AllapplicationDto
                                     {
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         fee = q1.fee,
                                         templateid = q1.templateid,
                                         appstartdate = q1.appstartdate,
                                         templatename = tr.Value,
                                         doctypeid = tm.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid,
                                         stagetypename = q1.stagetypename,
                                         //UserName =q1.appid>0?employees.Contains(apptrak.Where(y => y.appid == q1.appid).Select(z => z.userid).FirstOrDefault().Value) ?
                                         //_EngineCoreDBContext.User.Where(x => x.Id == apptrak.Where(y => y.appid == q1.appid).Select(z => z.userid).FirstOrDefault()).Select(p => p.FullName).FirstOrDefault() : null:null





                                     });

                //now i need to get document name fro every app
                // if documentid==null then document name=null by left join in query
                var documentNameQuery = (
                       from qtl in templateQuery
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id //into g
                       //from q in g.DefaultIfEmpty()
                       join tr in _EngineCoreDBContext.SysTranslation 
                       on lv.Shortcut equals tr.Shortcut
                       where tr.Lang==lang

                       select new AllapplicationDto
                       {
                           percent = qtl.percent,
                           lastUpdateStage = qtl.lastUpdateStage,
                           periodLate = qtl.periodLate,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           fee = qtl.fee,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           documentname = tr.Value,
                           stagetypeid = qtl.stagetypeid,
                           stagetypename = qtl.stagetypename,
                           UserName = qtl.UserName
                       }
                       );
             
                //get applications status translation
                var Appstatustranslate = (
                    from q3 in documentNameQuery
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    join tr5 in _EngineCoreDBContext.SysTranslation
                    on lv.Shortcut equals tr5.Shortcut
                    where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        stagetypeid = q3.stagetypeid,
                        stagetypename = q3.stagetypename,
                        UserName = q3.UserName


                    });
                //we need to know owner of All applications
                var isownerQuery = (
                    from tranc in _EngineCoreDBContext.AppTransaction
                    join  par in _EngineCoreDBContext.ApplicationParty
                    on tranc.Id equals par.TransactionId
                    where par.IsOwner == true
                    select new
                    {
                        appid=tranc.ApplicationId,
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile
                    });
             

                //apply search for All Applications
                var searchQuery = (
                    from q4 in isownerQuery
                    join q5 in Appstatustranslate on q4.appid equals q5.appid

                 
                    select new Apps
                    {
                        UserName = q5.UserName,
                        percent = q5.percent,
                        lastUpdateStage = q5.lastUpdateStage,
                        periodLate = q5.periodLate,
                        appStatusName = q5.appStatusName,
                        appStatusId = q5.appStatusId,
                        appid = q4.appid,
                        serviceid = q5.serviceid,
                        servicename = q5.servicename,
                        fee = q5.fee,
                        appstartdate = q5.appstartdate,
                        templateid = q5.templateid,
                        templatename = q5.templatename,
                        doctypeid = q5.doctypeid,
                        documentname = q5.documentname,
                        fullname = q4.fullname,
                        email = q4.email,
                        mobile = q4.mobile,
                        stagetypeid = q5.stagetypeid,
                        stagetypename = q5.stagetypename,
                       // islate = (q5.stagetypename.Contains(Constants.stageDoneEN) || (q5.stagetypename.Contains(Constants.stageDoneAR))) ? false : workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result,
                        ///islocked = _iApplicationRepository.IfBookedUp((int)q4.appid, userid, lang).Result.Id == -1


                        // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                    }
                    ).ToList();



               
                    int count = searchQuery.Count();
                    applicationCountDto.count = count; // if late not clicked on card select this 
                    applicationCountDto.Applications = searchQuery.OrderByDescending(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

               
            }
            else if(searchDto.stagetypeid==ReviewStageTypeId.ToString() && (_iUserRepository.IsAdmin() ||_iUserRepository.IsInspector()))
            {

                List<int> StagesHours = await _EngineCoreDBContext.AdmStage.Select(x => (int)x.PeriodForLate).Distinct().ToListAsync();
                Dictionary<int, DateTime> lateDate = await _IWorkingTimeRepository.GetDeadline(StagesHours.Distinct().ToList());


                var AppStageService = // get service name and stage for each application and translate Service shortcut
                       (from app in _EngineCoreDBContext.Application.Where(x => x.Id > 0 && x.StateId !=RejectedStateId)
                        join srv in _EngineCoreDBContext.AdmService
                        on app.ServiceId equals srv.Id
                        join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId == ReviewStageTypeId)
                        on app.CurrentStageId equals stg.Id
                        join tr in _EngineCoreDBContext.SysTranslation
                        on srv.Shortcut equals tr.Shortcut

                        where tr.Lang == lang
                        select new AllapplicationDto
                        {
                            LastDateReader = (DateTime)app.LastReadDate,
                            percent = 0,
                            lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                            periodLate = (int)stg.PeriodForLate,
                            appStatusId = app.StateId,
                            appid = app.Id,
                            stagetypeid = stg.StageTypeId,
                            serviceid = srv.Id,
                            servicename = tr.Value,
                            fee = srv.Fee,
                            templateid = app.TemplateId,
                            appstartdate = app.CreatedDate,
                            stagetypename = (stagetypetranslation.Where(x => x.Id == stg.StageTypeId).Select(y => y.Value).FirstOrDefault())
                        }
                       );//.ToListAsync(); 

                    //we need to get All name of template for All applications 
                    //if template id ==null then template name is null by left join in query
                    var templateQuery = (from q1 in AppStageService
                                         join tm in _EngineCoreDBContext.Template
                                         on q1.templateid equals tm.Id
                                         join tr in _EngineCoreDBContext.SysTranslation
                                         on tm.TitleShortcut equals tr.Shortcut
                                         where tr.Lang == lang
                                         select new AllapplicationDto
                                         {
                                             percent = q1.percent,
                                             lastUpdateStage = q1.lastUpdateStage,
                                             periodLate = q1.periodLate,
                                             appStatusId = q1.appStatusId,
                                             appid = q1.appid,
                                             serviceid = q1.serviceid,
                                             servicename = q1.servicename,
                                             fee = q1.fee,
                                             templateid = q1.templateid,
                                             appstartdate = q1.appstartdate,
                                             templatename = tr.Value,
                                             doctypeid = tm.DocumentTypeId,
                                             stagetypeid = q1.stagetypeid,
                                             stagetypename = q1.stagetypename,
                                             //UserName = employees.Contains(apptrak.Where(y => y.appid == q1.appid).Select(z => z.userid).FirstOrDefault().Value) ?
                                             //     _EngineCoreDBContext.User.Where(x => x.Id == apptrak.Where(y => y.appid == q1.appid).Select(z => z.userid).FirstOrDefault()).Select(p => p.FullName).FirstOrDefault() : null


                                         });

                    //now i need to get document name fro every app
                    // if documentid==null then document name=null by left join in query
                    var documentNameQuery = (
                           from qtl in templateQuery
                           join lv in _EngineCoreDBContext.SysLookupValue
                           on qtl.doctypeid equals lv.Id //into g
                                                         //from q in g.DefaultIfEmpty()
                       join tr in _EngineCoreDBContext.SysTranslation
                           on lv.Shortcut equals tr.Shortcut
                           where tr.Lang == lang

                           select new AllapplicationDto
                           {
                               percent = qtl.percent,
                               lastUpdateStage = qtl.lastUpdateStage,
                               periodLate = qtl.periodLate,
                               appStatusId = qtl.appStatusId,
                               appid = qtl.appid,
                               serviceid = qtl.serviceid,
                               servicename = qtl.servicename,
                               fee = qtl.fee,
                               templateid = qtl.templateid,
                               appstartdate = qtl.appstartdate,
                               templatename = qtl.templatename,
                               doctypeid = qtl.doctypeid,
                               documentname = tr.Value,
                               stagetypeid = qtl.stagetypeid,
                               stagetypename = qtl.stagetypename,
                               UserName = qtl.UserName
                           }
                           );

                    //get applications status translation
                    //var Appstatustranslate = (
                    //    from q3 in documentNameQuery
                    //    join lv in _EngineCoreDBContext.SysLookupValue
                    //    on q3.appStatusId equals lv.Id
                    //    join tr5 in _EngineCoreDBContext.SysTranslation
                    //    on lv.Shortcut equals tr5.Shortcut
                    //    where tr5.Lang == lang
                    //    select new AllapplicationDto
                    //    {
                    //        percent = q3.percent,
                    //        lastUpdateStage = q3.lastUpdateStage,
                    //        periodLate = q3.periodLate,
                    //        appStatusName = tr5.Value,
                    //        appStatusId = q3.appStatusId,
                    //        appid = q3.appid,
                    //        serviceid = q3.serviceid,
                    //        servicename = q3.servicename,
                    //        fee = q3.fee,
                    //        templateid = q3.templateid,
                    //        appstartdate = q3.appstartdate,
                    //        templatename = q3.templatename,
                    //        doctypeid = q3.doctypeid,
                    //        documentname = q3.documentname,
                    //        stagetypeid = q3.stagetypeid,
                    //        stagetypename = q3.stagetypename,
                    //        UserName = q3.UserName

                    //    });
                    //we need to know owner of All applications
                    var isownerQuery = (
                        from tranc in _EngineCoreDBContext.AppTransaction
                        join par in _EngineCoreDBContext.ApplicationParty on tranc.Id equals par.TransactionId
                        where par.IsOwner == true
                        select new
                        {
                            appid = tranc.ApplicationId,
                            trancId = par.TransactionId,
                            fullname = par.FullName,
                            email = par.Email,
                            mobile = par.Mobile
                        });


                    //apply search for All Applications
                    var searchQuery = (
                        from q4 in isownerQuery
                        join q5 in documentNameQuery on q4.appid equals q5.appid


                        select new Apps
                        {
                            percent = q5.percent,
                            lastUpdateStage = q5.lastUpdateStage,
                            periodLate = q5.periodLate,
                            appStatusName = q5.appStatusName,
                            appStatusId = q5.appStatusId,
                            appid = q4.appid,
                            serviceid = q5.serviceid,
                            servicename = q5.servicename,
                            fee = q5.fee,
                            appstartdate = q5.appstartdate,
                            templateid = q5.templateid,
                            templatename = q5.templatename,
                            doctypeid = q5.doctypeid,
                            documentname = q5.documentname,
                            fullname = q4.fullname,
                            email = q4.email,
                            mobile = q4.mobile,
                            stagetypeid = q5.stagetypeid,
                            stagetypename = q5.stagetypename,
                            UserName = q5.UserName,
                            islocked = q4.appid > 0 ? DateTime.Now.Subtract((DateTime)q5.LastDateReader).TotalSeconds < Constants.LOCK_SECONDS_TIME ? true : false : false,
                            islate = q5.lastUpdateStage >= lateDate[q5.periodLate] ? false : true,


                           
                        }
                        ).ToList();



                    var searchLateQuery = (from q5 in searchQuery

                                             

                                           where (q5.islate)
                                           select new Apps
                                           {
                                               percent = q5.percent,
                                               lastUpdateStage = q5.lastUpdateStage,
                                               periodLate = q5.periodLate,
                                               appStatusName = q5.appStatusName,
                                               appStatusId = q5.appStatusId,
                                               appid = q5.appid,
                                               serviceid = q5.serviceid,
                                               servicename = q5.servicename,
                                               fee = q5.fee,
                                               appstartdate = q5.appstartdate,
                                               templateid = q5.templateid,
                                               templatename = q5.templatename,
                                               doctypeid = q5.doctypeid,
                                               documentname = q5.documentname,
                                               fullname = q5.fullname,
                                               email = q5.email,
                                               mobile = q5.mobile,
                                               UserName = q5.UserName,
                                               stagetypeid = q5.stagetypeid,
                                               islate = q5.islate,//workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result

                                               // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                                           }
                       );

                    if (searchDto.isLate)
                    {
                        int count = searchLateQuery.Count();// if islate click  on card select this
                        applicationCountDto.count = count;
                        applicationCountDto.Applications = searchLateQuery.Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                    }
                    else
                    {
                        int count = searchQuery.Count();
                        applicationCountDto.count = count; // if late not clicked on card select this 
                        applicationCountDto.Applications = searchQuery.OrderBy(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();
                    }

                }
           
            else if (searchDto.stagetypeid == RejectedStageId.ToString())
            {


                var isemployee = (from appt in _EngineCoreDBContext.ApplicationTrack
                                  join ur in _EngineCoreDBContext.UserRole on appt.UserId equals ur.UserId
                                  where ur.RoleId == 17
                                  select new
                                  {
                                      appid = appt.ApplicationId,
                                      userid = appt.UserId,
                                      created_date = appt.CreatedDate
                                  });



                var GroupappTrak = (
                    from appt in isemployee
                    group appt by appt.appid into t




                    select new
                    {
                        appid = t.Key,
                        created_date = t.Max(x => x.created_date)
                    });
                var apptrak = (from appt in _EngineCoreDBContext.ApplicationTrack
                               join gt in GroupappTrak on new { colA = appt.ApplicationId, colB = appt.CreatedDate } equals new { colA = gt.appid, colB = gt.created_date }
                               join us in _EngineCoreDBContext.User on appt.UserId equals us.Id
                               select new
                               {
                                   username = us.FullName,
                                   appid = appt.ApplicationId,
                                   userid = appt.UserId
                               });
                var AppStageService = // get service name and stage for each application and translate Service shortcut
                   (from app in _EngineCoreDBContext.Application.Where(x => x.Id > 0 && x.StateId == RejectedStateId )
                    join srv in _EngineCoreDBContext.AdmService
                    on app.ServiceId equals srv.Id
                    join tr in _EngineCoreDBContext.SysTranslation
                    on srv.Shortcut equals tr.Shortcut

                    where tr.Lang == lang
                    select new AllapplicationDto
                    {
                      
                        percent =0,// (int)(stagetypetranslation.Where(x => x.Id == stg.StageTypeId).Select(y => y.percent).FirstOrDefault()),
                        lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                        appStatusId = app.StateId,
                        appid = app.Id,
                        serviceid = srv.Id,
                        servicename = tr.Value,
                        templateid = app.TemplateId,
                        appstartdate = app.CreatedDate,
                    }
                   );

                //we need to get All name of template for All applications 
                //if template id ==null then template name is null by left join in query
                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template
                                     on q1.templateid equals tm.Id
                                     join tr in _EngineCoreDBContext.SysTranslation
                                     on tm.TitleShortcut equals tr.Shortcut
                                     where tr.Lang==lang
                                     select new AllapplicationDto
                                     {
                                        
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         templateid = tm.Id,
                                         appstartdate = q1.appstartdate,
                                         templatename = tr.Value,
                                         doctypeid = tm.DocumentTypeId,
                                         UserName =
                                         apptrak.FirstOrDefault(x => x.appid == q1.appid).username
                                         //UserName = employees.Contains(apptrak.Where(y => y.appid == q1.appid).Select(z => z.userid).FirstOrDefault().Value) ?
                                         //_EngineCoreDBContext.User.Where(x => x.Id == apptrak.Where(y => y.appid == q1.appid).Select(z => z.userid).FirstOrDefault()).Select(p => p.FullName).FirstOrDefault() : null





                                     });

                //now i need to get document name fro every app
                // if documentid==null then document name=null by left join in query
                var documentNameQuery = (
                       from qtl in templateQuery
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id
                        join tr in _EngineCoreDBContext.SysTranslation
                        on lv.Shortcut equals tr.Shortcut
                       where tr.Lang==lang

                       select new AllapplicationDto
                       {
                           LastDateReader=qtl.LastDateReader,
                           lastUpdateStage = qtl.lastUpdateStage,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           documentname = tr.Value,
                           UserName = qtl.UserName
                       }
                       );
               

                //get applications status translation
                var Appstatustranslate = (
                    from q3 in documentNameQuery
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    join tr5 in _EngineCoreDBContext.SysTranslation
                    on lv.Shortcut equals tr5.Shortcut
                    where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                      
                        lastUpdateStage = q3.lastUpdateStage,
                        appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        UserName = q3.UserName


                    });
                //we need to know owner of All applications
                var isownerQuery = (
                    from tranc in _EngineCoreDBContext.AppTransaction
                    join par in _EngineCoreDBContext.ApplicationParty
                    on tranc.Id equals par.TransactionId
                    where par.IsOwner == true
                    select new
                    {
                        appid=tranc.ApplicationId,
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile
                    });
             

                //apply search for All Applications
                var searchQuery = (
                    from q4 in isownerQuery
                    join q5 in Appstatustranslate on q4.appid equals q5.appid

                   
                    select new Apps
                    {
                        UserName = q5.UserName,
                        percent = q5.percent,
                        lastUpdateStage = q5.lastUpdateStage,
                        appStatusName = q5.appStatusName,
                        appStatusId = q5.appStatusId,
                        appid = q4.appid,
                        serviceid = q5.serviceid,
                        servicename = q5.servicename,
                        appstartdate = q5.appstartdate,
                        templateid = q5.templateid,
                        templatename = q5.templatename,
                        doctypeid = q5.doctypeid,
                        documentname = q5.documentname,
                        fullname = q4.fullname,
                        email = q4.email,
                        mobile = q4.mobile,
                        stagetypeid = q5.stagetypeid,
                        islocked = q4.appid > 0 ? DateTime.Now.Subtract((DateTime)q5.LastDateReader).TotalSeconds < Constants.LOCK_SECONDS_TIME ? true : false : false


                        
                    }
                    ).ToList();



             
                    int count = searchQuery.Count();
                     applicationCountDto.AutoCancelledId = AutoCancelId;
                    applicationCountDto.count = count; // if late not clicked on card select this 
                    applicationCountDto.Applications = searchQuery.OrderBy(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

               
            }
            else if (searchDto.stagetypeid == MeetingStageId.ToString())
            {
                List<int> StagesHours = await _EngineCoreDBContext.AdmStage.Select(x => (int)x.PeriodForLate).Distinct().ToListAsync();
                Dictionary<int, DateTime> lateDate = await _IWorkingTimeRepository.GetDeadline(StagesHours.Distinct().ToList());
                //searchDto.eDate = DateTime.Now.ToShortDateString();
                //searchDto.sDate = DateTime.Now.AddDays(-100).ToShortDateString();

                var isemployee = (from appt in _EngineCoreDBContext.ApplicationTrack
                                  join ur in _EngineCoreDBContext.UserRole on appt.UserId equals ur.UserId
                                  where ur.RoleId == 17
                                  select new
                                  {
                                      appid = appt.ApplicationId,
                                      userid = appt.UserId,
                                      created_date = appt.CreatedDate
                                  });



                var GroupappTrak = (
                    from appt in isemployee
                    group appt by appt.appid into t



                    select new
                    {
                        appid = t.Key,
                        created_date = t.Max(x => x.created_date)
                    });
                var apptrak = (from appt in _EngineCoreDBContext.ApplicationTrack
                               join gt in GroupappTrak on new { colA = appt.ApplicationId, colB = appt.CreatedDate } equals new { colA = gt.appid, colB = gt.created_date }
                               join us in _EngineCoreDBContext.User on appt.UserId equals us.Id
                               select new
                               {
                                   username = us.FullName,
                                   appid = appt.ApplicationId,
                                   userid = appt.UserId
                               });


                var AppStageService = // get service name and stage for each application and translate Service shortcut
                   (from app in _EngineCoreDBContext.Application.Where(x => x.Id>0 && x.StateId != RejectedStateId && x.StateId!=AutoCancelId //&& 
                    ///(searchDto.sDate == null || x.AppLastUpdateDate >= Convert.ToDateTime(searchDto.sDate)) &&
                    //(searchDto.eDate == null || x.AppLastUpdateDate <= Convert.ToDateTime(searchDto.eDate).AddDays(1))
                    )
                    join srv in _EngineCoreDBContext.AdmService
                    on app.ServiceId equals srv.Id
                    join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId == MeetingStageId)
                    on app.CurrentStageId equals stg.Id
                    join tr in _EngineCoreDBContext.SysTranslation
                    on srv.Shortcut equals tr.Shortcut

                    where tr.Lang == lang
                    select new AllapplicationDto
                    {
                        LastDateReader=(DateTime)app.LastReadDate,
                        percent = 50,
                        lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                        periodLate = (int)stg.PeriodForLate,
                        appid = app.Id,
                        stagetypeid = stg.StageTypeId,
                        serviceid = srv.Id,
                        servicename = tr.Value,
                        templateid = app.TemplateId,
                        appstartdate = app.CreatedDate,
                        stagetypename = (stagetypetranslation.Where(x => x.Id == stg.StageTypeId).Select(y => y.Value).FirstOrDefault())
                    }
                   );

                //we need to get All name of template for All applications 
                //if template id ==null then template name is null by left join in query
                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id 
                                    join tr in _EngineCoreDBContext.SysTranslation
                                    on tm.TitleShortcut equals tr.Shortcut
                                    where tr.Lang==lang
                                     select new AllapplicationDto
                                     {
                                         LastDateReader=q1.LastDateReader,
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         templateid = q1.templateid,
                                         appstartdate = q1.appstartdate,
                                         templatename = tr.Value,
                                         doctypeid = tm.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid,
                                         stagetypename = q1.stagetypename,
                                         UserName =
                                         apptrak.FirstOrDefault(x => x.appid == q1.appid).username



                                     });

                //now i need to get document name fro every app
                // if documentid==null then document name=null by left join in query
                var documentNameQuery = (
                       from qtl in templateQuery
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id
                      join tr in _EngineCoreDBContext.SysTranslation
                      on lv.Shortcut equals tr.Shortcut
                       where tr.Lang==lang

                       select new AllapplicationDto
                       {
                           LastDateReader=qtl.LastDateReader,
                           percent = qtl.percent,
                           lastUpdateStage = qtl.lastUpdateStage,
                           periodLate = qtl.periodLate,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           fee = qtl.fee,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           documentname = tr.Value,
                           stagetypeid = qtl.stagetypeid,
                           stagetypename = qtl.stagetypename,
                           UserName = qtl.UserName,

                           isOnlineStatus = _IMeetingRepository.IsAttendedByAppNo((int)qtl.appid).Result.IsOnline && !_IMeetingRepository.IsAttendedByAppNo((int)qtl.appid).Result.IsLate ? 1
                                      : _IMeetingRepository.IsAttendedByAppNo((int)qtl.appid).Result.IsLate && _IMeetingRepository.IsAttendedByAppNo((int)qtl.appid).Result.IsLate ? 2 : 0

                       }
                       );
             

                //get applications status translation
                //var Appstatustranslate = (
                //    from q3 in documentNameQuery
                //    join lv in _EngineCoreDBContext.SysLookupValue
                //    on q3.appStatusId equals lv.Id
                //    join tr5 in _EngineCoreDBContext.SysTranslation
                //    on lv.Shortcut equals tr5.Shortcut
                //    where tr5.Lang == lang
                //    select new AllapplicationDto
                //    {
                //        LastDateReader=q3.LastDateReader,
                //        percent = q3.percent,
                //        lastUpdateStage = q3.lastUpdateStage,
                //        periodLate = q3.periodLate,
                //        appStatusName = tr5.Value,
                //        appStatusId = q3.appStatusId,
                //        appid = q3.appid,
                //        serviceid = q3.serviceid,
                //        servicename = q3.servicename,
                //        fee = q3.fee,
                //        templateid = q3.templateid,
                //        appstartdate = q3.appstartdate,
                //        templatename = q3.templatename,
                //        doctypeid = q3.doctypeid,
                //        documentname = q3.documentname,
                //        stagetypeid = q3.stagetypeid,
                //        stagetypename = q3.stagetypename,
                //        UserName = q3.UserName,
                //        isOnlineStatus= _IMeetingRepository.IsAttendedByAppNo((int)q3.appid).Result.IsOnline && !_IMeetingRepository.IsAttendedByAppNo((int)q3.appid).Result.IsLate?1
                //                      : _IMeetingRepository.IsAttendedByAppNo((int)q3.appid).Result.IsLate && _IMeetingRepository.IsAttendedByAppNo((int)q3.appid).Result.IsLate ? 2:0

                //    });
                //we need to know owner of All applications
                var isownerQuery = (
                    from tranc in _EngineCoreDBContext.AppTransaction.Where(x=>x.Id>0)
                    join par in _EngineCoreDBContext.ApplicationParty.Where(x=>x.TransactionId>0)
                    on tranc.Id equals par.TransactionId
                    where par.IsOwner == true
                    select new
                    {
                        appid=tranc.ApplicationId,
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile
                    });
             

                //apply search for All Applications
                var searchQuery = (
                    from q4 in isownerQuery
                    join q5 in documentNameQuery on q4.appid equals q5.appid

                  
                    select new Apps
                    {
                        UserName = q5.UserName,
                        percent = q5.percent,
                        lastUpdateStage = q5.lastUpdateStage,
                        periodLate = q5.periodLate,
                        appid = q4.appid,
                        serviceid = q5.serviceid,
                        servicename = q5.servicename,
                        appstartdate = q5.appstartdate,
                        templateid = q5.templateid,
                        templatename = q5.templatename,
                        doctypeid = q5.doctypeid,
                        documentname = q5.documentname,
                        fullname = q4.fullname,
                        email = q4.email,
                        mobile = q4.mobile,
                        stagetypeid = q5.stagetypeid,
                        stagetypename = q5.stagetypename,
                        islate=q5.lastUpdateStage>= lateDate[q5.periodLate]?false:true,
                        islocked  = q4.appid > 0 ? DateTime.Now.Subtract((DateTime)q5.LastDateReader).TotalSeconds < Constants.LOCK_SECONDS_TIME ? true : false : false,
                        isOnlineStatus = q5.isOnlineStatus

                    }
                    ).ToList();



                var searchLateQuery = (from q5 in searchQuery

                                        

                                       where (q5.islate)
                                       select new Apps
                                       {
                                           UserName = q5.UserName,
                                           percent = q5.percent,
                                           lastUpdateStage = q5.lastUpdateStage,
                                           periodLate = q5.periodLate,
                                           appStatusName = q5.appStatusName,
                                           appStatusId = q5.appStatusId,
                                           appid = q5.appid,
                                           serviceid = q5.serviceid,
                                           servicename = q5.servicename,
                                           fee = q5.fee,
                                           appstartdate = q5.appstartdate,
                                           templateid = q5.templateid,
                                           templatename = q5.templatename,
                                           doctypeid = q5.doctypeid,
                                           documentname = q5.documentname,
                                           fullname = q5.fullname,
                                           email = q5.email,
                                           mobile = q5.mobile,
                                           stagetypeid = q5.stagetypeid,
                                           stagetypename = q5.stagetypename,
                                           islate = q5.islate,//workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result
                                           islocked = q5.islocked,
                                           // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                                       }
                   );
                int CountMeeting = searchQuery.Where(x => x.isOnlineStatus == 1 || x.isOnlineStatus == 2).Count();
                if (searchDto.isLate)
                {
                    int count = searchLateQuery.Count();// if islate click  on card select this
                    applicationCountDto.count = count;
                    applicationCountDto.Applications = searchLateQuery.Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                }
                else if (CountMeeting>0)
                {
                    int count = searchQuery.Count();
                    applicationCountDto.count = count; // if late not clicked on card select this 
                    applicationCountDto.Applications = searchQuery.OrderByDescending(x => x.isOnlineStatus).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                }
                else
                {
                    int count = searchQuery.Count();
                    applicationCountDto.count = count; // if late not clicked on card select this 
                    applicationCountDto.Applications = searchQuery.OrderBy(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                }
            }
            else if(searchDto.stagetypeid==PayStageTypeId.ToString())


            {

                List<int> StagesHours = await _EngineCoreDBContext.AdmStage.Select(x => (int)x.PeriodForLate).Distinct().ToListAsync();
                Dictionary<int, DateTime> lateDate = await _IWorkingTimeRepository.GetDeadline(StagesHours.Distinct().ToList());

                var isemployee = (from appt in _EngineCoreDBContext.ApplicationTrack
                                  join ur in _EngineCoreDBContext.UserRole on appt.UserId equals ur.UserId
                                  where ur.RoleId == 17
                                  select new
                                  {
                                      appid = appt.ApplicationId,
                                      userid = appt.UserId,
                                      created_date = appt.CreatedDate
                                  });



                var GroupappTrak = (
                    from appt in isemployee
                    group appt by appt.appid into t



                    select new
                    {
                        appid = t.Key,
                        created_date = t.Max(x => x.created_date)
                    });
                var apptrak = (from appt in _EngineCoreDBContext.ApplicationTrack
                               join gt in GroupappTrak on new { colA = appt.ApplicationId, colB = appt.CreatedDate } equals new { colA = gt.appid, colB = gt.created_date }
                               join us in _EngineCoreDBContext.User on appt.UserId equals us.Id
                               select new
                               {
                                   username = us.FullName,
                                   appid = appt.ApplicationId,
                                   userid = appt.UserId
                               });


                var AppStageService = // get service name and stage for each application and translate Service shortcut
                       (from app in _EngineCoreDBContext.Application.Where(x => x.Id>0&& x.StateId != RejectedStateId && x.StateId!=AutoCancelId)
                        join srv in _EngineCoreDBContext.AdmService
                       on app.ServiceId equals srv.Id
                        join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId ==PayStageTypeId)
                        on app.CurrentStageId equals stg.Id
                        join tr in _EngineCoreDBContext.SysTranslation
                        on srv.Shortcut equals tr.Shortcut

                        where tr.Lang == lang
                        select new AllapplicationDto
                        {
                            LastDateReader=(DateTime)app.LastReadDate,
                            percent =75,
                            lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                            periodLate = (int)stg.PeriodForLate,
                            appid = app.Id,
                            stagetypeid = stg.StageTypeId,
                            serviceid = srv.Id,
                            servicename = tr.Value,
                            templateid = app.TemplateId,
                            appstartdate = app.CreatedDate,
                            stagetypename = (stagetypetranslation.Where(x => x.Id == stg.StageTypeId).Select(y => y.Value).FirstOrDefault())
                        }
                       );

                    //we need to get All name of template for All applications 
                    //if template id ==null then template name is null by left join in query
                    var templateQuery = (from q1 in AppStageService
                                         join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id
                                         join tr in _EngineCoreDBContext.SysTranslation
                                         on tm.TitleShortcut equals tr.Shortcut
                                         where tr.Lang==lang
                                         select new AllapplicationDto
                                         {
                                             LastDateReader=q1.LastDateReader,
                                             percent = q1.percent,
                                             lastUpdateStage = q1.lastUpdateStage,
                                             periodLate = q1.periodLate,
                                             appid = q1.appid,
                                             serviceid = q1.serviceid,
                                             servicename = q1.servicename,
                                             templateid = q1.templateid,
                                             appstartdate = q1.appstartdate,
                                             templatename = tr.Value,
                                             doctypeid = tm.DocumentTypeId,
                                             stagetypeid = q1.stagetypeid,
                                             stagetypename = q1.stagetypename,
                                             UserName =
                                         apptrak.FirstOrDefault(x => x.appid == q1.appid).username,
                                             //UserName =employees.Contains(apptrak.Where(y => y.appid == q1.appid).Select(z => z.userid).FirstOrDefault().Value) ?
                                             //_EngineCoreDBContext.User.Where(x => x.Id == apptrak.Where(y => y.appid == q1.appid).Select(z => z.userid).FirstOrDefault()).Select(p => p.FullName).FirstOrDefault() : null





                                         });

                    //now i need to get document name fro every app
                    // if documentid==null then document name=null by left join in query
                    var documentNameQuery = (
                           from qtl in templateQuery
                           join lv in _EngineCoreDBContext.SysLookupValue
                           on qtl.doctypeid equals lv.Id
                           join tr in _EngineCoreDBContext.SysTranslation
                           on lv.Shortcut equals tr.Shortcut
                           where tr.Lang==lang


                           select new AllapplicationDto
                           {
                               LastDateReader=qtl.LastDateReader,
                               percent = qtl.percent,
                               lastUpdateStage = qtl.lastUpdateStage,
                               periodLate = qtl.periodLate,
                               appid = qtl.appid,
                               serviceid = qtl.serviceid,
                               servicename = qtl.servicename,
                               templateid = qtl.templateid,
                               appstartdate = qtl.appstartdate,
                               templatename = qtl.templatename,
                               doctypeid = qtl.doctypeid,
                               documentname = tr.Value,
                               stagetypeid = qtl.stagetypeid,
                               stagetypename = qtl.stagetypename,
                               UserName = qtl.UserName
                           }
                           );
                   

                    //get applications status translation
                    //var Appstatustranslate = (
                    //    from q3 in documentNameQuery
                    //    join lv in _EngineCoreDBContext.SysLookupValue
                    //    on q3.appStatusId equals lv.Id
                    //    join tr5 in _EngineCoreDBContext.SysTranslation
                    //    on lv.Shortcut equals tr5.Shortcut
                    //    where tr5.Lang == lang
                    //    select new AllapplicationDto
                    //    {
                    //        percent = q3.percent,
                    //        lastUpdateStage = q3.lastUpdateStage,
                    //        periodLate = q3.periodLate,
                    //        appStatusName = tr5.Value,
                    //        appStatusId = q3.appStatusId,
                    //        appid = q3.appid,
                    //        serviceid = q3.serviceid,
                    //        servicename = q3.servicename,
                    //        fee = q3.fee,
                    //        templateid = q3.templateid,
                    //        appstartdate = q3.appstartdate,
                    //        templatename = q3.templatename,
                    //        doctypeid = q3.doctypeid,
                    //        documentname = q3.documentname,
                    //        stagetypeid = q3.stagetypeid,
                    //        stagetypename = q3.stagetypename,
                    //        UserName = q3.UserName


                    //    });
                    //we need to know owner of All applications
                    var isownerQuery = (
                    from tranc in _EngineCoreDBContext.AppTransaction
                        join par in _EngineCoreDBContext.ApplicationParty
                        on tranc.Id equals par.TransactionId
                        where par.IsOwner == true
                        select new
                        {
                            appid=tranc.ApplicationId,
                            trancId = par.TransactionId,
                            fullname = par.FullName,
                            email = par.Email,
                            mobile = par.Mobile
                        });
                   

                    //apply search for All Applications
                    var searchQuery = (
                        from q4 in isownerQuery
                        join q5 in documentNameQuery on q4.appid equals q5.appid

                        select new Apps
                        {
                            
                            UserName = q5.UserName,
                            percent = q5.percent,
                            lastUpdateStage = q5.lastUpdateStage,
                            periodLate = q5.periodLate,
                            appid = q4.appid,
                            serviceid = q5.serviceid,
                            servicename = q5.servicename,
                            appstartdate = q5.appstartdate,
                            templateid = q5.templateid,
                            templatename = q5.templatename,
                            doctypeid = q5.doctypeid,
                            documentname = q5.documentname,
                            fullname = q4.fullname,
                            email = q4.email,
                            mobile = q4.mobile,
                            stagetypeid = q5.stagetypeid,
                            stagetypename = q5.stagetypename,
                            islate = q5.lastUpdateStage >= lateDate[q5.periodLate] ? false : true,
                            islocked = q4.appid > 0 ? DateTime.Now.Subtract((DateTime)q5.LastDateReader).TotalSeconds < Constants.LOCK_SECONDS_TIME ? true : false : false,


                           
                        }
                        ).ToList();



                    var searchLateQuery = (from q5 in searchQuery

                                             

                                           where (q5.islate)
                                           select new Apps
                                           {
                                               UserName = q5.UserName,
                                               percent = q5.percent,
                                               lastUpdateStage = q5.lastUpdateStage,
                                               periodLate = q5.periodLate,
                                               appStatusName = q5.appStatusName,
                                               appStatusId = q5.appStatusId,
                                               appid = q5.appid,
                                               serviceid = q5.serviceid,
                                               servicename = q5.servicename,
                                               fee = q5.fee,
                                               appstartdate = q5.appstartdate,
                                               templateid = q5.templateid,
                                               templatename = q5.templatename,
                                               doctypeid = q5.doctypeid,
                                               documentname = q5.documentname,
                                               fullname = q5.fullname,
                                               email = q5.email,
                                               mobile = q5.mobile,
                                               stagetypeid = q5.stagetypeid,
                                               stagetypename = q5.stagetypename,
                                               islate = q5.islate,
                                               islocked = q5.islocked,
                                              
                                           }
                       );

                    if (searchDto.isLate)
                    {
                        int count = searchLateQuery.Count();// if islate click  on card select this
                    if (searchDto.stagetypeid == ReviewStageTypeId.ToString())
                        applicationCountDto.StageTypeId = ReviewStageTypeId;
                    applicationCountDto.count = count;
                        applicationCountDto.Applications = searchLateQuery.Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                    }
                    else
                    {
                        
                    int count = searchQuery.Count();
                    if (searchDto.stagetypeid == ReviewStageTypeId.ToString())
                        applicationCountDto.StageTypeId = ReviewStageTypeId;
                    applicationCountDto.count = count; // if late not clicked on card select this 
                        applicationCountDto.Applications = searchQuery.OrderBy(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                    }
                
            }
            else if (searchDto.stagetypeid == AutoCancelledStageId.ToString())
            {


                var AppStageService = // get service name and stage for each application and translate Service shortcut
                   (from app in _EngineCoreDBContext.Application.Where(x => x.Id > 0 && x.StateId == AutoCancelId)
                    join srv in _EngineCoreDBContext.AdmService
                    on app.ServiceId equals srv.Id
                    join tr in _EngineCoreDBContext.SysTranslation
                    on srv.Shortcut equals tr.Shortcut

                    where tr.Lang == lang
                    select new AllapplicationDto
                    {
                        LastDateReader=(DateTime)app.LastReadDate,
                        percent = 0,// (int)(stagetypetranslation.Where(x => x.Id == stg.StageTypeId).Select(y => y.percent).FirstOrDefault()),
                        lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                        appStatusId = app.StateId,
                        appid = app.Id,
                        serviceid = srv.Id,
                        servicename = tr.Value,
                        templateid = app.TemplateId,
                        appstartdate = app.CreatedDate,
                    }
                   );

                //we need to get All name of template for All applications 
                //if template id ==null then template name is null by left join in query
                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template
                                     on q1.templateid equals tm.Id
                                     join tr in _EngineCoreDBContext.SysTranslation
                                     on tm.TitleShortcut equals tr.Shortcut
                                     where tr.Lang == lang
                                     select new AllapplicationDto
                                     {
                                         LastDateReader=q1.LastDateReader,
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         templateid = tm.Id,
                                         appstartdate = q1.appstartdate,
                                         templatename = tr.Value,
                                         doctypeid = tm.DocumentTypeId,
                                         //UserName =
                                         //apptrak.FirstOrDefault(x => x.appid == q1.appid).username
                                         //UserName = employees.Contains(apptrak.Where(y => y.appid == q1.appid).Select(z => z.userid).FirstOrDefault().Value) ?
                                         //_EngineCoreDBContext.User.Where(x => x.Id == apptrak.Where(y => y.appid == q1.appid).Select(z => z.userid).FirstOrDefault()).Select(p => p.FullName).FirstOrDefault() : null





                                     });

                //now i need to get document name fro every app
                // if documentid==null then document name=null by left join in query
                var documentNameQuery = (
                       from qtl in templateQuery
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id
                       join tr in _EngineCoreDBContext.SysTranslation
                       on lv.Shortcut equals tr.Shortcut
                       where tr.Lang == lang

                       select new AllapplicationDto
                       {
                           LastDateReader = qtl.LastDateReader,
                           lastUpdateStage = qtl.lastUpdateStage,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           documentname = tr.Value,
                           UserName = qtl.UserName
                       }
                       );


                //get applications status translation
                var Appstatustranslate = (
                    from q3 in documentNameQuery
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    join tr5 in _EngineCoreDBContext.SysTranslation
                    on lv.Shortcut equals tr5.Shortcut
                    where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                        LastDateReader=q3.LastDateReader,
                        lastUpdateStage = q3.lastUpdateStage,
                        appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        UserName = q3.UserName


                    });
                //we need to know owner of All applications
                var isownerQuery = (
                    from tranc in _EngineCoreDBContext.AppTransaction.Where(x=>x.Id>0)
                    join par in _EngineCoreDBContext.ApplicationParty.Where(x=>x.TransactionId>0)
                    on tranc.Id equals par.TransactionId
                    where par.IsOwner == true
                    select new
                    {
                        appid = tranc.ApplicationId,
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile
                    });


                //apply search for All Applications
                var searchQuery = (
                    from q4 in isownerQuery
                    join q5 in Appstatustranslate on q4.appid equals q5.appid


                    select new Apps
                    {
                        UserName = q5.UserName,
                        percent = q5.percent,
                        lastUpdateStage = q5.lastUpdateStage,
                        appStatusName = q5.appStatusName,
                        appStatusId = q5.appStatusId,
                        appid = q4.appid,
                        serviceid = q5.serviceid,
                        servicename = q5.servicename,
                        appstartdate = q5.appstartdate,
                        templateid = q5.templateid,
                        templatename = q5.templatename,
                        doctypeid = q5.doctypeid,
                        documentname = q5.documentname,
                        fullname = q4.fullname,
                        email = q4.email,
                        mobile = q4.mobile,
                        stagetypeid = q5.stagetypeid,
                        islocked = q4.appid > 0 ? DateTime.Now.Subtract((DateTime)q5.LastDateReader).TotalSeconds < Constants.LOCK_SECONDS_TIME ? true : false : false



                    }
                    ).ToList();




                int count = searchQuery.Count();
                applicationCountDto.AutoCancelledId = AutoCancelId;
                applicationCountDto.count = count; // if late not clicked on card select this 
                applicationCountDto.Applications = searchQuery.OrderBy(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();


            }

            return applicationCountDto;
        }

        public async Task<ApplicationCountDto> SearchUserByStage(string lang, searchDto searchDto, int currentpage, int perpage)
        {
            ApplicationCountDto applicationCountDto = new ApplicationCountDto();
            if (_iUserRepository.IsAdmin() || _iUserRepository.IsEmployee() || _iUserRepository.IsInspector())
            {
                applicationCountDto = await SearchByStage(lang, searchDto, currentpage, perpage);

            }
            else
            {
                SysLookupType sysLookupType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "stage_type").FirstOrDefault();
                int lookupTypeId = sysLookupType.Id;



                var stagetypetranslation = // get All stage type translation in system lookup type table
                    (from sys_lookup_value in _EngineCoreDBContext.SysLookupValue
                     join sys_translation in _EngineCoreDBContext.SysTranslation on sys_lookup_value.Shortcut equals sys_translation.Shortcut
                     where
                       sys_lookup_value.LookupTypeId == lookupTypeId &&
                       sys_translation.Lang == lang
                     select new
                     {
                         sys_translation.Value,
                         sys_translation.Lang,
                         lvshortcut = sys_lookup_value.Shortcut,
                         sys_lookup_value.LookupTypeId,
                         sys_lookup_value.Id
                     });


                int userid = _iUserRepository.GetUserID();
                
                DateTime startDate = DateTime.Now;
                DateTime EDate = DateTime.Now.AddDays(1);

                try
                {
                    startDate = _EngineCoreDBContext.Application.Min(x => x.AppLastUpdateDate).Value;
                    EDate = _EngineCoreDBContext.Application.Max(x => x.AppLastUpdateDate).Value.AddMinutes(_EngineCoreDBContext.AdmStage.Max(x => x.PeriodForLate).Value * 60);
                }
                catch
                {
                    startDate = DateTime.Now;
                    EDate = DateTime.Now.AddDays(10);
                }

                WorkingTimeRepository workingTimeRepository = new WorkingTimeRepository(_EngineCoreDBContext, _IGeneralRepository, _iGlobalDayOffRepository);
                await workingTimeRepository.InitialaizeWorkingDic(startDate.Date, EDate.Date);

                // this query get stage of apps with translate stage name
                var appStageService = // get service name and stage for each app
                (from app in _EngineCoreDBContext.Application.Where(x => x.ServiceId.ToString().Contains(searchDto.serviceNo) || searchDto.serviceNo == null)
                 join srv in _EngineCoreDBContext.AdmService
                 on app.ServiceId equals srv.Id
                 join stg in _EngineCoreDBContext.AdmStage
                 on app.CurrentStageId equals stg.Id
                 join tr in _EngineCoreDBContext.SysTranslation
                 on srv.Shortcut equals tr.Shortcut

                 where tr.Lang == lang
                 select new AllapplicationDto
                 {
                     percent = 0, //(float)100 / (_EngineCoreDBContext.AdmStage.Where(x => x.ServiceId == app.ServiceId).Count() - 1) * (float)stg.OrderNo,
                     lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                     periodLate = (int)stg.PeriodForLate,
                     appStatusId = app.StateId,
                     appid = app.Id,
                     stagetypeid = stg.StageTypeId,
                     serviceid = srv.Id,
                     servicename = tr.Value,
                     fee = srv.Fee,
                     templateid = app.TemplateId,
                     appstartdate = app.CreatedDate,
                     stagetypename = stagetypetranslation.Where(x => x.Id == stg.StageTypeId).Select(y => y.Value).FirstOrDefault()
                 }
                );//.ToListAsync(); 


                var templateQuery = (from q1 in appStageService
                                     join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id 
                                     join tr in _EngineCoreDBContext.SysTranslation on tm.TitleShortcut equals tr.Shortcut
                                     where tr.Lang==lang
                                     //into g
                                     //from q in g.DefaultIfEmpty()
                                     select new AllapplicationDto
                                     {
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         fee = q1.fee,
                                         templateid = q1.templateid,
                                         appstartdate = q1.appstartdate,
                                         templatename = tr.Value,
                                         doctypeid = tm.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid,
                                         stagetypename = q1.stagetypename

                                     });

                var documentNamesQuery = (
                       from qtl in templateQuery
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id //into g
                      // from q in g.DefaultIfEmpty()
                        join tr in _EngineCoreDBContext.SysTranslation 
                        on lv.Shortcut equals tr.Shortcut
                       where tr.Lang==lang

                       select new AllapplicationDto
                       {
                           percent = qtl.percent,
                           lastUpdateStage = qtl.lastUpdateStage,
                           periodLate = qtl.periodLate,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           fee = qtl.fee,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           documentname = tr.Value,
                           stagetypeid = qtl.stagetypeid,
                           stagetypename = qtl.stagetypename
                       }
                       );

                


                var appstatustranslation = (
                    from q3 in documentNamesQuery
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    join tr5 in _EngineCoreDBContext.SysTranslation
                    on lv.Shortcut equals tr5.Shortcut
                    where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        stagetypeid = q3.stagetypeid,
                        stagetypename = q3.stagetypename

                    });
                // we need to know transactions of user
                var userTransactions = (
                    from par in _EngineCoreDBContext.ApplicationParty
                    where par.PartyId == userid
                    select new
                    {
                        trancId = (int)par.TransactionId,// transactions of user
                    });
                // int count = userTransactions.Count();

                // now i need to know who is owner App 
                var isowner = (
                    from par in _EngineCoreDBContext.ApplicationParty
                    where par.IsOwner == true
                    select new
                    {
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile

                    }
                    );

                //by left join i get apps for every user and i get owner for these Apps
                var userAppwithOwner = (from iq in userTransactions
                                        join q in isowner
                                        on iq.trancId equals q.trancId into g
                                        from q in g.DefaultIfEmpty()
                                        select new
                                        {
                                            trancid = iq.trancId,
                                            fullname = q.fullname,
                                            email = q.email,
                                            mobile = q.mobile
                                        });
                //get info of user Apps Owner
                var ownerInfo = (
                     from app in _EngineCoreDBContext.Application
                     join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                     join q in userAppwithOwner on tranc.Id equals q.trancid
                     select new AppPartyDto
                     {
                         appid = app.Id,
                         fullname = q.fullname,
                         email = q.email,
                         mobile = q.mobile
                     });

                var result = (
                  from q4 in ownerInfo
                  join q5 in appstatustranslation on q4.appid equals q5.appid

                  where (q4.appid.ToString().Contains(searchDto.Id) || searchDto.Id == null) &&
                      (q4.fullname.Contains(searchDto.Name) || searchDto.Name == null) &&
                      (q4.email.Contains(searchDto.Email) || searchDto.Email == null) &&
                      (q4.mobile.Contains(searchDto.Phone) || searchDto.Phone == null)// &&

                     // (q5.stagetypeid.ToString().Contains(searchDto.stagetypeid) || searchDto.stagetypeid == null)

                  select new Apps
                  {
                      percent = q5.percent,
                      lastUpdateStage = q5.lastUpdateStage,
                      periodLate = q5.periodLate,
                      appStatusName = q5.appStatusName,
                      appStatusId = q5.appStatusId,
                      appid = q4.appid,
                      serviceid = q5.serviceid,
                      servicename = q5.servicename,
                      fee = q5.fee,
                      appstartdate = q5.appstartdate,
                      templateid = q5.templateid,
                      templatename = q5.templatename,
                      doctypeid = q5.doctypeid,
                      documentname = q5.documentname,
                      fullname = q4.fullname,
                      email = q4.email,
                      mobile = q4.mobile,
                      stagetypeid = q5.stagetypeid,
                      stagetypename = q5.stagetypename,
                      islate = (q5.stagetypename.Contains(Constants.stageDoneAR) || q5.stagetypename.Contains(Constants.stageDoneEN)) ? false : workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result

                      // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :


                  }
                  ).ToList();






                var searchLateQuery = (from q5 in result

                                           //orderby q5.appstartdate

                                       where (q5.islate)
                                       select new Apps
                                       {
                                           percent = q5.percent,
                                           lastUpdateStage = q5.lastUpdateStage,
                                           periodLate = q5.periodLate,
                                           appStatusName = q5.appStatusName,
                                           appStatusId = q5.appStatusId,
                                           appid = q5.appid,
                                           serviceid = q5.serviceid,
                                           servicename = q5.servicename,
                                           fee = q5.fee,
                                           appstartdate = q5.appstartdate,
                                           templateid = q5.templateid,
                                           templatename = q5.templatename,
                                           doctypeid = q5.doctypeid,
                                           documentname = q5.documentname,
                                           fullname = q5.fullname,
                                           email = q5.email,
                                           mobile = q5.mobile,
                                           stagetypeid = q5.stagetypeid,
                                           islate = q5.islate //workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result

                                           // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                                       }
               );


                //applicationCountDto.count = count;
                //applicationCountDto.Applications = await result.OrderByDescending(x=>x.appstartdate).Skip(perpage * (currentpage - 1)).Take(perpage).ToListAsync();
                //   applicationCountDto.AppBySatge = finallResult.ToList();

                if (searchDto.isLate)
                {
                    int count = searchLateQuery.Count();
                    applicationCountDto.count = count;
                    applicationCountDto.Applications = searchLateQuery.Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                }
                else
                {
                    int count = result.Count();
                    applicationCountDto.count = count;
                    applicationCountDto.Applications = result.OrderBy(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();
                }
            }

            return applicationCountDto;
        }


        public async Task<ApplicationCountDto> SearchEverything(string lang, searchDto searchDto, int currentpage, int perpage)
        {

            if (searchDto.sDate == "" || searchDto.sDate == null)
                searchDto.sDate = _EngineCoreDBContext.Application.Min(x => x.AppLastUpdateDate).ToString();
            if (searchDto.eDate == "" || searchDto.sDate == null)
                searchDto.eDate = _EngineCoreDBContext.Application.Max(x => x.AppLastUpdateDate).ToString();


            ApplicationCountDto applicationCountDto = new ApplicationCountDto();

            List<int> employees = new List<int>(_iUserRepository.GetEmployees().Result.Keys);

            int RejectedStateId = await _ISysValueRepository.GetIdByShortcut("REJECTED");
            int ReturnedStateId = await _ISysValueRepository.GetIdByShortcut("RETURNED");
            int DoneStateId = await _ISysValueRepository.GetIdByShortcut("DONE");
            int ReviewStageTypeId = await _ISysValueRepository.GetIdByShortcut("REVIEW");
            int DoneStageTypeId = await _ISysValueRepository.GetIdByShortcut("SysLookupValue_Shortcut677");
            int RejectedStageTypeId = await _ISysValueRepository.GetIdByShortcut("REJECTEDSTAGE");
            SysLookupType sysLookupType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "stage_type").FirstOrDefault();
            int lookupTypeId = sysLookupType.Id;

            string draftshorcut = _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stageDraftAR) || x.Value.ToLower()
                 .Contains(Constants.stageDraftEN)).Select(y => y.Shortcut).FirstOrDefault();

            int draftId = _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == draftshorcut).Select(y => y.Id).FirstOrDefault();

            var stagetypetranslation = // get All stage type translation in system lookup type table
               (from lv in _EngineCoreDBContext.SysLookupValue
                join tr in _EngineCoreDBContext.SysTranslation on lv.Shortcut equals tr.Shortcut
                where
                 lv.LookupTypeId == lookupTypeId &&
                 tr.Lang == lang


                select new
                {
                    StageTypeId = lv.Id,
                    StageName = tr.Value,
                    percent = lv.Order
                });
            Dictionary<int, string> stages = new Dictionary<int, string>();
            foreach (var stg in stagetypetranslation)
            {
                stages.Add(stg.StageTypeId, stg.StageName);
            }
            Dictionary<int, string> services = new Dictionary<int, string>();
            List<ServiceNamesDto> serviceNames = _IAdmServiceRepository.GetserviceNAmes(lang).Result;
            foreach (var srv in serviceNames)
            {
                services.Add((int)srv.Id, srv.serviceName);
            }


            Dictionary<int, string> templates = new Dictionary<int, string>();
            var TemplatesTrans = (from temp in _EngineCoreDBContext.Template
                                  join tr in _EngineCoreDBContext.SysTranslation on temp.TitleShortcut equals tr.Shortcut
                                  where tr.Lang == lang
                                  select new
                                  {
                                      tempId = temp.Id,
                                      tr.Value
                                  });
            foreach (var temp in TemplatesTrans)
            {
                templates.Add(temp.tempId, temp.Value);
            }

            Dictionary<int, string> documents = new Dictionary<int, string>();
            var documentsTrans = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == 2018)
                                  join tr in _EngineCoreDBContext.SysTranslation on lv.Shortcut equals tr.Shortcut
                                  where tr.Lang == lang
                                  select new
                                  {
                                      lv.Id,
                                      tr.Value
                                  });

            foreach (var doc in documentsTrans)
            {
                documents.Add(doc.Id, doc.Value);
            }



            //DateTime startDate = DateTime.Now;
            //DateTime EDate = DateTime.Now.AddDays(1);

            //try
            //{
            //    startDate = _EngineCoreDBContext.Application.Min(x => x.AppLastUpdateDate).Value;
            //    EDate = _EngineCoreDBContext.Application.Max(x => x.AppLastUpdateDate).Value.AddMinutes(_EngineCoreDBContext.AdmStage.Max(x => x.PeriodForLate).Value * 60);
            //}
            //catch
            //{
            //    startDate = DateTime.Now;
            //    EDate = DateTime.Now.AddDays(10);
            //}

            //WorkingTimeRepository workingTimeRepository = new WorkingTimeRepository(_EngineCoreDBContext, _IGeneralRepository, _iGlobalDayOffRepository);
            //await workingTimeRepository.InitialaizeWorkingDic(startDate.Date, EDate.Date);

            //var isemployee = (from appt in _EngineCoreDBContext.ApplicationTrack
            //                  join ur in _EngineCoreDBContext.UserRole on appt.UserId equals ur.UserId
            //                  where ur.RoleId == 17
            //                  select new
            //                  {
            //                      appid = appt.ApplicationId,
            //                      userid = appt.UserId,
            //                      created_date = appt.CreatedDate
            //                  });



            //var GroupappTrak = (
            //    from appt in isemployee
            //    group appt by appt.appid into t



            //    select new
            //    {
            //        appid = t.Key,
            //        created_date = t.Max(x => x.created_date)
            //    });
            //var apptrak = (from appt in _EngineCoreDBContext.ApplicationTrack
            //               join gt in GroupappTrak on new { colA = appt.ApplicationId, colB = appt.CreatedDate } equals new { colA = gt.appid, colB = gt.created_date }
            //               join us in _EngineCoreDBContext.User on appt.UserId equals us.Id
            //               select new
            //               {
            //                   username = us.FullName,
            //                   appid = appt.ApplicationId,
            //                   userid = appt.UserId
            //               });

            int userid = _iUserRepository.GetUserID();
            if (searchDto.onlyMyApps)
            {
                applicationCountDto = await NotarySearchAll(lang, searchDto, currentpage, perpage);
            }


            else
            {




                var AppStageService = // get service name and stage for each application and translate Service shortcut
                     (from app in _EngineCoreDBContext.Application.Where(x => (x.ServiceId.ToString().Contains(searchDto.serviceNo) || searchDto.serviceNo == null) //&& x.StateId!=RejectedStateId
                          && (x.Id.ToString().Contains(searchDto.Id) || x.ApplicationNo.Contains(searchDto.Id) || searchDto.Id == null)
                         )
                      join srv in _EngineCoreDBContext.AdmService
                      on app.ServiceId equals srv.Id
                      join stg in _EngineCoreDBContext.AdmStage
                      on app.CurrentStageId equals stg.Id
                      select new AllapplicationDto
                      {
                          LastDateReader = (DateTime)app.LastReadDate,
                          applicationNo = app.ApplicationNo,
                           percent = (int)(stagetypetranslation.Where(x => x.StageTypeId == stg.StageTypeId).Select(y => y.percent).FirstOrDefault()),
                          lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                          periodLate = (int)stg.PeriodForLate,
                          appStatusId = app.StateId,
                          appid = app.Id,
                          stagetypeid = stg.StageTypeId,
                          serviceid = srv.Id,
                          fee = srv.Fee,
                          templateid = app.TemplateId,
                          appstartdate = app.CreatedDate,
                      });

                //we need to get All name of template for All applications 
                //if template id ==null then template name is null by left join in query
                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id 
                                     select new AllapplicationDto
                                     {
                                         LastDateReader = q1.LastDateReader,
                                         applicationNo = q1.applicationNo,
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         fee = q1.fee,
                                         templateid = q1.templateid,
                                         appstartdate = q1.appstartdate,
                                         doctypeid = tm.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid,
                                         stagetypename = q1.stagetypename

                                     });

                //now i need to get document name fro every app
                // if documentid==null then document name=null by left join in query
                var documentNameQuery = (
                       from qtl in templateQuery
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id

                       select new AllapplicationDto
                       {
                           LastDateReader = qtl.LastDateReader,
                           applicationNo = qtl.applicationNo,
                           percent = qtl.percent,
                           lastUpdateStage = qtl.lastUpdateStage,
                           periodLate = qtl.periodLate,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           fee = qtl.fee,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           stagetypeid = qtl.stagetypeid,
                           stagetypename = qtl.stagetypename,
                           //     UserName = qtl.appid > 0 ?//_IAdmServiceRepository.LastNotary((int)qtl.appid):null
                           // apptrak.FirstOrDefault(x => x.appid == qtl.appid).username : null
                           //_EngineCoreDBContext.User.Where(x => x.Id == apptrak.Where(y => y.appid == qtl.appid).Select(z => z.userid).FirstOrDefault()).Select(p => p.FullName).FirstOrDefault() : null : null

                       }
                      );


                //get applications status translation
                var Appstatustranslate = (
                    from q3 in documentNameQuery
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    select new AllapplicationDto
                    {
                        LastDateReader = q3.LastDateReader,
                        applicationNo = q3.applicationNo,
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        stagetypeid = q3.stagetypeid,
                        stagetypename = q3.stagetypename,
                        UserName = q3.UserName

                    });

                //get all parties with there emirates id and extra attachment if found or not
                var extraquery = (from par in _EngineCoreDBContext.ApplicationParty
                                  join ex in _EngineCoreDBContext.ApplicationPartyExtraAttachment on par.Id equals ex.ApplicationPartyId into g
                                  from ex in g.DefaultIfEmpty()
                                  select new
                                  {
                                      id = par.Id,
                                      trancid = par.TransactionId,
                                      partyfullname = par.FullName,
                                      partyemail = par.Email,
                                      partyphone = par.Mobile,
                                      partyemirateid = par.EmiratesIdNo,
                                      partydocid = ex.Number

                                  });

                //we need to know owner of All applications
                var isownerQuery = (
                    from par in _EngineCoreDBContext.ApplicationParty//.Where(x=>x.TransactionId.ToString().Contains(searchDto.TransactionID)|| searchDto.TransactionID==null)
                    where par.IsOwner == true
                    select new
                    {
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile,

                    });

                var allparty = (
                    from exq in extraquery
                    join iso in isownerQuery on exq.trancid equals iso.trancId into g

                    from iso in g.DefaultIfEmpty()
                    select new
                    {
                        trancid = exq.trancid,
                        partyfullname = exq.partyfullname,
                        partyemail = exq.partyemail,
                        partyphone = exq.partyphone,
                        partyemirateid = exq.partyemirateid,
                        partyexdocid = exq.partydocid,
                        ownername = iso.fullname,
                        owneremail = iso.email,
                        ownerphone = iso.mobile,

                    }
                    );

                // get transaction for owner applications
                var userTransactionsQuery = (
                     from q in allparty
                     join tranc in _EngineCoreDBContext.AppTransaction
                     on q.trancid equals tranc.Id
                     join oldtranc in _EngineCoreDBContext.TransactionOldVersion on tranc.Id equals oldtranc.TransactionId into g



                     from oldtranc in g.DefaultIfEmpty()

                     select new AppPartyDto
                     {
                         oldtransactionno = oldtranc.TransactionNo,
                         trancID = tranc.Id,
                         TransactionId = tranc.TransactionNo,
                         appid = tranc.ApplicationId,
                         fullname = q.ownername,
                         email = q.owneremail,
                         mobile = q.ownerphone,
                         extradocid = q.partyexdocid,
                         paremirateid = q.partyemirateid,
                         paremail = q.partyemail,
                         parfullname = q.partyfullname,
                         parmobile = q.partyphone
                     });//.ToListAsync();

                
                //apply search for All Applications
                var searchQuery = (
                    from q4 in userTransactionsQuery //olduserTransactionsQuery
                    join q5 in Appstatustranslate on q4.appid equals q5.appid

                    
                    where 
                              ((searchDto.sDate == null || (q5.lastUpdateStage != null && q5.lastUpdateStage >= Convert.ToDateTime(searchDto.sDate))) &&
                    (searchDto.eDate == null || (q5.lastUpdateStage != null && q5.lastUpdateStage <= Convert.ToDateTime(searchDto.eDate).AddDays(1)))) &&
                           (q4.parfullname.Contains(searchDto.Name) || searchDto.Name == null) &&
                           (q4.parmobile.Contains(searchDto.Phone) || searchDto.Phone == null) &&
                           (q4.paremail.Contains(searchDto.Email) || searchDto.Email == null) &&
                           (q4.paremirateid.Contains(searchDto.EmirateId) || q4.extradocid.Contains(searchDto.EmirateId) || searchDto.EmirateId == null) &&
                            (q4.TransactionId.Contains(searchDto.TransactionID) || q4.oldtransactionno.Contains(searchDto.TransactionID) || searchDto.TransactionID == null)
                    select new Apps
                    {

                        percent = q5.percent,
                        lastUpdateStage = q5.lastUpdateStage,
                        periodLate = q5.periodLate,
                        appStatusName = q5.appStatusName,
                        appStatusId = q5.appStatusId,
                        appid = q4.appid,
                        serviceid = q5.serviceid,
                        servicename = services[(int)q5.serviceid],
                        fee = q5.fee,
                        appstartdate = q5.appstartdate,
                        templateid = q5.templateid,
                        templatename = templates[(int)q5.templateid],
                        doctypeid = q5.doctypeid,
                        documentname = documents[(int)q5.doctypeid],
                        fullname = q4.fullname,
                        email = q4.email,
                        mobile = q4.mobile,
                        stagetypeid = q5.stagetypeid,
                        stagetypename = stages[(int)q5.stagetypeid],
                        UserName = q5.UserName,
                        // islate = ((q5.stagetypeid == DoneStageTypeId) || (q5.appStatusId == RejectedStateId) || (q5.stagetypeid == draftId)) ? false : workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result,
                        islocked = q4.appid > 0 ? DateTime.Now.Subtract((DateTime)q5.LastDateReader).TotalSeconds < Constants.LOCK_SECONDS_TIME ? true : false : false
                    }
                    ).ToList();


                applicationCountDto.Applications = searchQuery.GroupBy(z => z.appid).Select(p => p.FirstOrDefault()).OrderByDescending(x => x.appstartdate).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();
                int count = searchQuery.GroupBy(z => z.appid).Select(p => p.FirstOrDefault()).Count();
                applicationCountDto.count = count;
                // }


            }
            return applicationCountDto;
        }

        public async Task<ApplicationCountDto> NotaryCountApps(string lang)
        {
            var isemployee = (from appt in _EngineCoreDBContext.ApplicationTrack
                              join ur in _EngineCoreDBContext.UserRole on appt.UserId equals ur.UserId
                              where ur.RoleId == 17
                              select new
                              {
                                  appid = appt.ApplicationId,
                                  userid = appt.UserId,
                                  created_date = appt.CreatedDate
                              });



            var GroupappTrak = (
                from appt in isemployee
                group appt by appt.appid into t



                select new
                {
                    appid = t.Key,
                    created_date = t.Max(x => x.created_date)
                });
            var apptrak = (from appt in _EngineCoreDBContext.ApplicationTrack
                           join gt in GroupappTrak on new { colA = appt.ApplicationId, colB = appt.CreatedDate } equals new { colA = gt.appid, colB = gt.created_date }
                           select new
                           {
                               appid = appt.ApplicationId,
                               userid = appt.UserId
                           }

                );
            int userid = _iUserRepository.GetUserID();
            string drtafShortcut = _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stageDraftAR)).Select(y => y.Shortcut).FirstOrDefault();
            int draftId = _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == drtafShortcut).Select(y => y.Id).FirstOrDefault();
            int ReturnedStateId = await _ISysValueRepository.GetIdByShortcut("RETURNED");
            int RejectedStateId = await _ISysValueRepository.GetIdByShortcut("REJECTED");
            int ReviewStageTypeId = await _ISysValueRepository.GetIdByShortcut("REVIEW");
            int RejectedStageTypeId = await _ISysValueRepository.GetIdByShortcut("REJECTEDSTAGE");
            int AutoCancelId = await _ISysValueRepository.GetIdByShortcut("AutoCancel");
            int AutoCancelStageId = await _ISysValueRepository.GetIdByShortcut("SysCancelled");

            //DateTime startDate = _EngineCoreDBContext.Application.Min(x => x.AppLastUpdateDate).Value;
            //DateTime EDate = _EngineCoreDBContext.Application.Max(x => x.AppLastUpdateDate).Value.AddMinutes(_EngineCoreDBContext.AdmStage.Max(x => x.PeriodForLate).Value * 60);
            List<int> StagesHours = await _EngineCoreDBContext.AdmStage.Select(x => (int)x.PeriodForLate).Distinct().ToListAsync();
            Dictionary<int, DateTime> lateDate = await _IWorkingTimeRepository.GetDeadline(StagesHours.Distinct().ToList());
            //WorkingTimeRepository workingTimeRepository = new WorkingTimeRepository(_EngineCoreDBContext, _IGeneralRepository, _iGlobalDayOffRepository);
            //await workingTimeRepository.InitialaizeWorkingDic(startDate.Date, EDate.Date);
            //Task<List<AllapplicationDto>> query = null;  //AllapplicationDto
            ApplicationCountDto applicationCountDto = new ApplicationCountDto();

            var rejectedApps = // get service name and stage for each application and translate Service shortcut
                (from app in _EngineCoreDBContext.Application.Where(x => x.StateId == RejectedStateId)             //join srv in _EngineCoreDBContext.AdmService
                 join pt in apptrak.Where(x=>x.userid==userid) on app.Id equals pt.appid                                                                                              //on app.ServiceId equals srv.Id
                 join stg in _EngineCoreDBContext.AdmStage
                 on app.CurrentStageId equals stg.Id
                 //join apptrac in _EngineCoreDBContext.AppTransaction on app.Id equals apptrac.ApplicationId

                 select new AllapplicationDto
                 {
                   
                     lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                     periodLate = (int)stg.PeriodForLate,
                     appStatusId = app.StateId,
                     appid = app.Id,
                     stagetypeid = stg.StageTypeId,
                     appstartdate = app.CreatedDate,
                     
                 });
            var review = // get service name and stage for each application and translate Service shortcut
                (from app in _EngineCoreDBContext.Application.Where(x => x.StateId != RejectedStateId)             //join srv in _EngineCoreDBContext.AdmService
                                                                                                                //on app.ServiceId equals srv.Id
                 join stg in _EngineCoreDBContext.AdmStage.Where(x=>x.StageTypeId== ReviewStageTypeId)
                 on app.CurrentStageId equals stg.Id
                // join apptrac in _EngineCoreDBContext.AppTransaction on app.Id equals apptrac.ApplicationId

                 select new AllapplicationDto
                 {
                     lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                     periodLate = (int)stg.PeriodForLate,
                     appStatusId = app.StateId,
                     appid = app.Id,
                     stagetypeid = stg.StageTypeId,
                     appstartdate = app.CreatedDate
                 });

            var ReviewfinalResult = (

                   from q in review
                   where q.stagetypeid == ReviewStageTypeId
                   select new Apps
                   {
                       //lastUpdateStage = q.lastUpdateStage,
                       //periodLate = q.periodLate,
                       stagetypeid = q.stagetypeid,
                       islate = q.lastUpdateStage < lateDate[q.periodLate],

                   }).ToList();


            var AppStageService = // get service name and stage for each application and translate Service shortcut
                (from app in _EngineCoreDBContext.Application.Where(x => x.StateId != RejectedStateId && x.Id>0 && x.StateId!=AutoCancelId)
                 join apptrac in _EngineCoreDBContext.AppTransaction on app.Id equals apptrac.ApplicationId//join srv in _EngineCoreDBContext.AdmService
                 join pt in apptrak on app.Id equals pt.appid                                                                                            //on app.ServiceId equals srv.Id
                 join stg in _EngineCoreDBContext.AdmStage.Where(x=>x.StageTypeId!=draftId && x.StageTypeId!=AutoCancelStageId)
                 on app.CurrentStageId equals stg.Id
                 where pt.userid == userid
                 select new AllapplicationDto
                 {
                     //  percent = (float)100 / (_EngineCoreDBContext.AdmStage.Where(x => x.ServiceId == app.ServiceId).Count() - 1) * (float)stg.OrderNo,
                     lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                     periodLate = (int)stg.PeriodForLate,
                     appStatusId = app.StateId,
                     appid = app.Id,
                     stagetypeid = stg.StageTypeId,
                     // serviceid = srv.Id,
                     //servicename = tr.Value,
                     //fee = srv.Fee,
                     //templateid = app.TemplateId,
                     appstartdate = app.CreatedDate
                 });

            SysLookupType sysLookupType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "stage_type").FirstOrDefault();
            int lookupTypeId = sysLookupType.Id;


            var countAppinStage = // number of Application in each Stage
                        (from app in AppStageService

                         group app by app.stagetypeid into t
                         select new ApplicationByStageType
                         {
                             Count = t.Count(),
                             StageTypeId = (int)t.Key,


                         });

            var stageTypeTranslation = // get All stage type translation 
                (from sys_lookup_value in _EngineCoreDBContext.SysLookupValue.Where(x => x.Id!=draftId && x.Id!=AutoCancelStageId)
                 join sys_translation in _EngineCoreDBContext.SysTranslation on sys_lookup_value.Shortcut equals sys_translation.Shortcut
                 where
                   sys_lookup_value.LookupTypeId == lookupTypeId &&
                   sys_translation.Lang == lang

                 select new
                 {
                     sys_translation.Value,
                     sys_translation.Lang,
                     lvshortcut = sys_lookup_value.Shortcut,
                     sys_lookup_value.LookupTypeId,
                     sys_lookup_value.Id
                 });

            var currentAppstagetype =// translate current stage of applications
                (
                from lv in _EngineCoreDBContext.SysLookupValue
                join t in _EngineCoreDBContext.SysTranslation
                       on lv.Shortcut equals t.Shortcut
                join q in countAppinStage
                        on lv.Id equals q.StageTypeId


                where t.Lang == lang
                select new ApplicationByStageType
                {
                    Count = q.Count,
                    StageTypeId = q.StageTypeId,
                    StageTypeName = t.Value


                });//.ToListAsync();

            var groupStageType = // left join to get All stage and number of applications in each stage  
                  (from q2 in stageTypeTranslation
                   join q1 in currentAppstagetype on q2.Id equals q1.StageTypeId into g
                   from q1 in g.DefaultIfEmpty()
                   select new ApplicationByStageType
                   {
                       StageTypeName = q2.Value,
                       StageTypeId = q2.Id,
                       Count = q1.Count,

                   });
            var finalResult = (
                from q10 in groupStageType
                orderby q10.StageTypeId
                select new ApplicationByStage
                {
                    StageTypeId = q10.StageTypeId,
                    StageTypeName = q10.StageTypeName,
                    Count = q10.Count,
                    Appstage = (
                    from q in AppStageService
                    where q.stagetypeid == q10.StageTypeId
                    select new Apps
                    {
                        //lastUpdateStage = q.lastUpdateStage,
                        //periodLate = q.periodLate,
                        //appStatusName = q.appStatusName,
                        appStatusId = q.appStatusId,
                        //appid = q.appid,
                        //serviceid = q.serviceid,
                        //servicename = q.servicename,
                        //fee = q.fee,
                        //appstartdate = q.appstartdate,
                        //templateid = q.templateid,
                        //templatename = q.templatename,
                        //doctypeid = q.doctypeid,
                        //documentname = q.documentname,
                        //fullname = q.fullname,
                        //email = q.email,
                        //mobile = q.mobile,
                        stagetypeid = q.stagetypeid,
                        islate = (q10.StageTypeName.Contains(Constants.stageDoneAR) || q10.StageTypeName.Contains(Constants.stageDoneEN)) ? false :  q.lastUpdateStage >= lateDate[q.periodLate] ,
                    }).ToList()

                }
                   );
            applicationCountDto.RejectedStageTypeId = RejectedStageTypeId;
            applicationCountDto.rejectedCount = rejectedApps.Count();
            applicationCountDto.islateReview = ReviewfinalResult.Where(x => x.islate == true).Count();
            applicationCountDto.reviewCount = review.Where(x => x.stagetypeid == 5078).Count();
            applicationCountDto.returnedCount = AppStageService.Where(x => x.stagetypeid == draftId && x.appStatusId == ReturnedStateId).Count();
            applicationCountDto.AppBySatge = await finalResult.ToListAsync();
            return applicationCountDto;
        }

        public async Task<ApplicationCountDto> NotarySearchByStage(string lang, searchDto searchDto, int currentpage, int perpage)
        {
            int AutoCancelId = await _ISysValueRepository.GetIdByShortcut("AutoCancel");
            int MeetingStageId = await getShortCutStageByTranslate("en", Constants.stageMeetingEN);
            int RejectedStateId = await _ISysValueRepository.GetIdByShortcut("REJECTED");
            int ReturnedStateId = await _ISysValueRepository.GetIdByShortcut("RETURNED");
            int DoneStateId = await _ISysValueRepository.GetIdByShortcut("DONE");
            int ReviewStageTypeId = await _ISysValueRepository.GetIdByShortcut("REVIEW");
            string draftshorcut = _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stageDraftAR) || x.Value.ToLower()
                                                          .Contains(Constants.stageDraftEN)).Select(y => y.Shortcut).FirstOrDefault();
            int draftId = _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == draftshorcut).Select(y => y.Id).FirstOrDefault();
            List<int> employees = new List<int>(_iUserRepository.GetEmployees().Result.Keys);
            int DoneStageTypeId = await _ISysValueRepository.GetIdByShortcut("SysLookupValue_Shortcut677");
            int RejectedStageId = await _ISysValueRepository.GetIdByShortcut("REJECTEDSTAGE");


            List<int> StagesHours = await _EngineCoreDBContext.AdmStage.Select(x => (int)x.PeriodForLate).Distinct().ToListAsync();
            Dictionary<int, DateTime> lateDate = await _IWorkingTimeRepository.GetDeadline(StagesHours.Distinct().ToList());

            ApplicationCountDto applicationCountDto = new ApplicationCountDto();


            SysLookupType sysLookupType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "stage_type").FirstOrDefault();
            int lookupTypeId = sysLookupType.Id;



            var stagetypetranslation = // get All stage type translation in system lookup type table
                (from lv in _EngineCoreDBContext.SysLookupValue
                 join tr in _EngineCoreDBContext.SysTranslation on lv.Shortcut equals tr.Shortcut
                 where
                  lv.LookupTypeId == lookupTypeId &&
                  tr.Lang == lang


                 select new
                 {
                     StageTypeId = lv.Id,
                     StageName = tr.Value,
                     percent = lv.Order
                 });
            Dictionary<int, string> stages = new Dictionary<int, string>();
            foreach (var stg in stagetypetranslation)
            {
                stages.Add(stg.StageTypeId, stg.StageName);
            }
            Dictionary<int, string> services = new Dictionary<int, string>();
            List<ServiceNamesDto> serviceNames = _IAdmServiceRepository.GetserviceNAmes(lang).Result;
            foreach (var srv in serviceNames)
            {
                services.Add((int)srv.Id, srv.serviceName);
            }


            Dictionary<int, string> templates = new Dictionary<int, string>();
            var TemplatesTrans = (from temp in _EngineCoreDBContext.Template
                                  join tr in _EngineCoreDBContext.SysTranslation on temp.TitleShortcut equals tr.Shortcut
                                  where tr.Lang == lang
                                  select new
                                  {
                                      tempId = temp.Id,
                                      tr.Value
                                  });
            foreach (var temp in TemplatesTrans)
            {
                templates.Add(temp.tempId, temp.Value);
            }

            Dictionary<int, string> documents = new Dictionary<int, string>();
            var documentsTrans = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == 2018)
                                  join tr in _EngineCoreDBContext.SysTranslation on lv.Shortcut equals tr.Shortcut
                                  where tr.Lang == lang
                                  select new
                                  {
                                      lv.Id,
                                      tr.Value
                                  });

            foreach (var doc in documentsTrans)
            {
                documents.Add(doc.Id, doc.Value);
            }


            DateTime startDate = DateTime.Now;
            DateTime EDate = DateTime.Now.AddDays(1);

            try
            {
                startDate = _EngineCoreDBContext.Application.Min(x => x.AppLastUpdateDate).Value;
                EDate = _EngineCoreDBContext.Application.Max(x => x.AppLastUpdateDate).Value.AddMinutes(_EngineCoreDBContext.AdmStage.Max(x => x.PeriodForLate).Value * 60);
            }
            catch
            {
                startDate = DateTime.Now;
                EDate = DateTime.Now.AddDays(10);
            }

            WorkingTimeRepository workingTimeRepository = new WorkingTimeRepository(_EngineCoreDBContext, _IGeneralRepository, _iGlobalDayOffRepository);
            await workingTimeRepository.InitialaizeWorkingDic(startDate.Date, EDate.Date);
            //ApplicationCountDto applicationCountDto = new ApplicationCountDto();

            var isemployee = (from appt in _EngineCoreDBContext.ApplicationTrack
                              join ur in _EngineCoreDBContext.UserRole on appt.UserId equals ur.UserId
                              where ur.RoleId == 17
                              select new
                              {
                                  appid = appt.ApplicationId,
                                  userid = appt.UserId,
                                  created_date = appt.CreatedDate
                              });



            var GroupappTrak = (
                from appt in isemployee
                group appt by appt.appid into t



                select new
                {
                    appid = t.Key,
                    created_date = t.Max(x => x.created_date)
                });
            var apptrak = (from appt in _EngineCoreDBContext.ApplicationTrack
                           join gt in GroupappTrak on new { colA = appt.ApplicationId, colB = appt.CreatedDate } equals new { colA = gt.appid, colB = gt.created_date }
                           join us in _EngineCoreDBContext.User on appt.UserId equals us.Id
                           select new
                           {
                               username = us.FullName,
                               appid = appt.ApplicationId,
                               userid = appt.UserId
                           });

            int userid = _iUserRepository.GetUserID();
            if (searchDto.stagetypeid == RejectedStageId.ToString())
            {
                var AppStageService = // get service name and stage for each application and translate Service shortcut
                   (from app in _EngineCoreDBContext.Application.Where(x => x.StateId == RejectedStateId && x.Id>0)
                    join pt in apptrak.Where(x => x.userid == userid)
                    on app.Id equals pt.appid
                    join srv in _EngineCoreDBContext.AdmService
                    on app.ServiceId equals srv.Id
                    join stg in _EngineCoreDBContext.AdmStage//Where(x => x.StageTypeId != draftId)
                    on app.CurrentStageId equals stg.Id
                    join tr in _EngineCoreDBContext.SysTranslation
                    on srv.Shortcut equals tr.Shortcut

                    where tr.Lang == lang
                    select new AllapplicationDto
                    {
                        percent = (int)(stagetypetranslation.Where(x => x.StageTypeId == stg.StageTypeId).Select(y => y.percent).FirstOrDefault()),
                        lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                        periodLate = (int)stg.PeriodForLate,
                        appStatusId = app.StateId,
                        appid = app.Id,
                        stagetypeid = stg.StageTypeId,
                        serviceid = srv.Id,
                        servicename = tr.Value,
                        fee = srv.Fee,
                        templateid = app.TemplateId,
                        appstartdate = app.CreatedDate,
                        stagetypename = (stagetypetranslation.Where(x => x.StageTypeId == stg.StageTypeId).Select(y => y.StageName).FirstOrDefault())
                    }
                   );//.ToListAsync(); 

                //we need to get All name of template for All applications 
                //if template id ==null then template name is null by left join in query
                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id into g
                                     from q in g.DefaultIfEmpty()
                                     select new AllapplicationDto
                                     {
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         fee = q1.fee,
                                         templateid = q.Id,
                                         appstartdate = q1.appstartdate,
                                         templatename = q.TitleShortcut,
                                         doctypeid = q.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid,
                                         stagetypename = q1.stagetypename

                                     });//.ToListAsync();
                                        //then we get translations of template name 
                                        //if template name ==null then template name translation is null by left join in query
                var querytemplatetrans = (from q1 in templateQuery
                                          join tm in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang) on q1.templatename equals tm.Shortcut into g
                                          from q in g.DefaultIfEmpty()
                                          select new AllapplicationDto
                                          {
                                              percent = q1.percent,
                                              lastUpdateStage = q1.lastUpdateStage,
                                              periodLate = q1.periodLate,
                                              appStatusId = q1.appStatusId,
                                              appid = q1.appid,
                                              serviceid = q1.serviceid,
                                              servicename = q1.servicename,
                                              fee = q1.fee,
                                              templateid = q.Id,
                                              appstartdate = q1.appstartdate,
                                              templatename = q.Value,
                                              doctypeid = q1.doctypeid,
                                              stagetypeid = q1.stagetypeid,
                                              stagetypename = q1.stagetypename,
                                              //UserName = employees.Contains(apptrak.Where(y => y.appid == q1.appid).Select(z => z.userid).FirstOrDefault().Value) ?
                                              //_EngineCoreDBContext.User.Where(x => x.Id == apptrak.Where(y => y.appid == q1.appid).Select(z => z.userid).FirstOrDefault()).Select(p => p.FullName).FirstOrDefault() : null


                                          });//.ToListAsync();

                //now i need to get document name fro every app
                // if documentid==null then document name=null by left join in query
                var documentNameQuery = (
                       from qtl in querytemplatetrans
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id into g
                       from q in g.DefaultIfEmpty()


                       select new AllapplicationDto
                       {
                           percent = qtl.percent,
                           lastUpdateStage = qtl.lastUpdateStage,
                           periodLate = qtl.periodLate,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           fee = qtl.fee,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           documentname = q.Shortcut,
                           stagetypeid = qtl.stagetypeid,
                           stagetypename = qtl.stagetypename
                       }
                       );
                //get translations of document name 
                //if  document name=null then document name translation is null by left join in query
                var querydoctrans = (
                    from q3 in documentNameQuery
                    join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang)
                    on q3.documentname equals tr.Shortcut into g
                    from q in g.DefaultIfEmpty()
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q.Value,
                        stagetypeid = q3.stagetypeid,
                        stagetypename = q3.stagetypename
                    }

                    );

                //get applications status translation
                var Appstatustranslate = (
                    from q3 in querydoctrans
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    join tr5 in _EngineCoreDBContext.SysTranslation
                    on lv.Shortcut equals tr5.Shortcut
                    where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        stagetypeid = q3.stagetypeid,
                        stagetypename = q3.stagetypename

                    });
                //we need to know owner of All applications
                var isownerQuery = (
                    from par in _EngineCoreDBContext.ApplicationParty
                    where par.IsOwner == true
                    select new
                    {
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile
                    });
                // get transaction for owner applications
                var userTransactionsQuery = (
                     from app in apptrak.Where(x => x.userid == userid)//_EngineCoreDBContext.Application.Where(x => (x.ServiceId.ToString().Contains(searchDto.serviceNo) || searchDto.serviceNo == null) && x.StateId != RejectedStateId)
                     join tranc in _EngineCoreDBContext.AppTransaction on app.appid equals tranc.ApplicationId
                     join q in isownerQuery on tranc.Id equals q.trancId into g

                     from q in g.DefaultIfEmpty()


                     select new AppPartyDto
                     {
                         appid = app.appid,
                         fullname = q.fullname,
                         email = q.email,
                         mobile = q.mobile
                     });//.ToListAsync();

                //apply search for All Applications


                var searchQuery = (
                    from q4 in userTransactionsQuery
                    join q5 in Appstatustranslate on q4.appid equals q5.appid

                   
                    select new Apps
                    {
                        percent = q5.percent,
                        lastUpdateStage = q5.lastUpdateStage,
                        periodLate = q5.periodLate,
                        appStatusName = q5.appStatusName,
                        appStatusId = q5.appStatusId,
                        appid = q4.appid,
                        serviceid = q5.serviceid,
                        servicename = q5.servicename,
                        fee = q5.fee,
                        appstartdate = q5.appstartdate,
                        templateid = q5.templateid,
                        templatename = q5.templatename,
                        doctypeid = q5.doctypeid,
                        documentname = q5.documentname,
                        fullname = q4.fullname,
                        email = q4.email,
                        mobile = q4.mobile,
                        stagetypeid = q5.stagetypeid,
                        stagetypename = q5.stagetypename,
                        islate = (q5.stagetypename.Contains(Constants.stageDoneEN) || q5.stagetypename.Contains(Constants.stageDoneAR) || (searchDto.AppStateId != 0) || (q5.stagetypename.Contains(Constants.stageDraftAR)) || (q5.stagetypename.Contains(Constants.stageDraftEN))) ? false : workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result,
                         islocked = _iApplicationRepository.IfBookedUp((int)q4.appid, userid, lang).Result.Id == -1


                        // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                    }
                    ).ToList();



                var searchLateQuery = (from q5 in searchQuery

                                           //orderby q5.appstartdate

                                       where (q5.islate)
                                       select new Apps
                                       {
                                           percent = q5.percent,
                                           lastUpdateStage = q5.lastUpdateStage,
                                           periodLate = q5.periodLate,
                                           appStatusName = q5.appStatusName,
                                           appStatusId = q5.appStatusId,
                                           appid = q5.appid,
                                           serviceid = q5.serviceid,
                                           servicename = q5.servicename,
                                           fee = q5.fee,
                                           appstartdate = q5.appstartdate,
                                           templateid = q5.templateid,
                                           templatename = q5.templatename,
                                           doctypeid = q5.doctypeid,
                                           documentname = q5.documentname,
                                           fullname = q5.fullname,
                                           email = q5.email,
                                           mobile = q5.mobile,
                                           stagetypeid = q5.stagetypeid,
                                           stagetypename = q5.stagetypename,
                                           islate = q5.islate,//workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result
                                           islocked=q5.islocked
                                           // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                                       }
                   );

                if (searchDto.isLate)
                {
                    int count = searchLateQuery.Count();// if islate click  on card select this
                    applicationCountDto.count = count;
                    applicationCountDto.Applications = searchLateQuery.Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                }
                else
                {
                    int count = searchQuery.Count();
                    applicationCountDto.count = count; // if late not clicked on card select this 
                    applicationCountDto.Applications = searchQuery.OrderBy(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                }
            }
           else if (searchDto.stagetypeid == draftId.ToString())
            {
                var AppStageService = // get service name and stage for each application and translate Service shortcut
                   (from app in _EngineCoreDBContext.Application.Where(x =>x.StateId ==ReturnedStateId)
                    join pt in apptrak.Where(x => x.userid == userid)
                    on app.Id equals pt.appid
                    join srv in _EngineCoreDBContext.AdmService
                    on app.ServiceId equals srv.Id
                    join stg in _EngineCoreDBContext.AdmStage//Where(x => x.StageTypeId != draftId)
                    on app.CurrentStageId equals stg.Id
                    join tr in _EngineCoreDBContext.SysTranslation
                    on srv.Shortcut equals tr.Shortcut

                    where tr.Lang == lang
                    select new AllapplicationDto
                    {
                        percent = 0,
                    lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                        periodLate = (int)stg.PeriodForLate,
                        appStatusId = app.StateId,
                        appid = app.Id,
                        stagetypeid = stg.StageTypeId,
                        serviceid = srv.Id,
                        servicename = tr.Value,
                        fee = srv.Fee,
                        templateid = app.TemplateId,
                        appstartdate = app.CreatedDate,
                        stagetypename = (stagetypetranslation.Where(x => x.StageTypeId == stg.StageTypeId).Select(y => y.StageName).FirstOrDefault())
                    }
                   );//.ToListAsync(); 

                //we need to get All name of template for All applications 
                //if template id ==null then template name is null by left join in query
                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id into g
                                     from q in g.DefaultIfEmpty()
                                     select new AllapplicationDto
                                     {
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         fee = q1.fee,
                                         templateid = q.Id,
                                         appstartdate = q1.appstartdate,
                                         templatename = q.TitleShortcut,
                                         doctypeid = q.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid,
                                         stagetypename = q1.stagetypename

                                     });//.ToListAsync();
                                        //then we get translations of template name 
                                        //if template name ==null then template name translation is null by left join in query
                var querytemplatetrans = (from q1 in templateQuery
                                          join tm in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang) on q1.templatename equals tm.Shortcut into g
                                          from q in g.DefaultIfEmpty()
                                          select new AllapplicationDto
                                          {
                                              percent = q1.percent,
                                              lastUpdateStage = q1.lastUpdateStage,
                                              periodLate = q1.periodLate,
                                              appStatusId = q1.appStatusId,
                                              appid = q1.appid,
                                              serviceid = q1.serviceid,
                                              servicename = q1.servicename,
                                              fee = q1.fee,
                                              templateid = q.Id,
                                              appstartdate = q1.appstartdate,
                                              templatename = q.Value,
                                              doctypeid = q1.doctypeid,
                                              stagetypeid = q1.stagetypeid,
                                              stagetypename = q1.stagetypename

                                          });//.ToListAsync();

                //now i need to get document name fro every app
                // if documentid==null then document name=null by left join in query
                var documentNameQuery = (
                       from qtl in querytemplatetrans
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id into g
                       from q in g.DefaultIfEmpty()


                       select new AllapplicationDto
                       {
                           percent = qtl.percent,
                           lastUpdateStage = qtl.lastUpdateStage,
                           periodLate = qtl.periodLate,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           fee = qtl.fee,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           documentname = q.Shortcut,
                           stagetypeid = qtl.stagetypeid,
                           stagetypename = qtl.stagetypename
                       }
                       );
                //get translations of document name 
                //if  document name=null then document name translation is null by left join in query
                var querydoctrans = (
                    from q3 in documentNameQuery
                    join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang)
                    on q3.documentname equals tr.Shortcut into g
                    from q in g.DefaultIfEmpty()
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q.Value,
                        stagetypeid = q3.stagetypeid,
                        stagetypename = q3.stagetypename
                    }

                    );

                //get applications status translation
                var Appstatustranslate = (
                    from q3 in querydoctrans
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    join tr5 in _EngineCoreDBContext.SysTranslation
                    on lv.Shortcut equals tr5.Shortcut
                    where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        stagetypeid = q3.stagetypeid,
                        stagetypename = q3.stagetypename

                    });
                //we need to know owner of All applications
                var isownerQuery = (
                    from par in _EngineCoreDBContext.ApplicationParty
                    where par.IsOwner == true
                    select new
                    {
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile
                    });
                // get transaction for owner applications
                var userTransactionsQuery = (
                     from app in apptrak.Where(x => x.userid == userid)//_EngineCoreDBContext.Application.Where(x => (x.ServiceId.ToString().Contains(searchDto.serviceNo) || searchDto.serviceNo == null) && x.StateId != RejectedStateId)
                 join tranc in _EngineCoreDBContext.AppTransaction on app.appid equals tranc.ApplicationId
                     join q in isownerQuery on tranc.Id equals q.trancId into g

                     from q in g.DefaultIfEmpty()


                     select new AppPartyDto
                     {
                         appid = app.appid,
                         fullname = q.fullname,
                         email = q.email,
                         mobile = q.mobile
                     });//.ToListAsync();

                //apply search for All Applications


                var searchQuery = (
                    from q4 in userTransactionsQuery
                    join q5 in Appstatustranslate on q4.appid equals q5.appid

              
                    select new Apps
                    {
                        percent = q5.percent,
                        lastUpdateStage = q5.lastUpdateStage,
                        periodLate = q5.periodLate,
                        appStatusName = q5.appStatusName,
                        appStatusId = q5.appStatusId,
                        appid = q4.appid,
                        serviceid = q5.serviceid,
                        servicename = q5.servicename,
                        fee = q5.fee,
                        appstartdate = q5.appstartdate,
                        templateid = q5.templateid,
                        templatename = q5.templatename,
                        doctypeid = q5.doctypeid,
                        documentname = q5.documentname,
                        fullname = q4.fullname,
                        email = q4.email,
                        mobile = q4.mobile,
                        stagetypeid = q5.stagetypeid,
                        stagetypename = q5.stagetypename,
                       // islate = (q5.stagetypename.Contains(Constants.stageDoneEN) || q5.stagetypename.Contains(Constants.stageDoneAR) || (searchDto.AppStateId != 0) || (q5.stagetypename.Contains(Constants.stageDraftAR)) || (q5.stagetypename.Contains(Constants.stageDraftEN))) ? false : workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result,
                    // islocked = _iApplicationRepository.ifLocked((int)q4.appid, userid, lang).Result.Id == -1


                    // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                }
                    ).ToList();



                var searchLateQuery = (from q5 in searchQuery

                                           //orderby q5.appstartdate

                                       where (q5.islate)
                                       select new Apps
                                       {
                                           percent = q5.percent,
                                           lastUpdateStage = q5.lastUpdateStage,
                                           periodLate = q5.periodLate,
                                           appStatusName = q5.appStatusName,
                                           appStatusId = q5.appStatusId,
                                           appid = q5.appid,
                                           serviceid = q5.serviceid,
                                           servicename = q5.servicename,
                                           fee = q5.fee,
                                           appstartdate = q5.appstartdate,
                                           templateid = q5.templateid,
                                           templatename = q5.templatename,
                                           doctypeid = q5.doctypeid,
                                           documentname = q5.documentname,
                                           fullname = q5.fullname,
                                           email = q5.email,
                                           mobile = q5.mobile,
                                           stagetypeid = q5.stagetypeid,
                                           stagetypename = q5.stagetypename,
                                           islate = q5.islate,//workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result

                                           // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                                       }
                   );

                if (searchDto.isLate)
                {
                    int count = searchLateQuery.Count();// if islate click  on card select this
                    applicationCountDto.count = count;
                    applicationCountDto.Applications = searchLateQuery.Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                }
                else
                {
                    int count = searchQuery.Count();
                    applicationCountDto.count = count; // if late not clicked on card select this 
                    applicationCountDto.Applications = searchQuery.OrderBy(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                }
            }
            else if(searchDto.stagetypeid==ReviewStageTypeId.ToString())
            {
                /*var AppStageService = // get service name and stage for each application and translate Service shortcut
                   (from app in _EngineCoreDBContext.Application
                    join srv in _EngineCoreDBContext.AdmService
                    on app.ServiceId equals srv.Id
                    join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId == ReviewStageTypeId)
                    on app.CurrentStageId equals stg.Id
                   
                    join tr in _EngineCoreDBContext.SysTranslation
                    on srv.Shortcut equals tr.Shortcut

                    where tr.Lang == lang && app.StateId != RejectedStateId
                    select new AllapplicationDto
                    {
                        percent = 0,//(float)100 / (_EngineCoreDBContext.AdmStage.Where(x => x.ServiceId == app.ServiceId).Count() - 1) * (float)stg.OrderNo,
                        lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                        periodLate = (int)stg.PeriodForLate,
                        appStatusId = app.StateId,
                        appid = app.Id,
                        stagetypeid = stg.StageTypeId,
                        serviceid = srv.Id,
                        servicename = tr.Value,
                        fee = srv.Fee,
                        templateid = app.TemplateId,
                        appstartdate = app.CreatedDate,
                        stagetypename = (stagetypetranslation.Where(x => x.Id == stg.StageTypeId).Select(y => y.Value).FirstOrDefault())
                    }
                   );//.ToListAsync(); 

                //we need to get All name of template for All applications 
                //if template id ==null then template name is null by left join in query
                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id into g
                                     from q in g.DefaultIfEmpty()
                                     select new AllapplicationDto
                                     {
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         fee = q1.fee,
                                         templateid = q.Id,
                                         appstartdate = q1.appstartdate,
                                         templatename = q.TitleShortcut,
                                         doctypeid = q.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid,
                                         stagetypename = q1.stagetypename

                                     });//.ToListAsync();
                                        //then we get translations of template name 
                                        //if template name ==null then template name translation is null by left join in query
                var querytemplatetrans = (from q1 in templateQuery
                                          join tm in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang) on q1.templatename equals tm.Shortcut into g
                                          from q in g.DefaultIfEmpty()
                                          select new AllapplicationDto
                                          {
                                              percent = q1.percent,
                                              lastUpdateStage = q1.lastUpdateStage,
                                              periodLate = q1.periodLate,
                                              appStatusId = q1.appStatusId,
                                              appid = q1.appid,
                                              serviceid = q1.serviceid,
                                              servicename = q1.servicename,
                                              fee = q1.fee,
                                              templateid = q.Id,
                                              appstartdate = q1.appstartdate,
                                              templatename = q.Value,
                                              doctypeid = q1.doctypeid,
                                              stagetypeid = q1.stagetypeid,
                                              stagetypename = q1.stagetypename

                                          });//.ToListAsync();

                //now i need to get document name fro every app
                // if documentid==null then document name=null by left join in query
                var documentNameQuery = (
                       from qtl in querytemplatetrans
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id into g
                       from q in g.DefaultIfEmpty()


                       select new AllapplicationDto
                       {
                           percent = qtl.percent,
                           lastUpdateStage = qtl.lastUpdateStage,
                           periodLate = qtl.periodLate,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           fee = qtl.fee,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           documentname = q.Shortcut,
                           stagetypeid = qtl.stagetypeid,
                           stagetypename = qtl.stagetypename
                       }
                       );
                //get translations of document name 
                //if  document name=null then document name translation is null by left join in query
                var querydoctrans = (
                    from q3 in documentNameQuery
                    join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang)
                    on q3.documentname equals tr.Shortcut into g
                    from q in g.DefaultIfEmpty()
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q.Value,
                        stagetypeid = q3.stagetypeid,
                        stagetypename = q3.stagetypename
                    }

                    );

                //get applications status translation
                var Appstatustranslate = (
                    from q3 in querydoctrans
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    join tr5 in _EngineCoreDBContext.SysTranslation
                    on lv.Shortcut equals tr5.Shortcut
                    where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        stagetypeid = q3.stagetypeid,
                        stagetypename = q3.stagetypename,
                        UserName = employees.Contains(apptrak.Where(y => y.appid == q3.appid).Select(z => z.userid).FirstOrDefault().Value) ?
                                              _EngineCoreDBContext.User.Where(x => x.Id == apptrak.Where(y => y.appid == q3.appid).Select(z => z.userid).FirstOrDefault()).Select(p => p.FullName).FirstOrDefault() : null


                    });
                //we need to know owner of All applications
                var isownerQuery = (
                    from par in _EngineCoreDBContext.ApplicationParty
                    where par.IsOwner == true
                    select new
                    {
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile
                    });
                // get transaction for owner applications
                var userTransactionsQuery = (
                    from app in  _EngineCoreDBContext.Application.Where(x => x.StateId != RejectedStateId)
                     join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                     join q in isownerQuery on tranc.Id equals q.trancId into g

                     from q in g.DefaultIfEmpty()


                     select new AppPartyDto
                     {
                         appid = app.Id,
                         fullname = q.fullname,
                         email = q.email,
                         mobile = q.mobile
                     });//.ToListAsync();

                //apply search for All Applications


                var searchQuery = (
                    from q4 in userTransactionsQuery
                    join q5 in Appstatustranslate on q4.appid equals q5.appid

                    //orderby q5.appstartdate

                    //where (q4.appid.ToString().Contains(searchDto.Id) || searchDto.Id == null) &&
                    //           (q4.fullname.Contains(searchDto.Name) || searchDto.Name == null) &&
                    //           (q4.mobile.Contains(searchDto.Phone) || searchDto.Phone == null) &&
                    //           (q4.email.Contains(searchDto.Email) || searchDto.Email == null) &&
                    //           (q5.stagetypeid.ToString().Contains(searchDto.stagetypeid) || searchDto.stagetypeid == null)
                    select new Apps
                    {
                        percent = q5.percent,
                        lastUpdateStage = q5.lastUpdateStage,
                        periodLate = q5.periodLate,
                        appStatusName = q5.appStatusName,
                        appStatusId = q5.appStatusId,
                        appid = q4.appid,
                        serviceid = q5.serviceid,
                        servicename = q5.servicename,
                        fee = q5.fee,
                        appstartdate = q5.appstartdate,
                        templateid = q5.templateid,
                        templatename = q5.templatename,
                        doctypeid = q5.doctypeid,
                        documentname = q5.documentname,
                        fullname = q4.fullname,
                        email = q4.email,
                        mobile = q4.mobile,
                        stagetypeid = q5.stagetypeid,
                        stagetypename = q5.stagetypename,
                        UserName=q5.UserName,
                         islate = (q5.stagetypename.Contains(Constants.stageDoneEN) || q5.stagetypename.Contains(Constants.stageDoneAR) || (searchDto.AppStateId != 0) || (q5.stagetypename.Contains(Constants.stageDraftAR)) || (q5.stagetypename.Contains(Constants.stageDraftEN))) ? false : workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result,
                        islocked = _iApplicationRepository.IfBookedUp((int)q4.appid, userid, lang).Result.Id == -1


                        // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                    }
                    ).ToList();



                var searchLateQuery = (from q5 in searchQuery

                                           //orderby q5.appstartdate

                                       where (q5.islate)
                                       select new Apps
                                       {
                                           percent = q5.percent,
                                           lastUpdateStage = q5.lastUpdateStage,
                                           periodLate = q5.periodLate,
                                           appStatusName = q5.appStatusName,
                                           appStatusId = q5.appStatusId,
                                           appid = q5.appid,
                                           serviceid = q5.serviceid,
                                           servicename = q5.servicename,
                                           fee = q5.fee,
                                           appstartdate = q5.appstartdate,
                                           templateid = q5.templateid,
                                           templatename = q5.templatename,
                                           doctypeid = q5.doctypeid,
                                           documentname = q5.documentname,
                                           fullname = q5.fullname,
                                           email = q5.email,
                                           mobile = q5.mobile,
                                           stagetypeid = q5.stagetypeid,
                                           stagetypename = q5.stagetypename,
                                           islate = q5.islate,//workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result
                                           islocked=q5.islocked,
                                           // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                                           UserName=q5.UserName
                                       }
                   );

                if (searchDto.isLate)
                {
                    int count = searchLateQuery.Count();// if islate click  on card select this
                    applicationCountDto.count = count;
                    applicationCountDto.Applications = searchLateQuery.Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                }
                else
                {
                    int count = searchQuery.Count();
                    applicationCountDto.count = count; // if late not clicked on card select this 
                    applicationCountDto.Applications = searchQuery.OrderBy(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                }*/
                applicationCountDto.StageTypeId = ReviewStageTypeId;
            }

            else if (searchDto.stagetypeid == DoneStageTypeId.ToString())
            {
                var AppStageService = // get service name and stage for each application and translate Service shortcut
                   (from app in _EngineCoreDBContext.Application
                    join pt in apptrak.Where(x => x.userid == userid)
                  on app.Id equals pt.appid
                    join srv in _EngineCoreDBContext.AdmService
                    on app.ServiceId equals srv.Id
                    join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId == DoneStageTypeId)
                    on app.CurrentStageId equals stg.Id

                    join tr in _EngineCoreDBContext.SysTranslation
                    on srv.Shortcut equals tr.Shortcut

                    where tr.Lang == lang && app.StateId != RejectedStateId
                    select new AllapplicationDto
                    {
                        percent = 100,
                        lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                        periodLate = (int)stg.PeriodForLate,
                        appStatusId = app.StateId,
                        appid = app.Id,
                        stagetypeid = stg.StageTypeId,
                        serviceid = srv.Id,
                        servicename = tr.Value,
                        fee = srv.Fee,
                        templateid = app.TemplateId,
                        appstartdate = app.CreatedDate,
                        stagetypename = (stagetypetranslation.Where(x => x.StageTypeId == stg.StageTypeId).Select(y => y.StageName).FirstOrDefault())
                    }
                   );//.ToListAsync(); 

                //we need to get All name of template for All applications 
                //if template id ==null then template name is null by left join in query
                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id into g
                                     from q in g.DefaultIfEmpty()
                                     select new AllapplicationDto
                                     {
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         fee = q1.fee,
                                         templateid = q.Id,
                                         appstartdate = q1.appstartdate,
                                         templatename = q.TitleShortcut,
                                         doctypeid = q.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid,
                                         stagetypename = q1.stagetypename

                                     });//.ToListAsync();
                                        //then we get translations of template name 
                                        //if template name ==null then template name translation is null by left join in query
                var querytemplatetrans = (from q1 in templateQuery
                                          join tm in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang) on q1.templatename equals tm.Shortcut into g
                                          from q in g.DefaultIfEmpty()
                                          select new AllapplicationDto
                                          {
                                              percent = q1.percent,
                                              lastUpdateStage = q1.lastUpdateStage,
                                              periodLate = q1.periodLate,
                                              appStatusId = q1.appStatusId,
                                              appid = q1.appid,
                                              serviceid = q1.serviceid,
                                              servicename = q1.servicename,
                                              fee = q1.fee,
                                              templateid = q.Id,
                                              appstartdate = q1.appstartdate,
                                              templatename = q.Value,
                                              doctypeid = q1.doctypeid,
                                              stagetypeid = q1.stagetypeid,
                                              stagetypename = q1.stagetypename

                                          });//.ToListAsync();

                //now i need to get document name fro every app
                // if documentid==null then document name=null by left join in query
                var documentNameQuery = (
                       from qtl in querytemplatetrans
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id into g
                       from q in g.DefaultIfEmpty()


                       select new AllapplicationDto
                       {
                           percent = qtl.percent,
                           lastUpdateStage = qtl.lastUpdateStage,
                           periodLate = qtl.periodLate,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           fee = qtl.fee,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           documentname = q.Shortcut,
                           stagetypeid = qtl.stagetypeid,
                           stagetypename = qtl.stagetypename
                       }
                       );
                //get translations of document name 
                //if  document name=null then document name translation is null by left join in query
                var querydoctrans = (
                    from q3 in documentNameQuery
                    join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang)
                    on q3.documentname equals tr.Shortcut into g
                    from q in g.DefaultIfEmpty()
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q.Value,
                        stagetypeid = q3.stagetypeid,
                        stagetypename = q3.stagetypename
                    }

                    );

                //get applications status translation
                var Appstatustranslate = (
                    from q3 in querydoctrans
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    join tr5 in _EngineCoreDBContext.SysTranslation
                    on lv.Shortcut equals tr5.Shortcut
                    where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        stagetypeid = q3.stagetypeid,
                        stagetypename = q3.stagetypename,
                        //UserName = employees.Contains(apptrak.Where(y => y.appid == q3.appid).Select(z => z.userid).FirstOrDefault().Value) ?
                        //                      _EngineCoreDBContext.User.Where(x => x.Id == apptrak.Where(y => y.appid == q3.appid).Select(z => z.userid).FirstOrDefault()).Select(p => p.FullName).FirstOrDefault() : null


                    });
                //we need to know owner of All applications
                var isownerQuery = (
                    from par in _EngineCoreDBContext.ApplicationParty
                    where par.IsOwner == true
                    select new
                    {
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile
                    });
                // get transaction for owner applications
                var userTransactionsQuery = (
                    from app in _EngineCoreDBContext.Application.Where(x => x.StateId != RejectedStateId)
                    join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                    join q in isownerQuery on tranc.Id equals q.trancId into g

                    from q in g.DefaultIfEmpty()


                    select new AppPartyDto
                    {
                        appid = app.Id,
                        fullname = q.fullname,
                        email = q.email,
                        mobile = q.mobile
                    });//.ToListAsync();

                //apply search for All Applications


                var searchQuery = (
                    from q4 in userTransactionsQuery
                    join q5 in Appstatustranslate on q4.appid equals q5.appid

                   
                    select new Apps
                    {
                        percent = q5.percent,
                        lastUpdateStage = q5.lastUpdateStage,
                        periodLate = q5.periodLate,
                        appStatusName = q5.appStatusName,
                        appStatusId = q5.appStatusId,
                        appid = q4.appid,
                        serviceid = q5.serviceid,
                        servicename = q5.servicename,
                        fee = q5.fee,
                        appstartdate = q5.appstartdate,
                        templateid = q5.templateid,
                        templatename = q5.templatename,
                        doctypeid = q5.doctypeid,
                        documentname = q5.documentname,
                        fullname = q4.fullname,
                        email = q4.email,
                        mobile = q4.mobile,
                        stagetypeid = q5.stagetypeid,
                        stagetypename = q5.stagetypename,
                        UserName = q5.UserName,
                        islate = (q5.stagetypename.Contains(Constants.stageDoneEN) || q5.stagetypename.Contains(Constants.stageDoneAR) || (searchDto.AppStateId != 0) || (q5.stagetypename.Contains(Constants.stageDraftAR)) || (q5.stagetypename.Contains(Constants.stageDraftEN))) ? false : workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result,
                        islocked = _iApplicationRepository.IfBookedUp((int)q4.appid, userid, lang).Result.Id == -1


                        // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                    }
                    ).ToList();



                var searchLateQuery = (from q5 in searchQuery

                                           //orderby q5.appstartdate

                                       where (q5.islate)
                                       select new Apps
                                       {
                                           percent = q5.percent,
                                           lastUpdateStage = q5.lastUpdateStage,
                                           periodLate = q5.periodLate,
                                           appStatusName = q5.appStatusName,
                                           appStatusId = q5.appStatusId,
                                           appid = q5.appid,
                                           serviceid = q5.serviceid,
                                           servicename = q5.servicename,
                                           fee = q5.fee,
                                           appstartdate = q5.appstartdate,
                                           templateid = q5.templateid,
                                           templatename = q5.templatename,
                                           doctypeid = q5.doctypeid,
                                           documentname = q5.documentname,
                                           fullname = q5.fullname,
                                           email = q5.email,
                                           mobile = q5.mobile,
                                           stagetypeid = q5.stagetypeid,
                                           stagetypename = q5.stagetypename,
                                           islate = q5.islate,//workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result
                                           islocked = q5.islocked,
                                           // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                                           UserName = q5.UserName
                                       }
                   );

                if (searchDto.isLate)
                {
                    int count = searchLateQuery.Count();// if islate click  on card select this
                    applicationCountDto.count = count;
                    applicationCountDto.Applications = searchLateQuery.Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                }
                else
                {
                    int count = searchQuery.Count();
                    applicationCountDto.count = count; // if late not clicked on card select this 
                    applicationCountDto.Applications = searchQuery.OrderByDescending(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                }
            }
            else if (searchDto.stagetypeid == MeetingStageId.ToString())
            {
                var AppStageService = // get service name and stage for each application and translate Service shortcut
                   (from app in _EngineCoreDBContext.Application.Where(x=>x.Id>0 && x.StateId!=RejectedStateId && x.StateId!= AutoCancelId)
                    join pt in apptrak.Where(x => x.userid == userid)
                  on app.Id equals pt.appid
                    join srv in _EngineCoreDBContext.AdmService
                    on app.ServiceId equals srv.Id
                    join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId == MeetingStageId)
                    on app.CurrentStageId equals stg.Id

                    //join tr in _EngineCoreDBContext.SysTranslation
                    //on srv.Shortcut equals tr.Shortcut

                    //where tr.Lang == lang && app.StateId != RejectedStateId
                    select new AllapplicationDto
                    {
                        percent = 75,
                        lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                        periodLate = (int)stg.PeriodForLate,
                        appStatusId = app.StateId,
                        appid = app.Id,
                        stagetypeid = stg.StageTypeId,
                        serviceid = srv.Id,
                       // servicename = tr.Value,
                        fee = srv.Fee,
                        templateid = app.TemplateId,
                        appstartdate = app.CreatedDate,
                       // stagetypename = (stagetypetranslation.Where(x => x.Id == stg.StageTypeId).Select(y => y.Value).FirstOrDefault())
                    }
                   );

                //we need to get All name of template for All applications 
                //if template id ==null then template name is null by left join in query
                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id //into g
                                    // from q in g.DefaultIfEmpty()
                                     select new AllapplicationDto
                                     {
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                        // servicename = q1.servicename,
                                         fee = q1.fee,
                                         templateid = tm.Id,
                                         appstartdate = q1.appstartdate,
                                        // templatename = q.TitleShortcut,
                                         doctypeid = tm.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid,
                                      //   stagetypename = q1.stagetypename

                                     });
                                        //then we get translations of template name 
                                        //if template name ==null then template name translation is null by left join in query
                //var querytemplatetrans = (from q1 in templateQuery
                //                          join tm in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang) on q1.templatename equals tm.Shortcut into g
                //                          from q in g.DefaultIfEmpty()
                //                          select new AllapplicationDto
                //                          {
                //                              percent = q1.percent,
                //                              lastUpdateStage = q1.lastUpdateStage,
                //                              periodLate = q1.periodLate,
                //                              appStatusId = q1.appStatusId,
                //                              appid = q1.appid,
                //                              serviceid = q1.serviceid,
                //                              servicename = q1.servicename,
                //                              fee = q1.fee,
                //                              templateid = q.Id,
                //                              appstartdate = q1.appstartdate,
                //                              templatename = q.Value,
                //                              doctypeid = q1.doctypeid,
                //                              stagetypeid = q1.stagetypeid,
                //                              stagetypename = q1.stagetypename

                //                          });//.ToListAsync();

                //now i need to get document name fro every app
                // if documentid==null then document name=null by left join in query
                //var documentNameQuery = (
                //       from qtl in templateQuery
                //       join lv in _EngineCoreDBContext.SysLookupValue
                //       on qtl.doctypeid equals lv.Id into g
                //       from q in g.DefaultIfEmpty()


                //       select new AllapplicationDto
                //       {
                //           percent = qtl.percent,
                //           lastUpdateStage = qtl.lastUpdateStage,
                //           periodLate = qtl.periodLate,
                //           appStatusId = qtl.appStatusId,
                //           appid = qtl.appid,
                //           serviceid = qtl.serviceid,
                //           servicename = qtl.servicename,
                //           fee = qtl.fee,
                //           templateid = qtl.templateid,
                //           appstartdate = qtl.appstartdate,
                //           templatename = qtl.templatename,
                //           doctypeid = qtl.doctypeid,
                //           documentname = q.Shortcut,
                //           stagetypeid = qtl.stagetypeid,
                //           stagetypename = qtl.stagetypename
                //       }
                //       );
                ////get translations of document name 
                //if  document name=null then document name translation is null by left join in query
                //var querydoctrans = (
                //    from q3 in documentNameQuery
                //    join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang)
                //    on q3.documentname equals tr.Shortcut into g
                //    from q in g.DefaultIfEmpty()
                //    select new AllapplicationDto
                //    {
                //        percent = q3.percent,
                //        lastUpdateStage = q3.lastUpdateStage,
                //        periodLate = q3.periodLate,
                //        appStatusId = q3.appStatusId,
                //        appid = q3.appid,
                //        serviceid = q3.serviceid,
                //        servicename = q3.servicename,
                //        fee = q3.fee,
                //        templateid = q3.templateid,
                //        appstartdate = q3.appstartdate,
                //        templatename = q3.templatename,
                //        doctypeid = q3.doctypeid,
                //        documentname = q.Value,
                //        stagetypeid = q3.stagetypeid,
                //        stagetypename = q3.stagetypename
                //    }

                //    );

                //get applications status translation
                var Appstatustranslate = (
                    from q3 in templateQuery
                    //join lv in _EngineCoreDBContext.SysLookupValue
                    //on q3.appStatusId equals lv.Id
                    //join tr5 in _EngineCoreDBContext.SysTranslation
                    //on lv.Shortcut equals tr5.Shortcut
                    //where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                       // appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                       // servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                      //  templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                       // documentname = q3.documentname,
                        stagetypeid = q3.stagetypeid,
                        //   stagetypename = q3.stagetypename,
                        //UserName  = q3.appid > 0 ?
                        //   apptrak.FirstOrDefault(x => x.appid == q3.appid).username : null,
                        isOnlineStatus = _IMeetingRepository.IsAttendedByAppNo((int)q3.appid).Result.IsOnline && _IMeetingRepository.IsAttendedByAppNo((int)q3.appid).Result.IsLate ? 2
                                     : _IMeetingRepository.IsAttendedByAppNo((int)q3.appid).Result.IsOnline && !_IMeetingRepository.IsAttendedByAppNo((int)q3.appid).Result.IsLate ? 1 : 0


                    });
                //we need to know owner of All applications
                var isownerQuery = (
                    from par in _EngineCoreDBContext.ApplicationParty
                    where par.IsOwner == true
                    select new
                    {
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile
                    });
                // get transaction for owner applications
                var userTransactionsQuery = (
                    from app in _EngineCoreDBContext.Application.Where(x => x.StateId != RejectedStateId)
                    join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                    join q in isownerQuery on tranc.Id equals q.trancId into g

                    from q in g.DefaultIfEmpty()


                    select new AppPartyDto
                    {
                        appid = app.Id,
                        fullname = q.fullname,
                        email = q.email,
                        mobile = q.mobile
                    });//.ToListAsync();

                //apply search for All Applications


                var searchQuery = (
                    from q4 in userTransactionsQuery
                    join q5 in Appstatustranslate on q4.appid equals q5.appid

                   
                    select new Apps
                    {
                        percent = q5.percent,
                        lastUpdateStage = q5.lastUpdateStage,
                        periodLate = q5.periodLate,
                        appStatusName = q5.appStatusName,
                        appStatusId = q5.appStatusId,
                        appid = q4.appid,
                        serviceid = q5.serviceid,
                        servicename = services[(int)q5.serviceid],
                        fee = q5.fee,
                        appstartdate = q5.appstartdate,
                        templateid = q5.templateid,
                        templatename = templates[(int)q5.templateid],
                        doctypeid = q5.doctypeid,
                        documentname = documents[(int)q5.doctypeid],
                        fullname = q4.fullname,
                        email = q4.email,
                        mobile = q4.mobile,
                        stagetypeid = q5.stagetypeid,
                        stagetypename = stages[(int)q5.stagetypeid],
                        UserName = q5.UserName,
                        islocked = q4.appid > 0 ? DateTime.Now.Subtract((DateTime)q5.LastDateReader).TotalSeconds < Constants.LOCK_SECONDS_TIME ? true : false : false,
                        islate = q5.lastUpdateStage >= lateDate[q5.periodLate] ? false : true,
                        isOnlineStatus =q5.isOnlineStatus
                        

                       
                    }
                    ).ToList();



                var searchLateQuery = (from q5 in searchQuery

                                           //orderby q5.appstartdate

                                       where (q5.islate)
                                       select new Apps
                                       {
                                           percent = q5.percent,
                                           lastUpdateStage = q5.lastUpdateStage,
                                           periodLate = q5.periodLate,
                                           appStatusName = q5.appStatusName,
                                           appStatusId = q5.appStatusId,
                                           appid = q5.appid,
                                           serviceid = q5.serviceid,
                                           servicename = q5.servicename,
                                           fee = q5.fee,
                                           appstartdate = q5.appstartdate,
                                           templateid = q5.templateid,
                                           templatename = q5.templatename,
                                           doctypeid = q5.doctypeid,
                                           documentname = q5.documentname,
                                           fullname = q5.fullname,
                                           email = q5.email,
                                           mobile = q5.mobile,
                                           stagetypeid = q5.stagetypeid,
                                           stagetypename = q5.stagetypename,
                                           islate = q5.islate,//workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result
                                           islocked = q5.islocked,
                                           // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                                           UserName = q5.UserName
                                       }
                   );
                int CountMeeting = searchQuery.Where(x => x.isOnlineStatus == 1 || x.isOnlineStatus == 2).Count();
                if (searchDto.isLate)
                {
                    int count = searchLateQuery.Count();// if islate click  on card select this
                    applicationCountDto.count = count;
                    applicationCountDto.Applications = searchLateQuery.Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                }
                else if(CountMeeting>0)
                {
                    int count = searchQuery.Count();
                    applicationCountDto.count = count; // if late not clicked on card select this 
                    applicationCountDto.Applications = searchQuery.OrderByDescending(x => x.isOnlineStatus).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                }
                else 
                {
                    int count = searchQuery.Count();
                    applicationCountDto.count = count; // if late not clicked on card select this 
                    applicationCountDto.Applications = searchQuery.OrderByDescending(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                }
            }
            else
            
                {
                    var AppStageService = // get service name and stage for each application and translate Service shortcut
                       (from app in _EngineCoreDBContext.Application.Where(x => x.StateId != RejectedStateId)
                        join pt in apptrak.Where(x => x.userid == userid)
                        on app.Id equals pt.appid
                        join srv in _EngineCoreDBContext.AdmService
                        on app.ServiceId equals srv.Id
                        join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId != ReviewStageTypeId && x.StageTypeId!=draftId)
                        on app.CurrentStageId equals stg.Id
                        join tr in _EngineCoreDBContext.SysTranslation
                        on srv.Shortcut equals tr.Shortcut

                        where tr.Lang == lang
                        select new AllapplicationDto
                        {
                            percent = (int)(stagetypetranslation.Where(x => x.StageTypeId == stg.StageTypeId).Select(y => y.percent).FirstOrDefault()),
                        lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                            periodLate = (int)stg.PeriodForLate,
                            appStatusId = app.StateId,
                            appid = app.Id,
                            stagetypeid = stg.StageTypeId,
                            serviceid = srv.Id,
                            servicename = tr.Value,
                            fee = srv.Fee,
                            templateid = app.TemplateId,
                            appstartdate = app.CreatedDate,
                            stagetypename = (stagetypetranslation.Where(x => x.StageTypeId == stg.StageTypeId).Select(y => y.StageName).FirstOrDefault())
                        }
                       );//.ToListAsync(); 

                    //we need to get All name of template for All applications 
                    //if template id ==null then template name is null by left join in query
                    var templateQuery = (from q1 in AppStageService
                                         join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id into g
                                         from q in g.DefaultIfEmpty()
                                         select new AllapplicationDto
                                         {
                                             percent = q1.percent,
                                             lastUpdateStage = q1.lastUpdateStage,
                                             periodLate = q1.periodLate,
                                             appStatusId = q1.appStatusId,
                                             appid = q1.appid,
                                             serviceid = q1.serviceid,
                                             servicename = q1.servicename,
                                             fee = q1.fee,
                                             templateid = q.Id,
                                             appstartdate = q1.appstartdate,
                                             templatename = q.TitleShortcut,
                                             doctypeid = q.DocumentTypeId,
                                             stagetypeid = q1.stagetypeid,
                                             stagetypename = q1.stagetypename

                                         });//.ToListAsync();
                                            //then we get translations of template name 
                                            //if template name ==null then template name translation is null by left join in query
                    var querytemplatetrans = (from q1 in templateQuery
                                              join tm in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang) on q1.templatename equals tm.Shortcut into g
                                              from q in g.DefaultIfEmpty()
                                              select new AllapplicationDto
                                              {
                                                  percent = q1.percent,
                                                  lastUpdateStage = q1.lastUpdateStage,
                                                  periodLate = q1.periodLate,
                                                  appStatusId = q1.appStatusId,
                                                  appid = q1.appid,
                                                  serviceid = q1.serviceid,
                                                  servicename = q1.servicename,
                                                  fee = q1.fee,
                                                  templateid = q.Id,
                                                  appstartdate = q1.appstartdate,
                                                  templatename = q.Value,
                                                  doctypeid = q1.doctypeid,
                                                  stagetypeid = q1.stagetypeid,
                                                  stagetypename = q1.stagetypename

                                              });//.ToListAsync();

                    //now i need to get document name fro every app
                    // if documentid==null then document name=null by left join in query
                    var documentNameQuery = (
                           from qtl in querytemplatetrans
                           join lv in _EngineCoreDBContext.SysLookupValue
                           on qtl.doctypeid equals lv.Id into g
                           from q in g.DefaultIfEmpty()


                           select new AllapplicationDto
                           {
                               percent = qtl.percent,
                               lastUpdateStage = qtl.lastUpdateStage,
                               periodLate = qtl.periodLate,
                               appStatusId = qtl.appStatusId,
                               appid = qtl.appid,
                               serviceid = qtl.serviceid,
                               servicename = qtl.servicename,
                               fee = qtl.fee,
                               templateid = qtl.templateid,
                               appstartdate = qtl.appstartdate,
                               templatename = qtl.templatename,
                               doctypeid = qtl.doctypeid,
                               documentname = q.Shortcut,
                               stagetypeid = qtl.stagetypeid,
                               stagetypename = qtl.stagetypename
                           }
                           );
                    //get translations of document name 
                    //if  document name=null then document name translation is null by left join in query
                    var querydoctrans = (
                        from q3 in documentNameQuery
                        join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang)
                        on q3.documentname equals tr.Shortcut into g
                        from q in g.DefaultIfEmpty()
                        select new AllapplicationDto
                        {
                            percent = q3.percent,
                            lastUpdateStage = q3.lastUpdateStage,
                            periodLate = q3.periodLate,
                            appStatusId = q3.appStatusId,
                            appid = q3.appid,
                            serviceid = q3.serviceid,
                            servicename = q3.servicename,
                            fee = q3.fee,
                            templateid = q3.templateid,
                            appstartdate = q3.appstartdate,
                            templatename = q3.templatename,
                            doctypeid = q3.doctypeid,
                            documentname = q.Value,
                            stagetypeid = q3.stagetypeid,
                            stagetypename = q3.stagetypename
                        }

                        );

                    //get applications status translation
                    var Appstatustranslate = (
                        from q3 in querydoctrans
                        join lv in _EngineCoreDBContext.SysLookupValue
                        on q3.appStatusId equals lv.Id
                        join tr5 in _EngineCoreDBContext.SysTranslation
                        on lv.Shortcut equals tr5.Shortcut
                        where tr5.Lang == lang
                        select new AllapplicationDto
                        {
                            percent = q3.percent,
                            lastUpdateStage = q3.lastUpdateStage,
                            periodLate = q3.periodLate,
                            appStatusName = tr5.Value,
                            appStatusId = q3.appStatusId,
                            appid = q3.appid,
                            serviceid = q3.serviceid,
                            servicename = q3.servicename,
                            fee = q3.fee,
                            templateid = q3.templateid,
                            appstartdate = q3.appstartdate,
                            templatename = q3.templatename,
                            doctypeid = q3.doctypeid,
                            documentname = q3.documentname,
                            stagetypeid = q3.stagetypeid,
                            stagetypename = q3.stagetypename,
                            //UserName = employees.Contains(apptrak.Where(y => y.appid == q3.appid).Select(z => z.userid).FirstOrDefault().Value) ?
                            //                      _EngineCoreDBContext.User.Where(x => x.Id == apptrak.Where(y => y.appid == q3.appid).Select(z => z.userid).FirstOrDefault()).Select(p => p.FullName).FirstOrDefault() : null


                        });
                    //we need to know owner of All applications
                    var isownerQuery = (
                        from par in _EngineCoreDBContext.ApplicationParty
                        where par.IsOwner == true
                        select new
                        {
                            trancId = par.TransactionId,
                            fullname = par.FullName,
                            email = par.Email,
                            mobile = par.Mobile
                        });
                    // get transaction for owner applications
                    var userTransactionsQuery = (
                        from app in apptrak//.Where(x => x.userid == userid)// _EngineCoreDBContext.Application.Where(x => (x.ServiceId.ToString().Contains(searchDto.serviceNo) || searchDto.serviceNo == null) && x.StateId != RejectedStateId)
                    join tranc in _EngineCoreDBContext.AppTransaction on app.appid equals tranc.ApplicationId
                        join q in isownerQuery on tranc.Id equals q.trancId into g

                        from q in g.DefaultIfEmpty()


                        select new AppPartyDto
                        {
                            appid = app.appid,
                            fullname = q.fullname,
                            email = q.email,
                            mobile = q.mobile
                        });//.ToListAsync();

                    //apply search for All Applications


                    var searchQuery = (
                        from q4 in userTransactionsQuery
                        join q5 in Appstatustranslate on q4.appid equals q5.appid

                    //orderby q5.appstartdate

                    where// (q4.appid.ToString().Contains(searchDto.Id) || searchDto.Id == null) &&
                    //               (q4.fullname.Contains(searchDto.Name) || searchDto.Name == null) &&
                    //               (q4.mobile.Contains(searchDto.Phone) || searchDto.Phone == null) &&
                    //               (q4.email.Contains(searchDto.Email) || searchDto.Email == null) &&
                                   (q5.stagetypeid.ToString().Contains(searchDto.stagetypeid) || searchDto.stagetypeid == null)
                        select new Apps
                        {
                            percent = q5.percent,
                            lastUpdateStage = q5.lastUpdateStage,
                            periodLate = q5.periodLate,
                            appStatusName = q5.appStatusName,
                            appStatusId = q5.appStatusId,
                            appid = q4.appid,
                            serviceid = q5.serviceid,
                            servicename = q5.servicename,
                            fee = q5.fee,
                            appstartdate = q5.appstartdate,
                            templateid = q5.templateid,
                            templatename = q5.templatename,
                            doctypeid = q5.doctypeid,
                            documentname = q5.documentname,
                            fullname = q4.fullname,
                            email = q4.email,
                            mobile = q4.mobile,
                            stagetypeid = q5.stagetypeid,
                            stagetypename = q5.stagetypename,
                            UserName = q5.UserName,
                            islate = (q5.stagetypename.Contains(Constants.stageDoneEN) || q5.stagetypename.Contains(Constants.stageDoneAR) || (searchDto.AppStateId != 0) || (q5.stagetypename.Contains(Constants.stageDraftAR)) || (q5.stagetypename.Contains(Constants.stageDraftEN))) ? false : workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result,
                            islocked = _iApplicationRepository.IfBookedUp((int)q4.appid, userid, lang).Result.Id == -1


                        // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                    }
                        ).ToList();



                    var searchLateQuery = (from q5 in searchQuery

                                               //orderby q5.appstartdate

                                           where (q5.islate)
                                           select new Apps
                                           {
                                               percent = q5.percent,
                                               lastUpdateStage = q5.lastUpdateStage,
                                               periodLate = q5.periodLate,
                                               appStatusName = q5.appStatusName,
                                               appStatusId = q5.appStatusId,
                                               appid = q5.appid,
                                               serviceid = q5.serviceid,
                                               servicename = q5.servicename,
                                               fee = q5.fee,
                                               appstartdate = q5.appstartdate,
                                               templateid = q5.templateid,
                                               templatename = q5.templatename,
                                               doctypeid = q5.doctypeid,
                                               documentname = q5.documentname,
                                               fullname = q5.fullname,
                                               email = q5.email,
                                               mobile = q5.mobile,
                                               stagetypeid = q5.stagetypeid,
                                               stagetypename = q5.stagetypename,
                                               islate = q5.islate,//workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result
                                               islocked = q5.islocked,
                                               // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                                               UserName = q5.UserName
                                           }
                       );

                    if (searchDto.isLate)
                    {
                        int count = searchLateQuery.Count();// if islate click  on card select this
                        applicationCountDto.count = count;
                        applicationCountDto.Applications = searchLateQuery.Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                    }
                    else
                    {
                        int count = searchQuery.Count();
                        applicationCountDto.count = count; // if late not clicked on card select this 
                        applicationCountDto.Applications = searchQuery.OrderBy(x => x.lastUpdateStage).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();

                    }
                
            }

            return applicationCountDto;
        }

        public async Task<ApplicationCountDto> NotarySearchAll(string lang, searchDto searchDto, int currentpage, int perpage)// if onlymyApps ==true;
        {

            if (searchDto.sDate == "" || searchDto.sDate == null)
                searchDto.sDate = _EngineCoreDBContext.Application.Min(x => x.AppLastUpdateDate).ToString();
            if (searchDto.eDate == "" || searchDto.sDate == null)
                searchDto.eDate = _EngineCoreDBContext.Application.Max(x => x.AppLastUpdateDate).ToString();


            ApplicationCountDto applicationCountDto = new ApplicationCountDto();

            List<int> employees = new List<int>(_iUserRepository.GetEmployees().Result.Keys);

            int RejectedStateId = await _ISysValueRepository.GetIdByShortcut("REJECTED");
            int ReturnedStateId = await _ISysValueRepository.GetIdByShortcut("RETURNED");
            int DoneStateId = await _ISysValueRepository.GetIdByShortcut("DONE");
            int ReviewStageTypeId = await _ISysValueRepository.GetIdByShortcut("REVIEW");
            int DoneStageTypeId = await _ISysValueRepository.GetIdByShortcut("SysLookupValue_Shortcut677");
            int RejectedStageTypeId = await _ISysValueRepository.GetIdByShortcut("REJECTEDSTAGE");
            SysLookupType sysLookupType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "stage_type").FirstOrDefault();
            int lookupTypeId = sysLookupType.Id;

            string draftshorcut = _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stageDraftAR) || x.Value.ToLower()
                 .Contains(Constants.stageDraftEN)).Select(y => y.Shortcut).FirstOrDefault();

            int draftId = _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == draftshorcut).Select(y => y.Id).FirstOrDefault();

            var stagetypetranslation = // get All stage type translation in system lookup type table
               (from lv in _EngineCoreDBContext.SysLookupValue
                join tr in _EngineCoreDBContext.SysTranslation on lv.Shortcut equals tr.Shortcut
                where
                 lv.LookupTypeId == lookupTypeId &&
                 tr.Lang == lang


                select new
                {
                    StageTypeId = lv.Id,
                    StageName = tr.Value,
                    percent = lv.Order
                });
            Dictionary<int, string> stages = new Dictionary<int, string>();
            foreach (var stg in stagetypetranslation)
            {
                stages.Add(stg.StageTypeId, stg.StageName);
            }
            Dictionary<int, string> services = new Dictionary<int, string>();
            List<ServiceNamesDto> serviceNames = _IAdmServiceRepository.GetserviceNAmes(lang).Result;
            foreach (var srv in serviceNames)
            {
                services.Add((int)srv.Id, srv.serviceName);
            }


            Dictionary<int, string> templates = new Dictionary<int, string>();
            var TemplatesTrans = (from temp in _EngineCoreDBContext.Template
                                  join tr in _EngineCoreDBContext.SysTranslation on temp.TitleShortcut equals tr.Shortcut
                                  where tr.Lang == lang
                                  select new
                                  {
                                      tempId = temp.Id,
                                      tr.Value
                                  });
            foreach (var temp in TemplatesTrans)
            {
                templates.Add(temp.tempId, temp.Value);
            }

            Dictionary<int, string> documents = new Dictionary<int, string>();
            var documentsTrans = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == 2018)
                                  join tr in _EngineCoreDBContext.SysTranslation on lv.Shortcut equals tr.Shortcut
                                  where tr.Lang == lang
                                  select new
                                  {
                                      lv.Id,
                                      tr.Value
                                  });

            foreach (var doc in documentsTrans)
            {
                documents.Add(doc.Id, doc.Value);
            }



            //DateTime startDate = DateTime.Now;
            //DateTime EDate = DateTime.Now.AddDays(1);

            //try
            //{
            //    startDate = _EngineCoreDBContext.Application.Min(x => x.AppLastUpdateDate).Value;
            //    EDate = _EngineCoreDBContext.Application.Max(x => x.AppLastUpdateDate).Value.AddMinutes(_EngineCoreDBContext.AdmStage.Max(x => x.PeriodForLate).Value * 60);
            //}
            //catch
            //{
            //    startDate = DateTime.Now;
            //    EDate = DateTime.Now.AddDays(10);
            //}

            //WorkingTimeRepository workingTimeRepository = new WorkingTimeRepository(_EngineCoreDBContext, _IGeneralRepository, _iGlobalDayOffRepository);
            //await workingTimeRepository.InitialaizeWorkingDic(startDate.Date, EDate.Date);

            //var isemployee = (from appt in _EngineCoreDBContext.ApplicationTrack
            //                  join ur in _EngineCoreDBContext.UserRole on appt.UserId equals ur.UserId
            //                  where ur.RoleId == 17
            //                  select new
            //                  {
            //                      appid = appt.ApplicationId,
            //                      userid = appt.UserId,
            //                      created_date = appt.CreatedDate
            //                  });



            //var GroupappTrak = (
            //    from appt in isemployee
            //    group appt by appt.appid into t



            //    select new
            //    {
            //        appid = t.Key,
            //        created_date = t.Max(x => x.created_date)
            //    });
            //var apptrak = (from appt in _EngineCoreDBContext.ApplicationTrack
            //               join gt in GroupappTrak on new { colA = appt.ApplicationId, colB = appt.CreatedDate } equals new { colA = gt.appid, colB = gt.created_date }
            //               join us in _EngineCoreDBContext.User on appt.UserId equals us.Id
            //               select new
            //               {
            //                   username = us.FullName,
            //                   appid = appt.ApplicationId,
            //                   userid = appt.UserId
            //               });

            int userid = _iUserRepository.GetUserID();
           




                var AppStageService = // get service name and stage for each application and translate Service shortcut
                     (from app in _EngineCoreDBContext.Application.Where(x => (x.ServiceId.ToString().Contains(searchDto.serviceNo) || searchDto.serviceNo == null) //&& x.StateId!=RejectedStateId
                          && (x.Id.ToString().Contains(searchDto.Id) || x.ApplicationNo.Contains(searchDto.Id) || searchDto.Id == null)
                         )
                      join srv in _EngineCoreDBContext.AdmService
                      on app.ServiceId equals srv.Id
                      join stg in _EngineCoreDBContext.AdmStage//.Where(x => x.StageTypeId != draftId) //&& x.StageTypeId!=DoneStageTypeId)
                      on app.CurrentStageId equals stg.Id
                      //join tr in _EngineCoreDBContext.SysTranslation
                      //on srv.Shortcut equals tr.Shortcut

                      //where tr.Lang == lang
                      select new AllapplicationDto
                      {
                          LastDateReader = (DateTime)app.LastReadDate,
                          applicationNo = app.ApplicationNo,
                          // percent = (int)(stagetypetranslation.Where(x => x.Id == stg.StageTypeId).Select(y => y.percent).FirstOrDefault()),
                          lastUpdateStage = (DateTime)app.AppLastUpdateDate,
                          periodLate = (int)stg.PeriodForLate,
                          appStatusId = app.StateId,
                          appid = app.Id,
                          stagetypeid = stg.StageTypeId,
                          serviceid = srv.Id,
                          //  servicename = tr.Value,
                          fee = srv.Fee,
                          templateid = app.TemplateId,
                          appstartdate = app.CreatedDate,
                          //stagetypename = (stagetypetranslation.Where(x => x.Id == stg.StageTypeId).Select(y => y.Value).FirstOrDefault())
                      });

                //we need to get All name of template for All applications 
                //if template id ==null then template name is null by left join in query
                var templateQuery = (from q1 in AppStageService
                                     join tm in _EngineCoreDBContext.Template on q1.templateid equals tm.Id //into g
                                                                                                            //  from q in g.DefaultIfEmpty()
                                                                                                            //join tr in _EngineCoreDBContext.SysTranslation
                                                                                                            //on tm.TitleShortcut equals tr.Shortcut
                                                                                                            //where tr.Lang == lang
                                     select new AllapplicationDto
                                     {
                                         LastDateReader = q1.LastDateReader,
                                         applicationNo = q1.applicationNo,
                                         percent = q1.percent,
                                         lastUpdateStage = q1.lastUpdateStage,
                                         periodLate = q1.periodLate,
                                         appStatusId = q1.appStatusId,
                                         appid = q1.appid,
                                         serviceid = q1.serviceid,
                                         servicename = q1.servicename,
                                         fee = q1.fee,
                                         templateid = q1.templateid,
                                         appstartdate = q1.appstartdate,
                                         // templatename = tr.Value,
                                         doctypeid = tm.DocumentTypeId,
                                         stagetypeid = q1.stagetypeid,
                                         stagetypename = q1.stagetypename

                                     });

                //now i need to get document name fro every app
                // if documentid==null then document name=null by left join in query
                var documentNameQuery = (
                       from qtl in templateQuery
                       join lv in _EngineCoreDBContext.SysLookupValue
                       on qtl.doctypeid equals lv.Id //into g
                                                     //from q in g.DefaultIfEmpty()
                                                     //    join tr in _EngineCoreDBContext.SysTranslation
                                                     //on lv.Shortcut equals tr.Shortcut
                                                     //        where tr.Lang == lang


                       select new AllapplicationDto
                       {
                           LastDateReader = qtl.LastDateReader,
                           applicationNo = qtl.applicationNo,
                           percent = qtl.percent,
                           lastUpdateStage = qtl.lastUpdateStage,
                           periodLate = qtl.periodLate,
                           appStatusId = qtl.appStatusId,
                           appid = qtl.appid,
                           serviceid = qtl.serviceid,
                           servicename = qtl.servicename,
                           fee = qtl.fee,
                           templateid = qtl.templateid,
                           appstartdate = qtl.appstartdate,
                           templatename = qtl.templatename,
                           doctypeid = qtl.doctypeid,
                           //  documentname = tr.Value,
                           stagetypeid = qtl.stagetypeid,
                           stagetypename = qtl.stagetypename,
                           //     UserName = qtl.appid > 0 ?//_IAdmServiceRepository.LastNotary((int)qtl.appid):null
                           // apptrak.FirstOrDefault(x => x.appid == qtl.appid).username : null
                           //_EngineCoreDBContext.User.Where(x => x.Id == apptrak.Where(y => y.appid == qtl.appid).Select(z => z.userid).FirstOrDefault()).Select(p => p.FullName).FirstOrDefault() : null : null

                       }
                      );


                //get applications status translation
                var Appstatustranslate = (
                    from q3 in documentNameQuery
                    join lv in _EngineCoreDBContext.SysLookupValue
                    on q3.appStatusId equals lv.Id
                    //join tr5 in _EngineCoreDBContext.SysTranslation
                    //on lv.Shortcut equals tr5.Shortcut
                    //where tr5.Lang == lang
                    select new AllapplicationDto
                    {
                        LastDateReader = q3.LastDateReader,
                        applicationNo = q3.applicationNo,
                        percent = q3.percent,
                        lastUpdateStage = q3.lastUpdateStage,
                        periodLate = q3.periodLate,
                        //  appStatusName = tr5.Value,
                        appStatusId = q3.appStatusId,
                        appid = q3.appid,
                        serviceid = q3.serviceid,
                        servicename = q3.servicename,
                        fee = q3.fee,
                        templateid = q3.templateid,
                        appstartdate = q3.appstartdate,
                        templatename = q3.templatename,
                        doctypeid = q3.doctypeid,
                        documentname = q3.documentname,
                        stagetypeid = q3.stagetypeid,
                        stagetypename = q3.stagetypename,
                        UserName = q3.UserName

                    });

                //get all parties with there emirates id and extra attachment if found or not
                var extraquery = (from par in _EngineCoreDBContext.ApplicationParty
                                  join ex in _EngineCoreDBContext.ApplicationPartyExtraAttachment on par.Id equals ex.ApplicationPartyId into g
                                  from ex in g.DefaultIfEmpty()
                                  select new
                                  {
                                      id = par.Id,
                                      trancid = par.TransactionId,
                                      partyfullname = par.FullName,
                                      partyemail = par.Email,
                                      partyphone = par.Mobile,
                                      partyemirateid = par.EmiratesIdNo,
                                      partydocid = ex.Number

                                  });

                //we need to know owner of All applications
                var isownerQuery = (
                    from par in _EngineCoreDBContext.ApplicationParty//.Where(x=>x.TransactionId.ToString().Contains(searchDto.TransactionID)|| searchDto.TransactionID==null)
                    where par.IsOwner == true
                    select new
                    {
                        trancId = par.TransactionId,
                        fullname = par.FullName,
                        email = par.Email,
                        mobile = par.Mobile,

                    });

                var allparty = (
                    from exq in extraquery
                    join iso in isownerQuery on exq.trancid equals iso.trancId into g

                    from iso in g.DefaultIfEmpty()
                    select new
                    {
                        trancid = exq.trancid,
                        partyfullname = exq.partyfullname,
                        partyemail = exq.partyemail,
                        partyphone = exq.partyphone,
                        partyemirateid = exq.partyemirateid,
                        partyexdocid = exq.partydocid,
                        ownername = iso.fullname,
                        owneremail = iso.email,
                        ownerphone = iso.mobile,

                    }
                    );

                // get transaction for owner applications
                var userTransactionsQuery = (
                     from q in allparty
                     join tranc in _EngineCoreDBContext.AppTransaction
                     on q.trancid equals tranc.Id
                     //join app in AppStageService
                     //  on tranc.ApplicationId equals app.appid// into g

                     //from app in g.DefaultIfEmpty()
                     join oldtranc in _EngineCoreDBContext.TransactionOldVersion on tranc.Id equals oldtranc.TransactionId into g



                     from oldtranc in g.DefaultIfEmpty()

                     select new AppPartyDto
                     {
                         oldtransactionno = oldtranc.TransactionNo,
                         trancID = tranc.Id,
                         TransactionId = tranc.TransactionNo,
                         appid = tranc.ApplicationId,
                         fullname = q.ownername,
                         email = q.owneremail,
                         mobile = q.ownerphone,
                         extradocid = q.partyexdocid,
                         paremirateid = q.partyemirateid,
                         paremail = q.partyemail,
                         parfullname = q.partyfullname,
                         parmobile = q.partyphone
                     });//.ToListAsync();

                //var olduserTransactionsQuery = (
                //   from  tranc in userTransactionsQuery
                //     join oldtranc  in _EngineCoreDBContext.TransactionOldVersion on tranc.trancID equals oldtranc.TransactionId into g



                //   from oldtranc in g.DefaultIfEmpty()


                //   select new AppPartyDto
                //   {
                //       oldtransactionno=oldtranc.TransactionNo,
                //       TransactionId = tranc.TransactionId,
                //       appid = tranc.appid,
                //       fullname = tranc.fullname,
                //       email = tranc.email,
                //       mobile = tranc.mobile,
                //       extradocid = tranc.extradocid,
                //       paremirateid = tranc.paremirateid,
                //       paremail = tranc.paremail,
                //       parfullname = tranc.parfullname,
                //       parmobile = tranc.parmobile
                //   });//.ToListAsync();

                //apply search for All Applications
                var searchQuery = (
                    from q4 in userTransactionsQuery //olduserTransactionsQuery
                    join q5 in Appstatustranslate on q4.appid equals q5.appid

                    //orderby q5.appstartdate

                    where //((q4.appid.ToString().Contains(searchDto.Id) || searchDto.Id == null) || (q5.applicationNo.Contains(searchDto.Id) || searchDto.Id == null)) &&
                              ((searchDto.sDate == null || (q5.lastUpdateStage != null && q5.lastUpdateStage >= Convert.ToDateTime(searchDto.sDate))) &&
                    (searchDto.eDate == null || (q5.lastUpdateStage != null && q5.lastUpdateStage <= Convert.ToDateTime(searchDto.eDate).AddDays(1)))) &&
                           (q4.parfullname.Contains(searchDto.Name) || searchDto.Name == null) &&
                           (q4.parmobile.Contains(searchDto.Phone) || searchDto.Phone == null) &&
                           (q4.paremail.Contains(searchDto.Email) || searchDto.Email == null) &&
                           // (q5.stagetypeid.ToString().Contains(searchDto.stagetypeid) || searchDto.stagetypeid == null) &&
                           (q4.paremirateid.Contains(searchDto.EmirateId) || q4.extradocid.Contains(searchDto.EmirateId) || searchDto.EmirateId == null) &&
                            (q4.TransactionId.Contains(searchDto.TransactionID) || q4.oldtransactionno.Contains(searchDto.TransactionID) || searchDto.TransactionID == null)
                    select new Apps
                    {

                        percent = q5.percent,
                        lastUpdateStage = q5.lastUpdateStage,
                        periodLate = q5.periodLate,
                        appStatusName = q5.appStatusName,
                        appStatusId = q5.appStatusId,
                        appid = q4.appid,
                        serviceid = q5.serviceid,
                        servicename = services[(int)q5.serviceid],
                        fee = q5.fee,
                        appstartdate = q5.appstartdate,
                        templateid = q5.templateid,
                        templatename = templates[(int)q5.templateid],
                        doctypeid = q5.doctypeid,
                        documentname = documents[(int)q5.doctypeid],
                        fullname = q4.fullname,
                        email = q4.email,
                        mobile = q4.mobile,
                        stagetypeid = q5.stagetypeid,
                        stagetypename = stages[(int)q5.stagetypeid],
                        UserName = q5.UserName,
                        // islate = ((q5.stagetypeid == DoneStageTypeId) || (q5.appStatusId == RejectedStateId) || (q5.stagetypeid == draftId)) ? false : workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result,
                        //   islocked = q4.appid > 0 ? _iApplicationRepository.IfBookedUp((int)q4.appid, userid, lang).Result.Id == -1 : false
                        islocked = q4.appid > 0 ? DateTime.Now.Subtract((DateTime)q5.LastDateReader).TotalSeconds < Constants.LOCK_SECONDS_TIME ? true : false : false


                        // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                    }
                    ).ToList();


                //var searchLateQuery = (from q5 in searchQuery

                //                           //orderby q5.appstartdate

                //                       where (q5.islate)
                //                       select new Apps
                //                       {
                //                           percent = q5.percent,
                //                           lastUpdateStage = q5.lastUpdateStage,
                //                           periodLate = q5.periodLate,
                //                           appStatusName = q5.appStatusName,
                //                           appStatusId = q5.appStatusId,
                //                           appid = q5.appid,
                //                           serviceid = q5.serviceid,
                //                           servicename = q5.servicename,
                //                           fee = q5.fee,
                //                           appstartdate = q5.appstartdate,
                //                           templateid = q5.templateid,
                //                           templatename = q5.templatename,
                //                           doctypeid = q5.doctypeid,
                //                           documentname = q5.documentname,
                //                           fullname = q5.fullname,
                //                           email = q5.email,
                //                           mobile = q5.mobile,
                //                           UserName = q5.UserName,
                //                           stagetypeid = q5.stagetypeid,
                //                           islate = q5.islate,//workingTimeRepository.IsOrderLate(q5.lastUpdateStage, DateTime.Now, q5.periodLate * 60).Result
                //                           islocked = q5.islocked
                //                           // (q4.StageTypeName == "منجزة" || q10.StageTypeName == "Done" ? false :
                //                       }
                //   );

                //if (searchDto.isLate)
                //{

                //    applicationCountDto.Applications = searchLateQuery.GroupBy(z => z.appid).Select(p => p.FirstOrDefault()).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();
                //    int count = applicationCountDto.Applications.Count();// if islate click  on card select this
                //    applicationCountDto.count = count;
                //}
                //else
                //{
                applicationCountDto.Applications = searchQuery.GroupBy(z => z.appid).Select(p => p.FirstOrDefault()).OrderByDescending(x => x.appstartdate).Skip(perpage * (currentpage - 1)).Take(perpage).ToList();
                int count = searchQuery.GroupBy(z => z.appid).Select(p => p.FirstOrDefault()).Count();
                applicationCountDto.count = count;
                // }





                return applicationCountDto;
        }

       public async Task<int> getShortCutStageByTranslate(string lang,string value)
        {
            var Translate = (from tr in _EngineCoreDBContext.SysTranslation
                             where tr.Value.Contains(value) && tr.Lang == lang
                             select new
                             {
                                 ShortCut=tr.Shortcut,
                                 Value=tr.Value
                             });
            var result = (from trans in Translate
                          join sys in _EngineCoreDBContext.SysLookupValue
                          on trans.ShortCut equals sys.Shortcut
                          where sys.LookupTypeId == 2017
                          select new
                          {
                              shotId = sys.Id,

                          });
            return await result.Select(x => x.shotId).FirstOrDefaultAsync();

        }
        public async Task<object> addAppTrack()
        {
            List<Application> applications = await _EngineCoreDBContext.Application.ToListAsync();
            foreach (var Row in applications)
            {
                ApplicationTrack applicationTrack = new ApplicationTrack()
                {
                    ApplicationId = Row.Id,
                    UserId = 6067,
                    StageId = 23,
                    NextStageId = 7094,
                    CreatedDate = Row.CreatedDate
                };
                _IGeneralRepository.Add(applicationTrack);


                await _IGeneralRepository.Save();


            }
          
            return 1;
        }

        public string VerifyTransaction(VerifyDto verifyDto, string lang)
        {
            if (verifyDto.TransactionNo == "" || verifyDto.PayNo == "" || verifyDto.PayDate == "")
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "DataRequired"));
                throw _exception;
            }
            var query = (from pay in _EngineCoreDBContext.Payment.Where(x => x.InvoiceNo == verifyDto.PayNo)
                         join app in _EngineCoreDBContext.Application on pay.ApplicationId equals app.Id
                         join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                         where (DateTime.Compare(Convert.ToDateTime(verifyDto.PayDate), pay.PaymentDate.Value.Date) == 0) &&
                         (app.ApplicationNo==verifyDto.TransactionNo || tranc.TransactionNo==verifyDto.TransactionNo)
                         select new { TransactionURL = tranc.DocumentUrl }).ToList();//.FirstOrDefault().TransactionURL;
            if (query.Count() == 0)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "APPNF"));
                throw _exception;
            }

            return query.Select(x => x.TransactionURL).FirstOrDefault();
        }


        

    }
}
