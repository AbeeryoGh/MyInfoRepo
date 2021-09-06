using EngineCoreProject.Services;
using EngineCoreProject.DTOs.ChannelDto;
using EngineCoreProject.IRepository.IChannelRepository;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MailKit.Security;
using System.Net.Sockets;

namespace EngineCoreProject.Services.Channels
{
    public class ChannelRepository : IChannelRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _iGeneralRepository;
        private IConfiguration _configuration;

        public ChannelRepository(EngineCoreDBContext EngineCoreDBContext, IConfiguration configuration, IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _configuration = configuration;

        }

        public List<ChannelGetDto> GetChannelsName(string lang)
        {
            var query = from srv in _EngineCoreDBContext.SysLookupType
                        join s in _EngineCoreDBContext.SysLookupValue on srv.Id equals s.LookupTypeId
                        join stg in _EngineCoreDBContext.SysTranslation on s.Shortcut equals stg.Shortcut
                        where srv.Value == Constants.NOTIFICATION_CHANNEL && stg.Lang == lang
                        select new ChannelGetDto { ChannelName = stg.Value, ChannelNameShortcut = stg.Shortcut, Id = s.Id };

            List<ChannelGetDto> ChannelName = query.ToList<ChannelGetDto>();

            return ChannelName;
        }
        
        public void AddChannelMailFirstSetting(ChannelMailFirstSetting config)
        {
            _configuration["ChannelMailFirstSetting:Host"]     = config.Host;
            _configuration["ChannelMailFirstSetting:Port"]     = config.Port.ToString();
            _configuration["ChannelMailFirstSetting:Mail"]     = config.Mail;
            _configuration["ChannelMailFirstSetting:Password"] = config.Password;
        }

        public ChannelMailFirstSetting GetChannelMailFirstConfig()
        {
            return _configuration.GetSection("ChannelMailFirstSetting").Get<ChannelMailFirstSetting>();
        }


        public void TestEmailFirstSettingConnection(ChannelMailFirstSetting config, string lang)
        {
            try
            {
                using var client = new MailKit.Net.Smtp.SmtpClient();

                client.Connect(config.Host, config.Port, false);
                client.Authenticate(config.Mail, config.Password);
                client.Disconnect(true);

            }
            catch (AuthenticationException)
            {
                throw new InvalidOperationException(Constants.getMessage(lang, "errorAuthentication"));
            }
            catch (SocketException)
            {
                throw new InvalidOperationException(Constants.getMessage(lang, "errorConfig"));
            }
        }
    }
}

