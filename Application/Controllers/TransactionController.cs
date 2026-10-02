using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using GDB.Api.Application.Dtos;
using GDB.Api.Application.Services.Contracts;
using GDB.Api.Common.Constants;
using GDB.Api.Domain.Enums;


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
        public async Task<IActionResult> Deposit(
            [FromBody] TransactionDto transactionDto)
        {
            if (transactionDto == null)
                return BadRequest("Transaction data is required.");

            if (string.IsNullOrWhiteSpace(transactionDto.AccountNumber))
                return BadRequest("Account number is required.");

            if (transactionDto.Amount <= 0)
                return BadRequest("Amount must be greater than zero.");

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
        public async Task<IActionResult> Withdraw(
            [FromBody] TransactionDto transactionDto)
        {
            if (transactionDto == null)
                return BadRequest("Transaction data is required.");

            if (string.IsNullOrWhiteSpace(transactionDto.AccountNumber))
                return BadRequest("Account number is required.");

            if (transactionDto.Amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            if(transactionDto.Pin == null || transactionDto.Pin.Length != 4)
                return BadRequest("PIN must be a 4-digit number.");

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
        public async Task<IActionResult> Transfer(
            [FromBody] TransactionDto transactionDto)
        {
            if (transactionDto == null)
                return BadRequest("Transaction data is required.");

            if (string.IsNullOrWhiteSpace(transactionDto.FromAccount))
                return BadRequest("From Account number is required.");

            if (string.IsNullOrWhiteSpace(transactionDto.ToAccount))
                return BadRequest("To Account number is required.");

            if (transactionDto.Amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            if (transactionDto.Pin == null || transactionDto.Pin.Length != 4)
                return BadRequest("PIN must be a 4-digit number.");


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


    }
}
