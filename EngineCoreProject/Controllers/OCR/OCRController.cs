using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using EngineCoreProject.Services;
using EngineCoreProject.IRepository.IChannelRepository;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Configuration;
using EngineCoreProject.DTOs.ChannelDto;
using EngineCoreProject.IRepository.IOCRRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace EngineCoreProject.Controllers.OCR
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OCRController : ControllerBase
    {

        private readonly IOCRRepository _iOCRRepository;
        public OCRController(IOCRRepository iOCRRepository)
        {
            _iOCRRepository = iOCRRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("CallOCR")]
        public async Task<IActionResult> CallOCR(IFormFile file, bool upload)
        {
            try
            {
                var obj = await _iOCRRepository.OCRCard(file, upload);
                return this.StatusCode(StatusCodes.Status200OK, obj);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status400BadRequest , ex.Message);
            }
        }
    }
}
