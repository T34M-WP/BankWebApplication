using System;
using System.ComponentModel.DataAnnotations;

namespace BankWebApplication.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public int UserId { get; set; }  // Foreign keyฟ
        public User User { get; set; }    // Navigation property

        [StringLength(50)]
        public string Action { get; set; }

        [StringLength(50)]
        public string From { get; set; }

        public string To { get; set; }

        public decimal Amount { get; set; }
    }
}