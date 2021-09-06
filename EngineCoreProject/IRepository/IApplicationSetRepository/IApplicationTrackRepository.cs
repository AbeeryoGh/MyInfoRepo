using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IApplicationSetRepository
{
    public interface IApplicationTrackRepository
    {
        Task<List<ApplicationTrack>> GetAll(int? ApplicationId);
        Task<List<AppTrackWithUser>> GetAllWithUser(int ApplicationId);
        Task<ApplicationTrack> GetOne(int id);
        Task<List<int>> DeleteMany(int[] ids);
        Task<int> DeleteOne(int id);
        Task<int> Add(ApplicationTrackDto applicationTrackDto); 
        //Task<string> Adds(ApplicationTrackDto applicationTrackDto);
        Task<int> Update(int id, ApplicationTrackDto applicationTrackDto);
    }
}
