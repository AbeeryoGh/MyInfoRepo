using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.AccountDto;
using EngineCoreProject.DTOs.UnifiedGateDto;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.Models;
using EngineCoreProject.Services;
using EngineCoreProject.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EngineCoreProject.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
     
    public class UserController : ControllerBase
    {

        private readonly IGeneralRepository _iGeneralRepository;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _IUserRepository;
        private readonly SignInWithUGateSettings _SignInWithUGateSettings;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        public UserController(IWebHostEnvironment iWebHostInviroment, UserManager<User> userManager,
                              IUserRepository iUsersRepository, IGeneralRepository iGeneralRepository,
                              IUserRepository iUserRepository, IOptions<SignInWithUGateSettings> signInWithUGateSettings
                              ,  EngineCoreDBContext EngineCoreDBContext)
        {
            _IUserRepository = iUserRepository;
            _iGeneralRepository = iGeneralRepository;
            _userManager = userManager;
            _iGeneralRepository = iGeneralRepository;
            _userManager = userManager;
            _SignInWithUGateSettings = signInWithUGateSettings.Value;
            _IWebHostEnvironment = iWebHostInviroment;
            _EngineCoreDBContext= EngineCoreDBContext;
    }
          [DisableCors]
       [EnableCors("ACTIVE_DIRECTORY")]
       [HttpGet("ADLogin")]
       [Authorize(AuthenticationSchemes = NegotiateDefaults.AuthenticationScheme, Roles = Constants.EmployeePolicy)]
        public async Task<ActionResult> ADLoginAsync([FromHeader] string lang)
        {
            string DomainName = Environment.UserDomainName;// JUSTICE

           // if(DomainName !=Constants.DomainName)
            //    return NotFound("INCORRECT DOAMIN!!!");

            //string AccountName = Environment.UserName;     
            string AccountName = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;
            var obj = await _IUserRepository.WindowsSignIn(AccountName, lang);


            if (obj.StatusCode == "404")
            {
                return NotFound("EMAIL NOT FOUND !!!!");
            }

            if (obj.StatusCode == "406")
            {
                return NotFound("DISABLED ACCOUNT !!!!");
            }


            return Ok(obj);

        }





        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn(LogInDtoLocal logInDto, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.LocalSignIn(logInDto, lang);
            return Ok(obj);
        }

        [HttpPost("SignOut")]
        public IActionResult SignOut(bool WithUnifiedgate)
        {
            _IUserRepository.SignOut();
            if (WithUnifiedgate)
                return Redirect(_SignInWithUGateSettings.signOut);

            else return Ok("User SignOut !!!");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles([FromHeader] string lang)
        {
            var obj = await _IUserRepository.GetUserRoles(_IUserRepository.GetUserID(), lang);
            return Ok(obj);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetUsersRoles")]
        public async Task<IActionResult> GetUsersRoles(string blindSearch, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.GetUsersRoles(blindSearch, lang);
            return Ok(obj);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetUserActionsPermissions")]
        public async Task<IActionResult> GetUserActionsPermissions([FromHeader] string lang)
        {
            var obj = await _IUserRepository.GetUserActionsPermissions(6065);
            return Ok(obj);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("EditUserRolesAsync")]
        public async Task<ActionResult> EditUserRolesAsync(int userId, List<int> userRoles, [FromHeader] string lang)
        {
            var result = await _IUserRepository.EditUserRolesAsync(userId, userRoles);
            if (result.Succeeded)
            {
                return StatusCode(StatusCodes.Status200OK, result);
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound, Constants.getMessage(lang, "zeroResult"));
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("GetUserId")]
        public ActionResult GetUserId()
        {
            return Ok(_IUserRepository.GetUserID().ToString());
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("IsAdmin")]
        public ActionResult IsAdmin()
        {
            return Ok(_IUserRepository.IsAdmin());
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("IsEmployee")]
        public ActionResult IsEmployee()
        {
            return Ok(_IUserRepository.IsEmployee());
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserPostDto UserPostDto, [FromHeader] string lang)
        {
            string ImageUrl = "wwwroot/transactions/UsersPhoto/" + SaveImage(UserPostDto.ImageFile);

            UserResultDto obj = await _IUserRepository.CreateUser(UserPostDto, ImageUrl, true, lang, false);
            if (obj.user == null)
            {
                return NotFound(obj);
            }
            return Ok(obj);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("EditPassword")]
        public async Task<IActionResult> EditPassword(EditUserPasswordDTO editUserPasswordDTO, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.EditPassword(editUserPasswordDTO, lang);
            return Ok(obj);
        }

        /*
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("GetOneUser")]
        public async Task<IActionResult> GetOneUser(int userID)
        {
            var obj = await _IUserRepository.GetOne(userID);
            return Ok(obj);
        }
        */


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("CreateUserTest")]
        public IActionResult CreateUserTest(IFormFile ImageName)
        {

            var resul = SaveImage(ImageName);
            string Image = "wwwroot/transactions/UsersPhoto/" + resul;
            //  UserResultDto obj = await _IUserRepository.CreateUser(UserPostDto);
            if (resul == null)
            {
                return NotFound(resul);
            }
            return Ok(resul);
        }

        [NonAction]
        public string SaveImage(IFormFile imageFile)
        {
            string imageName;
            if (imageFile == null) return null;
            imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(" ", "_");
            imageName = imageName + DateTime.Now.ToString("yyyymmdd") + Path.GetExtension(imageFile.FileName);

            var imagePath = Path.Combine(_IWebHostEnvironment.ContentRootPath, "wwwroot/transactions/UsersPhoto", imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                imageFile.CopyToAsync(fileStream);
            }
            return imageName;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.EmployeePolicy)]
        [HttpPost("EditUser")]
        public async Task<IActionResult> EditUser(int id, [FromForm] UserPostDto userPostDto, [FromHeader] string lang)
        {
            UserResultDto obj = await _IUserRepository.EditUser(id, userPostDto, false, false, lang);
            if (obj.user == null)
            {
                return NotFound(obj);
            }
            return Ok(obj);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpPost("EditUserByAdmin")]
        public async Task<IActionResult> EditUserByAdmin(int id, [FromBody] UserPostDto userPostDto, [FromHeader] string lang)
        {
            UserResultDto obj = await _IUserRepository.EditUser(id, userPostDto, false, true, lang);
            if (obj.user == null)
            {
                return NotFound(obj);
            }
            return Ok(obj);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AddEditSignature")]
        public async Task<IActionResult> AddEditSignature([FromForm] SignaturePostDto signaturePostDto, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.AddEditSignature(signaturePostDto, lang);
            return Ok(obj);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("signatureBase64")]
        public async Task<IActionResult> AddEditSignature64([FromBody] SignatureBase64PostDto signatureBase64, [FromHeader] string lang)
        {
            var obj = await _IUserRepository.AddEditSignature64(signatureBase64.SignatureBase64, lang);
            return Ok(obj);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            UserResultDto obj = await _IUserRepository.DeleteUser(id);
            if (obj.user == null)
            {
                return NotFound(obj);
            }
            return Ok(obj);
        }

        [HttpGet("FindUserById/{id}")]
        public async Task<IActionResult> FindUserById(int id, [FromHeader] string lang)
        {
            UserDto obj = await _IUserRepository.FindUserById(id, lang);
            if (obj == null)
            {
                return NotFound(obj);
            }
            return Ok(obj);
        }

        [HttpGet("VisitorsCount")]
        public async Task<IActionResult> VisitorsCount()
        {
            var obj = await _IUserRepository.VisitorsCount();
            return Ok(obj);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            List<UserDto> obj = await _IUserRepository.GetUsers();
            return Ok(obj);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetMyTabsPermissions")]
        public async Task<IActionResult> GetMyTabsPermissions([FromHeader] string lang)
        {
            var obj = await _IUserRepository.GetUserTabsPermissions(lang);
            return Ok(obj);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminPolicy)]
        [HttpGet("GetEmployees")]
        public async Task<IActionResult> GetEmployees()
        {
            var obj = await _IUserRepository.GetEmployees();
            return Ok(obj);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetOnlineEmployees")]
        public async Task<IActionResult> GetOnlineEmployees()
        {
            var obj = await _IUserRepository.GetOnlineEmployees();
            return Ok(obj);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("StartStopWork")]
        public async Task<IActionResult> StartStopWork(bool? start)
        {
            var obj = await _IUserRepository.StartStopWork(start);
            return Ok(obj);
        }

        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("CreateUserForOldAppParties")]
        public async Task<IActionResult> CreateUserForOldAppParties(OldUserPostDto UserPostDto)
        {
            var obj = await _IUserRepository.CreateUserForOldAppParties(UserPostDto);
            return Ok(obj);
        }


        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("AddUO")]
        public async Task<IActionResult> AddUO()
        {
            var obj = await _IUserRepository.Addoldusers();
            return Ok(obj);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var obj = await _IUserRepository.RefreshToken();
            return Ok(obj);
        }
    }

}
