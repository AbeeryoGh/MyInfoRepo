using EngineCoreProject.DTOs.AramexDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IAramex
{
    public interface IAramexRepository
    {
        Task<int> UpdateAramexRequest(int id,AramexPostDto aramexPostDto);
        Task<List<AramexGetDto>> AramexSearch(string lang, searchAramexDto searchDto,int op);
        Task<AramexDetails> AramexDetails(int appId,string lang);
       
    }
}
