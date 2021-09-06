using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.FileDto;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.ApplicationSet
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _ITransactionRepository;

        public TransactionsController(ITransactionRepository iTransactionRepository)
        {
            _ITransactionRepository = iTransactionRepository;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> All([FromQuery] int? applicationId)
        {
            try
            {
                var result = await _ITransactionRepository.GetAll(applicationId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,//e.Message.ToString()
                new { message = e.Message.ToString()/*Constants.getMessage(lang, "UnknownError") */});
            }
        }

        //----------------------------------------------------------------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task<ActionResult> One([FromRoute] int id, [FromHeader] string lang)
        {
            try
            {
                var result = await _ITransactionRepository.GetOne(id);
                if (result != null)
                    return Ok(result);
                else
                    return NotFound(new { message = Constants.getMessage(lang, "zeroResult") });
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                      new { message = Constants.getMessage(lang, "UnknownError") });
            }
        }



        //-------------------------------------------------------------------------
       /* [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TransactionDto transactionDto, [FromHeader] string lang)
        {
            int result = await _ITransactionRepository.Update(id, transactionDto);
            return result switch
            {
                Constants.OK => Ok(new { message = Constants.getMessage(lang, "sucsessUpdate") }),
                Constants.NOT_FOUND => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }*/

        //------------------------------Post Just Record-------------------------------------------
     /*   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TransactionDto transactionDto, [FromHeader] string lang)
        {
            int result = await _ITransactionRepository.Add(transactionDto);
            return result switch
            {
                Constants.ERROR => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
                _ => Ok(new
                {
                    message = Constants.getMessage(lang, "sucsessAdd"),
                    id = result
                })
            };
        }*/

        //------------------------------Post Record and upload file-------------------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("UploadTransaction")]
        public async Task<ActionResult> Upload([FromForm] TransactionFileDto transactionDto)
        {
            UploadedFileMessage message;
            message = await _ITransactionRepository.Upload(transactionDto);
            return message.SuccessUpload switch
            {
                false => BadRequest(message),
                _ => Ok(message)
            };
        }
        //-------------------------------------------------------------------------              
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromHeader] string lang)
        {
            int result = await _ITransactionRepository.DeleteOne(id);
            return result switch
            {
                Constants.OK => Ok(new { message = Constants.getMessage(lang, "sucsessDelete") }),
                Constants.NOT_FOUND => NotFound(new { message = Constants.getMessage(lang, "zeroResult") }),
                _ => BadRequest(new { error = Constants.getMessage(lang, "UnknownError") }),
            };
        }

        //-------------------------------------Upload-------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Upload")]
        public async Task<ActionResult> UploadAttachment(IFormFile File)
        {
            UploadedFileMessage m = await _ITransactionRepository.UploadTransactionDocument(File);
            return m.SuccessUpload switch
            {
                false => BadRequest(m),//new { error = Constants.getMessage(lang, "UnknownError") }
                _ => Ok(m)
            };
        }
        //-------------------------------------get parties---------------------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Parties/{transactionId}")]
        public async Task<ActionResult> GetAttachments([FromRoute] int transactionId, [FromQuery] string lang)
        {
            try
            {
                var result = await _ITransactionRepository.getRelatedParties(transactionId, lang);
                return Ok(result);

            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                       new { message = Constants.getMessage(lang, "UnknownError") });
            }
        }


        [HttpGet("TransactionInfo")]
        public async Task<ActionResult> tInfo([FromQuery] int transactionId, [FromQuery] string lang)
        {
            try
            {
                var result = await _ITransactionRepository.GetTransactionStatus(transactionId);
                if(result==null)
                {
                    return NotFound("رقم وثيقة غير موجود!");
                }
                return Ok(result);

            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                       new { message = Constants.getMessage(lang, "UnknownError") });
            }
        }
    }
}