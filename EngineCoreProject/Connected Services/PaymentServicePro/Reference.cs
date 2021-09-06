﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PaymentServicePro
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PaymentServiceResponse", Namespace="http://schemas.datacontract.org/2004/07/PMRWeb.PaymentRouter")]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(PaymentServicePro.PaymentGetStatusDirectResponse))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(PaymentServicePro.PaymentInquiryServiceResponse))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(PaymentServicePro.PaymentGetStatusResponse))]
    public partial class PaymentServiceResponse : object
    {
        
        private string ConfirmationIDField;
        
        private string StatusField;
        
        private string StatusMessageField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ConfirmationID
        {
            get
            {
                return this.ConfirmationIDField;
            }
            set
            {
                this.ConfirmationIDField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Status
        {
            get
            {
                return this.StatusField;
            }
            set
            {
                this.StatusField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StatusMessage
        {
            get
            {
                return this.StatusMessageField;
            }
            set
            {
                this.StatusMessageField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PaymentGetStatusDirectResponse", Namespace="http://schemas.datacontract.org/2004/07/PMRWeb.PaymentRouter")]
    public partial class PaymentGetStatusDirectResponse : PaymentServicePro.PaymentServiceResponse
    {
        
        private string AmountField;
        
        private string CollectionCenterFeesField;
        
        private string EDirhamFeesField;
        
        private string EServiceDataField;
        
        private string InvoiceIDField;
        
        private string OrginalTransactionMessageStatusField;
        
        private string OrginalTransactionStatusField;
        
        private string PUNField;
        
        private string TransactionResponseDateField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Amount
        {
            get
            {
                return this.AmountField;
            }
            set
            {
                this.AmountField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CollectionCenterFees
        {
            get
            {
                return this.CollectionCenterFeesField;
            }
            set
            {
                this.CollectionCenterFeesField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EDirhamFees
        {
            get
            {
                return this.EDirhamFeesField;
            }
            set
            {
                this.EDirhamFeesField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EServiceData
        {
            get
            {
                return this.EServiceDataField;
            }
            set
            {
                this.EServiceDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string InvoiceID
        {
            get
            {
                return this.InvoiceIDField;
            }
            set
            {
                this.InvoiceIDField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string OrginalTransactionMessageStatus
        {
            get
            {
                return this.OrginalTransactionMessageStatusField;
            }
            set
            {
                this.OrginalTransactionMessageStatusField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string OrginalTransactionStatus
        {
            get
            {
                return this.OrginalTransactionStatusField;
            }
            set
            {
                this.OrginalTransactionStatusField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PUN
        {
            get
            {
                return this.PUNField;
            }
            set
            {
                this.PUNField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TransactionResponseDate
        {
            get
            {
                return this.TransactionResponseDateField;
            }
            set
            {
                this.TransactionResponseDateField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PaymentInquiryServiceResponse", Namespace="http://schemas.datacontract.org/2004/07/PMRWeb.PaymentRouter")]
    public partial class PaymentInquiryServiceResponse : PaymentServicePro.PaymentServiceResponse
    {
        
        private string CollectionCentreFeesField;
        
        private PaymentServicePro.InquiredService[] InquiryServicesField;
        
        private string TransactionAmountField;
        
        private string TransactionIdField;
        
        private string TransactionResponseDateField;
        
        private string eDirhamFeesField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CollectionCentreFees
        {
            get
            {
                return this.CollectionCentreFeesField;
            }
            set
            {
                this.CollectionCentreFeesField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public PaymentServicePro.InquiredService[] InquiryServices
        {
            get
            {
                return this.InquiryServicesField;
            }
            set
            {
                this.InquiryServicesField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TransactionAmount
        {
            get
            {
                return this.TransactionAmountField;
            }
            set
            {
                this.TransactionAmountField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TransactionId
        {
            get
            {
                return this.TransactionIdField;
            }
            set
            {
                this.TransactionIdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TransactionResponseDate
        {
            get
            {
                return this.TransactionResponseDateField;
            }
            set
            {
                this.TransactionResponseDateField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string eDirhamFees
        {
            get
            {
                return this.eDirhamFeesField;
            }
            set
            {
                this.eDirhamFeesField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PaymentGetStatusResponse", Namespace="http://schemas.datacontract.org/2004/07/PMRWeb.PaymentRouter")]
    public partial class PaymentGetStatusResponse : PaymentServicePro.PaymentServiceResponse
    {
        
        private string AmountField;
        
        private string CollectionCenterFeesField;
        
        private string EDirhamFeesField;
        
        private string EServiceDataField;
        
        private string InvoiceIDField;
        
        private string PUNField;
        
        private string TransactionResponseDateField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Amount
        {
            get
            {
                return this.AmountField;
            }
            set
            {
                this.AmountField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CollectionCenterFees
        {
            get
            {
                return this.CollectionCenterFeesField;
            }
            set
            {
                this.CollectionCenterFeesField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EDirhamFees
        {
            get
            {
                return this.EDirhamFeesField;
            }
            set
            {
                this.EDirhamFeesField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EServiceData
        {
            get
            {
                return this.EServiceDataField;
            }
            set
            {
                this.EServiceDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string InvoiceID
        {
            get
            {
                return this.InvoiceIDField;
            }
            set
            {
                this.InvoiceIDField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PUN
        {
            get
            {
                return this.PUNField;
            }
            set
            {
                this.PUNField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TransactionResponseDate
        {
            get
            {
                return this.TransactionResponseDateField;
            }
            set
            {
                this.TransactionResponseDateField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Runtime.Serialization.DataContractAttribute(Name="InquiredService", Namespace="http://schemas.datacontract.org/2004/07/PMRWeb.PaymentRouter")]
    public partial class InquiredService : object
    {
        
        private string AmountField;
        
        private string ArabicDescriptionField;
        
        private string EnglishDescriptionField;
        
        private string ServiceCodeField;
        
        private string TotalAmountField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Amount
        {
            get
            {
                return this.AmountField;
            }
            set
            {
                this.AmountField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ArabicDescription
        {
            get
            {
                return this.ArabicDescriptionField;
            }
            set
            {
                this.ArabicDescriptionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EnglishDescription
        {
            get
            {
                return this.EnglishDescriptionField;
            }
            set
            {
                this.EnglishDescriptionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ServiceCode
        {
            get
            {
                return this.ServiceCodeField;
            }
            set
            {
                this.ServiceCodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TotalAmount
        {
            get
            {
                return this.TotalAmountField;
            }
            set
            {
                this.TotalAmountField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PaymentServicePro.IPaymentService")]
    public interface IPaymentService
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPaymentService/GetPaymentStatus", ReplyAction="http://tempuri.org/IPaymentService/GetPaymentStatusResponse")]
        System.Threading.Tasks.Task<PaymentServicePro.PaymentGetStatusResponse> GetPaymentStatusAsync(string PurchaseId, string SecureHash, int entityID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPaymentService/Test", ReplyAction="http://tempuri.org/IPaymentService/TestResponse")]
        System.Threading.Tasks.Task<PaymentServicePro.PaymentGetStatusResponse> TestAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPaymentService/GetPaymentDirectStatus", ReplyAction="http://tempuri.org/IPaymentService/GetPaymentDirectStatusResponse")]
        System.Threading.Tasks.Task<PaymentServicePro.PaymentGetStatusDirectResponse> GetPaymentDirectStatusAsync(string PurchaseId, string SecureHash, int entityID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPaymentService/GeteDebitServiceDelivery", ReplyAction="http://tempuri.org/IPaymentService/GeteDebitServiceDeliveryResponse")]
        System.Threading.Tasks.Task<PaymentServicePro.PaymentGetStatusResponse> GeteDebitServiceDeliveryAsync(string PurchaseId, string SecureHash, int entityID, string DeliveryStatus);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPaymentService/CompletePayment", ReplyAction="http://tempuri.org/IPaymentService/CompletePaymentResponse")]
        System.Threading.Tasks.Task<PaymentServicePro.PaymentServiceResponse> CompletePaymentAsync(string PurchaseId, string SecureHash);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPaymentService/CancelPayment", ReplyAction="http://tempuri.org/IPaymentService/CancelPaymentResponse")]
        System.Threading.Tasks.Task<PaymentServicePro.PaymentServiceResponse> CancelPaymentAsync(string PurchaseId, string SecureHash);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPaymentService/RefundPayment", ReplyAction="http://tempuri.org/IPaymentService/RefundPaymentResponse")]
        System.Threading.Tasks.Task<PaymentServicePro.PaymentServiceResponse> RefundPaymentAsync(string PurchaseId, string SecureHash);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPaymentService/VoidPayment", ReplyAction="http://tempuri.org/IPaymentService/VoidPaymentResponse")]
        System.Threading.Tasks.Task<PaymentServicePro.PaymentServiceResponse> VoidPaymentAsync(string PurchaseId, string SecureHash);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPaymentService/InquirePayment", ReplyAction="http://tempuri.org/IPaymentService/InquirePaymentResponse")]
        System.Threading.Tasks.Task<PaymentServicePro.PaymentInquiryServiceResponse> InquirePaymentAsync(string PurchaseId, string IntendedEDirhamService, string[] ServicesToPay, string SecureHash);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public interface IPaymentServiceChannel : PaymentServicePro.IPaymentService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public partial class PaymentServiceClient : System.ServiceModel.ClientBase<PaymentServicePro.IPaymentService>, PaymentServicePro.IPaymentService
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public PaymentServiceClient() : 
                base(PaymentServiceClient.GetDefaultBinding(), PaymentServiceClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.SOAPEndPoint.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public PaymentServiceClient(EndpointConfiguration endpointConfiguration) : 
                base(PaymentServiceClient.GetBindingForEndpoint(endpointConfiguration), PaymentServiceClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public PaymentServiceClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(PaymentServiceClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public PaymentServiceClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(PaymentServiceClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public PaymentServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.Threading.Tasks.Task<PaymentServicePro.PaymentGetStatusResponse> GetPaymentStatusAsync(string PurchaseId, string SecureHash, int entityID)
        {
            return base.Channel.GetPaymentStatusAsync(PurchaseId, SecureHash, entityID);
        }
        
        public System.Threading.Tasks.Task<PaymentServicePro.PaymentGetStatusResponse> TestAsync()
        {
            return base.Channel.TestAsync();
        }
        
        public System.Threading.Tasks.Task<PaymentServicePro.PaymentGetStatusDirectResponse> GetPaymentDirectStatusAsync(string PurchaseId, string SecureHash, int entityID)
        {
            return base.Channel.GetPaymentDirectStatusAsync(PurchaseId, SecureHash, entityID);
        }
        
        public System.Threading.Tasks.Task<PaymentServicePro.PaymentGetStatusResponse> GeteDebitServiceDeliveryAsync(string PurchaseId, string SecureHash, int entityID, string DeliveryStatus)
        {
            return base.Channel.GeteDebitServiceDeliveryAsync(PurchaseId, SecureHash, entityID, DeliveryStatus);
        }
        
        public System.Threading.Tasks.Task<PaymentServicePro.PaymentServiceResponse> CompletePaymentAsync(string PurchaseId, string SecureHash)
        {
            return base.Channel.CompletePaymentAsync(PurchaseId, SecureHash);
        }
        
        public System.Threading.Tasks.Task<PaymentServicePro.PaymentServiceResponse> CancelPaymentAsync(string PurchaseId, string SecureHash)
        {
            return base.Channel.CancelPaymentAsync(PurchaseId, SecureHash);
        }
        
        public System.Threading.Tasks.Task<PaymentServicePro.PaymentServiceResponse> RefundPaymentAsync(string PurchaseId, string SecureHash)
        {
            return base.Channel.RefundPaymentAsync(PurchaseId, SecureHash);
        }
        
        public System.Threading.Tasks.Task<PaymentServicePro.PaymentServiceResponse> VoidPaymentAsync(string PurchaseId, string SecureHash)
        {
            return base.Channel.VoidPaymentAsync(PurchaseId, SecureHash);
        }
        
        public System.Threading.Tasks.Task<PaymentServicePro.PaymentInquiryServiceResponse> InquirePaymentAsync(string PurchaseId, string IntendedEDirhamService, string[] ServicesToPay, string SecureHash)
        {
            return base.Channel.InquirePaymentAsync(PurchaseId, IntendedEDirhamService, ServicesToPay, SecureHash);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.SOAPEndPoint))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                result.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.SOAPEndPoint))
            {
                return new System.ServiceModel.EndpointAddress("https://edg2pmrgw.moj.gov.ae/PMRWeb/PaymentRouter/PaymentService.svc/soap");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return PaymentServiceClient.GetBindingForEndpoint(EndpointConfiguration.SOAPEndPoint);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return PaymentServiceClient.GetEndpointAddress(EndpointConfiguration.SOAPEndPoint);
        }
        
        public enum EndpointConfiguration
        {
            
            SOAPEndPoint,
        }
    }
}
