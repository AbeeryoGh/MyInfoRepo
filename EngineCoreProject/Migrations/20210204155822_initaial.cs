using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EngineCoreProject.Migrations
{
    public partial class initaial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "seq");

            migrationBuilder.CreateSequence(
                name: "SeqForPayment",
                startValue: 1000L,
                minValue: 1000L,
                maxValue: 9999999999999999L);

            migrationBuilder.CreateSequence(
                name: "SequenceForMeetingId",
                startValue: 100000L);

            migrationBuilder.CreateTable(
                name: "country",
                columns: table => new
                {
                    CNT_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CNT_Country_EN = table.Column<string>(maxLength: 64, nullable: true),
                    CNT_Country_AR = table.Column<string>(maxLength: 64, nullable: true),
                    CNT_Official_name_EN = table.Column<string>(maxLength: 64, nullable: true),
                    CNT_Official_name_AR = table.Column<string>(maxLength: 64, nullable: true),
                    CNT_Region_EN = table.Column<string>(maxLength: 64, nullable: true),
                    CNT_Continent_AR = table.Column<string>(maxLength: 64, nullable: true),
                    CNT_Continent_EN = table.Column<string>(maxLength: 64, nullable: true),
                    CNT_Region_AR = table.Column<string>(maxLength: 64, nullable: true),
                    CNT_Capital_EN = table.Column<string>(maxLength: 64, nullable: true),
                    CNT_Capital_AR = table.Column<string>(maxLength: 64, nullable: true),
                    CNT_ISO2 = table.Column<string>(maxLength: 64, nullable: true),
                    CNT_ISO3 = table.Column<string>(maxLength: 64, nullable: true),
                    CNT_GLOBAL_CODE = table.Column<string>(maxLength: 16, nullable: true),
                    CNT_REG_ID_FK = table.Column<int>(nullable: true),
                    CNT_CON_ID_FK = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("country_PK", x => x.CNT_ID);
                });

            migrationBuilder.CreateTable(
                name: "global_day_off",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
                    reason_shortcut = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_global_day_off", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_log",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    notification_title = table.Column<string>(nullable: false),
                    notification_body = table.Column<string>(nullable: false),
                    notification_channel_id = table.Column<int>(nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    send_report_id = table.Column<string>(nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    is_sent = table.Column<byte>(nullable: true),
                    sent_count = table.Column<byte>(nullable: false),
                    report_value_id = table.Column<string>(nullable: true),
                    user_id = table.Column<int>(nullable: true),
                    to_address = table.Column<string>(nullable: true),
                    lang = table.Column<string>(fixedLength: true, maxLength: 10, nullable: true),
                    hostsetting = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_template",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    notification_name_shortcut = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_template", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OCRDocuments",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    documentName = table.Column<string>(unicode: false, nullable: true),
                    documentType = table.Column<string>(unicode: false, nullable: true),
                    score = table.Column<double>(nullable: true),
                    Xmin = table.Column<int>(nullable: true),
                    Xmax = table.Column<int>(nullable: true),
                    Ymin = table.Column<int>(nullable: true),
                    Ymax = table.Column<int>(nullable: true),
                    bboxImg = table.Column<string>(unicode: false, nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OCRDocuments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(maxLength: 100, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "service_kind",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    service_kind_name_shortcut = table.Column<string>(maxLength: 100, nullable: false),
                    employee_count = table.Column<int>(nullable: false),
                    estimated_time_per_process = table.Column<int>(nullable: false),
                    symbol = table.Column<string>(maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_kind", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sys_language",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lang = table.Column<string>(unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_language", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sys_lookup_type",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_lookup_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sys_translation",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    shortcut = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    lang = table.Column<string>(unicode: false, maxLength: 5, nullable: true),
                    value = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_translation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tab",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    tab_name_shortcut = table.Column<string>(maxLength: 200, nullable: false),
                    link = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tab", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "table_name",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    db = table.Column<string>(maxLength: 50, nullable: true),
                    name_en = table.Column<string>(maxLength: 50, nullable: true),
                    name_ar = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_name", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "working_hours",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    working_time_name_shortcut = table.Column<string>(maxLength: 50, nullable: false),
                    day_of_week = table.Column<int>(nullable: false),
                    start_from = table.Column<int>(nullable: false),
                    finish_at = table.Column<int>(nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    finish_date = table.Column<DateTime>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_working_hours", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OCRDocumentFields",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fieldClass = table.Column<string>(unicode: false, nullable: true),
                    documentId = table.Column<int>(nullable: true),
                    score = table.Column<double>(nullable: true),
                    Xmin = table.Column<int>(nullable: true),
                    Xmax = table.Column<int>(nullable: true),
                    Ymin = table.Column<int>(nullable: true),
                    Ymax = table.Column<int>(nullable: true),
                    text = table.Column<string>(nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OCRDocumentFields", x => x.id);
                    table.ForeignKey(
                        name: "FK__OCRDocume__docum__0A295FE6",
                        column: x => x.documentId,
                        principalTable: "OCRDocuments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role-claim",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    roleid = table.Column<int>(name: "role-id", nullable: false),
                    claimtype = table.Column<string>(name: "claim-type", maxLength: 200, nullable: false),
                    claimvalue = table.Column<string>(name: "claim-value", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role-claim", x => x.id);
                    table.ForeignKey(
                        name: "FK_role-claim_role-id",
                        column: x => x.roleid,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(maxLength: 200, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 200, nullable: true),
                    email = table.Column<string>(maxLength: 200, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 200, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(maxLength: 200, nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 200, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    full_name = table.Column<string>(maxLength: 200, nullable: true),
                    security_question_id = table.Column<int>(nullable: true),
                    security_question_answer = table.Column<string>(maxLength: 100, nullable: true),
                    DOB = table.Column<string>(maxLength: 50, nullable: true),
                    gender = table.Column<string>(maxLength: 50, nullable: true),
                    nat_id = table.Column<int>(nullable: true),
                    tel_no = table.Column<string>(maxLength: 50, nullable: true),
                    status = table.Column<int>(nullable: true),
                    email_lang = table.Column<string>(maxLength: 50, nullable: true),
                    SMS_lang = table.Column<string>(maxLength: 50, nullable: true),
                    area_id = table.Column<int>(nullable: true),
                    notification_type = table.Column<int>(nullable: true),
                    profile_status = table.Column<int>(nullable: true),
                    address = table.Column<string>(maxLength: 100, nullable: true),
                    emirates_id = table.Column<string>(maxLength: 50, nullable: true),
                    role_id = table.Column<int>(nullable: true),
                    image = table.Column<string>(maxLength: 200, nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true),
                    notary_place_id = table.Column<int>(nullable: true),
                    LockoutEndDateUtc = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                    table.ForeignKey(
                        name: "user_role_FK",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "queue_processes",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    process_no = table.Column<string>(maxLength: 50, nullable: false),
                    service_kind_no = table.Column<int>(nullable: false),
                    expected_date_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    status = table.Column<byte>(nullable: false),
                    ticket_id = table.Column<int>(nullable: false),
                    note = table.Column<string>(maxLength: 200, nullable: true),
                    provider = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_queue_processes", x => x.id);
                    table.ForeignKey(
                        name: "FK_queue_processes_service_no",
                        column: x => x.service_kind_no,
                        principalTable: "service_kind",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sys_lookup_value",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lookup_type_id = table.Column<int>(nullable: true),
                    shortcut = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_lookup_value", x => x.id);
                    table.ForeignKey(
                        name: "FK_LookupValue_LookupType",
                        column: x => x.lookup_type_id,
                        principalTable: "sys_lookup_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "adm_service",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ug_id = table.Column<int>(nullable: true),
                    shortcut = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    fee = table.Column<int>(nullable: true),
                    icon = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    order = table.Column<int>(nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true),
                    service_kind_no = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adm_service", x => x.id);
                    table.ForeignKey(
                        name: "adm_service_created_by_FK",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "adm_service_updated_by_FK",
                        column: x => x.last_updated_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_service_service_kind",
                        column: x => x.service_kind_no,
                        principalTable: "service_kind",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "document_storage",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    file_name = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    file_path = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    id_user = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_storage", x => x.id);
                    table.ForeignKey(
                        name: "FK_document_storage_user",
                        column: x => x.id_user,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "file_configuration",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<string>(maxLength: 50, nullable: true),
                    extension = table.Column<string>(maxLength: 50, nullable: true),
                    max_size = table.Column<int>(nullable: true),
                    min_size = table.Column<int>(nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file_configuration", x => x.id);
                    table.ForeignKey(
                        name: "file_configuration_created_by _FK",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "file_configuration_updated_by_FK",
                        column: x => x.last_updated_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "meeting",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    topic = table.Column<string>(maxLength: 200, nullable: false),
                    description = table.Column<string>(nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    time_zone = table.Column<string>(maxLength: 50, nullable: false),
                    password = table.Column<string>(maxLength: 50, nullable: false),
                    password_req = table.Column<bool>(nullable: true),
                    meeting_id = table.Column<string>(maxLength: 20, nullable: false),
                    status = table.Column<int>(nullable: false),
                    user_id = table.Column<int>(nullable: false),
                    order_no = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meeting", x => x.id);
                    table.ForeignKey(
                        name: "FK_meeting_user_id_users_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user-claim",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user-claim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user-login",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey, x.UserId });
                    table.ForeignKey(
                        name: "FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user-role",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user-token",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    userId = table.Column<int>(nullable: false),
                    logInProvider = table.Column<string>(maxLength: 150, nullable: false),
                    name = table.Column<string>(maxLength: 150, nullable: false),
                    value = table.Column<string>(maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user-token", x => x.id);
                    table.ForeignKey(
                        name: "FK_user-token_user",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "adm_action",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    shortcut = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    action_type_id = table.Column<int>(nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adm_action", x => x.id);
                    table.ForeignKey(
                        name: "FK_adm_action_sys_lookup_value",
                        column: x => x.action_type_id,
                        principalTable: "sys_lookup_value",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notary_place",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    emirate_value_id = table.Column<int>(nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notary_place", x => x.id);
                    table.ForeignKey(
                        name: "notary_place_emirate_value_FK",
                        column: x => x.emirate_value_id,
                        principalTable: "sys_lookup_value",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notification_template_detail",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    notification_template_id = table.Column<int>(nullable: false),
                    notification_channel_id = table.Column<int>(nullable: false),
                    title_shortcut = table.Column<string>(maxLength: 250, nullable: true),
                    body_shortcut = table.Column<string>(maxLength: 250, nullable: true),
                    change_able = table.Column<bool>(nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_template_detail", x => x.id);
                    table.ForeignKey(
                        name: "FK_notification_template_channel_id_lookup_value",
                        column: x => x.notification_channel_id,
                        principalTable: "sys_lookup_value",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_notification_template_detail_notification_template_id",
                        column: x => x.notification_template_id,
                        principalTable: "notification_template",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "template",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    document_type_id = table.Column<int>(nullable: true),
                    title_shortcut = table.Column<string>(maxLength: 50, nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_template", x => x.id);
                    table.ForeignKey(
                        name: "template_created_by_FK",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "template_docType_FK",
                        column: x => x.document_type_id,
                        principalTable: "sys_lookup_value",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "template_updated_by_FK",
                        column: x => x.last_updated_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "adm_stage",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    service_id = table.Column<int>(nullable: true),
                    shortcut = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    period_for_archive = table.Column<int>(nullable: true),
                    period_for_late = table.Column<int>(nullable: true),
                    order_no = table.Column<int>(nullable: true),
                    fee = table.Column<int>(nullable: true),
                    stage_type_id = table.Column<int>(nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adm_stage", x => x.id);
                    table.ForeignKey(
                        name: "FK_adm_stage_adm_service",
                        column: x => x.service_id,
                        principalTable: "adm_service",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "adm_stage_stage_type_id_FK",
                        column: x => x.stage_type_id,
                        principalTable: "sys_lookup_value",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    amount = table.Column<string>(maxLength: 200, nullable: true),
                    price = table.Column<double>(nullable: true),
                    invoice_no = table.Column<string>(maxLength: 200, nullable: true),
                    status = table.Column<string>(maxLength: 200, nullable: true),
                    payment_status = table.Column<string>(maxLength: 200, nullable: true),
                    payment_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    collection_center_fee = table.Column<string>(maxLength: 200, nullable: true),
                    secure_hash = table.Column<string>(maxLength: 200, nullable: true),
                    e_dirham_fee = table.Column<string>(maxLength: 200, nullable: true),
                    e_service_data = table.Column<string>(nullable: true),
                    status_message = table.Column<string>(maxLength: 200, nullable: true),
                    confirmation_id = table.Column<string>(maxLength: 200, nullable: true),
                    pun = table.Column<string>(maxLength: 200, nullable: true),
                    payment_type = table.Column<string>(maxLength: 200, nullable: true),
                    payment_source = table.Column<string>(maxLength: 200, nullable: true),
                    payment_method_type = table.Column<string>(maxLength: 200, nullable: true),
                    application_id = table.Column<int>(nullable: true),
                    stage_id = table.Column<int>(nullable: true),
                    service_id = table.Column<int>(nullable: true),
                    user_id = table.Column<int>(nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true),
                    transaction_response_date = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.id);
                    table.ForeignKey(
                        name: "payments_service_FK",
                        column: x => x.service_id,
                        principalTable: "adm_service",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "payments_user_FK",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "calendar",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    notify_me = table.Column<int>(nullable: true),
                    description = table.Column<string>(maxLength: 200, nullable: true),
                    title = table.Column<string>(maxLength: 50, nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    meeting_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calendar", x => x.id);
                    table.ForeignKey(
                        name: "FK_calendar_meeting_id_meeting_id",
                        column: x => x.meeting_id,
                        principalTable: "meeting",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_calendar_calendar_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "meeting_participant",
                columns: table => new
                {
                    meeting_id = table.Column<int>(nullable: false),
                    user_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meeting_participant", x => new { x.meeting_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_meeting_participant_meeting_id_meeting_id",
                        column: x => x.meeting_id,
                        principalTable: "meeting",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_meeting_participant_userid_users_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "adm_stage_action_role",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    action_id = table.Column<int>(nullable: false),
                    role_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adm_stage_action_role", x => x.id);
                    table.ForeignKey(
                        name: "FK_adm_stage_action_role_adm_action",
                        column: x => x.action_id,
                        principalTable: "adm_action",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_adm_stage_action_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notification_action",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    action_id = table.Column<int>(nullable: false),
                    notification_template_id = table.Column<int>(nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_action", x => x.id);
                    table.ForeignKey(
                        name: "FK_notification_action_adm_action",
                        column: x => x.action_id,
                        principalTable: "adm_action",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "notification_action_notification_template_FK",
                        column: x => x.notification_template_id,
                        principalTable: "notification_template",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sys_execution",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    action_id = table.Column<int>(nullable: false),
                    execution_order = table.Column<int>(nullable: false),
                    to_execute = table.Column<string>(maxLength: 200, nullable: false),
                    parameter1 = table.Column<string>(maxLength: 500, nullable: true),
                    parameter2 = table.Column<string>(maxLength: 200, nullable: true),
                    method = table.Column<string>(maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_execution", x => x.id);
                    table.ForeignKey(
                        name: "sys_execution_action_id_FK",
                        column: x => x.action_id,
                        principalTable: "adm_action",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "template_attachment",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    template_id = table.Column<int>(nullable: false),
                    attachment_id = table.Column<int>(nullable: false),
                    required = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_template_attachment", x => x.id);
                    table.ForeignKey(
                        name: "template_attachment_attachmentId_FK",
                        column: x => x.attachment_id,
                        principalTable: "sys_lookup_value",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "template_attachment_template_id_FK",
                        column: x => x.template_id,
                        principalTable: "template",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "template_party",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    template_id = table.Column<int>(nullable: false),
                    party_id = table.Column<int>(nullable: false),
                    required = table.Column<bool>(nullable: true),
                    sign_required = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_template_party", x => x.id);
                    table.ForeignKey(
                        name: "template_party_partyId_FK",
                        column: x => x.party_id,
                        principalTable: "sys_lookup_value",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "template_party_template_id_FK",
                        column: x => x.template_id,
                        principalTable: "template",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "term",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    template_id = table.Column<int>(nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true),
                    title = table.Column<string>(maxLength: 100, nullable: true),
                    content = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_term", x => x.id);
                    table.ForeignKey(
                        name: "term_created_by_FK",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "term_updated_FK",
                        column: x => x.last_updated_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "term_template_id_FK",
                        column: x => x.template_id,
                        principalTable: "template",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "adm_stage_action",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    stage_id = table.Column<int>(nullable: true),
                    action_id = table.Column<int>(nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true),
                    show_order = table.Column<byte>(nullable: true),
                    enabled = table.Column<bool>(nullable: true, defaultValueSql: "((1))"),
                    group = table.Column<string>(unicode: false, maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adm_stage_action", x => x.id);
                    table.ForeignKey(
                        name: "FK_adm_stage_action_adm_action",
                        column: x => x.action_id,
                        principalTable: "adm_action",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "stage_action_created_by_FK",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "stage_action_updated_by_FK",
                        column: x => x.last_updated_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_adm_stage_action_adm_stage",
                        column: x => x.stage_id,
                        principalTable: "adm_stage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "application",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    service_id = table.Column<int>(nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true),
                    application_no = table.Column<string>(maxLength: 100, nullable: true),
                    transaction_start_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    transaction_end_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    template_id = table.Column<int>(nullable: true),
                    current_stage_id = table.Column<int>(nullable: true),
                    state_id = table.Column<int>(nullable: true),
                    unlimited_validity = table.Column<bool>(nullable: true),
                    note = table.Column<string>(maxLength: 255, nullable: true),
                    app_last_update_date = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application", x => x.id);
                    table.ForeignKey(
                        name: "application_created_by_FK",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "application_current_stage_id_FK",
                        column: x => x.current_stage_id,
                        principalTable: "adm_stage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "application_updated_by_FK",
                        column: x => x.last_updated_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "applications_service_FK",
                        column: x => x.service_id,
                        principalTable: "adm_service",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "application_template_id_FK",
                        column: x => x.template_id,
                        principalTable: "template",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "stage_master_attachment",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    stage_id = table.Column<int>(nullable: true),
                    master_attachment_id = table.Column<int>(nullable: true),
                    template_id = table.Column<int>(nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    required = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stage_master_attachment", x => x.id);
                    table.ForeignKey(
                        name: "FK_stage_master_attachment_sys_lookup_value",
                        column: x => x.master_attachment_id,
                        principalTable: "sys_lookup_value",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stage_master_attachment_adm_stage",
                        column: x => x.stage_id,
                        principalTable: "adm_stage",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "app_transaction",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    application_id = table.Column<int>(nullable: true),
                    file_name = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true),
                    content = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_transaction", x => x.id);
                    table.ForeignKey(
                        name: "transaction_application_id__FK",
                        column: x => x.application_id,
                        principalTable: "application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "application_attachment",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    mime_type = table.Column<string>(maxLength: 200, nullable: true),
                    size_ = table.Column<long>(nullable: true),
                    application_id = table.Column<int>(nullable: true),
                    attachment_id = table.Column<int>(nullable: true),
                    file_name = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true),
                    note = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_attachment", x => x.id);
                    table.ForeignKey(
                        name: "application_attachments_application_FK",
                        column: x => x.application_id,
                        principalTable: "application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "application_attachment_lookup_value_FK",
                        column: x => x.attachment_id,
                        principalTable: "sys_lookup_value",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "application_attachment_created_by_FK",
                        column: x => x.created_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "application_attachment_updated_by_FK",
                        column: x => x.last_updated_by,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "application_track",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    application_id = table.Column<int>(nullable: false),
                    user_id = table.Column<int>(nullable: true),
                    stage_id = table.Column<int>(nullable: true),
                    next_stage_id = table.Column<int>(nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true),
                    note = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_track", x => x.id);
                    table.ForeignKey(
                        name: "application_track_application_id_FK",
                        column: x => x.application_id,
                        principalTable: "application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "application_track_user_id_FK",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "application_party",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    party_id = table.Column<int>(nullable: true),
                    transaction_id = table.Column<int>(nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true),
                    is_owner = table.Column<bool>(nullable: true),
                    party_type_value_id = table.Column<int>(nullable: true),
                    full_name = table.Column<string>(maxLength: 100, nullable: false),
                    mobile = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    email = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    nationality = table.Column<int>(nullable: true),
                    birth_date = table.Column<DateTime>(type: "date", nullable: true),
                    marital_status = table.Column<int>(nullable: true),
                    gender = table.Column<int>(nullable: true),
                    emirates_id_no = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    id_expiration_date = table.Column<DateTime>(type: "date", nullable: true),
                    id_attachment = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    unified_number = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    sign_required = table.Column<bool>(nullable: true),
                    signed = table.Column<bool>(nullable: true),
                    sign_type = table.Column<int>(nullable: true),
                    sign_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    sign_url = table.Column<string>(unicode: false, maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_party", x => x.id);
                    table.ForeignKey(
                        name: "application_party_nationality_FK",
                        column: x => x.nationality,
                        principalTable: "country",
                        principalColumn: "CNT_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "applications_parties_user_FK",
                        column: x => x.party_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "application_party_id_lookup_value_FK",
                        column: x => x.party_type_value_id,
                        principalTable: "sys_lookup_value",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "application_party_transaction_id_FK",
                        column: x => x.transaction_id,
                        principalTable: "app_transaction",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "application_party_extra_attachment",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    application_party_id = table.Column<int>(nullable: false),
                    attachment_id = table.Column<int>(nullable: true),
                    attachment_name = table.Column<string>(maxLength: 100, nullable: true),
                    attachment_url = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    number = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    country_of_Issue = table.Column<int>(nullable: true),
                    expiration_date = table.Column<DateTime>(type: "date", nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_updated_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    start_effective_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    end_effective_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<int>(nullable: true),
                    last_updated_by = table.Column<int>(nullable: true),
                    rec_status = table.Column<string>(unicode: false, maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_party_extra_attachment", x => x.id);
                    table.ForeignKey(
                        name: "party_extra_attachment_application_party_FK",
                        column: x => x.application_party_id,
                        principalTable: "application_party",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "application_party_extra_attachment_attachment_id_FK",
                        column: x => x.attachment_id,
                        principalTable: "sys_lookup_value",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "application_party_extra_attachment_FK",
                        column: x => x.country_of_Issue,
                        principalTable: "country",
                        principalColumn: "CNT_ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_adm_action_action_type_id",
                table: "adm_action",
                column: "action_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_adm_service_created_by",
                table: "adm_service",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_adm_service_last_updated_by",
                table: "adm_service",
                column: "last_updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_adm_service_service_kind_no",
                table: "adm_service",
                column: "service_kind_no");

            migrationBuilder.CreateIndex(
                name: "IX_adm_stage_service_id",
                table: "adm_stage",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_adm_stage_stage_type_id",
                table: "adm_stage",
                column: "stage_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_adm_stage_action_action_id",
                table: "adm_stage_action",
                column: "action_id");

            migrationBuilder.CreateIndex(
                name: "IX_adm_stage_action_created_by",
                table: "adm_stage_action",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_adm_stage_action_last_updated_by",
                table: "adm_stage_action",
                column: "last_updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_adm_stage_action_stage_id",
                table: "adm_stage_action",
                column: "stage_id");

            migrationBuilder.CreateIndex(
                name: "IX_adm_stage_action_role_role_id",
                table: "adm_stage_action_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "uq_adm_stage_action_role",
                table: "adm_stage_action_role",
                columns: new[] { "action_id", "role_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "app_transaction_app_id_UN",
                table: "app_transaction",
                column: "application_id",
                unique: true,
                filter: "[application_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_application_created_by",
                table: "application",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_application_current_stage_id",
                table: "application",
                column: "current_stage_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_last_updated_by",
                table: "application",
                column: "last_updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_application_service_id",
                table: "application",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_template_id",
                table: "application",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_attachment_application_id",
                table: "application_attachment",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_attachment_attachment_id",
                table: "application_attachment",
                column: "attachment_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_attachment_created_by",
                table: "application_attachment",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_application_attachment_last_updated_by",
                table: "application_attachment",
                column: "last_updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_application_party_nationality",
                table: "application_party",
                column: "nationality");

            migrationBuilder.CreateIndex(
                name: "IX_application_party_party_id",
                table: "application_party",
                column: "party_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_party_party_type_value_id",
                table: "application_party",
                column: "party_type_value_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_party_transaction_id",
                table: "application_party",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_party_extra_attachment_application_party_id",
                table: "application_party_extra_attachment",
                column: "application_party_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_party_extra_attachment_attachment_id",
                table: "application_party_extra_attachment",
                column: "attachment_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_party_extra_attachment_country_of_Issue",
                table: "application_party_extra_attachment",
                column: "country_of_Issue");

            migrationBuilder.CreateIndex(
                name: "IX_application_track_application_id",
                table: "application_track",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_track_user_id",
                table: "application_track",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_calendar_meeting_id",
                table: "calendar",
                column: "meeting_id");

            migrationBuilder.CreateIndex(
                name: "IX_calendar_user_id",
                table: "calendar",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_document_storage_id_user",
                table: "document_storage",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_file_configuration_created_by",
                table: "file_configuration",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_file_configuration_last_updated_by",
                table: "file_configuration",
                column: "last_updated_by");

            migrationBuilder.CreateIndex(
                name: "meeting_id_Uniqe",
                table: "meeting",
                column: "meeting_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_meeting_user_id",
                table: "meeting",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_meeting_participant_user_id",
                table: "meeting_participant",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_notary_place_emirate_value_id",
                table: "notary_place",
                column: "emirate_value_id");

            migrationBuilder.CreateIndex(
                name: "IX_notification_action_notification_template_id",
                table: "notification_action",
                column: "notification_template_id");

            migrationBuilder.CreateIndex(
                name: "uq_action_id_notification_template_id",
                table: "notification_action",
                columns: new[] { "action_id", "notification_template_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__nameshortcut",
                table: "notification_template",
                column: "notification_name_shortcut",
                unique: true,
                filter: "[notification_name_shortcut] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_notification_template_detail_notification_channel_id",
                table: "notification_template_detail",
                column: "notification_channel_id");

            migrationBuilder.CreateIndex(
                name: "IX_notification_template_detail_notification_template_id",
                table: "notification_template_detail",
                column: "notification_template_id");

            migrationBuilder.CreateIndex(
                name: "IX_OCRDocumentFields_documentId",
                table: "OCRDocumentFields",
                column: "documentId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_service_id",
                table: "payment",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_user_id",
                table: "payment",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "unique_process_no",
                table: "queue_processes",
                column: "process_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_queue_processes_service_kind_no",
                table: "queue_processes",
                column: "service_kind_no");

            migrationBuilder.CreateIndex(
                name: "uq_queue_processes_process_no_service_kind_no",
                table: "queue_processes",
                columns: new[] { "process_no", "service_kind_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "role",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_role-claim_role-id",
                table: "role-claim",
                column: "role-id");

            migrationBuilder.CreateIndex(
                name: "UC_symbol",
                table: "service_kind",
                column: "symbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stage_master_attachment_master_attachment_id",
                table: "stage_master_attachment",
                column: "master_attachment_id");

            migrationBuilder.CreateIndex(
                name: "IX_stage_master_attachment_stage_id",
                table: "stage_master_attachment",
                column: "stage_id");

            migrationBuilder.CreateIndex(
                name: "IX_sys_execution_action_id",
                table: "sys_execution",
                column: "action_id");

            migrationBuilder.CreateIndex(
                name: "IX_sys_lookup_value_lookup_type_id",
                table: "sys_lookup_value",
                column: "lookup_type_id");

            migrationBuilder.CreateIndex(
                name: "sys_lookup_value_UN",
                table: "sys_lookup_value",
                column: "shortcut",
                unique: true,
                filter: "[shortcut] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "sys_translation_shortcut_IDX",
                table: "sys_translation",
                column: "shortcut");

            migrationBuilder.CreateIndex(
                name: "sys_translation_shortcut_lang_UN",
                table: "sys_translation",
                columns: new[] { "shortcut", "lang" },
                unique: true,
                filter: "[shortcut] IS NOT NULL AND [lang] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_template_created_by",
                table: "template",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_template_document_type_id",
                table: "template",
                column: "document_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_template_last_updated_by",
                table: "template",
                column: "last_updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_template_attachment_attachment_id",
                table: "template_attachment",
                column: "attachment_id");

            migrationBuilder.CreateIndex(
                name: "IX_template_attachment_template_id",
                table: "template_attachment",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "IX_template_party_party_id",
                table: "template_party",
                column: "party_id");

            migrationBuilder.CreateIndex(
                name: "IX_template_party_template_id",
                table: "template_party",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "IX_term_created_by",
                table: "term",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_term_last_updated_by",
                table: "term",
                column: "last_updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_term_template_id",
                table: "term",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "user",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "user",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_user_role_id",
                table: "user",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_user-claim_UserId",
                table: "user-claim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user-login_UserId",
                table: "user-login",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user-role_RoleId",
                table: "user-role",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_user-token_userId",
                table: "user-token",
                column: "userId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adm_stage_action");

            migrationBuilder.DropTable(
                name: "adm_stage_action_role");

            migrationBuilder.DropTable(
                name: "application_attachment");

            migrationBuilder.DropTable(
                name: "application_party_extra_attachment");

            migrationBuilder.DropTable(
                name: "application_track");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "calendar");

            migrationBuilder.DropTable(
                name: "document_storage");

            migrationBuilder.DropTable(
                name: "file_configuration");

            migrationBuilder.DropTable(
                name: "global_day_off");

            migrationBuilder.DropTable(
                name: "meeting_participant");

            migrationBuilder.DropTable(
                name: "notary_place");

            migrationBuilder.DropTable(
                name: "notification_action");

            migrationBuilder.DropTable(
                name: "notification_log");

            migrationBuilder.DropTable(
                name: "notification_template_detail");

            migrationBuilder.DropTable(
                name: "OCRDocumentFields");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "queue_processes");

            migrationBuilder.DropTable(
                name: "role-claim");

            migrationBuilder.DropTable(
                name: "stage_master_attachment");

            migrationBuilder.DropTable(
                name: "sys_execution");

            migrationBuilder.DropTable(
                name: "sys_language");

            migrationBuilder.DropTable(
                name: "sys_translation");

            migrationBuilder.DropTable(
                name: "tab");

            migrationBuilder.DropTable(
                name: "table_name");

            migrationBuilder.DropTable(
                name: "template_attachment");

            migrationBuilder.DropTable(
                name: "template_party");

            migrationBuilder.DropTable(
                name: "term");

            migrationBuilder.DropTable(
                name: "user-claim");

            migrationBuilder.DropTable(
                name: "user-login");

            migrationBuilder.DropTable(
                name: "user-role");

            migrationBuilder.DropTable(
                name: "user-token");

            migrationBuilder.DropTable(
                name: "working_hours");

            migrationBuilder.DropTable(
                name: "application_party");

            migrationBuilder.DropTable(
                name: "meeting");

            migrationBuilder.DropTable(
                name: "notification_template");

            migrationBuilder.DropTable(
                name: "OCRDocuments");

            migrationBuilder.DropTable(
                name: "adm_action");

            migrationBuilder.DropTable(
                name: "country");

            migrationBuilder.DropTable(
                name: "app_transaction");

            migrationBuilder.DropTable(
                name: "application");

            migrationBuilder.DropTable(
                name: "adm_stage");

            migrationBuilder.DropTable(
                name: "template");

            migrationBuilder.DropTable(
                name: "adm_service");

            migrationBuilder.DropTable(
                name: "sys_lookup_value");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "service_kind");

            migrationBuilder.DropTable(
                name: "sys_lookup_type");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropSequence(
                name: "seq");

            migrationBuilder.DropSequence(
                name: "SeqForPayment");

            migrationBuilder.DropSequence(
                name: "SequenceForMeetingId");
        }
    }
}
