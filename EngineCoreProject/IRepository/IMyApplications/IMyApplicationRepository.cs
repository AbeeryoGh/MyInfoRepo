using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using EngineCoreProject.DTOs.MyApplicationDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IMyApplications
{
    public interface IMyApplicationRepository
    {
        

        /// <summary>
        /// get number of applications in every stage for cards view page when admin or employee log in 
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="currentpage"></param> // select current page for pagination
        /// <param name="perpage"></param>// how number of applications in one page
        /// <returns></returns>
        Task<ApplicationCountDto> AllAppByStageType(searchDto searchDto,string lang,int currentpage,int perpage);
        /// <summary>
        /// get All applications for table view page when Admin or Employee log in
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="currentpage"></param>
        /// <param name="perpage"></param>
        /// <returns></returns>
        Task<ApplicationCountDto> appPages(searchDto searchDto, string lang, int currentpage, int perpage);
        /// <summary>
        /// get All applications when Normal user log in
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="currentpage"></param>
        /// <param name="perpage"></param>
        /// <returns></returns>
        Task<ApplicationCountDto> userAllApps(searchDto searchDto, string lang, int currentpage, int perpage);
        /// <summary>
        /// get All applications by search (appnumber , owner ,email, phone) in table view page when Admin or Employee login
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="searchDto"></param>
        /// <param name="currentpage"></param>
        /// <param name="perpage"></param>
        /// <returns></returns>
        Task<ApplicationCountDto> SearchPages(string lang, searchDto searchDto , int currentpage,int perpage);
        /// <summary>
        /// get All applications by search (appnumber , owner ,email, phone) in table view page when Normal user log in
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="searchDto"></param>
        /// <param name="currentpage"></param>
        /// <param name="perpage"></param>
        /// <returns></returns>
        Task<ApplicationCountDto> SearchUserPages(string lang, searchDto searchDto, int currentpage, int perpage);
        /// <summary>
        /// get infromation for stages cards in cards view page when Normal user log in
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="currentpage"></param>
        /// <param name="perpage"></param>
        /// <returns></returns>
        Task<ApplicationCountDto> userAppsBystage(searchDto searchDto, string lang,int currentpage,int perpage);
        /// <summary>
        ///search fro all applications in card view paage when admin or employee log in 
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="searchDto"></param>
        /// <param name="currentpage"></param>
        /// <param name="perpage"></param>
        /// <returns></returns>
        Task<ApplicationCountDto> SearchByStage(string lang, searchDto searchDto, int currentpage, int perpage);
        /// <summary>
        /// search fro all applications in card view paage when user log in
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="searchDto"></param>
        /// <param name="currentpage"></param>
        /// <param name="perpage"></param>
        /// <returns></returns>
        Task<ApplicationCountDto> SearchUserByStage(string lang, searchDto searchDto, int currentpage, int perpage);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="searchDto"></param>
        /// <param name="currentpage"></param>
        /// <param name="perpage"></param>
        /// <returns></returns>
        Task<ApplicationCountDto> SearchEverything(string lang, searchDto searchDto, int currentpage, int perpage);
        ///////////////////////// Notary //////////////////////////

        Task<ApplicationCountDto> NotaryCountApps(string lang);
        Task<ApplicationCountDto> NotarySearchByStage(string lang, searchDto searchDto, int currentpage, int perpage);
        Task<ApplicationCountDto> NotarySearchAll(string lang, searchDto searchDto, int currentpage, int perpage);

        Task<int> getShortCutStageByTranslate(string lang,string value);
        Task<object> addAppTrack();
        string VerifyTransaction(VerifyDto verifyDto,string lang);
       
       

    }
}
