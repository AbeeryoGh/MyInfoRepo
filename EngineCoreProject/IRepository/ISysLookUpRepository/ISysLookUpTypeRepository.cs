using EngineCoreProject.DTOs.SysLookUpDtos;
using EngineCoreProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.ISysLookUpRepository
{
    public interface ISysLookUpTypeRepository
    {
        Task<SysLookupType> FindTranslationTypeById(int id);
        Task<SysLookupType> FindTranslationTypeByType(string Type);
        Task<List<TranslationTypeDtoGet>> GetTranslationTypes(string lang);

        Task<List<AllTranslationTypeDtoGet>> GetAllTranslationTypes();
        Task<TranslationTypeDtoGet> GetLookupType(int id);

        // public async Task<TranslationTypeDtoGet> FindTranslationTypeByType(int id,string lang)

        Task<TranslationTypeDtoGet> GetTranslationTypesByID(int id, string lang);





    }
}
