using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository
{
    public interface IFileConfigRepository
    {
        Task<FileConfiguration[]> GetFileConfigurations();
        Task<FileConfiguration> GetFileConfigurationById(int id);
        Task<FileConfiguration> GetFileConfigurationByExtension(string extention);
    }
}
