using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.Shared.DTO;
using WebApp.Shared.Model;

namespace WebApp.Shared.Services
{
    public interface ITransactionService
    {
        Task<Response<List<Transaction>>> GetAllTransactionsAsync();
        Task<Response<bool>> DeleteTransactionAsync(int ID);
        Task<Response<Transaction>> GetTransactionAsync(int ID);
        Task<Response<Transaction>> AddTransactionAsync(TransactionDTO transaction);
        Task<Response<Transaction>> UpdateTransactionAsync(Transaction transaction);
        Task<Response<List<Transaction>>> GetUserTransactionsAsync(int ID);
    }
}
