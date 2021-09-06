
using EngineCoreProject.DTOs.QueueDto;
using EngineCoreProject.DTOs.GeneralSettingDto;
using EngineCoreProject.IRepository.IQueueRepository;
using EngineCoreProject.Models;
using EngineCoreProject.Services.GeneralSetting;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using EngineCoreProject.IRepository.IGeneralSetting;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.IRepository.AdmServices;
using EngineCoreProject.Services.ApplicationSet;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.IRepository.IMeetingRepository;
using EngineCoreProject.Services.Meetings;
using EngineCoreProject.IRepository;
using Microsoft.Extensions.Logging;
using EngineCoreProject.IRepository.IBlackList;

namespace EngineCoreProject.Services.Queue
{
    public class QueueRepository : IQueueRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGlobalDayOffRepository _iGlobalDayOffRepository;
        private readonly IWorkingTimeRepository _iWorkingTimeRepository;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly IAdmServiceRepository _iAdmServiceRepository;
        private readonly ISysValueRepository _iSysValueRepository;
        private readonly IUserRepository _iUserRepository;
        private readonly ITransactionRepository _iTransactionRepository;
        private readonly IApplicationPartyRepository _iApplicationPartyRepository;
        private readonly IBlackListRepository _iBlackListRepository;
        private readonly ILogger<QueueRepository> _iLogger;
        ValidatorException _exception;

        public QueueRepository(EngineCoreDBContext EngineCoreDBContext, IGlobalDayOffRepository iGlobalDayOffRepository, IWorkingTimeRepository iWorkingTimeRepository, ILogger<QueueRepository> iLogger,
                               IUserRepository iUserRepository, ISysValueRepository iSysValueRepository, IAdmServiceRepository iAdmServiceRepository, IGeneralRepository iGeneralRepository, IApplicationPartyRepository iApplicationPartyRepository,
                               ITransactionRepository iTransactionRepository, IBlackListRepository iBlackListRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGlobalDayOffRepository = iGlobalDayOffRepository;
            _iWorkingTimeRepository = iWorkingTimeRepository;
            _iGeneralRepository = iGeneralRepository;
            _iAdmServiceRepository = iAdmServiceRepository;
            _iUserRepository = iUserRepository;
            _iSysValueRepository = iSysValueRepository;
            _iTransactionRepository = iTransactionRepository;
            _iApplicationPartyRepository = iApplicationPartyRepository;
            _iBlackListRepository = iBlackListRepository;
            _iLogger = iLogger;
            _exception = new ValidatorException();
        }
        public async Task<int> AddQueueProcess(QueuePostDto queueDto)
        {
            // TODO: add validations.
            QueueProcesses queueProcess = queueDto.GetEntity();
            queueProcess.Status = (int)Constants.PROCESS_STATUS.PENDING;
            _EngineCoreDBContext.QueueProcesses.Add(queueProcess);
            await _EngineCoreDBContext.SaveChangesAsync();
            return queueProcess.Id;
        }

        public async Task<int> UpdateQueueProcess(int rowId, QueuePostDto queueDto)
        {
            int res = 0;
            QueueProcesses originalQueue = await _EngineCoreDBContext.QueueProcesses.Where(a => a.Id == rowId).FirstOrDefaultAsync();

            if (originalQueue == null)
            {
                return res;
            }

            originalQueue.ExpectedDateTime = queueDto.ExpectedDate;
            originalQueue.ProcessNo = queueDto.ProcessNo;
            originalQueue.ServiceKindNo = queueDto.ServiceKindNo;
            originalQueue.TicketId = queueDto.TicketId;
            originalQueue.Status = queueDto.Status;
            originalQueue.Provider = queueDto.Provider;
            originalQueue.Note = queueDto.Note;
            originalQueue.CreatedBy = queueDto.CreatedBy;
            originalQueue.CreatedDate = queueDto.CreatedDate;
            originalQueue.LastUpdatedBy = queueDto.UpdatedBy;
            originalQueue.LastUpdatedDate = queueDto.UpdatedDate;
            originalQueue.Note = queueDto.Note;
            originalQueue.NotifyHighLevel = queueDto.NotifyHighLevel;
            originalQueue.NotifyLowLevel = queueDto.NotifyLowLevel;
            originalQueue.NotifyMediumLevel = queueDto.NotifyLowLevel;

            _EngineCoreDBContext.QueueProcesses.Update(originalQueue);
            await _EngineCoreDBContext.SaveChangesAsync();
            res = originalQueue.Id;
            return res;
        }

        public async Task<QueueGetDto> GetQueueProcess(string processNo, int serviceKindNo)
        {
            QueueProcesses createdQueue = new QueueProcesses();
            createdQueue = await _EngineCoreDBContext.QueueProcesses.Where(d => d.ProcessNo == processNo && d.ServiceKindNo == serviceKindNo).FirstOrDefaultAsync();
            if (createdQueue == null)
            {
                throw new InvalidOperationException("No process match!");
            }
            return QueueGetDto.GetDTO(createdQueue);
        }

        public async Task<List<QueueProcesses>> GetQueueForStatus(Constants.PROCESS_STATUS processStatus)
        {
            return await _EngineCoreDBContext.QueueProcesses.Where(d => d.Status == (byte)processStatus).ToListAsync();
        }

        public async Task<QueueTodayQueueInfo> GetCurrentQueueStatistics(bool onlyMyApp)
        {
            MeetingRepository meetingRepository = new MeetingRepository(_EngineCoreDBContext);
            var allTodayQueue = await _EngineCoreDBContext.QueueProcesses.ToListAsync();

            var pendingTickets = allTodayQueue.Where(x => x.Status == (byte)Constants.PROCESS_STATUS.PENDING).ToList();
            var pendingTicketsForMeeting = 0;

            List<QueueOnLineApp> onlineTickets = new List<QueueOnLineApp>();

            foreach (var pend in pendingTickets)
            {
                if (int.TryParse(pend.ProcessNo, out int n))
                {
                    var isAttended = await meetingRepository.IsAttendedByAppNo(n);
                    if (isAttended.IsOnline)
                    {
                        var app = await (
                        from ap in _EngineCoreDBContext.Application
                        join que in _EngineCoreDBContext.QueueProcesses
                        on ap.Id.ToString() equals que.ProcessNo
                        join usr in _EngineCoreDBContext.User
                        on ap.LastUpdatedBy equals usr.Id

                        where ap.Id == n
                        select new { UserName = usr.FullName, UserId = usr.Id }
                             ).FirstOrDefaultAsync();

                        pendingTicketsForMeeting++;
                        QueueOnLineApp queueOnLineApp = new QueueOnLineApp
                        {
                            ApplicationId = n,
                            TicketNo = pend.TicketId,

                            ExpectedDate = pend.ExpectedDateTime,

                            LastUpdateBy = app.UserName,
                            IsLate = isAttended.IsLate,
                            FirstLogin = isAttended.LastLogIn,
                            LastUpdateId = app.UserId
                        };

                        if (isAttended.IsLate)
                        {
                            onlineTickets.Add(queueOnLineApp);
                        }
                        else
                        {
                            if (onlyMyApp)
                            {
                                if (queueOnLineApp.LastUpdateId == _iUserRepository.GetUserID())
                                {
                                    onlineTickets.Add(queueOnLineApp);
                                }
                            }
                            else
                            {
                                onlineTickets.Add(queueOnLineApp);
                            }
                        }

                    }
                }
            }

            QueueTodayQueueInfo todayProcesses = new QueueTodayQueueInfo()
            {
                AllTicketsCount = allTodayQueue.Count,
                PendingOnLineTicketsCount = pendingTicketsForMeeting,
                PerformedTicketsCount = allTodayQueue.Where(x => x.Status == (byte)Constants.PROCESS_STATUS.FINISHED).Count(),
                InProgressTicketsCount = allTodayQueue.Where(x => x.Status == (byte)Constants.PROCESS_STATUS.INPROGRESS).Count(),
                LastTicketInProgress = allTodayQueue.Where(x => x.Status == (byte)Constants.PROCESS_STATUS.INPROGRESS).OrderByDescending(x => x.TicketId).Select(x => x.TicketId).FirstOrDefault(),
                OnLineTickets = onlineTickets
            };
            return todayProcesses;
        }


        public async Task<QueueNextAppDto> GetNextOrder(int askedById)
        {
            var blackListApps = await _iBlackListRepository.GetBlackListApplications();

            MeetingRepository meetingRepository = new MeetingRepository(_EngineCoreDBContext);
            await _iUserRepository.StartStopWork(true);
            QueueNextAppDto nextApp = new QueueNextAppDto();

            int RejectedStateId = await _iSysValueRepository.GetIdByShortcut("REJECTED");
            int AutoRejectedStateId = await _iSysValueRepository.GetIdByShortcut("AutoCancel");
            var interviewOrderStages = await _iAdmServiceRepository.GetInterviewStagesId();

            var interviewApps = await _EngineCoreDBContext.Application.Where(x => x.StateId != RejectedStateId && 
                                                                                  x.StateId != AutoRejectedStateId && 
                                                                                  interviewOrderStages.Contains((int)x.CurrentStageId) &&
                                                                                  !blackListApps.Contains(x.Id)
                                                                                  ).OrderBy(x => x.CreatedDate).ToListAsync();

            List<QueueNextAppDto> lateOrders = new List<QueueNextAppDto>();

            foreach (var pend in interviewApps)
            {
                var isAttended = await meetingRepository.IsAttendedByAppNo(pend.Id);
                if (isAttended.IsOnline)
                {

                    var appTran = await _iTransactionRepository.GetAll(pend.Id);
                    if (appTran.Count < 1)
                    {
                        _iLogger.LogInformation("GetNextOrder fault configuration for application at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " !!!!!!!! no transaction for application " + pend.Id.ToString());
                        continue;
                    }


                    bool locked = true;
                    try
                    {
                        var signResult = await _iApplicationPartyRepository.IsSignEditByAnotherUser((int)appTran[0].Id, askedById);
                        locked = signResult.Result == Constants.AppStatus.LOCKED;
                    }
                    catch (Exception ex)
                    {
                        _iLogger.LogInformation("GetNextOrder fault configuration for locked at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " !!!!!!!! in IsSignEditByAnotherUser, error is " + ex.Message);
                        continue;
                    }


                    bool release = (DateTime.Now.Subtract((DateTime)pend.LastReadDate).TotalSeconds > Constants.LOCK_SECONDS_TIME) && (!locked);

                    // Currently is open by me         new App,                      done by me and free
                    if (pend.LastReader == askedById || pend.LastReadDate == null || (pend.LastUpdatedBy == askedById && release))
                    {
                        nextApp.ApplicationId = pend.Id;
                        nextApp.ServiceId = (int)pend.ServiceId;

                        // start process.
                        await ChangeTicketsStatusToInProgressByProcessNo(askedById, pend.Id.ToString());                      
                        _iLogger.LogInformation("GetNextOrder is requested at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " returned online application for me  is " + nextApp.ApplicationId.ToString());
                        return nextApp;
                    }

                    if (isAttended.IsLate && release)
                    {
                        QueueNextAppDto nextApp1 = new QueueNextAppDto
                        {
                            ApplicationId = pend.Id,
                            ServiceId = (int)pend.ServiceId
                        };
                        lateOrders.Add(nextApp1);
                    }
                    else if (isAttended.IsLate && locked)
                    {
                        _iLogger.LogInformation(" Warning!!: GetNextOrder is requested at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " for the application " + pend.Id.ToString() + " it is late but it is not return because it is locked contains signatures. "  );
                    }
                }
            }

            // no online for me in interview, check if online and late not to me.
            if (lateOrders.Count > 0)
            {
                // start process.
                await ChangeTicketsStatusToInProgressByProcessNo(askedById, lateOrders[0].ApplicationId.ToString());
                _iLogger.LogInformation("GetNextOrder is requested at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " returned online application for other is " + lateOrders[0].ApplicationId.ToString());
                return lateOrders[0];
            }

            
            try
            {
                // no Application in interview sage get from reviewer stage.
                var reviewedStages = await _iAdmServiceRepository.GetReviewStagesId();

                if (reviewedStages.Count == 0)
                {
                    _iLogger.LogInformation("GetNextOrder is wrong configuration at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " reviedStages is empty ");
                }

                var reviewApps = await _EngineCoreDBContext.Application.Where(x => x.StateId != RejectedStateId && 
                                                                              reviewedStages.Contains((int)x.CurrentStageId) &&
                                                                              !blackListApps.Contains(x.Id)
                                                                              ).OrderBy(x => x.CreatedDate).ToListAsync();
                foreach (var reviewApp in reviewApps)
                {
                    if (reviewApp.LastReader == askedById || reviewApp.LastReadDate == null || (DateTime.Now.Subtract((DateTime)reviewApp.LastReadDate).TotalSeconds > Constants.LOCK_SECONDS_TIME))
                    {
                        nextApp.ApplicationId = reviewApp.Id;
                        nextApp.ServiceId = (int)reviewApp.ServiceId;
                        _iLogger.LogInformation("GetNextOrder is requested at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " returned application for review is " + nextApp.ApplicationId.ToString());

                        return nextApp;
                    }
                }

                _iLogger.LogInformation("GetNextOrder is Not find any result !!! at " + DateTime.Now.ToString() + " by user " + _iUserRepository.GetUserName() + " application count in review is " + reviewApps.Count.ToString());

            }
            catch (Exception ex)
            {
                _exception.AttributeMessages.Add("Failed in the defined review stage number, call the admin please." + ex.Message);
                _iLogger.LogInformation("Exception in getNextOrder " + ex.Message);

                throw _exception;
            }
            return nextApp;
        }

        public async Task<PickTicket> PickTicket(string processNo, int serviceKindNo, int userId, DateTime expectedDateStartFrom, bool bookTicket)
        {
            if (expectedDateStartFrom.AddMinutes(10) < DateTime.Now)
            {
                throw new InvalidOperationException("You can't pick a ticket for previous date");
            }

            bool availableDay = false;

            var serviceKind = _EngineCoreDBContext.ServiceKind.Where(x => x.Id == serviceKindNo).SingleOrDefault();
            if (serviceKind == null)
            {
                throw new InvalidOperationException("There is no configuration for the chosen service kind.");
            }

            int employeeCount = serviceKind.EmployeeCount;
            int minutesPerProcess = serviceKind.EstimatedTimePerProcess;
            string symbol = serviceKind.Symbol;

            // result information.
            PickTicket picTicket = new PickTicket();

            while (!availableDay)
            {

                if (expectedDateStartFrom.Date > DateTime.Now.AddDays(Constants.MAX_QUEUE_DAYS).Date)
                {
                    throw new InvalidOperationException("There is no possibility to pick a ticket after " + Constants.MAX_QUEUE_DAYS.ToString());
                }

                while (await _iGlobalDayOffRepository.IsDayOff(expectedDateStartFrom))
                {
                    // the day is a holiday, pick a ticket at another day.
                    expectedDateStartFrom = expectedDateStartFrom.AddDays(1);
                    TimeSpan resetTime = new TimeSpan(0, 0, 0);
                    expectedDateStartFrom = expectedDateStartFrom.Date + resetTime;
                }

                Dictionary<int, int> dic = new Dictionary<int, int>();

                List<WorkingTimeGetDto> workingHours = new List<WorkingTimeGetDto>();
                workingHours = await _iWorkingTimeRepository.GetWorkingForDate(expectedDateStartFrom.Date);

                // get the number of working hours.
                int workDayInMinutes = 0;
                foreach (var work in workingHours)
                {
                    dic.Add(work.StartFrom, work.FinishAt);
                    workDayInMinutes += work.FinishAt - work.StartFrom;
                }
                dic.OrderBy(k => k.Key);

                if (workDayInMinutes == 0)
                {
                    // no dayOff and working hours for this day, it is a holiday.
                    expectedDateStartFrom = expectedDateStartFrom.AddDays(1);
                    TimeSpan resetTime = new TimeSpan(0, 0, 0);
                    expectedDateStartFrom = expectedDateStartFrom.Date + resetTime;
                    continue;
                }


                // get non executed processes.
                int countOfUnExecutedProcesses = _EngineCoreDBContext.QueueProcesses.Where(x => x.ExpectedDateTime.Date == expectedDateStartFrom.Date && x.Status != (int)Constants.PROCESS_STATUS.FINISHED && x.ServiceKindNo == serviceKindNo && x.ProcessNo != processNo).ToList().Count;


                int reservedTime = countOfUnExecutedProcesses / employeeCount * minutesPerProcess;

                if (reservedTime > workDayInMinutes - minutesPerProcess)
                {
                    // all the day is booked up.
                    expectedDateStartFrom = expectedDateStartFrom.AddDays(1);
                    TimeSpan resetTime = new TimeSpan(0, 0, 0);
                    expectedDateStartFrom = expectedDateStartFrom.Date + resetTime;
                    continue;
                }

                int askedMin = expectedDateStartFrom.Hour * 60 + expectedDateStartFrom.Minute;
                // if the picked date is currently (during the work) and no more processes available or after the working hours for the chosen day.
                if ((askedMin + reservedTime + minutesPerProcess) >= dic.Values.Last())
                {
                    // the expected time is after the working hours for the chosen day.
                    expectedDateStartFrom = expectedDateStartFrom.AddDays(1);
                    TimeSpan resetTime = new TimeSpan(0, 0, 0);
                    expectedDateStartFrom = expectedDateStartFrom.Date + resetTime;
                    continue;
                }

                // calculate estimated time.
                foreach (var element in dic)
                {
                    if (element.Value - element.Key >= reservedTime + minutesPerProcess)
                    {
                        if (askedMin == 0)
                        {
                            expectedDateStartFrom = expectedDateStartFrom.AddMinutes(element.Key + reservedTime);
                            break;
                        }
                        else
                        {
                            if (askedMin <= element.Key)
                            {
                                TimeSpan resetTime = new TimeSpan(element.Key / 60 + reservedTime / 60, element.Key % 60 + reservedTime % 60, 0);
                                expectedDateStartFrom = expectedDateStartFrom.Date + resetTime;
                                break;
                            }

                            if (askedMin <= element.Value && askedMin >= element.Key)
                            {
                                expectedDateStartFrom = expectedDateStartFrom.AddMinutes(reservedTime);
                                break;
                            }

                            reservedTime = Math.Max(reservedTime - (element.Value - element.Key), 0);
                        }
                    }
                    else
                    {
                        reservedTime = Math.Max(reservedTime - (element.Value - element.Key), 0);
                    }
                }

                picTicket.ExpectDateTime = expectedDateStartFrom;
                picTicket.WorkingHours = dic;

                // validate the final expect time if in the working hours, should not reach.
                int expectedMin = picTicket.ExpectDateTime.Hour * 60 + picTicket.ExpectDateTime.Minute;
                bool wrongExpect = true;
                foreach (var element in dic)
                {
                    if (expectedMin >= element.Key || expectedMin <= element.Value)
                    {
                        wrongExpect = false;
                    }
                }
                if (wrongExpect)
                {
                    throw new InvalidOperationException("Wrong in the calculation of the estimated time.");
                }

                if (bookTicket)
                {
                    QueueProcesses proc = _EngineCoreDBContext.QueueProcesses.Where(x => x.ProcessNo == processNo && x.ServiceKindNo == serviceKindNo).FirstOrDefault(); 
                    int previousTicketsCount = _EngineCoreDBContext.QueueProcesses.Where(x => x.ExpectedDateTime.Date == expectedDateStartFrom.Date && x.ServiceKindNo == serviceKindNo).ToList().Count;
                  
                    if (proc == null)
                    {
                        // add a new process to the queue.
                        QueueProcesses newProcess = new QueueProcesses
                        {
                            ServiceKindNo = serviceKindNo,
                            ExpectedDateTime = expectedDateStartFrom,
                            Status = (byte)Constants.PROCESS_STATUS.PENDING,
                            ProcessNo = processNo,
                            TicketId = previousTicketsCount + 1,
                            CreatedDate = DateTime.Now,
                            NotifyLowLevel = false,
                            NotifyMediumLevel = false,
                            NotifyHighLevel = false
                            
                        };

                        if (userId != 0)
                        {
                            newProcess.CreatedBy = userId;
                        }
                        _EngineCoreDBContext.QueueProcesses.Add(newProcess);
                        try { await _EngineCoreDBContext.SaveChangesAsync(); }
                        catch (Exception e)
                        {
                            var s = e.Message.ToString();
                            var d = e.InnerException.ToString();
                        }

                        picTicket.TicketId = symbol + Convert.ToString(newProcess.TicketId);
                    }
                    else
                    {
                        // update the appointment.
                        string note = "change no from " + proc.TicketId.ToString() + " to " + (previousTicketsCount + 1).ToString();
                        QueuePostDto updatedQueue = new QueuePostDto
                        {
                            ServiceKindNo = proc.ServiceKindNo,
                            Status = (byte)Constants.PROCESS_STATUS.PENDING,
                            ExpectedDate = expectedDateStartFrom,
                            ProcessNo = proc.ProcessNo,
                            TicketId = previousTicketsCount + 1,
                            Note = (proc.Note != null && proc.Note.Length < 100)? proc.Note + note : note,
                            CreatedBy = proc.CreatedBy,
                            CreatedDate = proc.CreatedDate,
                            UpdatedDate = DateTime.Now,
                            NotifyHighLevel = false,
                            NotifyMediumLevel = false,
                            NotifyLowLevel = false                       
                        };

                        if (userId != 0)
                        {
                            updatedQueue.UpdatedBy = userId;
                        }
                        await UpdateQueueProcess(proc.Id, updatedQueue);
                        picTicket.TicketId = symbol + Convert.ToString(proc.TicketId);
                    }
                }
                availableDay = true;
            }

            return picTicket;
        }
       
        public async Task<bool> ChangeTicketsStatusToInProgressByProcessNo(int userId, string processNo)
        {
            try
            {
                QueueProcesses originalQueue = await _EngineCoreDBContext.QueueProcesses.Where(a => a.ProcessNo == processNo).FirstOrDefaultAsync();
                if (originalQueue == null || originalQueue.Status == (byte)Constants.PROCESS_STATUS.INPROGRESS || originalQueue.Status == (byte)Constants.PROCESS_STATUS.FINISHED)
                {
                    return true;
                }

                originalQueue.Status = (byte)Constants.PROCESS_STATUS.INPROGRESS;
                originalQueue.StartEffectiveDate = DateTime.Now;
                originalQueue.LastUpdatedDate = DateTime.Now;
                originalQueue.LastUpdatedBy = userId;
                _EngineCoreDBContext.QueueProcesses.Update(originalQueue);
                await _EngineCoreDBContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ChangeTicketStatusBackToPendingByProcessNo(int userId, string processNo)
        {
            QueueProcesses originalQueue = await _EngineCoreDBContext.QueueProcesses.Where(a => a.ProcessNo == processNo).FirstOrDefaultAsync();
            if (originalQueue == null || originalQueue.Status == (byte)Constants.PROCESS_STATUS.PENDING || originalQueue.Status == (byte)Constants.PROCESS_STATUS.FINISHED)
            {
                return false;
            }

            originalQueue.Status = (byte)Constants.PROCESS_STATUS.PENDING;
            originalQueue.LastUpdatedDate = DateTime.Now;
            originalQueue.LastUpdatedBy = userId;
            _EngineCoreDBContext.QueueProcesses.Update(originalQueue);
            await _EngineCoreDBContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeTicketStatusToDone(int userId, string processNo)
        {
            QueueProcesses originalQueue = await _EngineCoreDBContext.QueueProcesses.Where(a => a.ProcessNo == processNo).FirstOrDefaultAsync();
            if (originalQueue == null)
            {
                return false;
            }

            originalQueue.Status = (byte)Constants.PROCESS_STATUS.FINISHED;
            originalQueue.EndEffectiveDate = DateTime.Now;
            originalQueue.LastUpdatedDate = DateTime.Now;
            originalQueue.LastUpdatedBy = userId;
            _EngineCoreDBContext.QueueProcesses.Update(originalQueue);
            await _EngineCoreDBContext.SaveChangesAsync();
            return true;
        }
    
    }
}
