using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BankWebApplication.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public decimal Balance { get; set; }
   
    }
}
