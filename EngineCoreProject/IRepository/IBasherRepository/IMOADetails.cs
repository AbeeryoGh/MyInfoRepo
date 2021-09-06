using EngineCoreProject.DTOs.BasherDto;
using EngineCoreProject.Models;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace EngineCoreProject.IRepository.IBasherRepository
{
    [ServiceContract(Namespace = "https://enotary.moj.gov.ae/enotary/MOADetails")]
    public interface IMOADetails
    {

        [OperationContract]
        Task<sendMOADetailsMOJResponse> SendMOADetails_MOJ(SendAppMOADetails_MOJ sendMOADetails_MOJRequest);
    }
}
