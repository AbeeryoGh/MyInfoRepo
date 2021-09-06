using DinkToPdf;
using DinkToPdf.Contracts;
using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.oldDataDto;
using EngineCoreProject.DTOs.Payment;
using EngineCoreProject.DTOs.PDFDto;
using EngineCoreProject.DTOs.PDFGenerator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IGeneratorRepository
{
    public interface IGenerator
    {
        string SetBodyDocument(string lang, string FileName, string Argument1, string Argument2, string Argument3, string Argument4, string Argument5, string Argument6, string Argument7, string Argument8, string Argument9, string Argument10);
        string PartiesTable(string lang, List<Party> parties, int type);
        string GetNewPdfFileName();
        Task<AutoCreatePdfPaths> autoCreatePDFAsync(string lang, int appId, string path);
        string MergePdfs(string lang, List<string> imageArray, List<string> fileArray, GlobalSettings globalSettings);
        Task MuhdirTasdiq(TemplateInfoDto mudirTasdiqDto);
        Task EkhtarAdli(TemplateInfoDto EkhtarAdliDto);
        Task EmptyDoc(TemplateInfoDto mudirTasdiqDto);
        public List<string> ConvertPdfToImages(string lang, string pdfPath);
        Task risalatTabligh(TemplateInfoDto risalatTablighDto);
        Task akhtarTanfiz(TemplateInfoDto akhtarTanfiz);
        Task DocumentTemplete(TemplateInfoDto DocumentTempleteDto);
        Task suraTubiqAlaasl(TemplateInfoDto akhtarTanfizDto);
        Task<int> Oldtranc(string lang, string path, int id);
        Task<APIResult> CreateMergedPDF(string lang, int  id);
        Task<APIResult> CreateAdDocument(int appId);

    }
}
