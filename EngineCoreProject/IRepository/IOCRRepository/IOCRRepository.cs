using EngineCoreProject.DTOs.OCRDto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IOCRRepository
{
    public interface IOCRRepository
    {
        Task<OCRGetDto> OCRCard(IFormFile formFile, bool upload);
    }
}
