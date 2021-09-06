using EngineCoreProject.Services;
using EngineCoreProject.DTOs.SysLookUpDtos;
using EngineCoreProject.IRepository.ISysLookUpRepository;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.SysLookUpService
{
    public class SysTranslationTableRepository : ISysTranslationTableRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;

        public SysTranslationTableRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;

        }

        public async Task<bool> FindByShortCutAndLang(string ShortCut, string Lang)
        {
            bool found = true;
            SysTranslation translationTable = await _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut == ShortCut && x.Lang==Lang).FirstOrDefaultAsync();

            if (translationTable == null) found = false;

            return found;

        }

        public async Task<SysTranslation> FindTranslationTableById(int id)
        {
            id = Convert.ToInt32(id);
            SysTranslation translationTable = await _EngineCoreDBContext.SysTranslation.Where(x => x.Id == id).FirstOrDefaultAsync();

            return translationTable;
        }

        

        public async Task<SysTranslation> GetTranslationTable(int id)
        {
            SysTranslation sysTranslationTable = await _EngineCoreDBContext.SysTranslation.Where(x => x.Id == id).FirstOrDefaultAsync();

            return sysTranslationTable;
        }

        public async Task<List<SysTranslation>> GetTranslationTables()
        {
            List<SysTranslation> translationTableList = await _EngineCoreDBContext.SysTranslation.ToListAsync();
            return translationTableList;
        }

        
    }
}
