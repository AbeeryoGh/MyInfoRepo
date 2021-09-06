
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EngineCoreProject.Models;
using System.Reflection;
using Microsoft.Data.SqlClient;
using EngineCoreProject.DTOs.SysLookUpDtos;
using Microsoft.Extensions.Options;
using EngineCoreProject.DTOs.Payment;
using DeviceDetectorNET.Parser;
using EngineCoreProject.DTOs;
using System.Web;
using System.Text;
using DinkToPdf;
using DinkToPdf.Contracts;
using EngineCoreProject.DTOs.PDFGenerator;
using System.IO;
using EngineCoreProject.DTOs.AdmService;
using EngineCoreProject.IRepository.IFilesUploader;

namespace EngineCoreProject.Services
{
    public class GeneralRepository : IGeneralRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private IConverter _converter;
       
        private readonly FileNaming _pdfFileNaming;

        private readonly PaymentSettings _paymentSettings;



        public GeneralRepository( EngineCoreDBContext EngineCoreDBContext, IOptions<PaymentSettings> paymentSettings, IConverter converter, IOptions<FileNaming> pdfFileNaming)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _paymentSettings = paymentSettings.Value;
            _converter = converter;
       
            _pdfFileNaming = pdfFileNaming.Value;
        }

        public void Add<T>(T entity) where T : class
        {
            _EngineCoreDBContext.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _EngineCoreDBContext.Remove(entity);
        }

        public async Task<bool> FindLanguage(string lang)
        {
            bool found = true;
            var result = await _EngineCoreDBContext.SysLanguage.Where(x => x.Lang == lang).ToListAsync();
            if (result.Count == 0)

                found = false;

            return found;
        }

        public async Task<bool> FindShortCut(string ShortCut)
        {
            bool found = true;
            var result = await _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut == ShortCut).ToListAsync();
            if (result.Count == 0) found = false;

            return found;
        }


        public string GenerateShortCut(string tableName, string columnName)
        {
            string shortCut = tableName + "_" + columnName + this.GetNewValueBySec().ToString();
            return shortCut;
        }

        public Dictionary<string, string> GenerateShortCutForAllPropertiesModel(string model)
        {
            Object TargetClass = null;
            Dictionary<string, string> MyDictionary = new Dictionary<string, string>();
            /////////////////////////////////////generate next value
            Type TargetType = Type.GetType("EngineCoreProject.Models." + model, true);
            TargetClass = (Activator.CreateInstance(TargetType));

            PropertyInfo[] Properties = TargetType.GetProperties(BindingFlags.Public);
            foreach (PropertyInfo Property in Properties)
            {
                var p = new SqlParameter("@result", System.Data.SqlDbType.Int);
                p.Direction = System.Data.ParameterDirection.Output;
                _EngineCoreDBContext.Database.ExecuteSqlRaw("set @result = next value for seq", p);
                int sequenceNum = (int)p.Value;
                string shortcut = model + "_" + Property.Name + "_" + sequenceNum;
                MyDictionary.Add(Property.Name, shortcut);

            }

            return MyDictionary;

        }

        public int GetNewValueBySec()
        {
            var p = new SqlParameter("@result", System.Data.SqlDbType.Int);
            p.Direction = System.Data.ParameterDirection.Output;
            _EngineCoreDBContext.Database.ExecuteSqlRaw("set @result = next value for seq", p);
            int sequenceNum = (int)p.Value;
            return sequenceNum;
        }

        public int GetNewValueForSMSBySec()
        {
            var p = new SqlParameter("@result", System.Data.SqlDbType.Int);
            p.Direction = System.Data.ParameterDirection.Output;
            _EngineCoreDBContext.Database.ExecuteSqlRaw("set @result = next value for SequenceForMessageRequestId", p);
            int sequenceNum = (int)p.Value;
            return sequenceNum;
        }

        public string getTranslateByIdFromLookUpValueId(string lang, int id)
        {


            var query = from slt in _EngineCoreDBContext.SysLookupValue

                        join st in _EngineCoreDBContext.SysTranslation on slt.Shortcut equals st.Shortcut


                        where (slt.Id == id && st.Lang == lang)

                        select st.Value;
            if (query.Count() == 0) return "";
            return query.FirstOrDefault().ToString();
        }

        public async Task<string> GetTranslateByShortCut(string lang, string shortCut)
        {
            string res = "";
            var query = (from translate in _EngineCoreDBContext.SysTranslation
                         where translate.Shortcut == shortCut && translate.Lang == lang
                         select translate.Value);
            if (query != null)
            {
                res = await query.FirstOrDefaultAsync();
            }

            return res;
        }

        public async Task<List<SysTranslation>> insertListTranslationAsync(List<TranslationDto> TranslationList)
        {
            List<SysTranslation> ListSysTranslation = new List<SysTranslation>();
            foreach (TranslationDto translationDt in TranslationList)
            {
                SysTranslation sysTranslation = new SysTranslation();
                sysTranslation.Lang = translationDt.Lang;
                sysTranslation.Shortcut = translationDt.Shortcut;
                sysTranslation.Value = translationDt.Value;

                SysTranslation translations = await _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut == translationDt.Shortcut && x.Lang == translationDt.Lang).FirstOrDefaultAsync();


                if (translations != null)
                {
                    translations.Value = translationDt.Value;
                    _EngineCoreDBContext.Update(translations);
                    _EngineCoreDBContext.SaveChanges();
                    ListSysTranslation.Add(translations);

                }

                else
                {

                    _EngineCoreDBContext.Add(sysTranslation);
                    _EngineCoreDBContext.SaveChanges();
                    ListSysTranslation.Add(sysTranslation);
                }


            }

            return ListSysTranslation;
        }

        public async Task<List<SysTranslation>> InsertUpdateSingleTranslation(string shortCut, Dictionary<string, string> TranslationDictionary)
        {
            List<SysTranslation> ListSysTranslation = new List<SysTranslation>();

            var oldTranslations = await _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut == shortCut).ToListAsync();
            if (oldTranslations.Count != 0)
            {
                _EngineCoreDBContext.SysTranslation.RemoveRange(oldTranslations);
            }

            foreach (var langVal in TranslationDictionary)
            {
                SysTranslation sysTranslation = new SysTranslation
                {
                    Lang = langVal.Key,
                    Shortcut = shortCut,
                    Value = langVal.Value
                };

                ListSysTranslation.Add(sysTranslation);
            }

            await _EngineCoreDBContext.SysTranslation.AddRangeAsync(ListSysTranslation);
            await _EngineCoreDBContext.SaveChangesAsync();

            return ListSysTranslation;
        }

        public async Task<List<Dictionary<string, string>>> insertDictionaryTranslationAsync(string shortCut, List<Dictionary<string, string>> TranslationDictionary)//key lang , value translate value
        {
            List<Dictionary<string, string>> ListSysTranslation = new List<Dictionary<string, string>>();
            //if (!await this.FindShortCut(shortCut))
            //{
            //    Dictionary<string, string> result = new Dictionary<string, string>();
            //    result.Add("error shortcut", shortCut + " : this lashortCutnguage not found ");
            //    ListSysTranslation.Add(result);
            //    return ListSysTranslation;

            //}
            foreach (Dictionary<string, string> entry in TranslationDictionary)//key lang , value translate value
            {

                foreach (KeyValuePair<string, string> subentry in entry)//key lang , value translate value
                {


                    Dictionary<string, string> result = new Dictionary<string, string>();

                    if (!await this.FindLanguage(subentry.Key))
                    {
                        result.Add("error language", subentry.Key + " :this language not found ");
                        ListSysTranslation.Add(result);
                        continue;

                    }

                    SysTranslation sysTranslationEdited = await _EngineCoreDBContext.SysTranslation.Where(x => x.Lang == subentry.Key && x.Shortcut == shortCut).FirstOrDefaultAsync();

                    if (sysTranslationEdited != null)
                    {

                        sysTranslationEdited.Value = subentry.Value;


                        _EngineCoreDBContext.Update(sysTranslationEdited);
                        if (_EngineCoreDBContext.SaveChanges() > 0)
                            result.Add(sysTranslationEdited.Lang, sysTranslationEdited.Shortcut + " updated it's translation as " + sysTranslationEdited.Value + " in " + sysTranslationEdited.Lang + " language ");
                        else result.Add(sysTranslationEdited.Lang, "couldn't update translation for " + sysTranslationEdited.Shortcut + "  as " + sysTranslationEdited.Value + " in " + sysTranslationEdited.Lang + " language ");


                    }

                    else
                    {

                        SysTranslation sysTranslation = new SysTranslation();
                        sysTranslation.Lang = subentry.Key;
                        sysTranslation.Shortcut = shortCut;
                        sysTranslation.Value = subentry.Value;


                        _EngineCoreDBContext.Add(sysTranslation);
                        if (_EngineCoreDBContext.SaveChanges() > 0) result.Add(sysTranslation.Lang, sysTranslation.Shortcut + " translated as " + sysTranslation.Value + " in " + sysTranslation.Lang + " language ");
                        else result.Add(sysTranslation.Lang, "couldn't translate " + sysTranslation.Shortcut + "  as " + sysTranslation.Value + " in " + sysTranslation.Lang + " language ");
                    }
                    ListSysTranslation.Add(result);




                }
            }
            return ListSysTranslation;
        }


        public async Task<AddNewTransResult> insertTrans(int inserttype, string target, string lang, string value)
        {
            AddNewTransResult newTrans = null;

            Dictionary<string, string> MyDictionary = new Dictionary<string, string>();

            List<string> ModelList = new List<string> {
                "Action",
                "AdmService",
                "AdmServiceStage",
                "AdmServiceStageAction",
                "Application",
                "ApplicationAttachment",
                "ApplicationNatoryView",
                "ApplicationParty",
                "Country",
                "DocumentStorage",
                "DocumentType",
                "EngineCoreDBContext",
                "FileConfiguration",
                "MasterAttachment",
                "NotaryEmail",
                "NotaryMessage",
                "NotaryPlace",
                "Notification",
                "NotificationTemplate",
                "NotificationTemplateAction",
                "NotificationTempleteAction",
                "NotificationType",
                "PartyTypeTodelete",
                "Payment",
                "Role",
                "Serviceinfo",
                "Serviceinfobyid",
                "SysLookupType",
                "SysTranslation",
                "SysLookupValue",
                "SysyLanguage",
                "TableName",
                "Template",
                "TemplateParty",
                "Term",
                "User",
                "WordModel",
            };
            List<string> Summary = new List<string>();


            List<string> currentLang = await _EngineCoreDBContext.SysLanguage.Select(x => x.Lang).ToListAsync();
            if (!currentLang.Any(str => str.Contains(lang)))
            {
                Summary.Add(lang + " is not avalible language");
                newTrans = new AddNewTransResult(Summary, MyDictionary);
                return newTrans;
            }


            string shortCut = null;
            ////////////////////////////////////////////////GET NEXT VALUE///////////////////////////////////
            var p = new SqlParameter("@result", System.Data.SqlDbType.Int);
            p.Direction = System.Data.ParameterDirection.Output;
            _EngineCoreDBContext.Database.ExecuteSqlRaw("set @result = next value for seq", p);
            int sequenceNum = (int)p.Value;
            ///////////////////////////////////////////////FINISH GET NEXT VALUE /////////////////////////////            
            ///////////////////////////////GET SHORTCUT////////////////////////
            if (inserttype == 1)
            {
                try
                {
                    Convert.ToInt32(target);
                }
                catch (Exception)
                {
                    Summary.Add("ID (" + target + ") isn't integer value, please enter correct ID from sys_lookup_type ");
                    newTrans = new AddNewTransResult(Summary, MyDictionary);
                    return newTrans;
                }

                var result = from SysLookupType in _EngineCoreDBContext.SysLookupType
                             where SysLookupType.Id == Convert.ToInt32(target)
                             select new
                             {
                                 SysLookupType.Value
                             };

                if (!result.Any())
                {
                    Summary.Add("ID (" + target + ") isn't found in sys_Lookup_Type ");
                    newTrans = new AddNewTransResult(Summary, MyDictionary);
                    return newTrans;
                }
                shortCut = await result.Select(x => x.Value).FirstOrDefaultAsync(); //get shortcut for type

                Summary.Add("you get shortCut for type: (" + shortCut + ") from sys_Lookup_Type successfully");

            }
            else
            {

                if (!ModelList.Any(str => str.Contains(target)))
                {
                    Summary.Add(target + " is not a table");
                    newTrans = new AddNewTransResult(Summary, MyDictionary);
                    return newTrans;
                }


                shortCut = target;
                Summary.Add("you get shortCut for table: (" + target + ")   successfully");
            }

            shortCut = shortCut + sequenceNum;

            Summary.Add("shortCut (" + shortCut + ") for new value was generated successfully");
            MyDictionary.Add("shortcut", shortCut);
            ////////////////////////////////END GET SHORTCUT////////////////////////

            //////////////////////////////TRY TO INSERT INTO TARTGET TABLE////////////////////////
            ///
            Object TargetClass = null;
            if (inserttype == 1)
            {
                SysLookupValue newValue = new SysLookupValue
                {
                    LookupTypeId = Convert.ToInt32(target),
                    Shortcut = shortCut
                };

                _EngineCoreDBContext.Add(newValue);
                if (_EngineCoreDBContext.SaveChanges() >= 0)
                {
                    Summary.Add("you added row to sys_lookup_value table successfuly");


                    MyDictionary.Add("Id", newValue.Id.ToString());
                }

            }

            else
            {
                Type TargetType = Type.GetType("EngineCoreProject.Models." + target, true);
                TargetClass = (Activator.CreateInstance(TargetType));
                string val = shortCut;
                PropertyInfo propertyInfo = TargetClass.GetType().GetProperty("Shortcut");
                propertyInfo.SetValue(TargetClass, Convert.ChangeType(val, propertyInfo.PropertyType), null);

                _EngineCoreDBContext.Add(TargetClass);
                if (_EngineCoreDBContext.SaveChanges() >= 0)
                {
                    Summary.Add("you added row to " + target + " table successfuly");
                    propertyInfo = TargetClass.GetType().GetProperty("Id");
                    propertyInfo.GetValue(TargetClass, null);
                    MyDictionary.Add("Id", propertyInfo.GetValue(TargetClass, null).ToString());

                }
                else
                {
                    Summary.Add("you couldn't add row to target table successfuly,, error occured");
                    newTrans = new AddNewTransResult(Summary, MyDictionary);
                    return newTrans;
                }
            }


            //////////////////////////////FINISH TRY TO INSERT INTO TARTGET TABLE////////////////////////
            ////////////////////////////////TRY TO INSERT INTO TRANSLATION TABLE////////////////////////
            List<SysLanguage> CurrentLanguage = await _EngineCoreDBContext.SysLanguage.ToListAsync();
            foreach (SysLanguage language in CurrentLanguage)
            {
                SysTranslation newTranslation = new SysTranslation() { Lang = language.Lang, Shortcut = shortCut, Value = value };
                _EngineCoreDBContext.SysTranslation.Add(newTranslation);
                if (_EngineCoreDBContext.SaveChanges() >= 0)
                {
                    Summary.Add("you add (" + value + ") to your " + language.Lang + " dictionary successfuly");
                }
                else
                {
                    Summary.Add("you couldn't add (" + value + ") to your " + language.Lang + " dictionary successfuly, error ocurred");
                    newTrans = new AddNewTransResult(Summary, MyDictionary);
                    return newTrans;
                }

            }


            ////////////////////////////////FINISH TRY TO INSERT INTO TRANSLATION TABLE////////////////////////

            newTrans = new AddNewTransResult(Summary, MyDictionary);
            return newTrans;

        }

        public async Task<Dictionary<string, string>> getTranslationsForShortCut(string shortCut)
        {
            Dictionary<string, string> MyDictionary = new Dictionary<string, string>();
            List<SysTranslation> translations = await _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut == shortCut).ToListAsync();

            if (translations != null)
            {
                foreach (SysTranslation tran in translations)
                {
                    MyDictionary.Add(tran.Lang, tran.Value);
                }
            }
            return MyDictionary;
        }

        public async Task<AddNewTransResult> insertTranslation(string shortcut, string value, string currentLanguage)
        {
            AddNewTransResult newTrans = null;
            Dictionary<string, string> MyDictionary = new Dictionary<string, string>();
            List<string> Summary = new List<string>();
            MyDictionary.Add("shortcut", shortcut);

            SysTranslation tran = await _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut == shortcut & x.Lang == currentLanguage).FirstOrDefaultAsync();

            if (tran != null)
            {
                tran.Value = value;
                _EngineCoreDBContext.SysTranslation.Update(tran);
                if (_EngineCoreDBContext.SaveChanges() >= 0)
                {
                    Summary.Add("you update (" + value + ") to your " + currentLanguage + " dictionary successfuly");
                    MyDictionary.Add(currentLanguage, value);
                    newTrans = new AddNewTransResult(Summary, MyDictionary);
                    return newTrans;
                }
                else
                {
                    Summary.Add("you couldn't update (" + value + ") to your " + currentLanguage + " dictionary successfuly, error ocurred");
                    newTrans = new AddNewTransResult(Summary, MyDictionary);
                    return newTrans;
                }

            }
            else
            {








                List<SysLanguage> CurrentLanguage = await _EngineCoreDBContext.SysLanguage.ToListAsync();
                foreach (SysLanguage language in CurrentLanguage)
                {
                    SysTranslation newTranslation = new SysTranslation() { Lang = language.Lang, Shortcut = shortcut, Value = value };
                    _EngineCoreDBContext.SysTranslation.Add(newTranslation);
                    if (_EngineCoreDBContext.SaveChanges() >= 0)
                    {
                        Summary.Add("you add (" + value + ") to your " + language.Lang + " dictionary successfuly");
                        MyDictionary.Add(language.Lang, value);

                    }
                    else
                    {
                        Summary.Add("you couldn't add (" + value + ") to your " + language.Lang + " dictionary successfuly, error ocurred");

                    }

                }




                newTrans = new AddNewTransResult(Summary, MyDictionary);
                return newTrans;
            }
        }

        public async Task<bool> Save()
        {
            return (await _EngineCoreDBContext.SaveChangesAsync()) >= 0;
        }

        public void Update<T>(T entity) where T : class
        {
            _EngineCoreDBContext.Update(entity);
        }

        public async Task<SysTranslation> updateTranslation(string lang, string value, string shortCut)
        {
            SysTranslation translations = await _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut == shortCut && x.Lang == lang).FirstOrDefaultAsync();

            translations.Value = value;

            _EngineCoreDBContext.Update(translations);
            if (_EngineCoreDBContext.SaveChanges() >= 0)
            {
                return translations;
            }
            else
            {
                return null;
            }
        }


        public async Task<int> DeleteTranslation(string shortCut)
        {
            List<SysTranslation> translations = await _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut == shortCut).ToListAsync();
            _EngineCoreDBContext.SysTranslation.RemoveRange(translations);
            if (_EngineCoreDBContext.SaveChanges() >= 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }


        public string SecureHashGenerationHMACSHA256(string StringForHash, string Securekey)
        {
            try
            {
                byte[] stringAfterEncodingByte = null;
                byte[] keydSalt = System.Text.Encoding.UTF8.GetBytes(Securekey);
                byte[] StringForHashbyte = System.Text.Encoding.UTF8.GetBytes(StringForHash);
                using (var hmac = new System.Security.Cryptography.HMACSHA256(keydSalt))
                {
                    stringAfterEncodingByte = hmac.ComputeHash(StringForHashbyte);
                }
                string hashString = ToHex(stringAfterEncodingByte, false);
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(hashString);
                string stringAfterEncodingBase64 = System.Convert.ToBase64String(plainTextBytes);
                return stringAfterEncodingBase64;
            }
            catch
            {
                return "";
            }
        }
        private string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
            return result.ToString();
        }
        
        public int? CalculateFee(int serviceId)
        {
            int? result = _EngineCoreDBContext.AdmStage.Where(x => x.ServiceId == serviceId).Sum(x => x.Fee);
            return result;
        }

        public string GetDecviceInfo(string userAgent)
        {
            DeviceDetectorNET.DeviceDetector.SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);

            var result = DeviceDetectorNET.DeviceDetector.GetInfoFromUserAgent(userAgent);
            var output = result.Success ? result.Match.DeviceType: "Unknown";
            var id = _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut.Contains(output.ToLower())).Select(y => y.Id).FirstOrDefault();
            return output;
        }

        public async Task<List<AllTranslationDto>> GetAllTranslation(string shortcut)
        {
            Task<List<AllTranslationDto>> query = null;
            query = (from t in _EngineCoreDBContext.SysTranslation
                     where t.Shortcut == shortcut
                     select new AllTranslationDto
                     {
                         shortcut = t.Shortcut,
                         lang = t.Lang,
                         translate = t.Value
                     }).ToListAsync();

            return await query;
        }

        public string GenerateURL(Dictionary<string, string> DictionaryQueryString, string URL)
        {
            string URlString = null;
            var array = (
                   from key in DictionaryQueryString.Keys
                   select string.Format(
                       "{0}={1}",
                       key,
                      DictionaryQueryString[key])
                   ).ToArray();
            URlString = string.Join("&", array);
            URlString = URL + "?" + HttpUtility.UrlPathEncode(URlString);
            URlString = HttpUtility.UrlPathEncode(URlString);
            return URlString;
        }
        public string ConvertFromHTMLTpPDF(string HTML)
        {
            string FileName = _pdfFileNaming.TermPdfFileName + GetNewValueBySec() + ".pdf";
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report",
                Out = @"wwwroot/pdfTerm/" + FileName
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = HTML,
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var file = _converter.Convert(pdf);
            return "pdfTerm/" + FileName;
        }



        public async Task<string> getServiceNameTranslateAsync(string lang, int? PaymentId)
        {
            int? ServiceId = await _EngineCoreDBContext.Payment.Where(x => x.Id == PaymentId).Select(x => x.ServiceId).FirstOrDefaultAsync();
            string ServiceShortCut = await _EngineCoreDBContext.AdmService.Where(x => x.Id == ServiceId).Select(x => x.Shortcut).FirstOrDefaultAsync();
            string TranslateServiceName = await GetTranslateByShortCut(lang, ServiceShortCut);


            return TranslateServiceName;
        }



        public async Task<string> getServiceNameTranslateByAppIdAsync(string lang, int? applicationId)
        {
            int? ServiceId = await _EngineCoreDBContext.Application.Where(x => x.Id == applicationId).Select(x => x.ServiceId).FirstOrDefaultAsync();
            string ServiceShortCut = await _EngineCoreDBContext.AdmService.Where(x => x.Id == ServiceId).Select(x => x.Shortcut).FirstOrDefaultAsync();
            string TranslateServiceName = await GetTranslateByShortCut(lang, ServiceShortCut);


            return TranslateServiceName;
        }

        public int GetNextSecForPayment()
        {

            var p = new SqlParameter("@result", System.Data.SqlDbType.Int);
            p.Direction = System.Data.ParameterDirection.Output;
            _EngineCoreDBContext.Database.ExecuteSqlRaw("set @result = next value for SeqForPayment", p);
            int sequenceNum = (int)p.Value;
            return sequenceNum;
        }

        public int AppDevice(string userAgent)
        {
            var query = (
                from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == 8077)
                //join tr in _EngineCoreDBContext.SysTranslation
                //on lv.Shortcut equals tr.Shortcut
                //where tr.Lang == "en"

                select new
                {
                    id = lv.Id,
                    channelname = lv.Shortcut
                });

            DeviceDetectorNET.DeviceDetector.SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);

            var result = DeviceDetectorNET.DeviceDetector.GetInfoFromUserAgent(userAgent);
            var output = result.Success ? result.Match.DeviceType : "Unknown";
            var id = query.Where(x => x.channelname.ToLower().Contains(output.ToLower())).Select(z => z.id).FirstOrDefault();

            if (id == null || id==0)
                return query.Where(x => x.channelname.ToLower().Contains("desktop")).Select(z => z.id).FirstOrDefault(); //11154;
            else
                return id;
        }

        public async Task<ClassDto> getShortCutId(string shortcut)
        {
            Dictionary<string, string> MyDictionary = new Dictionary<string, string>();
            List<SysTranslation> translations = await _EngineCoreDBContext.SysTranslation.Where(x => x.Shortcut.ToLower() == shortcut.ToLower()).ToListAsync();
            int id = _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut.ToLower() == shortcut.ToLower()).Select(y => y.Id).FirstOrDefault();
            if (translations != null)
            {
                foreach (SysTranslation tran in translations)
                {
                    MyDictionary.Add(tran.Lang, tran.Value);
                }
            }
            var result = new ClassDto
            {
                id = id,
                mydic = MyDictionary
            };
            return result;
        }

        public int TypeofSign(int id)
        {
            int result = 0;
            int sum = 0;
            // applicationParty = new List<ApplicationParty>();
            List<ApplicationParty> applicationParty = _EngineCoreDBContext.ApplicationParty.Where(x => x.TransactionId == id && x.SignRequired == true).ToList();
            if (applicationParty.Count() > 0)
            {
                foreach (var par in applicationParty)
                {
                    sum += (int)par.SignType;

                }
                if (applicationParty.Count() == sum)
                {
                    result = 1;
                }
                else if (applicationParty.Count() * 2 == sum)
                {
                    result = 2;
                }
                else
                {
                    result = 3;
                }
            }
            return result;
        }

       
    }
}
