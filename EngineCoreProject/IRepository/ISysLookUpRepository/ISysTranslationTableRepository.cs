using EngineCoreProject.DTOs.SysLookUpDtos;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.ISysLookUpRepository
{
    public interface ISysTranslationTableRepository
    {
        Task<SysTranslation> FindTranslationTableById(int id);
      //  Task<SysTranslation> FindTranslationTableByTable(string TableCode);
      //  Task<SysTranslation> FindTranslationTableBytypeId(int TypeId);
        Task<List<SysTranslation>> GetTranslationTables();
        Task<SysTranslation> GetTranslationTable(int id);
        Task<bool> FindByShortCutAndLang(string ShortCut, string Lang);



    }
}
