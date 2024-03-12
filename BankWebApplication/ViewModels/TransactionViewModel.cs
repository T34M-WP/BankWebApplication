using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BankWebApplication.Models
{
    public class TransactionViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public decimal Balance { get; set; }
        public List<Transaction> Transactions { get; set; }

        // เพิ่ม properties อื่น ๆ ตามต้องการ
    }
}
