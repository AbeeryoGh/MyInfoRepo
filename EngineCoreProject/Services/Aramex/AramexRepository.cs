using EngineCoreProject.DTOs.AramexDto;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.IAramex;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace EngineCoreProject.Services.Aramex
{
    public class AramexRepository : IAramexRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly ISysValueRepository _ISysValueRepository;

        public AramexRepository(EngineCoreDBContext EngineCoreDBContext, ISysValueRepository iSysValueRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _ISysValueRepository = iSysValueRepository;
        }

        public async Task<List<AramexGetDto>> AramexSearch(string lang, searchAramexDto searchDto,int op)
        {
            int CancelledStateId = await _ISysValueRepository.GetIdByShortcut("AramexCancelled");
            int DeliveriedStateId = await _ISysValueRepository.GetIdByShortcut("AramexDeliveried");
            int NotDeliveriedStateId = await _ISysValueRepository.GetIdByShortcut("AramexNotDeliveried");


            //List<AramexPostDto> aramexPostDtos = new List<AramexPostDto>();
            var query = op==1?(from app in _EngineCoreDBContext.AramexRequests.Where(x=>x.StateId== NotDeliveriedStateId)
                         join app1 in _EngineCoreDBContext.Application.Where(x=>x.Id>0 && x.Delivery==true) 
                         on app.ApplicationId equals app1.Id 
                         join temp in _EngineCoreDBContext.Template on app1.TemplateId equals temp.Id
                         join tr in _EngineCoreDBContext.SysTranslation on temp.TitleShortcut equals tr.Shortcut
                         join lv in _EngineCoreDBContext.SysLookupValue on app.StateId equals lv.Id
                         join tr1 in _EngineCoreDBContext.SysTranslation on lv.Shortcut equals tr1.Shortcut
                         where tr.Lang==lang && tr1.Lang==lang
                         select new AramexGetDto
                         {
                            Id=app.Id,
                            ApplicationId=app.ApplicationId,
                            Mobile=app.Mobile,
                            Email=app.Email,
                            OwnerName=app.OwnerName,
                            StateId=app.StateId,
                            TemplateName=tr.Value,
                            StateName=tr1.Value
                            
                         }).ToList()
                         :
                         (from app in _EngineCoreDBContext.AramexRequests.Where(x => (x.OwnerName.Contains(searchDto.OwnerName) || searchDto.OwnerName==null) &&
                          (x.Mobile.Contains(searchDto.Mobile)||searchDto.Mobile==null) && (x.Email.Contains(searchDto.Email) || searchDto.Email==null))
                          join app1 in _EngineCoreDBContext.Application.Where(x => x.Id > 0 && x.Delivery == true)
                          on app.ApplicationId equals app1.Id
                          join temp in _EngineCoreDBContext.Template on app1.TemplateId equals temp.Id
                          join tr in _EngineCoreDBContext.SysTranslation on temp.TitleShortcut equals tr.Shortcut
                          join lv in _EngineCoreDBContext.SysLookupValue on app.StateId equals lv.Id
                          join tr1 in _EngineCoreDBContext.SysTranslation on lv.Shortcut equals tr1.Shortcut
                          where tr.Lang == lang && tr1.Lang == lang
                          select new AramexGetDto
                          {
                              Id = app.Id,
                              ApplicationId = app.ApplicationId,
                              Mobile = app.Mobile,
                              Email = app.Email,
                              OwnerName = app.OwnerName,
                              StateId = app.StateId,
                              TemplateName = tr.Value,
                              StateName = tr1.Value

                          }).ToList();
            return query;
        }

       public async Task<AramexDetails> AramexDetails(int appId,string lang)
        {
            Task<AramexDetails> query_ = null;
             var query = (from app in _EngineCoreDBContext.AramexRequests.Where(x => x.ApplicationId == appId)
                         join app1 in _EngineCoreDBContext.Application.Where(x => x.Id > 0)
                         on app.ApplicationId equals app1.Id
                         join tranc in _EngineCoreDBContext.AppTransaction on app.ApplicationId equals tranc.ApplicationId
                         join temp in _EngineCoreDBContext.Template on app1.TemplateId equals temp.Id
                         join tr in _EngineCoreDBContext.SysTranslation on temp.TitleShortcut equals tr.Shortcut
                          join lv in _EngineCoreDBContext.SysLookupValue on app.StateId equals lv.Id
                          join tr1 in _EngineCoreDBContext.SysTranslation on lv.Shortcut equals tr1.Shortcut
                          where tr.Lang == lang && tr1.Lang == lang
                         select new AramexDetails
                         {
                             Id=app.Id,
                             ApplicationId = app.ApplicationId,
                             Mobile = app.Mobile,
                             Email = app.Email,
                             OwnerName = app.OwnerName,
                             StateId = app.StateId,
                             TemplateName = tr.Value,
                             DocumentUrl =tranc.DocumentUrl,
                             CreatedDate=app.CreatedDate,
                             StateName=tr1.Value,
                             Note=app.Note
                         });
            query_ = query.FirstOrDefaultAsync();
            return await query_;
        }


        public async Task<int> UpdateAramexRequest(int id,AramexPostDto aramexPostDto)
        {

            AramexRequests aramexRequests = await _EngineCoreDBContext.AramexRequests.Where(x => x.Id == id).FirstOrDefaultAsync();
            aramexRequests.StateId = aramexPostDto.StateId;
                aramexRequests.Note = aramexPostDto.Note;
            try
            {
                _EngineCoreDBContext.Update(aramexRequests);
                await _EngineCoreDBContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return 0;
            }

            return aramexRequests.Id;
        }
    }
}
