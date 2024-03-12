using System.Threading.Tasks;
using BankWebApplication.Data;
using BankWebApplication.Models;

public class TransactionService
{
    private readonly AppDbContext _dbContext;

    public TransactionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Deposit(Transaction transaction)
    {
        // Update user balance
        var user = await _dbContext.Users.FindAsync(transaction.UserId);
        user.Balance += transaction.Amount;

        // Add transaction record
        _dbContext.Transactions.Add(transaction);

        // Save changes to database
        await _dbContext.SaveChangesAsync();
    }

    // Other methods for withdraw, transfer, etc.
}
