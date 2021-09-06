using EngineCoreProject.DTOs.BasherDto;
using EngineCoreProject.IRepository.IBasherRepository;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.G2G
{

    [ApiController]
    [Route("api/v1/[controller]")]
    public class G2GController : ControllerBase
    {
        private readonly INotaryFee _iNotaryFee;
        private readonly IMOADetails _imOADetails;
        public G2GController(INotaryFee iNotaryFee, IMOADetails imOADetails)
        {
             _iNotaryFee = iNotaryFee;
            _imOADetails = imOADetails;
        }

        [HttpPost("RetrieveNotaryFees_MOJ")]
        public async Task<IActionResult> RetrieveNotaryFees_MOJ(RetrieveNotaryFeesMOJRequest logInDto)
        {
            var  obj = await _iNotaryFee.RetrieveNotaryFees_MOJ(logInDto);
            return Ok(obj);
        }


        [HttpPost("SendMOADetails_MOJ")]
        public async Task<IActionResult> SendMOADetails_MOJ(SendAppMOADetails_MOJ sendAppMOADetails_MOJ)
        {
            var obj = await _imOADetails.SendMOADetails_MOJ(sendAppMOADetails_MOJ);
            return Ok(obj);
        }

    }
}
