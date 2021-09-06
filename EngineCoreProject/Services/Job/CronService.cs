using System;
using System.Threading;
using System.Threading.Tasks;
//using EngineCoreProject.IRepository.IEmail;
using EngineCoreProject.IRepository.IPaymentRepository;
using EngineCoreProject.IRepository.INotificationSettingRepository;
using EngineCoreProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EngineCoreProject.IRepository.IMeetingRepository;
using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.IFilesUploader;


namespace EngineCoreProject.Services.Job
{
    public interface ICronService
    {
        Task DoWork(CancellationToken cancellationToken);
    }

    public class CronService : ICronService
    {
        private readonly ILogger<CronService> _logger;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IMeetingRepository _ImeetingRepository;
        private readonly IFilesUploaderRepositiory _IFilesUploaderRepositiory;
        private readonly ISendNotificationRepository _ISendNotificationRepository;
        private readonly IBlockChain _IBlockChain;
        private readonly IPaymentRepository _IPaymentRepository;

        public CronService( IBlockChain iBlockChain, ILogger<CronService> logger, IServiceScopeFactory scopeFactory, ISendNotificationRepository iSendNotificationRepository, 
                            IMeetingRepository imeetingRepository, IPaymentRepository iPaymentRepository, IFilesUploaderRepositiory iFilesUploaderRepositiory)

        {
            _logger = logger;
            this.scopeFactory = scopeFactory;
            _ISendNotificationRepository = iSendNotificationRepository;
            _IPaymentRepository = iPaymentRepository;
             _ImeetingRepository = imeetingRepository;
            _IBlockChain = iBlockChain;
            _IFilesUploaderRepositiory = iFilesUploaderRepositiory;
        }

        public async Task DoWork(CancellationToken cancellationToken)
        {
            using (var scope = scopeFactory.CreateScope())
            {

                var _EngineCoreDBContext = scope.ServiceProvider.GetRequiredService<EngineCoreDBContext>();
                // _logger.LogInformation("start pay");
                try
                {
                    await _IPaymentRepository.UpdatePaymentTableCrons();
                }
                catch (Exception e)
                {
                    _logger.LogInformation("error in updating payments " + e.Message);
                }

                //_logger.LogInformation("end pay");

                _logger.LogInformation("start BlockChain");
                try
                {
                    await _IBlockChain.ResendAsync();
                }
                catch (Exception e)
                {
                    _logger.LogInformation("error in updating BlockChain " + e.Message);
                }
                _logger.LogInformation("end BlockChain");

                try
                {
                    _logger.LogInformation($"{DateTime.Now:hh:mm:ss} ====sending failed Emails and SMS>>");
                    await _ISendNotificationRepository.ReSend();
                }
                catch (Exception e) 
                {
                    _logger.LogInformation("error in sending fail notifications " + e.Message);
                }


                // Stopped by BASHAR.
                //try
                //{
                    //_logger.LogInformation($"{DateTime.Now:hh:mm:ss} ====sending Queue Emails and SMS>>");
                   // await _iSendNotificationRepository.NotifyQueue();
                //}
                //catch (Exception e)
                //{
                   // _logger.LogInformation("error in sending Queue Emails and SMS notifications " + e.Message);
                //}


                // Daily Job.
                try
                {
                    if (DateTime.Now.Minute > 1200 && DateTime.Now.Minute < 1220)
                    {
                        _logger.LogInformation($"{DateTime.Now:hh:mm:ss} ====get meeting logger >>");
                        await _ImeetingRepository.GetMeetingLogger();

                        // Postpone to manual delete
                        _logger.LogInformation($"{DateTime.Now:hh:mm:ss} ==== DeleteTemporaryFiles >>");
                        _logger.LogInformation(" the count od deleted files is : " + _IFilesUploaderRepositiory.DeleteTemporaryFiles(DateTime.Now).ToString());
                    }
                }
                catch (Exception e)
                {
                    _logger.LogInformation(" error daily job " + e.Message);
                }


                try
                {
                   // _logger.LogInformation("start regenerate messed pdf");
                  //  await _ICronsRepository.GenerateMessedPdf();
                }
                catch (Exception e)
                {
                    _logger.LogInformation("error in  GenerateMessedPdf " + e.Message);
                }

                //_logger.LogInformation("end regenerate messed pdf");
            }

            await Task.Delay(1000 * 20, cancellationToken);
        }
    }
}
