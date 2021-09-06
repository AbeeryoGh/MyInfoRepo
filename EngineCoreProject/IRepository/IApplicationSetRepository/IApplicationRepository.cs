using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.AdmService.ModelView;
using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.ApplicationDtos.Action;
using EngineCoreProject.DTOs.ApplicationDtos.IdDtos;
using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using EngineCoreProject.DTOs.ApplicationDtos.RelatedContent;
using EngineCoreProject.DTOs.BasherDto;
using EngineCoreProject.DTOs.FileDto;
using EngineCoreProject.DTOs.MyApplicationDto;
using EngineCoreProject.DTOs.NotificationDtos;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static EngineCoreProject.Services.Constants;

namespace EngineCoreProject.IRepository.IApplicationSetRepository
{
    public interface IApplicationRepository

    {
        Task<List<Application>> GetAll(int? serviceId);
        Task<Application> GetOne(int id);
        Task<byte[]> GetRowVersion(int id);
        Task<Application> GetOneWithAllRelated(int id);
        Task<List<RelatedContentView>> GetAppRelatedContents(int id,string lang);
        Task<List<int>> DeleteMany(int[] ids);
        Task<int> DeleteOne(int id);
        Task<int> Add(ApplicationDto applicationDto,int? addBy);
        Task<int> Add(TargetApplicationDto applicationTargetDto);
       
        Task<int> Add(AppRelatedContentDto appRelatedContent);
        Task<AppRelatedContent> GetOneRelatedContent(int id);
        Task<APIResult> UpdateRContent(int id, AppRelatedContentDto appRelatedContent);
        Task<string> getAppState(int? stateId, string lang);


        Task<int> AddToStage(ApplicationDto applicationDto,int stageOrder,int userId);
        Task<APIResult> Update(int id, int userId ,/*byte[] rowVersion,*/ ApplicationDto applicationDto);

        Task<APIResult> RefreshReadingDate(int id, int userId);
        Task<List<StageOfService>> getStageOfService(int appId,/*int first_stage, */int id,string lang);
        Task<List<ActionOfStage>>  getActions(int stageId, string lang);

        Task<List<ActionOfStage>> getActionsByUserRole(int stageId, int userId,string lang);
        Task<int> NextPreviewsStage(int serviceId, int currentStage, int step);

        Task<int> SetStageForward (StagePayload stagePayload,int userId);
        Task<int> SetStageBackward(StagePayload stagePayload, int userId);
        Task<APIResult> SetToFirstStage (StagePayload stagePayload, int userId);
        Task<APIResult> SetToStage(StagePayload sp, int stageId, int userId,string state);

        Task<int> SetToState(StagePayload stagePayload,int userId,string state);
        Task<List<int>> StageAndNext(int? serviceId, int stageOrder ,int shift);


        Task<List<ApplicationByStageType>>    IncomingApplications(string lang);
        Task<List<ApplicationAttachmentView>> getRelatedAttachments(int appId, string lang);
       /* Task<int> SaveAllApplicationDataAppId( ApplicationDto applicationDto, 
                                               List<ApplicationAttachmentDto> applicationAttachmentDtos,
                                               List<ApplicationPartyWithExtraDto> applicationPartyDtos,
                                               List<ApplicationPartyExtraAttachmentDto> applicationPartyExtraAttachmentWIds,
                                               TransactionDto transactionDto,
                                               ApplicationTrackDto applicationTrackDto,
                                               int AppId=0);*/
        Task<APIResult> SaveAllApplicationData (ApplicationWIdDto applicationDto,
                                          List<TargetApplicationDto> applicationTargetDto,
                                          List<ApplicationAttachmentDto> applicationAttachmentDtos,
                                          List<ApplicationPartyWithExtraDto> applicationPartyDtos,
                                          List<ApplicationPartyExtraAttachmentDto> applicationPartyExtraAttachmentWIds,
                                          List<AppRelatedContentDto> appRelatedContentDtos,
                                          TransactionDto transactionDto,
                                          ApplicationTrackDto applicationTrackDto,
                                          int userId,
                                          string lang

                                          );
        Task<APIResult> PostAllApplicationData_(ApplicationDto applicationDto,
                                          List<TargetApplicationDto> applicationTargetDto,
                                         List<ApplicationAttachmentDto> applicationAttachmentDtos,
                                         List<ApplicationPartyWithExtraDto> applicationPartyDtos,
                                         List<AppRelatedContentDto> appRelatedContentDtos,
                                         TransactionDto transactionDto,
                                         ApplicationTrackDto applicationTrackDto,
                                         FIERST_SAVE_STAGE toStage ,
                                         int userId,
                                         string lang);
        /*Task<int> PostAllApplicationData(ApplicationDto applicationDto,
                                         List<ApplicationAttachmentDto> applicationAttachmentDtos,
                                         List<ApplicationPartyWithExtraDto> applicationPartyDtos,
                                         TransactionDto transactionDto,
                                         ApplicationTrackDto applicationTrackDto);*/

        Task<APIResult> UpdateAllApplicationData(ApplicationWIdDto appDto,
                                           List<TargetApplicationWIdDto> targetAppDto,
                                           List<ApplicationAttachmentWIdDto> appAttachmentDtos,
                                           List<ApplicationPartyWIdDto> appPartyDtos,
                                           List<ApplicationPartyExtraAttachmentWIdDto> appPartyExtraAttachmentWIds,
                                           List<AppRelatedContentWIdDto> appRelatedContentDtos,
                                           TransactionWIdDto transactionDto,
                                           ApplicationTrackWIdDto appTrackDto,
                                           int userId,
                                           string lang);
        Task<APIResult> DeleteRelatedApplicationData(List<int> attachments,List<int> parties, List<int> extraAttachments);

        Task<dynamic> Schedule(string appId,int ServiceTypeId, int UserId,/*List<int> parties,*/int serviceId, string title, DateTime date);
        Task<APIResult> UpdateSchedule(string appId, int ServiceTypeId,int serviceId,int actionId, int UserId, DateTime newDate);

        Task<dynamic> GetAppScheduleInfo(string orderNo);
        
        Task<List<int>> GetPartyByAppID(int AppId);


        Task<List<Receiver>> AddUserReceiverData(List<Receiver> receivers);
        Task<bool> IsParty(int AppId,int userId);

        // TODO should removed duplicated.
        Task<List<NotificationLogPostDto>> BuildNotificationObjectFromResponse(List<NotificationLogPostDto> notifications,  List<int> reciverIDs);
        Task<List<NotificationLogPostDto>> BuildNotificationObjectFromResponse(List<NotificationLogPostDto> notifications, List<Receiver> recivers);


        Task<List<NotificationLogPostDto>> BuildNotificationObjectFromResponseByUser(List<NotificationLogPostDto> notifications, List<Receiver> receivers, List<string> notyBody);
        Task<int> NotifyAppParties(int appId, List<NotificationLogPostDto> notifications);

        Task<int> NotifyParties(List<int> parties, List<NotificationLogPostDto> notifications);
        Task<int> NotifyParties(List<Receiver> receiver, List<NotificationLogPostDto> notifications);

        //  Task<int> NotifyPartiesOneByOne(List<int> parties, List<NotificationLogPostDto> notifications, List<string> notifBody);


        Task<bool> IsSignedApp(int appId);

        Task<APIResult> ESignIt(int appPartyId, int userId, int appId, string Base64Sign,int signType);

        Task<List<ApplicationPartySignState>> SignersAndNot(int appId);

        ApplicationWIdDto FromObjectToDto(Application application);
        Task<int> MakeItDone(int appId,int userId );

        Task<partiesInfo> PartyFinalDocument(int appId);
        Task<APIResult> EndAppMeetings  (int appId);


        Task<List<UserApplication>> GetUserTransaction(int serviceId, int userId,string lang);
        Task<List<UserApplication>> GetUserTransaction(ICollection<TargetService> targetServices, int userId, string lang);
        Task<List<UserApplication>> GetUserApplication(int serviceId, int userId, string lang,List<int> AcceptableStage, List<string> AcceptableState);
        Task<List<AppServiceStage>> GetRelatedApplicationsInfo(int appId, string lang);



        Task<dynamic>   GetPreviewStageData(string isNext,int appId, int userId , string lang);
        Task<dynamic>   GetPreviewStageDataLight(int appId, int userId, string lang);
        Task<dynamic>   GetInitialData(int serviceId, string lang,int userId);
        Task<APIResult> FullUpdateWithForwardNoti(FullUpdate fullUpdate,int userId, string lang, int actionId);
        Task<APIResult> FullUpdateWithForward (FullUpdate fullUpdate, int actionId,int userId, string lang);
        Task<APIResult> FullUpdateAndStay(FullUpdate fullUpdate, int userId,string state, string lang);
        Task<APIResult> BackToFirstStageNoti(FullUpdate fullUpdate,int userId, string lang);
        Task<APIResult> BackToApprovalStageNoti(FullUpdate fullUpdate, int userId, string lang);
        Task<APIResult> SigningNotiForward(FullUpdate fullUpdate, int actionId,int userId, string lang);
        Task<APIResult> ChangeAppStateWithNoti(FullUpdate fullUpdate, string to, int actionId,int userId, string lang); 
        Task<APIResult> PostAllWithNotification(FullUpdate fullUpdate, FIERST_SAVE_STAGE toStage, int actionId,int userId, string lang);
        Task<APIResult> PostAll(FullUpdate fullUpdate, FIERST_SAVE_STAGE toStage,int userId, string lang);

        Task<APIResult> StageForward(FullUpdate fu, int actionId, int userId, string lang);
        Task<APIResult> StageBackward(FullUpdate fu, int actionId, int userId, string lang);

        Task<NotaryInfo> GetLastUpdaterNotary(int appId);

        Task<byte[]> ChangeLockStatus(int appId,int userId,ApplicationDto applicationDto, bool locked);

        CreateFolderMessage CreateAppFolder(string serviceId,string appId);
        void DeleteAppFolder(string serviceId, string appId);
        bool MoveAppAttachment(string source, string destination);
        Task<dynamic> GetNotificationsByAction(int actionId, string lang, int serviceId,int stageId,int appId);


        Task<dynamic> GetRequiredData(string lang);

        Task<APIResult> IfBookedUp(int appId,int userId, string lang);
        Task<APIResult> freeApplication(int appId, int userId);

        Task<sendMOADetailsMOJResponse> AddApplicationBashr(SendAppMOADetails_MOJ sendAppMOADetails_MOJ);
        Task<APIResult> NotifyWithTokenLink(int appId, int serviceId, int actionId, string lang);

        Task<APIResult> RebuildPDFDocuments(FullUpdate fu, int userId,string lang);
        Task<APIResult> BuildPDFDocuments(int appId);

        Task<APIResult> ClearRelatedPartiesSignInfo(ApplicationTrackDto applicationTrackDto, int userId, string lang);

        Task<APIResult> OwnApplication(int appId, int userId, string lang);
        Task<APIResult> ReleaseApplication(int appId, int userId, string lang);

        Task<List<UserApplication>> Search(SearchObject searchObject, string lang);

        Task<APIResult> NotifyLateAppsPartyies(List<ServiceApplication> serviceApplications);

        Task<int> AddAppObjection(AppObjectionDto appObjectionDto, string lang);

        Task<int> AddAppObjectionParty(int customerUserId, int appId, string reason, string lang);

        Task<List<AppObjectionDto>> GetAppObjection(int appId);

        Task<List<LateAppsDto>> GetLateApps(string lessDate);
        Task<APIResult> RejectApps(string lessDate);
        Task<List<LateAppsDto>> GetNotLateApps();
        Task<APIResult> DailyNotify();

        bool InitialBlockChain(int appid);

        Task<APIResult> AddNoteTrack(ApplicationTrackDto applicationTrackDto);
        Task<APIResult> GetDoneNoPay(int appId, int userId, int? timeLimit, List<string> timeoutMsgs, string lang);
    }
}
