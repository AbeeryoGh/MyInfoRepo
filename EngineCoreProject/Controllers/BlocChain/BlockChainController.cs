using EngineCoreProject.DTOs.BlockChainDto;
using EngineCoreProject.DTOs.Credential;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.ICredential;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.BlockChain
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlockChainController : ControllerBase
    {
        private readonly IBlockChain _iBlockChain;
        public BlockChainController(IBlockChain iBlockChain)
        {
            _iBlockChain = iBlockChain;

        }
        [HttpPost("RequestForVCID")]
        public async Task<ActionResult> RequestForVCID(int id )
        {
            var Statistics =  _iBlockChain.GetVcID(id);
            return Ok(Statistics);
        }

        [HttpPost("RevokeCredential")]
        public async Task<ActionResult> RevokeVcID(RevokeDto revokeDto,int id)
        {
            var Statistics =  _iBlockChain.RevokevcID(revokeDto,id);
            return Ok(Statistics);
        }

        [HttpGet("GetBlockChain")]
        public async Task<ActionResult> getchains( string opr)
        {
            var Statistics =await _iBlockChain.GetBlockChain(opr);
            return Ok(Statistics);
        }

        [HttpPost("ReSend")]
        public async Task<ActionResult> resend()
        {
            var Statistics =await _iBlockChain.ResendAsync();
            return Ok(Statistics);
        }

    }
}
