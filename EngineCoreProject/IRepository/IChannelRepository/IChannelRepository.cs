using EngineCoreProject.DTOs.ChannelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IChannelRepository
{
    public interface IChannelRepository
    {
       List<ChannelGetDto> GetChannelsName(string lang);
       ChannelMailFirstSetting GetChannelMailFirstConfig();
       void AddChannelMailFirstSetting (ChannelMailFirstSetting channelMailFirstSetting);

        void TestEmailFirstSettingConnection(ChannelMailFirstSetting config, string lang);
    }
}
