using GDB.Api.Application.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GDB.Api.Application.Dtos;

namespace GDB.Api.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult GetAllAccounts()
        {
            var accounts = _accountService.GetAllAccounts();

            return Ok(accounts);
        }

        [HttpGet("{accNo}")]
        public async Task<IActionResult> GetAccountAsync(string accNo)
        {
            var account = await _accountService.GetAccountAsync(accNo);

            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        [HttpGet("{accNo}/balance")] 
        public async Task<IActionResult> GetBalanceAsync(string accNo) 
        { 
            var balance = await _accountService.GetBalanceAsync(accNo);
            
            return Ok(balance); 
        }
        [HttpGet("{accNo}/view")] 
        public async Task<IActionResult> ViewAccountAsync(string accNo) 
        { 
            var account = await _accountService.ViewAccountAsync(accNo); 
            return Ok(account); 
        }
        [HttpPost] public IActionResult CreateAccount(CreateAccountRequestDto request) 
        { 
            var account = _accountService.CreateAccount(request); 
            return Ok(account); 
        }
        [HttpPost("close")] public async Task<IActionResult> CloseAccountAsync(CloseAccountRequestDto request) 
        { 
            var account = await _accountService.CloseAccountAsync(request); 
            return Ok(account); 
        }
    }
}
