using EngineCoreProject.Services;
using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.SysLookUpDtos;
using EngineCoreProject.DTOs.TemplateSetDtos;
using EngineCoreProject.IRepository;
using EngineCoreProject.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services
{
    public class SysValueRepository : ISysValueRepository
    {
        private readonly EngineCoreDBContext   _EngineCoreDBContext;
        private readonly IGeneralRepository _IGeneralRepository;

        public SysValueRepository(EngineCoreDBContext iEngineCoreDBContext, IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = iEngineCoreDBContext;
            _IGeneralRepository = iGeneralRepository;
        }
     /*   public async Task<string> AddForFirst(ArabicValue arabicValue, int docTypeId)
        {

            var shortcut = new SqlParameter("@SHORTCUT", SqlDbType.VarChar);
            shortcut.Direction = ParameterDirection.Output;
            shortcut.Size = 25;
            var type = new SqlParameter("@TYPE", arabicValue.LookupType);
            var lang = new SqlParameter("@LANG", "ar");
            var value = new SqlParameter("@VALUE", arabicValue.Value);
            var docType_id = new SqlParameter("@DOCTYPE_ID", docTypeId);
            await _EngineCoreDBContext.Database.ExecuteSqlRawAsync("Exec dbo.AddRecord @TYPE , @LANG, @VALUE,@DOCTYPE_ID, @SHORTCUT out",
                                                                 type, lang, value, docType_id, shortcut);

            return shortcut.Value.ToString();
        }*/

 public async Task<string> AddRecord(int id, string tableName, string fieldName,int parentId, string parentFieldName)
        {
            var shortcut = new SqlParameter("@SHORTCUT", SqlDbType.VarChar);
            shortcut.Direction = ParameterDirection.Output;
            shortcut.Size = 25;
            var template_id = new SqlParameter("@ID", id);
            var table_name =  new SqlParameter("@TABLENAME", tableName);
            var field_name =  new SqlParameter("@FIELDNAME", fieldName);
            var parent_id =   new SqlParameter("@PARENTID", parentId);
            var parent_field_name = new SqlParameter("@PARENTFIELDNAME", parentFieldName);

            await _EngineCoreDBContext.Database.ExecuteSqlRawAsync("Exec dbo.NewRecord @TABLENAME,@FIELDNAME,@ID,@PARENTID,@PARENTFIELDNAME, @SHORTCUT out",
                                                                 table_name, field_name,template_id, parent_id, parent_field_name, shortcut);

            return shortcut.Value.ToString();
        }

 public async Task<string> AddTranslation(TranslationDto translationDto)
        {
            var last_id = new SqlParameter("@LASTID", SqlDbType.Int);
            last_id.Direction = ParameterDirection.Output;

            var Shortcut = new SqlParameter("@SHORTCUT",translationDto.Shortcut);
            var lang     = new SqlParameter("@LANG"    ,translationDto.Lang);
            var value    = new SqlParameter("@VALUE"   ,translationDto.Value);

            await _EngineCoreDBContext.Database.ExecuteSqlRawAsync("Exec dbo.AddTranslation  @LANG, @VALUE, @SHORTCUT,@LASTID out",
                                                                 lang, value, Shortcut, last_id);

            return last_id.Value.ToString();
        }

        public async Task<List<CountryDto>> GetAllCountry(string lang)
        {
            if(lang=="ar")
                return await _EngineCoreDBContext.Country.Where(u=>u.UgId!= null).Select(u => new CountryDto
            {

                    //Id = u.CntId,
                    Id = (int)u.UgId,
                    Name = u.CntCountryAr,
                
            }).ToListAsync();

            else
                return await _EngineCoreDBContext.Country.Where(u => u.UgId != null).Select(u => new CountryDto
                {

                    Id = (int)u.UgId,
                    Name = u.CntCountryEn,

                }).ToListAsync();
        }

        public async Task<int> GetIdByShortcut(string shortcut)
        {
            SysLookupValue lookupValue = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut== shortcut).FirstOrDefaultAsync();
            if (lookupValue != null)
                return lookupValue.Id;
            else return -1;
        }


        /*public async Task<List<IdValueDto>> GetTypeAll(string _lang, string _type)
          {

              var type = new SqlParameter("@TYPE", _type);
              var lang = new SqlParameter("@LANG", _lang);
              var result= await _EngineCoreDBContext.IdValueDto.FromSqlRaw("Exec dbo.get_type_elements @TYPE , @LANG", type, lang)
                                                            .ToListAsync();

              return result;
          }*/

        public async Task<List<TypeList>> GetTypeAll(string lang, string type)
        {

          //  Task<List<TypeList>> result = null;
          var  query = (from t in _EngineCoreDBContext.SysTranslation
                        join lv in _EngineCoreDBContext.SysLookupValue
                           on t.Shortcut equals lv.Shortcut
                        join lt in _EngineCoreDBContext.SysLookupType
                           on lv.LookupTypeId equals lt.Id

                     where t.Lang == lang
                     where lt.Value== type
                     select new TypeList
                     {
                       Id=lv.Id,
                       ValueId=t.Id,
                       Value=t.Value,
                       ShortCut=t.Shortcut,
                       BoolParameter=lv.BoolParameter,
                       Order=lv.Order                     
                     });

            return await query.ToListAsync();
        }

        public async Task<string> GetValueById(int id,string lang)
        {

            Task<string> query = null;
            query = (from lv in _EngineCoreDBContext.SysLookupValue
                     join t in _EngineCoreDBContext.SysTranslation
                     on lv.Shortcut equals t.Shortcut


                     where t.Lang == lang
                     where lv.Id ==id
                     select t.Value
                 ).FirstOrDefaultAsync();

            return await query;

        }

        public async Task<string> GetValueByShortcut( string shortcut,string lang)
        {
            /*var query = (from t in _EngineCoreDBContext.SysTranslation
                          where t.Lang == lang
                         where t.Shortcut == shortcut
                         select new { value=t.Value }
                        );
            return await query.FirstOrDefaultAsync().ToString();*/
            SysTranslation translationTable = await _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut == shortcut && x.Lang == lang).FirstOrDefaultAsync();
            if (translationTable != null)
                return translationTable.Value;
            else return "Translation Not Found!";
        }
    }
}
