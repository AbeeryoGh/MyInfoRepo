
using EngineCoreProject.DTOs.BlockChainDto;
using EngineCoreProject.DTOs.Credential;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.ICredential;
using EngineCoreProject.IRepository.IFilesUploader;
using EngineCoreProject.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.BlockChain
{
    public class BlockChainRepository : IBlockChain
    {

        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly ISysValueRepository _ISysValueRepository;
        ValidatorException _exception;
        private readonly ILogger<BlockChainRepository> _logger;
        private IConfiguration _IConfiguration;
        private readonly IFilesUploaderRepositiory _iFilesUploaderRepositiory;
        private readonly ICredentialRepository _ICredentialRepository;


        public BlockChainRepository(ICredentialRepository iCredentialRepository, IFilesUploaderRepositiory iFilesUploaderRepositiory, IConfiguration iConfiguration, ILogger<BlockChainRepository> logger, ISysValueRepository iSysValueRepository, EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository)

        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _ISysValueRepository = iSysValueRepository;
            _exception = new ValidatorException();
            _logger = logger;
            _IConfiguration = iConfiguration;
            _iFilesUploaderRepositiory = iFilesUploaderRepositiory;
            _ICredentialRepository = iCredentialRepository;
        }

        public bool GetVcID(int appid)
        {

            UpdateVCIDResDto updateVCIDResDto = new UpdateVCIDResDto();
            UpdateVCIDDto updateVCIDDto = new UpdateVCIDDto();
            string appNo = _EngineCoreDBContext.Application.Where(x => x.Id == appid).Select(z => z.ApplicationNo).FirstOrDefault();
            string transactionNo = _EngineCoreDBContext.AppTransaction.Where(x => x.ApplicationId == appid).Select(z => z.TransactionNo).FirstOrDefault();
            AppTransaction appTransaction = _EngineCoreDBContext.AppTransaction.Where(x => x.ApplicationId == appid).FirstOrDefault();
            string documentUrl = appTransaction.DocumentUrl;
            if (documentUrl == null)
            {
                _logger.LogInformation(" BlockChain empty document Url ");
                return false;
            }


            GetVcIDDto getVcIDDto = new GetVcIDDto();
            evidenceBlockChain evidenceBlockChain = new evidenceBlockChain();
            claimBlockChain claimBlockChain = new claimBlockChain();
            documentMetaData documentMetaData = new documentMetaData();

            getVcIDDto.transactionRefNo = "1234567890hh115688914015890" + appid.ToString();
            getVcIDDto.documentType = "POA";


            evidenceBlockChain.file = pdfToBase64(Path.Combine(_iFilesUploaderRepositiory.GetRootPath(), documentUrl));
            evidenceBlockChain.fileType = "pdf";
            getVcIDDto.evidence = evidenceBlockChain;

            claimBlockChain.documentId = transactionNo;
            claimBlockChain.applicationId = appNo;
            getVcIDDto.claims = claimBlockChain;

            documentMetaData.documentId = transactionNo;
            documentMetaData.applicationId = appNo;
            getVcIDDto.documentMetaData = documentMetaData;



            using var client = new HttpClient();

            var url = _IConfiguration.GetSection("BlockChainURLs")["RequestVcIDURL"];
            string json = "";
            try
            {
                json = JsonConvert.SerializeObject(getVcIDDto);

                if (json == "")
                {
                    _logger.LogInformation("empty json ");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("BlockChain" + ex.Message);
                return false;
            }
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            try
            {
                client.DefaultRequestHeaders.Add("Authorization", "Basic QXBpQ29kZTpkN2ZhMGM2NDk5YjY0YzZlYjI4Yzg1ODM1ZmYxNGM0NA==");

                httpResponseMessage = client.PostAsync(url, data).Result;
                if (httpResponseMessage == null)
                {
                    _logger.LogInformation(" BlockChain empty httpResponseMessage ");
                    return false;
                }
            }

            catch (System.Net.Http.HttpRequestException ex)
            {

                _logger.LogInformation(ex.Message);
                return false;

            }
            catch (Exception ex)
            {

                _logger.LogInformation(ex.Message);
                return false;
            }
            GetVcIDRes getVcIDRes = new GetVcIDRes();
            string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
            _logger.LogInformation("the result of BlockChain GetVCID :  "+ result);
            try
            {
                getVcIDRes = JsonConvert.DeserializeObject<GetVcIDRes>(result);

                if (getVcIDRes == null)
                {
                    _logger.LogInformation("BlockChain empty result in DeserializeObject " + result);
                    return false;
                }



            }

            catch (Exception ex)
            {
                _logger.LogInformation("BlockChain Error in DeserializeObject " + result + " the error is " + ex.Message);
                return false;
            }

            BlockChainPoa blockChainPoa = _EngineCoreDBContext.BlockChainPoa.Where(x => x.AppId == appid).FirstOrDefault();
            updateVCIDDto.transactionRefNo =  "Eno2021" + appid.ToString();
            updateVCIDDto.VCID = getVcIDRes.vcID;
            updateVCIDDto.transactionRefOfRequest = "1234567890hh115688914015890" + appid.ToString();

            updateVCIDResDto = _ICredentialRepository.UpdateVCID(updateVCIDDto).Result;
            blockChainPoa.Vcid = getVcIDRes.vcID;
            blockChainPoa.IsSentUg = true;
            _iGeneralRepository.Update(blockChainPoa);



            return _iGeneralRepository.Save().Result;
        }

        static string pdfToBase64(string filePath)
        {

            Byte[] bytes = File.ReadAllBytes(filePath);
            string pdfbase64 = Convert.ToBase64String(bytes);
            return pdfbase64;

        }

        public bool RevokevcID(RevokeDto revokeDto, int appid)
        {
            bool Rs = false;

            using var client = new HttpClient();

            var url = _IConfiguration.GetSection("BlockChainURLs")["RevokeVcIDURL"];
            string json = "";
            try
            {
                json = JsonConvert.SerializeObject(revokeDto);

                if (json == "")
                {
                    _logger.LogInformation("blockchain empty json");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("blockchain  " + ex.Message);
                return false;
            }
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            try
            {
                client.DefaultRequestHeaders.Add("Authorization", "Basic QXBpQ29kZTpkN2ZhMGM2NDk5YjY0YzZlYjI4Yzg1ODM1ZmYxNGM0NA==");

                httpResponseMessage = client.PostAsync(url, data).Result;
                if (httpResponseMessage == null)
                {
                    _logger.LogInformation("blockchain empty httpResponseMessage");
                    return false;
                }
            }

            catch (System.Net.Http.HttpRequestException ex)
            {
                _logger.LogInformation("blockchain " + ex.Message);
                return false;

            }
            catch (Exception ex)
            {
                _logger.LogInformation("blockchain " + ex.Message);
                return false;
            }

            string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
            _logger.LogInformation("the result of revoked blockchain POA  :" + result);
            if (result == "200")
            {
                BlockChainPoa blockChainPoa = _EngineCoreDBContext.BlockChainPoa.Where(x => x.AppId == appid).FirstOrDefault();
                blockChainPoa.IsUgCancelled = true;
                _iGeneralRepository.Update(blockChainPoa);
                if (_iGeneralRepository.Save().Result)
                {
                    Rs = true;
                }
                else
                    Rs = false;

            }

            return Rs;


        }

        public async Task<List<BlockChainPoa>> GetBlockChain(string opr)
        {
            List<BlockChainPoa> blockChainPoas = new List<BlockChainPoa>();
            if (opr == "Yes")
            {
                blockChainPoas = await _EngineCoreDBContext.BlockChainPoa.Where(x => (x.Vcid != null && x.IsUgCancelled == true) || (x.Vcid != null && x.IsSysCancelled == false)).ToListAsync();
            }
            else if (opr == "No")
            {
                blockChainPoas = await _EngineCoreDBContext.BlockChainPoa.Where(x => x.Vcid == null || (x.IsSysCancelled == true && x.IsUgCancelled == false)).ToListAsync();
            }
            return blockChainPoas;
        }

        private static string AddFooterToExistPDF(string originalFile, string newFile, string footer)
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

        public async Task<bool> ResendAsync()
        {
            List<BlockChainPoa> blockChainPoas = new List<BlockChainPoa>();
            blockChainPoas = await GetBlockChain("No");
            foreach (var app in blockChainPoas)
            {
                if (app.Vcid == null)
                {
                    GetVcID((int)app.AppId);
                }
                else
                {
                    RevokeDto revokeDto = new RevokeDto()
                    {
                        VcID = app.Vcid,
                        RevocationReason = "Cancel by user"
                    };
                    RevokevcID(revokeDto, (int)app.AppId);
                }
            }
            return false;
        }
    }
}
