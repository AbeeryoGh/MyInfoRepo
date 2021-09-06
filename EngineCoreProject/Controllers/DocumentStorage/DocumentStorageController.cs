using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using EngineCoreProject.Models;
using EngineCoreProject.DTOs;
using EngineCoreProject.IRepository;
using EngineCoreProject.Services;
using System.Security.Claims;

namespace EngineCoreProject.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentStorageController : ControllerBase
    {
/*
        private readonly IDocumentStorageRepository _IDocumentStorageRepository;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IConfiguration _configuration;

        public DocumentStorageController(IDocumentStorageRepository iDocumentStorageRepository, IConfiguration configuration, IWebHostEnvironment appEnvironment)
        {
            _IDocumentStorageRepository = iDocumentStorageRepository;
            _appEnvironment = appEnvironment;
            _configuration = configuration;
        }


        [HttpPost("[Action]")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);
            LoadFileMessage resultMessage = new LoadFileMessage();                          // NB! maybe resultMessage need add param

            //var userId = this.AspNetUsers.FindFirstValue(ClaimTypes.NameIdentifier);             //Get idUser!!!!!!
            var userId = 2020;
           resultMessage = await _IDocumentStorageRepository.UploadFile(file, userId);

            if (resultMessage.result)
                return Ok(resultMessage);
            else
                return BadRequest(resultMessage);
        }


        [HttpGet("[Action]/{idFile}")]
        public async Task<IActionResult> DownloadFile(int idFile)
        {
            string lang = Request.Headers["lang"].ToString().ToLower();
            Response.Headers.Add("lang", lang);

            var result = await _IDocumentStorageRepository.GetOneFile(idFile);

            if (result !=null)
                return Ok(result);
            else
                return BadRequest();
        }

        [HttpDelete("[Action]/{idFile}")]
        public async Task<ActionResult> Delete(int idFile)
        {
            string lang = Request.Headers["lang"].ToString().ToLower();
            int result = await _IDocumentStorageRepository.Delete(idFile);

            if (result == Constants.OK)
                return Ok();
            else
                return BadRequest();             
        }*/

    }
}
