 
using DinkToPdf.Contracts;
using EngineCoreProject.DTOs.ConfigureWritableDto;
using EngineCoreProject.DTOs.PDFDto;
using EngineCoreProject.DTOs.PDFGenerator;
using EngineCoreProject.IRepository.IApplicationSetRepository;

using EngineCoreProject.IRepository.IGeneratorRepository;
using EngineCoreProject.IRepository.IPaymentRepository;
using EngineCoreProject.Models;
using EngineCoreProject.Services;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using EngineCoreProject.IRepository.IFilesUploader;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using iText.Layout;
using iText.Kernel.Geom;
using System.Drawing;
using PdfiumViewer;
using EngineCoreProject.DTOs.oldDataDto;

namespace EngineCoreProject.Controllers.Generator
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GeneratorController : ControllerBase
    {
        private IConfiguration Configuration;
        private IConverter _converter;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly FileNaming _FileNaming;
        private readonly IGenerator _iGenerator;
        private readonly IApplicationRepository _IApplicationRepositiory;
        private readonly IPaymentRepository _PaymentRepository;
        private readonly Pdfdocumentsetting _Pdfdocumentsetting;
        private readonly ITransactionRepository _ITransactionRepository;
        private readonly EngineCoreDBContext _EngineCoreDBContext;
    
        private readonly IFilesUploaderRepositiory _IFilesUploaderRepositiory;
        public GeneratorController(IFilesUploaderRepositiory iFilesUploaderRepositiory, EngineCoreDBContext EngineCoreDBContext, ITransactionRepository iTransactionRepository, IOptions<Pdfdocumentsetting> pDFDocumentSetting, IConfiguration configuration, IPaymentRepository paymentRepository, IApplicationRepository iApplicationRepository, IGenerator iPdfGenerator, IGeneralRepository iGeneralRepository, IConverter converter, IOptions<FileNaming> FileNaming)
        {
            _converter = converter;
            _iGeneralRepository = iGeneralRepository;
            _FileNaming = FileNaming.Value;
            _iGenerator = iPdfGenerator;
            _IApplicationRepositiory = iApplicationRepository;
            _PaymentRepository = paymentRepository;
            Configuration = configuration;
            _Pdfdocumentsetting = pDFDocumentSetting.Value;
            _EngineCoreDBContext = EngineCoreDBContext;
         
            _ITransactionRepository = iTransactionRepository;
            _IFilesUploaderRepositiory = iFilesUploaderRepositiory;
          
        }
        [HttpPost("ConvertFilesToPDF")]
        public IActionResult ConvertFilesToPDF([FromBody] FilePathesList FilePathesList)
        {
            string lang = Request.Headers["lang"].ToString().ToLower(); ;
            string output = _iGenerator.GetNewPdfFileName();
            var res = _iGenerator.MergePdfs(lang, FilePathesList.imageArray, FilePathesList.FilesArray, new DinkToPdf.GlobalSettings { Out = output, });

            return this.StatusCode(StatusCodes.Status200OK, res);
        }
        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("CreatePDF")]
        public async Task<IActionResult> CreatePDFAsync([FromBody] DocToPdfDto DocToPdfDto)
        {
            string path = Constants.transactionsPath;
            string lang = Request.Headers["lang"].ToString().ToLower();
            string FileName = _iGenerator.GetNewPdfFileName();

            string Url = null;
            if (DocToPdfDto.TransactionNo == null) Url = null;
            else
            {
                int? AppTransactionId = await _EngineCoreDBContext.AppTransaction.Where(x => x.TransactionNo == DocToPdfDto.TransactionNo).Select(x => x.Id).FirstOrDefaultAsync();
                 Url = _Pdfdocumentsetting.BaseURL + AppTransactionId;
            }
            TemplateInfoDto TemplateInfoDto = new TemplateInfoDto
            {
                parties = DocToPdfDto.parties,
                path = path,
                lang = lang,
                notaryInfo = DocToPdfDto.notaryInfo,
                TransactionNo = DocToPdfDto.TransactionNo,
                RecQRUrl = Url,
                FileName = FileName,
                appID= DocToPdfDto.appID,
                PaymentDate = DocToPdfDto.PaymentDate.ToString(),
                PaymentFee = DocToPdfDto.PaymentFee.ToString(),
                ReceiptNumber = DocToPdfDto.ReceiptNumber

            };
            if (DocToPdfDto.ShortCut == null)
            {
                TemplateInfoDto.Title = DocToPdfDto.Title;
                if(TemplateInfoDto.Title ==null) TemplateInfoDto.Title = DocToPdfDto.TemplateName;

                TemplateInfoDto.Content = DocToPdfDto.transactionText;
            
                await _iGenerator.DocumentTemplete(TemplateInfoDto);
            }
            else if (DocToPdfDto.ShortCut == "RT")
            {
                TemplateInfoDto.Content = DocToPdfDto.relatedContents[0].Content;
                TemplateInfoDto.Title = DocToPdfDto.relatedContents[0].Title;
                await _iGenerator.risalatTabligh(TemplateInfoDto);

            }
            else if (DocToPdfDto.ShortCut == "MT")
            {
                TemplateInfoDto.Content = DocToPdfDto.relatedContents[0].Content;
                TemplateInfoDto.Title = DocToPdfDto.relatedContents[0].Title;
                TemplateInfoDto.MeetingDate = DocToPdfDto.MeetingDate;
                await _iGenerator.MuhdirTasdiq(TemplateInfoDto);

            }
            else if (DocToPdfDto.ShortCut == "EA")
            {
                TemplateInfoDto.Content = DocToPdfDto.relatedContents[0].Content;
                TemplateInfoDto.Title = DocToPdfDto.relatedContents[0].Title;
                await _iGenerator.EkhtarAdli(TemplateInfoDto);

            }

            else if (DocToPdfDto.ShortCut == "EM")
            {
                TemplateInfoDto.Content = DocToPdfDto.relatedContents[0].Content;
                TemplateInfoDto.Title = DocToPdfDto.relatedContents[0].Title;
                await _iGenerator.EmptyDoc(TemplateInfoDto);

            }
            else if (DocToPdfDto.ShortCut == "EX")
            {
                TemplateInfoDto.Content = DocToPdfDto.relatedContents[0].Content;
                TemplateInfoDto.Title = DocToPdfDto.relatedContents[0].Title;
                TemplateInfoDto.MeetingDate = DocToPdfDto.MeetingDate;
                await _iGenerator.akhtarTanfiz(TemplateInfoDto);

            }
            return this.StatusCode(StatusCodes.Status200OK, path + FileName);
        }
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("autoCreatePDF")]
        public async Task<IActionResult> autoCreatePDFAsync(int AppId)
        {
            string path = Constants.transactionsPath;

            string lang = Request.Headers["lang"].ToString().ToLower(); ;
            AutoCreatePdfPaths paths = await _iGenerator.autoCreatePDFAsync(lang, AppId, path);
            
            return this.StatusCode(StatusCodes.Status200OK, paths);
        }

        [HttpPost("ConvertPdfToPages")]
        public IActionResult ConvertPdfToPages()
        {
            string lang = "ar";
            string FileName = "transactions\\a.pdf";
            string pdfPath = System.IO.Path.Combine(_IFilesUploaderRepositiory.GetRootPath(), FileName);
            _iGenerator.ConvertPdfToImages(lang, pdfPath);
            return this.StatusCode(StatusCodes.Status200OK, "ok");
        }


        [HttpPost("DeleteHTMLFiles")]
        public IActionResult DeleteHTMLFiles()
        {
            
            string[] files = System.IO.Directory.GetFiles(System.IO.Path.Combine(_IFilesUploaderRepositiory.GetRootPath(),Constants.transactionsFolder), "*.html");
            foreach (string f in files)
            {
                System.IO.File.Delete(f);
            }
            return this.StatusCode(StatusCodes.Status200OK, files);
        }
        //

        //[HttpPost("oldpdf")]
        //public async Task<IActionResult> oldpdf(int AppId)
        //{
        //    string path = Constants.transactionsPath;

        //    string lang = Request.Headers["lang"].ToString().ToLower(); ;
        //   var result = await _iGenerator.oldtranc(lang, path,AppId);

        //    return this.StatusCode(StatusCodes.Status200OK, result);
        //}

        [HttpPost("MergeCreateOldCert")]
        public async Task<IActionResult> mergepdfsdddd(int id)
        {
            // string path = Constants.transactionsPath;
            string lang = Request.Headers["lang"].ToString().ToLower(); ;
            var result = await _iGenerator.CreateMergedPDF(lang,id);

            return this.StatusCode(StatusCodes.Status200OK, result);
        }



        [HttpPost("createAd/{appId}")]
        public async Task<IActionResult> createAd([FromRoute] int appId)
        {
            var a = await _iGenerator.CreateAdDocument(appId);
            return Ok(a);
        }

    }
}


