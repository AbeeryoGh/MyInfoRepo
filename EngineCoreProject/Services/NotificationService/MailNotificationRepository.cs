using EngineCoreProject.Services;
using EngineCoreProject.DTOs;
using EngineCoreProject.DTOs.NotificationDtos;
using EngineCoreProject.IRepository.INotificationSettingRepository;
using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Collections.Generic;
using EngineCoreProject.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EngineCoreProject.DTOs.ChannelDto;
using MailKit.Security;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeKit.Utils;

namespace EngineCoreProject.Services.NotificationService
{
    class MailNotification : INotificationObserver
    {
        private readonly ChannelMailFirstSetting _mailSettings;
        private readonly List<NotificationLogPostDto> _notificationsLogPostDto;
        private readonly EngineCoreDBContext _EngineCoreDBContext;

        public MailNotification(List<NotificationLogPostDto> notificationsLogPostDto, IOptions<ChannelMailFirstSetting> mailSettings, EngineCoreDBContext EngineCoreDBContext)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _mailSettings = mailSettings.Value;
            _notificationsLogPostDto = new List<NotificationLogPostDto>();
            _notificationsLogPostDto = notificationsLogPostDto;
        }

        public async Task<List<NotificationLogPostDto>> Notify(bool sendImmediately)
        {
            List<NotificationLogPostDto> res = new List<NotificationLogPostDto>();
            foreach (var notify in _notificationsLogPostDto)
            {
                using (var client = new SmtpClient())
                    try
                    {
                        if (sendImmediately)
                        {
                            notify.HostSetting = "Host: " + _mailSettings.Host + "Sender: " + _mailSettings.Mail + "Port: " + _mailSettings.Port;
                            client.CheckCertificateRevocation = false;

                            // configuration for deployment.
                            await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.None);

                            // configuration for local.
                            // await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, false);

                            // Note: Not all SMTP servers support authentication, but GMail does.
                            if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication))
                            {
                                client.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                            }

                            var email = new MimeMessage
                            {
                                Sender = MailboxAddress.Parse(_mailSettings.Mail),
                                Subject = notify.NotificationTitle
                            };

                            email.To.Add(MailboxAddress.Parse(notify.ToAddress));


                            var atringBuilder = new StringBuilder();
                            var builder = new BodyBuilder();

                            try
                            {
                                // try to send by template HTML.
                                using (var reader = File.OpenText(@"wwwroot\Template_Email\Template.html"))
                                {
                                    atringBuilder.Append(reader.ReadToEnd());
                                    atringBuilder = atringBuilder.Replace("{body}", notify.NotificationBody);
                                }

                                var imageLogo = builder.LinkedResources.Add(@"wwwroot\Template_Email\logo\MOJ_LOGO.png");
                                imageLogo.ContentId = MimeUtils.GenerateMessageId();
                                atringBuilder = atringBuilder.Replace("{logo}", imageLogo.ContentId);

                                var footerIcon = builder.LinkedResources.Add(@"wwwroot\Template_Email\logo\footer.png");
                                footerIcon.ContentId = MimeUtils.GenerateMessageId();
                                atringBuilder = atringBuilder.Replace("{footer}", footerIcon.ContentId);

                            }
                            catch (Exception ex)
                            {
                                atringBuilder.Append(notify.NotificationBody);
                                Console.WriteLine(ex.Message);
                            }

                            builder.HtmlBody = atringBuilder.ToString();
                            email.Body = builder.ToMessageBody();
                            client.Send(email);
                            client.Disconnect(true);

                            notify.ReportValueId = email.MessageId;
                            notify.IsSent = (int)Constants.NOTIFICATION_STATUS.SENT;
                            notify.UpdatedDate = DateTime.Now;
                            notify.SentCount += 1;
                            string autoMessageId = Guid.NewGuid().ToString();
                            notify.SendReportId = autoMessageId;
                        }
                        else
                        {
                            notify.IsSent = (int)Constants.NOTIFICATION_STATUS.PENDING;
                            notify.SentCount = 0;
                        }

                    }

                    catch (ArgumentNullException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" host or user name or password is null error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        client.Disconnect(true);
                    }

                    catch (ArgumentOutOfRangeException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" port is not between 0 and 65535 error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        client.Disconnect(true);
                    }

                    catch (ArgumentException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" The host is a zero-length string error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        client.Disconnect(true);
                    }

                    catch (ObjectDisposedException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" MailKit.Net.Smtp.SmtpClient has been disposed error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        client.Disconnect(true);
                    }

                    catch (InvalidOperationException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" The MailKit.Net.Smtp.SmtpClient is already connected or authenticated error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        client.Disconnect(true);
                    }

                    catch (NotSupportedException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" options was set to MailKit.Security.SecureSocketOptions.StartTls and the SMTP server does not support the STARTTLS extension. error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        client.Disconnect(true);
                    }

                    catch (OperationCanceledException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" The operation was canceled error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        client.Disconnect(true);
                    }

                    catch (System.Net.Sockets.SocketException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" A socket error occurred trying to connect to the remote host error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        client.Disconnect(true);
                    }

                    catch (MailKit.Security.SslHandshakeException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format("  An error occurred during the SSL/TLS negotiations. error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        client.Disconnect(true);
                    }

                    catch (System.IO.IOException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format("An I/O error occurred at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        client.Disconnect(true);
                    }

                    catch (SmtpCommandException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" An SMTP command failed. Error at attempt {0}, trying to connect: {1}", notify.SentCount, ex.Message);
                        client.Disconnect(true);
                    }

                    catch (SmtpProtocolException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" An SMTP protocol error occurred error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        client.Disconnect(true);
                    }

                    catch (AuthenticationException ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format("  Authentication using the supplied credentials has failed. error at attempt {0}, while trying to connect: {1}", notify.SentCount, ex.Message);
                        client.Disconnect(true);
                    }

                    catch (Exception ex)
                    {
                        notify.IsSent = (int)Constants.NOTIFICATION_STATUS.ERROR;
                        notify.UpdatedDate = DateTime.Now;
                        notify.SentCount += 1;
                        notify.SendReportId += string.Format(" Inner error at attempt {0}, is {1}", notify.SentCount, ex.Message);
                        client.Disconnect(true);
                    }

                res.Add(notify);
            }
            return res;
        }
    }
}
