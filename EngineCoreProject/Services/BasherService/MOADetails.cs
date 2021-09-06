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
    [WebService(Namespace = "https://enotary.moj.gov.ae/enotary/MOADetails")]
    public class MOADetails : WebService, IMOADetails
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly ITransactionFeeRepository _iTransactionFeeRepository;
        private readonly IGeneralRepository _iGeneralRepository;
        private readonly IApplicationRepository _iApplicationRepository;
        public ValidationSoapHeader Authentication;

        [MessageHeader]
        public MemberSoapHeaderDetail RequestHeader;


        public MOADetails(ITransactionFeeRepository transactionFeeRepository, EngineCoreDBContext EngineCoreDBContext, IApplicationRepository iApplicationRepository, IGeneralRepository iGeneralRepository)
        {
            _iTransactionFeeRepository = transactionFeeRepository;
            _EngineCoreDBContext = EngineCoreDBContext;
            _iGeneralRepository = iGeneralRepository;
            _iApplicationRepository = iApplicationRepository;
        }

      //  [SoapHeader("memHeader", Direction = SoapHeaderDirection.InOut)]
        [WebMethod]
        public async Task<sendMOADetailsMOJResponse> SendMOADetails_MOJ(SendAppMOADetails_MOJ sendMOADetails_MOJRequest)
        {      
            return await _iApplicationRepository.AddApplicationBashr(sendMOADetails_MOJRequest);
        }


    }
}
