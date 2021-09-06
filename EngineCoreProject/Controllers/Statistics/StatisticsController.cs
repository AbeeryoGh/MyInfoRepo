using EngineCoreProject.DTOs.Statistics;
using EngineCoreProject.IRepository;
using EngineCoreProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Controllers.Statistics
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsRepository _iStatisticsRepository;
        public StatisticsController(IStatisticsRepository iStatisticsRepository)
        {
            _iStatisticsRepository = iStatisticsRepository;

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("CountAll")]
        public async Task<ActionResult> GetCount()
        {
            var Statistics = await _iStatisticsRepository.countAll();
            return Ok(Statistics);
        }
        //[HttpGet("signtype")]
        //public async Task<ActionResult> signtype(int id)
        //{
        //    var Statistics =  _iStatisticsRepository.Signtype(id);
        //    return Ok(Statistics);
        //}

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("GetCharts")]
        public async Task<ActionResult> GetCharts([FromHeader] string lang, searchChartsDto searchCharts)
        {
            var Statistics = await _iStatisticsRepository.AllCharts(lang, searchCharts);
            return Ok(Statistics);
        }


       [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("NotaryApps")]
        public async Task<ActionResult> NotaryApps(searchChartsDto searchCharts)
        {
            var Statistics = await _iStatisticsRepository.NotaryAppsDone(searchCharts);
            return Ok(Statistics);
        }
    }
}
