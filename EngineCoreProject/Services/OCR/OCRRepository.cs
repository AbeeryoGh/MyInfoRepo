using EngineCoreProject.DTOs.OCRDto;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.IRepository.IOCRRepository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.OCR
{

    public class OCRRepository : IOCRRepository
    {
        private readonly IApplicationPartyRepository _IApplicationPartyRepositiory;
        public OCRRepository(IApplicationPartyRepository iApplicationPartyRepositiory)
        {
            _IApplicationPartyRepositiory = iApplicationPartyRepositiory;
        }

        public async Task<OCRGetDto> OCRCard(IFormFile formFile, bool upload)
        {
            OCRGetDto res = new OCRGetDto();
            HttpContent fileStreamContent = new StreamContent(formFile.OpenReadStream());
            fileStreamContent.Headers.Add("Content-Type", formFile.ContentType);

            using var client = new HttpClient();
            using var formData = new MultipartFormDataContent();

            // Add the HttpContent objects to the form data.
            formData.Add(fileStreamContent, "file", formFile.FileName);

            // Invoke the request to the server.  
            var response = await client.PostAsync(Constants.OCR_SERVICE_URL, formData);

            // ensure the request was a success
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            Stream stream2 = await response.Content.ReadAsStreamAsync();
            StreamReader reader2 = new StreamReader(stream2);
            res.OCRResult = reader2.ReadToEnd();

            if (upload)
            {
                res.FileUploadResult = new DTOs.FileDto.UploadedFileMessage();
                res.FileUploadResult = await _IApplicationPartyRepositiory.UploadPartyAttachment(formFile);
            }

            return res;
        }
    }
}
