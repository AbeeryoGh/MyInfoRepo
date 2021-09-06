using EngineCoreProject.IRepository;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services
{
    public class FileConfigRepository : IFileConfigRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _IGeneralRepository;

        public FileConfigRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _IGeneralRepository = iGeneralRepository;
        }


        public async Task<FileConfiguration> GetFileConfigurationById(int id)
        {
            id = Convert.ToInt32(id);
            var query = _EngineCoreDBContext.FileConfiguration.Where(x => x.Id == id);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<FileConfiguration[]> GetFileConfigurations()
        {
            var query = _EngineCoreDBContext.FileConfiguration.ToArrayAsync();
            return await query;
        }


        public async Task<FileConfiguration> GetFileConfigurationByExtension(string extention)
        {
            var query = _EngineCoreDBContext.FileConfiguration.Where(a => a.Extension.Contains(extention));
            return await query.FirstOrDefaultAsync();
        }
    }
}
