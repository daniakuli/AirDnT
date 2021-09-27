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
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public UserType Type { get; set; } = UserType.Customer;
    }
}
