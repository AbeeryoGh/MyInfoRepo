using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EngineCoreProject.Models
{
    public partial class EngineCoreDBContext : IdentityDbContext<
        User, Role, int, UserClaim, UserRole, UserLogin,
        RoleClaim, IdentityUserToken<int>>

    {
        public EngineCoreDBContext()
        {
        }

        public EngineCoreDBContext(DbContextOptions<EngineCoreDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AdmAction> AdmAction { get; set; }
        public virtual DbSet<AdmService> AdmService { get; set; }
        public virtual DbSet<AdmServiceDocumentType> AdmServiceDocumentType { get; set; }
        public virtual DbSet<AdmStage> AdmStage { get; set; }
        public virtual DbSet<AdmStageAction> AdmStageAction { get; set; }
        public virtual DbSet<AppRelatedContent> AppRelatedContent { get; set; }
        public virtual DbSet<AppTransaction> AppTransaction { get; set; }
        public virtual DbSet<Application> Application { get; set; }
        public virtual DbSet<ApplicationAttachment> ApplicationAttachment { get; set; }
        public virtual DbSet<ApplicationObjection> ApplicationObjection { get; set; }
        public virtual DbSet<ApplicationObjectionAttachment> ApplicationObjectionAttachment { get; set; }
        public virtual DbSet<ApplicationParty> ApplicationParty { get; set; }
        public virtual DbSet<ApplicationPartyExtraAttachment> ApplicationPartyExtraAttachment { get; set; }
        public virtual DbSet<ApplicationTrack> ApplicationTrack { get; set; }
        public virtual DbSet<AramexRequests> AramexRequests { get; set; }
        public virtual DbSet<BlackListApplication> BlackListApplication { get; set; }
        public virtual DbSet<BlockChainPoa> BlockChainPoa { get; set; }
        public virtual DbSet<Calendar> Calendar { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<DocumentStorage> DocumentStorage { get; set; }
        public virtual DbSet<DocumentTypeKind> DocumentTypeKind { get; set; }
        public virtual DbSet<EmployeeSetting> EmployeeSetting { get; set; }
        public virtual DbSet<FileConfiguration> FileConfiguration { get; set; }
        public virtual DbSet<G2gRequests> G2gRequests { get; set; }
        public virtual DbSet<GlobalDayOff> GlobalDayOff { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<Meeting> Meeting { get; set; }
        public virtual DbSet<MeetingLogging> MeetingLogging { get; set; }
        public virtual DbSet<NotaryPlace> NotaryPlace { get; set; }
        public virtual DbSet<NotificationAction> NotificationAction { get; set; }
        public virtual DbSet<NotificationLog> NotificationLog { get; set; }
        public virtual DbSet<NotificationTemplate> NotificationTemplate { get; set; }
        public virtual DbSet<NotificationTemplateDetail> NotificationTemplateDetail { get; set; }
        public virtual DbSet<OcrdocumentFields> OcrdocumentFields { get; set; }
        public virtual DbSet<Ocrdocuments> Ocrdocuments { get; set; }
        public virtual DbSet<OtpLog> OtpLog { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<PaymentDetails> PaymentDetails { get; set; }
        public virtual DbSet<PaymentGateAttempt> PaymentGateAttempt { get; set; }
        public virtual DbSet<QueueProcesses> QueueProcesses { get; set; }
        public virtual DbSet<RelatedContent> RelatedContent { get; set; }
        public virtual DbSet<RelatedData> RelatedData { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RoleClaim> RoleClaim { get; set; }
        public virtual DbSet<ServiceFee> ServiceFee { get; set; }
        public virtual DbSet<ServiceKind> ServiceKind { get; set; }
        public virtual DbSet<ShortenUrl> ShortenUrl { get; set; }
        public virtual DbSet<StageMasterAttachment> StageMasterAttachment { get; set; }
        public virtual DbSet<SysExecution> SysExecution { get; set; }
        public virtual DbSet<SysLanguage> SysLanguage { get; set; }
        public virtual DbSet<SysLookupType> SysLookupType { get; set; }
        public virtual DbSet<SysLookupValue> SysLookupValue { get; set; }
        public virtual DbSet<SysTranslation> SysTranslation { get; set; }
        public virtual DbSet<Tab> Tab { get; set; }
        public virtual DbSet<TableName> TableName { get; set; }
        public virtual DbSet<TargetApplication> TargetApplication { get; set; }
        public virtual DbSet<TargetService> TargetService { get; set; }
        public virtual DbSet<Template> Template { get; set; }
        public virtual DbSet<TemplateAttachment> TemplateAttachment { get; set; }
        public virtual DbSet<TemplateParty> TemplateParty { get; set; }
        public virtual DbSet<Term> Term { get; set; }
        public virtual DbSet<TransactionFee> TransactionFee { get; set; }
        public virtual DbSet<TransactionOldVersion> TransactionOldVersion { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserClaim> UserClaim { get; set; }
        public virtual DbSet<UserLogger> UserLogger { get; set; }
        public virtual DbSet<UserLogin> UserLogin { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<UserToken> UserToken { get; set; }
        public virtual DbSet<View1> View1 { get; set; }
        public virtual DbSet<WorkingHours> WorkingHours { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=167.86.72.50,1433;Database=lilacnet;User ID=lilacnet;Password=sUeP98kkQkFDQ;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AdmAction>(entity =>
            {
                entity.ToTable("adm_action");

                entity.HasComment("جدول الأزرار (حفظ , ارسال ,اعتماد ..) يتم حفظ فيه جميع الازرار مع اسمائها ");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActionTypeId).HasColumnName("action_type_id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('active')");

                entity.Property(e => e.Shortcut)
                    .HasColumnName("shortcut")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.ActionType)
                    .WithMany(p => p.AdmAction)
                    .HasForeignKey(d => d.ActionTypeId)
                    .HasConstraintName("FK_adm_action_sys_lookup_value");
            });

            modelBuilder.Entity<AdmService>(entity =>
            {
                entity.ToTable("adm_service");

                entity.HasComment("جدول الخدمات ويحتوي على المعلومات الخاصة بكل خدمة (رقم الخدمة , اسم الخدمة , رقم الخدمة في خدماتي , ...)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApprovalStage).HasColumnName("approval_stage");

                entity.Property(e => e.ApprovalText).HasColumnName("approval_text");

                entity.Property(e => e.BlockTarget).HasColumnName("block_target");

                entity.Property(e => e.CancellationText).HasColumnName("cancellation_text");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DefaultUser).HasColumnName("default_user");

                entity.Property(e => e.Delivery)
                    .HasColumnName("delivery")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DocumentTypeId).HasColumnName("document_type_id");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ExternalFileRequired).HasColumnName("external_file_required");

                entity.Property(e => e.Fee).HasColumnName("fee");

                entity.Property(e => e.GuidFilePathAr)
                    .HasColumnName("guid_file_path_ar")
                    .HasMaxLength(250);

                entity.Property(e => e.GuidFilePathEn)
                    .HasColumnName("guid_file_path_en")
                    .HasMaxLength(250);

                entity.Property(e => e.HasDocument).HasColumnName("has_document");

                entity.Property(e => e.HasReason).HasColumnName("has_reason");

                entity.Property(e => e.Icon)
                    .HasColumnName("icon")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.KhadamatiServiceNo)
                    .HasColumnName("khadamatiServiceNo")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LimitedTime).HasColumnName("limited_time");

                entity.Property(e => e.Order).HasColumnName("order");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('active')");

                entity.Property(e => e.ServiceKindNo).HasColumnName("service_kind_no");

                entity.Property(e => e.ServiceResult)
                    .HasColumnName("service_result")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Shortcut)
                    .HasColumnName("shortcut")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TargetService).HasColumnName("target_service");

                entity.Property(e => e.TemplateId).HasColumnName("template_id");

                entity.Property(e => e.Templated).HasColumnName("templated");

                entity.Property(e => e.UgId).HasColumnName("ug_id");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.AdmServiceCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("adm_service_created_by_FK");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.AdmServiceLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("adm_service_updated_by_FK");

                entity.HasOne(d => d.ServiceKindNoNavigation)
                    .WithMany(p => p.AdmService)
                    .HasForeignKey(d => d.ServiceKindNo)
                    .HasConstraintName("FK_service_service_kind");
            });

            modelBuilder.Entity<AdmServiceDocumentType>(entity =>
            {
                entity.ToTable("adm_service_document_type");

                entity.HasComment("جدول يحتوي على الوثائق المرتبطة بكل خدمة");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DocumentTypeId).HasColumnName("document_type_id");

                entity.Property(e => e.ServiceId).HasColumnName("service_id");

                entity.HasOne(d => d.DocumentType)
                    .WithMany(p => p.AdmServiceDocumentType)
                    .HasForeignKey(d => d.DocumentTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("adm_service_document_type_document_type_id_FK");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.AdmServiceDocumentType)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("adm_service_document_type_service_id_FK");
            });


            modelBuilder.Entity<AdmStage>(entity =>
            {
                entity.ToTable("adm_stage");

                entity.HasComment("جدول يحتوي على جميع المراحل في كل خدمة (مسودة , مراجعة , مقابلة ,.. )");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Fee).HasColumnName("fee");

                entity.Property(e => e.Icon)
                    .HasColumnName("icon")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.KhadamatiServiceNo)
                    .HasColumnName("khadamatiServiceNo")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.OrderNo).HasColumnName("order_no");

                entity.Property(e => e.PeriodForArchive).HasColumnName("period_for_archive");

                entity.Property(e => e.PeriodForLate).HasColumnName("period_for_late");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('active')");

                entity.Property(e => e.ServiceId).HasColumnName("service_id");

                entity.Property(e => e.Shortcut)
                    .HasColumnName("shortcut")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StageTypeId).HasColumnName("stage_type_id");

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.AdmStage)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_adm_stage_adm_service");

                entity.HasOne(d => d.StageType)
                    .WithMany(p => p.AdmStage)
                    .HasForeignKey(d => d.StageTypeId)
                    .HasConstraintName("adm_stage_stage_type_id_FK");
            });

            modelBuilder.Entity<AdmStageAction>(entity =>
            {
                entity.ToTable("adm_stage_action");

                entity.HasComment("جدول يحتوي جميع الازرار المرتبطة بكل مرحلة والتي تظهر في صفحة المراحل عند الانتقال من مرحلة لأخرى");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActionId).HasColumnName("action_id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Enabled)
                    .HasColumnName("enabled")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Group)
                    .HasColumnName("group")
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('action_btn')");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('active')");

                entity.Property(e => e.ShowOrder).HasColumnName("show_order");

                entity.Property(e => e.StageId).HasColumnName("stage_id");

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.AdmStageAction)
                    .HasForeignKey(d => d.ActionId)
                    .HasConstraintName("FK_adm_stage_action_adm_action");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.AdmStageActionCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("stage_action_created_by_FK");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.AdmStageActionLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("stage_action_updated_by_FK");

                entity.HasOne(d => d.Stage)
                    .WithMany(p => p.AdmStageAction)
                    .HasForeignKey(d => d.StageId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_adm_stage_action_adm_stage");
            });

            modelBuilder.Entity<AppRelatedContent>(entity =>
            {
                entity.ToTable("app_related_content");

                entity.HasComment("جدول يحتوي على مكملات المعاملات من محاضر تصديق ومذكرات تبليغ ...");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AppId).HasColumnName("app_id");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content");

                entity.Property(e => e.ContentUrl)
                    .HasColumnName("content_url")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TitleShortcut)
                    .IsRequired()
                    .HasColumnName("title_shortcut")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.App)
                    .WithMany(p => p.AppRelatedContent)
                    .HasForeignKey(d => d.AppId)
                    .HasConstraintName("app_related_content_app_id_FK");
            });

            modelBuilder.Entity<AppTransaction>(entity =>
            {
                entity.ToTable("app_transaction");

                entity.HasComment("يحتوي على المعاملات النهائية وارقام المعاملات التي تم اعتمادها بشكل نهائي");

                entity.HasIndex(e => e.ApplicationId)
                    .HasName("app_transaction_app_id_UN")
                    .IsUnique();

                entity.HasIndex(e => e.TransactionNo)
                    .HasName("app_transaction_transaction_no_IDX")
                    .IsUnique()
                    .HasFilter("([transaction_no] IS NOT NULL)");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApplicationId).HasColumnName("application_id");

                entity.Property(e => e.Content)
                    .HasColumnName("content")
                    .HasColumnType("ntext");

                entity.Property(e => e.ContractValue).HasColumnName("contract_value");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DecisionText).HasColumnName("decision_text");

                entity.Property(e => e.DocumentUrl)
                    .HasColumnName("document_url")
                    .HasMaxLength(100);

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Exist).HasColumnName("exist");

                entity.Property(e => e.FileName)
                    .HasColumnName("file_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Qrcode)
                    .HasColumnName("qrcode")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('active')");

                entity.Property(e => e.RevocationReason)
                    .HasColumnName("revocationReason")
                    .HasMaxLength(200);

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(150);

                entity.Property(e => e.TransactionCreatedDate)
                    .HasColumnName("transaction_created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.TransactionEndDate)
                    .HasColumnName("transaction_end_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.TransactionNo)
                    .HasColumnName("transaction_no")
                    .HasMaxLength(100);

                entity.Property(e => e.TransactionStartDate)
                    .HasColumnName("transaction_start_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.TransactionStatus).HasColumnName("transaction_status");

                entity.Property(e => e.UnlimitedValidity).HasColumnName("unlimited_validity");

                entity.Property(e => e.Vcid).HasColumnName("VCID");

                entity.HasOne(d => d.Application)
                    .WithOne(p => p.AppTransaction)
                    .HasForeignKey<AppTransaction>(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("transaction_application_id__FK");
            });

            modelBuilder.Entity<Application>(entity =>
            {
                entity.ToTable("application");

                entity.HasComment("جدول التطبيقات يحتوي جميع الطلبات المقدمة من المتعاملين وكل طلب لأي خدمة يتبع وماهي حالته وبأي مرحلة موجود حاليا");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AppLastUpdateDate)
                    .HasColumnName("app_last_update_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ApplicationDate)
                    .HasColumnName("application_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ApplicationNo)
                    .HasColumnName("application_no")
                    .HasMaxLength(100);

                entity.Property(e => e.Channel).HasColumnName("channel");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CurrentStageId).HasColumnName("current_stage_id");

                entity.Property(e => e.Delivery)
                    .HasColumnName("delivery")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastReadDate)
                    .HasColumnName("last_read_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastReader).HasColumnName("last_reader");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Locked)
                    .HasColumnName("locked")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(255);

                entity.Property(e => e.OldId).HasColumnName("old_id");

                entity.Property(e => e.OldTemplateId).HasColumnName("old_template_id");

                entity.Property(e => e.Owner).HasColumnName("owner");

                entity.Property(e => e.Reason).HasColumnName("reason");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('active')");

                entity.Property(e => e.RowVersion)
                    .HasColumnName("row_version")
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.ServiceId).HasColumnName("service_id");

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.StateId).HasColumnName("state_id");

                entity.Property(e => e.TemplateId).HasColumnName("template_id");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ApplicationCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("application_created_by_FK");

                entity.HasOne(d => d.CurrentStage)
                    .WithMany(p => p.Application)
                    .HasForeignKey(d => d.CurrentStageId)
                    .HasConstraintName("application_current_stage_id_FK");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.ApplicationLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("application_updated_by_FK");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Application)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("applications_service_FK");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.Application)
                    .HasForeignKey(d => d.TemplateId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("application_template_id_FK");
            });

            modelBuilder.Entity<ApplicationAttachment>(entity =>
            {
                entity.ToTable("application_attachment");

                entity.HasComment("جدول يحتوي مرفقات الطلبات ");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApplicationId).HasColumnName("application_id");

                entity.Property(e => e.AttachmentId).HasColumnName("attachment_id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.FileName)
                    .HasColumnName("file_name")
                    .HasMaxLength(200);

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.MimeType)
                    .HasColumnName("mime_type")
                    .HasMaxLength(200);

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(200);

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('active')");

                entity.Property(e => e.Size).HasColumnName("size_");

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.ApplicationAttachment)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("application_attachments_application_FK");

                entity.HasOne(d => d.Attachment)
                    .WithMany(p => p.ApplicationAttachment)
                    .HasForeignKey(d => d.AttachmentId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("application_attachment_lookup_value_FK");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ApplicationAttachmentCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("application_attachment_created_by_FK");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.ApplicationAttachmentLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("application_attachment_updated_by_FK");
            });

            modelBuilder.Entity<ApplicationObjection>(entity =>
            {
                entity.ToTable("application_objection");

                entity.HasComment("جدل يحتوي على الاعتراضات المقدمة من الاطراف على الطلبات في مرحلتي الدفع والمنجزة");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(200);

                entity.Property(e => e.ApplicationId).HasColumnName("application_id");

                entity.Property(e => e.Birthday)
                    .HasColumnName("birthday")
                    .HasColumnType("date");

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(75);

                entity.Property(e => e.EmaraId).HasColumnName("emara_id");

                entity.Property(e => e.EmiratesId)
                    .HasColumnName("emirates_id")
                    .HasMaxLength(50);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("full_name")
                    .HasMaxLength(100);

                entity.Property(e => e.Gender)
                    .HasColumnName("gender")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Nationality)
                    .HasColumnName("nationality");

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(300);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(50);

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasColumnName("reason")
                    .HasMaxLength(300);
            });

            modelBuilder.Entity<ApplicationObjectionAttachment>(entity =>
            {
                entity.ToTable("application_objection_attachment");

                entity.HasComment("جدول يحتوي على مرفقات الاعتراضات في حال وجدت");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Attachment)
                    .IsRequired()
                    .HasColumnName("attachment")
                    .HasMaxLength(150);

                entity.Property(e => e.ObjectionId).HasColumnName("objection_id");

                entity.HasOne(d => d.Objection)
                    .WithMany(p => p.ApplicationObjectionAttachment)
                    .HasForeignKey(d => d.ObjectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_application_objection_attachment_application_objection_id");
            });

            modelBuilder.Entity<ApplicationParty>(entity =>
            {
                entity.ToTable("application_party");

                entity.HasComment("جدول الاطراف في المعاملات وبياناتهم الشخصية من الاسم ورقم الهاتف والعنوان والبريد الالكتروني ");

                entity.HasIndex(e => new { e.PartyId, e.TransactionId })
                    .HasName("party_id_transaction_id_UN")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(255);

                entity.Property(e => e.AlternativeEmail)
                    .HasColumnName("alternative_email")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BirthDate)
                    .HasColumnName("birth_date")
                    .HasColumnType("date");

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(100);

                entity.Property(e => e.EditableSign)
                    .HasColumnName("editable_sign")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Emirate).HasColumnName("emirate");

                entity.Property(e => e.EmiratesIdNo)
                    .HasColumnName("emirates_id_no")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnName("full_name")
                    .HasMaxLength(100);

                entity.Property(e => e.Gender).HasColumnName("gender");

                entity.Property(e => e.IdAttachment)
                    .HasColumnName("id_attachment")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.IdExpirationDate)
                    .HasColumnName("id_expiration_date")
                    .HasColumnType("date");

                entity.Property(e => e.IsOwner).HasColumnName("is_owner");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.MaritalStatus).HasColumnName("marital_status");

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Nationality).HasColumnName("nationality");

                entity.Property(e => e.NotaryId).HasColumnName("notary_id");

                entity.Property(e => e.PartyId).HasColumnName("party_id");

                entity.Property(e => e.PartyTypeValueId).HasColumnName("party_type_value_id");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('active')");

                entity.Property(e => e.SignDate)
                    .HasColumnName("sign_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.SignRequired).HasColumnName("sign_required");

                entity.Property(e => e.SignType).HasColumnName("sign_type");

                entity.Property(e => e.SignUrl)
                    .HasColumnName("sign_url")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Signed).HasColumnName("signed");

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

                entity.Property(e => e.UnifiedNumber)
                    .HasColumnName("unified_number")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Party)
                    .WithMany(p => p.ApplicationParty)
                    .HasForeignKey(d => d.PartyId)
                    .HasConstraintName("application_party_user_id_FK");

                entity.HasOne(d => d.PartyTypeValue)
                    .WithMany(p => p.ApplicationParty)
                    .HasForeignKey(d => d.PartyTypeValueId)
                    .HasConstraintName("application_party_lookup_value_id_FK");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.ApplicationParty)
                    .HasForeignKey(d => d.TransactionId)
                    .HasConstraintName("application_party_transaction_id_FK");
            });

            modelBuilder.Entity<ApplicationPartyExtraAttachment>(entity =>
            {
                entity.ToTable("application_party_extra_attachment");

                entity.HasComment("جدول المرفقات الاضافية للطرف مثل جواز السفر ورخصة قيادة .");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApplicationPartyId).HasColumnName("application_party_id");

                entity.Property(e => e.AttachmentId).HasColumnName("attachment_id");

                entity.Property(e => e.AttachmentName)
                    .HasColumnName("attachment_name")
                    .HasMaxLength(100);

                entity.Property(e => e.AttachmentUrl)
                    .HasColumnName("attachment_url")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CountryOfIssue).HasColumnName("country_of_Issue");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ExpirationDate)
                    .HasColumnName("expiration_date")
                    .HasColumnType("date");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Number)
                    .HasColumnName("number")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('active')");

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.ApplicationParty)
                    .WithMany(p => p.ApplicationPartyExtraAttachment)
                    .HasForeignKey(d => d.ApplicationPartyId)
                    .HasConstraintName("party_extra_attachment_application_party_FK");

                entity.HasOne(d => d.Attachment)
                    .WithMany(p => p.ApplicationPartyExtraAttachment)
                    .HasForeignKey(d => d.AttachmentId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("application_party_extra_attachment_attachment_id_FK");
            });

            modelBuilder.Entity<ApplicationTrack>(entity =>
            {
                entity.ToTable("application_track");

                entity.HasComment("جدول يحتوي على مسار المعاملة من مرحلة الى مرحلة مع الملاحظات المسجلة في كل مرحلة");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApplicationId).HasColumnName("application_id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.NextStageId).HasColumnName("next_stage_id");

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(500);

                entity.Property(e => e.NoteKind)
                    .HasColumnName("note_kind")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('active')");

                entity.Property(e => e.StageId).HasColumnName("stage_id");

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.ApplicationTrack)
                    .HasForeignKey(d => d.ApplicationId)
                    .HasConstraintName("application_track_application_id_FK");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.ApplicationTrack)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("application_track_location_id_FK");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ApplicationTrack)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("application_track_user_id_FK");
            });

            modelBuilder.Entity<AramexRequests>(entity =>
            {
                entity.ToTable("aramex_requests");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApplicationId).HasColumnName("application_id");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(200);

                entity.Property(e => e.OwnerName)
                    .HasColumnName("owner_name")
                    .HasMaxLength(100);

                entity.Property(e => e.StateId).HasColumnName("state_id");

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.AramexRequests)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_aramex_requests_aramex_requests_applicaton_id");
            });

            modelBuilder.Entity<BlackListApplication>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("black_list_application");

                entity.Property(e => e.ApplicationId).HasColumnName("application_id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Application)
                    .WithMany()
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_black_list_application_application");
            });

            modelBuilder.Entity<BlockChainPoa>(entity =>
            {
                entity.ToTable("block_chain_POA");

                entity.HasComment("جدول خاص بال المحفظة الاللكترونية وال UAEPass");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AppId).HasColumnName("app_id");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsSentUg).HasColumnName("is_sent_UG");

                entity.Property(e => e.IsSysCancelled).HasColumnName("is_sys_cancelled");

                entity.Property(e => e.IsUgCancelled).HasColumnName("is_UG_cancelled");

                entity.Property(e => e.Vcid)
                    .HasColumnName("VCID")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Calendar>(entity =>
            {
                entity.ToTable("calendar");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(1000);

                entity.Property(e => e.EndDate)
                    .HasColumnName("end_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.MeetingId).HasColumnName("meeting_id");

                entity.Property(e => e.NotifyMe).HasColumnName("notify_me");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('active')");

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(50);

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Meeting)
                    .WithMany(p => p.Calendar)
                    .HasForeignKey(d => d.MeetingId)
                    .HasConstraintName("FK_calendar_meeting_id_meeting_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Calendar)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_calendar_calendar_user_id");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.CntId)
                    .HasName("country_PK");

                entity.ToTable("country");

                entity.Property(e => e.CntId).HasColumnName("CNT_ID");

                entity.Property(e => e.CntCapitalAr)
                    .HasColumnName("CNT_Capital_AR")
                    .HasMaxLength(64);

                entity.Property(e => e.CntCapitalEn)
                    .HasColumnName("CNT_Capital_EN")
                    .HasMaxLength(64);

                entity.Property(e => e.CntConIdFk).HasColumnName("CNT_CON_ID_FK");

                entity.Property(e => e.CntContinentAr)
                    .HasColumnName("CNT_Continent_AR")
                    .HasMaxLength(64);

                entity.Property(e => e.CntContinentEn)
                    .HasColumnName("CNT_Continent_EN")
                    .HasMaxLength(64);

                entity.Property(e => e.CntCountryAr)
                    .HasColumnName("CNT_Country_AR")
                    .HasMaxLength(64);

                entity.Property(e => e.CntCountryEn)
                    .HasColumnName("CNT_Country_EN")
                    .HasMaxLength(64);

                entity.Property(e => e.CntGlobalCode)
                    .HasColumnName("CNT_GLOBAL_CODE")
                    .HasMaxLength(16);

                entity.Property(e => e.CntIso2)
                    .HasColumnName("CNT_ISO2")
                    .HasMaxLength(64);

                entity.Property(e => e.CntIso3)
                    .HasColumnName("CNT_ISO3")
                    .HasMaxLength(64);

                entity.Property(e => e.CntOfficialNameAr)
                    .HasColumnName("CNT_Official_name_AR")
                    .HasMaxLength(64);

                entity.Property(e => e.CntOfficialNameEn)
                    .HasColumnName("CNT_Official_name_EN")
                    .HasMaxLength(64);

                entity.Property(e => e.CntRegIdFk).HasColumnName("CNT_REG_ID_FK");

                entity.Property(e => e.CntRegionAr)
                    .HasColumnName("CNT_Region_AR")
                    .HasMaxLength(64);

                entity.Property(e => e.CntRegionEn)
                    .HasColumnName("CNT_Region_EN")
                    .HasMaxLength(64);

                entity.Property(e => e.UgId).HasColumnName("UG_ID");
            });

            modelBuilder.Entity<DocumentStorage>(entity =>
            {
                entity.ToTable("document_storage");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasColumnName("file_name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasColumnName("file_path")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.DocumentStorage)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_document_storage_user");
            });

            modelBuilder.Entity<DocumentTypeKind>(entity =>
            {
                entity.ToTable("document_type_kind");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DocumentTypeId).HasColumnName("document_type_id");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.HasOne(d => d.DocumentType)
                    .WithMany(p => p.DocumentTypeKind)
                    .HasForeignKey(d => d.DocumentTypeId)
                    .HasConstraintName("document_type_kind_lookup_value_FK");
            });

            modelBuilder.Entity<EmployeeSetting>(entity =>
            {
                entity.ToTable("employee_setting");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActiveDirectoryAccount)
                    .HasColumnName("active_directory_account")
                    .HasMaxLength(100);

                entity.Property(e => e.Channel)
                    .HasColumnName("channel")
                    .HasMaxLength(50);

                entity.Property(e => e.DefaultMuteVoice).HasColumnName("default_mute_voice");

                entity.Property(e => e.DefaultShowCam).HasColumnName("default_show_cam");

                entity.Property(e => e.DefaultViewCards).HasColumnName("default_view_cards");

                entity.Property(e => e.EnotaryId).HasColumnName("enotary_id");

                entity.Property(e => e.EntityCode)
                    .HasColumnName("entity_code")
                    .HasMaxLength(50);

                entity.Property(e => e.ExpiredSessionToken)
                    .HasColumnName("expired_session_token")
                    .HasColumnType("datetime");

                entity.Property(e => e.LocationCode)
                    .HasColumnName("location_code")
                    .HasMaxLength(50);

                entity.Property(e => e.NotaryLocationId).HasColumnName("notary_location_id");

                entity.Property(e => e.SessionToken)
                    .HasColumnName("session_token")
                    .HasMaxLength(50);

                entity.Property(e => e.SourceReference)
                    .HasColumnName("sourceReference")
                    .HasMaxLength(50);

                entity.Property(e => e.TerminalId)
                    .HasColumnName("terminal_id")
                    .HasMaxLength(50);

                entity.Property(e => e.Tokens)
                    .HasColumnName("tokens")
                    .HasMaxLength(50);

                entity.Property(e => e.TransactionReference)
                    .HasColumnName("transactionReference")
                    .HasMaxLength(50);

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(50);

                entity.HasOne(d => d.Enotary)
                    .WithMany(p => p.EmployeeSetting)
                    .HasForeignKey(d => d.EnotaryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_employee_setting_user");

                entity.HasOne(d => d.NotaryLocation)
                    .WithMany(p => p.EmployeeSetting)
                    .HasForeignKey(d => d.NotaryLocationId)
                    .HasConstraintName("FK_employee_setting_location");
            });

            modelBuilder.Entity<FileConfiguration>(entity =>
            {
                entity.ToTable("file_configuration");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Extension)
                    .HasColumnName("extension")
                    .HasMaxLength(50);

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.MaxSize).HasColumnName("max_size");

                entity.Property(e => e.MinSize).HasColumnName("min_size");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(50);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.FileConfigurationCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("file_configuration_created_by _FK");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.FileConfigurationLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("file_configuration_updated_by_FK");
            });

            modelBuilder.Entity<G2gRequests>(entity =>
            {
                entity.ToTable("g2g_requests");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApiName)
                    .IsRequired()
                    .HasColumnName("api_name")
                    .HasMaxLength(50);

                entity.Property(e => e.ReqDate)
                    .HasColumnName("reqDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Reqest)
                    .IsRequired()
                    .HasColumnName("reqest");

                entity.Property(e => e.Response)
                    .IsRequired()
                    .HasColumnName("response");
            });

            modelBuilder.Entity<GlobalDayOff>(entity =>
            {
                entity.ToTable("global_day_off");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndDate)
                    .HasColumnName("end_date")
                    .HasColumnType("date");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ReasonShortcut)
                    .HasColumnName("reason_shortcut")
                    .HasMaxLength(100);

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date")
                    .HasColumnType("date");

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("location");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.NameShortcut)
                    .IsRequired()
                    .HasColumnName("name_shortcut")
                    .HasMaxLength(250);

                entity.Property(e => e.ParentLocationId).HasColumnName("parent_location_id");

                entity.HasOne(d => d.ParentLocation)
                    .WithMany(p => p.InverseParentLocation)
                    .HasForeignKey(d => d.ParentLocationId)
                    .HasConstraintName("FK_location_location");
            });

            modelBuilder.Entity<Meeting>(entity =>
            {
                entity.ToTable("meeting");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.EndDate)
                    .HasColumnName("end_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.MeetingId)
                    .IsRequired()
                    .HasColumnName("meeting_id")
                    .HasMaxLength(20);

                entity.Property(e => e.MeetingLog).HasColumnName("meeting_log");

                entity.Property(e => e.OrderNo)
                    .HasColumnName("order_no")
                    .HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(50);

                entity.Property(e => e.PasswordReq).HasColumnName("password_req");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TimeZone)
                    .IsRequired()
                    .HasColumnName("time_zone")
                    .HasMaxLength(50);

                entity.Property(e => e.Topic)
                    .IsRequired()
                    .HasColumnName("topic")
                    .HasMaxLength(200);

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Meeting)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_meeting_user_id_users_user_id");
            });

            modelBuilder.Entity<MeetingLogging>(entity =>
            {
                entity.ToTable("meeting_logging");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FirstLogin)
                    .HasColumnName("first_login")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsModerator).HasColumnName("is_moderator");

                entity.Property(e => e.LoginDate)
                    .HasColumnName("login_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.MeetingId).HasColumnName("meeting_id");

                entity.Property(e => e.PreviousLoginList)
                    .HasColumnName("previous_login_list")
                    .HasMaxLength(1000);

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Meeting)
                    .WithMany(p => p.MeetingLogging)
                    .HasForeignKey(d => d.MeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_meeting_logging_meeting");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MeetingLogging)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_meeting_logging_user");
            });

            modelBuilder.Entity<NotaryPlace>(entity =>
            {
                entity.ToTable("notary_place");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EmirateValueId).HasColumnName("emirate_value_id");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.EmirateValue)
                    .WithMany(p => p.NotaryPlace)
                    .HasForeignKey(d => d.EmirateValueId)
                    .HasConstraintName("notary_place_emirate_value_FK");
            });

            modelBuilder.Entity<NotificationAction>(entity =>
            {
                entity.ToTable("notification_action");

                entity.HasIndex(e => new { e.ActionId, e.NotificationTemplateId })
                    .HasName("uq_action_id_notification_template_id")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActionId).HasColumnName("action_id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.NotificationTemplateId).HasColumnName("notification_template_id");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('active')");

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.NotificationAction)
                    .HasForeignKey(d => d.ActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_notification_action_adm_action");

                entity.HasOne(d => d.NotificationTemplate)
                    .WithMany(p => p.NotificationAction)
                    .HasForeignKey(d => d.NotificationTemplateId)
                    .HasConstraintName("notification_action_notification_template_FK");
            });

            modelBuilder.Entity<NotificationLog>(entity =>
            {
                entity.ToTable("notification_log");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApplicationId).HasColumnName("application_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Hostsetting).HasColumnName("hostsetting");

                entity.Property(e => e.IsSent).HasColumnName("is_sent");

                entity.Property(e => e.Lang)
                    .HasColumnName("lang")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.NotificationBody)
                    .IsRequired()
                    .HasColumnName("notification_body");

                entity.Property(e => e.NotificationChannelId).HasColumnName("notification_channel_id");

                entity.Property(e => e.NotificationTitle)
                    .IsRequired()
                    .HasColumnName("notification_title");

                entity.Property(e => e.ReportValueId).HasColumnName("report_value_id");

                entity.Property(e => e.SendReportId).HasColumnName("send_report_id");

                entity.Property(e => e.SentCount).HasColumnName("sent_count");

                entity.Property(e => e.ToAddress).HasColumnName("to_address");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<NotificationTemplate>(entity =>
            {
                entity.ToTable("notification_template");

                entity.HasIndex(e => e.NotificationNameShortcut)
                    .HasName("UQ__nameshortcut")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.NotificationNameShortcut)
                    .HasColumnName("notification_name_shortcut")
                    .HasMaxLength(100);

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('active')");

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<NotificationTemplateDetail>(entity =>
            {
                entity.ToTable("notification_template_detail");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BodyShortcut)
                    .HasColumnName("body_shortcut")
                    .HasMaxLength(250);

                entity.Property(e => e.ChangeAble).HasColumnName("change_able");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.NotificationChannelId).HasColumnName("notification_channel_id");

                entity.Property(e => e.NotificationTemplateId).HasColumnName("notification_template_id");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('active')");

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TitleShortcut)
                    .HasColumnName("title_shortcut")
                    .HasMaxLength(250);

                entity.HasOne(d => d.NotificationChannel)
                    .WithMany(p => p.NotificationTemplateDetail)
                    .HasForeignKey(d => d.NotificationChannelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_notification_template_channel_id_lookup_value");

                entity.HasOne(d => d.NotificationTemplate)
                    .WithMany(p => p.NotificationTemplateDetail)
                    .HasForeignKey(d => d.NotificationTemplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_notification_template_detail_notification_template_id");
            });

            modelBuilder.Entity<OcrdocumentFields>(entity =>
            {
                entity.ToTable("OCRDocumentFields");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("createdAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.DocumentId).HasColumnName("documentId");

                entity.Property(e => e.FieldClass)
                    .HasColumnName("fieldClass")
                    .IsUnicode(false);

                entity.Property(e => e.Score).HasColumnName("score");

                entity.Property(e => e.Text).HasColumnName("text");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updatedAt")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.OcrdocumentFields)
                    .HasForeignKey(d => d.DocumentId)
                    .HasConstraintName("FK__OCRDocume__docum__752F0E57");
            });

            modelBuilder.Entity<Ocrdocuments>(entity =>
            {
                entity.ToTable("OCRDocuments");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BboxImg)
                    .HasColumnName("bboxImg")
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("createdAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.DocumentName)
                    .HasColumnName("documentName")
                    .IsUnicode(false);

                entity.Property(e => e.DocumentType)
                    .HasColumnName("documentType")
                    .IsUnicode(false);

                entity.Property(e => e.Score).HasColumnName("score");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updatedAt")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<OtpLog>(entity =>
            {
                entity.ToTable("otp_log");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GeneratedDate)
                    .HasColumnName("generated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.OtpCode)
                    .HasColumnName("otp_code")
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.OtpLog)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_otp_user_id");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActionId).HasColumnName("action_id");

                entity.Property(e => e.ActualPaid).HasColumnName("actual_paid");

                entity.Property(e => e.ApplicationId).HasColumnName("application_id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.InvoiceNo)
                    .IsRequired()
                    .HasColumnName("invoice_no")
                    .HasMaxLength(200);

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.PaymentDate)
                    .HasColumnName("payment_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.PaymentMethodType)
                    .HasColumnName("payment_method_type")
                    .HasMaxLength(200);

                entity.Property(e => e.PaymentSource)
                    .HasColumnName("payment_source")
                    .HasMaxLength(200);

                entity.Property(e => e.PaymentStatus)
                    .HasColumnName("payment_status")
                    .HasMaxLength(200);

                entity.Property(e => e.PaymentType)
                    .HasColumnName("payment_type")
                    .HasMaxLength(200);

                entity.Property(e => e.Printed).HasColumnName("printed");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('active')");

                entity.Property(e => e.ReceiptNo)
                    .HasColumnName("receipt_no")
                    .HasMaxLength(100);

                entity.Property(e => e.ServiceId).HasColumnName("service_id");

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasMaxLength(200);

                entity.Property(e => e.StatusMessage)
                    .HasColumnName("status_message")
                    .HasMaxLength(200);

                entity.Property(e => e.TotalAmount).HasColumnName("total_amount");

                entity.Property(e => e.TransactionResponseDate)
                    .HasColumnName("transaction_response_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Payment)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("payments_service_FK");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Payment)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("payments_user_FK");
            });

            modelBuilder.Entity<PaymentDetails>(entity =>
            {
                entity.ToTable("payment_details");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AmountWithFees).HasColumnName("amountWithFees");

                entity.Property(e => e.AmountWithoutFees).HasColumnName("amountWithoutFees");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.OwnerFees).HasColumnName("ownerFees");

                entity.Property(e => e.PaymentId).HasColumnName("payment_id");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('active')");

                entity.Property(e => e.ServiceMainCode)
                    .HasColumnName("service_main_code")
                    .HasMaxLength(100);

                entity.Property(e => e.ServiceSubCode)
                    .HasColumnName("service_sub_code")
                    .HasMaxLength(100);

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.PaymentDetails)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("FK_payment_details_payment");
            });

            modelBuilder.Entity<PaymentGateAttempt>(entity =>
            {
                entity.ToTable("payment_gate_attempt");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CollectionCenterFee)
                    .HasColumnName("collection_center_fee")
                    .HasMaxLength(200);

                entity.Property(e => e.ConfirmationId)
                    .HasColumnName("confirmation_id")
                    .HasMaxLength(200);

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.EDirhamFee)
                    .HasColumnName("e_dirham_fee")
                    .HasMaxLength(200);

                entity.Property(e => e.EServiceData).HasColumnName("e_service_data");

                entity.Property(e => e.PaidAttemptDate)
                    .HasColumnName("paid_attempt_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.PaymentAttemptInvoiceNo)
                    .IsRequired()
                    .HasColumnName("payment_attempt_invoice_no")
                    .HasMaxLength(50);

                entity.Property(e => e.PaymentId).HasColumnName("payment_id");

                entity.Property(e => e.Pun)
                    .HasColumnName("pun")
                    .HasMaxLength(200);

                entity.Property(e => e.SecureHash)
                    .HasColumnName("secure_hash")
                    .HasMaxLength(200);

                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("updated_date")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.PaymentGateAttempt)
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_payment_gate_attempt_payment_id");
            });

            modelBuilder.Entity<QueueProcesses>(entity =>
            {
                entity.ToTable("queue_processes");

                entity.HasIndex(e => e.ProcessNo)
                    .HasName("unique_process_no")
                    .IsUnique();

                entity.HasIndex(e => new { e.ProcessNo, e.ServiceKindNo })
                    .HasName("uq_queue_processes_process_no_service_kind_no")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ExpectedDateTime)
                    .HasColumnName("expected_date_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(200);

                entity.Property(e => e.NotifyHighLevel).HasColumnName("notify_high_level");

                entity.Property(e => e.NotifyLowLevel).HasColumnName("notify_low_level");

                entity.Property(e => e.NotifyMediumLevel).HasColumnName("notify_medium_level");

                entity.Property(e => e.ProcessNo)
                    .IsRequired()
                    .HasColumnName("process_no")
                    .HasMaxLength(50);

                entity.Property(e => e.Provider)
                    .HasColumnName("provider")
                    .HasMaxLength(50);

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ServiceKindNo).HasColumnName("service_kind_no");

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TicketId).HasColumnName("ticket_id");

                entity.HasOne(d => d.ServiceKindNoNavigation)
                    .WithMany(p => p.QueueProcesses)
                    .HasForeignKey(d => d.ServiceKindNo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_queue_processes_service_no");
            });

            modelBuilder.Entity<RelatedContent>(entity =>
            {
                entity.ToTable("related_content");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content");

                entity.Property(e => e.IsOutput).HasColumnName("is_output");

                entity.Property(e => e.ServiceId).HasColumnName("service_id");

                entity.Property(e => e.StageId).HasColumnName("stage_id");

                entity.Property(e => e.TemplateId).HasColumnName("template_id");

                entity.Property(e => e.TitleShortcut)
                    .IsRequired()
                    .HasColumnName("title_shortcut")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.RelatedContent)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("related_content_service_id_FK");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.RelatedContent)
                    .HasForeignKey(d => d.TemplateId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("related_content_template_id_FK");
            });

            modelBuilder.Entity<RelatedData>(entity =>
            {
                entity.ToTable("related_data");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ServiceId).HasColumnName("service_id");

                entity.Property(e => e.ShowApplication).HasColumnName("show_application");

                entity.Property(e => e.ShowTransaction).HasColumnName("show_transaction");

                entity.Property(e => e.TemplateId).HasColumnName("template_id");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.RelatedData)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("related_data_service_id_FK");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.RelatedData)
                    .HasForeignKey(d => d.TemplateId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("related_data_template_id_FK");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<RoleClaim>(entity =>
            {
                entity.ToTable("role-claim");

                entity.HasIndex(e => new { e.ClaimType, e.ClaimValue, e.RoleId })
                    .HasName("uq_role_type_value_roleId")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClaimType)
                    .IsRequired()
                    .HasColumnName("claim-type")
                    .HasMaxLength(200);

                entity.Property(e => e.ClaimValue)
                    .IsRequired()
                    .HasColumnName("claim-value")
                    .HasMaxLength(200);

                entity.Property(e => e.RoleId).HasColumnName("role-id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleClaim)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_role-claim_role-id");
            });

            modelBuilder.Entity<ServiceFee>(entity =>
            {
                entity.ToTable("service_fee");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DocumentKind).HasColumnName("document_kind");

                entity.Property(e => e.FeeNo).HasColumnName("fee_no");

                entity.Property(e => e.ProcessKind).HasColumnName("process_kind");

                entity.Property(e => e.Required).HasColumnName("required");

                entity.Property(e => e.ServiceNo).HasColumnName("service_no");

                entity.HasOne(d => d.FeeNoNavigation)
                    .WithMany(p => p.ServiceFee)
                    .HasForeignKey(d => d.FeeNo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_service_fee_service_fee_fee_id");

                entity.HasOne(d => d.ServiceNoNavigation)
                    .WithMany(p => p.ServiceFee)
                    .HasForeignKey(d => d.ServiceNo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_service_fee_service_no_service_id");
            });

            modelBuilder.Entity<ServiceKind>(entity =>
            {
                entity.ToTable("service_kind");

                entity.HasIndex(e => e.Symbol)
                    .HasName("UC_symbol")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ApplicationsPerNotary).HasColumnName("applications_per_notary");

                entity.Property(e => e.EmployeeCount).HasColumnName("employee_count");

                entity.Property(e => e.EstimatedTimePerProcess).HasColumnName("estimated_time_per_process");

                entity.Property(e => e.ServiceKindNameShortcut)
                    .IsRequired()
                    .HasColumnName("service_kind_name_shortcut")
                    .HasMaxLength(100);

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasColumnName("symbol")
                    .HasMaxLength(5);
            });

            modelBuilder.Entity<ShortenUrl>(entity =>
            {
                entity.HasKey(e => e.GuidUrl);

                entity.ToTable("shorten_url");

                entity.Property(e => e.GuidUrl)
                    .HasColumnName("guid_url")
                    .ValueGeneratedNever();

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("url");
            });

            modelBuilder.Entity<StageMasterAttachment>(entity =>
            {
                entity.ToTable("stage_master_attachment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.MasterAttachmentId).HasColumnName("master_attachment_id");

                entity.Property(e => e.Required).HasColumnName("required");

                entity.Property(e => e.StageId).HasColumnName("stage_id");

                entity.Property(e => e.TemplateId).HasColumnName("template_id");

                entity.HasOne(d => d.MasterAttachment)
                    .WithMany(p => p.StageMasterAttachment)
                    .HasForeignKey(d => d.MasterAttachmentId)
                    .HasConstraintName("FK_stage_master_attachment_sys_lookup_value");

                entity.HasOne(d => d.Stage)
                    .WithMany(p => p.StageMasterAttachment)
                    .HasForeignKey(d => d.StageId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_stage_master_attachment_adm_stage");
            });

            modelBuilder.Entity<SysExecution>(entity =>
            {
                entity.ToTable("sys_execution");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ActionId).HasColumnName("action_id");

                entity.Property(e => e.ExecutionOrder).HasColumnName("execution_order");

                entity.Property(e => e.Method)
                    .HasColumnName("method")
                    .HasMaxLength(5);

                entity.Property(e => e.Parameter1)
                    .HasColumnName("parameter1")
                    .HasMaxLength(500);

                entity.Property(e => e.Parameter2)
                    .HasColumnName("parameter2")
                    .HasMaxLength(200);

                entity.Property(e => e.ToExecute)
                    .IsRequired()
                    .HasColumnName("to_execute")
                    .HasMaxLength(200);

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.SysExecution)
                    .HasForeignKey(d => d.ActionId)
                    .HasConstraintName("sys_execution_action_id_FK");
            });

            modelBuilder.Entity<SysLanguage>(entity =>
            {
                entity.ToTable("sys_language");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Lang)
                    .IsRequired()
                    .HasColumnName("lang")
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SysLookupType>(entity =>
            {
                entity.ToTable("sys_lookup_type");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<SysLookupValue>(entity =>
            {
                entity.ToTable("sys_lookup_value");

                entity.HasIndex(e => e.Shortcut)
                    .HasName("sys_lookup_value_UN")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BoolParameter).HasColumnName("bool_parameter");

                entity.Property(e => e.LookupTypeId).HasColumnName("lookup_type_id");

                entity.Property(e => e.Order).HasColumnName("order");

                entity.Property(e => e.Shortcut)
                    .HasColumnName("shortcut")
                    .HasMaxLength(50);

                entity.HasOne(d => d.LookupType)
                    .WithMany(p => p.SysLookupValue)
                    .HasForeignKey(d => d.LookupTypeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_LookupValue_LookupType");
            });

            modelBuilder.Entity<SysTranslation>(entity =>
            {
                entity.ToTable("sys_translation");

                entity.HasIndex(e => new { e.Shortcut, e.Lang })
                    .HasName("sys_translation_shortcut_lang_UN")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Lang)
                    .IsRequired()
                    .HasColumnName("lang")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Shortcut)
                    .IsRequired()
                    .HasColumnName("shortcut")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value");
            });

            modelBuilder.Entity<Tab>(entity =>
            {
                entity.ToTable("tab");

                entity.HasIndex(e => new { e.ParentId, e.TabOrder })
                    .HasName("uq_tab_parent_id_tab_order")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Icon).HasColumnName("icon");

                entity.Property(e => e.IconString)
                    .HasColumnName("icon_string")
                    .HasMaxLength(250);

                entity.Property(e => e.Link)
                    .HasColumnName("link")
                    .HasMaxLength(200);

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.TabNameShortcut)
                    .IsRequired()
                    .HasColumnName("tab_name_shortcut")
                    .HasMaxLength(200);

                entity.Property(e => e.TabOrder).HasColumnName("tab_order");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId);
            });

            modelBuilder.Entity<TableName>(entity =>
            {
                entity.ToTable("table_name");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Db)
                    .HasColumnName("db")
                    .HasMaxLength(50);

                entity.Property(e => e.NameAr)
                    .HasColumnName("name_ar")
                    .HasMaxLength(50);

                entity.Property(e => e.NameEn)
                    .HasColumnName("name_en")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<TargetApplication>(entity =>
            {
                entity.ToTable("target_application");

                entity.HasIndex(e => new { e.AppId, e.TargetAppId })
                    .HasName("app_id_target_app_id_UN")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AppId).HasColumnName("app_id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TargetAppDesc)
                    .HasColumnName("target_app_desc")
                    .HasMaxLength(500);

                entity.Property(e => e.TargetAppDocument)
                    .HasColumnName("target_app_document")
                    .HasMaxLength(100);

                entity.Property(e => e.TargetAppId).HasColumnName("target_app_id");

                entity.HasOne(d => d.App)
                    .WithMany(p => p.TargetApplicationApp)
                    .HasForeignKey(d => d.AppId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("target_application_app_id_FK");

                entity.HasOne(d => d.TargetApp)
                    .WithMany(p => p.TargetApplicationTargetApp)
                    .HasForeignKey(d => d.TargetAppId)
                    .HasConstraintName("target_application_target_app_id_FK");
            });

            modelBuilder.Entity<TargetService>(entity =>
            {
                entity.ToTable("target_service");

                entity.HasIndex(e => new { e.ServiceId, e.TargetServiceId })
                    .HasName("target_service_UN")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ServiceId).HasColumnName("service_id");

                entity.Property(e => e.TargetDocumentTypeId).HasColumnName("target_document_type_id");

                entity.Property(e => e.TargetServiceId).HasColumnName("target_service_id");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.TargetServiceService)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("target_service_service_id_FK");

                entity.HasOne(d => d.TargetDocumentType)
                    .WithMany(p => p.TargetService)
                    .HasForeignKey(d => d.TargetDocumentTypeId)
                    .HasConstraintName("target_service_target_document_type_id_FK");

                entity.HasOne(d => d.TargetServiceNavigation)
                    .WithMany(p => p.TargetServiceTargetServiceNavigation)
                    .HasForeignKey(d => d.TargetServiceId)
                    .HasConstraintName("target_service_target_service_id_FK");
            });

            modelBuilder.Entity<Template>(entity =>
            {
                entity.ToTable("template");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DocumentTypeId).HasColumnName("document_type_id");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TitleShortcut)
                    .HasColumnName("title_shortcut")
                    .HasMaxLength(50);

                entity.Property(e => e.Type).HasColumnName("type");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.TemplateCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("template_created_by_FK");

                entity.HasOne(d => d.DocumentType)
                    .WithMany(p => p.Template)
                    .HasForeignKey(d => d.DocumentTypeId)
                    .HasConstraintName("template_docType_FK");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.TemplateLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("template_updated_by_FK");
            });

            modelBuilder.Entity<TemplateAttachment>(entity =>
            {
                entity.ToTable("template_attachment");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AttachmentId).HasColumnName("attachment_id");

                entity.Property(e => e.Required).HasColumnName("required");

                entity.Property(e => e.TemplateId).HasColumnName("template_id");

                entity.HasOne(d => d.Attachment)
                    .WithMany(p => p.TemplateAttachment)
                    .HasForeignKey(d => d.AttachmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("template_attachment_attachmentId_FK");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.TemplateAttachment)
                    .HasForeignKey(d => d.TemplateId)
                    .HasConstraintName("template_attachment_template_id_FK");
            });

            modelBuilder.Entity<TemplateParty>(entity =>
            {
                entity.ToTable("template_party");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PartyId).HasColumnName("party_id");

                entity.Property(e => e.Required).HasColumnName("required");

                entity.Property(e => e.SignRequired).HasColumnName("sign_required");

                entity.Property(e => e.TemplateId).HasColumnName("template_id");

                entity.HasOne(d => d.Party)
                    .WithMany(p => p.TemplateParty)
                    .HasForeignKey(d => d.PartyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("template_party_partyId_FK");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.TemplateParty)
                    .HasForeignKey(d => d.TemplateId)
                    .HasConstraintName("template_party_template_id_FK");
            });

            modelBuilder.Entity<Term>(entity =>
            {
                entity.ToTable("term");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Content).HasColumnName("content");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TemplateId).HasColumnName("template_id");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(100);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.TermCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("term_created_by_FK");

                entity.HasOne(d => d.LastUpdatedByNavigation)
                    .WithMany(p => p.TermLastUpdatedByNavigation)
                    .HasForeignKey(d => d.LastUpdatedBy)
                    .HasConstraintName("term_updated_FK");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.Term)
                    .HasForeignKey(d => d.TemplateId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("term_template_id_FK");
            });

            modelBuilder.Entity<TransactionFee>(entity =>
            {
                entity.ToTable("transaction_fee");

                entity.HasIndex(e => e.SubClass)
                    .HasName("uq_transaction_sub_class")
                    .IsUnique();

                entity.HasIndex(e => new { e.SubClass, e.PrimeClass })
                    .HasName("uq_transaction_fee_sub_prime_class")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.EntityCodeEpos)
                    .HasColumnName("entity_code_epos")
                    .HasMaxLength(50);

                entity.Property(e => e.EntityGlCodeEpos)
                    .HasColumnName("entity_gl_code_epos")
                    .HasMaxLength(50);

                entity.Property(e => e.LessThan).HasColumnName("less_than");

                entity.Property(e => e.LimitedValue).HasColumnName("limited_value");

                entity.Property(e => e.MappingFmisCodeEpos)
                    .HasColumnName("mapping_FMIS_code_epos")
                    .HasMaxLength(50);

                entity.Property(e => e.MaxLimitedTax).HasColumnName("max_limited_tax");

                entity.Property(e => e.MoreThan).HasColumnName("more_than");

                entity.Property(e => e.Multiplied).HasColumnName("multiplied");

                entity.Property(e => e.Notes)
                    .HasColumnName("notes")
                    .HasMaxLength(500);

                entity.Property(e => e.PerPage).HasColumnName("per_page");

                entity.Property(e => e.Percentage).HasColumnName("percentage");

                entity.Property(e => e.PrimeClass)
                    .IsRequired()
                    .HasColumnName("prime_class")
                    .HasMaxLength(50);

                entity.Property(e => e.ServiceCodeEpos)
                    .HasColumnName("service_code_epos")
                    .HasMaxLength(50);

                entity.Property(e => e.ServiceGlCodeEpos)
                    .HasColumnName("service_gl_code_epos")
                    .HasMaxLength(50);

                entity.Property(e => e.SubClass)
                    .IsRequired()
                    .HasColumnName("sub_class")
                    .HasMaxLength(50);

                entity.Property(e => e.TransactionNameShortcut)
                    .IsRequired()
                    .HasColumnName("transaction_name_shortcut")
                    .HasMaxLength(250);

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<TransactionOldVersion>(entity =>
            {
                entity.ToTable("transaction_old_version");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedBy).HasColumnName("created_by");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.DocumentUrl)
                    .IsRequired()
                    .HasColumnName("document_url")
                    .HasMaxLength(100);

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedBy).HasColumnName("last_updated_by");

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasMaxLength(500);

                entity.Property(e => e.OldTransactionId).HasColumnName("old_transaction_id");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.TransactionCreatedDate)
                    .HasColumnName("transaction_created_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

                entity.Property(e => e.TransactionNo)
                    .IsRequired()
                    .HasColumnName("transaction_no")
                    .HasMaxLength(100);

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.TransactionOldVersion)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("transaction_old_version_transaction_id_FK");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(255);

                entity.Property(e => e.AreaId).HasColumnName("area_id");

                entity.Property(e => e.BirthdayDate)
                    .HasColumnName("birthday_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ConcurrencyStamp).HasMaxLength(200);

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("created_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.EmailIdOldUsers)
                    .HasColumnName("email_id_old_users")
                    .HasMaxLength(50);

                entity.Property(e => e.EmailLang)
                    .HasColumnName("email_lang")
                    .HasMaxLength(50);

                entity.Property(e => e.EmaritIdOldUsers)
                    .HasColumnName("emaritID_old_users")
                    .HasMaxLength(50);

                entity.Property(e => e.EmiratesId)
                    .HasColumnName("emirates_id")
                    .HasMaxLength(50);

                entity.Property(e => e.EndEffectiveDate)
                    .HasColumnName("end_effective_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.FullName)
                    .HasColumnName("full_name")
                    .HasMaxLength(200);

                entity.Property(e => e.Gender)
                    .HasColumnName("gender")
                    .HasMaxLength(50);

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(200);

                entity.Property(e => e.LastUpdatedDate)
                    .HasColumnName("last_updated_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.LockoutEndDateUtc).HasColumnType("datetime");

                entity.Property(e => e.NatId).HasColumnName("nat_id");

                entity.Property(e => e.NormalizedEmail).HasMaxLength(200);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(200);

                entity.Property(e => e.NotaryPlaceId).HasColumnName("notary_place_id");

                entity.Property(e => e.NotificationType).HasColumnName("notification_type");

                entity.Property(e => e.PasswordHash).HasMaxLength(200);

                entity.Property(e => e.PhoneNumber).HasMaxLength(200);

                entity.Property(e => e.ProfileStatus).HasColumnName("profile_status");

                entity.Property(e => e.RecStatus)
                    .HasColumnName("rec_status")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.SecurityQuestionAnswer)
                    .HasColumnName("security_question_answer")
                    .HasMaxLength(100);

                entity.Property(e => e.SecurityQuestionId).HasColumnName("security_question_id");

                entity.Property(e => e.Sign)
                    .HasColumnName("sign")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SmsLang)
                    .HasColumnName("SMS_lang")
                    .HasMaxLength(50);

                entity.Property(e => e.StartEffectiveDate)
                    .HasColumnName("start_effective_date")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TelNo)
                    .HasColumnName("tel_no")
                    .HasMaxLength(50);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<UserClaim>(entity =>
            {
                entity.ToTable("user-claim");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserClaim)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId");
            });

            modelBuilder.Entity<UserLogger>(entity =>
            {
                entity.ToTable("user_logger");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.LoggingDate)
                    .HasColumnName("logging_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.StartWorkForEmployee).HasColumnName("start_work_for_employee");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserLogger)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user_logger_user");
            });

            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.ToTable("user-login");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.LoginDate)
                    .HasColumnName("login_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.LoginProvider)
                    .IsRequired()
                    .HasColumnName("login_provider")
                    .HasMaxLength(128);

                entity.Property(e => e.ProviderDisplayName)
                    .HasColumnName("provider_display_name")
                    .HasMaxLength(128);

                entity.Property(e => e.ProviderKey)
                    .IsRequired()
                    .HasColumnName("provider_key")
                    .HasMaxLength(128);

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserLogin)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserId");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("PK_dbo.AspNetUserRoles");

                entity.ToTable("user-role");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId");
            });

            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.ToTable("user-token");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.LogInProvider)
                    .IsRequired()
                    .HasColumnName("logInProvider")
                    .HasMaxLength(150);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(150);

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value")
                    .HasMaxLength(150);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserToken)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user-token_user");
            });

            modelBuilder.Entity<View1>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_1");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Lang)
                    .IsRequired()
                    .HasColumnName("lang")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.LookupTypeId).HasColumnName("lookup_type_id");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value");
            });

            modelBuilder.Entity<WorkingHours>(entity =>
            {
                entity.ToTable("working_hours");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.DayOfWeek).HasColumnName("day_of_week");

                entity.Property(e => e.FinishAt).HasColumnName("finish_at");

                entity.Property(e => e.FinishDate)
                    .HasColumnName("finish_date")
                    .HasColumnType("date");

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date")
                    .HasColumnType("date");

                entity.Property(e => e.StartFrom).HasColumnName("start_from");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.WorkingTimeNameShortcut)
                    .IsRequired()
                    .HasColumnName("working_time_name_shortcut")
                    .HasMaxLength(50);
            });

            modelBuilder.HasSequence("seq");

            modelBuilder.HasSequence("SeqForPayment")
                .StartsAt(1000)
                .HasMin(1000)
                .HasMax(9999999999999999);

            modelBuilder.HasSequence("Sequence_login").StartsAt(14101);

            modelBuilder.HasSequence("SequenceForMeetingId").StartsAt(100000);

            modelBuilder.HasSequence("transaction_seq").StartsAt(21734);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
