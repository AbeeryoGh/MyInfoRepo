using EngineCoreProject.DTOs.BasherDto;
using EngineCoreProject.DTOs.TransactionFeeDto;
using EngineCoreProject.IRepository.IBasherRepository;
using EngineCoreProject.IRepository.ITransactionFeeRepository;
using EngineCoreProject.IRepository.IApplicationSetRepository;
//using EngineCoreProject.IRepository.ITransactionFeeRepository;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;


using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Http;
using Nancy;
using System.ServiceModel;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace EngineCoreProject.Services.Basher
{
    [ServiceContract]
    [WebService(Namespace = "https://enotary.moj.gov.ae/enotary/notaryFee")]
    public class NotaryFee : WebService, INotaryFee
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly ITransactionFeeRepository _iTransactionFeeRepository;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly IApplicationRepository _iApplicationRepository;
        public ValidationSoapHeader Authentication;

        [MessageHeader]
        public MemberSoapHeader RequestHeader;

        public NotaryFee(ITransactionFeeRepository transactionFeeRepository, EngineCoreDBContext EngineCoreDBContext, IApplicationRepository iApplicationRepository, IGeneralRepository iGeneralRepository)
        {
            _iTransactionFeeRepository = transactionFeeRepository;
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _iApplicationRepository = iApplicationRepository;
        }

        
        // [SoapHeader("memHeader", Direction = SoapHeaderDirection.InOut)]
        [WebMethod]
        public async Task<retrieveNotaryFees_MOJResponse> RetrieveNotaryFees_MOJ(RetrieveNotaryFeesMOJRequest retrieveNotaryFees_MOJRequest)
        {
            retrieveNotaryFees_MOJResponse res = new retrieveNotaryFees_MOJResponse();

            RetrieveNotaryFeesMOJResponseError error = new RetrieveNotaryFeesMOJResponseError
            {
                ErrorCode = "ERR101",
                ErrorDescription = "Un-Expected Error"
            };


            if (retrieveNotaryFees_MOJRequest.EODBTrackingNumber == null || retrieveNotaryFees_MOJRequest.EODBTrackingNumber.Length == 0)
            {
                error.ErrorCode = "ERR102";
                error.ErrorDescription = "Invalid/Missing Parameter " + retrieveNotaryFees_MOJRequest.EODBTrackingNumber;
                FaultReason fr = new FaultReason("Invalid/Missing Parameter");
                throw new FaultException<RetrieveNotaryFeesMOJResponseError>(error, fr);
            }

            if (retrieveNotaryFees_MOJRequest.numberOfInvestors <= 0 || retrieveNotaryFees_MOJRequest.numberOfInvestors > 9999)
            {
                error.ErrorCode = "ERR102";
                error.ErrorDescription = "Invalid/Missing Parameter " + retrieveNotaryFees_MOJRequest.numberOfInvestors;
                FaultReason fr = new FaultReason("Invalid/Missing Parameter");
                throw new FaultException<RetrieveNotaryFeesMOJResponseError>(error, fr);
            }

            if (retrieveNotaryFees_MOJRequest.capitalAmount > 999999999999999)
            {
                error.ErrorCode = "ERR102";
                error.ErrorDescription = "Invalid/Missing Parameter " + retrieveNotaryFees_MOJRequest.capitalAmount;
                FaultReason fr = new FaultReason("Invalid/Missing Parameter");
                throw new FaultException<RetrieveNotaryFeesMOJResponseError>(error, fr);
            }

            if (retrieveNotaryFees_MOJRequest.EODBTrackingNumber.Length > 50)
            {
                error.ErrorCode = "ERR103";
                error.ErrorDescription = "Field Length is exceeded " + retrieveNotaryFees_MOJRequest.EODBTrackingNumber;
                FaultReason fr = new FaultReason("Invalid/Missing Parameter");
                throw new FaultException<RetrieveNotaryFeesMOJResponseError>(error, fr);
            }

            try
            {
                res.responseCode = "SUC100";
                res.responseDescription = "";
                res.EODBTrackingNumber = retrieveNotaryFees_MOJRequest.EODBTrackingNumber;

                List<TransactionFeeOutput> resList = new List<TransactionFeeOutput>();

                var editServiceNo = await _EngineCoreDBContext.AdmService.Where(x => x.UgId == Constants.UnifiedGateEditorConfirmServiceID).Select(x => x.Id).FirstOrDefaultAsync();
                TransactionFeeInput transactionFeeInput = new TransactionFeeInput()
                {
                    Amount = (double)retrieveNotaryFees_MOJRequest.capitalAmount,
                    DocumentKind = Constants.DOCUMENT_KIND.CONTRACTOREDITOR,  // ASK BASHER IF ALWAYS CONTRACT.
                    ProcessKind = Constants.PROCESS_KIND.CONFIRM,  // ASK BASHER IF ALWAYS CONFIRM.
                    PartiesCount = retrieveNotaryFees_MOJRequest.numberOfInvestors,
                    ServiceNo = editServiceNo
                };

                resList = await _iTransactionFeeRepository.CalculateTransactionFee(transactionFeeInput, "en");
                var amount = 0.0;
                foreach (var tranFee in resList)
                {

                    var feeNameShortCut = await _EngineCoreDBContext.TransactionFee.Where(x => x.Id == tranFee.FeeNo).Select(x => x.TransactionNameShortcut).FirstOrDefaultAsync();
                    FeesBreakDown toltalFeePart = new FeesBreakDown
                    {
                        serviceCode = tranFee.SubCalss.ToString(),
                        serviceFees = (double)tranFee.FeeValue,
                        serviceDescAr = await _iGeneralRepository.GetTranslateByShortCut("ar", feeNameShortCut),
                        serviceDescEn = tranFee.FeeName
                    };

                    amount += tranFee.FeeValue;
                    res.FeeDetailsList.Add(toltalFeePart);
                }

                res.totalFeesAmount = (double)amount;

                return res;
            }
            catch (Exception ex)
            {
                var x = ex.Message;
                FaultReason fr = new FaultReason("Un-Expected Error");
                throw new FaultException<RetrieveNotaryFeesMOJResponseError>(error, fr);
            }
        }
    }
}
