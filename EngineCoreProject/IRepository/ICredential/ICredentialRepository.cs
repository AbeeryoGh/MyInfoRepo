using EngineCoreProject.DTOs.BlockChainDto;
using EngineCoreProject.DTOs.Credential;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.ICredential
{
    public interface ICredentialRepository
    {
 
            Task<object> Prefetch(PrefetchRequestDto prefetchRequestDto);
            Task<object> RequestCredentials(RequestCredentialsReqDto requestCredentialsReqDto);
            Task<UpdateVCIDResDto> UpdateVCID(UpdateVCIDDto updateVCIDDto);
         

    }
    

}

