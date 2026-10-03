using Asp.Versioning;
using GDB.Api.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GDB.Api.Application.Dtos;
using GDB.Api.Common.Constants;

namespace GDB.Api.Application.Controllers
{
    [ApiVersion(ApiConstants.Version1)]
    [Route(ApiConstants.BaseAccounts)]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
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
