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
    public class SysLookUpValueRepository : ISysLookUpValueRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;

        public SysLookUpValueRepository(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;

        }

        public async Task<SysLookupValue> FindTranslationValueById(int id)
        {
            id = Convert.ToInt32(id);
            SysLookupValue translationValue = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Id == id).FirstOrDefaultAsync();

            return translationValue;
        }

        public async Task<SysLookupValue> FindTranslationValueByValue(string ValueCode)
        {

            SysLookupValue translationValue = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == ValueCode).FirstOrDefaultAsync();

            return translationValue;
        }


        public async Task<SysLookupType> FindTranslationValueBytypeId(int TypeId)
        {

            SysLookupType translationValue = await _EngineCoreDBContext.SysLookupType.Where(x => x.Id == TypeId).FirstOrDefaultAsync();

            return translationValue;
        }

        public async Task<List<TranslationValueDtoGet>> GetTranslationValues(string lang)
        {
            var result = from a in _EngineCoreDBContext.SysLookupValue
                         join c in _EngineCoreDBContext.SysLookupType on a.LookupTypeId equals c.Id
                         join b in _EngineCoreDBContext.SysTranslation on a.Shortcut equals b.Shortcut
                         select (new
                         {

                             Id = a.Id,
                             Type = c.Value,
                             Value = a.Shortcut,
                             TranslationForValue = b.Value,
                             TranslationForType = _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut == c.Value && x.Lang == lang).Select(y => y.Value).FirstOrDefault(),
                             lang = b.Lang
                         });


            List<TranslationValueDtoGet> translationValueList = await result.Where(y => y.lang == lang).Select(x => new TranslationValueDtoGet
            {
                Id = x.Id,
                Type = x.Type,
                Value = x.Value,
                translationType = x.TranslationForType,
                translationValue = x.TranslationForValue
            }).ToListAsync(); //await _EngineCoreDBContext.SysLookupValue.Include(y=>y.LookupType).Select((x) => new TranslationValueDtoGet
            //{

            //    Id = x.Id,
            //    TranslateType = ,
            //    TranslateValue = x.ValueCode

            //}).ToListAsync();
            return translationValueList;
        }

        public async Task<List<TranslationValueDtoGet>> GetTranslationValuesForType(string lang, string type)
        {
            var result = from a in _EngineCoreDBContext.SysLookupValue
                         join c in _EngineCoreDBContext.SysLookupType on a.LookupTypeId equals c.Id
                         join b in _EngineCoreDBContext.SysTranslation on a.Shortcut equals b.Shortcut
                         select (new
                         {

                             Id = a.Id,
                             Type = c.Value,
                             Value = a.Shortcut,
                             TranslationForValue = b.Value,
                             TranslationForType = _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut == c.Value && x.Lang == lang).Select(y => y.Value).FirstOrDefault(),
                             lang = b.Lang
                         });


            List<TranslationValueDtoGet> translationValueList = await result.Where(y => y.lang == lang).Select(x => new TranslationValueDtoGet
            {
                Id = x.Id,
                Type = x.Type,
                Value = x.Value,
                translationType = x.TranslationForType,
                translationValue = x.TranslationForValue
            }).ToListAsync(); //await _EngineCoreDBContext.SysLookupValue.Include(y=>y.LookupType).Select((x) => new TranslationValueDtoGet
            //{

            //    Id = x.Id,
            //    TranslateType = ,
            //    TranslateValue = x.ValueCode

            //}).ToListAsync();
            return translationValueList.Where(x => x.Type == type).ToList(); ;
        }
        public async Task<List<AllTranslationValueDtoGet>> GetAllTranslationValues()
        {
            var result = from a in _EngineCoreDBContext.SysLookupValue
                         join c in _EngineCoreDBContext.SysLookupType on a.LookupTypeId equals c.Id
                         join b in _EngineCoreDBContext.SysTranslation on a.Shortcut equals b.Shortcut
                         select (new
                         {

                             Id = a.Id,
                             Type = c.Value,
                             Value = a.Shortcut,
                             TranslationForValue = b.Value,
                             TranslationForType = _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut == c.Value && x.Lang==b.Lang).Select(y => y.Value).FirstOrDefault(),
                             lang = b.Lang
                         });


            List<AllTranslationValueDtoGet> translationValueList = await result.Select(x => new AllTranslationValueDtoGet
            {
                Id = x.Id,
                Type = x.Type,
                Value = x.Value,
                translationType = x.TranslationForType,
                translationValue = x.TranslationForValue,
                lang=x.lang
            }).ToListAsync(); //await _EngineCoreDBContext.SysLookupValue.Include(y=>y.LookupType).Select((x) => new TranslationValueDtoGet
            //{

            //    Id = x.Id,
            //    TranslateType = ,
            //    TranslateValue = x.ValueCode

            //}).ToListAsync();
            return translationValueList;
        }

        public async Task<TranslationValueDtoGet> GetLookupValue(int id,string lang)
        {
            var result = from a in _EngineCoreDBContext.SysLookupValue
                         join c in _EngineCoreDBContext.SysLookupType on a.LookupTypeId equals c.Id
                         join b in _EngineCoreDBContext.SysTranslation on a.Shortcut equals b.Shortcut
                         select (new
                         {

                             Id = a.Id,
                             Type = c.Value,
                             Value = a.Shortcut,
                             TranslationForValue = b.Value,
                             TranslationForType = _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut == c.Value && x.Lang == lang).Select(y => y.Value).FirstOrDefault(),
                             lang = b.Lang
                         });


            List<TranslationValueDtoGet> translationValueList = await result.Where(y => y.lang == lang && y.Id==id).Select(x => new TranslationValueDtoGet
            {
                Id = x.Id,
                Type = x.Type,
                Value = x.Value,
                translationType = x.TranslationForType,
                translationValue = x.TranslationForValue
            }).ToListAsync();

            //TranslationValueDtoGet lookupValue = await result.Where(x => x.Id == id).Select(y => new TranslationValueDtoGet
            //{
            //    Id = y.Id,
            //    Type =y.,
            //    Value=y.TranslateValue
            //}).FirstOrDefaultAsync();

            return translationValueList.FirstOrDefault(); ;
        }

        public async Task<TranslationValueDtoGet> FindTranslationValueByIdWithTrans(int id, string lang)
        {
            var result = from a in _EngineCoreDBContext.SysLookupValue
                         join c in _EngineCoreDBContext.SysLookupType on a.LookupTypeId equals c.Id
                         join b in _EngineCoreDBContext.SysTranslation on a.Shortcut equals b.Shortcut
                         select (new
                         {
                             Id = a.Id,
                             Type = c.Value,
                             Value = a.Shortcut,
                             TranslationForValue = b.Value,
                             TranslationForType = _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut == c.Value && x.Lang == lang).Select(y => y.Value).FirstOrDefault(),
                             lang = b.Lang
                         });


            TranslationValueDtoGet translationValueList = await result.Where(y => y.lang == lang && y.Id==id).Select(x => new TranslationValueDtoGet
            {
                Id = x.Id,
                Type = x.Type,
                Value = x.Value,
                translationType = x.TranslationForType,
                translationValue = x.TranslationForValue
            }).FirstOrDefaultAsync(); //await _EngineCoreDBContext.SysLookupValue.Include(y=>y.LookupType).Select((x) => new TranslationValueDtoGet
            //{

            //    Id = x.Id,
            //    TranslateType = ,
            //    TranslateValue = x.ValueCode

            //}).ToListAsync();
            return translationValueList;
        }
    }
}
