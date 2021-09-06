
using EngineCoreProject.DTOs.AramexDto;
using EngineCoreProject.DTOs.BlockChainDto;
using EngineCoreProject.DTOs.Credential;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.IAramex;
using EngineCoreProject.IRepository.ICredential;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  EngineCoreProject.Controllers.Aramex
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AramexController : ControllerBase
    {
        private readonly IAramexRepository _IAramexRepository;
        public AramexController(IAramexRepository iAramexRepository)
        {
            _IAramexRepository = iAramexRepository;

        }
        [HttpPost("SearchAramex/{op}")]
        public async Task<ActionResult> search([FromRoute] int op,[FromHeader] string lang,searchAramexDto searchDto)
        {
            var Statistics =  await _IAramexRepository.AramexSearch(lang,searchDto,op);
            return Ok(Statistics);
        }


        [HttpGet("AramexDetails")]
        public async Task<ActionResult> searchDetails([FromHeader] string lang, int appId)
        {
            var Statistics = await _IAramexRepository.AramexDetails(appId,lang);
            return Ok(Statistics);
        }

        [HttpPost("Update")]
        public async Task<ActionResult> update( int appId, AramexPostDto aramexPostDto)
        {
            var Statistics = await _IAramexRepository.UpdateAramexRequest(appId, aramexPostDto);
            return Ok(Statistics);
        }

    }
}
