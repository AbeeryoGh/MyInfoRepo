using DinkToPdf;
using DinkToPdf.Contracts;
using EngineCoreProject;
using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.ApplicationDtos.ModelView;
using EngineCoreProject.DTOs.ConfigureWritableDto;
using EngineCoreProject.DTOs.oldDataDto;
using EngineCoreProject.DTOs.Payment;
using EngineCoreProject.DTOs.PDFDto;
using EngineCoreProject.DTOs.PDFGenerator;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.IRepository.IFilesUploader;
using EngineCoreProject.IRepository.IGeneratorRepository;
using EngineCoreProject.IRepository.IPaymentRepository;
using EngineCoreProject.Models;
using EngineCoreProject.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Patagames.Pdf.Net;
using PdfiumViewer;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ColorMode = DinkToPdf.ColorMode;
using Image = System.Drawing.Image;

namespace EngineCoreProject.Services.GeneratorServices
{
    public class GeneratorRepository : IGenerator
    {
        private readonly IApplicationRepository _IApplicationRepositiory;
        private readonly IPaymentRepository _PaymentRepository;
        private readonly EngineCoreDBContext _EnotaryDBContext;
        private readonly FileNaming _FileNaming;
        private readonly IGeneralRepository _IGeneralRepository;
        private readonly Pdfdocumentsetting _Pdfdocumentsetting;
        private readonly IFilesUploaderRepositiory _IFilesUploaderRepositiory;
        private IConverter _IConverter;
        ValidatorException _exception;
        public GeneratorRepository(IConverter converter, EngineCoreDBContext enotaryDBContext, IOptions<FileNaming> fileNaming,
            IGeneralRepository iGeneralRepository, IPaymentRepository paymentRepository,
            IApplicationRepository iApplicationRepository, IOptions<Pdfdocumentsetting> pDFDocumentSetting, IFilesUploaderRepositiory iFilesUploaderRepositiory)
        {
            _IApplicationRepositiory = iApplicationRepository;
            _PaymentRepository = paymentRepository;
            _EnotaryDBContext = enotaryDBContext;
            _FileNaming = fileNaming.Value;
            _EnotaryDBContext = enotaryDBContext;
            _IGeneralRepository = iGeneralRepository;
            _Pdfdocumentsetting = pDFDocumentSetting.Value;
            _IConverter = converter;
            _IFilesUploaderRepositiory = iFilesUploaderRepositiory;
            _exception = new ValidatorException();
        }


        public GeneratorRepository(EngineCoreDBContext enotaryDBContext, IApplicationRepository iApplicationRepositiory, IPaymentRepository paymentRepository,
            IGeneralRepository iGeneralRepository, IOptions<FileNaming> fileNaming, IConverter converter, IOptions<Pdfdocumentsetting> pDFDocumentSetting, IFilesUploaderRepositiory iFilesUploaderRepositiory)
        {
            _EnotaryDBContext = enotaryDBContext;
            _IApplicationRepositiory = iApplicationRepositiory;
            _PaymentRepository = paymentRepository;
            _IGeneralRepository = iGeneralRepository;
            _FileNaming = fileNaming.Value;
            _Pdfdocumentsetting = pDFDocumentSetting.Value;
            _IFilesUploaderRepositiory = iFilesUploaderRepositiory;
            _IConverter = converter;
        }
        public ObjectSettings GetPagesSettings(string bodyHtml, string HeaderUrl, string FooterUrl)
        {

            HeaderSettings headerSettings = new HeaderSettings { Spacing = 3, Line = false, HtmUrl = HeaderUrl }; ;
            FooterSettings footerSettings = new FooterSettings { Spacing = 1, Right = "", Line = false, HtmUrl = FooterUrl, FontName = "DroidArabicKufiRegular", FontSize = 10 };// _Pdfdocumentsetting.PagesSettings.FooterSettings.HtmUrl };

            ObjectSettings PagesSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = bodyHtml,// TemplateGenerator.GetHTMLString(),
                                       //Page = "https://code-maze.com/", //USE THIS PROPERTY TO GENERATE PDF CONTENT FROM AN HTML PAGE
                WebSettings = { },
                HeaderSettings = headerSettings,
                FooterSettings = footerSettings
            };

            return PagesSettings;
        }
        public string GetHtmlToPdfDocument(List<PdfPageSetting> PdfPageSettings, GlobalSettings GlobalSettings, string path)
        {
            ObjectSettings[] PagesSettingsArr = new ObjectSettings[PdfPageSettings.Count];
            int i = 0;
            foreach (PdfPageSetting PdfPageSetting in PdfPageSettings)
            {
                ObjectSettings PagesSettings = GetPagesSettings(PdfPageSetting.BodyDoc, PdfPageSetting.HeaderUrl, PdfPageSetting.FooterUrl);
                PagesSettingsArr[i] = PagesSettings;
                i++;
            }


            var globalSettings = GetGlobalSettings(GlobalSettings, path);
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
            };
            foreach (ObjectSettings ObjectSettings in PagesSettingsArr)
                pdf.Objects.Add(ObjectSettings);
            var file = _IConverter.Convert(pdf);
            return Path.Combine(path, GlobalSettings.Out);// globalSettings.Out;
        }
        public GlobalSettings GetGlobalSettings(GlobalSettings GlobalSettings, string path)
        {
            GlobalSettings.Margins.Bottom = (GlobalSettings.Margins.Bottom == null) ? 34 : GlobalSettings.Margins.Bottom;
            GlobalSettings.Margins.Top = (GlobalSettings.Margins.Top == null) ? 33 : GlobalSettings.Margins.Bottom;
            GlobalSettings.Margins.Left = (GlobalSettings.Margins.Left == null) ? 0 : GlobalSettings.Margins.Left;
            GlobalSettings.Margins.Right = (GlobalSettings.Margins.Right == null) ? 0 : GlobalSettings.Margins.Right;


            GlobalSettings globalSettings = new GlobalSettings
            {
                //(party.partyType == null) ? "" : party.partyType;
                Orientation = (GlobalSettings.Orientation == null) ? Orientation.Portrait : GlobalSettings.Orientation,
                PaperSize = (GlobalSettings.PaperSize == null) ? PaperKind.A4 : GlobalSettings.PaperSize,
                DocumentTitle = (GlobalSettings.DocumentTitle == null) ? "PDF REPORT" : GlobalSettings.DocumentTitle,
                Out = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), path, GlobalSettings.Out),
                Margins = GlobalSettings.Margins
            };

            return globalSettings;
        }
        public string SetBodyDocument(string lang, string FileName, string Argument1, string Argument2, string Argument3, string Argument4, string Argument5, string Argument6, string Argument7, string Argument8, string Argument9, string Argument10)
        {
            try
            {
                string HtmlFile = File.ReadAllText(Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "DocumentHtml", "Templete", FileName));
                string html = String.Format(HtmlFile, Argument1, Argument2, Argument3, Argument4, Argument5, Argument6, Argument7, Argument8, Argument9, Argument10);
                return html;
            }
            catch (Exception e)

            {
                throw new Exception(Constants.getMessage(lang, "FileNotFound"));
            }
        }
        public string PartiesTable(string lang, List<Party> parties, int type)
        {
            string PartiesTableRows = null;
            string PartiesTable = null;
            if (parties != null && parties.Count > 0)
            {
                foreach (Party party in parties)
                {
                    party.partyType = (party.partyType == null) ? "" : party.partyType;
                    party.fullName = (party.fullName == null) ? "" : party.fullName;
                    party.nationality = (party.nationality == null) ? "" : party.nationality;
                    party.documentType = (party.documentType == null) ? "" : party.documentType;
                    party.documentNumber = (party.documentNumber == null) ? "" : party.documentNumber;
                    party.countryOfIssue = (party.countryOfIssue == null) ? "" : party.countryOfIssue;
                    party.signtureUrl = (party.signtureUrl == null) ? "" : party.signtureUrl;
                    party.phoneNumber = (party.phoneNumber == null) ? "" : party.phoneNumber;
                    party.address = (party.address == null) ? "" : party.address;
                    if (type == 1)
                    {
                        if (party.SignRequired == true)
                            PartiesTableRows += SetBodyDocument(lang, "PartiesTableWithSignBody.html", party.fullName.Trim(), party.partyType.Trim(), Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), party.signtureUrl), null, null, null, null, null, null, null);
                    }
                    else if (type == 0) PartiesTableRows += SetBodyDocument(lang, "PartiesTableWithoutSignBody.html", party.fullName.Trim(), party.partyType.Trim(), party.nationality, party.documentType.Trim(), party.documentNumber.Trim(), party.phoneNumber.Trim(), party.address.Trim(), null, null, null);

                    else if (type == 2)
                    {
                        if (party.SignRequired == true)
                            PartiesTableRows += SetBodyDocument(lang, "PartiesTableWithoutAddressWithSignBody.html", party.fullName.Trim(), party.partyType.Trim(), party.nationality, party.documentType.Trim(), party.documentNumber.Trim(), party.phoneNumber.Trim(), Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), party.signtureUrl), null, null, null);
                    }
                }
                string FileName = null;
                if (type == 1) FileName = "PartiesTableWithSign.html";
                if (type == 0) FileName = "PartiesTableWithoutSign.html";
                if (type == 2) FileName = "PartiesTableWithoutAddressWithSign.html";
                PartiesTable = SetBodyDocument(lang, FileName, PartiesTableRows, null, null, null, null, null, null, null, null, null);
            }


            if (PartiesTableRows == null) return null;


            return PartiesTable;

        }
        public string GetNewPdfFileName()
        {
            return _FileNaming.TermPdfFileName + _IGeneralRepository.GetNewValueBySec() + ".pdf";
        }
        public async Task<AutoCreatePdfPaths> autoCreatePDFAsync(string lang, int appId, string path)
        {
            AutoCreatePdfPaths AutoCreatePdfPaths = new AutoCreatePdfPaths();
            AutoCreatePdfPaths.RecordIdPaths = new Dictionary<int, string>();
            AutoCreatePdfPaths.RecordPaths = new Dictionary<string, string>();
            List<string> Docpaths = new List<string>();
            await CheckIfApplicationExistAsync(lang, appId);
            DocToPdfDto DocToPdfDto = await ConverterDTOAsync(appId);
            if (DocToPdfDto == null)
                throw new System.InvalidOperationException(Constants.getMessage(lang, "UnknownError"));

            string FileName = null; ;
            int? AppTransactionId;
            try
            {
                AppTransactionId = await _EnotaryDBContext.AppTransaction.Where(x => x.TransactionNo == DocToPdfDto.TransactionNo).Select(x => x.Id).FirstOrDefaultAsync();
            }
            catch (Exception e) { AppTransactionId = -1; }


            if (DocToPdfDto.transactionText != null && DocToPdfDto.transactionText.Trim() != "") //&& 
            {
                FileName = GetNewPdfFileName();
                await DocumentTemplete(new TemplateInfoDto
                {

                    parties = DocToPdfDto.parties,
                    path = path,
                    lang = lang,
                    notaryInfo = DocToPdfDto.notaryInfo,
                    Content = DocToPdfDto.transactionText,
                    Title = DocToPdfDto.Title,//DocToPdfDto.Title==null? DocToPdfDto.TemplateName :
                    TransactionNo = DocToPdfDto.TransactionNo,
                    RecQRUrl = generateQRCodeURL(_Pdfdocumentsetting.BaseURL, new List<string> { AppTransactionId.ToString() /*, DocToPdfDto.TransactionNo*/}),//, DocToPdfDto.TransactionNo 
                    FileName = FileName,
                    appID=appId

                });

                Docpaths.Add(Path.Combine(path, FileName));
            }
            else if (DocToPdfDto.FileName != null && DocToPdfDto.FileName != "")
            {
                CheckIfFilesExist(lang, new List<string> { DocToPdfDto.FileName });
                List<string> ImagesPaths = ConvertPdfToImages(lang, DocToPdfDto.FileName);
                FileName = await AddFooterAndHeaderToOldTransactionAsync(lang, new InfoDoc { ImagePaths = ImagesPaths, TransactionNo = DocToPdfDto.TransactionNo, Title = DocToPdfDto.Title, RecQRUrl = _Pdfdocumentsetting.BaseURL+ AppTransactionId, path = path });
                Docpaths.Add(Path.Combine(path, FileName));
            }
            else if (DocToPdfDto.relatedTransactions != null && DocToPdfDto.relatedTransactions.Count > 0)
            {
                if (DocToPdfDto.ServiceResult == 3)
                {
                    CheckIfFilesExist(lang, DocToPdfDto.relatedTransactions);
                    string mergedFile = GetNewPdfFileName();
                    string pathPdf;
                    if (DocToPdfDto.relatedTransactions.Count > 1)
                        pathPdf = MergePdfs(lang, null, DocToPdfDto.relatedTransactions, new GlobalSettings { Out = mergedFile });
                    else pathPdf = DocToPdfDto.relatedTransactions[0];
                    Docpaths.Add(pathPdf);
                }
                else if (DocToPdfDto.ServiceResult == 2)
                {
                    //CheckIfFilesExist(lang, DocToPdfDto.relatedTransactions);
                    //string mergedFile = GetNewPdfFileName();
                    //string pathPdf;
                    //if (DocToPdfDto.relatedTransactions.Count > 1)
                    //    pathPdf = MergePdfs(lang, null, DocToPdfDto.relatedTransactions, new GlobalSettings { Out = mergedFile });
                    //else pathPdf = DocToPdfDto.relatedTransactions[0];                   
                    //Docpaths.Add(pathPdf);

                    CheckIfFilesExist(lang, DocToPdfDto.relatedTransactions);
                    string pathPdf;
                    List<string> Files = new List<string>();
                    foreach (string doc in DocToPdfDto.relatedTransactions)
                    {
                        // pathPdf = doc;
                        //List<string> ImagesPaths = ConvertPdfToImages(lang, pathPdf);
                        // FileName = await AddFooterAndHeaderToOldTransactionAsync(lang, new InfoDoc { ImagePaths = ImagesPaths, TransactionNo = DocToPdfDto.TransactionNo, Title = DocToPdfDto.Title, RecQRUrl = _Pdfdocumentsetting.BaseURL + AppTransactionId, path = path });
                        string FileNameBeforSign = doc;
                        FileName = GetNewPdfFileName();
                        await suraTubiqAlaaslAsync(FileName, path, FileNameBeforSign, DocToPdfDto.PaymentFee.ToString(), DocToPdfDto.notaryInfo.SignUrl, DocToPdfDto.notaryInfo.FullName, AppTransactionId);
                        // string FileNameAfterAddSign = GetNewPdfFileName();
                        //pageNumberingAndSign(FileNameAfterAddSign, path, FileName, false);
                        Files.Add(Path.Combine(path, FileName));
                    }
                    if (DocToPdfDto.relatedTransactions.Count == 1) Docpaths.Add(Path.Combine(path, FileName));
                    else
                    {
                        string mergedFile = GetNewPdfFileName();
                        pathPdf = MergePdfs(lang, null, Files, new GlobalSettings { Out = mergedFile });
                        Docpaths.Add(pathPdf);
                    }


                }
            }

            if (DocToPdfDto.relatedContents != null && DocToPdfDto.relatedContents.Count != 0)
            {
                foreach (RelatedContentView RelatedContentView in DocToPdfDto.relatedContents)
                {
                    FileName = GetNewPdfFileName();
                    TemplateInfoDto TemplateInfoDto = new TemplateInfoDto
                    {
                        Content = RelatedContentView.Content,
                        parties = DocToPdfDto.parties,
                        path = path,
                        lang = lang,
                        notaryInfo = DocToPdfDto.notaryInfo,
                        PaymentDate = DocToPdfDto.PaymentDate.ToString(),
                        Title = RelatedContentView.Title,
                        TransactionNo = DocToPdfDto.TransactionNo,
                        PaymentFee = DocToPdfDto.PaymentFee.ToString(),
                        ReceiptNumber = DocToPdfDto.ReceiptNumber,
                        RecQRUrl = generateQRCodeURL(_Pdfdocumentsetting.BaseURL, new List<string> { AppTransactionId.ToString() /*, DocToPdfDto.TransactionNo*/ }),
                        //RecQRUrl = _Pdfdocumentsetting.BaseURL + AppTransactionId+"/"+DocToPdfDto.TransactionNo,
                        FileName = FileName,
                        MeetingDate = DocToPdfDto.MeetingDate,
                        appID=appId


                    };
                    if (RelatedContentView.TitleSortcut.Contains("EA")) await EkhtarAdli(TemplateInfoDto);
                    if (RelatedContentView.TitleSortcut.Contains("MT")) await MuhdirTasdiq(TemplateInfoDto);
                    if (RelatedContentView.TitleSortcut.Contains("RT")) await risalatTabligh(TemplateInfoDto); ;
                    if (RelatedContentView.TitleSortcut.Contains("EX"))
                    {
                        TemplateInfoDto.FileNameWithoutSign = GetNewPdfFileName();
                        await akhtarTanfiz(TemplateInfoDto);
                    }
                    //if (RelatedContentView.TitleSortcut.Contains("TA"))
                    //{
                    //    TemplateInfoDto.FileNameWithoutSign = GetNewPdfFileName();
                    //    await suraTubiqAlaasl(TemplateInfoDto); ;
                    //}
                    Docpaths.Add(Path.Combine(path, FileName));
                    AutoCreatePdfPaths.RecordPaths.Add(RelatedContentView.TitleSortcut, Path.Combine(path, FileName));
                    AutoCreatePdfPaths.RecordIdPaths.Add(RelatedContentView.Id, Path.Combine(path, FileName));

                }
                FileName = GetNewPdfFileName();
                MergePdfs(lang, null, Docpaths, new GlobalSettings { Out = FileName, });
            }
            string FileNameAfterSign = GetNewPdfFileName();
            if (DocToPdfDto.TransactionNo != null)
            {
                if (DocToPdfDto.ServiceResult == 2)// DocToPdfDto.ServiceResult != 2 &&
                {
                    pageNumberingAndSign(FileNameAfterSign, path, FileName, true, false);
                    AutoCreatePdfPaths.TransactionDoc = Path.Combine(path, FileNameAfterSign);
                }
                else if (DocToPdfDto.ServiceResult != 3)// DocToPdfDto.ServiceResult != 2 &&
                {
                    pageNumberingAndSign(FileNameAfterSign, path, FileName, true, true);
                    AutoCreatePdfPaths.TransactionDoc = Path.Combine(path, FileNameAfterSign);
                }
                else AutoCreatePdfPaths.TransactionDoc = Path.Combine(path, FileName);
            }

            else
                if(DocToPdfDto.ServiceResult!=0)
               
                AutoCreatePdfPaths.TransactionDoc = Docpaths[0];

            return AutoCreatePdfPaths;

        }

        private string generateQRCodeURL(string baseURL, List<string> URLelement)
        {
            string url = baseURL;
            foreach (string el in URLelement)
                url += "/" + el;
            return url;
        }

        public async Task suraTubiqAlaaslAsync(string fileNameAfterSign, string path, string oldFileName, string PaymentFee, string SignUrl, string NotaryName, int? AppTransactionId)
        {

            string RecQRUrl = generateQRCodeURL(_Pdfdocumentsetting.BaseURL, new List<string> { AppTransactionId.ToString() /*, DocToPdfDto.TransactionNo*/});

            string TransactionNo = await _EnotaryDBContext.AppTransaction.Where(x => x.Id == AppTransactionId).Select(y => y.TransactionNo).FirstOrDefaultAsync();
            string QrFileName = await generateQRAsync(RecQRUrl, TransactionNo,Color.Blue);


            string pdfFile2 = System.IO.Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), oldFileName);

            string pdfFileWithSign2 = System.IO.Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), Constants.transactionsFolder, fileNameAfterSign);
            PdfReader PDFReader2 = new PdfReader(pdfFile2);
            FileStream Stream2 = new FileStream(pdfFileWithSign2, FileMode.Create, FileAccess.Write);
            PdfStamper PDFStamper2 = new PdfStamper(PDFReader2, Stream2);


            int numberOfPages = PDFStamper2.Reader.NumberOfPages;

            PdfContentByte PDFData = PDFStamper2.GetOverContent(numberOfPages);
            string ArialFontFile = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "kufi", "DroidKufi-Regular.ttf");
            BaseFont bfArialUniCode = BaseFont.CreateFont(ArialFontFile, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            iTextSharp.text.Font f = new iTextSharp.text.Font(bfArialUniCode, 10, 0, CMYKColor.BLUE);

            PdfPTable table2 = new PdfPTable(1); // a table with 1 cell
            float[] x = new float[1] { 260 };
            table2.SetTotalWidth(x);
            table2.RunDirection = PdfWriter.RUN_DIRECTION_RTL; // can also be set on the cell
            Phrase text = new Phrase("دولة الإمارات العربية المتحدة", f);

            PdfPCell cell = new PdfPCell(text);
            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            cell.BorderWidthTop = (float)0.5;
            cell.BorderWidthRight = (float)0.5;
            cell.BorderWidthBottom = 0;
            cell.BorderWidthLeft = (float)0.5;
            cell.BorderColor = CMYKColor.BLUE;

            table2.AddCell(cell);
            text = new Phrase("وزارة العدل", f);
            cell = new PdfPCell(text);
            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = (float)0.5;
            cell.BorderWidthBottom = 0;
            cell.BorderWidthLeft = (float)0.5;
            cell.BorderColor = CMYKColor.BLUE;
            table2.AddCell(cell);

            text = new Phrase("إدارة الكاتب العدل و التصديقات", f);
            cell = new PdfPCell(text);
            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = (float)0.5;
            cell.BorderWidthBottom = 0;
            cell.BorderWidthLeft = (float)0.5;
            cell.BorderColor = CMYKColor.BLUE;
            table2.AddCell(cell);

            text = new Phrase("صورة طبق الأصل", f);
            cell = new PdfPCell(text);
            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            cell.MinimumHeight = (float)17;
            cell.BorderWidthTop = 0;
            cell.BorderWidthRight = (float)0.5;
            cell.BorderWidthBottom = 0;
            cell.BorderWidthLeft = (float)0.5;
            cell.BorderColor = CMYKColor.BLUE;
            table2.AddCell(cell);

            table2.WriteSelectedRows(0, -1, 200, 201, PDFData);



            PdfPTable table = new PdfPTable(2); // a table with 2 cell
            x = new float[2] { 130, 130 };
            table.SetTotalWidth(x);
            table.RunDirection = PdfWriter.RUN_DIRECTION_RTL; // can also be set on the cell
            text = new Phrase("التاريخ", f);
            cell = new PdfPCell(text);
            cell.BorderWidthTop = (float)0.5;
            cell.BorderWidthRight = (float)0.5;
            cell.BorderWidthBottom = (float)0.5;
            cell.BorderWidthLeft = (float)0.5;
            cell.BorderColor = CMYKColor.BLUE;
            cell.MinimumHeight = (float)17;
            table.AddCell(cell);


            text = new Phrase(DateTime.Now.ToString("yyyy-MM-dd"), f);
            cell = new PdfPCell(text);
            cell.BorderWidthTop = (float)0.5;
            cell.BorderWidthLeft = (float)0.5;
            cell.VerticalAlignment = PdfPCell.ALIGN_BOTTOM;
            cell.MinimumHeight = (float)17;
            cell.BorderWidthBottom = (float)0.5;
            cell.BorderWidthRight = 0;
            cell.BorderColor = CMYKColor.BLUE;
            table.AddCell(cell);


            text = new Phrase("الرسوم", f);
            cell = new PdfPCell(text);
            cell.MinimumHeight = (float)17;
            cell.BorderWidthRight = (float)0.5;
            cell.BorderWidthBottom = (float)0.5;
            cell.BorderWidthLeft = (float)0.5;
            cell.BorderWidthTop = 0;
            cell.BorderColor = CMYKColor.BLUE;
            table.AddCell(cell);

            text = new Phrase("           درهم", f);
            cell.MinimumHeight = (float)17;
            cell = new PdfPCell(text);
            cell.BorderWidthLeft = (float)0.5;
            cell.BorderWidthBottom = (float)0.5;
            cell.BorderWidthRight = 0;
            cell.BorderWidthTop = 0;
            cell.BorderColor = CMYKColor.BLUE;
            table.AddCell(cell);

            text = new Phrase("اسم كاتب العدل\n" + NotaryName, f);
            cell = new PdfPCell(text);
            cell.BorderWidthBottom = (float)0.5;
            cell.BorderWidthRight = (float)0.5;
            cell.BorderWidthLeft = (float)0.5;
            cell.BorderWidthTop = 0;
            cell.MinimumHeight = (float)48;
            cell.BorderColor = CMYKColor.BLUE;
            table.AddCell(cell);

            text = new Phrase("", f);
            cell = new PdfPCell(text);
            cell.BorderWidthBottom = (float)0.5; ;
            cell.BorderWidthRight = 0;
            cell.MinimumHeight = (float)17;
            cell.BorderWidthLeft = (float)0.5; ;
            cell.BorderWidthTop = 0;
            cell.MinimumHeight = (float)48;
            cell.BorderColor = CMYKColor.BLUE;
            table.AddCell(cell);




            table.WriteSelectedRows(0, -1, 200, 143, PDFData);

            PDFData = PDFStamper2.GetOverContent(numberOfPages);
            string EnotarySignUrl;
            try
            {
                EnotarySignUrl = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), SignUrl);
            }
            catch (Exception e) { EnotarySignUrl = ""; }
            iTextSharp.text.Image sigimage = null;
            try
            {
                sigimage = iTextSharp.text.Image.GetInstance(EnotarySignUrl);
                sigimage.SetAbsolutePosition(290, 66);
                sigimage.ScaleAbsolute(40, 40);
                PDFData.AddImage(sigimage);
            }
            catch { }
            // //

            PDFData = PDFStamper2.GetOverContent(numberOfPages);
            string CompleteQRFileNameURL;
            try
            {
                CompleteQRFileNameURL = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "QRCode", QrFileName);
            }
            catch (Exception e) { CompleteQRFileNameURL = ""; }
            iTextSharp.text.Image QRimage = null;
            try
            {
                QRimage = iTextSharp.text.Image.GetInstance(CompleteQRFileNameURL);
                QRimage.SetAbsolutePosition(490, 110);
                QRimage.ScaleAbsolute(100, 100);
                PDFData.AddImage(QRimage);
            }
            catch { }





            PDFData = PDFStamper2.GetOverContent(numberOfPages);
            ArialFontFile = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "kufi", "DroidKufi-Regular.ttf");
            BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
            f = new iTextSharp.text.Font(baseFont, 10, 0, CMYKColor.BLUE);
            table = new PdfPTable(2); // a table with 1 cell
            x = new float[2] { 130, 130 };
            table.SetTotalWidth(x);
            table.RunDirection = PdfWriter.RUN_DIRECTION_RTL; // can also be set on the cell
            text = new Phrase("تاريخ الاصدار", f);
            cell = new PdfPCell(text);
            cell.BorderWidthBottom = 0;
            cell.BorderWidthRight = 0;
            cell.MinimumHeight = (float)17;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthTop = 0;
            table.AddCell(cell);


            text = new Phrase(DateTime.Now.ToString("yyyy-MM-dd"), f);
            cell = new PdfPCell(text);
            cell.BorderWidthBottom = 0;
            cell.BorderWidthRight = 0;
            cell.MinimumHeight = (float)17;
            cell.BorderWidthLeft = 0;
            cell.VerticalAlignment = PdfPCell.ALIGN_BOTTOM;
            cell.BorderWidthTop = 0;
            table.AddCell(cell);


            text = new Phrase("الرسوم", f);
            cell = new PdfPCell(text);
            cell.BorderWidthBottom = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.MinimumHeight = (float)17;
            cell.VerticalAlignment = PdfPCell.ALIGN_CENTER;
            cell.BorderWidthTop = 0;
            table.AddCell(cell);

            text = new Phrase(PaymentFee, f);
            cell = new PdfPCell(text);
            cell.BorderWidthBottom = 0;
            cell.BorderWidthRight = 0;
            cell.VerticalAlignment = PdfPCell.ALIGN_BOTTOM;
            cell.BorderWidthLeft = 0;
            cell.MinimumHeight = (float)17;
            cell.BorderWidthTop = 0;
            table.AddCell(cell);

            text = new Phrase("توقيع كاتب العدل ", f);
            cell = new PdfPCell(text);
            cell.BorderWidthBottom = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthTop = 0;
            cell.MinimumHeight = (float)50;
            table.AddCell(cell);
            text = new Phrase(" ", f);
            cell = new PdfPCell(text);
            cell.BorderWidthBottom = 0;
            cell.BorderWidthRight = 0;
            cell.BorderWidthLeft = 0;
            cell.BorderWidthTop = 0;
            cell.MinimumHeight = (float)50;
            table.AddCell(cell);


            

            table.WriteSelectedRows(0, -1, 200, 143, PDFData);

            PDFData = PDFStamper2.GetOverContent(numberOfPages);
            try
            {
                sigimage = iTextSharp.text.Image.GetInstance(System.IO.Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), SignUrl));
                sigimage.SetAbsolutePosition(290, 66);
                sigimage.ScaleAbsolute(40, 40);
                PDFData.AddImage(sigimage);
            }
            catch { }


            // PDFData = PDFStamper2.GetOverContent(iCount + 1);
            // ArialFontFile = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "kufi", "DroidKufi-Regular.ttf");
            //  bfArialUniCode = BaseFont.CreateFont(ArialFontFile, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            f = new iTextSharp.text.Font(bfArialUniCode, 10, 0, CMYKColor.BLUE);
            table = new PdfPTable(1); // a table with 1 cell
            x = new float[1] { 600 };
            table.SetTotalWidth(x);
            text = new Phrase("وثيقة الكترونية معتمدة صادرة من وزارة العدل للتحقق منها يرجى مسح رمز الوصول السريع               الموجود على يمين الصفحة", f);
            cell = new PdfPCell(text);
            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            cell.BorderWidth = 0;

            table.RunDirection = PdfWriter.RUN_DIRECTION_RTL; // can also be set on the cell
            table.AddCell(cell);
            table.WriteSelectedRows(0, -1, 0, 63, PDFData);


            f = new iTextSharp.text.Font(baseFont, 10, 0, CMYKColor.BLUE);
            table = new PdfPTable(1); // a table with 1 cell
            x = new float[1] { 100 };
            table.SetTotalWidth(x);
            text = new Phrase(" (QR Code) ", f);
            cell = new PdfPCell(text);
            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            cell.BorderWidth = 0;

            table.RunDirection = PdfWriter.RUN_DIRECTION_LTR; // can also be set on the cell
            table.AddCell(cell);
            table.WriteSelectedRows(0, -1, 110, 63, PDFData);




            PDFStamper2.Close();
            PDFReader2.Close();
        }
        private void pageNumberingAndSign(string FileName, string path, string FileNameBeforNumbering, bool addNumbering, bool addSign)
        {
            string pdfFile2 = System.IO.Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), path, FileNameBeforNumbering);

            string pdfFileWithSign2 = System.IO.Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), Constants.transactionsFolder, FileName);
            PdfReader PDFReader2 = new PdfReader(pdfFile2);
            FileStream Stream2 = new FileStream(pdfFileWithSign2, FileMode.Create, FileAccess.Write);
            PdfStamper PDFStamper2 = new PdfStamper(PDFReader2, Stream2);
            if (addNumbering)
            {
                for (int iCount = 0; iCount < PDFStamper2.Reader.NumberOfPages; iCount++)
                {
                    PdfContentByte PDFData = PDFStamper2.GetOverContent(iCount + 1);
                    string ArialFontFile = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "kufi", "DroidKufi-Regular.ttf");
                    BaseFont bfArialUniCode = BaseFont.CreateFont(ArialFontFile, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    iTextSharp.text.Font f = new iTextSharp.text.Font(bfArialUniCode, 10, 0);
                    PdfPTable table = new PdfPTable(1); // a table with 1 cell
                    float[] x = new float[1] { 40 };
                    table.SetTotalWidth(x);
                    Phrase text = new Phrase("الصفحة", f);
                    PdfPCell cell = new PdfPCell(text);
                    cell.BorderWidth = 0;
                    table.RunDirection = PdfWriter.RUN_DIRECTION_RTL; // can also be set on the cell
                    table.AddCell(cell);
                    table.WriteSelectedRows(0, -1, 527, 98, PDFData);

                }
            }
            for (int iCount = 0; iCount < PDFStamper2.Reader.NumberOfPages; iCount++)
            {
                PdfContentByte PDFData = PDFStamper2.GetOverContent(iCount + 1);
                string ArialFontFile = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "kufi", "DroidKufi-Regular.ttf");
                //Reference a Unicode font to be sure that the symbols are present. 
                //BaseFont bfArialUniCode = BaseFont.CreateFont(ArialFontFile, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
                if (addNumbering)
                {

                    PDFData.BeginText();
                    PDFData.SetColorFill(CMYKColor.BLACK);
                    PDFData.SetFontAndSize(baseFont, 10);
                    PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, PDFStamper2.Reader.NumberOfPages + "/" + (iCount + 1).ToString(), 520, 86, 0);
                    PDFData.EndText();
                }
                PDFData.BeginText();
                PDFData.SetColorFill(CMYKColor.BLACK);
                PDFData.SetFontAndSize(baseFont, 10);
                PDFData.ShowTextAligned(PdfContentByte.ALIGN_CENTER, DateTime.Now.ToString("yyyy-MM-dd"), 539, 68, 0);
                PDFData.EndText();

            }
            if (addSign)
            {
                for (int iCount = 0; iCount < PDFStamper2.Reader.NumberOfPages; iCount++)
                {
                    PdfContentByte PDFData = PDFStamper2.GetOverContent(iCount + 1);
                    iTextSharp.text.Image sigimage = iTextSharp.text.Image.GetInstance(System.IO.Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "DocumentHtml", "Templete", "photo", "sign.png"));
                    sigimage.SetAbsolutePosition(80, 80);
                    sigimage.ScaleAbsolute(110, 110);
                    PDFData.AddImage(sigimage);
                }
            }

            PDFStamper2.Close();
            PDFReader2.Close();
        }
        private string GetFooter(string color)
        {
            string footer = "";

            try
            {
                if (color == "gold")
                    footer = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "DocumentHtml", "Templete", "RecordFooterWithoutSign.html");
                if (color == "red")
                    footer = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "DocumentHtml", "Templete", "RecordFooterWithoutSignRed.html");
                if (color == "BlockChain")
                    footer = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "DocumentHtml", "Templete", "BlockChainFooter.html");
            }
            catch (Exception e) { footer = ""; }

            return footer;
        }
        private string GetHeader(string QrFileName, string lang, string TransactionNo, string Title, string FileName, string color)
        {
            string QrFileNameUrl = "";
            string HeaderPhoto = "";
            try
            {
                QrFileNameUrl = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "QRCode", QrFileName);
            }
            catch (Exception e) { QrFileNameUrl = ""; }
            try
            {
                HeaderPhoto = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "DocumentHtml", "Templete", "photo", "header.png");
            }
            catch (Exception e) { HeaderPhoto = ""; }
            string HeaderContent = null;
            if (color == "gold")
                HeaderContent = SetBodyDocument(lang, "RecordHeader.html", TransactionNo, Title, QrFileNameUrl, HeaderPhoto, null, null, null, null, null, null);
            if (color == "red")
                HeaderContent = SetBodyDocument(lang, "RecordHeaderRed.html", TransactionNo, Title, QrFileNameUrl, HeaderPhoto, null, null, null, null, null, null);
            string HeaderFileName = "RecordHeaderTemp" + FileName + ".html";
            using (FileStream fs = new FileStream(Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), Constants.transactionsFolder, HeaderFileName), FileMode.Create))
            {
                using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                {
                    w.WriteLine(HeaderContent);
                }
            }

            string header;

            try
            {
                header = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), Constants.transactionsFolder, HeaderFileName);
            }
            catch (Exception e) { header = ""; }

            return header;

        }
        public async Task DocumentTemplete(TemplateInfoDto DocumentTempleteDto)
        {
            SysLookupType DocumentType = _EnotaryDBContext.SysLookupType.Where(x => x.Value == "document_type").FirstOrDefault();
            int documentTypeId = DocumentType.Id;
            int POAId = (from lv in _EnotaryDBContext.SysLookupValue.Where(x => x.LookupTypeId == documentTypeId)
                         join tr in _EnotaryDBContext.SysTranslation on lv.Shortcut equals tr.Shortcut
                         where tr.Value.Contains("Power of Attorney")
                         select new { id = lv.Id }
                ).FirstOrDefault().id;
            string QrFileName;

            Template template = null;
            if (DocumentTempleteDto.appID != null)
            {
                Application application = _EnotaryDBContext.Application.Where(x => x.Id == DocumentTempleteDto.appID).FirstOrDefault();
                template = _EnotaryDBContext.Template.Where(x => x.Id == application.TemplateId).FirstOrDefault();
            }

            string docBody = SetBodyDocument(DocumentTempleteDto.lang, "DocumentTemplete.html", PartiesTable(DocumentTempleteDto.lang, DocumentTempleteDto.parties, 0), DocumentTempleteDto.Content, PartiesTable(DocumentTempleteDto.lang, DocumentTempleteDto.parties, 1), null, null, null, null, null, null, null);
            if (DocumentTempleteDto.TransactionNo != null)
                QrFileName = await generateQRAsync(DocumentTempleteDto.RecQRUrl, DocumentTempleteDto.TransactionNo, Color.Black);
            else QrFileName = null;
            string header = GetHeader(QrFileName, DocumentTempleteDto.lang, DocumentTempleteDto.TransactionNo, DocumentTempleteDto.Title, DocumentTempleteDto.FileName, "gold");


            GlobalSettings GlobalSettings = new GlobalSettings { Out = DocumentTempleteDto.FileName, Margins = new MarginSettings { Bottom = 43, Top = 43, Right = 10, Left = 10 } };
            if(template != null && template.DocumentTypeId==POAId)
            GetHtmlToPdfDocument(new List<PdfPageSetting> { new PdfPageSetting { BodyDoc = docBody, HeaderUrl = header, FooterUrl = GetFooter("BlockChain") } }, GlobalSettings, DocumentTempleteDto.path);
            else
                GetHtmlToPdfDocument(new List<PdfPageSetting> { new PdfPageSetting { BodyDoc = docBody, HeaderUrl = header, FooterUrl = GetFooter("gold") } }, GlobalSettings, DocumentTempleteDto.path);
        }
        public async Task risalatTabligh(TemplateInfoDto risalatTablighDto)
        {
            string QrFileName;
            string EnotarySignUrl;
            try
            {
                EnotarySignUrl = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), risalatTablighDto.notaryInfo.SignUrl);
            }
            catch (Exception e) { EnotarySignUrl = ""; }
            string EnotaryInfo = SetBodyDocument(risalatTablighDto.lang, "EnotaryInfo.html", risalatTablighDto.notaryInfo.FullName, EnotarySignUrl, null, null, null, null, null, null, null, null);
            string docBody = SetBodyDocument(risalatTablighDto.lang, "risalatTabligh.html", PartiesTable(risalatTablighDto.lang, risalatTablighDto.parties, 0), risalatTablighDto.Content, EnotaryInfo, null, null, null, null, null, null, null);//to do
            if (risalatTablighDto.TransactionNo != null)
                QrFileName = await generateQRAsync(risalatTablighDto.RecQRUrl, risalatTablighDto.TransactionNo, Color.Black);
            else QrFileName = null;
            string header = GetHeader(QrFileName, risalatTablighDto.lang, risalatTablighDto.TransactionNo, risalatTablighDto.Title, risalatTablighDto.FileName, "gold");
            GlobalSettings GlobalSettings = new GlobalSettings { Out = risalatTablighDto.FileName, Margins = new MarginSettings { Bottom = 43, Top = 43, Right = 10, Left = 10 } };
            GetHtmlToPdfDocument(new List<PdfPageSetting> { new PdfPageSetting { BodyDoc = docBody, HeaderUrl = header, FooterUrl = GetFooter("gold") } }, GlobalSettings, risalatTablighDto.path);
        }
        public async Task EkhtarAdli(TemplateInfoDto mudirTasdiqDto)
        {
            string QrFileName;
            string docBody = SetBodyDocument(mudirTasdiqDto.lang, "EkhtarAdli.html", PartiesTable(mudirTasdiqDto.lang, mudirTasdiqDto.parties, 0), mudirTasdiqDto.Content, PartiesTable(mudirTasdiqDto.lang, mudirTasdiqDto.parties, 1), null, null, null, null, null, null, null);
            if (mudirTasdiqDto.TransactionNo != null)
                QrFileName = await generateQRAsync(mudirTasdiqDto.RecQRUrl, mudirTasdiqDto.TransactionNo, Color.Black);
            else QrFileName = null;
            string header = GetHeader(QrFileName, mudirTasdiqDto.lang, mudirTasdiqDto.TransactionNo, mudirTasdiqDto.Title, mudirTasdiqDto.FileName, "gold");
            GlobalSettings GlobalSettings = new GlobalSettings { Out = mudirTasdiqDto.FileName, Margins = new MarginSettings { Bottom = 43, Top = 43, Right = 10, Left = 10 } };
            GetHtmlToPdfDocument(new List<PdfPageSetting> { new PdfPageSetting { BodyDoc = docBody, HeaderUrl = header, FooterUrl = GetFooter("gold") } }, GlobalSettings, mudirTasdiqDto.path);
        }
        public async Task MuhdirTasdiq(TemplateInfoDto mudirTasdiqDto)
        {
            SysLookupType DocumentType = _EnotaryDBContext.SysLookupType.Where(x => x.Value == "document_type").FirstOrDefault();
            int documentTypeId = DocumentType.Id;
            int POAId = (from lv in _EnotaryDBContext.SysLookupValue.Where(x => x.LookupTypeId == documentTypeId)
                         join tr in _EnotaryDBContext.SysTranslation on lv.Shortcut equals tr.Shortcut
                         where tr.Value.Contains("Power of Attorney")
                         select new { id = lv.Id }
                ).FirstOrDefault().id;

            Application application = _EnotaryDBContext.Application.Where(x => x.Id == mudirTasdiqDto.appID).FirstOrDefault();
            Template template = _EnotaryDBContext.Template.Where(x => x.Id == application.TemplateId).FirstOrDefault();


            string EnotarySignUrl;
            string MeetingDateString;
            string QrFileName;
            try
            {
                EnotarySignUrl = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), mudirTasdiqDto.notaryInfo.SignUrl);
            }
            catch (Exception e) { EnotarySignUrl = ""; }
            try
            {
                MeetingDateString = mudirTasdiqDto.MeetingDate.ToString("yyyy-MM-dd");
            }
            catch (Exception e) { MeetingDateString = ""; }

            string EnotaryInfo = SetBodyDocument(mudirTasdiqDto.lang, "EnotaryInfo.html", mudirTasdiqDto.notaryInfo.FullName, EnotarySignUrl, null, null, null, null, null, null, null, null);
            string DocumentInfo = SetBodyDocument(mudirTasdiqDto.lang, "DocumentInfo.html", mudirTasdiqDto.PaymentFee.ToString(), mudirTasdiqDto.ReceiptNumber, mudirTasdiqDto.PaymentDate, null, null, null, null, null, null, null);
            string docBody = SetBodyDocument(mudirTasdiqDto.lang, "muhdirTasdiq.html", MeetingDateString, PartiesTable(mudirTasdiqDto.lang, mudirTasdiqDto.parties, 2), mudirTasdiqDto.Content, DocumentInfo, EnotaryInfo, null, null, null, null, null);
            if (mudirTasdiqDto.TransactionNo != null)
                QrFileName = await generateQRAsync(mudirTasdiqDto.RecQRUrl, mudirTasdiqDto.TransactionNo, Color.Black);
            else QrFileName = null;
            string header = GetHeader(QrFileName, mudirTasdiqDto.lang, mudirTasdiqDto.TransactionNo, mudirTasdiqDto.Title, mudirTasdiqDto.FileName, "gold");
            GlobalSettings GlobalSettings = new GlobalSettings { Out = mudirTasdiqDto.FileName, Margins = new MarginSettings { Bottom = 43, Top = 43, Right = 10, Left = 10 } };
            if (template.DocumentTypeId == POAId)
                GetHtmlToPdfDocument(new List<PdfPageSetting> { new PdfPageSetting { BodyDoc = docBody, HeaderUrl = header, FooterUrl = GetFooter("BlockChain") } }, GlobalSettings, mudirTasdiqDto.path);
            else
                GetHtmlToPdfDocument(new List<PdfPageSetting> { new PdfPageSetting { BodyDoc = docBody, HeaderUrl = header, FooterUrl = GetFooter("gold") } }, GlobalSettings, mudirTasdiqDto.path);

        }
        public async Task EmptyDoc(TemplateInfoDto EmptyDoc)
        {
            string QrFileName;
            string docBody = SetBodyDocument(EmptyDoc.lang, "EmptyDoc.html", ".", null, null, null, null, null, null, null, null, null);

            if (EmptyDoc.TransactionNo != null)
                QrFileName = await generateQRAsync(EmptyDoc.RecQRUrl, EmptyDoc.TransactionNo, Color.Black);
            else QrFileName = null;
            string header = GetHeader(QrFileName, EmptyDoc.lang, EmptyDoc.TransactionNo, EmptyDoc.Title, EmptyDoc.FileName, "gold");
            GlobalSettings GlobalSettings = new GlobalSettings { Out = EmptyDoc.FileName, Margins = new MarginSettings { Bottom = 43, Top = 43, Right = 10, Left = 10 } };
            GetHtmlToPdfDocument(new List<PdfPageSetting> { new PdfPageSetting { BodyDoc = docBody, HeaderUrl = header, FooterUrl = GetFooter("gold") } }, GlobalSettings, EmptyDoc.path);

        }
        public async Task akhtarTanfiz(TemplateInfoDto akhtarTanfizDto)
        {
            string EnotarySignUrl;
            string MeetingDateString;
            string QrFileName;
            try
            {
                EnotarySignUrl = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), akhtarTanfizDto.notaryInfo.SignUrl);
            }
            catch (Exception e) { EnotarySignUrl = ""; }
            try
            {
                MeetingDateString = akhtarTanfizDto.MeetingDate.ToString("yyyy-MM-dd");
            }
            catch (Exception e) { MeetingDateString = ""; }

            string EnotaryInfo = SetBodyDocument(akhtarTanfizDto.lang, "EnotaryInfo.html", akhtarTanfizDto.notaryInfo.FullName, EnotarySignUrl, null, null, null, null, null, null, null, null);
            string akhtarTanfizPayInfo = SetBodyDocument(akhtarTanfizDto.lang, "akhtarTanfizPayInfo.html", akhtarTanfizDto.PaymentFee.ToString() + "  درهم  ", MeetingDateString, akhtarTanfizDto.PaymentDate + " , " + akhtarTanfizDto.ReceiptNumber, null, null, null, null, null, null, null);
            string docBody = SetBodyDocument(akhtarTanfizDto.lang, "akhtarTanfiz.html", PartiesTable(akhtarTanfizDto.lang, akhtarTanfizDto.parties, 0), akhtarTanfizDto.Content, akhtarTanfizPayInfo, EnotaryInfo, null, null, null, null, null, null);
            if (akhtarTanfizDto.TransactionNo != null)
                QrFileName = await generateQRAsync(akhtarTanfizDto.RecQRUrl, akhtarTanfizDto.TransactionNo, Color.Black);
            else QrFileName = null;
            string header = GetHeader(QrFileName, akhtarTanfizDto.lang, akhtarTanfizDto.TransactionNo, akhtarTanfizDto.Title, akhtarTanfizDto.FileName, "red");
            GlobalSettings GlobalSettings = new GlobalSettings { Out = akhtarTanfizDto.FileNameWithoutSign, Margins = new MarginSettings { Bottom = 43, Top = 43, Right = 10, Left = 10 } };
            GetHtmlToPdfDocument(new List<PdfPageSetting> { new PdfPageSetting { BodyDoc = docBody, HeaderUrl = header, FooterUrl = GetFooter("red") } }, GlobalSettings, akhtarTanfizDto.path);

            pageNumberingAndSign(akhtarTanfizDto.FileName, akhtarTanfizDto.path, akhtarTanfizDto.FileNameWithoutSign, false, true);
        }
        public async Task suraTubiqAlaasl(TemplateInfoDto suraTubiqAlaaslDto)
        {
            string EnotarySignUrl;
            string MeetingDateString;
            string QrFileName;
            try
            {
                EnotarySignUrl = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), suraTubiqAlaaslDto.notaryInfo.SignUrl);
            }
            catch (Exception e) { EnotarySignUrl = ""; }
            try
            {
                MeetingDateString = suraTubiqAlaaslDto.MeetingDate.ToString("yyyy-MM-dd");
            }
            catch (Exception e) { MeetingDateString = ""; }

            string EnotaryInfo = SetBodyDocument(suraTubiqAlaaslDto.lang, "EnotaryInfo.html", suraTubiqAlaaslDto.notaryInfo.FullName, EnotarySignUrl, null, null, null, null, null, null, null, null);
            string suraTubiqAlaaslInfo = SetBodyDocument(suraTubiqAlaaslDto.lang, "akhtarTanfizPayInfo.html", suraTubiqAlaaslDto.PaymentFee.ToString() + "  درهم  ", MeetingDateString, suraTubiqAlaaslDto.PaymentDate + " , " + suraTubiqAlaaslDto.ReceiptNumber, null, null, null, null, null, null, null);
            string docBody = SetBodyDocument(suraTubiqAlaaslDto.lang, "suraTubiqAlaasl.html", PartiesTable(suraTubiqAlaaslDto.lang, suraTubiqAlaaslDto.parties, 2), suraTubiqAlaaslDto.Content, suraTubiqAlaaslInfo, EnotaryInfo, null, null, null, null, null, null);
            if (suraTubiqAlaaslDto.TransactionNo != null)
                QrFileName = await generateQRAsync(suraTubiqAlaaslDto.RecQRUrl, suraTubiqAlaaslDto.TransactionNo, Color.Black);
            else QrFileName = null;
            string header = GetHeader(QrFileName, suraTubiqAlaaslDto.lang, suraTubiqAlaaslDto.TransactionNo, suraTubiqAlaaslDto.Title, suraTubiqAlaaslDto.FileName, "gold");
            GlobalSettings GlobalSettings = new GlobalSettings { Out = suraTubiqAlaaslDto.FileNameWithoutSign, Margins = new MarginSettings { Bottom = 43, Top = 43, Right = 10, Left = 10 } };
            GetHtmlToPdfDocument(new List<PdfPageSetting> { new PdfPageSetting { BodyDoc = docBody, HeaderUrl = header, FooterUrl = GetFooter("gold") } }, GlobalSettings, suraTubiqAlaaslDto.path);

            pageNumberingAndSign(suraTubiqAlaaslDto.FileName, suraTubiqAlaaslDto.path, suraTubiqAlaaslDto.FileNameWithoutSign, false, true);
        }
        private async Task CheckIfApplicationExistAsync(string lang, int? appId)
        {
            var res = await _EnotaryDBContext.Application.Where(x => x.Id == appId).ToListAsync();
            if (res.Count == 0)

                throw new System.InvalidOperationException(Constants.getMessage(lang, "ApplicationNotFound"));
        }
        public async Task<DocToPdfDto> ConverterDTOAsync(int appId)
        {
            partiesInfo result = null;
            List<ApplicationPartyFinalDocumentGrouped> a = null;
            DocToPdfDto DocToPdfDto = null;
            try
            {
                result = await _IApplicationRepositiory.PartyFinalDocument(appId);
                if (result != null)
                {
                    DocToPdfDto = new DocToPdfDto();
                    a = result.parties;
                    DocToPdfDto.parties = new List<Party>();
                    DocToPdfDto.TemplateName = result.TemplateName;
                    DocToPdfDto.transactionText = result.transactionText;
                    DocToPdfDto.FileName = result.FileName;// New Line by Yhab...
                    DocToPdfDto.TransactionNo = result.TransactionNo;
                    DocToPdfDto.ApplicationNo = result.ApplicationNo;//
                    DocToPdfDto.decisionText = result.decisionText;// 
                    DocToPdfDto.DocumentType = result.DocumentType;
                    DocToPdfDto.notaryInfo = result.notaryInfo;
                    DocToPdfDto.relatedContents = new List<RelatedContentView>();
                    DocToPdfDto.relatedContents = result.relatedContents;
                    DocToPdfDto.relatedTransactions = result.relatedTransactions;
                    DocToPdfDto.Title = result.Title;
                    DocToPdfDto.ServiceResult = result.ServiceResult;
                    DocToPdfDto.MeetingDate = result.MeetingDate;
                    DocToPdfDto.appID = appId;
                    var paymentsId = await _EnotaryDBContext.Payment.Where(x => x.ApplicationId == appId).Select(x => x.Id).ToListAsync();
                    int paymentId = paymentsId.LastOrDefault();
                    PaymentDto paymentDto = await _PaymentRepository.GetPaymentInfo(paymentId, "ar");
                    if (paymentDto != null)
                    {
                        try
                        {
                            DocToPdfDto.PaymentDate = (DateTime)paymentDto.DatePayment;
                        }
                        catch (Exception)
                        { DocToPdfDto.PaymentDate = null; }
                        DocToPdfDto.PaymentFee = paymentDto.Total;
                        DocToPdfDto.ReceiptNumber = paymentDto.InvoiceNo;
                    }
                    if (result.ServiceResult != 0)
                    {
                        foreach (ApplicationPartyFinalDocumentGrouped ApplicationPartyFinalDocumentGrouped in a)
                        {
                            Party party = Converter(ApplicationPartyFinalDocumentGrouped);
                            DocToPdfDto.parties.Add(party);
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return DocToPdfDto;


        }
        Party Converter(ApplicationPartyFinalDocumentGrouped ApplicationPartyFinalDocumentGrouped)
        {
            Party Party = new Party();
            Party.fullName = ApplicationPartyFinalDocumentGrouped.FullName;
            Party.partyType = ApplicationPartyFinalDocumentGrouped.PartyType;
            Party.nationality = ApplicationPartyFinalDocumentGrouped.Nationality;
            Party.signtureUrl = ApplicationPartyFinalDocumentGrouped.SignUrl;
            Party.address = ApplicationPartyFinalDocumentGrouped.Address;
            Party.phoneNumber = ApplicationPartyFinalDocumentGrouped.Mobile;
            Party.SignRequired = ApplicationPartyFinalDocumentGrouped.SignRequired;
            Party.documentNumber = ApplicationPartyFinalDocumentGrouped.AttachmentNo == null ? ApplicationPartyFinalDocumentGrouped.AttachmentsList[0].AttachmentNo : ApplicationPartyFinalDocumentGrouped.AttachmentNo; ;
            Party.documentType = ApplicationPartyFinalDocumentGrouped.AttachmentName == null ? ApplicationPartyFinalDocumentGrouped.AttachmentsList[0].AttachmentName : ApplicationPartyFinalDocumentGrouped.AttachmentName;
            Party.countryOfIssue = ApplicationPartyFinalDocumentGrouped.CountryOfIssue == null ? ApplicationPartyFinalDocumentGrouped.AttachmentsList[0].CountryOfIssue : ApplicationPartyFinalDocumentGrouped.CountryOfIssue; ;


            return Party;
        }
        public async Task<string> generateQRAsync(string text, string TransactionNo,Color color)
        {
        

            if (text == null) return null;

            string FileName = String.Concat(Convert.ToString(Guid.NewGuid()), ".png");
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20, color, Color.White, true);

            qrCodeImage.Save(Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "QRCode", FileName), ImageFormat.Png);
            try
            {
                AppTransaction AppTransaction = await _EnotaryDBContext.AppTransaction.Where(x => x.TransactionNo == TransactionNo).FirstOrDefaultAsync();
                if (AppTransaction != null)
                {
                    if (AppTransaction.Qrcode == null || AppTransaction.Qrcode == "")
                    {
                        AppTransaction.Qrcode = Path.Combine("QRCode", FileName);// ImageFormat.Png);// "QRCode/" +FileName;
                        _EnotaryDBContext.Update(AppTransaction);

                        if (_EnotaryDBContext.SaveChanges() > 0)
                        {
                            Console.WriteLine("saved");

                        }

                    }
                }
            }
            catch (Exception e) { }
            return FileName;
        }


        public string generateQRAsyncManual(string text)
        {

            if (text == null) return null;

            string FileName = String.Concat(Convert.ToString(Guid.NewGuid()), ".png");
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            qrCodeImage.Save(Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "QRCode", FileName), ImageFormat.Png);
            //try
            //{
            //    AppTransaction AppTransaction = await _EnotaryDBContext.AppTransaction.Where(x => x.TransactionNo == TransactionNo).FirstOrDefaultAsync();
            //    if (AppTransaction != null)
            //    {
            //        if (AppTransaction.Qrcode == null || AppTransaction.Qrcode == "")
            //        {
            //            AppTransaction.Qrcode = Path.Combine("QRCode", FileName);// ImageFormat.Png);// "QRCode/" +FileName;
            //            _EnotaryDBContext.Update(AppTransaction);

            //            if (_EnotaryDBContext.SaveChanges() > 0)
            //            {
            //                Console.WriteLine("saved");

            //            }

            //        }
            //    }
            //}
            //catch (Exception e) { }
            return FileName;
        }
        private string ReturnNumberOFpages(string path)
        {
            try
            {
                PdfReader pdfReader = new PdfReader(Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), path));
                var pageCount = pdfReader.NumberOfPages.ToString();
                pdfReader.Close();
                pdfReader.Dispose();
                return pageCount;
            }
            catch (Exception e) { return ""; }
        }
        public string MergePdfs(string lang, List<string> imageArray, List<string> fileArray, GlobalSettings GlobalSettings)
        {

            List<PdfPageSetting> PdfPageSettingList = new List<PdfPageSetting>();
            if (fileArray != null && imageArray != null)
            {
                string PathFolderContainFile = Constants.transactionsPath;// imageArray[0].Substring(0, imageArray[0].LastIndexOf("\\"));
                CheckIfFilesExist(lang, fileArray);
                CheckIfFilesExist(lang, imageArray);
                string FileName = GetNewPdfFileName();
                ObjectSettings[] PagesSettingsArr = new ObjectSettings[imageArray.Count];
                int i = 0;
                foreach (string ImagePath in imageArray)
                {
                    string RecordTitleTable = SetBodyDocument(lang, "ImageAsHtml.html", ImagePath, null, null, null, null, null, null, null, null, null);
                    PdfPageSetting pdfPageSetting = new PdfPageSetting { BodyDoc = RecordTitleTable, HeaderUrl = null, FooterUrl = null };
                    PdfPageSettingList.Add(pdfPageSetting);
                }
                GetHtmlToPdfDocument(PdfPageSettingList, GlobalSettings, PathFolderContainFile);

                fileArray.Add(System.IO.Path.Combine(Constants.transactionsFolder, FileName));
                string subpath = MergePDF(fileArray, Path.Combine(PathFolderContainFile, GlobalSettings.Out));
                return subpath;
            }
            if (fileArray != null && imageArray == null)
            {
                string disPathFolderContainFile = Constants.transactionsPath; //fileArray[0].Substring(0, fileArray[0].LastIndexOf("\\"));
                CheckIfFilesExist(lang, fileArray);
                string subpath = MergePDF(fileArray, Path.Combine(disPathFolderContainFile, GlobalSettings.Out));
                return subpath;
            }
            if (fileArray == null && imageArray != null)
            {
                string PathFolderContainFile = Constants.transactionsPath;// imageArray[0].Substring(0, imageArray[0].LastIndexOf("\\"));
                CheckIfFilesExist(lang, imageArray);
                //string FileName = GetNewPdfFileName();
                ObjectSettings[] PagesSettingsArr = new ObjectSettings[imageArray.Count];
                int i = 0;
                foreach (string ImagePath in imageArray)
                {
                    string RecordTitleTable = SetBodyDocument(lang, "ImageAsHtml.html", Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), ImagePath), null, null, null, null, null, null, null, null, null);
                    PdfPageSetting pdfPageSetting = new PdfPageSetting { BodyDoc = RecordTitleTable, HeaderUrl = "", FooterUrl = "" };
                    PdfPageSettingList.Add(pdfPageSetting);
                }
                GetHtmlToPdfDocument(PdfPageSettingList, GlobalSettings, PathFolderContainFile);
                string sourceFile = System.IO.Path.Combine(PathFolderContainFile, GlobalSettings.Out);
                return sourceFile;
            }
            return null;
        }
        public void CheckIfFilesExist(string lang, List<string> paths)
        {
            foreach (string path in paths)
                if (!CheckIfFileExist(path)) throw new Exception(Constants.getMessage(lang, "FileNotFound"));
        }
        public bool CheckIfFileExist(string path)
        {
            string d = _IFilesUploaderRepositiory.GetRootPath();
            bool exist = File.Exists(Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), path));//"./wwwroot/" + path
            return exist;
        }
        private string MergePDF(List<string> fileArray, string FileName)
        {

            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage;
            string outputPdfPath = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), FileName);
            sourceDocument = new Document();
            pdfCopyProvider = new PdfCopy(sourceDocument, new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create));
            sourceDocument.Open();
            for (int f = 0; f < fileArray.Count; f++)
            {
                int pages = TotalPageCount(Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), fileArray[f]));
                reader = new PdfReader(Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), fileArray[f]));
                for (int i = 1; i <= pages; i++)
                {
                    importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                    pdfCopyProvider.AddPage(importedPage);
                }
                reader.Close();
            }
            sourceDocument.Close();
            return FileName;
        }
        private int TotalPageCount(string file)
        {
            PdfReader pdfReader = new PdfReader(System.IO.File.OpenRead(Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), file)));
            int numberOfPages = pdfReader.NumberOfPages;
            return numberOfPages;

            //using (StreamReader sr = new StreamReader(System.IO.File.OpenRead(Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), file))))
            //{
            //    Regex regex = new Regex(@"/Type\s*/Page[^s]");
            //    MatchCollection matches = regex.Matches(sr.ReadToEnd());
            //    return matches.Count;
            //}
        }
        //private static System.Drawing.Image GetPageImage(int pageNumber, Size size, PdfiumViewer.PdfDocument document, int dpi)
        //{
        //    return document.Render(pageNumber - 1, size.Width, size.Height, dpi, dpi, PdfRenderFlags.Annotations);
        //}
        public List<string> ConvertPdfToImages(string lang, string pdfPath)
        {
            try
            {
                CheckIfFilesExist(lang, new List<string> { pdfPath });
                List<string> ImagePaths = new List<string>();
                Size size = new Size() { Height = 300, Width = 300 };
                int NumberOfPages = Convert.ToInt32(ReturnNumberOFpages(pdfPath));
                for (int i = 0; i < NumberOfPages; i++)
                {
                    using (var document = PdfiumViewer.PdfDocument.Load(Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), pdfPath)))
                    {
                        string guid = System.Guid.NewGuid().ToString();
                        string ImageName = guid + ".png";
                        string outputPath = System.IO.Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "TempPhotos", ImageName);
                        var image = document.Render(i, size.Width, size.Height, PdfRenderFlags.CorrectFromDpi| PdfRenderFlags.ForPrinting);
                        image.Save(outputPath, ImageFormat.Png);
                        ImagePaths.Add(outputPath);
                    }
                }
                return ImagePaths;
            }

            catch (Exception e)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "ConvertImageToPDF"));
                throw _exception;
            }
        }
        public async Task<string> AddFooterAndHeaderToOldTransactionAsync(string lang, InfoDoc infoDoc)
        {

            string QrFileName;
            try
            {
                List<PdfPageSetting> PdfPageSettingList = new List<PdfPageSetting>();
                CheckIfFilesExist(lang, infoDoc.ImagePaths);
                string FileName = GetNewPdfFileName();
                ObjectSettings[] PagesSettingsArr = new ObjectSettings[infoDoc.ImagePaths.Count];
                if (infoDoc.TransactionNo != null)
                    QrFileName = await generateQRAsync(infoDoc.RecQRUrl, infoDoc.TransactionNo, Color.Black);
                else QrFileName = null;
                string header = GetHeader(QrFileName, lang, infoDoc.TransactionNo, infoDoc.Title, FileName, "gold");
                foreach (string ImagePath in infoDoc.ImagePaths)
                {
                    string BodyDoc = SetBodyDocument(lang, "EmptyDoc.html", ImagePath, null, null, null, null, null, null, null, null, null);
                    PdfPageSetting pdfPageSetting = new PdfPageSetting { BodyDoc = BodyDoc, HeaderUrl = header, FooterUrl = GetFooter("gold") };
                    PdfPageSettingList.Add(pdfPageSetting);
                }
                GlobalSettings GlobalSettings = new GlobalSettings { Out = FileName, Margins = new MarginSettings { Bottom = 43, Top = 43, Right = 10, Left = 10 } };

                GetHtmlToPdfDocument(PdfPageSettingList, GlobalSettings, infoDoc.path);
                //string FileNameAfterNumbering = GetNewPdfFileName();
                //pageNumbering(FileNameAfterNumbering, path, FileName);                  
                return FileName;
            }
            catch (Exception e)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "SignPDF"));
                throw _exception;
            }
        }

      public async   Task<int> Oldtranc(string lang,string path,int id)
        {
            AppTransaction app =  _EnotaryDBContext.AppTransaction.Where(x => x.ApplicationId == id).FirstOrDefault();
            //foreach(var app in appTransactions)
            //{
                AutoCreatePdfPaths paths=await     autoCreatePDFAsync(lang, (int)app.ApplicationId, path);
                string sr = Path.GetDirectoryName(app.FileName);
                _IFilesUploaderRepositiory.MoveFile(paths.TransactionDoc, sr + "/infoCert.pdf");
                AppTransaction appTransaction = _EnotaryDBContext.AppTransaction.Where(x => x.ApplicationId == app.ApplicationId).FirstOrDefault();

                appTransaction.DocumentUrl = sr + "/infoCert.pdf";
                
                _IGeneralRepository.Update(appTransaction);
              await  _IGeneralRepository.Save();
                int x = 0;
          //  }
            return 1;
        }

        public async Task<APIResult> CreateMergedPDF(string lang,int id)
        {
            APIResult ApiResult = new APIResult();
          
            string path = Constants.transactionsPath;
            try
            {
                ApplicationAttachment applicationAttachment = _EnotaryDBContext.ApplicationAttachment.Where(x => x.ApplicationId == id).FirstOrDefault();
                Application application = _EnotaryDBContext.Application.Where(x => x.Id == id).FirstOrDefault();

                string appFolder = Path.GetDirectoryName(applicationAttachment.FileName);
                string targetPDF = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), appFolder);
                // File.Create(targetPDF + "/merged.pdf").Dispose();

                //using (FileStream stream = new FileStream(targetPDF + "/merged.pdf", FileMode.OpenOrCreate))
                //{

                Document pdfDoc = new Document(PageSize.A4);
                PdfCopy pdf = new PdfCopy(pdfDoc, new System.IO.FileStream(targetPDF + "/merged.pdf", System.IO.FileMode.Create));
                pdfDoc.Open();

                bool footer = _IFilesUploaderRepositiory.FileExist(targetPDF + "\\footer.pdf");
                bool coverpage = _IFilesUploaderRepositiory.FileExist(targetPDF + "\\coverpage.pdf");
                bool covertemp = _IFilesUploaderRepositiory.FileExist(targetPDF + "\\coverpage_TEMP" + application.ApplicationNo + ".pdf");
                if (footer && coverpage && covertemp)
                {
                    PdfReader coverpage_TEMP_pdfReader = new PdfReader(targetPDF + "\\coverpage_TEMP" + application.ApplicationNo + ".pdf");
                    pdf.AddDocument(coverpage_TEMP_pdfReader);
                    coverpage_TEMP_pdfReader.Close();
                    PdfReader footer_pdfReader = new PdfReader(targetPDF + "\\footer.pdf");
                    pdf.AddDocument(footer_pdfReader);
                    footer_pdfReader.Close();
                    PdfReader coverpage_pdfReader = new PdfReader(targetPDF + "\\coverpage.pdf");
                    pdf.AddDocument(coverpage_pdfReader);
                    coverpage_pdfReader.Close();
                    //  pdf.AddDocument(new PdfReader(targetPDF + "\\coverpage_TEMP" + application.ApplicationNo + ".pdf"));
                    // pdf.AddDocument(new PdfReader(targetPDF + "\\footer.pdf"));
                    // pdf.AddDocument(new PdfReader(targetPDF + "\\coverpage.pdf"));


                }
                else if (!footer && covertemp && coverpage)
                {
                    PdfReader coverpage_TEMP_pdfReader = new PdfReader(targetPDF + "\\coverpage_TEMP" + application.ApplicationNo + ".pdf");
                    pdf.AddDocument(coverpage_TEMP_pdfReader);
                    coverpage_TEMP_pdfReader.Close();
                    //PdfReader footer_pdfReader = new PdfReader(targetPDF + "\\footer.pdf");
                    //pdf.AddDocument(footer_pdfReader);
                    //footer_pdfReader.Close();
                    PdfReader coverpage_pdfReader = new PdfReader(targetPDF + "\\coverpage.pdf");
                    pdf.AddDocument(coverpage_pdfReader);
                    coverpage_pdfReader.Close();

                    //pdf.AddDocument(new PdfReader(targetPDF + "\\coverpage_TEMP" + application.ApplicationNo + ".pdf"));
                    ////   pdf.AddDocument(new PdfReader(targetPDF + "\\footer.pdf"));
                    //pdf.AddDocument(new PdfReader(targetPDF + "\\coverpage.pdf"));

                }
                else
                {
                    ApiResult.Id = -1;
                    ApiResult.Message.Add(Constants.getMessage("ar", "oldCertificate"));
                }
                //    i++;
                //}
                pdf.Flush();
                pdfDoc.Close();
                if (pdfDoc != null)
                    pdfDoc.Close();
                GC.Collect();
                pdfDoc.Dispose();
                AppTransaction appTransaction = _EnotaryDBContext.AppTransaction.Where(x => x.ApplicationId == id).FirstOrDefault();
                appTransaction.FileName = appFolder + "\\merged.pdf";
                _IGeneralRepository.Update(appTransaction);



                if (await _IGeneralRepository.Save())
                {
                    // stream.Flush();
                    AppTransaction app = _EnotaryDBContext.AppTransaction.Where(x => x.ApplicationId == id).FirstOrDefault();

                    AutoCreatePdfPaths paths = await autoCreatePDFAsync("en", (int)app.ApplicationId, path);
                    string sr = Path.GetDirectoryName(app.FileName);
                    _IFilesUploaderRepositiory.MoveFile(paths.TransactionDoc, sr + "/infoCert.pdf");
                    AppTransaction appTransaction1 = _EnotaryDBContext.AppTransaction.Where(x => x.ApplicationId == app.ApplicationId).FirstOrDefault();

                    appTransaction1.DocumentUrl = sr + "/infoCert.pdf";
                    appTransaction1.Exist = true;

                    _IGeneralRepository.Update(appTransaction1);
                    if (await _IGeneralRepository.Save())
                    {
                        ApiResult.Id = 1;
                        ApiResult.Result = application.RowVersion;
                        ApiResult.Message.Add(Constants.getMessage("ar", "createCertSuccess"));

                    }
                    else
                    {
                        ApiResult.Id = -1;
                        ApiResult.Message.Add(Constants.getMessage("ar", "createCertError"));


                    }

                }
                //    stream.Close();



                //  }


                return ApiResult;
            }
            catch(Exception ex)
                {
                ApiResult.Id = -1;
                ApiResult.Message.Add("خطأ في في عملية الدمج يرجى ارسال تفاصيل الخطأ الى مدير النظام");
                return ApiResult;
            }
        }


        public async Task<APIResult> CreateAdDocument(int appId)
        {
            Application app = await _EnotaryDBContext.Application.Where(x => x.Id == appId).Include(x => x.AppTransaction).FirstOrDefaultAsync();
            AppTransaction transactions = app.AppTransaction;
            var RecQRUrl = generateQRCodeURL(_Pdfdocumentsetting.ADVBaseURL, new List<string> { "ObjectForm", appId.ToString() });
            var QrFileName = await generateQRAsync(RecQRUrl, appId.ToString(), Color.Black);
            var QrFilePath = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "QRCode", QrFileName);
            var sb = new StringBuilder();
            sb.AppendFormat(@"    
     <!DOCTYPE html>
     <html> <head><title></title>    
    
</head>
 
<body style='direction: rtl;margin-left:175px;margin-right:175px;padding-top: 150px;'>
<div style='border:1px solid #555555;padding:10px;padding-right:20px;padding-left:20px;'>   
<h2 style='text-align:center'>{0}</h2>
   <div>
     <div>
       <img src = '{2}'  width = '125px' height = '125px' style='float: left;margin: 5px;margin-top:0px;margin-bottom:0px;border:1px solid #555555' >
     </div>
    <p style='text-align:justify;font-size:14px;'>{1}</p>
   </div>
</div>
</body></html>", transactions.Title, transactions.Content, QrFilePath);

            string path = Path.Combine("applications", Path.Combine(app.ServiceId.ToString(), appId.ToString()), "Ad_" + appId.ToString() + ".pdf");
            string fullPath = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), path);
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report",
                Out = fullPath,
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = sb.ToString(),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), "style.css") },
                HeaderSettings = null,
                FooterSettings = null
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            _IConverter.Convert(pdf);

            APIResult result = new APIResult();
            result.Id = appId;
            result.Result = path;
            return result;
        }

    }
}
