using EngineCoreProject.DTOs.MeetingDto;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EngineCoreProject.Services;

namespace EngineCoreProject.IRepository.IMeetingRepository
{
    public interface IMeetingRepository
    {

        /// <summary>
        /// Add a new schedule meeting.
        /// </summary>
        /// <param name="meetingDto">The meeting details to be added of type MeetingPostDto</param>
        /// <exception cref="INVALD_ENTRY_ERROR">Thrown when ....</exception>
        Task<Meeting> AddMeeting(MeetingPostDto meetingDto, int userId, string lang);

        /// <summary>
        /// Update an existing meeting.
        /// </summary>
        /// <param name="rowId">The id of the record</param>
        /// <param name="meetingDto">The meeting details to be added of type MeetingPostDto</param>
        /// <param name="userId">The id of the user</param>
        /// <exception cref="INVALD_ENTRY_ERROR">Thrown when ....</exception>
        Task<Meeting> UpdateMeeting(int rowId, MeetingPostDto meetingDto, int userId, string lang);

        /// <summary>
        ///    Get a list of the meeting which created by the user.
        /// </summary>
        /// <param name="userId">The id of the certain user</param>
        /// <exception cref="INVALD_ENTRY_ERROR">Thrown when ....</exception>
        Task<List<MeetingGetDto>> GetMeetings(int userId, string lang);


        /// <summary>
        /// Get a  meeting by row id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MeetingGetDto> GetMeetingById(int id, string lang);

        /// <summary>
        /// Get a  meeting accord to meetingId which created by the user.
        /// </summary>
        /// <param name="meetingId"></param>
        /// <exception cref="INVALD_ENTRY_ERROR">Thrown when ....</exception>
        Task<MeetingGetDto> GetMeetingByMeetingId(string meetingId, string lang);

        /// <summary>
        /// Get a  meeting accord to meetingId and password.
        /// </summary>
        /// <param name="userId">The id of the certain user</param>
        /// <param name="meetingId"></param>
        /// <exception cref="INVALD_ENTRY_ERROR">Thrown when ....</exception>
        Task<MeetingGetDto> GetMeetingByMeetingIdAndPassword(string meetingId, string password, string lang);


        /// <summary>
        /// Set the status of an existing meeting.
        /// </summary>
        /// <param name="meetingId"></param>
        /// <param name="newStatus">FINISHED = -1 OR PENDING = 0 OR STARTED = 1</param>
        /// <param name="cahngeAppointment">Convert the meeting appointment to the current date if true</param>
        /// <returns></returns>
        Task<int> SetMeetingStatus(string meetingId, Constants.MEETING_STATUS newStatus, bool cahngeAppointment, string lang);

        /// <summary>
        /// Get true if the password of the meeting is required otherwise false.
        /// </summary>
        /// <param name="meetingId">The id of the meeting</param>
        /// <exception cref="INVALD_ENTRY_ERROR">Thrown when ....</exception>
        Task<bool> MeetingHasPassword(string meetingId);

        /// <summary>
        /// Determine if meeting id is existed before.
        /// </summary>
        /// <param name="meetingId"></param>
        /// <returns>true if exist otherwise false</returns>
        Task<bool> IfExistMeeting(string meetingId);

        Task GetMeetingLogger();
        Task<object> MeetingJWT(string  meetingId, int? userId, string userName, string lang, string meetingPassword = null);

        /// <summary>
        /// Get a list of meeting for a related order no.
        /// </summary>
        /// <param name="orderNo">the number of the order</param>
        /// <returns></returns>
        Task<List<MeetingGetDto>> GetMeetingByOrderNo(string orderNo);

        Task<bool> ChangeMeetingStatusBackToPendingByAppId(string applicationId, string lang);

        Task<IsAttended> IsAttendedByAppNo(int orderNo);

        Task<bool> LogInToMeeting(string meetingNo);
    }
}
