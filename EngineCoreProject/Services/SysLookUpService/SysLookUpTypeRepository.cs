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
    public class SysLookUpTypeRepository : ISysLookUpTypeRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;

        public SysLookUpTypeRepository(EngineCoreDBContext EngineCoreDBContext , IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;            
            _iGeneralRepository = iGeneralRepository;
            
        }

        public async Task<SysLookupType> FindTranslationTypeById(int id)
        {
            id = Convert.ToInt32(id);
            SysLookupType translationType = await _EngineCoreDBContext.SysLookupType.Where(x => x.Id == id).FirstOrDefaultAsync();

            return translationType;
        }

        public async Task<SysLookupType> FindTranslationTypeByType(string Type)
        {
            
            SysLookupType translationType = await _EngineCoreDBContext.SysLookupType.Where(x => x.Value == Type).FirstOrDefaultAsync();

            return translationType;
        }

        public async Task<List<TranslationTypeDtoGet>> GetTranslationTypes(string lang)
        {
            var result = from a in _EngineCoreDBContext.SysLookupType
                         join b in _EngineCoreDBContext.SysTranslation on a.Value equals b.Shortcut
                         select (new
                         {

                             Id = a.Id,
                             TranslateType = a.Value,

                             TranslationShortcut = b.Value,
                             lang = b.Lang
                         });


            List<TranslationTypeDtoGet> translationTypeList = await result.Where(y => y.lang == lang).Select(x => new TranslationTypeDtoGet
            {

                TypeID = x.Id,
                shortcut = x.TranslateType,
                translationValue = x.TranslationShortcut

            }).ToListAsync();
            return translationTypeList;
        }

        public async Task<List<AllTranslationTypeDtoGet>> GetAllTranslationTypes( )
        {
            var result = from a in _EngineCoreDBContext.SysLookupType
                         join b in _EngineCoreDBContext.SysTranslation on a.Value equals b.Shortcut
                         select (new
                         {

                             Id = a.Id,
                             TranslateType = a.Value,
                             TranslationShortcut = b.Value,
                             lang = b.Lang
                         });


            List<AllTranslationTypeDtoGet> translationTypeList = await result.Select(x => new AllTranslationTypeDtoGet
            {

                TypeID = x.Id,
                shortcut = x.TranslateType,
                translationValue = x.TranslationShortcut,
                lang=x.lang
               
            }).ToListAsync();
            return translationTypeList;
        }





        public async Task<TranslationTypeDtoGet> GetTranslationTypesByID(int id,string lang)
        {
            var result = from a in _EngineCoreDBContext.SysLookupType
                         join b in _EngineCoreDBContext.SysTranslation on a.Value equals b.Shortcut
                         select (new
                         {

                             Id = a.Id,
                             TranslateType = a.Value,

                             TranslationShortcut = b.Value,
                             lang = b.Lang
                         });


            TranslationTypeDtoGet translationTypeList = await result.Where(y => y.lang == lang && y.Id==id).Select(x => new TranslationTypeDtoGet
            {

                TypeID = x.Id,
                shortcut = x.TranslateType,
                translationValue = x.TranslationShortcut

            }).FirstOrDefaultAsync();
            return translationTypeList;
        }


        public async Task<TranslationTypeDtoGet> GetLookupType(int id)
        {
            TranslationTypeDtoGet lookupType = await _EngineCoreDBContext.SysLookupType.Where(x => x.Id == id).Select(y=>new TranslationTypeDtoGet {
                TypeID = y.Id,
                shortcut = y.Value
            }).FirstOrDefaultAsync();
             
            return lookupType;
        }




    }
}
