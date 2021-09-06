using EngineCoreProject.DTOs.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository
{
    public interface IStatisticsRepository
    {
        Task<StatisticsDto> countAll();
        Task<ChartsDto> AllCharts(string lang, searchChartsDto searchCharts);
        //int TypeofSign(int id);
        Task<object> NotaryAppsDone(searchChartsDto searchCharts);
        
    }
}
