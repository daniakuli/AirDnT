using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirDnT.Models
{
    public enum UserType
    {
        Admin,
        Customer,
        Owner
    }
    public class User
    {
        [Key]
        [Required]
        [RegularExpression("(^[a-zA-Z0-9]{2,15}$)", ErrorMessage = "Should Contain alphabet and numeric characters")]
        [StringLength(15)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public UserType Type { get; set; } = UserType.Customer;
    }
}
