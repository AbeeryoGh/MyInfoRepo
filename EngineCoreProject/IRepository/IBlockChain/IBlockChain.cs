using EngineCoreProject.DTOs.BlockChainDto;
using EngineCoreProject.DTOs.Credential;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository
{
    public interface IBlockChain
    {
        
        bool GetVcID(int appid);
        bool RevokevcID(RevokeDto revokeDto,int appid);
        Task<List<BlockChainPoa>> GetBlockChain(string opr);
        Task<bool> ResendAsync();
       
    }
}
