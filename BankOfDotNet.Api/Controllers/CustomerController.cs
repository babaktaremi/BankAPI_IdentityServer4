using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankOfDotNet.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BankOfDotNet.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly BankContext _bankContext;

        public CustomerController(BankContext bankContext)
        {
            _bankContext = bankContext;
        }


        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> Create(Customer model)
        {
            _bankContext.Customers.Add(model);

            await _bankContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _bankContext.Customers.ToArrayAsync());
        }
    }
}
