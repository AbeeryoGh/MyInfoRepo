using EngineCoreProject.DTOs.Statistics;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.AdmServices;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.StatisticsService
{
    public class StatisticsRepository : IStatisticsRepository
    {
        
            private readonly  EngineCoreDBContext _EngineCoreDBContext;
            private readonly IGeneralRepository _iGeneralRepository;
            private readonly ISysValueRepository _ISysValueRepository;
            private readonly IAdmServiceRepository _IAdmServiceRepository;

        public StatisticsRepository(IAdmServiceRepository iAdmServiceRepository, ISysValueRepository iSysValueRepository, EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)

        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _ISysValueRepository = iSysValueRepository;
            _IAdmServiceRepository = iAdmServiceRepository;

        }

        public async  Task<StatisticsDto> countAll()
        {
            StatisticsDto statisticsDto = new StatisticsDto();
            string draftshorcut = _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stageDraftAR) || x.Value.ToLower()
                .Contains(Constants.stageDraftEN)).Select(y => y.Shortcut).FirstOrDefault();

            int draftId = _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == draftshorcut).Select(y => y.Id).FirstOrDefault();
            int DoneStageTypeId = await _ISysValueRepository.GetIdByShortcut("SysLookupValue_Shortcut677");


            var AllApps = (from apps in _EngineCoreDBContext.Application.Where(x => x.Id > 0)
                           join
stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId != draftId)
on apps.CurrentStageId equals stg.Id
                           join
tranc in _EngineCoreDBContext.AppTransaction.Where(x => x.Id > 0) on apps.Id equals tranc.ApplicationId
                           join par in _EngineCoreDBContext.ApplicationParty on tranc.Id equals par.TransactionId
                           select new
                           {
                               StageId=apps.CurrentStageId,
                               appid = apps.Id,
                               isowner = par.IsOwner,
                               trancid = tranc.Id,
                               signRequired = par.SignRequired,
                               signType = par.SignType
                               //  trancId=_iGeneralRepository.TypeofSign(tranc.Id)
                           }).ToList();
            statisticsDto.Allapps = AllApps.GroupBy(x => x.appid).Count();

            statisticsDto.services = (from srv in _EngineCoreDBContext.AdmService where srv.Icon != null select srv.Id).Count();

            statisticsDto.party = AllApps.Where(x => x.isowner == false).Count();

            statisticsDto.isowner = AllApps.Where(x => x.isowner == true).Count();

            statisticsDto.payments = (from pay in _EngineCoreDBContext.Payment.Where(x => x.ApplicationId > 0) select pay.Id).Count();

            List<int> e = await _IAdmServiceRepository.GetDoneStagesId();

            

            var countSign = from par in AllApps.Where(x =>x.signRequired == true && e.Contains((int)x.StageId ))
                             group par by par.trancid into t
                             select new
                             {
                                 trancid = t.Key,
                                 count = t.Where(z => z.signRequired == true).Count(),
                                 signsum = t.Where(z => z.signRequired == true).Sum(x => x.signType)
                             }
                                      ;


            statisticsDto.onlineSign = countSign.Where(x => x.signsum == x.count).Count();
            statisticsDto.offlineSign = countSign.Where(x => x.signsum == x.count * 2).Count();
            statisticsDto.mixerSign = countSign.Count() - statisticsDto.onlineSign - statisticsDto.offlineSign;// AllApps.Where(x => x.signsum != x.count).GroupBy(z => z.appid).Count();
            return statisticsDto;



        }

        public async Task<ChartsDto> AllCharts(string lang,searchChartsDto searchCharts)
        {
            int OnProgressStateId = await _ISysValueRepository.GetIdByShortcut("ONPROGRESS");
            int DoneStateId = await _ISysValueRepository.GetIdByShortcut("DONE");
            int AutoCancelId = await _ISysValueRepository.GetIdByShortcut("AutoCancel");
            int RejectedStateId = await _ISysValueRepository.GetIdByShortcut("REJECTED");
            int ReturnedStateId = await _ISysValueRepository.GetIdByShortcut("RETURNED");
            string draftshorcut = _EngineCoreDBContext.SysTranslation.Where(x => x.Value.Contains(Constants.stageDraftAR) || x.Value.ToLower()
                                                        .Contains(Constants.stageDraftEN)).Select(y => y.Shortcut).FirstOrDefault();
            int draftId = _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == draftshorcut).Select(y => y.Id).FirstOrDefault();
            int RejectedStageId = await _ISysValueRepository.GetIdByShortcut("REJECTEDSTAGE");
            string check = searchCharts.appStateId;
            if (searchCharts.serviceId == "999999")
                searchCharts.serviceId = null;
            if (searchCharts.appStateId == "999999")
                searchCharts.appStateId = null;
            if (searchCharts.appChannelId == "999999")
                searchCharts.appChannelId = null;
                        // if (searchCharts.appStateId == RejectedStageId.ToString())
                        //     {
                        //        searchCharts.appStateId = RejectedStateId.ToString();
                        //        searchCharts.stageTypeId = null;
                        //     }
                        //else if (searchCharts.appStateId == ReturnedStateId.ToString())
                        //     {
                        //        searchCharts.appStateId = ReturnedStateId.ToString();
                        //        searchCharts.stageTypeId = null;
                        //     }
                        //else if (searchCharts.appStateId == AutoCancelId.ToString())
                        //     {
                        //    searchCharts.appStateId = AutoCancelId.ToString();
                        //    searchCharts.stageTypeId = null;
                        //    }
                        //else
                        //{
                        //                    searchCharts.stageTypeId = searchCharts.appStateId;
                        //                    searchCharts.appStateId = null;
                        //}


            ChartsDto result = new ChartsDto();
            var Allservices = (
                from srv in _EngineCoreDBContext.AdmService
                join tr in _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == lang)
                on srv.Shortcut equals tr.Shortcut
                select new
                {
                    KhadamatiNo=srv.KhadamatiServiceNo,
                    serviceId = srv.Id,
                    servicName = tr.Value,
                    icon=srv.Icon

                }
                );
            var AllAppsService =searchCharts.dateKind=="date1"?( searchCharts.appStateId == RejectedStateId.ToString() ? ((from app in _EngineCoreDBContext.Application.Where(x=>x.Id>0)
                                  where (app.ServiceId.ToString().Contains(searchCharts.serviceId) || searchCharts.serviceId == null)&&
                                        ((searchCharts.sDate == null || app.ApplicationDate >= Convert.ToDateTime(searchCharts.sDate)) && 
                                        (searchCharts.eDate == null|| app.ApplicationDate <= Convert.ToDateTime(searchCharts.eDate).AddDays(1)))
                                         &&(app.StateId.ToString().Contains(searchCharts.appStateId)) && // || searchCharts.appStateId == null) &&
                                            (app.Channel.ToString().Contains(searchCharts.appChannelId) || searchCharts.appChannelId==null)
                                     join stg in _EngineCoreDBContext.AdmStage.Where(x=>x.StageTypeId!=draftId) on app.CurrentStageId equals stg.Id
                                   
                                    join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                                    join par in _EngineCoreDBContext.ApplicationParty.Where(x=>x.IsOwner==true) on tranc.Id equals par.TransactionId

                                  select new
                                        {
                                            appId=app.Id,
                                            serviceId=app.ServiceId,
                                            appStateId=app.StateId,
                                            appChannelId=app.Channel,
                                            currentStageId=app.CurrentStageId,
                                            stageTypeId=stg.StageTypeId,
                                            createdate=app.ApplicationDate,
                                            transactionid=par.TransactionId,
                                            userId=par.PartyId,
                                            emirateId=par.Emirate,
                                            fullname=par.FullName,
                                            phone=par.Mobile,
                                            birthdate=par.BirthDate,
                                            genderId=par.Gender,
                                            nationalityId=par.Nationality,
                                            ID=par.EmiratesIdNo,
                                            email=par.Email
                                        })): searchCharts.appStateId == ReturnedStateId.ToString()?((from app in _EngineCoreDBContext.Application.Where(x => x.Id > 0)
                                               where (app.ServiceId.ToString().Contains(searchCharts.serviceId) || searchCharts.serviceId == null) &&
                                                     ((searchCharts.sDate == null || app.ApplicationDate >= Convert.ToDateTime(searchCharts.sDate)) &&
                                                     (searchCharts.eDate == null || app.ApplicationDate <= Convert.ToDateTime(searchCharts.eDate).AddDays(1)))
                                                      && (app.StateId.ToString().Contains(searchCharts.appStateId)) &&
                                                         (app.Channel.ToString().Contains(searchCharts.appChannelId) || searchCharts.appChannelId == null)
                                               join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId == draftId) on app.CurrentStageId equals stg.Id
                                           
                                               join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                                               join par in _EngineCoreDBContext.ApplicationParty.Where(x => x.IsOwner == true) on tranc.Id equals par.TransactionId

                                               select new
                                               {
                                                   appId = app.Id,
                                                   serviceId = app.ServiceId,
                                                   appStateId = app.StateId,
                                                   appChannelId = app.Channel,
                                                   currentStageId = app.CurrentStageId,
                                                   stageTypeId = stg.StageTypeId,
                                                   createdate = app.ApplicationDate,
                                                   transactionid = par.TransactionId,
                                                   userId = par.PartyId,
                                                   emirateId = par.Emirate,
                                                   fullname = par.FullName,
                                                   phone = par.Mobile,
                                                   birthdate = par.BirthDate,
                                                   genderId = par.Gender,
                                                   nationalityId = par.Nationality,
                                                   ID = par.EmiratesIdNo,
                                                   email = par.Email
                                               })): searchCharts.appStateId == AutoCancelId.ToString() ? 
                                               ((from app in _EngineCoreDBContext.Application.Where(x => x.Id > 0)
                                                    where (app.ServiceId.ToString().Contains(searchCharts.serviceId) || searchCharts.serviceId == null) &&
                                                        ((searchCharts.sDate == null || app.ApplicationDate >= Convert.ToDateTime(searchCharts.sDate)) &&
                                                        (searchCharts.eDate == null || app.ApplicationDate <= Convert.ToDateTime(searchCharts.eDate).AddDays(1)))
                                                            && (app.StateId.ToString().Contains(searchCharts.appStateId)) &&
                                                            (app.Channel.ToString().Contains(searchCharts.appChannelId) || searchCharts.appChannelId == null)
                                                    join stg in _EngineCoreDBContext.AdmStage//.Where(x => x.StageTypeId == draftId)
                                                    on  app.CurrentStageId equals stg.Id
                                                //  where (stg.StageTypeId.ToString().Contains(searchCharts.stageTypeId) || searchCharts.stageTypeId == null)
                                                    join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                                                    join par in _EngineCoreDBContext.ApplicationParty.Where(x => x.IsOwner == true) on tranc.Id equals par.TransactionId

                                                    select new
                                                    {
                                                        appId = app.Id,
                                                        serviceId = app.ServiceId,
                                                        appStateId = app.StateId,
                                                        appChannelId = app.Channel,
                                                        currentStageId = app.CurrentStageId,
                                                        stageTypeId = stg.StageTypeId,
                                                        createdate = app.ApplicationDate,
                                                        transactionid = par.TransactionId,
                                                        userId = par.PartyId,
                                                        emirateId = par.Emirate,
                                                        fullname = par.FullName,
                                                        phone = par.Mobile,
                                                        birthdate = par.BirthDate,
                                                        genderId = par.Gender,
                                                        nationalityId = par.Nationality,
                                                        ID = par.EmiratesIdNo,
                                                        email = par.Email
                                                    })) : searchCharts.appStateId == OnProgressStateId.ToString() ? ((from app in _EngineCoreDBContext.Application.Where(x => x.Id > 0)
                                                        where (app.ServiceId.ToString().Contains(searchCharts.serviceId) || searchCharts.serviceId == null) &&
                                                                ((searchCharts.sDate == null || app.ApplicationDate >= Convert.ToDateTime(searchCharts.sDate)) &&
                                                                (searchCharts.eDate == null || app.ApplicationDate <= Convert.ToDateTime(searchCharts.eDate).AddDays(1)))
                                                                && (app.StateId.ToString().Contains(searchCharts.appStateId) ) &&
                                                                    (app.Channel.ToString().Contains(searchCharts.appChannelId) || searchCharts.appChannelId == null)
                                                        join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId != draftId)
                                                        on app.CurrentStageId equals stg.Id
                                                        // where (stg.StageTypeId.ToString().Contains(searchCharts.stageTypeId) || searchCharts.stageTypeId == null)
                                                        join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                                                        join par in _EngineCoreDBContext.ApplicationParty.Where(x => x.IsOwner == true) on tranc.Id equals par.TransactionId

                                                        select new
                                                        {
                                                            appId = app.Id,
                                                            serviceId = app.ServiceId,
                                                            appStateId = app.StateId,
                                                            appChannelId = app.Channel,
                                                            currentStageId = app.CurrentStageId,
                                                            stageTypeId = stg.StageTypeId,
                                                            createdate = app.ApplicationDate,
                                                            transactionid = par.TransactionId,
                                                            userId = par.PartyId,
                                                            emirateId = par.Emirate,
                                                            fullname = par.FullName,
                                                            phone = par.Mobile,
                                                            birthdate = par.BirthDate,
                                                            genderId = par.Gender,
                                                            nationalityId = par.Nationality,
                                                            ID = par.EmiratesIdNo,
                                                            email = par.Email
                                                        })) :
                                                         searchCharts.appStateId == DoneStateId.ToString() ? ((from app in _EngineCoreDBContext.Application.Where(x => x.Id > 0)
                                                            where (app.ServiceId.ToString().Contains(searchCharts.serviceId) || searchCharts.serviceId == null) &&
                                                                    ((searchCharts.sDate == null || app.ApplicationDate >= Convert.ToDateTime(searchCharts.sDate)) &&
                                                                    (searchCharts.eDate == null || app.ApplicationDate <= Convert.ToDateTime(searchCharts.eDate).AddDays(1)))
                                                                    && (app.StateId.ToString().Contains(searchCharts.appStateId)) &&
                                                                        (app.Channel.ToString().Contains(searchCharts.appChannelId) || searchCharts.appChannelId == null)
                                                            join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId != draftId)
                                                            on app.CurrentStageId equals stg.Id
                                                            // where (stg.StageTypeId.ToString().Contains(searchCharts.stageTypeId) || searchCharts.stageTypeId == null)
                                                            join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                                                            join par in _EngineCoreDBContext.ApplicationParty.Where(x => x.IsOwner == true) on tranc.Id equals par.TransactionId

                                                            select new
                                                            {
                                                                appId = app.Id,
                                                                serviceId = app.ServiceId,
                                                                appStateId = app.StateId,
                                                                appChannelId = app.Channel,
                                                                currentStageId = app.CurrentStageId,
                                                                stageTypeId = stg.StageTypeId,
                                                                createdate = app.ApplicationDate,
                                                                transactionid = par.TransactionId,
                                                                userId = par.PartyId,
                                                                emirateId = par.Emirate,
                                                                fullname = par.FullName,
                                                                phone = par.Mobile,
                                                                birthdate = par.BirthDate,
                                                                genderId = par.Gender,
                                                                nationalityId = par.Nationality,
                                                                ID = par.EmiratesIdNo,
                                                                email = par.Email
                                                            })) : ((from app in _EngineCoreDBContext.Application.Where(x => x.Id > 0)
                                                      where (app.ServiceId.ToString().Contains(searchCharts.serviceId) || searchCharts.serviceId == null) &&
                                                            ((searchCharts.sDate == null || app.ApplicationDate >= Convert.ToDateTime(searchCharts.sDate)) &&
                                                            (searchCharts.eDate == null || app.ApplicationDate <= Convert.ToDateTime(searchCharts.eDate).AddDays(1)))
                                                             && (app.StateId.ToString().Contains(searchCharts.appStateId) || searchCharts.appStateId == null) &&
                                                                (app.Channel.ToString().Contains(searchCharts.appChannelId) || searchCharts.appChannelId == null)
                                                      join stg in _EngineCoreDBContext.AdmStage//.Where(x => x.StageTypeId != draftId) 
                                                      on app.CurrentStageId equals stg.Id
                                                      where app.StateId!=9112
                                                     // where (stg.StageTypeId.ToString().Contains(searchCharts.stageTypeId) || searchCharts.stageTypeId == null)
                                                      join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                                                      join par in _EngineCoreDBContext.ApplicationParty.Where(x => x.IsOwner == true) on tranc.Id equals par.TransactionId

                                                      select new
                                                      {
                                                          appId = app.Id,
                                                          serviceId = app.ServiceId,
                                                          appStateId = app.StateId,
                                                          appChannelId = app.Channel,
                                                          currentStageId = app.CurrentStageId,
                                                          stageTypeId = stg.StageTypeId,
                                                          createdate = app.ApplicationDate,
                                                          transactionid = par.TransactionId,
                                                          userId = par.PartyId,
                                                          emirateId = par.Emirate,
                                                          fullname = par.FullName,
                                                          phone = par.Mobile,
                                                          birthdate = par.BirthDate,
                                                          genderId = par.Gender,
                                                          nationalityId = par.Nationality,
                                                          ID = par.EmiratesIdNo,
                                                          email = par.Email
                                                      }))): (searchCharts.appStateId == RejectedStateId.ToString() ? ((from app in _EngineCoreDBContext.Application.Where(x => x.Id > 0)
                                                            where (app.ServiceId.ToString().Contains(searchCharts.serviceId) || searchCharts.serviceId == null) &&
                                                                    ((searchCharts.sDate == null || app.AppLastUpdateDate >= Convert.ToDateTime(searchCharts.sDate)) &&
                                                                    (searchCharts.eDate == null || app.AppLastUpdateDate <= Convert.ToDateTime(searchCharts.eDate).AddDays(1)))
                                                                    && (app.StateId.ToString().Contains(searchCharts.appStateId))&&// || searchCharts.appStateId == null) &&
                                                                        (app.Channel.ToString().Contains(searchCharts.appChannelId) || searchCharts.appChannelId == null)
                                                            join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId != draftId) on app.CurrentStageId equals stg.Id

                                                            join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                                                            join par in _EngineCoreDBContext.ApplicationParty.Where(x => x.IsOwner == true) on tranc.Id equals par.TransactionId

                                                            select new
                                                            {
                                                                appId = app.Id,
                                                                serviceId = app.ServiceId,
                                                                appStateId = app.StateId,
                                                                appChannelId = app.Channel,
                                                                currentStageId = app.CurrentStageId,
                                                                stageTypeId = stg.StageTypeId,
                                                                createdate = app.ApplicationDate == null ? app.CreatedDate : app.ApplicationDate,
                                                                transactionid = par.TransactionId,
                                                                userId = par.PartyId,
                                                                emirateId = par.Emirate,
                                                                fullname = par.FullName,
                                                                phone = par.Mobile,
                                                                birthdate = par.BirthDate,
                                                                genderId = par.Gender,
                                                                nationalityId = par.Nationality,
                                                                ID = par.EmiratesIdNo,
                                                                email = par.Email
                                                            })) : searchCharts.appStateId == ReturnedStateId.ToString() ? ((from app in _EngineCoreDBContext.Application.Where(x => x.Id > 0)
                                                                where (app.ServiceId.ToString().Contains(searchCharts.serviceId) || searchCharts.serviceId == null) &&
                                                                        ((searchCharts.sDate == null || app.AppLastUpdateDate >= Convert.ToDateTime(searchCharts.sDate)) &&
                                                                        (searchCharts.eDate == null || app.AppLastUpdateDate <= Convert.ToDateTime(searchCharts.eDate).AddDays(1)))
                                                                        && (app.StateId.ToString().Contains(searchCharts.appStateId)) &&
                                                                            (app.Channel.ToString().Contains(searchCharts.appChannelId) || searchCharts.appChannelId == null)
                                                                join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId == draftId) on app.CurrentStageId equals stg.Id

                                                                join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                                                                join par in _EngineCoreDBContext.ApplicationParty.Where(x => x.IsOwner == true) on tranc.Id equals par.TransactionId

                                                                select new
                                                                {
                                                                    appId = app.Id,
                                                                    serviceId = app.ServiceId,
                                                                    appStateId = app.StateId,
                                                                    appChannelId = app.Channel,
                                                                    currentStageId = app.CurrentStageId,
                                                                    stageTypeId = stg.StageTypeId,
                                                                    createdate = app.ApplicationDate == null ? app.CreatedDate : app.ApplicationDate,
                                                                    transactionid = par.TransactionId,
                                                                    userId = par.PartyId,
                                                                    emirateId = par.Emirate,
                                                                    fullname = par.FullName,
                                                                    phone = par.Mobile,
                                                                    birthdate = par.BirthDate,
                                                                    genderId = par.Gender,
                                                                    nationalityId = par.Nationality,
                                                                    ID = par.EmiratesIdNo,
                                                                    email = par.Email
                                                                })) : searchCharts.appStateId == AutoCancelId.ToString() ? ((from app in _EngineCoreDBContext.Application.Where(x => x.Id > 0)
                                                                where (app.ServiceId.ToString().Contains(searchCharts.serviceId) || searchCharts.serviceId == null) &&
                                                                        ((searchCharts.sDate == null || app.AppLastUpdateDate >= Convert.ToDateTime(searchCharts.sDate)) &&
                                                                        (searchCharts.eDate == null || app.AppLastUpdateDate <= Convert.ToDateTime(searchCharts.eDate).AddDays(1)))
                                                                        && (app.StateId.ToString().Contains(searchCharts.appStateId)) &&
                                                                            (app.Channel.ToString().Contains(searchCharts.appChannelId) || searchCharts.appChannelId == null)
                                                                join stg in _EngineCoreDBContext.AdmStage//.Where(x => x.StageTypeId == draftId)
                                                                on app.CurrentStageId equals stg.Id
                                                                //  where (stg.StageTypeId.ToString().Contains(searchCharts.stageTypeId) || searchCharts.stageTypeId == null)
                                                                join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                                                                join par in _EngineCoreDBContext.ApplicationParty.Where(x => x.IsOwner == true) on tranc.Id equals par.TransactionId

                                                                select new
                                                                {
                                                                    appId = app.Id,
                                                                    serviceId = app.ServiceId,
                                                                    appStateId = app.StateId,
                                                                    appChannelId = app.Channel,
                                                                    currentStageId = app.CurrentStageId,
                                                                    stageTypeId = stg.StageTypeId,
                                                                    createdate = app.ApplicationDate == null ? app.CreatedDate : app.ApplicationDate,
                                                                    transactionid = par.TransactionId,
                                                                    userId = par.PartyId,
                                                                    emirateId = par.Emirate,
                                                                    fullname = par.FullName,
                                                                    phone = par.Mobile,
                                                                    birthdate = par.BirthDate,
                                                                    genderId = par.Gender,
                                                                    nationalityId = par.Nationality,
                                                                    ID = par.EmiratesIdNo,
                                                                    email = par.Email
                                                                })) : searchCharts.appStateId == OnProgressStateId.ToString() ? ((from app in _EngineCoreDBContext.Application.Where(x => x.Id > 0)
                                                                  where (app.ServiceId.ToString().Contains(searchCharts.serviceId) || searchCharts.serviceId == null) &&
                                                                ((searchCharts.sDate == null || app.AppLastUpdateDate >= Convert.ToDateTime(searchCharts.sDate)) &&
                                                                (searchCharts.eDate == null || app.AppLastUpdateDate <= Convert.ToDateTime(searchCharts.eDate).AddDays(1)))
                                                                    && (app.StateId.ToString().Contains(searchCharts.appStateId)) &&
                                                                    (app.Channel.ToString().Contains(searchCharts.appChannelId) || searchCharts.appChannelId == null)
                                                            join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId != draftId)
                                                            on app.CurrentStageId equals stg.Id
                                                            // where (stg.StageTypeId.ToString().Contains(searchCharts.stageTypeId) || searchCharts.stageTypeId == null)
                                                            join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                                                            join par in _EngineCoreDBContext.ApplicationParty.Where(x => x.IsOwner == true) on tranc.Id equals par.TransactionId

                                                            select new
                                                            {
                                                                appId = app.Id,
                                                                serviceId = app.ServiceId,
                                                                appStateId = app.StateId,
                                                                appChannelId = app.Channel,
                                                                currentStageId = app.CurrentStageId,
                                                                stageTypeId = stg.StageTypeId,
                                                                createdate = app.ApplicationDate == null ? app.CreatedDate : app.ApplicationDate,
                                                                transactionid = par.TransactionId,
                                                                userId = par.PartyId,
                                                                emirateId = par.Emirate,
                                                                fullname = par.FullName,
                                                                phone = par.Mobile,
                                                                birthdate = par.BirthDate,
                                                                genderId = par.Gender,
                                                                nationalityId = par.Nationality,
                                                                ID = par.EmiratesIdNo,
                                                                email = par.Email
                                                            })) : searchCharts.appStateId == DoneStateId.ToString() ? ((from app in _EngineCoreDBContext.Application.Where(x => x.Id > 0)
                                                                where (app.ServiceId.ToString().Contains(searchCharts.serviceId) || searchCharts.serviceId == null) &&
                                                            ((searchCharts.sDate == null || app.AppLastUpdateDate >= Convert.ToDateTime(searchCharts.sDate)) &&
                                                            (searchCharts.eDate == null || app.AppLastUpdateDate <= Convert.ToDateTime(searchCharts.eDate).AddDays(1)))
                                                                && (app.StateId.ToString().Contains(searchCharts.appStateId)) &&
                                                                (app.Channel.ToString().Contains(searchCharts.appChannelId) || searchCharts.appChannelId == null)
                                                                join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId != draftId)
                                                                on app.CurrentStageId equals stg.Id
                                                                // where (stg.StageTypeId.ToString().Contains(searchCharts.stageTypeId) || searchCharts.stageTypeId == null)
                                                                join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                                                                join par in _EngineCoreDBContext.ApplicationParty.Where(x => x.IsOwner == true) on tranc.Id equals par.TransactionId

                                                                select new
                                                                {
                                                                    appId = app.Id,
                                                                    serviceId = app.ServiceId,
                                                                    appStateId = app.StateId,
                                                                    appChannelId = app.Channel,
                                                                    currentStageId = app.CurrentStageId,
                                                                    stageTypeId = stg.StageTypeId,
                                                                    createdate = app.ApplicationDate==null?app.CreatedDate : app.ApplicationDate,
                                                                    transactionid = par.TransactionId,
                                                                    userId = par.PartyId,
                                                                    emirateId = par.Emirate,
                                                                    fullname = par.FullName,
                                                                    phone = par.Mobile,
                                                                    birthdate = par.BirthDate,
                                                                    genderId = par.Gender,
                                                                    nationalityId = par.Nationality,
                                                                    ID = par.EmiratesIdNo,
                                                                    email = par.Email
                                                                })) : ((from app in _EngineCoreDBContext.Application.Where(x => x.Id > 0)
                                                                where (app.ServiceId.ToString().Contains(searchCharts.serviceId) || searchCharts.serviceId == null) &&
                                                                    ((searchCharts.sDate == null || app.AppLastUpdateDate >= Convert.ToDateTime(searchCharts.sDate)) &&
                                                                    (searchCharts.eDate == null || app.AppLastUpdateDate <= Convert.ToDateTime(searchCharts.eDate).AddDays(1)))
                                                                        && (app.StateId.ToString().Contains(searchCharts.appStateId) || searchCharts.appStateId == null) &&
                                                                        (app.Channel.ToString().Contains(searchCharts.appChannelId) || searchCharts.appChannelId == null)
                                                                join stg in _EngineCoreDBContext.AdmStage//.Where(x => x.StageTypeId != draftId) 
                                                                on app.CurrentStageId equals stg.Id
                                                                where app.StateId != 9112
                                                                // where (stg.StageTypeId.ToString().Contains(searchCharts.stageTypeId) || searchCharts.stageTypeId == null)
                                                                join tranc in _EngineCoreDBContext.AppTransaction on app.Id equals tranc.ApplicationId
                                                                join par in _EngineCoreDBContext.ApplicationParty.Where(x => x.IsOwner == true) on tranc.Id equals par.TransactionId

                                                                select new
                                                                {
                                                                    appId = app.Id,
                                                                    serviceId = app.ServiceId,
                                                                    appStateId = app.StateId,
                                                                    appChannelId = app.Channel,
                                                                    currentStageId = app.CurrentStageId,
                                                                    stageTypeId = stg.StageTypeId,
                                                                    createdate = app.ApplicationDate == null ? app.CreatedDate : app.ApplicationDate,
                                                                    transactionid = par.TransactionId,
                                                                    userId = par.PartyId,
                                                                    emirateId = par.Emirate,
                                                                    fullname = par.FullName,
                                                                    phone = par.Mobile,
                                                                    birthdate = par.BirthDate,
                                                                    genderId = par.Gender,
                                                                    nationalityId = par.Nationality,
                                                                    ID = par.EmiratesIdNo,
                                                                    email = par.Email
                                                                })));


            //var AllAppsServicebystatus = (from app in AllAppsService
            //                              where 
            //                            (app.appStateId.ToString().Contains(searchCharts.appStateId) || searchCharts.appStateId == null) 
            //                      select new
            //                      {
            //                          appId = app.appId,
            //                          serviceId = app.serviceId,
            //                          appStateId = app.appStateId
            //                      });

            //var AllAppsServicebyChannel = (from app in AllAppsService
            //                              where
            //                            (app.appChannelId.ToString().Contains(searchCharts.appChannelId) || searchCharts.appChannelId == null)
            //                              select new
            //                              {
            //                                  appId = app.appId,
            //                                  serviceId = app.serviceId,
            //                                  appStateId = app.appStateId,
            //                                  appChannelId=app.appChannelId
            //                              });



            ///////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////
SysLookupType channelLookupType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "Application_Channel").FirstOrDefault();
            int channellookupTypeId = channelLookupType.Id;
            //searchCharts.sDate = searchCharts.sDate == null ? "1990/01/01" : searchCharts.sDate;
            //searchCharts.eDate = searchCharts.eDate == null ? "2990/01/01" : searchCharts.eDate;

            SysLookupType stageType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "stage_type").FirstOrDefault();
            int lookupStageTypeId = stageType.Id;

            var stageTranslation = // get All stage translation 
                   (from sys_lookup_value in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == lookupStageTypeId && x.Id!=draftId)
                    join sys_translation in _EngineCoreDBContext.SysTranslation on sys_lookup_value.Shortcut equals sys_translation.Shortcut
                    where
                    sys_lookup_value.Id != RejectedStageId &&
                      sys_translation.Lang == lang
                     orderby sys_lookup_value.Id 
                    select new
                    {
                        appstage = sys_translation.Value,
                        appstageId = sys_lookup_value.Id
                    });


            var ResultInfo = (from app in AllAppsService
                                  //  join trac in _EngineCoreDBContext.AppTransaction on app.appId equals trac.ApplicationId
                                  // join par in _EngineCoreDBContext.ApplicationParty.Where(x => x.IsOwner == true) on trac.Id equals par.TransactionId
                              select new ServiceInfoDto
                              {
                                  serviceId = app.serviceId,
                                  ChannelId = (int)app.appChannelId,
                                  TransactionId = (int)app.transactionid,
                                  Userid = (int)app.userId,
                                  ApplicationNo = app.appId.ToString(), //application_party.Transaction.Application.Id.ToString(),
                                  ServiceName = _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut == _EngineCoreDBContext.AdmService.Where(z => z.Id == app.serviceId).Select(s => s.Shortcut).FirstOrDefault() && x.Lang == lang).Select(y => y.Value).FirstOrDefault(),
                                  channelName = (from tr in _EngineCoreDBContext.SysTranslation

                                                 join srv in _EngineCoreDBContext.SysLookupValue.Where(x => x.Id == app.appChannelId)
                                                on tr.Shortcut equals srv.Shortcut
                                                 where tr.Lang == lang
                                                 select new { statusName = tr.Value }).FirstOrDefault().statusName,
                                  ApplicationDate = app.createdate,
                                  Emirate = app.emirateId == 1 ? "ابوظبي" :
                                               (app.emirateId == 2 ? "دبي" : (app.emirateId == 3 ? "الشارقة" :
                                                (app.emirateId == 4 ? "رأس الخيمة" : (app.emirateId == 5 ? "الفجيرة" : app.emirateId == 6 ? "عجمان" :
                                                (app.emirateId == 7 ? "أم القيوين" : ""
                                                ))))),
                                  FullName = app.fullname,
                                  Email = app.email,
                                  PhoneNumber = app.phone,
                                  BirthDate = app.birthdate.ToString(),
                                  ID = app.ID,
                                  Gender = app.genderId == null ? null : (from tr in _EngineCoreDBContext.SysTranslation
                                                                          join srv in _EngineCoreDBContext.SysLookupValue.Where(x => x.Id == app.genderId)
                                                                         on tr.Shortcut equals srv.Shortcut
                                                                          where tr.Lang == lang
                                                                          select new { gender = tr.Value }).FirstOrDefault().gender,
                                  NationalityAr = app.nationalityId == null ? null : (from Country in _EngineCoreDBContext.Country
                                                                                      where Country.UgId == app.nationalityId
                                                                                      select new { CountryAr = Country.CntCountryAr }
                                                ).FirstOrDefault().CountryAr,
                                  //NationalityEn = par.Nationality == null ? null : (from Country in _EngineCoreDBContext.Country
                                  //                                                  where Country.UgId == par.Nationality
                                  //                                                  select new { CountryEn = Country.CntCountryEn }
                                  //              ).FirstOrDefault().CountryEn,
                                  stageName = app.appStateId==AutoCancelId?"ملغي تلقائيا": app.appStateId == ReturnedStateId ?"مرجعة": stageTranslation.Where(x => x.appstageId == app.stageTypeId).Select(y => y.appstage).FirstOrDefault() // _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut == application_party.Transaction.Application.CurrentStage.Shortcut && x.Lang == lang).Select(y => y.Value).FirstOrDefault()
                              }

                          ) ;//.ToList();//

            //List<ServiceInfoDto> ServiceInfoResult = Result.ToList();
            //.Where(x => (x.ApplicationDate >= Convert.ToDateTime(searchCharts.sDate) || searchCharts.sDate==null)
            //&& (x.ApplicationDate <= Convert.ToDateTime(searchCharts.eDate).AddDays(1) || searchCharts.eDate == null)).ToList();
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            var UserGroup = (from srv in ResultInfo
                             group srv by srv.FullName into t
                             select new
                             {
                                 userid = t.Key,
                                 trancId = t.Max(x => x.TransactionId),
                                 // emirateid=t.Key.ID,
                                 appcount = t.Count()
                             }
                            );
            var UserInfo = (from par in ResultInfo
                            join ug in UserGroup on new { colA = par.FullName, colB = (int)par.TransactionId } equals new { colA = ug.userid, colB = ug.trancId }
                            select new UserInfo
                            {
                                username = par.FullName,
                                nationality = par.NationalityAr,
                                mobile = par.PhoneNumber,
                                email = par.Email,
                                ID = par.ID,
                                BirthDate = par.BirthDate,
                                Gender = par.Gender,
                                appCount = ug.appcount

                            });

            ////////////////////////////////////////////////////////////////////////////////
            var groupAppServiceXls = (
                  from app in ResultInfo


                  group app by new { app.serviceId, app.ChannelId } into t
                  select new
                  {
                      countapp = t.Count(),
                      serviceid = t.Key.serviceId,
                      channelId = t.Key.ChannelId
                  }
                  ).ToList();
          
       


            var groupAppService = (
                from app in ResultInfo


                group app by  app.serviceId into t
                select new
                {
                    countapp = t.Count(),
                    serviceid = t.Key,
                    LastAppDate=t.Max(x=>x.ApplicationDate)
                }
                );

            var result1 = (from allsrv in Allservices
                         
                           join appsrv in groupAppService on allsrv.serviceId equals appsrv.serviceid
                            into g
                           from q in g.DefaultIfEmpty()
                          
                           orderby q.countapp descending
                           select new serviceCharts
                           {
                               KhadamatiNo=allsrv.KhadamatiNo,
                               ServiceId=allsrv.serviceId,
                               pv = 2400,
                               uv = q.countapp,
                               name = allsrv.servicName,
                               icon = allsrv.icon,
                               LastAppDate= q.LastAppDate==null?(DateTime?)null:(DateTime)q.LastAppDate
                         
                           }).ToList();

            var resultxls = (from rs in result1
                             select new
                             serviceCharts
                             {
                                 KhadamatiNo=rs.KhadamatiNo,
                                 ServiceId=rs.ServiceId,
                                 uv=rs.uv,
                                 name=rs.name,
                                 LastAppDate=rs.LastAppDate,
                                 CountMobile= groupAppServiceXls.Where(x=>x.channelId==11155 && x.serviceid==rs.ServiceId).Select(z=>z.countapp).FirstOrDefault(),
                                 CountWeb = groupAppServiceXls.Where(x => x.channelId == 11154 && x.serviceid == rs.ServiceId).Select(z => z.countapp).FirstOrDefault(),
                                 CountNotary= groupAppServiceXls.Where(x => x.channelId == 11156 && x.serviceid == rs.ServiceId).Select(z => z.countapp).FirstOrDefault()
                             });


            SysLookupType sysLookupType = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "application_state").FirstOrDefault();
            int lookupTypeId = sysLookupType.Id;

            var AppstatusTranslation = // get All stage translation 
                    (from sys_lookup_value in _EngineCoreDBContext.SysLookupValue.Where(x=>x.LookupTypeId==lookupTypeId)
                     join sys_translation in _EngineCoreDBContext.SysTranslation on sys_lookup_value.Shortcut equals sys_translation.Shortcut
                     where
                     (sys_lookup_value.Id==RejectedStateId || sys_lookup_value.Id==ReturnedStateId|| sys_lookup_value.Id == AutoCancelId
                     ||sys_lookup_value.Id==DoneStateId || sys_lookup_value.Id==OnProgressStateId) &&
                       sys_translation.Lang == lang
                     select new
                     {
                         appstatus=sys_translation.Value,
                         appstatusId=sys_lookup_value.Id
                     });

            var Appstatus = (
                from app in AllAppsService.Where(x=>x.appStateId==RejectedStateId || x.appStateId==ReturnedStateId || x.appStateId==AutoCancelId
                || x.appStateId==DoneStateId || x.appStateId==OnProgressStateId)//AllAppsServicebystatus
                group app by app.appStateId into t
                
                select new
                {
                    countapp = t.Count(),
                    stateId = t.Key
                }
                );
            var result2 = (from allsts in AppstatusTranslation
                           join appsts in Appstatus on allsts.appstatusId equals appsts.stateId into g
                       from q in g.DefaultIfEmpty()
                       select new appStatusCharts
                       {
                           appStateId = allsts.appstatusId,
                           Appcount = q.countapp,
                           statusName = allsts.appstatus
                       }).ToList();





            //var Appstages = (
            //    from app in AllAppsService.Where(x => x.appStateId != RejectedStateId && x.appStateId != ReturnedStateId)//AllAppsServicebystatus
            //    group app by app.stageTypeId into t

            //    select new
            //    {
            //        countapp = t.Count(),
            //        stateId = t.Key
            //    }
            //    );
            //var result22 = (from allsts in stageTranslation.Where(x=>x.==DoneStageTypeId)
            //                join appsts in Appstages on allsts.appstageId equals appsts.stateId into g
            //               from q in g.DefaultIfEmpty()
            //               select new appStatusCharts
            //               {
            //                   appStateId = allsts.appstageId,
            //                   Appcount = q.countapp,
            //                   statusName = allsts.appstage
            //               }).ToList();


            var result222 = (from res2 in result2
                             select new appStatusCharts
                             {
                                 appStateId = res2.appStateId,
                                 Appcount = res2.Appcount,
                                 statusName = res2.statusName
                             });
                             //.Union(from res22 in result22
                             //       select new appStatusCharts
                             //       {
                             //           appStateId = res22.appStateId,
                             //           Appcount = res22.Appcount,
                             //           statusName = res22.statusName
                             //       });
            var result2222 = (from res222 in result222
                              
                            select new appStatusCharts
                            {
                                appStateId = res222.appStateId,
                                Appcount = res222.Appcount,
                                statusName = res222.statusName
                            }).ToList();
            //////////////////////////////////////////////////////////////////////////////////////////////////////


            var AppChannelsTranslation = // get All stage translation 
                    (from sys_lookup_value in _EngineCoreDBContext.SysLookupValue
                     join sys_translation in _EngineCoreDBContext.SysTranslation on sys_lookup_value.Shortcut equals sys_translation.Shortcut
                     where
                       sys_lookup_value.LookupTypeId == channellookupTypeId &&
                       sys_translation.Lang == lang
                     select new
                     {
                         appChannelName = sys_translation.Value,
                         appChannelId = sys_lookup_value.Id
                     });

            var AppChannels = (
                from app in AllAppsService//AllAppsServicebyChannel
                group app by app.appChannelId into t
                select new
                {
                    countapp = t.Count(),
                    channelId = t.Key
                }
                );
            var result3 = (from allsts in AppChannelsTranslation
                           join appsts in AppChannels on allsts.appChannelId equals appsts.channelId into g
                           from q in g.DefaultIfEmpty()
                           select new appChannelsCharts
                           {
                               appChannelId = allsts.appChannelId,
                               Appcount = q.countapp,
                               channelName = allsts.appChannelName
                           }).ToList();
            ////////////////////////////////////////////////////////////////////////////////
            result.countAppByservice = ResultInfo.Count();
            result.countAppByStatus = ResultInfo.Count();
            result.countAppByChannels = ResultInfo.Count();
            result.serviceCharts = result1;
            result.serviceChartsXls = resultxls.ToList();
            result.appStatusCharts =result2222;
            result.appChannelsCharts = result3;
            result.ServiceInfo = ResultInfo.ToList();
            result.Userinfo = UserInfo.ToList();

            return  result;
        }

      

        public async Task<object> NotaryAppsDone(searchChartsDto searchCharts)
        {
            var AppsDone =searchCharts.dateKind=="date1"? (from app in _EngineCoreDBContext.Application.Where(x=>x.Id>0 &&
                                        ((searchCharts.sDate == null || x.ApplicationDate >= Convert.ToDateTime(searchCharts.sDate)) &&
                                        (searchCharts.eDate == null || x.ApplicationDate <= Convert.ToDateTime(searchCharts.eDate).AddDays(1))))
                            join stg in _EngineCoreDBContext.AdmStage.Where(x=>x.StageTypeId==11139)
                            on app.CurrentStageId equals stg.Id
                            select new
                            {
                                appid=app.Id
                            }).ToList(): (from app in _EngineCoreDBContext.Application.Where(x => x.Id > 0 &&
                                         ((searchCharts.sDate == null || x.AppLastUpdateDate >= Convert.ToDateTime(searchCharts.sDate)) &&
                                         (searchCharts.eDate == null || x.AppLastUpdateDate <= Convert.ToDateTime(searchCharts.eDate).AddDays(1))))
                                          join stg in _EngineCoreDBContext.AdmStage.Where(x => x.StageTypeId == 11139)
                                          on app.CurrentStageId equals stg.Id
                                          select new
                                          {
                                              appid = app.Id
                                          }).ToList();

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


            var final = (
                from app in AppsDone
                join appt in apptrak on app.appid equals appt.appid
                select new
                {
                    appid = app.appid,
                    userid = appt.userid
                });

            var groupfinal = (from app in final
                              group app by app.userid into t
                              select new
                              {
                                  userid = t.Key,
                                  count = t.Count()
                              }
                            );

            var AllNotaryApps = (
                from gr in groupfinal
                join us in _EngineCoreDBContext.User on gr.userid equals us.Id
                select new
                {
                    userid = gr.userid,
                    username = us.FullName,
                    emirate=us.Address,
                    count = gr.count
                });
            return AllNotaryApps;
        }

      
    }
    }

