using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.SysLookUpDtos;
using EngineCoreProject.DTOs.TemplateSetDtos;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository
{
    public interface ISysValueRepository
    {
        /* Task<List<IdValueDto>> GetTypeAll( string lang,string type);

        Task<string> AddForFirst(ArabicValue arabicValue, int docType_id);*/
        Task<List<TypeList>> GetTypeAll(string lang, string type);
        Task<string> GetValueByShortcut( string shortcut,string lang);

        Task<int> GetIdByShortcut(string shortcut);
        Task<string> GetValueById(int id, string lang);
        Task<string> AddRecord(int id,string table_name,string field_name,int parent_id, string parent_field_name);
        Task<string> AddTranslation(TranslationDto translationDto);

        Task<List<CountryDto>> GetAllCountry(string lang);

    }
}
