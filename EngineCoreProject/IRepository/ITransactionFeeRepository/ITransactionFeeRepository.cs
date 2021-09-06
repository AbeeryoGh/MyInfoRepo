using EngineCoreProject.DTOs.TransactionFeeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.ITransactionFeeRepository
{
    public interface ITransactionFeeRepository
    {
        /// <summary>
        /// Add a transaction fee
        /// </summary>
        /// <param name="transactionFeePostDto">Object contains the proprieties of the fee.
        /// </param>
        /// <param name="lang"></param>
        /// <returns></returns>
        Task<int> AddTransactionFee(TransactionFeePostDto transactionFeePostDto, string lang);

        /// <summary>
        /// Update certain fee by it's row Id.
        /// </summary>
        /// <param name="transactionFeePostDto">the fee object contains the new values</param>
        /// <param name="rowId"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        Task<int> UpdateTransactionFee(TransactionFeePostDto transactionFeePostDto, int rowId, string lang);

        /// <summary>
        /// Delete certain fee by it's row Id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        Task<int> DeleteTransactionFee(int id, string lang);

        /// <summary>
        /// Get all the transaction fees of the system for all services.
        /// </summary>
        /// <returns></returns>
        Task<List<TransactionFeeGetDto>> GetTransactionFees();

        /// <summary>
        /// Calculate the fee for a certain transaction.
        /// </summary>
        /// <param name="transactionFeeInput">object contains the inputs of the 
        /// ServiceNo,
        /// DocumentKind( 1 for CONTRACTOREDITOR, 2 for  AGENCY, 3 for both)
        /// ProcessKind (1 for EDITING , 2 for  CONFIRM, 3 for both), Quantity (parties or pages count)
        /// Amount the contract value.
        /// </param>
        /// <param name="lang"></param>
        /// <returns>A list of fees </returns>
        Task<List<TransactionFeeOutput>> CalculateTransactionFee(TransactionFeeInput transactionFeeInput, string lang);

    }
}
