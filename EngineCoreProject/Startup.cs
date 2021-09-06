using EngineCoreProject.DTOs.KeyDto;
using EngineCoreProject.DTOs.JWTDto;
using EngineCoreProject.DTOs.SMSDto;
using EngineCoreProject.DTOs.Payment;
using EngineCoreProject.DTOs.UnifiedGateDto;

using EngineCoreProject.Services;
using EngineCoreProject.Services.SysLookUpService;
using EngineCoreProject.Services.Channels;
using EngineCoreProject.Services.NotificationService;
using EngineCoreProject.Services.GeneralSetting;
using EngineCoreProject.Services.TemplateSetService;
using EngineCoreProject.Services.Meetings;
using EngineCoreProject.Services.Admservice;
using EngineCoreProject.Services.Job;
using EngineCoreProject.Services.Queue;
using EngineCoreProject.Services.CalendarService;
using EngineCoreProject.Services.MyApplication;
using EngineCoreProject.Services.ApplicationSet;
using EngineCoreProject.Services.FilesUploader;
using EngineCoreProject.Services.ActionButton;
using EngineCoreProject.Services.UnifiedGateSubServicesRepository;
using EngineCoreProject.Services.Payment;

using EngineCoreProject.IRepository;
using EngineCoreProject.IRepository.IChannelRepository;
using EngineCoreProject.IRepository.INotificationSettingRepository;
using EngineCoreProject.IRepository.ISysLookUpRepository;
using EngineCoreProject.IRepository.ITemplateSetRepository;
using EngineCoreProject.IRepository.TemplateSetRepository;
using EngineCoreProject.IRepository.IMeetingRepository;
using EngineCoreProject.IRepository.IGeneralSetting;
using EngineCoreProject.IRepository.IQueueRepository;
using EngineCoreProject.IRepository.ICalendarRepository;
using EngineCoreProject.IRepository.AdmServices;
using EngineCoreProject.IRepository.IUnifiedGateSubServicesRepository;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.IRepository.IFilesUploader;
using EngineCoreProject.IRepository.IPaymentRepository;
using EngineCoreProject.IRepository.IActionButtonRepository;
using EngineCoreProject.IRepository.IMyApplications;
using EngineCoreProject.IRepository.IOCRRepository;

using EngineCoreProject.Models;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using DinkToPdf.Contracts;
using DinkToPdf;
using PDF_Generator.Utility;
using EngineCoreProject.DTOs.PDFGenerator;
using EngineCoreProject.DTOs.ChannelDto;
using Microsoft.AspNetCore.Identity;
using EngineCoreProject.IRepository.IGeneratorRepository;
using EngineCoreProject.Services.GeneratorServices;
using EngineCoreProject.IRepository.IUserRepository;
using EngineCoreProject.Services.UserService;
using EngineCoreProject.Services.RoleService;
using EngineCoreProject.IRepository.IRoleRepository;
using EngineCoreProject.IRepository.ITabRepository;
using EngineCoreProject.Services.TabService;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using EngineCoreProject.Services.ConfigureWritable;
using EngineCoreProject.DTOs.ConfigureWritableDto;

using EngineCoreProject.Services.StatisticsService;
using EngineCoreProject.Services.OCR;

using System.ServiceModel;
using SoapCore;
using EngineCoreProject.IRepository.IBasherRepository;
using EngineCoreProject.Services.Basher;

using EngineCoreProject.IRepository.IEPOSMachineRepository;
using EngineCoreProject.Services.EPOSMachine;
using EngineCoreProject.Services.TransactionFeeService;
using EngineCoreProject.IRepository.ITransactionFeeRepository;
using Microsoft.AspNetCore.Authentication.Negotiate;
using ReactWindowsAuth;
using Microsoft.AspNetCore.Server.IISIntegration;
using EngineCoreProject.DTOs.EPOSMachineDto;
using EngineCoreProject.IRepository.ICredential;
using EngineCoreProject.Services.Credential;
using EngineCoreProject.Services.BlockChain;
using EngineCoreProject.Services.Aramex;
using EngineCoreProject.IRepository.IAramex;
using EngineCoreProject.IRepository.IBlackList;
using EngineCoreProject.Services.BackList;

namespace EngineCoreProject
{
    public class Constantss
    {
        public const string CORS_ORIGINS = "CorsOrigins";
    }
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll"));

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            services.AddControllers();
            services.AddCors(opt =>
            {
                opt.AddDefaultPolicy(builder => builder
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowAnyOrigin());

                opt.AddPolicy("ACTIVE_DIRECTORY", builder => builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins(
                                 "http://unifiedgateuat.moj.gov.ae",
                                 "http://enotaryuat.moj.gov.ae",
                                 "http://localhost:3000",
                                 "https://localhost:3000",
                                 "http://localhost:80",
                                 "https://localhost:80",
                                 "https://localhost",
                                 "http://localhost",
                                 "https://localhost:44342",
                                 "http://localhost:44342",
                                 "http://172.16.58.43",
                                 "https://172.16.58.43")

                      .AllowCredentials()
                     );
            });
            //services.AddCronJob<MyCronJob1>(c =>
            //{
            //    c.TimeZoneInfo = TimeZoneInfo.Local;
            //    c.CronExpression = @"*/5 * * * *";
            //});

            //MyCronJob2 calls the scoped service MyScopedService
            services.AddCronJob<WorkerJob>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"* * * * *";
            });

            //services.AddCronJob<WorkerJob>(c =>
            //{
            //    c.TimeZoneInfo = TimeZoneInfo.Local;
            //    c.CronExpression = @"* * * * *";
            //});


            //   services.AddMvc(
            ///    config => config.Filters.Add(new MyExceptionFilter())
            //    );

            services.AddMvc(option => option.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddDbContext<EngineCoreDBContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("LilacNetDB")));
            //services.AddCors();

            //----------------------------UnifiedGate-------------------
            services.Configure<SignInWithUGateSettings>(Configuration.GetSection("SignInWithUGateSettings"));//
            //---------------------Key ----------------------------
            services.Configure<SecretKey>(Configuration.GetSection("SecretKey"));

            ////////////////////////generate pdf///
            services.Configure<FileNaming>(Configuration.GetSection("PdfFileNaming"));//

            //---------------------token ----------------------------
            services.Configure<jwt>(Configuration.GetSection("Jwt"));
            var Key = Encoding.ASCII.GetBytes("this-is-my-secret-key");

            services.AddAuthentication(au =>
            {
                au.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                au.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


            }).AddJwtBearer(jwt =>
            {
                jwt.RequireHttpsMetadata = false;
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // active token expiration.
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true
                };

            }
            );
            services.AddAuthentication(IISDefaults.AuthenticationScheme).AddNegotiate();



            services.AddIdentity<User, Role>().AddEntityFrameworkStores<EngineCoreDBContext>().AddDefaultTokenProviders();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1.0", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Admin Panel API", Version = "v1.0" });
                options.DocInclusionPredicate((docName, description) => true);

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                // Add this filter as well.
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });



            services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.Configure<ChannelMailFirstSetting>(Configuration.GetSection("ChannelMailFirstSetting"));
            services.Configure<ChannelSMSSetting>(Configuration.GetSection("ChannelSMSSetting"));
            services.Configure<PaymentSettings>(Configuration.GetSection("PaymentSettings"));
            services.Configure<Pdfdocumentsetting>(Configuration.GetSection("Pdfdocumentsetting"));
            services.Configure<ApiEposUrl>(Configuration.GetSection("ApiEposUrl"));


            services.AddSoapCore();
            RegisterServices(services);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseCors(builder => builder
            //           .AllowAnyOrigin()
            //           .AllowAnyMethod()
            //           .AllowAnyHeader());
            app.UseRouting();
            app.UseCors();
            app.UseCors("ACTIVE_DIRECTORY");

            app.UseHsts();
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseMvc();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();


            app.UseSwagger();




            app.UseSwaggerUI(ui =>
            {
                ui.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Enotary System  API Endpoint");
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSoapEndpoint<INotaryFee>("/NotaryFee.asmx", new BasicHttpBinding(), SoapSerializer.XmlSerializer);
            app.UseSoapEndpoint<IMOADetails>("/MOADetails.asmx", new BasicHttpBinding(), SoapSerializer.XmlSerializer);
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IAramexRepository, AramexRepository>();
            services.AddScoped<IActionButtonRepository, ActionButtonRepository>();
            services.AddScoped<IApplicationRepository, ApplicationRepositiory>();
            services.AddScoped<IApplicationAttachmentRepository, ApplicationAttachmentRepositiory>();
            services.AddScoped<IApplicationPartyRepository, ApplicationPartyRepositiory>();
            services.AddScoped<IApplicationTrackRepository, ApplicationTrackRepository>();
            services.AddScoped<IAdmServiceRepository, AdmServiceRepository>();
            services.AddScoped<IAdmStageRepository, AdmStageRepository>();

            services.AddScoped<IBlackListRepository, BlackListRepository>();

            services.AddScoped<ICalendarRepository, CalendarRepository>();
            services.AddScoped<IChannelRepository, ChannelRepository>();
            services.AddScoped<ICronService, CronService>();
            services.AddScoped<IDocumentStorageRepository, DocumentStorageRepository>();

            services.AddScoped<IEPOSMachine, EPOSMachine>();
            services.AddScoped<IEmployeeSettingRepository, EmployeeSettingRepository>();

            services.AddScoped<IFileConfigurationRepository, FileConfigurationRepository>();
            services.AddScoped<IFileConfigRepository, FileConfigRepository>();
            services.AddScoped<IFilesUploaderRepositiory, FilesUploaderRepository>();
            services.AddScoped<IGeneralRepository, GeneralRepository>();
            services.AddScoped<IGenerator, GeneratorRepository>();
            services.AddScoped<IGlobalDayOffRepository, GlobalDayOffRepository>();

            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IMyApplicationRepository, MyApplicationRepository>();
            services.AddScoped<IMeetingRepository, MeetingRepository>();

            services.AddScoped<IMeetingRepository, MeetingRepository>();
            services.AddScoped<IMOADetails, MOADetails>();
            services.AddScoped<IMyApplicationRepository, MyApplicationRepository>();
            services.AddScoped<ICredentialRepository, CredentialRepository>();
            services.AddScoped<INotaryFee, NotaryFee>();
            services.AddScoped<INotificationLogRepository, NotificationLogRepository>();
            services.AddScoped<INotificationSettingRepository, NotificationSettingRepository>();
            services.AddScoped<IOCRRepository, OCRRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IQRrepository, QRGeneratorRepository>();
            services.AddScoped<IQueueRepository, QueueRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IServiceKindRepository, ServiceKindRepository>();
            services.AddScoped<ISendNotificationRepository, SendNotificationRepository>();
            services.AddScoped<IServiceFeeRepository, ServiceFeeRepository>();
            services.AddScoped<IStageMasterAttachmentRepository, StageMasterAttachmentRepository>();
            services.AddScoped<IStageActionsRepository, StageActionRepository>();
            services.AddScoped<IStatisticsRepository, StatisticsRepository>();
            services.AddScoped<ISysTranslationTableRepository, SysTranslationTableRepository>();
            services.AddScoped<ISysLookUpTypeRepository, SysLookUpTypeRepository>();
            services.AddScoped<ISysLookUpValueRepository, SysLookUpValueRepository>();
            services.AddScoped<ISysValueRepository, SysValueRepository>();
            services.AddScoped<ITabRepository, TabRepository>();
            services.AddScoped<ITermRepository, TermRepository>();
            services.AddScoped<ITemplateRepository, TemplateRepository>();
            services.AddScoped<ITemplatePartyRepository, TemplatePartyRepository>();
            services.AddScoped<ITemplateAttachmentRepository, TemplateAttachmentRepository>();
            services.AddScoped<ITransactionFeeRepository, TransactionFeeRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IUnifiedGateSubServicesRepository, UnifiedGateSubServices>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBlockChain, BlockChainRepository>();
            services.AddScoped<IWorkingTimeRepository, WorkingTimeRepository>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddTransient<Microsoft.AspNetCore.Authentication.IClaimsTransformation, ClaimsTransformer>();

        }
    }
}
