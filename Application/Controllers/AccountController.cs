using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GDB.Api.Application.Services.Contracts;

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
    }
}
