using EngineCoreProject.DTOs.SysLookUpDtos;
using EngineCoreProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.ISysLookUpRepository
{
    public interface ISysLookUpValueRepository
    {


        Task<SysLookupValue> FindTranslationValueById(int id);
        Task<TranslationValueDtoGet> FindTranslationValueByIdWithTrans(int id,string lang);
        Task<SysLookupValue> FindTranslationValueByValue(string ValueCode);
        Task<SysLookupType> FindTranslationValueBytypeId(int TypeId);
        Task<List<TranslationValueDtoGet>> GetTranslationValues(string lang);//GetAllTranslationValues
        Task<List<TranslationValueDtoGet>> GetTranslationValuesForType(string lang,string type);
        Task<List<AllTranslationValueDtoGet>> GetAllTranslationValues();
        Task<TranslationValueDtoGet> GetLookupValue(int id,string lang);


         

    }
}
