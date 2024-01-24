using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Shared;
using WebApp.Shared.DTO;
using WebApp.Shared.Model;
using WebApp.Shared.Services;

namespace WebApp.Services
{
    public class TransactionService : ITransactionService
	{
        private readonly DataContext _dataContext;

        public TransactionService(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<Response<List<Transaction>>> GetAllTransactionsAsync()
        {
            var transactions = await _dataContext.Transactions.ToListAsync();
            
            return new Response<List<Transaction>>() {
                Data = transactions,
                Message = "Ok",
                Success = true
            };
        }

        public async Task<Response<bool>> DeleteTransactionAsync(int ID)
        {
            if (ID < 0)
            {
				return new Response<bool>()
				{
					Data = false,
                    Message = "Transaction couldn't be deleted - ID cannot be < 0",
                    Success = false
				};
            }
			var res = new Response<bool>();

			var sameIDTransaction = await _dataContext.Transactions.FirstOrDefaultAsync(b => b.Id == ID);
            if (sameIDTransaction != null)
            {
                _dataContext.Transactions.Remove(sameIDTransaction);
                await _dataContext.SaveChangesAsync();
                res.Success = true;
                res.Message = "Transaction deleted successfully";
            }
            else
            {
                res.Success = false;
                res.Message = "Cannot find transaction to delete";
            }

            return res;
        }

        public async Task<Response<Transaction>> GetTransactionAsync(int ID)
        {
            if (ID < 0)
            {
                return new Response<Transaction>()
                {
                    Data = null,
                    Message = "ID cannot be < 0",
                    Success = false
                };
            }

            var sameIDTransaction = await _dataContext.Transactions.FirstOrDefaultAsync(b => b.Id == ID);
            if (sameIDTransaction == null)
            {
                return new Response<Transaction>()
                {
                    Data = null,
                    Message = $"Couldn't find transaction with ID: {ID}",
                    Success = false
                };
            }
            else
            {
                return new Response<Transaction>()
                {
                    Data = sameIDTransaction,
                    Message = "Ok",
                    Success = true
                };
            }
        }

        public async Task<Response<Transaction>> AddTransactionAsync(TransactionDTO transaction)
        {
            var res = new Response<Transaction>()
            {
                Data = null,
                Message = "",
                Success = false
            };

            var sameIDTransaction = await _dataContext.Transactions.FirstOrDefaultAsync(u => u.Id == transaction.Id);

            if (sameIDTransaction != null)
                res.Message = "Found Transaction with same ID, ID must be unique!";
            if (transaction == null)
                res.Message = "Message couldn't be added - user is null";
            else if (string.IsNullOrEmpty(transaction.Title))
                res.Message = "Transaction couldn't be added - Title is empty";
            else if (string.IsNullOrEmpty(transaction.Content))
                res.Message = "Transaction couldn't be added - Content is empty";
            else if (transaction.Value <= 0)
                res.Message = "Transaction couldn't be added - Value cannot be < 0";

            if (res.Message != "")
                return res;

            try
            {
				Transaction trans = new Transaction();
				trans.Title = transaction.Title;
				trans.Content = transaction.Content;
				trans.Date = transaction.Date;
				trans.SenderId = transaction.SenderId;
				trans.RecieverId = transaction.RecieverId;
				trans.Sender = _dataContext.Users.ElementAt(trans.SenderId);
				trans.Reciever = _dataContext.Users.ElementAt(trans.RecieverId);

                _dataContext.Transactions.Add(trans);
                await _dataContext.SaveChangesAsync();

                res.Data = trans;
                res.Success = true;
                res.Message = "Transaction added successfully";
            }
            catch (Exception ex)
            {
                res.Message = "Error while adding transaction: " + ex.Message;
            }

            return res;
        }

        public async Task<Response<Transaction>> UpdateTransactionAsync(Transaction transaction)
        {
            var res = new Response<Transaction>()
            {
                Data = null,
                Message = "",
                Success = false
            };
            

            var sameIDTransaction = await _dataContext.Transactions.FirstOrDefaultAsync(u => u.Id == transaction.Id);

            if (sameIDTransaction == null)
                res.Message = "Cannot find with same ID, cannot update not existing transaction";
            if (transaction == null)
                res.Message = "Message couldn't be added - user is null";
            else if (string.IsNullOrEmpty(transaction.Title))
                res.Message = "Transaction couldn't be added - Title is empty";
            else if (string.IsNullOrEmpty(transaction.Content))
                res.Message = "Transaction couldn't be added - Content is empty";
            else if (transaction.Value <= 0)
                res.Message = "Transaction couldn't be added - Value cannot be < 0";

            if (res.Message != "")
                return res;

            if (sameIDTransaction != null)
            {
				sameIDTransaction.Title = transaction.Title;
				sameIDTransaction.Content = transaction.Content;
				sameIDTransaction.Date = transaction.Date;
				sameIDTransaction.Reciever = transaction.Reciever;
				sameIDTransaction.RecieverId = transaction.RecieverId;
				sameIDTransaction.Sender = transaction.Sender;
				sameIDTransaction.SenderId = transaction.SenderId;
                await _dataContext.SaveChangesAsync();

                res.Success = true;
                res.Message = "Transaction updated successfully";
            }
            return res;
        }

        public async Task<Response<List<Transaction>>> GetUserTransactionsAsync(int ID)
        {
            if (ID < 0)
            {
				return new Response<List<Transaction>>() {
					Data = null,
					Message = "Transaction couldn't be fetched - User ID cannot be < 0",
					Success = false
				};
            }

            List<Transaction> allUserTransactions = new List<Transaction>();
            var sameIDTransactionSender = _dataContext.Transactions.Where(b => b.SenderId == ID).ToList();
            var sameIDTransactionReciever = _dataContext.Transactions.Where(b => b.RecieverId == ID).ToList();

            allUserTransactions.AddRange(sameIDTransactionSender);
            allUserTransactions.AddRange(sameIDTransactionReciever);

			if (allUserTransactions == null)
            {
                return new Response<List<Transaction>>()
                {
                    Data = null,
                    Message = $"Couldn't find transaction with ID: {ID}",
                    Success = false
                };
            }
            else
            {
                return new Response<List<Transaction>>()
                {
                    Data = allUserTransactions,
                    Message = "Ok",
                    Success = true
                };
            }
        }
    }
}
