using System.Collections.Generic;


namespace EngineCoreProject.Services
{
    internal class Emirate
    {
        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string EnglishName { get; set; }
    }

    public class Constants
    {
        public const int NOT_FOUND = -204;
        public const int ERROR = -500;
        public const int OK = -200;
        public const int ConcurrencyError = -600;

        public enum FIERST_SAVE_STAGE
        {
            DRAFT = 1,
            REVIEW = 2
        }

        public enum SERVICE_RESULT
        {
            CREATE = 1,
            CANCEL = -1,
            COPY = 2,
            EXECUTE = 3,
            NONE = 0
        }

        public enum PAYMENT_STATUS_ENUM
        {
            UNPAID = 0,
            PAID = 1,
            PartialPaid = 2,
            NOPAYMENT = 3
        }

        private static readonly List<Emirate> EMIRATES_LIST = new List<Emirate>{
                                                            new Emirate{
                                                              Id= 1,
                                                              ArabicName="ابوظبي",
                                                              EnglishName= "Abu Dhabi"
                                                            },
                                                            new Emirate{
                                                              Id= 2,
                                                              ArabicName="دبي",
                                                              EnglishName= "Dubai"
                                                            },
                                                            new Emirate
                                                            {
                                                                Id = 3,
                                                                ArabicName = "الشارقة",
                                                                EnglishName = "Sharjah"
                                                            },
                                                            new Emirate
                                                            {
                                                                Id = 4,
                                                               ArabicName = "رأس الخيمة",
                                                               EnglishName = "RAK"
                                                            },
                                                            new Emirate
                                                            {
                                                                Id = 5,
                                                                ArabicName = "الفجيرة",
                                                                EnglishName = "Abu Fujairah"
                                                            },
                                                            new Emirate
                                                            {
                                                                Id = 6,
                                                                ArabicName = "عجمان",
                                                                EnglishName = "Ajman"

                                                            },
                                                            new Emirate
                                                            {
                                                                Id = 7,
                                                                ArabicName = "أم القيوين",
                                                                EnglishName = "Umm Al Quwain"

                                                            }

          };

        public const int LOCK_SECONDS_TIME = 360;
        public static string GetEmirate(int EmirateId, string lang)
        {
            try
            {
                string Emirate = null;
                if (lang == "ar") Emirate = EMIRATES_LIST[EmirateId - 1].ArabicName;
                else Emirate = EMIRATES_LIST[EmirateId - 1].EnglishName;
                return Emirate;
            }
            catch { return null; }
        }

        public enum NoteKind
        {
            ISNEXT = 5,
            ClearSign=4,
            Rejected=3,
            Returned=2,
            Normal=0,
            AutoCancelled=1
        };

        public const int TRANSACTION_EDIT_DAY = 2;

        public enum AppStatus
        {
            OWNED = 4,
            BOOKEDUP = 3,
            LOCKED = 2,
            AVAILABLE = 1,
            UNAVAILABLE = 0,
            ISNEXT = 5,
        }

        //
        public const string DomainName = "JUSTICE";
        // constants for requests and stage type
        public const string stageDoneAR = "منجز";
        public const string stageDoneEN = "Done";
        public const string stageDraftAR = "مسود";
        public const string stageDraftEN = "draft";
        public const string stageMeetingEN = "meeting";
        public const string stageMeetingAR = "مقابلة";
        public const string stagePaymentAR = "الدفع";
        public const string stagePaymentEN = "payment";

        // constants for policy
        public const string AdminPolicy = "admin";
        public const string DefaultUserPolicy = "user";
        public const string EmployeePolicy = "employee";
        public const string DefaultVisitorPolicy = "visitor";
        public const string InspectorPolicy = "inspector";

        public const string INVALID_EMAIL_SUFFIX = "@invalidEmail.com";
        public const string INVALID_EMAIL_PREFIX = "invalidEmail";

        // Secret constants.
        public const string otpBase32Secret = "6L4OH6DDC4PLNQBA5422GM67KXRDIQQP";
        public const string meetingBase32Ssecret = "+z3=Ng_Wq)W]My^R*Aa8?+D{Hc]sE[gHdeL6FX";
        public const string OTP_TITLE_AR = "  أهلا بك في نظام كاتب العدل ";
        public const string OTP_TITLE_EN = "  Welcome to notary system ";
        public const string OTP_BODY_AR = " رمز التحقق ل ";
        public const string OTP_BODY_EN = " Verification code for  ";
        public const int OTP_PERIOD_If_MISSED_IN_APP_SETTING = 60;
        public const int MAX_PRIORITY_SMS = 5;
        public const int MIN_PRIROITY_SMS = 1;
        public const string DEFAULT_PRIROITY_SMS = "2";
        //

        public const string UnifiedGateEnotarySystemCode = "Notary System";
        public const string UnifiedGateEnotarySystemId = "7";
        public const string UnifiedGateCenterName = "Center";

        // UNIFIED GATE APPLICATIONS STATUS
        public const string UnifiedGateUnderProcessing = "Under Processing";
        public const string UnifiedGateApproved = "Approved";
        public const string UnifiedGateRejected = "Rejected";

        public const string UnifiedGateUnderProcessingAR = "العمل جاري عليه";
        public const string UnifiedGateApprovedAR = "تمت الموافقة";
        public const string UnifiedGateRejectedAR = "رفض الطلب";

        public const string UnifiedGateUnderProcessingID = "1";
        public const string UnifiedGateApprovedID = "2";
        public const string UnifiedGateRejectedID = "3";
        public const string UnifiedGateTotal = "0";
        public const string UnifiedGateWithoutStatus = "-1";

        public const int UnifiedGateEditorConfirmServiceID = 6710;

        public static readonly Dictionary<string, string> UGStatus = new Dictionary<string, string>()
        {
            {"Under Processing", "1" },
            {"Approved", "2" },
            {"Rejected", "3" },
            {"Total", "0" }
        };

        // constants for payment response message
        public const string ResponseStatus = "Response.Status";
        public const string ResponseInvoiceNo = "Response.InvoiceNo";
        public const string ResponseStatusMessage = "Response.StatusMessage";
        public const string ResponseConfirmationID = "Response.ConfirmationID";
        public const string ResponseAmount = "Response.Amount";
        public const string ResponsePUN = "Response.PUN";
        public const string ResponseCollectionCenterFees = "Response.CollectionCenterFees";
        public const string ResponseEDirhamFees = "Response.EDirhamFees";
        public const string ResponsePaymentMethodType = "Response.PaymentMethodType";
        public const string ResponseEServiceData = "Response.EServiceData";
        public const string ResponseSecureHash = "Response.SecureHash";
        public const string ResponseTransactionResponseDate = "Response.TransactionResponseDate";

        public const string ResponseBankID = "Response.BankID";
        public const string ResponseMerchantID = "Response.MerchantID";
        public const string ResponseMerchantModuleSessionID = "Response.MerchantModuleSessionID";
        public const string ResponseTerminalID = "Response.TerminalID";
        public const string ResponseReceiptID = "Response.ReceiptID";
        public const string ResponseCurrency = "Response.Currency";

        public const string SuccessfulCodeStatus = "0000";//WALLET
        public const string SuccessfulEPOSCodeStatus = "00";
        public const string SuccessfulCodeStatusWALLET = "WALLET-0000";//WALLETPendingBankCodeStatus
        public const string PendingBankCodeStatus = "0011";//WALLET

        public const string InitialPaymentStatus = "0";
        public const string InitialPaymentStatusBeforPay = "-1";
        public const string InitialStatus = "0";
        public const string InitialStatusBeforPay = "-1";
        public const string SuccessfullPaymentStatus = "1";
        public const string FailedPaymentStatusWithConfirmationId = "2";
        public const string FailedPaymentStatusWithOutConfirmationId = "3";
        public const string PandingBankPaymentStatus = "4";

        public const string PAYMENT_STATUS_READY_FOR_PAID_AR = " جاهز للدفع ";
        public const string PAYMENT_STATUS_READY_FOR_PAID_EN = " Ready for pay ";
        public const string EPOS_PAYMENT = "EPOS";

        public const string PAYMENT_STATUS_PAID_AR = " مدفوع ";
        public const string PAYMENT_STATUS_PAID_EN = " Paid ";

        public const string PAYMENT_STATUS_PARTIAL_AR = "  دفعة فاشلة او غير مدفوعة ";
        public const string PAYMENT_STATUS_PARTIAL_EN = " Filed payment or unpaid ";

        public const string PAYMENT_STATUS_NOPAYMENT_AR = "  لايوجد دفعات  ";
        public const string PAYMENT_STATUS_NOPAYMENT_EN = " No Payments ";

        // constants for the maximum days of the queue system tickets reserve.
        public const int MAX_QUEUE_DAYS = 100;

        // constants for the maximum number of fault attempts to send notification.
        public const int MAX_NOTIFY_SEND_ATTEMPTS = 3;

        // constants for tables names which contains shortcut fields.
        // tables.
        public const string SERVICE_KIND = "service_kind";
        public const string WORKING_HOURS = "working_hours";
        public const string DAY_OFF = "global_day_off";
        public const string NOTIFICATION_TEMPLATE = "notification_template";
        public const string NOTIFICATION_TEMPLATE_DETAIL = "notification_template_detail";
        public const string ROLE = "role";
        public const string TAB = "tab";
        public const string TRANSACTION_FEE = "transaction_fee";
        public const string LOCATION = "location";

        // tables inside lookupType.
        public const string NOTIFICATION_CHANNEL = "notification_channel";
        public const string NOTIFICATION_MAIL_CHANNEL = "notification_channel_mail";  // TODO rename to first channel and add second accord to Figma design.
        public const string NOTIFICATION_SMS_CHANNEL = "notification_channel_sms";
        public const string NOTIFICATION_INTERNAL_CHANNEL = "notification_channel_internal";

        // fields.
        public const string SERVICE_KIND_NAME_SHORTCUT = "ServiceKindNameShortcut";
        public const string LOCATION_NAME_SHORTCUT = "nameShortcut";
        public const string WORKING_TIME_NAME_SHORTCUT = "WorkingTimeNameShortcut";
        public const string TAB_NAME_SHORTCUT = "TabNameShortcut";
        public const string TRANSACTION_NAME_SHORTCUT = "TransactionNameShortcut";
        public const string DAY_OFF_REASON_SHORTCUT = "DayOffReasonShortcut";
        public const string NOTIFICATION_TEMPLATE_NAME_SHORTCUT = "NotificationTemplateNameShortcut";
        public const string NOTIFICATION_TEMPLATE_DETAIL_TITLE_SHORTCUT = "NotificationTemplateDetailTitleShortcut";
        public const string NOTIFICATION_TEMPLATE_DETAIL_BODY_SHORTCUT = "NotificationTemplateDetailBodyShortcut";
        public const string ROLE_NAME_SHORTCUT = "roleNameShortcut";

        // constants for parameter list.
        public const string EXPECTED_DATE = "ExpectedDate";
        public const string EXPECTED_TIME = "ExpectedTime";
        public const string APPLICATION_NUMBER = "ApplicationNo";
        public const string APPLICATION_NUMBER_MOB = "ApplicationNoMob";
        public const string TICKET_NUMBER = "TicketNumber";
        public const string INTEREVIEW_LINK = "InterviewLink";
        public const string INTEREVIEW_LINK_MOB = "IntereviewLinkMob";
        public const string PAYMENT_STATUS = "Payment_Status";
        public const string PAYMENT_STATUS_AR = "Payment_Status_ar";
        public const string APPLICATION_OBJECTION_TITLE = " اعتراض على المعاملة رقم ";
        public const int APPLICATION_OBJECTION_PERIOD_INMINUTES_DEFAULT = 1440;
        public const int APPLICATION_OBJECTION_NOTES_MAX_COUNT = 3;


        public const string transactionsPath = "transactions\\";
        public const string transactionsFolder = "transactions";
        // constants for OCR URL.
        public const string OCR_SERVICE_URL = "http://localhost:8888/ocr/v1/upload/EID";

        public const string FIX_BUTTON = "fix_btn";
        public enum MEETING_STATUS
        {
            FINISHED = -1,
            PENDING = 0,
            STARTED = 1
        }

        public enum NOTIFICATION_STATUS
        {
            PENDING = 0,
            SENT = 1,
            ERROR = 2,
        }

        public enum PROCESS_STATUS
        {
            PENDING = 0,
            INPROGRESS = 1,
            FINISHED = 2
        }

        public enum PROFILE_STATUS
        {
            ENABLED = 1,
            DISABLED = 0,
            BLOCKED = 2
        }

        public enum DOCUMENT_KIND
        {
            CONTRACTOREDITOR = 1,
            AGENCY = 2,
            ALL = 3
        }

        public enum PROCESS_KIND
        {
            EDIT = 1,
            CONFIRM = 2,
            ALL = 3
        }

        public const string ONPROGRESS = "ONPROGRESS";
        public const string FORSEND = "FORSEND";
        public const string RETURNED = "RETURNED";
        public const string REJECTED = "REJECTED";
        public const string AUTOCANCEL = "AutoCancel";
        public const string CANCELED = "CANCELED";
        public const string EXECUTED = "EXECUTED";
        public const string DONE = "DONE";

        public const string VIDEO_BTN = "video_btn";
        public const string PAY_BTN = "pay_btn";


        public const int TRANSLATOR = 11166;
        public const string APPLICATION_NO_PREFIX = "MOJEN_";
        public const string TRANSACTION_NO_PREFIX = "MOJAU_";

        public const string APPLICATION_NO_G2G_PREFIX = "G2GEN_";
        public const string TRANSACTION_NO_G2G_PREFIX = "G2GAU_";

        public static readonly Dictionary<string, string> ParameterDic = new Dictionary<string, string>()
        {
            {EXPECTED_DATE, "Date" },
            {EXPECTED_TIME, "Time" },
            {APPLICATION_NUMBER, "string" },
            {APPLICATION_NUMBER_MOB, "string" },
            {TICKET_NUMBER, "string" },
            {INTEREVIEW_LINK, "string" },
            {INTEREVIEW_LINK_MOB, "string" },
            {PAYMENT_STATUS, "string" },
            {PAYMENT_STATUS_AR, "string" }
        };

        public static string ReplaceParemeterByValues(Dictionary<string, string> parameterDic, string text)
        {
            foreach (var para in parameterDic)
            {
                if (text.Contains('@' + para.Key))
                {
                    text = text.Replace('@' + para.Key, para.Value);
                }
            }
            return text;
        }


        public const int TRANSLATOR_ID = 11166;

        public static readonly Dictionary<string, string[]> MessageList = new Dictionary<string, string[]>()
           {
            { "createCertSuccess",        new string[]   { "تم دمج الملفات وتوليد المعاملة بنجاح", "create certificate successfully" } },
             { "createCertError",        new string[]   { "حدث خطأ أثناء حفظ المعاملة", "An error occurred while saving the transaction" } },
             { "oldCertificate",        new string[]   { "يوجد نقص في ملفات النظام القديم يرجى الاتصال بمدير النظام", "The old system files are missing, please contact the system administrator" } },
             { "UTFD",        new string[]   { "-13 :Unable to fetch ddetails", "-13: Unable to fetch details" } },
            { "VCIDR",        new string[]   { "-10 : vcID is required", "-10: vcID is required" } },
             { "RDR",        new string[]   { " requesredData is required", "requestedData is required" } },
            { "VCIR",        new string[]   { " vcID is required", "vcID is required" } },
             { "OOR",        new string[]   {"Only VCID Or Requested Data is required", "Only VCID Or Requested Data is required" } },
            { "DataRequired",        new string[]   { "من فضللك أدخل جميع المعلومات المطلوبة", "please enter All required data" } },
            { "APPNF",        new string[]   { "لم يتم العثور على أية معاملات ", "Unable to find the application Specified" } },
            { "OLDTRN",        new string[]   { "-9 :Transaction reference number is Not Found", "-9 :Transaction reference number is Not Found" } },
            { "VCAPP",        new string[]   { "-11 :requestedData Or vcID is required", "-11 :  requestedData Or vcID is required" } },
            { "TRNF",        new string[]   { "-2 : Transaction reference number is required", "-2 : Transaction reference number is required" } },
            { "ENF",        new string[]   {"-3 : Emirates ID is required",   "-3 : Emirates ID is required" } },
            { "sucsessAdd",        new string[]   {"تمت الإضافة بنجاح",   "record added successfully" } },
            { "sucsessUpdate",     new string[]   {"تم التعديل بنجاح",   "record updated successfully" } },
            { "sucsessDelete",     new string[]   {"تم حذف السجل بنجاح", "record deleted successfully" } },
            { "failedAdd",         new string[]   {"لم تتم إضافة السجل" ,"record insert failed" } },
            { "faildDelete",       new string[]   {"لم يتم الحذف" ,       "record deleted failed" } },
            { "joinedRecord",      new string[]   {"السجل مرتبط بعملية اخرى" , "record is joined with another process" } },
            { "faildUpdate",       new string[]   {"لم يتم التعديل" ,    "record deleted failed" } },
            { "unallowedFileSize", new string[]   { "حجم الملف غير مقبول" , "not allowed file size" } },
            { "unallowedFileType", new string[]   {"نوع ملف غير مسموح به",  "not allowed file type" } },
            { "fileUploaded",      new string[]   {"تم رفع الملف بنجاح",    "File uploaded successfully" } },
            { "UnknownError",      new string[]   {"حدث خطأ ما...أعد المحاولة", "Something went wrong. Please try again later." } },
            { "parentNotFound",    new string[]   {"السجل الأب غير موجود", "parent record not found." } },
            { "repeatedRecord",    new string[]   {"السجل  مكرر", " The record is repeated." } },
            { "fileDeletedFailed", new string[]   {"لم يتم حذف الملف",    "file not deleted!." } },
            { "fileNotFound",      new string[]   {"الملف غير موجود",     "file not found!." } } ,
            { "ServiceNotFound",   new string[]   {"الخدمة غير موجودة",     "service not found!." } } ,
            { "FeeNotFound",       new string[]   {"الرسم غير موجود",     "Fee not found!." } } ,
            { "DocumentKindNotFound",       new string[]   {"نوع وثيقة غير موجود",     "Document kind not found!." } } ,
            { "ProcessKindNotFound",       new string[]   {"نوع عملية غير موجود",     "Process kind not found!." } } ,
            { "ConflictDocumentKind", new string[]   {"  تعارض في ربط الخدمة بالرسم في نوع الوثيقة ",     "Conflict in tie service with fee in document kind!." } } ,
            { "ConflictProcessKind",  new string[]   {"  تعارض في ربط الخدمة بالرسم في نوع العملية ",     "Conflict in tie service with fee in process kind!." } } ,
            { "EmailExistedBefore",   new string[]   {"عنوان الايميل موجود مسبقا مرتبط برقم وطني اخر", "Email address existed before and attached to another Emirate Id." } } ,
            { "InvalidEmailFormat",   new string[]   {" صيغة عنوان الايميل خاطئة ", "Invalid Email address format" } } ,
            { "InvalidPhoneNumber",   new string[]   {"رقم الهاتف غير صحيح", "Invalid phone number." } } ,
            { "InvalidSubClass",      new string[]   {"رقم   البند الفرعي غير صحيح ", "Invalid sub class number." } } ,
            { "InvalidPrimeClass",    new string[]   {"رقم البند الرئيسي غير صحيح", "Invalid prime class number." } } ,
            { "InvalidQuantityNumber",new string[]   {"رقم الكمية  غير صحيح", "Invalid quantity number." } } ,
            { "ExistedPhoneNumber",   new string[]   {"رقم الهاتف  موجود مسبقا", "Existed phone number." } } ,
            { "UserNotExistedBefore", new string[]   {"   المستخدم غير موجود  ", "User not existed." } } ,
            { "UserEmailError", new string[]   {" عنوان الايميل غير موجود يرجى التحقق ", " Email not found, check please. " } } ,
            { "UserUGPermissing", new string[]   {" ليس لديك سماحية للوصول الى نظام كاتب العدل من خلال البوابة ", " You do not have permission to access the enotary system. " } } ,
            { "UserAccountLocked", new string[]   {" حسابك مقفل الرجاء مراجعة المدير ", " Your account is blocked, please ask the admin. " } } ,
            { "wrongPassword", new string[]   {" كلمة المرور خاطئة يرجى التاكد منها ", " Wrong password, please check. " } } ,
            { "UserNameExistedBefore",new string[]   {"  اسم المستخدم موجود مسبقا ", "User name existed before." } } ,
            { "MissedFullName",       new string[]   {"ب6 محارف على الاقل الرجاء ادخال اسم المعترض ", "Please add your full name 6 character at least." } } ,
            { "EmiratesIDExistedBefore", new string[]   {" الرقم الاماراتي موجود مسبقا", "Emirates id existed before." } } ,
            { "missedParameter",   new string[]   {"بارامتر مفقود",       "Missed parameter!." } },
            { "zeroResult",        new string[]   {"لا يوجد نتائج مطابقة", "No matching result!." } },
            { "zeroNotAllowed",        new string[]   {"القيمة الصفرية للرسم غير مقبولة ", " Zero value is not allowed !." } },
            { "wrongParameter",    new string[]   {"بارامتر خاطئ",       "Wrong parameter!." } },
            { "existedBefore",     new string[]   {" قيمة موجودة مسبقا",       " parameter value existed before!." } },
            { "missedLocation",     new string[]   {" موقع غير موجود  ",       " Missed location" } },
            { "errorPayment",     new string[]   { "حدث خطأ غير معروف ", "UNKNOW ERROR!!!!!!" } },
            { "errorAuthentication", new string[]   { "خطا في معلومات تسجيل الدخول ", "Incorrect authentication data." } },
            { "errorConfig",     new string[]   { "خطا في معلومات اعدادات الحساب ", "No connection could be made. Recheck your configuration please." } },
            { "alreadySigned",   new string[]   { "لقد قمت مسبقا بالتوقيع على هذه المعاملة ", "You have already signed this transaction" } },
            { "meetingFinished", new string[]   {" الاجتماع بحالة منتهيه",       "The meeting is finished" } },
            { "ConcurrencyError",new string[]   {"تم التعديل على الطلب من قبل شخص آخر قبل قيامك بالحفظ", "Concurrency Error " } },
            { "PartyAddFail",    new string[]   {"خطأ بإضافة أحد الأطراف", "Party Add Fails" } },
            { "PartyUpdateFail", new string[]   {"خطأ بتعديل الطرف", "Party Update Fails" } },
            { "ExtraAttachmentUpdateFail",    new string[]   {"خطأ بتعديل المرفق الاضافي", "ExtraAttachmentUpdateFail" } },
            { "AttachmentUpdateFail", new string[]   {"خطأ بتعديل المرفق ", "Attachment Update Fail" } },
            { "AttachmentAddFail",    new string[]   {"خطأ بإضافة المرفق ", "Attachment Adding Fail" } },
            { "AttachmentFileAddFail",    new string[]   {"خطأ بإضافة الملف المرفق ", "Attachment file Adding Fail" } },
            { "TransactionAddFail",   new string[]   {"خطأ بإضافة المعاملة ", "Attachment Adding Fail" } },
            { "MissedAppReason",   new string[]   {" يجب ادخال سبب للتعديل من 10 محرف على الاقل ", "Missed reason, you needs to write a reason of 10 character at least." } },
            { "AppReasonAtMost",   new string[]   {" يجب ادخال سبب للتعديل لا يتجاوز 200 محرف   ", "Wrong reason, you needs to write a reason of 200 character at most." } },
            { "WrongObjection",   new string[]   {" خطا في اضافة الاعتراض ", "unknown error for adding the objection." } },
            { "TrackUpdateFail",      new string[]   {"خطأ بتعديل الملاخظات المرافقة للطلب ", "TrackUpdateFail" } },
            { "TransactionUpdateFail",    new string[]   {"خطأ بتعديل بيانات الوثيقة ", "TransactionUpdateFail" } },
            { "Done",    new string[]   {"تمت العملية بنجاح ", "Done Successfully" } },
            { "ChangeStateFail",    new string[]   {"خطأ في تغيير حالة الطلب ", "Changing application state failed" } },
            { "AppNotProvided",     new string[]   {"نقص في معلومات الطلب",     "Application data not provided." } } ,
            { "TargetAppNotAdded",  new string[]   {"خطـأ في إضافة الطلب الهدف",     "Target Application  not added." } } ,
            { "ExAttachmentNotAdded",new string[]   {"خطـأ في إضافة المرفق",     "Attachment  not added." } } ,
            { "PartyNotAdded",      new string[]   {"خطـأ في إضافة الطرف",     "Party  not added." } } ,
            { "PartyUserNotAdded",  new string[]   {"خطـأ في إضافة الطرف كمستخدم",     "Party as user  not added." } } ,
            { "MeetingUrl",   new string[]   {"رابط المقابلة : ",     "Meeting Link : " } } ,
            { "Success",      new string[]   {"ناجح", "successful" } } ,
            { "Fail",         new string[]   {"فاشل ", "Unsuccessful" } } ,
            { "FailToken",    new string[]   {"فشل في انشاء توكن  ", "Filed in generating the token" } } ,
            { "MissedGUID",   new string[]   {" توكن مفقود ", "Missed Token" } } ,
            { "ExpiredToken", new string[]   {" الرابط منتهي الصلاحية ", "Expired Token" } } ,
            { "NotActivatedToken",new string[]   {" الرابط  غير مفعل بعد ", "The URL is not enabled yet." } } ,
            { "InvalidToken",     new string[]   {" الرابط  غير صالح  ", "The URL is Invalid." } } ,
            { "unauthoraizedForAPP", new string[]   {"  ليس لديك سماحية للوصول الى هذا الطلب  ", "The User is not authorized for the chosen application." } } ,
            { "userHasNotAddresses", new string[]   {"لا يوجد عناوين مرتبطة بالمستخدم لارسال الرمز ", "The User has not addresses to send OTP." } } ,
            { "FiledInsendingOTP",   new string[]   {"خطا في ارسال الرمز الرجاء محاولة الدخول باستخدام حسابك في البوابة الموحدة ", "Failed in sending OTP, please try to log in by your account at undefined gate." } } ,
            { "ExpiredOTP",   new string[]   {"انتهت صلاحية الرمز المرسل, اعد طلب رمز جديد ", "Expired OTP, please renew another OTP" } } ,
            { "IncorrectOTP",   new string[]   {"  الرمز خاطئ يرجى التاكد ", "Incorrect OTP, please review the sending code" } } ,
            { "FailedAddedUser",     new string[]   {" خطا في اضافة المستخدم ", "Failed Added User." } },
            { "AppAddFail",          new string[]   {"حدث خطأ في حفظ الطلب ", "There was an error saving the application" } } ,
            { "AppUpdateFail",       new string[]   {"حدث خطأ في تعديل الطلب ", "There was an error updating the application" } } ,
            { "AppFolderFail",       new string[]   { "خطأ في إنشاء مجلد الطلب ", "There was an error creating the application folder" } } ,
            { "AppAppTrackFail",     new string[]   { "خطأ في إضافة ملاحظات الطلب ", "There was an error adding the application note" } } ,
            { "AppNotesExceeded",     new string[]   { " تجاوزت عدد الملاحظات التي يمكنك ارسالها لكاتب العدل ", "The count of application notes is exceeded" } } ,
            { "AppNotesPeriodExceeded",     new string[]   { " تجاوزت الزمن المحدد لارسال الملاحظات بعد انجاز المعاملة ", "The period of application notes is exceeded" } } ,
            { "AppNotFound",         new string[]   { "طلب غير موجود ", "App not found" } } ,
            { "SetStageFail",        new string[]   { "خطأ في تحديد رقم المرحلة ", "Error specifying the stage number" } } ,
            { "SetStateFail",        new string[]   { "خطأ في تحديد الحالة ", "Error specifying the state number" } } ,
            { "missedRoleName",   new string[]   {"اسم الصلاحية مفقود",       "Missed Role name!." } },
            { "duplicatedRoleName",  new string[]   {"اسم الصلاحية مكرر", "The name of the role is existed before!." } },
            { "RoleNotFound",   new string[]   {" الصلاحية غير موجودة ", "The Role is not found!." } },
            { "RoleJoined",   new string[]   {" الصلاحية  مرتبطة باحد المستخدمين يجب ازالتها اولا ", "The Role is joined to user, remove first!." } },
            { "MainRole",   new string[]   {" صلاحية اساسية لايمكن حذفها ", "The Role is minor, unable to delete!." } },
            { "Approval",   new string[]   {" نص الاعتماد : ", "Approval text : " } },
            { "SignFail",   new string[]   {"خطأ في حفظ التوقيع : ", "Sign saving fails " } },
            { "ApplicationNotFound",   new string[]   {"  الطلب غير موجود  ", "The Application is not found!." } },
            { "FeesListNotFound",   new string[]   { " قائمة الرسوم فارغة   ", "The List of fees is empty." } },
            { "FeesServiceNotFound",   new string[]   { " لايوجد رسوم لهذه الخدمة   ", "The List of fees is empty for this kind of application." } },
            { "unUsedFee",   new string[]   { "   رسم غير موجود   ", "The fee is not existed before." } },
            { "failedAddPayment",   new string[]   { "     فشل في اضافة الدفعة   ", " Failed in adding the payment." } },
            { "paidBefore",   new string[]   { " طلب مدفوع مسبقا ", " payment is paid before." } },
            { "noInvoiceNumberforAPP",   new string[]   { " لايوجد رقم فاتورة مجهزة لهذا الطلب ", " No Invoice number prepared for this application." } },
            { "nopaymentsforAPP",   new string[]   { " لايوجد دفعات مجهزة لهذا الطلب ", " No payments prepared for this application." } },
            { "nopaymentDeatilsforAPP",   new string[]   { " لايوجد تفاصيل دفعات مجهزة لهذا الطلب ", " No payments details prepared for this application." } },
            { "ExistPaymentForAppBefore",  new string[]   { "   يوجد دفعة مرتبطة بهذا الطلب مسبقا   ", "the application is joined to another payment before." } },
            { "missedFeeName",   new string[]   {"اسم الرسم مفقود",       "Missed Fee name!." } },
            { "missedFeeForService",   new string[]   {"هذه الخدمة تتطلب رسم غير مضاف في جدول الرسوم",       "Missed Fee for this service !." } },
            { "duplicatedFeeSubPrimeClass",   new string[]   {" الرمز الرئيسي والفرعي موجود مسبقا ", "The prime and sub class are existed before!." } },
            {"stageNotFound",new string[] {"المرحلة غير موجودة","Stage not Found"} },
            {"currentAppStage",new string[] {"لا يمكنك الحذف .. المرحلة مرتبطة مع الطلبات","Can't delete.. This stage is related with an application "} },
            {"stageAction",new string[] {"لا يمكنك الحذف ... المرحلة مرتبطة بأحداث خاصة بالطلبات","Can't delete .. this stage is related with Actions for Applications "} },
            {"serviceNotFound",new string[] {"الخدمة غير موجودة","Service not Found"} },
            {"servicesStage",new string[] {"هذه الخدمة مرتبطة بمراحل ... لا يمكنك الحذف","This Service is related with one or more of stages ... Can't delete"} },
            {"stageAttachment" , new string[] {"هذه المرحلة مرتبطة بعدة مرفقات ... لا يمكنك الحذف","This stage is related with many attachments ... Can't delete"} },
            {"templateProperties", new string[] {"هذا القالب يحوي أطراف أو مرفقات أو بنود ... لا يمكن حذفه","this template have parties or attachment or terms ... Cant't Delete"} },
            {"templateNotfound", new string[]{"القالب غير موجود","this template not found"} },
            {"applicationTemplate", new string[]{"هذا القالب مرتبط بأحد الطلبات ... لا يمكن حذفه","this template is related with application ... Can't Delete" } },
            {"FileNotFound", new string[]{"الملف غير موجود","file not found" } },
            {"NotIdenticalPassword", new string[]{" كلمتا المرور غير متطابقتين "," new passord id not identical with the confirmation" } },
            {"PaidSuccess", new string[]{  "تمت العملية بنجاح","operation accomplished successfully" } },//
            {"SessionTimeExpired", new string[]{  "انتهت مدة الجلسة","Session time expired" } },
            {"AskPaymentEposSuccess", new string[]{ "تمت عملية طلب الدفع بنجاح", "The payment request has been processed successfully" } },
            {"AskPaymentEposfailed", new string[]{  "فشل عملية طلب الدفع","The payment request process failed" } },
            {"UpdatePaymentEposStatus", new string[]{  "فشل تحديث حالة الدفع", "Payment status update failed" } },
            {"PaymentNotDefinedAsEpos", new string[]{  "  الدفع غير محدد مسبقا بجهاز الدفع ", "Payment status is not EPOS payment." } },
            {"bookedApp", new string[]{" الطلب محجوز من قبل كاتب عدل آخر  " ,  "The application is booked up by another notary "} },
            {"Party", new string[]{ "طرف في المعاملة", "Party" } },
            {"Owner", new string[]{ "مقدم الطلب", "Owner" } },
            {"notSigned", new string[]{ "يرجى التوقيع من جميع الأطراف المطلوب توقيعهم", "Please sign by all parties whose signature is required" } },
            {"YES", new string[]{ "نعم", "yes" } },
            {"NO", new string[]{ "لا", "No" } },
            {"NAvailableService", new string[]{ "هذه الخدمة غير متاحة حاليا", "This service is not currently available" } },
            {"ExistingReceiptNumber", new string[]{ "رقم ايصال الدفع موجود مسبقا", "A receipt number already exists" } },
            {"OkNotifyParties", new string[]{ "تم إخطار الأطراف", "Parties have been notified" } },
            {"NoNotifyParties", new string[]{ "حصل خطأ في إخطار الأطراف", "There was an error in notifying the parties" } },
            {"signNotAllowed", new string[]{ "التوقيع غير مسموح حاليا ..الرجاء انتظار كاتب العدل", "Signature is not allowed at this time .. please wait for the notary" } },
            {"partyNotFound", new string[]{ "الطرف غير موجود", "Party not found" } },
            {"AppDoneSuccessfully", new string[]{ "تم إنجاز الطلب بنجاح", "Application done successfully" } },
            {"ConvertImageToPDF", new string[]{ "فشل في استخراج الصور من الملف", "Failed to extract images from file" } },
            {"SignPDF", new string[]{ "حدث خطأ اثناء توثيق ملف المعاملة", "An error occurred while authenticating the transaction file" } },
            {"EposServiceCode", new string[]{ "لا يوجد رمز للخدمة", "There is no service code" } },
            {"rejectedApp", new string[]{ "تم رفض هذا الطلب..لاستكمال الطلب يرجى الضغط على زر التراجع عن الرفض", "This application is rejected" } },
            {"OwnedApp", new string[]{ "هذا الطلب عائد لكاتب عدل آخر", "This application is for another notary" } },
            {"pickApp", new string[]{ "الرجاء اختيار طلب من الجدول ", "Please select an application from the table" } },
            {"pickTransaction", new string[]{ "الرجاء تحديد المحرر المطلوب من خلال أحد الخيارات التالية :", "Please select the required document through one of the following options:" } },
            {"blindSearchText", new string[]{ "  الرجاء ادخال خمس محارف على الاقل للبحث", "Please enter 5 characters at least to search." } },
            {"notAuthoraizedNotary", new string[]{ "  عذرا : انت غير مخول بإعادة إنشاء هذه الوثيقة لأنها عائدة لكاتب عدل آخر", " Sorry, you can not rebuild this application because you are not the notary owner." } },
            {"notPaidRegenerateAPP", new string[]{ " عذرا :    لايمكنك  إنشاء هذه الوثيقة لأن حالة الدفع هي     ", " Sorry, you can not build this application because the status of it's payment is ." } }

        }
           ;//errorPayment
            //


        public static string getMessage(string lang, string key)
        {
            string message;
            try
            {

                message = lang == "ar" ? MessageList[key][0] : MessageList[key][1];
            }
            catch
            {
                message = "key not found";
            }

            return message;
        }

        public enum EXTERNAL_FILE_REQUIRED
        {
            NO = 0,
            ONE = 1,
            OPTIONAL = 2,
            MULTI = 3
        }
    }
}
