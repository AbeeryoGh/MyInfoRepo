using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

namespace EngineCoreProject.DTOs.Statistics
{
    public class ChartsDto
    {
        public int countAppByservice { get; set; }
        public int countAppByStatus { get; set; }
        public int countAppByChannels { get; set; }
        public int countAppByEmirates { get; set; }
        public List<serviceCharts> serviceCharts { get; set; }
        public List<serviceCharts> serviceChartsXls { get; set; }
        public List<appStatusCharts> appStatusCharts { get; set; }
        public List<appChannelsCharts> appChannelsCharts { get; set; }
        public List<ServiceInfoDto> ServiceInfo { get; set; }
        public List<UserInfo> Userinfo { get; set; }
    }
}
public class serviceCharts
{
    public string KhadamatiNo {get;set;}
    public int ServiceId { get; set; }
    public string name { get; set; }
    public int uv { get; set; }
    public int pv { get; set; }
    public string fill { get; set; }
    public string icon { get; set; }
    public int? CountWeb { get; set; }
    public int? CountMobile { get; set; }
    public int? CountNotary { get; set; }
    public DateTime? LastAppDate { get; set; }


}

public class appStatusCharts
{
    public int appStateId { get; set; }
    public string statusName { get; set; }
    public int Appcount { get; set; }
}

public class appChannelsCharts
{
    public int appChannelId { get; set; }
    public string channelName { get; set; }
    public int Appcount { get; set; }
}

public class ServiceInfoDto
{
    public int? serviceId { get; set; }
    public int ChannelId { get; set; }
    public int TransactionId { get; set; }
    public int Userid { get; set; }
    public string ApplicationNo { get; set; }
    public string ServiceName { get; set; }
    public string channelName { get; set; }
    public DateTime? ApplicationDate { get; set; }
    public string Emirate { get; set; }
    public string BirthDate { get; set; }
    public string Gender { get; set; }
    public string ID { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string NationalityAr { get; set; }
    public string NationalityEn { get; set; }
    public string stageName { get; set; }

}

public class UserInfo
{
    public string username { get; set; }
    public string nationality { get; set; }
    public string mobile { get; set; }
    public string email { get; set; }
    public string BirthDate { get; set; }
    public string Gender { get; set; }
    public string ID { get; set; }
    public int appCount { get; set; }

}