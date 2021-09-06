
using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.AdmService;
using EngineCoreProject.DTOs.SysLookUpDtos;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services
{
    public interface IGeneralRepository
    {
        void Add<T>(T entity) where T : class;

        void Delete<T>(T entity) where T : class;

        void Update<T>(T entity) where T : class;

        Task<bool> Save();

        Task<bool> FindLanguage(string lang);

        Task<bool> FindShortCut(string ShortCut);

        string GenerateShortCut(string tableName, string columnName);

        Task<AddNewTransResult> insertTrans(int inserttype, string target, string lang, string value);

        Task<SysTranslation> updateTranslation(string lang, string value, string shortCut);

        Task<int> DeleteTranslation(string shortCut);

        Task<AddNewTransResult> insertTranslation(string shortcut, string value, string currentLanguage);

        Task<List<SysTranslation>> InsertUpdateSingleTranslation(string shortCut, Dictionary<string, string> TranslationDictionary);

        Task<List<SysTranslation>> insertListTranslationAsync(List<TranslationDto> TranslationList);

        Task<List<Dictionary<string, string>>> insertDictionaryTranslationAsync(string shortCut, List<Dictionary<string, string>> TranslationDictionary);

        Task<Dictionary<string, string>> getTranslationsForShortCut(string shortCut);
        Task<ClassDto> getShortCutId(string shortcut);

        int GetNewValueBySec();
        int GetNewValueForSMSBySec();
        int GetNextSecForPayment();

        Task <string> GetTranslateByShortCut(string lang, string shortCut);

        string getTranslateByIdFromLookUpValueId(string lang, int id);

      //  string SecureHashGeneration(string StringForHash);
        //string SecureHashDecryption(string StringForHash);
        int? CalculateFee(int serviceId);

        string GetDecviceInfo(string userAgent);
        Task<List<AllTranslationDto>> GetAllTranslation(string shortcut);
        public string GenerateURL(Dictionary<string, string> DictionaryQueryString, string URL);
        public string SecureHashGenerationHMACSHA256(string StringForHash, string Securekey);
        public string ConvertFromHTMLTpPDF(string HTML);


        Task<string> getServiceNameTranslateAsync(string lang, int? PaymentId);
        Task<string> getServiceNameTranslateByAppIdAsync(string lang, int? applicationId);

        int AppDevice(string userAgent);
        int TypeofSign(int id);
        



    }
}
