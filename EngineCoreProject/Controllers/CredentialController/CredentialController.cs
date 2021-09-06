using EngineCoreProject.DTOs.Credential;
using EngineCoreProject.IRepository.ICredential;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.CredentialController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CredentialController : ControllerBase
    {
        private readonly ICredentialRepository _iCredentialRepository;
        public CredentialController(ICredentialRepository iCredentialRepository)
        {
            _iCredentialRepository = iCredentialRepository;

        }
        [HttpPost("Prefetch")]
        public async Task<ActionResult> Prefetch( PrefetchRequestDto prefetchRequestDto)
        {
            var Statistics = await _iCredentialRepository.Prefetch(prefetchRequestDto);
            return Ok(Statistics);
        }

        [HttpPost("RequestCredentials")]
        public async Task<ActionResult> RequestCredentials(RequestCredentialsReqDto prefetchRequestDto)
        {
            var Statistics = await _iCredentialRepository.RequestCredentials(prefetchRequestDto);
            return Ok(Statistics);
        }


        [HttpPost("UpdateVCID")]
        public async Task<ActionResult> UpdateVCID(UpdateVCIDDto updateVCIDDto)
        {
            var Statistics = await _iCredentialRepository.UpdateVCID(updateVCIDDto);
            return Ok(Statistics);
        }


        


    }
}
