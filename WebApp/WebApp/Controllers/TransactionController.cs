using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.Shared.Model;
using WebApp.Shared;
using WebApp.Shared.Services;
using WebApp.Shared.DTO;

namespace WebApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
			_transactionService = transactionService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<Response<List<Transaction>>>> GetAll()
        {
            var result = await _transactionService.GetAllTransactionsAsync();

            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }

        [HttpGet("Get/{ID}")]
        public async Task<ActionResult<Response<Transaction>>> Get(int ID)
        {
            var result = await _transactionService.GetTransactionAsync(ID);

            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }

        [HttpPost("Add")]
        public async Task<ActionResult<Response<Transaction>>> Add([FromBody] TransactionDTO message)
        {
            var result = await _transactionService.AddTransactionAsync(message);

            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }

        [HttpDelete("Delete/{ID}")]
        public async Task<ActionResult<Response<Transaction>>> Delete([FromBody] int ID)
        {
            var result = await _transactionService.DeleteTransactionAsync(ID);

            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }

        [HttpPut("Update")]
        public async Task<ActionResult<Response<Transaction>>> Update([FromBody] Transaction message)
        {
            var result = await _transactionService.UpdateTransactionAsync(message);

            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }

        [HttpGet("GetUserMsg/{ID}")]
        public async Task<ActionResult<Response<Transaction>>> GetUserMsg(int ID)
        {
            var result = await _transactionService.GetUserTransactionsAsync(ID);

            if (result.Success)
                return Ok(result);
            else
                return StatusCode(500, $"Internal server error {result.Message}");
        }
    }
}
