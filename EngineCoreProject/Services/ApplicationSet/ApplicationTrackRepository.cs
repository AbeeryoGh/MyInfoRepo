using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.ApplicationSet
{
    public class ApplicationTrackRepository : IApplicationTrackRepository
    {
        private readonly EngineCoreDBContext   _EngineCoreDBContext;
        private readonly IGeneralRepository _IGeneralRepository;

        public ApplicationTrackRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository )

        {
            _EngineCoreDBContext   = EngineCoreDBContext;
            _IGeneralRepository = iGeneralRepository;        
        }

        public async Task<int> Add(ApplicationTrackDto applicationTrackDto)
        {
           try
                {
                    ApplicationTrack applicationTrack = new ApplicationTrack
                     {
                        ApplicationId = (int)applicationTrackDto.ApplicationId,
                        StageId       = applicationTrackDto.StageId,
                        NextStageId   = applicationTrackDto.NextStageId,
                        Note          = applicationTrackDto.Note,
                        UserId        = applicationTrackDto.UserId,
                        CreatedDate   = DateTime.Now,
                        NoteKind=applicationTrackDto.NoteKind
                     };

                    _IGeneralRepository.Add(applicationTrack);
                    if (await _IGeneralRepository.Save())
                    {
                        return applicationTrack.Id;
                    }
                }
                catch (Exception)
                {
                    return Constants.ERROR;
                }

            return Constants.ERROR;
        }

      /*  public async Task<string> Adds(ApplicationTrackDto applicationTrackDto)
        {
            try
            {
                ApplicationTrack applicationTrack = new ApplicationTrack
                {

                    ApplicationId = applicationTrackDto.ApplicationId,
                    StageId = applicationTrackDto.StageId,
                    NextStageId = applicationTrackDto.NextStageId,
                    Note = applicationTrackDto.Note,
                    UserId = applicationTrackDto.UserId

                };

                _IGeneralRepository.Add(applicationTrack);
                if (await _IGeneralRepository.Save())
                {
                    return applicationTrack.Id.ToString();
                }
            }
            catch (Exception e)
            {
                return e.InnerException.ToString();
            }

            return "error";
        }*/

        public Task<List<int>> DeleteMany(int[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<int> DeleteOne(int id)
        {
            ApplicationTrack applicationTrack = await GetOne(id);
            if (applicationTrack == null)
                return Constants.NOT_FOUND;
            try
            {
                _IGeneralRepository.Delete(applicationTrack);
                if (await _IGeneralRepository.Save())
                    return Constants.OK;
            }
            catch (Exception)
            {
                return Constants.ERROR;
            }
            return Constants.ERROR;
        }

        public async Task<List<ApplicationTrack>> GetAll(int? ApplicationId)
        {
            Task<List<ApplicationTrack>> query = null;
            if (ApplicationId == null)

                //query = _EngineCoreDBContext.ApplicationTrack.Include(a=>a.User).ToListAsync();
                query = _EngineCoreDBContext.ApplicationTrack.ToListAsync();
            else
                query = _EngineCoreDBContext.ApplicationTrack.Where(s => s.ApplicationId == ApplicationId).Include(a => a.User).ToListAsync();
            return await query;
        }
        public async Task<List<AppTrackWithUser>> GetAllWithUser(int ApplicationId)//-------------Used in Preview stage to show track with user name
        {
            Task<List<AppTrackWithUser>> query = null;
                query = _EngineCoreDBContext.ApplicationTrack.Where(c=>c.ApplicationId== ApplicationId).Select(c=>new AppTrackWithUser 
                                                                                    { Id=c.Id ,
                                                                                      UserName=c.User.FullName,
                                                                                      ApplicationId = c.ApplicationId,
                                                                                      NextStageId = c.NextStageId,
                                                                                      StageId= c.StageId,
                                                                                      Note=c.Note,
                                                                                      UserId= (int)c.UserId,
                                                                                      Date= (DateTime)c.CreatedDate,
                                                                                   
                                                                                      
                                                                                     }).OrderByDescending(d=>d.Id).ToListAsync();

            return await query;
        }
        public async Task<ApplicationTrack> GetOne(int id)
        {
            var query = _EngineCoreDBContext.ApplicationTrack.Where(x => x.Id == id);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> Update(int id, ApplicationTrackDto applicationTrackDto)
        {
            ApplicationTrack applicationTrack = await GetOne(id);
            if (applicationTrack == null)
                return Constants.NOT_FOUND;
            try
            {
                applicationTrack.ApplicationId = (int)applicationTrackDto.ApplicationId;
                applicationTrack.StageId = applicationTrackDto.StageId;
                applicationTrack.NextStageId = applicationTrackDto.NextStageId;
                applicationTrack.Note = applicationTrackDto.Note;
                applicationTrack.UserId = applicationTrackDto.UserId;
                applicationTrack.LastUpdatedDate = DateTime.Now;
                _IGeneralRepository.Update(applicationTrack);
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
    }
}
