using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using EngineCoreProject.DTOs.AccountDto;
using EngineCoreProject.DTOs.RoleDto;
using Microsoft.Extensions.Options;
using EngineCoreProject.DTOs.JWTDto;
using EngineCoreProject.IRepository.IRoleRepository;
using EngineCoreProject.IRepository.IFilesUploader;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace EngineCoreProject.Services.UserService
{
    public class UserRepository : IUserRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IRoleRepository _IRoleRepository;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _IConfiguration;
        private readonly IFilesUploaderRepositiory _IFilesUploaderRepository;
        private readonly IWebHostEnvironment _IWebHostEnvironment;
        private readonly ILogger<UserRepository> _logger;

        ValidatorException _exception;
        private readonly jwt _jwt;
        public UserRepository(IRoleRepository iRoleRepository, IOptions<jwt> jwt, SignInManager<User> signInManager,
                             EngineCoreDBContext EngineCoreDBContext, IHttpContextAccessor httpContext,
                             IGeneralRepository iGeneralRepository, UserManager<User> userManager,
                             RoleManager<Role> roleManager, IConfiguration iConfiguration,
                             IFilesUploaderRepositiory iFilesUploaderRepository, IWebHostEnvironment iWebHostEnvironment,
                             ILogger<UserRepository> logger)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _httpContext = httpContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _iGeneralRepository = iGeneralRepository;
            _signInManager = signInManager;
            _IRoleRepository = iRoleRepository;
            _IConfiguration = iConfiguration;
            _IFilesUploaderRepository = iFilesUploaderRepository;
            _IWebHostEnvironment = iWebHostEnvironment;
            _logger = logger;
            _jwt = jwt.Value;
            _exception = new ValidatorException();
        }

        public int GetUserID()
        {
            int userID = 0;
            if (_httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                userID = int.Parse(_httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }
            return userID;
        }
        public string GetUserEid()
        {
            string EmirateId = _httpContext.HttpContext.User.Claims.Where(x => x.Type == "EmirateId").Select(x => x.Value).FirstOrDefault();// FindFirst("EmirateId").Value ;
            return EmirateId;
        }
        public string GetUserEmail()
        {
            string Email = _httpContext.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            return Email;
        }
        public string GetUserName()
        {
            string userName = _httpContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            return userName;
        }
        public bool IsAdmin()
        {
            return _httpContext.HttpContext.User.IsInRole(Constants.AdminPolicy);
        }
        public bool IsInspector()
        {
            return _httpContext.HttpContext.User.IsInRole(Constants.InspectorPolicy);
        }
        public bool IsEmployee()
        {
            return _httpContext.HttpContext.User.IsInRole(Constants.EmployeePolicy);
        }

        public async Task<bool> IsEmployee(int userId)
        {
            bool isEmployee = false;
            var user = await _userManager.Users.Include(x => x.UserRole).ThenInclude(x => x.Role).FirstOrDefaultAsync(u => u.Id == userId);
            var roleEmployee = await _roleManager.FindByNameAsync(Constants.EmployeePolicy);
            if (user != null && roleEmployee != null)
            {
                isEmployee = user.UserRole.Any(x => x.RoleId == roleEmployee.Id);
            }
            return isEmployee;
        }

        public async Task<List<RoleGetDto>> GetUserRoles(int userId, string lang)
        {
            var user = await _userManager.Users.Include(x => x.UserRole).ThenInclude(x => x.Role).FirstOrDefaultAsync(u => u.Id == userId);
            List<RoleGetDto> res = new List<RoleGetDto>();
            foreach (var role in user.UserRole)
            {
                RoleGetDto roleGetDto = new RoleGetDto()
                {
                    Id = role.RoleId,
                    RoleName = await _iGeneralRepository.GetTranslateByShortCut(lang, role.Role.Name)
                };
                res.Add(roleGetDto);
            }
            return res;
        }

        public async Task<List<UserDto>> GetUsersRoles(string blindSearch, string lang)
        {
            List<UserDto> res = new List<UserDto>();

            if (blindSearch == null || blindSearch.Length < 6)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "blindSearchText"));
                throw _exception;
            }
            var value = blindSearch.Trim().ToLower();
            var users = await _userManager.Users.Include(x => x.UserRole).ThenInclude(x => x.Role).Where(x =>
                                                         x.Email.Trim().ToLower().Contains(value) ||
                                                         x.UserName.Trim().ToLower().Contains(value) ||
                                                         x.PhoneNumber.Trim().ToLower().Contains(value) ||
                                                         x.FullName.Trim().ToLower().Contains(value) ||
                                                         x.Address.Trim().ToLower().Contains(value) ||
                                                         x.EmiratesId.Trim().ToLower().Contains(value)
                                                         ).ToListAsync();
            foreach (var user in users)
            {
                List<RoleGetDto> userRoles = new List<RoleGetDto>();
                foreach (var userRole in user.UserRole)
                {
                    RoleGetDto roleDto = new RoleGetDto()
                    {
                        Id = userRole.RoleId,
                        RoleName = await _iGeneralRepository.GetTranslateByShortCut(lang, userRole.Role.Name)
                    };
                    userRoles.Add(roleDto);
                }

                UserDto userDto = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    EmiratesId = user.EmiratesId,
                    Address = user.Address,
                    AreaId = user.AreaId,
                    BirthdayDate = user.BirthdayDate,
                    EmailLang = user.EmailLang,
                    Emarit = user.EmiratesId,
                    Gender = user.Gender,
                    NatId = user.NatId,
                    PhoneNumber = user.PhoneNumber,
                    TelNo = user.TelNo,
                    SmsLang = user.SmsLang,
                    RolesName = userRoles
                };
                res.Add(userDto);
            }

            return res;
        }
        public async Task<UserPermissionsDTO> GetUserPermissions(int userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            UserPermissionsDTO permissions = new UserPermissionsDTO();
            if (user == null)
            {
                return permissions;
            }

            permissions.UserID = user.Id;
            var userRoles = await _roleManager.Roles.Include(e => e.RoleClaim).ToListAsync();

            List<RoleClaim> userclaims = new List<RoleClaim>();
            foreach (var role in userRoles)
            {
                if (_userManager.IsInRoleAsync(user, role.Name).Result)
                {
                    userclaims.AddRange(role.RoleClaim.ToList());
                }
            }

            permissions.Permissions = userclaims.Distinct().ToList();
            return permissions;
        }
        public async Task<List<int>> GetUserActionsPermissions(int userId)
        {
            List<int> res = new List<int>();
            var userPermissionsDTO = await GetUserPermissions(userId);

            if (userPermissionsDTO == null)
            {
                return res;
            }
            res = userPermissionsDTO.Permissions.Where(x => x.ClaimType == CustomClaimTypes.Action).Select(x => Int32.Parse(x.ClaimValue)).ToList();
            return res;
        }
        public async Task<List<int>> GetUserTabsPermissions(string lang)
        {
            List<int> res = new List<int>();
            var userPermissionsDTO = await GetUserPermissions(GetUserID());
            if (userPermissionsDTO == null)
            {
                return res;
            }
            return userPermissionsDTO.Permissions.Where(x => x.ClaimType == CustomClaimTypes.Tab).Select(x => Int32.Parse(x.ClaimValue)).ToList();
        }
        public async Task<IdentityResult> EditUserRolesAsync(int userId, List<int> userRoles)
        {
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new InvalidOperationException("user not exist ...");
            }

            var oldRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, oldRoles);

            List<string> roles = new List<string>();
            roles = await _roleManager.Roles.Where(x => userRoles.Contains(x.Id)).Select(x => x.Name).ToListAsync();

            IdentityResult res = IdentityResult.Success;
            if (roles != null)
            {
                res = await _userManager.AddToRolesAsync(user, roles);
            }

            scope.Complete();
            return res;
        }
        public async Task<UserDto> FindUserById(int id, string lang)
        {
            List<RoleGetDto> roles = await GetUserRoles(id, lang);
            var userInfo = await _EngineCoreDBContext.User.Where(x => x.Id == id).FirstOrDefaultAsync();

            UserDto userDto = new UserDto
            {
                Address = userInfo.Address,
                BirthdayDate = userInfo.BirthdayDate,
                Email = userInfo.Email,
                EmailLang = userInfo.EmailLang,
                EmiratesId = userInfo.EmiratesId,
                FullName = userInfo.FullName,
                Gender = userInfo.Gender,
                Id = userInfo.Id,
                Image = userInfo.Image,
                PhoneNumber = userInfo.PhoneNumber,
                SmsLang = userInfo.SmsLang,
                TelNo = userInfo.TelNo,
                ProfileStatus = (int)userInfo.ProfileStatus,
                UserName = userInfo.UserName,
                AreaId =(int) userInfo.AreaId,
                NatId = (int)userInfo.NatId,
                RolesName = roles,
                //    Location = userInfo.LocationId,

            };

            if (userInfo.NatId != null)
            {
                var nat = await _EngineCoreDBContext.Country.Where(x => x.UgId == userInfo.NatId).FirstOrDefaultAsync();
                if (nat != null)
                {
                    userDto.NationalityName = (lang == "en") ? nat.CntCountryEn : nat.CntCountryAr;
                }
            }


            // get user sign.
            if (userInfo.Sign != null && File.Exists(Path.Combine(_IWebHostEnvironment.WebRootPath, userInfo.Sign)))
            {
                userDto.Sign = userInfo.Sign;
            }
            return userDto;
        }

        private string GetPhoneNumberWithCode(string phoneNumber)
        {
            string phoneNumberWithCode = phoneNumber;
            if (phoneNumberWithCode != null && phoneNumber != "" && phoneNumber.IndexOf("00971", 0, 5) == -1)
            {
                if (phoneNumber.IndexOf("0", 0, 1) == 0)
                {
                    phoneNumberWithCode = phoneNumber[1..];
                }
                phoneNumberWithCode = "00971" + phoneNumberWithCode;
            }

            return phoneNumberWithCode;
        }
        public async Task<UserResultDto> CreateUser(UserPostDto UserPostDto, string ImageUrl, bool updateRoles, string lang, bool FromUg)
        {
            if (await _EngineCoreDBContext.User.AnyAsync(x => x.EmiratesId.Trim() == UserPostDto.EmiratesId.Trim()))
            {
                throw new System.InvalidOperationException(Constants.getMessage(lang, "EmiratesIDExistedBefore"));
            }

            if (UserPostDto.PhoneNumber != null && UserPostDto.PhoneNumber.Length>0)
            {
                UserPostDto.PhoneNumber = GetPhoneNumberWithCode(UserPostDto.PhoneNumber);
                if (UserPostDto.PhoneNumber.Length > 25)
                {
                    throw new System.InvalidOperationException(Constants.getMessage(lang, "InvalidPhoneNumber"));
                }
            }


            if (await _EngineCoreDBContext.User.AnyAsync(x => x.Email.Trim() == UserPostDto.Email.Trim()))
            {
                if (FromUg)
                {
                    User user = await _EngineCoreDBContext.User.Where(x => x.Email.Trim() == UserPostDto.Email.Trim()).FirstOrDefaultAsync();
                    await GenerateInvalidEmailAsync(user);
                }
                else
                {
                    throw new System.InvalidOperationException(Constants.getMessage(lang, "EmailExistedBefore"));
                }
            }

            if (await _EngineCoreDBContext.User.AnyAsync(x => x.UserName.Trim() == UserPostDto.UserName.Trim()))
            {
                if (FromUg)
                {
                    User user = await _EngineCoreDBContext.User.Where(x => x.Email.Trim() == UserPostDto.Email.Trim()).FirstOrDefaultAsync();
                    await GenerateInvalidEmailAsync(user);
                }
                else
                {
                    throw new System.InvalidOperationException(Constants.getMessage(lang, "UserNameExistedBefore"));
                }
            }


            try
            {
                var addr = new System.Net.Mail.MailAddress(UserPostDto.Email);
            }
            catch
            {
                throw new System.InvalidOperationException(Constants.getMessage(lang, "InvalidEmailFormat"));
            }

            User newUser = new User()
            {
                TwoFactorEnabled = false,
                PhoneNumberConfirmed = false,
                PhoneNumber = UserPostDto.PhoneNumber,
                PasswordHash = UserPostDto.PasswordHash,
                EmailConfirmed = false,
                NormalizedEmail = UserPostDto.Email.ToUpper(),
                Email = UserPostDto.Email,
                NormalizedUserName = UserPostDto.UserName.ToLower(),
                UserName = UserPostDto.UserName,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                FullName = UserPostDto.FullName,
                BirthdayDate = UserPostDto.BirthdayDate,
                Gender = UserPostDto.Gender,
                TelNo = UserPostDto.TelNo,
                Address = UserPostDto.Address,
                EmiratesId = UserPostDto.EmiratesId,
                Image = ImageUrl,//UserPostDto.Image,
                CreatedDate = DateTime.Now,
                ProfileStatus = Convert.ToInt32(Constants.PROFILE_STATUS.ENABLED)
            };


            try
            {
                var result = await _userManager.CreateAsync(newUser, UserPostDto.PasswordHash);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(x => x.Description).ToList();
                    string errorResult = "";
                    foreach (var x in errors)
                    {
                        errorResult = errorResult + " , " + x;
                    }
                    throw new System.InvalidOperationException(errorResult);
                }

                if (newUser.Id == 0)
                {
                    throw new System.InvalidOperationException("failed in creating new user, auto identity is removed.");
                }
            }
            catch
            {
                throw new System.InvalidOperationException("failed in creating new user.");
            }

            if (updateRoles)
            {
                var res = await EditUserRolesAsync(newUser.Id, UserPostDto.UserRoles);
                if (!res.Succeeded)
                {
                    throw new System.InvalidOperationException("Failed assigned roles");
                }
            }

            UserResultDto UserResultDto = new UserResultDto()
            {
                Message = "User is Created.",
                user = newUser
            };

            //  scope.Complete();

            return UserResultDto;
        }
        private async Task GenerateInvalidEmailAsync(User user)
        {
            string Email = Constants.INVALID_EMAIL_PREFIX + _iGeneralRepository.GetNewValueBySec() + Constants.INVALID_EMAIL_SUFFIX;

            user.Email = Email;
            user.NormalizedEmail = Email.ToUpper();
            int index = Email.IndexOf("@");
            user.UserName = Email.Substring(0, index);
            await _userManager.UpdateAsync(user);
        }

        public async Task<UserResultDto> EditUser(int id, UserPostDto userPostDto, bool fromUg, bool updateRoles, string lang)
        {
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new UserResultDto { Message = "USER NOT FOUND", user = null };
            }

            if (await _EngineCoreDBContext.User.AnyAsync(x => x.Email.Trim() == userPostDto.Email.Trim() && x.Id != id))
            {
                if (fromUg)
                {
                    User oldUser = await _EngineCoreDBContext.User.Where(x => x.Email.Trim() == userPostDto.Email.Trim()).FirstOrDefaultAsync();
                    await GenerateInvalidEmailAsync(oldUser);
                }
                else
                {
                    throw new System.InvalidOperationException("EmailExistedBefore");
                }
            }

            if (await _EngineCoreDBContext.User.AnyAsync(x => x.UserName.Trim() == userPostDto.UserName.Trim() && x.Id != id))
            {
                if (fromUg)
                {
                    User oldUser = await _EngineCoreDBContext.User.Where(x => x.Email.Trim() == userPostDto.Email.Trim()).FirstOrDefaultAsync();
                    await GenerateInvalidEmailAsync(oldUser);
                }
                else
                {
                    throw new System.InvalidOperationException("UserNameExistedBefore");
                }
            }

            user.PhoneNumber = GetPhoneNumberWithCode(userPostDto.PhoneNumber);
            user.NormalizedEmail = userPostDto.Email.ToUpper();
            user.Email = userPostDto.Email;
            user.NormalizedUserName = userPostDto.UserName.ToLower();
            user.UserName = userPostDto.UserName;
            user.FullName = userPostDto.FullName;
            user.BirthdayDate = userPostDto.BirthdayDate;
            user.Gender = userPostDto.Gender;
            user.TelNo = userPostDto.TelNo;
            user.Address = userPostDto.Address;
            user.EmiratesId = userPostDto.EmiratesId;


            user.SecurityQuestionId = userPostDto.SecurityQuestionId;
            user.NatId = userPostDto.NatId;
            user.SecurityQuestionAnswer = userPostDto.SecurityQuestionAnswer;
            user.Status = userPostDto.Status;
            user.EmailLang = userPostDto.EmailLang;
            user.SmsLang = userPostDto.SmsLang;
            user.AreaId = userPostDto.AreaId;

            user.NotificationType = userPostDto.NotificationType;
            user.ProfileStatus = userPostDto.ProfileStatus;

            if (userPostDto.ImageFile != null)
            {
                var fileUploaded = await _IFilesUploaderRepository.UploadFile(userPostDto.ImageFile, "UserImageFolder");

                if (!fileUploaded.SuccessUpload)
                {
                    return new UserResultDto { Message = fileUploaded.Message, user = null };
                }

                user.Image = Path.Combine(_IConfiguration["UserImageFolder"], fileUploaded.FileName);
            }

            var result = await _userManager.UpdateAsync(user);
            var resUpdateRole = IdentityResult.Success;
            if (updateRoles)
            {
                resUpdateRole = await EditUserRolesAsync(user.Id, userPostDto.UserRoles);
            }

            if (userPostDto.PasswordHash != null)
            {
                var validators = _userManager.PasswordValidators;

                foreach (var validator in validators)
                {
                    var res = await validator.ValidateAsync(_userManager, null, userPostDto.PasswordHash);

                    if (!res.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            _exception.AttributeMessages.Add(error.Description);
                        }
                        throw _exception;
                    }
                }

                await _userManager.RemovePasswordAsync(user);
                var updatePassword = await _userManager.AddPasswordAsync(user, userPostDto.PasswordHash);

                if (!updatePassword.Succeeded)
                {
                    foreach (var x in updatePassword.Errors)
                    {
                        _exception.AttributeMessages.Add(x.Description);
                    }
                    throw _exception;
                }
            }

            scope.Complete();

            if (result.Succeeded && resUpdateRole.Succeeded)
            {
                return new UserResultDto { Message = Constants.getMessage(lang, "sucsessUpdate"), user = user };
            }
            else
            {
                return new UserResultDto { Message = result.Errors.FirstOrDefault().Description + resUpdateRole.Errors.FirstOrDefault().Description, user = null };
            }
        }

        public async Task<string> AddEditSignature64(string signatureBase64, string lang)
        {
            string pathSign = "";
            User user = _userManager.Users.FirstOrDefault(u => u.Id == GetUserID());
            if (user == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            string target = "Signature";
            pathSign = _IFilesUploaderRepository.FromBase64ToImage(signatureBase64, target);
            try
            {

                if (pathSign != "")
                {
                    user.Sign = pathSign;
                    await _userManager.UpdateAsync(user);
                    return pathSign;
                }
            }
            catch
            {
                return pathSign;
            }
            return pathSign;
        }

        public async Task<bool> AddEditSignature(SignaturePostDto signaturePostDto, string lang)
        {
            User user = _userManager.Users.FirstOrDefault(u => u.Id == GetUserID());
            if (user == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            string imageName;
            if (signaturePostDto.SignatureFile != null)
            {
                imageName = new String(Path.GetFileNameWithoutExtension(signaturePostDto.SignatureFile.FileName).Take(10).ToArray()).Replace(" ", "_");
                imageName = imageName + DateTime.Now.ToString("yyyymmdd") + Path.GetExtension(signaturePostDto.SignatureFile.FileName);

                var imagePath = Path.Combine(_IWebHostEnvironment.ContentRootPath, "wwwroot/Signature", imageName);
                using var fileStream = new FileStream(imagePath, FileMode.Create);

                await signaturePostDto.SignatureFile.CopyToAsync(fileStream);
                user.Sign = Path.Combine("Signature", imageName);

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

            return true;
        }


        public async Task<bool> EditPassword(EditUserPasswordDTO editUserPasswordDTO, string lang)
        {
            User user = _userManager.Users.FirstOrDefault(u => u.Id == GetUserID());
            if (user == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            if (editUserPasswordDTO.NewPassword != editUserPasswordDTO.ConfirmPassword)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "NotIdenticalPassword"));
                throw _exception;
            }

            var updatePassword = await _userManager.ChangePasswordAsync(user, editUserPasswordDTO.CurrentPassword, editUserPasswordDTO.NewPassword);

            if (!updatePassword.Succeeded)
            {
                foreach (var x in updatePassword.Errors)
                {
                    _exception.AttributeMessages.Add(x.Description);
                }
                throw _exception;
            }
            return true;
        }


        public async Task<UserResultDto> DeleteUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new UserResultDto { Message = "USER NOT FOUND", user = null };
            }
            if (await DisabledAccount(id))
            {
                return new UserResultDto { Message = "USER DISABLED BEFORE THIS TIME", user = null };
            }
            user.ProfileStatus = Convert.ToInt32(Constants.PROFILE_STATUS.DISABLED);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new UserResultDto { Message = "user was deleted ", user = user };
            }
            else
            {
                return new UserResultDto { Message = result.Errors.FirstOrDefault().Description, user = null };
            }
        }
        public async Task<UserResultDto> EnableUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return new UserResultDto { Message = "USER NOT FOUND", user = null };
            user.ProfileStatus = Convert.ToInt32(Constants.PROFILE_STATUS.ENABLED);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded) return new UserResultDto { Message = "USER WAS ENABLED ", user = user };
            else return new UserResultDto { Message = result.Errors.FirstOrDefault().Description, user = null };
        }
        public async Task<LogInResultDto> VisitorSignIn(LogInDtoLocal logInDto, string lang)
        {
            return await SignIn(new LogInDto { Email = logInDto.Email, PassWord = logInDto.PassWord, loginType = 2, ServiceId = 0 }, lang);
        }
        public async Task<LogInResultDto> LocalSignIn(LogInDtoLocal logInDto, string lang)
        {
            return await SignIn(new LogInDto { Email = logInDto.Email, PassWord = logInDto.PassWord, loginType = 0, ServiceId = 0 }, lang);
        }
        public async Task<LogInResultDto> WindowsSignIn(string Account, string lang)
        {
            User currentUser = null;
            int? UserId = null;

            try
            {
                if (Account != null)
                {
                    string AccountName = Account.Substring(Account.LastIndexOf("\\") + 1);


                    UserId = await _EngineCoreDBContext.EmployeeSetting.Where(x => x.ActiveDirectoryAccount == AccountName).Select(y => y.EnotaryId).FirstOrDefaultAsync();

                    if (UserId == 0)
                        return new LogInResultDto()
                        {
                            StatusCode = "401"

                        };

                }
                else return new LogInResultDto()
                {
                    StatusCode = "401"

                };
            }
            catch (Exception e)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            try
            {
                currentUser = await _EngineCoreDBContext.User.Where(x => x.Id == UserId).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserNotExistedBefore"));
                throw _exception;
            }

            LogInDtoLocal logInDto = new LogInDtoLocal { Email = currentUser.Email };

            return await SignIn(new LogInDto { Email = logInDto.Email, loginType = 4 }, lang);
        }


        public async Task<LogInResultDto> UGSignIn(LogInDtoUg LogInDtoUg, string lang)
        {
            return await SignIn(new LogInDto { Email = LogInDtoUg.Email, PassWord = LogInDtoUg.PassWord, loginType = 1, ServiceId = LogInDtoUg.ServiceId, AppId = LogInDtoUg.AppId }, lang);
        }

        public async Task<string> RefreshToken()
        {
            string refreshToken = "";
            var user = await _EngineCoreDBContext.User.Where(x => x.Id == GetUserID()).FirstOrDefaultAsync();
            if (user != null)
            {
                Claim[] claims = GenerateClames(user);
                refreshToken = GenerateToken(claims, _jwt.Key);
            }
            return refreshToken;
        }

        public async Task<int> VisitorsCount()
        {
            var users = await _EngineCoreDBContext.UserLogin.Select(x => x.UserId).Distinct().ToListAsync();
            return users.Count;
        }

        private async Task<LogInResultDto> SignIn(LogInDto logInDto, string lang)
        {
            using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var user = await _userManager.FindByEmailAsync(logInDto.Email);

            if (user == null)
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserEmailError"));
                throw _exception;
            }

            if (!await EnabledAccount(user.Id))
            {
                _exception.AttributeMessages.Add(Constants.getMessage(lang, "UserAccountLocked"));
                throw _exception;
            }

            var providor = "Local";
            //   var result = await _signInManager.CheckPasswordSignInAsync(user, logInDto.PassWord, false);
            if (logInDto.loginType == 1 || logInDto.loginType == 2)   // with UG OR Visitor by OTP.
            {
                await _signInManager.SignInAsync(user, false);

                providor = (logInDto.loginType == 1) ? "UG" : "OTP";
            }

            if (logInDto.loginType == 0)   // Local user.
            {
                var result = await _signInManager.PasswordSignInAsync(user, logInDto.PassWord, false, false);
                if (!result.Succeeded)
                {
                    _exception.AttributeMessages.Add(Constants.getMessage(lang, "wrongPassword"));
                    throw _exception;
                }                
            }

            if (logInDto.loginType == 4)   // windows auth.
            {
                await _signInManager.SignInAsync(user, false);
                providor = "Local-LDAP";

            }

            DateTime llog = DateTime.Now;

            try
            {

                var info = await _EngineCoreDBContext.UserLogin.Where(x => x.UserId == user.Id).ToListAsync();
                if (info.Count > 0)
                {
                    llog = (DateTime)info.Last().LoginDate;
                }


                UserLogin userLogin = new UserLogin
                {
                   Id = GetNewValueBySec(),
                    LoginProvider = providor,
                    UserId = user.Id,
                    ProviderDisplayName = user.UserName,
                    ProviderKey = Guid.NewGuid().ToString(), //user.Id.ToString(),    // TODO get a key form the provider.
                    LoginDate = DateTime.Now
                };

                await _EngineCoreDBContext.UserLogin.AddAsync(userLogin);
                await _EngineCoreDBContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += " inner exception is " + ex.InnerException.Message;
                }
                _logger.LogInformation("error in adding user logging" + message);
            }


            Claim[] claims = GenerateClames(user);
            string jwt = GenerateToken(claims, _jwt.Key);

            LogInResultDto logInResultDto = new LogInResultDto()
            {
                StatusCode = "200",
                token = jwt,
                UserId = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = logInDto.Email,
                PhoneNumber = user.PhoneNumber,
                EmirateId = user.EmiratesId,
                BirthdayDate = user.BirthdayDate,
                RolesName = await _userManager.GetRolesAsync(user),
                RolesId = await _IRoleRepository.GetRolesIdByUserId(user.Id),
                ServiceId = (int?)logInDto.ServiceId,
                GenderId = await getGenderIdAsync(user.Gender),
                ApplicationId = logInDto.AppId,
                Address = user.Address,
                CountryId = user.NatId /*await getUserCountryId(user.NatId)*/,
                AreaId = user.AreaId,
                LastLogin = llog
            };


            try
            {
                if (logInResultDto.Email.Trim().ToLower().StartsWith(Constants.INVALID_EMAIL_PREFIX.ToLower()))
                {
                    logInResultDto.Email = null;
                }

                // get user image.
                string getUserImageURL = _IConfiguration["GetUserImage"];
                if (user.Image != null && _IFilesUploaderRepository.FileExist("User_images", user.Image))
                {
                    logInResultDto.Image = Path.Combine("User_images", user.Image);
                }
                else
                {
                    if (_IFilesUploaderRepository.FileExist("User_images", "default.jpg"))
                    {
                        logInResultDto.Image = Path.Combine("User_images", "default.jpg");
                    }
                }
            }
            catch
            {

            }

            scope.Complete();
            return logInResultDto;
        }


        private async Task<int?> getGenderIdAsync(string gender)
        {

            if (gender == null) return null;

            if (gender.Contains('F') || gender.Contains('f') || gender.Contains('ث'))
            {
                SysLookupValue lookupValue = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == "FEMALE").FirstOrDefaultAsync();
                if (lookupValue != null)
                    return lookupValue.Id;
            }
            else
            {
                SysLookupValue lookupValue = await _EngineCoreDBContext.SysLookupValue.Where(x => x.Shortcut == "MALE").FirstOrDefaultAsync();
                if (lookupValue != null)
                    return lookupValue.Id;
            }


            throw new InvalidOperationException("invalid gender type ");
        }
        private string GenerateToken(Claim[] claims, string key)
        {
            int tokenPeriodInMinutes = 200;

            if (_IConfiguration["jwt:TokenInMinutes"] == null)
            {
                _logger.LogInformation("Warning!!! jwt:TokenInMinutes is missing");
            }
            else
            {
                bool success = int.TryParse(_IConfiguration["jwt:TokenInMinutes"], out int settingPeriod);
                if (!success || settingPeriod < 1)
                {
                    _logger.LogInformation("jwt:TokenInMinutes is invalid number or < 1 minute");
                }
                else
                {
                    tokenPeriodInMinutes = settingPeriod;
                }
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(signingCredentials: signingCredentials,
                claims: claims, expires: DateTime.Now.AddMinutes(tokenPeriodInMinutes));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        private Claim[] GenerateClames(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("EmirateId", user.EmiratesId),
            //    new Claim(ClaimTypes.Expiration, new DateTimeOffset().ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            foreach (var role in _roleManager.Roles.ToList())
            {
                if (_userManager.IsInRoleAsync(user, role.Name).Result)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }
            }
            return claims.ToArray();
        }
        public async Task<bool> IsExist(string EmiratId)
        {
            return await _EngineCoreDBContext.User.AnyAsync(m => m.EmiratesId == EmiratId);
        }
        public async Task<List<UserDto>> GetUsers()
        {
            List<UserDto> Users = await _EngineCoreDBContext.User.Select(x => new UserDto
            {
                Address = x.Address,
                BirthdayDate = x.BirthdayDate,
                Email = x.Email,
                EmailLang = x.EmailLang,
                EmiratesId = x.EmiratesId,
                FullName = x.FullName,
                Gender = x.Gender,
                Id = x.Id,
                Image = x.Image,
                PhoneNumber = x.PhoneNumber,
                SmsLang = x.SmsLang,
                TelNo = x.TelNo,
                ProfileStatus = (int)x.ProfileStatus,
                UserName = x.UserName
            }).ToListAsync();

            return Users;
        }
        public async Task<bool> DisabledAccount(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return true;
            if (user.ProfileStatus == Convert.ToInt32(Constants.PROFILE_STATUS.DISABLED)) return true;

            else return false;
        }
        public async Task<bool> EnabledAccount(int id)
        {
            User user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return false;
            }

            if (user.ProfileStatus == Convert.ToInt32(Constants.PROFILE_STATUS.ENABLED))
            {
                return true;
            }

            return false;
        }
        public void SignOut()
        {
            _signInManager.SignOutAsync();

            var uSERNAME = _httpContext.HttpContext.User.Identity;

            bool isAuthenticated = _httpContext.HttpContext.User.Identity.IsAuthenticated;
            if (isAuthenticated)
            {
                _signInManager.SignOutAsync();
            }
        }
        public async Task<UserDto> GetOne(int Id)
        {
            var query = _EngineCoreDBContext.User.Where(x => x.Id == Id)
                                         .Select(x => new UserDto
                                         {
                                             Address = x.Address,
                                             BirthdayDate = x.BirthdayDate,
                                             Email = x.Email,
                                             EmailLang = x.EmailLang,
                                             EmiratesId = x.EmiratesId,
                                             FullName = x.FullName,
                                             Id = x.Id,
                                             SmsLang = x.SmsLang,
                                             TelNo = x.TelNo,
                                             PhoneNumber = x.PhoneNumber,
                                             ProfileStatus = (int)x.ProfileStatus,
                                             UserName = x.UserName,
                                             AreaId = (int)x.AreaId,
                                             NatId = (int)x.NatId,
                                             Sign = x.Sign
                                         });

            return await query.FirstOrDefaultAsync();
        }
        public async Task<Dictionary<int, string>> GetEmployees()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            var allEmployeeUsers = await _userManager.GetUsersInRoleAsync(Constants.EmployeePolicy);
            dic = allEmployeeUsers.ToDictionary(x => x.Id, x => x.UserName);
            return dic;
        }

        public async Task<List<OnLineEmployee>> GetOnlineEmployees()
        {
            List<OnLineEmployee> result = new List<OnLineEmployee>();

            var logger = await (
                from employeeLog in _EngineCoreDBContext.UserLogger
                where employeeLog.LoggingDate.Date == DateTime.Now.Date
                group employeeLog by employeeLog.UserId into t
                select new { LastLog = t.Max(x => x.LoggingDate), UserId = t.Key}).ToListAsync();

            var logger2 = (
                           from employeeLog22 in logger
                            join employeeLog11 in _EngineCoreDBContext.UserLogger on 
                            new { colA = employeeLog22.UserId, colB  = employeeLog22.LastLog }  
                     equals new { colA = employeeLog11.UserId,   colB  = employeeLog11 .LoggingDate}

                          where (employeeLog11.StartWorkForEmployee == true)
                           select new { LastLog1 = employeeLog22.LastLog, UserId1 = employeeLog22.UserId}).ToList();

            var res =  (
                from employeeLog in logger2
                join useInfo in _EngineCoreDBContext.User on employeeLog.UserId1 equals useInfo.Id

                select new OnLineEmployee  {LastStartWork = employeeLog.LastLog1, UserId = employeeLog.UserId1, UserName = useInfo.FullName }).ToList();

            result.AddRange(res);

            return result;
        }

        public async Task<bool> StartStopWork(bool? start)
        {
            var userId = GetUserID();
            var logDay = await _EngineCoreDBContext.UserLogger.Where(x => x.UserId == userId && x.LoggingDate.Date == DateTime.Now.Date).OrderByDescending(c => c.LoggingDate).FirstOrDefaultAsync();
            if (logDay == null)
            {
                if (start == null)
                {
                    // first login today for the employee.
                    await _EngineCoreDBContext.UserLogger.AddAsync(new UserLogger { UserId = userId, LoggingDate = DateTime.Now, StartWorkForEmployee = false });
                    await _EngineCoreDBContext.SaveChangesAsync();
                    return false;
                }
                else
                {
                    // first login today for the employee.
                    await _EngineCoreDBContext.UserLogger.AddAsync(new UserLogger { UserId = userId, LoggingDate = DateTime.Now, StartWorkForEmployee = start });
                    await _EngineCoreDBContext.SaveChangesAsync();
                    return (bool)start;
                }
            }
            else
            {
                if (start == null)
                {
                    return (bool)logDay.StartWorkForEmployee;
                }
                else
                {
                    if (logDay.StartWorkForEmployee != start)
                    {
                        await _EngineCoreDBContext.UserLogger.AddAsync(new UserLogger { UserId = userId, LoggingDate = DateTime.Now, StartWorkForEmployee = start });
                        await _EngineCoreDBContext.SaveChangesAsync();
                        return (bool)start;
                    }
                    return (bool)start;
                }
            }
        }
        public async Task<CreateUserOldResultDto> CreateUserForOldAppParties(OldUserPostDto UserPostDto)
        {

            CreateUserOldResultDto res = new CreateUserOldResultDto();


            var emailIdOldUsers = UserPostDto.Email;
            var emaritIdOldUsers = UserPostDto.EmiratesId;

            if (UserPostDto.EmiratesId != null && UserPostDto.EmiratesId != "0" && UserPostDto.EmiratesId != "" && UserPostDto.EmiratesId.Trim() != null)
            {
                UserPostDto.EmiratesId = UserPostDto.EmiratesId.Trim();
                UserPostDto.EmiratesId = UserPostDto.EmiratesId.Trim().Replace(@"_", "");
                UserPostDto.EmiratesId = UserPostDto.EmiratesId.Trim().Replace(@"-", "");
                UserPostDto.EmiratesId = UserPostDto.EmiratesId.Trim().Replace(@"/", "");

                if (!UserPostDto.EmiratesId.StartsWith("784") || UserPostDto.EmiratesId.Length != 15)
                {
                    UserPostDto.EmiratesId = "Invalid_Old_" + UserPostDto.EmiratesId;
                }
            }
            else
            {

                var existUserName = await _EngineCoreDBContext.User.Where(x => x.FullName.Trim() == UserPostDto.FullName.Trim()).FirstOrDefaultAsync();
                if (existUserName != null)
                {
                    res.UserId = existUserName.Id;
                    res.User = UserPostDto;
                    res.Message = "Existed before same full Name";
                    return res;
                }

                UserPostDto.EmiratesId = "Invalid_Old_" + _iGeneralRepository.GetNewValueBySec();
            }

            var existUser = await _EngineCoreDBContext.User.Where(x => x.EmiratesId.Trim() == UserPostDto.EmiratesId.Trim()).FirstOrDefaultAsync();
            if (existUser != null)
            {
                res.UserId = existUser.Id;
                res.User = UserPostDto;
                res.Message = "Existed before same Emarit id";
                return res;
            }


            if (UserPostDto.Email == null || UserPostDto.Email == "0" || UserPostDto.Email == "")
            {
                string Email = Constants.INVALID_EMAIL_PREFIX + _iGeneralRepository.GetNewValueBySec() + Constants.INVALID_EMAIL_SUFFIX;
                UserPostDto.Email = Email;
            }
            else
            {
                if (await _EngineCoreDBContext.User.AnyAsync(x => x.Email.Trim() == UserPostDto.Email.Trim()))
                {
                    string Email = Constants.INVALID_EMAIL_PREFIX + _iGeneralRepository.GetNewValueBySec() + Constants.INVALID_EMAIL_SUFFIX;
                    UserPostDto.Email = Email;
                }
            }


            int index = UserPostDto.Email.IndexOf("@");
            UserPostDto.UserName = UserPostDto.Email.Substring(0, index);


            existUser = await _EngineCoreDBContext.User.Where(x => x.UserName.Trim() == UserPostDto.UserName.Trim()).FirstOrDefaultAsync();
            if (existUser != null)
            {
                res.UserId = existUser.Id;
                res.User = UserPostDto;
                res.Message = "Existed before same userName id";
                return res;
            }

            if (UserPostDto.PhoneNumber != null)
            {
                UserPostDto.PhoneNumber = GetPhoneNumberWithCode(UserPostDto.PhoneNumber);
                if (UserPostDto.PhoneNumber.Length > 25)
                {
                    throw new System.InvalidOperationException("invalid phone number");
                }
            }



            try
            {

                User newUser = new User()
                {
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    PhoneNumber = UserPostDto.PhoneNumber,
                    PasswordHash = UserPostDto.PasswordHash,
                    EmailConfirmed = false,
                    NormalizedEmail = UserPostDto.Email.ToUpper(),
                    Email = UserPostDto.Email,
                    NormalizedUserName = UserPostDto.UserName.ToLower(),
                    UserName = UserPostDto.UserName,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    FullName = UserPostDto.FullName,
                    BirthdayDate = UserPostDto.BirthdayDate,
                    Gender = UserPostDto.Gender,
                    TelNo = UserPostDto.TelNo,
                    Address = UserPostDto.Address,
                    EmiratesId = UserPostDto.EmiratesId,
                    CreatedDate = DateTime.Now,
                    ProfileStatus = Convert.ToInt32(Constants.PROFILE_STATUS.ENABLED),
                    EmailIdOldUsers = emailIdOldUsers,
                    EmaritIdOldUsers = emaritIdOldUsers
                };


                var result = await _userManager.CreateAsync(newUser, UserPostDto.PasswordHash);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(x => x.Description).ToList();
                    string errorResult = "";
                    foreach (var x in errors)
                    {
                        errorResult = errorResult + " , " + x;
                    }
                    res.UserId = existUser.Id;
                    res.User = UserPostDto;
                    res.Message = errorResult;
                    return res;
                }


                var resRole = await EditUserRolesAsync(newUser.Id, UserPostDto.UserRoles);
                if (!resRole.Succeeded)
                {
                    res.UserId = newUser.Id;
                    res.User = UserPostDto;
                    res.Message = "Success in adding but failed in adding role";
                    return res;
                }

                res.UserId = newUser.Id;
                res.User = UserPostDto;
                res.Message = "Success";
                return res;

            }

            catch (Exception ex)
            {
                res.UserId = 0;
                res.User = UserPostDto;
                res.Message = "Failed," + ex.Message;
                return res;
            }
        }


        private int GetNewValueBySec()
        {
            var p = new SqlParameter("@result", System.Data.SqlDbType.Int);
            p.Direction = System.Data.ParameterDirection.Output;
            _EngineCoreDBContext.Database.ExecuteSqlRaw("set @result = next value for Sequence_login", p);
            int sequenceNum = (int)p.Value;
            return sequenceNum;
        }

        public async Task<object> Addoldusers()
        {

            OldUserPostDto UserPostDto = new OldUserPostDto();


            List<ApplicationParty> applicationParty = await _EngineCoreDBContext.ApplicationParty.ToListAsync();



            List<CreateUserOldResultDto> res = new List<CreateUserOldResultDto>();
            int success = 0;
            int failed = 0;
            try
            {


                foreach (var Row in applicationParty)
                {
                    UserPostDto.Address = Row.Address;
                    UserPostDto.BirthdayDate = Row.BirthDate;
                    UserPostDto.Email = Row.Email;
                    UserPostDto.FullName = Row.FullName;
                    UserPostDto.PasswordHash = "P@ssw0rd_";
                    UserPostDto.ImageFile = null;
                    UserPostDto.Gender = "";
                    UserPostDto.EmiratesId = Row.EmiratesIdNo;
                    UserPostDto.UserName = Row.Email;
                    UserPostDto.PhoneNumber = Row.Mobile;
                    UserPostDto.UserRoles = new List<int> { 15 };


                    var res1 = await CreateUserForOldAppParties(UserPostDto);
                    if (res1.UserId == 0)
                    {
                        failed++;
                        res.Add(res1);
                    }
                    else
                    {
                        ApplicationParty appParty = Row;
                        appParty.PartyId = res1.UserId;
                        _iGeneralRepository.Update(appParty);
                        await _iGeneralRepository.Save();

                        success++;
                    }


                }

            }
            catch (Exception ex)
            {
                return new { ex.Message, failed, success };
            }
            return res.Count;
        }

    }
}
