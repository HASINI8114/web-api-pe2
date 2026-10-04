using Asp.Versioning;
using GDB.Api.Application.Dtos.Request;
using GDB.Api.Application.Dtos.Response;
using GDB.Api.Application.Services.Contracts;
using GDB.Api.Common.Constants;
using GDB.Api.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace GDB.Api.Application.Controllers
{
    [ApiVersion(ApiConstants.Version1)]
    [Route(ApiConstants.BaseTransactions)]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // POST: api/v1/transactions/deposit
        [HttpPost(ApiConstants.Deposit)]
        public async Task<IActionResult> DepositAsync(
            [FromBody] TransactionDto transactionDto)
        {
            if (transactionDto == null)
                return BadRequest("Transaction data is required.");

            if (string.IsNullOrWhiteSpace(transactionDto.AccountNumber))
                return BadRequest("Account number is required.");

            var result =
                await _transactionService.ProcessTransactionAsync<DepositResponseDto>(
                    transactionDto,
                    TransactionType.Deposit);

            if (result == null)
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Deposit failed.");

            return Ok(result);
        }

        // POST: api/v1/transactions/withdraw
        [HttpPost(ApiConstants.Withdraw)]
        public async Task<IActionResult> WithdrawAsync(
            [FromBody] TransactionDto transactionDto)
        {
            if (transactionDto == null)
                return BadRequest("Transaction data is required.");

            if (string.IsNullOrWhiteSpace(transactionDto.AccountNumber))
                return BadRequest("From Account number is required.");

            if(string.IsNullOrWhiteSpace(transactionDto.Pin))
                return BadRequest("PIN is required.");

            var result =
                await _transactionService.ProcessTransactionAsync<WithdrawResponseDto>(
                    transactionDto,
                    TransactionType.Withdraw);

            if (result == null)
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Withdrawal failed.");

            return Ok(result);
        }

        // POST: api/v1/transactions/transfer
        [HttpPost(ApiConstants.Transfer)]
        public async Task<IActionResult> TransferAsync(
            [FromBody] TransactionDto transactionDto)
        {
            if (transactionDto == null)
                return BadRequest("Transaction data is required.");

            if (string.IsNullOrWhiteSpace(transactionDto.FromAccount))
                return BadRequest("From Account number is required.");

            if (string.IsNullOrWhiteSpace(transactionDto.ToAccount))
                return BadRequest("To Account number is required.");

            if (string.IsNullOrWhiteSpace(transactionDto.Pin))
                return BadRequest("PIN is required.");


            var result =
                await _transactionService.ProcessTransactionAsync<TranferFundsResponseDto>(
                    transactionDto,
                    TransactionType.Transfer);

            if (result == null)
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Transfer failed.");

            return Ok(result);
        }

        // GET: api/v1/transactions/recent
        // GET /api/v1/transactions/recent?accountNumber=1000001001&pageNumber=1&pageSize=10
        [HttpGet(ApiConstants.RecentTransactions)]
        public async Task<IActionResult> GetRecentTransactionsAsync(
            [FromQuery] ViewRecentTransactionsRequestDto requestDto)
        {

            var transactions = await _transactionService.GetRecentTransactionsAsync(requestDto);

            return Ok(transactions);
        }   
    }
}
