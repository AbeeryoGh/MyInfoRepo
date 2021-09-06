using EngineCoreProject.DTOs.BasherDto;
using EngineCoreProject.Models;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace EngineCoreProject.IRepository.IBasherRepository
{
    [ServiceContract(Namespace = "https://enotary.moj.gov.ae/enotary/notaryFee")]
    public interface INotaryFee
    {
        /// <summary>
        /// Calculate the fees for Basher service.
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns>a list of fees accord to the inputs.</returns>
        /// <error>SUC100 Success, ERR101 Un-Expected Error, ERR102 Invalid/Missing Parameter, ERR103 Field Length is exceeded</error>
        [OperationContract]
        Task<retrieveNotaryFees_MOJResponse> RetrieveNotaryFees_MOJ(RetrieveNotaryFeesMOJRequest retrieveNotaryFees_MOJRequest);
    }
}
