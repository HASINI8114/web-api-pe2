using Asp.Versioning;
using GDB.Api.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GDB.Api.Common.Constants;
using GDB.Api.Application.Dtos.Request;

namespace GDB.Api.Application.Controllers
{
    [ApiVersion(ApiConstants.Version1)]
    [Route(ApiConstants.BaseAccounts)]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITransactionQueryService _transactionQueryService;

        public AccountController(
            IAccountService accountService,
            ITransactionQueryService transactionQueryService)
        {
            _accountService = accountService;
            _transactionQueryService = transactionQueryService;
        }

        // GET: api/v1/accounts
        [HttpGet]
        public async Task<IActionResult> GetAllAccountsAsync()
        {
            var accounts = await _accountService.GetAllAccountsAsync();

            return Ok(accounts);
        }

        // GET: api/v1/accounts/{accNo}
        [HttpGet(ApiConstants.AccountByNumber)]
        public async Task<IActionResult> GetAccountAsync(string accNo)
        {
            var account = await _accountService.GetAccountAsync(accNo);

            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        // GET: api/v1/accounts/{accNo}/balance
        [HttpGet(ApiConstants.AccountBalance)]
        public async Task<IActionResult> GetBalanceAsync(string accNo)
        {
            var balance = await _accountService.GetBalanceAsync(accNo);

            return Ok(balance);
        }

        // GET: api/v1/accounts/{accNo}/view
        [HttpGet(ApiConstants.AccountView)]
        public async Task<IActionResult> ViewAccountAsync(string accNo)
        {
            var account = await _accountService.ViewAccountAsync(accNo);

            return Ok(account);
        }

        // GET: api/v1/accounts/{accNo}/transactions
        [HttpGet(ApiConstants.AccountTransactions)]
        public async Task<IActionResult> GetRecentTransactionsAsync(string accNo)
        {
            if (string.IsNullOrWhiteSpace(accNo))
                return BadRequest("Account number is required.");

            var transactions = await _transactionQueryService.GetRecentTransactionsAsync(accNo);

            return Ok(transactions);
        }

        // POST: api/v1/accounts
        [HttpPost]
        public async Task<IActionResult> CreateAccountAsync(CreateAccountRequestDto request)
        {
            var account = await _accountService.CreateAccountAsync(request);

            return Ok(account);
        }

        // POST: api/v1/accounts/close
        [HttpPost(ApiConstants.CloseAccount)]
        public async Task<IActionResult> CloseAccountAsync(CloseAccountRequestDto request)
        {
            var account = await _accountService.CloseAccountAsync(request);

            return Ok(account);
        }
    }
}
