using EngineCoreProject.DTOs.AccountDto;
using EngineCoreProject.DTOs.UnifiedGateDto;
using EngineCoreProject.IRepository.IUnifiedGateSubServicesRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.UnifiedGate
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class remoteloginController : ControllerBase
    {
        private readonly IUnifiedGateSubServicesRepository _IUnifiedGateSubServicesRepository;
        public remoteloginController(IUnifiedGateSubServicesRepository iUnifiedGateSubServicesRepository)
        {
            _IUnifiedGateSubServicesRepository = iUnifiedGateSubServicesRepository;
        }


        [HttpPost("{EmiratesId}/{MobileNumber}/{Hash}/{ServiceId}/{Email}/{Lang}/{theme}")]
        public async Task<ActionResult> RemoteSignInAsync([FromRoute] string EmiratesId, [FromRoute] string MobileNumber, [FromRoute] string Hash, [FromRoute] string ServiceId, [FromRoute] string Email, [FromRoute] string Lang, [FromRoute] string theme)
        {
            RemoteLoginDto remoteLoginDto = new RemoteLoginDto();
            remoteLoginDto.MobileNumber = MobileNumber;
            remoteLoginDto.ServiceId = ServiceId;
            remoteLoginDto.EmiratesId = EmiratesId;
            remoteLoginDto.Email = Email;
            remoteLoginDto.Lang = Lang;
            remoteLoginDto.Hash = Hash;
            remoteLoginDto.theme = theme;

            LogInResultDto result = await _IUnifiedGateSubServicesRepository.remotelogin(remoteLoginDto, Lang);
            if (result == null)
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error! Please try again");
            else
            {
                if (result.StatusCode == "200")
                    return this.StatusCode(StatusCodes.Status200OK, result);

                if (result.StatusCode == "401")
                    return this.StatusCode(StatusCodes.Status401Unauthorized, "You do not have permission to access the enotary system");

                else
                    return this.StatusCode(StatusCodes.Status404NotFound, "User isn't found at unified gate system");
            }

        }

    }
}
