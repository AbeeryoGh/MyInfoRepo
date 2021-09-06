using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IFilesUploader
{
    public interface IFileConfigurationRepository
    {
        Task<FileConfiguration[]> GetFileConfigurations();

        Task<FileConfiguration> GetFileConfigurationById(int id);

        Task<FileConfiguration> GetFileConfigurationByExt(string extention);
    }
}
