using EngineCoreProject.DTOs.QueueDto;
using EngineCoreProject.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using EngineCoreProject.Services;

namespace EngineCoreProject.IRepository.IQueueRepository
{
    public interface IQueueRepository
    {
        Task<int> AddQueueProcess(QueuePostDto queuePostDto);

        Task<int> UpdateQueueProcess(int rowId, QueuePostDto queueDto);

        /// <summary>
        /// Get oldest process number in queue which in pending mode system which one off parties is logging to the video conference,
        //    and if reviewed by notary user as optional (onlyForProcessOwnerUser).
        /// </summary>
        /// <param name="onlyForProcessOwnerUser"></param>
        /// <returns></returns>
        Task<QueueNextAppDto> GetNextOrder(int askedById);
        Task<QueueGetDto> GetQueueProcess(string processId, int serviceKindNo);

        /// <summary>
        /// Add a new queue process and pick an expected date
        /// </summary>
        /// <param name="processNo">the number of the order</param>
        /// <param name="serviceKindNo">number of the service kind of the order</param>
        /// <param name="expectedDate">pick an expected date after expectedDate</param>
        /// <param name="bookTicket">book directly the ticket on the expected date</param>
        /// <returns></returns>
        Task<PickTicket> PickTicket(string processNo, int serviceKindNo, int userId, DateTime expectedDate, bool bookTicket);

        /// <summary>
        /// Mark the ticket as done ticket with current date.
        /// 
        /// </summary>
        /// <param name="processNo"></param>
        /// <returns>true if succeeded otherwise false</returns>
        Task<bool> ChangeTicketStatusToDone(int userId, string processNo);
        
        Task<bool> ChangeTicketsStatusToInProgressByProcessNo(int userId, string processNo);

        Task<QueueTodayQueueInfo> GetCurrentQueueStatistics(bool onlyMyApp);

        Task<bool> ChangeTicketStatusBackToPendingByProcessNo(int userId, string processNo);

        Task<List<QueueProcesses>> GetQueueForStatus(Constants.PROCESS_STATUS processStatus);
    }
}
