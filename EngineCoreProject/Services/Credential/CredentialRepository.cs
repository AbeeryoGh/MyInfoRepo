using EngineCoreProject.DTOs.Credential;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.ICredential;
using EngineCoreProject.IRepository.IFilesUploader;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using EngineCoreProject.IRepository.IGeneratorRepository;

namespace EngineCoreProject.Services.Credential
{
    public class CredentialRepository : ICredentialRepository

    {
        
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly ISysValueRepository _ISysValueRepository;
        ValidatorException _exception;
        private readonly IFilesUploaderRepositiory _iFilesUploaderRepositiory;
     

        public CredentialRepository(IFilesUploaderRepositiory iFilesUploaderRepositiory,ISysValueRepository iSysValueRepository, EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)

        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _ISysValueRepository = iSysValueRepository;
            _exception = new ValidatorException();
            _iFilesUploaderRepositiory = iFilesUploaderRepositiory;
            
        }

      
        public async Task<object> Prefetch(PrefetchRequestDto prefetchRequestDto)
        {
            int oldTemplateId = _EngineCoreDBContext.Template.Where(x => x.TitleShortcut.Contains("old_template")).Select(z => z.Id).FirstOrDefault();
            List<PrefetchResponseDto> query = new List<PrefetchResponseDto>();
            UpdateVCIDResDto updateVCIDResDto = new UpdateVCIDResDto();
            if (prefetchRequestDto.EmiratesID == "" || prefetchRequestDto.EmiratesID==null)
            {
                updateVCIDResDto.statusCode = "-3";
                updateVCIDResDto.statusMessage = "Emirates ID is required";
                return updateVCIDResDto;
            }
            else
                if (prefetchRequestDto.TransactionRefNo==""||prefetchRequestDto.TransactionRefNo==null)
            {
                updateVCIDResDto.statusCode = "-2";
                updateVCIDResDto.statusMessage = "Transaction reference number is required" ;
                return updateVCIDResDto;
              
            }


            SysLookupType DocumentType =  _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "document_type").FirstOrDefault();
            int documentTypeId = DocumentType.Id;
            int POAId = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x=>x.LookupTypeId==documentTypeId)
                         join tr in _EngineCoreDBContext.SysTranslation on lv.Shortcut equals tr.Shortcut
                         where tr.Value.Contains("Power of Attorney")
                         select new {id=lv.Id}
                ).FirstOrDefault().id;

            SysLookupType DocumentType1 = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "transaction_state").FirstOrDefault();
            int transaction_state_id = DocumentType1.Id;

            int transaction_state_cancel_id = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == transaction_state_id && x.Shortcut== "CANCELED")


                         select new { id = lv.Id }
               ).FirstOrDefault().id;

            query = (from par in _EngineCoreDBContext.ApplicationParty.Where(x => x.EmiratesIdNo.Contains(prefetchRequestDto.EmiratesID))
                         join tranc in _EngineCoreDBContext.AppTransaction.Where(x=>x.TransactionNo!=null && x.TransactionStatus!= transaction_state_cancel_id && x.DocumentUrl!=null) on par.TransactionId equals tranc.Id
                         join app in _EngineCoreDBContext.Application on tranc.ApplicationId equals app.Id
                         join temp in _EngineCoreDBContext.Template.Where(x=>x.DocumentTypeId==POAId || x.Id==9328) on app.TemplateId equals temp.Id
                         select new PrefetchResponseDto
                         {
                             //appNo = app.ApplicationNo,
                             contractNo = tranc.TransactionNo,
                             contractDate = tranc.CreatedDate.HasValue?Convert.ToDateTime(tranc.CreatedDate).ToString("dd MMM yyyy"):""
                         }).ToList() ;

            return query; 

        }

        public async Task<object> RequestCredentials(RequestCredentialsReqDto requestCredentialsReqDto)
        {
            
            var attachtranslateAR = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == 3017)
                                   join tr in _EngineCoreDBContext.SysTranslation on lv.Shortcut equals tr.Shortcut
                                     where tr.Lang == "ar"
                                     select new
                                     {
                                         attachId=lv.Id,
                                         tr.Value
                                     });
            var attachtranslateEN = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == 3017)
                                   join tr in _EngineCoreDBContext.SysTranslation on lv.Shortcut equals tr.Shortcut
                                     where tr.Lang == "en"
                                     select new
                                     {
                                         attachId = lv.Id,
                                         tr.Value
                                     });
            SysLookupType DocumentType1 = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "transaction_state").FirstOrDefault();
            int transaction_state_id = DocumentType1.Id;

            int transaction_state_cancel_id = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == transaction_state_id && x.Shortcut == "CANCELED")


                                               select new { id = lv.Id }
               ).FirstOrDefault().id;

            UpdateVCIDResDto updateVCIDResDto = new UpdateVCIDResDto();

            if (requestCredentialsReqDto.VCID == "") requestCredentialsReqDto.VCID = null;
            if (requestCredentialsReqDto.TransactionRefNo == "") requestCredentialsReqDto.TransactionRefNo = null;

            if (requestCredentialsReqDto.TransactionRefNo == null)
            {
                updateVCIDResDto.statusCode = "-2";
                updateVCIDResDto.statusMessage = "Transaction reference number is required";
                return updateVCIDResDto;
            }

            if (requestCredentialsReqDto.VCID==null && (requestCredentialsReqDto.requestedData.contractNo==null))
            {
                updateVCIDResDto.statusCode = "-11";
                updateVCIDResDto.statusMessage = "requestedData Or vcID is required";
                return updateVCIDResDto;
            }

            if (requestCredentialsReqDto.VCID != null && requestCredentialsReqDto.requestedData.contractNo != null)
            {
                AppTransaction appTransaction = _EngineCoreDBContext.AppTransaction.Where(x => x.Vcid==requestCredentialsReqDto.VCID &&
                                                                                           x.TransactionNo== requestCredentialsReqDto.requestedData.contractNo && x.TransactionStatus!= transaction_state_cancel_id).FirstOrDefault();
                if (appTransaction == null)
                {
                    updateVCIDResDto.statusCode = "-12";
                    updateVCIDResDto.statusMessage = "Unable to find the application Specified";
                    return updateVCIDResDto;
                }
             
            }
            else

            if (requestCredentialsReqDto.VCID != null && requestCredentialsReqDto.requestedData.contractNo == null)
            {
               AppTransaction appTransaction =  _EngineCoreDBContext.AppTransaction.Where(x => x.Vcid==requestCredentialsReqDto.VCID && x.TransactionStatus!= transaction_state_cancel_id).FirstOrDefault();
                if (appTransaction == null)
                {
                    updateVCIDResDto.statusCode = "-12";
                    updateVCIDResDto.statusMessage = "Unable to find the application Specified";
                    return updateVCIDResDto;
                }
             

            }
            else

             if (requestCredentialsReqDto.VCID == null && requestCredentialsReqDto.requestedData.contractNo != null)
            {
                AppTransaction appTransaction = _EngineCoreDBContext.AppTransaction.Where(x => x.TransactionNo == requestCredentialsReqDto.requestedData.contractNo && x.TransactionStatus!= transaction_state_cancel_id).FirstOrDefault();
                if (appTransaction == null)
                {
                    updateVCIDResDto.statusCode = "-12";
                    updateVCIDResDto.statusMessage = "Unable to find the application Specified";
                    return updateVCIDResDto;
                }
              

                }
            

            

            RequestCredentialsResDto requestCredentialsResDto = new RequestCredentialsResDto();
            if (requestCredentialsReqDto.dataOnly == "Y")
            {
               


                var query =
                  (from tranc in _EngineCoreDBContext.AppTransaction.Where(x => (x.TransactionNo == requestCredentialsReqDto.requestedData.contractNo || requestCredentialsReqDto.requestedData.contractNo == null)
                   && (x.Vcid==requestCredentialsReqDto.VCID || requestCredentialsReqDto.VCID==null)&& x.TransactionStatus!= transaction_state_cancel_id)
                   join app in _EngineCoreDBContext.Application on tranc.ApplicationId equals app.Id
                   join par in _EngineCoreDBContext.ApplicationParty.Where(x => x.IsOwner == true) on tranc.Id equals par.TransactionId
                   join pay in _EngineCoreDBContext.Payment on app.Id equals pay.ApplicationId
                  

                   select new RequestCredentialsResDto
                   {

                       credentialExpiryDate = tranc.TransactionEndDate == null ? "" : Convert.ToDateTime(tranc.TransactionEndDate).ToString("yyyy-MM-dd"),
                       claim = (new claim
                       {
                           appNo = app.ApplicationNo,
                           appNoLabelAr = "رقم الطلب",
                           appNoLabelEN = "Application No",
                           contractNo = tranc.TransactionNo,
                           contractNoLabelAr = "رقم المعاملة",
                           contractNoLabelEn = "Contract No",
                           contractDate = tranc.CreatedDate.HasValue? Convert.ToDateTime(tranc.CreatedDate).ToString("dd MMM yyyy"):"",
                           contractDateLabelAr = "تاريخ المعاملة",
                           contractDateLabelEn = "Contract Date",
                           contractStartDate = tranc.TransactionStartDate.HasValue ? Convert.ToDateTime(tranc.TransactionStartDate).ToString("dd MMM yyyy") : "",
                           contractStartDateLabelAr = "تاريخ البدء ",
                           contractStartDateLabelEn="Start Date",
                           contractEndtDate = tranc.TransactionEndDate.HasValue ? Convert.ToDateTime(tranc.TransactionEndDate).ToString("dd MMM yyyy") : "",
                           contractEndtDateLabelAr = "تاريخ الانتهاء ",
                           contractEndDateLabelEn = "End Date",
                           templateNameAr = app.TemplateId == null ? null : (from temp in _EngineCoreDBContext.Template.Where(x => x.Id == app.TemplateId)
                                                                             join tr in _EngineCoreDBContext.SysTranslation on temp.TitleShortcut equals tr.Shortcut
                                                                             where tr.Lang == "ar"
                                                                             select new { templateAr = tr.Value }
                                                                                                                ).FirstOrDefault().templateAr,
                           templateNameEn = app.TemplateId == null ? null : (from temp in _EngineCoreDBContext.Template.Where(x => x.Id == app.TemplateId)
                                                                             join tr in _EngineCoreDBContext.SysTranslation on temp.TitleShortcut equals tr.Shortcut
                                                                             where tr.Lang == "en"
                                                                             select new { templateEn = tr.Value }
                                                                                                                ).FirstOrDefault().templateEn,
                           templateNameLabelAr = "اسم القالب",
                           templateNameLabelEn = "Template Name",
                           fee = pay.ActualPaid.ToString(),
                           feeLabelAr = "قيمة الرسوم",
                           feeLabelEN = "Fee Value",
                           invoiceNo = pay.InvoiceNo,
                           invoiceNoLabelAr = "رقم الفاتورة",
                           invoiceNoLabelEn = "Invoice No",
                           invoiceDate = pay.LastUpdatedDate.HasValue ? Convert.ToDateTime(pay.LastUpdatedDate).ToString("dd MMM yyyy") : "",
                           invoiceDateLabelAr = "تاريخ الفاتورة",
                           invoiceDateLabelEn = "Invoice Date",

                       }),
                       evidence = (new evidence
                       {
                           file = pdfToBase64(Path.Combine(_iFilesUploaderRepositiory.GetRootPath(), tranc.DocumentUrl)),
                           type = "pdf"
                       }),
                       keyHash = (new keyHash
                       {
                           appNo=app.ApplicationNo,
                           contractNo = tranc.TransactionNo,
                           contractStartDate = tranc.TransactionStartDate.HasValue ? Convert.ToDateTime(tranc.TransactionStartDate).ToString("dd MMM yyyy") : "",
                           contractEndtDate = tranc.TransactionEndDate.HasValue ? Convert.ToDateTime(tranc.TransactionEndDate).ToString("dd MMM yyyy") : "",
                           contractDate = tranc.CreatedDate.HasValue ? Convert.ToDateTime(tranc.CreatedDate).ToString("dd MMM yyyy") : "",
                           templateNameAr = app.TemplateId == null ? null : (from temp in _EngineCoreDBContext.Template.Where(x => x.Id == app.TemplateId)
                                                                             join tr in _EngineCoreDBContext.SysTranslation on temp.TitleShortcut equals tr.Shortcut
                                                                             where tr.Lang == "ar"
                                                                             select new { templateAr = tr.Value }
                                                                                                                ).FirstOrDefault().templateAr,
                           templateNameEn = app.TemplateId == null ? null : (from temp in _EngineCoreDBContext.Template.Where(x => x.Id == app.TemplateId)
                                                                             join tr in _EngineCoreDBContext.SysTranslation on temp.TitleShortcut equals tr.Shortcut
                                                                             where tr.Lang == "en"
                                                                             select new { templateEn = tr.Value }
                                                                                                                ).FirstOrDefault().templateEn,
                           fee = pay.ActualPaid.ToString(),
                           invoiceNo = pay.InvoiceNo,
                           invoiceDate = pay.LastUpdatedDate.HasValue ? Convert.ToDateTime(pay.LastUpdatedDate).ToString("dd MMM yyyy") : "",
                       })


                   }) ;
                int TransactionId = (from q in query
                                     join tranc in _EngineCoreDBContext.AppTransaction on q.claim.contractNo equals tranc.TransactionNo
                                     select new
                                     {
                                         trancId = tranc.Id
                                     }).Select(x => x.trancId).FirstOrDefault();
                requestCredentialsResDto.credentialExpiryDate = query.Select(x => x.credentialExpiryDate).FirstOrDefault();
                requestCredentialsResDto.claim = query.Select(x => x.claim).FirstOrDefault();
                requestCredentialsResDto.keyHash = query.Select(x => x.keyHash).FirstOrDefault();
                requestCredentialsResDto.evidence = query.Select(x => x.evidence).FirstOrDefault();
                requestCredentialsResDto.claim.parties = (from par in _EngineCoreDBContext.ApplicationParty
                                                                                          .Where(x => x.TransactionId == TransactionId)
                                                                     select new CredentialsParty
                                                                     {
                                                                         partyName = par.FullName,
                                                                         partyNameLabelAr="اسم الطرف",
                                                                         partyNameLabelEn="Party Name",            
                                                                         nationalityAr = par.Nationality == null ? null : (from Country in _EngineCoreDBContext.Country
                                                                                where Country.UgId == par.Nationality
                                                                                select new { CountryAr = Country.CntCountryAr }
                                                                                                                           ).FirstOrDefault().CountryAr,
                                                                         nationalityEn = par.Nationality == null ? null : (from Country in _EngineCoreDBContext.Country
                                                                                                                           where Country.UgId == par.Nationality
                                                                                                                           select new { CountryEn = Country.CntCountryEn }
                                                                                                                           ).FirstOrDefault().CountryEn,
                                                                         nationalityLabelAr="الجنسية",
                                                                         nationalityLabelEn="Nationality",
                                                                         partyTypeAr=par.PartyTypeValueId==null?null: (from lv in _EngineCoreDBContext.SysLookupValue
                                                                                                                       join tr in _EngineCoreDBContext.SysTranslation
                                                                                                                       on lv.Shortcut equals tr.Shortcut
                                                                                                                       where tr.Lang=="ar"
                                                                                                                       select new { partytypear = tr.Value }
                                                                                                                           ).FirstOrDefault().partytypear,
                                                                         partyTypeEn = par.PartyTypeValueId == null ? null : (from lv in _EngineCoreDBContext.SysLookupValue
                                                                                                                              join tr in _EngineCoreDBContext.SysTranslation
                                                                                                                              on lv.Shortcut equals tr.Shortcut
                                                                                                                              where tr.Lang == "en"
                                                                                                                              select new { partytypeen = tr.Value }
                                                                                                                           ).FirstOrDefault().partytypeen,
                                                                         partyTypeLabelAr="نوع الطرف",
                                                                         partyTypeLabelEn="Party Type",
                                                                         idNumber=par.EmiratesIdNo ==null ? (from xpar in _EngineCoreDBContext.ApplicationPartyExtraAttachment
                                                                                                             where xpar.ApplicationPartyId==par.Id
                                                                                                             select new { number = xpar.Number }
                                                                                                                           ).FirstOrDefault().number: par.EmiratesIdNo,
                                                                         idNumberLabelAr="رقم الوثيقة",
                                                                         idNumberLabelEn= "Dcoument number",
                                                                         idDocTypeAr= par.EmiratesIdNo == null?attachtranslateAR.Where(x=>x.attachId.ToString()== (from xpar in _EngineCoreDBContext.ApplicationPartyExtraAttachment
                                                                                                                                                        where xpar.ApplicationPartyId == par.Id
                                                                                                                                                        select new { number = xpar.Number }
                                                                                                                           ).FirstOrDefault().number).Select(z=>z.Value).FirstOrDefault():"هوية إماراتية",
                                                                         idDocTypeEn = par.EmiratesIdNo == null ? attachtranslateEN.Where(x => x.attachId.ToString() == (from xpar in _EngineCoreDBContext.ApplicationPartyExtraAttachment
                                                                                                                                                                         where xpar.ApplicationPartyId == par.Id
                                                                                                                                                                         select new { number = xpar.Number }
                                                                                                                              ).FirstOrDefault().number).Select(z => z.Value).FirstOrDefault() : "EID",
                                                                         idDocTypeLabelAr ="نوع الوثيقة",
                                                                         idDocTypeLabelEn= "Document Type",

                                                                     }).ToList();




                AppTransaction appTransaction = _EngineCoreDBContext.AppTransaction.Where(x => x.TransactionNo == requestCredentialsResDto.claim.contractNo).FirstOrDefault();
                appTransaction.TransactionRefNo = requestCredentialsReqDto.TransactionRefNo;
                 _iGeneralRepository.Update(appTransaction);
                await _iGeneralRepository.Save();
            }
            else
            {//&& (x.TransactionRefNo.Contains(requestCredentialsReqDto.TransactionRefNo) || requestCredentialsReqDto.TransactionRefNo == null))
                var query =
                  (from tranc in _EngineCoreDBContext.AppTransaction.Where(x => (x.TransactionNo == requestCredentialsReqDto.requestedData.contractNo || requestCredentialsReqDto.requestedData.contractNo == null)&&
                   (x.Vcid==requestCredentialsReqDto.VCID || requestCredentialsReqDto.VCID==null)&&x.TransactionStatus!= transaction_state_cancel_id)
                   join app in _EngineCoreDBContext.Application on tranc.ApplicationId equals app.Id
                   join par in _EngineCoreDBContext.ApplicationParty.Where(x => x.IsOwner == true) on tranc.Id equals par.TransactionId
                   join pay in _EngineCoreDBContext.Payment on app.Id equals pay.ApplicationId
                   // where (tranc.TransactionNo == requestCredentialsReqDto.requestedData.contractNo || requestCredentialsReqDto.requestedData.contractNo == null) //&&
                   // (tranc.CreatedDate.ToString() == requestCredentialsReqDto.requestedData.contractDate || requestCredentialsReqDto.requestedData.contractDate == null)
                   select new RequestCredentialsResDto
                   {

                       credentialExpiryDate = tranc.TransactionEndDate == null ? "" : Convert.ToDateTime(tranc.TransactionEndDate).ToString("yyyy-MM-dd"),
                       claim = (new claim
                       {
                           appNo = app.ApplicationNo,
                           appNoLabelAr = "رقم الطلب",
                           appNoLabelEN = "Application No",
                           contractNo = tranc.TransactionNo,
                           contractNoLabelAr = "رقم المعاملة",
                           contractNoLabelEn = "Contract No",
                           contractDate = tranc.CreatedDate.HasValue ? Convert.ToDateTime(tranc.CreatedDate).ToString("dd MMM yyyy") : "",
                           contractDateLabelAr = "تاريخ المعاملة",
                           contractDateLabelEn = "Contract Date",
                           contractStartDate = tranc.TransactionStartDate.HasValue ? Convert.ToDateTime(tranc.TransactionStartDate).ToString("dd MMM yyyy") : "",
                           contractStartDateLabelAr = "تاريخ البدء ",
                           contractStartDateLabelEn = "Start Date",
                           contractEndtDate = tranc.TransactionEndDate.HasValue ? Convert.ToDateTime(tranc.TransactionEndDate).ToString("dd MMM yyyy") : "",
                           contractEndtDateLabelAr = "تاريخ الانتهاء ",
                           contractEndDateLabelEn = "End Date",
                           templateNameAr= app.TemplateId == null ? null : (from temp in _EngineCoreDBContext.Template.Where(x=> x.Id == app.TemplateId)
                                                                            join tr in _EngineCoreDBContext.SysTranslation on temp.TitleShortcut equals tr.Shortcut
                                                                            where tr.Lang=="ar"
                                                                                              select new { templateAr = tr.Value }
                                                                                                                ).FirstOrDefault().templateAr,
                           templateNameEn = app.TemplateId == null ? null : (from temp in _EngineCoreDBContext.Template.Where(x => x.Id == app.TemplateId)
                                                                             join tr in _EngineCoreDBContext.SysTranslation on temp.TitleShortcut equals tr.Shortcut
                                                                             where tr.Lang == "en"
                                                                             select new { templateEn = tr.Value }
                                                                                                                ).FirstOrDefault().templateEn,
                           templateNameLabelAr="اسم القالب",
                           templateNameLabelEn="Template Name",
                           fee =pay.ActualPaid.ToString(),
                           feeLabelAr="قيمة الرسوم",
                           feeLabelEN="Fee Value",
                           invoiceNo=pay.InvoiceNo,
                           invoiceNoLabelAr="رقم الفاتورة",
                           invoiceNoLabelEn="Invoice No",
                           invoiceDate=pay.LastUpdatedDate.HasValue ? Convert.ToDateTime(pay.LastUpdatedDate).ToString("dd MMM yyyy") : "",
                           invoiceDateLabelAr="تاريخ الفاتورة",
                           invoiceDateLabelEn="Invoice Date",

                       }),
                       evidence = (new evidence
                       {
                           file =pdfToBase64(Path.Combine(_iFilesUploaderRepositiory.GetRootPath(), tranc.DocumentUrl)),
                           type = "pdf"
                       }),
                       keyHash = (new keyHash
                       {
                           
                           contractNo = tranc.TransactionNo,
                           contractDate = tranc.CreatedDate.HasValue ? Convert.ToDateTime(tranc.CreatedDate).ToString("dd MMM yyyy") : "",
                           appNo = app.ApplicationNo, 
                           contractStartDate = tranc.TransactionStartDate.HasValue ? Convert.ToDateTime(tranc.TransactionStartDate).ToString("dd MMM yyyy") : "",
                           contractEndtDate = tranc.TransactionEndDate.HasValue ? Convert.ToDateTime(tranc.TransactionEndDate).ToString("dd MMM yyyy") : "",
                           templateNameAr = app.TemplateId == null ? null : (from temp in _EngineCoreDBContext.Template.Where(x => x.Id == app.TemplateId)
                                                                             join tr in _EngineCoreDBContext.SysTranslation on temp.TitleShortcut equals tr.Shortcut
                                                                             where tr.Lang == "ar"
                                                                             select new { templateAr = tr.Value }
                                                                                                                ).FirstOrDefault().templateAr,
                           templateNameEn = app.TemplateId == null ? null : (from temp in _EngineCoreDBContext.Template.Where(x => x.Id == app.TemplateId)
                                                                             join tr in _EngineCoreDBContext.SysTranslation on temp.TitleShortcut equals tr.Shortcut
                                                                             where tr.Lang == "en"
                                                                             select new { templateEn = tr.Value }
                                                                                                                ).FirstOrDefault().templateEn,
                           fee = pay.ActualPaid.ToString(),
                           invoiceNo = pay.InvoiceNo,
                           invoiceDate = pay.LastUpdatedDate.HasValue ? Convert.ToDateTime(pay.LastUpdatedDate).ToString("dd MMM yyyy") : "",
                       })


                   });
                int TransactionId = (from q in query
                                     join tranc in _EngineCoreDBContext.AppTransaction on q.claim.contractNo equals tranc.TransactionNo
                                     select new
                                     {
                                         trancId = tranc.Id
                                     }).Select(x => x.trancId).FirstOrDefault();
                requestCredentialsResDto.credentialExpiryDate = query.Select(x => x.credentialExpiryDate).FirstOrDefault();
                requestCredentialsResDto.claim = query.Select(x => x.claim).FirstOrDefault();
                requestCredentialsResDto.keyHash = query.Select(x => x.keyHash).FirstOrDefault();
                requestCredentialsResDto.evidence = query.Select(x => x.evidence).FirstOrDefault();
                requestCredentialsResDto.claim.parties = (from par in _EngineCoreDBContext.ApplicationParty
                                                                                          .Where(x => x.TransactionId == TransactionId)
                                                          select new CredentialsParty
                                                          {
                                                              partyName = par.FullName,
                                                              partyNameLabelAr = "اسم الطرف",
                                                              partyNameLabelEn = "Party Name",
                                                              nationalityAr = par.Nationality == null ? null : (from Country in _EngineCoreDBContext.Country
                                                                                                                where Country.UgId == par.Nationality
                                                                                                                select new { CountryAr = Country.CntCountryAr }
                                                                                                                ).FirstOrDefault().CountryAr,
                                                              nationalityEn = par.Nationality == null ? null : (from Country in _EngineCoreDBContext.Country
                                                                                                                where Country.UgId == par.Nationality
                                                                                                                select new { CountryEn = Country.CntCountryEn }
                                                                                                                ).FirstOrDefault().CountryEn,
                                                              nationalityLabelAr = "الجنسية",
                                                              nationalityLabelEn = "Nationality",
                                                              partyTypeAr = par.PartyTypeValueId == null ? null : (from lv in _EngineCoreDBContext.SysLookupValue
                                                                                                                   join tr in _EngineCoreDBContext.SysTranslation
                                                                                                                   on lv.Shortcut equals tr.Shortcut
                                                                                                                   where tr.Lang == "ar"
                                                                                                                   select new { partytypear = tr.Value }
                                                                                                                ).FirstOrDefault().partytypear,
                                                              partyTypeEn = par.PartyTypeValueId == null ? null : (from lv in _EngineCoreDBContext.SysLookupValue
                                                                                                                   join tr in _EngineCoreDBContext.SysTranslation
                                                                                                                   on lv.Shortcut equals tr.Shortcut
                                                                                                                   where tr.Lang == "en"
                                                                                                                   select new { partytypeen = tr.Value }
                                                                                                                ).FirstOrDefault().partytypeen,
                                                              partyTypeLabelAr = "نوع الطرف",
                                                              partyTypeLabelEn = "Party Type",
                                                              idNumber = par.EmiratesIdNo == null ? (from xpar in _EngineCoreDBContext.ApplicationPartyExtraAttachment
                                                                                                     where xpar.ApplicationPartyId == par.Id
                                                                                                     select new { number = xpar.Number }
                                                                                                                ).FirstOrDefault().number : par.EmiratesIdNo,
                                                              idNumberLabelAr = "رقم الوثيقة",
                                                              idNumberLabelEn = "Dcoument number",
                                                              idDocTypeAr = par.EmiratesIdNo == null ? attachtranslateAR.Where(x => x.attachId.ToString() == (from xpar in _EngineCoreDBContext.ApplicationPartyExtraAttachment
                                                                                                                                                              where xpar.ApplicationPartyId == par.Id
                                                                                                                                                              select new { number = xpar.Number }
                                                                                                                   ).FirstOrDefault().number).Select(z => z.Value).FirstOrDefault() : "هوية إماراتية",
                                                              idDocTypeEn = par.EmiratesIdNo == null ? attachtranslateEN.Where(x => x.attachId.ToString() == (from xpar in _EngineCoreDBContext.ApplicationPartyExtraAttachment
                                                                                                                                                              where xpar.ApplicationPartyId == par.Id
                                                                                                                                                              select new { number = xpar.Number }
                                                                                                                   ).FirstOrDefault().number).Select(z => z.Value).FirstOrDefault() : "EID",
                                                              idDocTypeLabelAr = "نوع الوثيقة",
                                                              idDocTypeLabelEn = "Document Type",

                                                          }).ToList();




                AppTransaction appTransaction = _EngineCoreDBContext.AppTransaction.Where(x => x.TransactionNo == requestCredentialsResDto.claim.contractNo).FirstOrDefault();
                appTransaction.TransactionRefNo = requestCredentialsReqDto.TransactionRefNo;
                _iGeneralRepository.Update(appTransaction);
                await _iGeneralRepository.Save();

            }
            return requestCredentialsResDto;
        }

        static string pdfToBase64(string filePath)
        {
            
            Byte[] bytes = File.ReadAllBytes(filePath);
            string pdfbase64 = Convert.ToBase64String(bytes);
            return pdfbase64;

        }

        public async  Task<UpdateVCIDResDto> UpdateVCID(UpdateVCIDDto updateVCIDDto)
        {
            SysLookupType DocumentType1 = _EngineCoreDBContext.SysLookupType.Where(x => x.Value == "transaction_state").FirstOrDefault();
            int transaction_state_id = DocumentType1.Id;

            int transaction_state_cancel_id = (from lv in _EngineCoreDBContext.SysLookupValue.Where(x => x.LookupTypeId == transaction_state_id && x.Shortcut == "CANCELED")


                                               select new { id = lv.Id }
               ).FirstOrDefault().id;

            UpdateVCIDResDto updateVCIDResDto = new UpdateVCIDResDto();
            if (updateVCIDDto.transactionRefNo == "" || updateVCIDDto.transactionRefOfRequest=="" || updateVCIDDto.transactionRefNo == null || updateVCIDDto.transactionRefOfRequest == null)
            {
                updateVCIDResDto.statusCode = "-2";
                updateVCIDResDto.statusMessage = "Transaction reference number is required";
                return updateVCIDResDto;
            }

            //if (updateVCIDDto.VCID == "")
            //{
            //    _exception.AttributeMessages.Add(Constants.getMessage("en", "VCIR"));
            //    throw _exception;
            //}


          
        
            AppTransaction appTransaction = await _EngineCoreDBContext.AppTransaction.Where(x => x.TransactionRefNo== updateVCIDDto.transactionRefOfRequest && x.TransactionStatus!= transaction_state_cancel_id).FirstOrDefaultAsync();
            if (appTransaction==null)
            {
                updateVCIDResDto.statusCode = "-12";
                updateVCIDResDto.statusMessage = "Unable to find the application Specified";
                return updateVCIDResDto;
            }
            string oldVcid = appTransaction.Vcid;
            appTransaction.Vcid = updateVCIDDto.VCID;
            _iGeneralRepository.Update(appTransaction);

            if ( await _iGeneralRepository.Save() && (oldVcid == null || oldVcid == ""))
            {
                updateVCIDResDto.statusCode = "3";
                updateVCIDResDto.statusMessage = "vcID is Updated Successfully";
            }
            else if (await _iGeneralRepository.Save() && (oldVcid == updateVCIDDto.VCID))
            {
                updateVCIDResDto.statusCode = "3";
                updateVCIDResDto.statusMessage = "vcID is Updated Successfully";
                
            }
            else if (await _iGeneralRepository.Save() && (oldVcid != updateVCIDDto.VCID))
            {
                updateVCIDResDto.statusCode = "4";
                updateVCIDResDto.statusMessage = "vcID is Revoked Successfully";
            }
            else
            {
                updateVCIDResDto.statusCode = "-8";
                updateVCIDResDto.statusMessage = "Unable to save the request";
            }

            return updateVCIDResDto;

        }

        private static string AddFooterToExistPDF(string originalFile,string newFile,string footer)
        {


            iTextSharp.text.Image sigimage = null;
            //    string originalFile = System.IO.Path.Combine(_iFilesUploaderRepositiory.GetRootPath(), filename);// "E:/BackEndProject/EngineCoreProject/wwwroot/transactions/Report40695.pdf";
            // string newfile = "E:/BackEndProject/EngineCoreProject/wwwroot/transactions/Report42324.pdf";// System.IO.Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), filename);
            // string newFile = System.IO.Path.Combine(_iFilesUploaderRepositiory.GetRootPath(), Constants.transactionsFolder, "UAEPass.pdf");
            PdfReader PDFReader2 = new PdfReader(originalFile);
            FileStream Stream2 = new FileStream(newFile, FileMode.Create, FileAccess.Write);
            PdfStamper PDFStamper2 = new PdfStamper(PDFReader2, Stream2);

            int numberOfPages = PDFStamper2.Reader.NumberOfPages;
            for (int i = 1; i <= numberOfPages; i++)
            {
                PdfContentByte PDFData = PDFStamper2.GetOverContent(i);
                PDFData = PDFStamper2.GetOverContent(i);

                sigimage = iTextSharp.text.Image.GetInstance(footer); // (System.IO.Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), footer));
                sigimage.SetAbsolutePosition(PageSize.A4.Bottom, 0);
                sigimage.ScaleAbsolute(600, 80);
                sigimage.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                PDFData.AddImage(sigimage);

            }
            PDFStamper2.Close();
            PDFReader2.Close();
            Byte[] bytes = File.ReadAllBytes(newFile);
            string pdfbase64 = Convert.ToBase64String(bytes);
            return pdfbase64;
        }

    }
}
