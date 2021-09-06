using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.IBlackList;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace EngineCoreProject.Services.BackList
{
    public class BlackListRepository : IBlackListRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;

        public BlackListRepository(EngineCoreDBContext EngineCoreDBContext)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
        }


        public async Task<List<int>> GetBlackListApplications()
        {
            List<int> result = new List<int>();
            var blackListApps = await _EngineCoreDBContext.BlackListApplication.ToListAsync();
            if (blackListApps != null & blackListApps.Count > 0)
            {
                result = blackListApps.Select(x => x.ApplicationId).ToList();
            }

            return result;
        }


    }
}
