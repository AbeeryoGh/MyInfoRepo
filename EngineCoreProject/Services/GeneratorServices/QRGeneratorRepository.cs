using EngineCoreProject.DTOs.PDFGenerator;
using EngineCoreProject.IRepository.IGeneratorRepository;
using EngineCoreProject.Models;
using Microsoft.Extensions.Options;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.GeneratorServices
{
    public class QRGeneratorRepository : IQRrepository
    {//, IOptions<FileNaming> fileNaming  //private readonly FileNaming _FileNaming;
        private readonly FileNaming _FileNaming;
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _IGeneralRepository;
        public QRGeneratorRepository(EngineCoreDBContext EngineCoreDBContext , IOptions<FileNaming> fileNaming, IGeneralRepository iGeneralRepository)
        {
            
            _EngineCoreDBContext = EngineCoreDBContext;
            _FileNaming = fileNaming.Value;
            _IGeneralRepository = iGeneralRepository;
        }
        public   Byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public string generateQR(string text)
        {
            string FileName = _FileNaming.TermQRCodeFileName + _IGeneralRepository.GetNewValueBySec() + ".png"; ;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            qrCodeImage.Save(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/QRCode", FileName), ImageFormat.Png);
            
            return   FileName;


        }
    }
}
