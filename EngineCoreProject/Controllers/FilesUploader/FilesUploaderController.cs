using EngineCoreProject.IRepository.IFilesUploader;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IO;
using System;

namespace EngineCoreProject.Controllers.FilesUploader
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class FilesUploaderController : Controller
    {
        private readonly IFilesUploaderRepositiory _IFilesUploaderRepositiory;
        public FilesUploaderController(IFilesUploaderRepositiory iFilesUploaderRepositiory)
        {
            _IFilesUploaderRepositiory = iFilesUploaderRepositiory;

        }
        //-------------------------------Upload-----------------------------------
        /*[HttpPost("{target}")]
               public async Task<ActionResult> Upload([FromForm] IFormFile file, [FromRoute] string target)
               {
                   string lang = Request.Headers["lang"].ToString().ToLower();
                   Response.Headers.Add("lang", lang);
       UploadedFileMessage m = await _IFilesUploaderRepositiory.UploadFile(file, target);
                   if (m.SuccessUpload == true)
                       return  Ok(m);
                   else 
                       return BadRequest(m);

                   }*/
        /* [HttpGet("{folder}")]
         public IActionResult DownloadFile([FromRoute] string folder, [FromQuery] string file)
         {
             string lang = Request.Headers["lang"].ToString().ToLower();
             Response.Headers.Add("lang", lang);

             var result = _IFilesUploaderRepositiory.ReadFile(folder, file);

             if (result != null)
                 return Ok(result);
             else
                 return BadRequest();
         }*/

        //-----------------------------------

        [HttpGet("File/{type}")]
        public async Task<IActionResult> downloadAsync(string type, [FromQuery] string fileName)
        {           
            if(!_IFilesUploaderRepositiory.FileExist( fileName))
            {
                return NotFound("Missing File!");
            }
            var path = _IFilesUploaderRepositiory.GetFilePath(fileName);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            if (type == "view")
                return File(memory, _IFilesUploaderRepositiory.GetMimeType(Path.GetExtension(fileName))/*, fileName*/);
            else
                return File(memory, _IFilesUploaderRepositiory.GetMimeType(Path.GetExtension(fileName)), fileName);
        }

        [HttpGet("GetFile/{file}")]
        public IActionResult Get(string file)
        {
            return PhysicalFile(_IFilesUploaderRepositiory.GetFilePath(file), _IFilesUploaderRepositiory.GetMimeType(file));
        }

        [HttpPost("createFolder")]
        public IActionResult createFolder(string folder)
        {
            var m= _IFilesUploaderRepositiory.CreateFolder(folder);
            return Ok(m);
        }

        [HttpPost("moveFile")]
        public IActionResult moveFile(string s, string t)
        {
            var m = _IFilesUploaderRepositiory.MoveFile(s,t);
            return Ok(m);
        }

        [HttpGet("GetFileNames/{folder}")]
        public IActionResult GetFileNames(string folder)
        {
            return Ok(_IFilesUploaderRepositiory.GetFolderFilesNames(folder));
        }

        [HttpGet("GetLogsFilesNames")]
        public IActionResult GetLogsFilesNames(string search)
        {
            return Ok(_IFilesUploaderRepositiory.GetLogsFilesNames(search));
        }



        [HttpPost("DeleteTemporaryFiles")]
        public IActionResult DeleteTemporaryFiles(DateTime dateTime)
        {
            return Ok(_IFilesUploaderRepositiory.DeleteTemporaryFiles(dateTime));
        }


        [HttpGet("DownloadDocument")]
        public IActionResult DownloadDocument(string fileName)
        {
            var path = Path.Combine("Logs", fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, "application/force-download", fileName);
        }

    }
}

