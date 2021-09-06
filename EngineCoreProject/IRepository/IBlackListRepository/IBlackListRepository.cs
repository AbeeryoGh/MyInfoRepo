using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IBlackList
{
    public interface IBlackListRepository
    {
        Task<List<int>> GetBlackListApplications();     
    }
}
