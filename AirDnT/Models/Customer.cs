using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AirDnT.Models
{
    public class Customer
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Please enter first name")]
        [StringLength(15, ErrorMessage = "First name can't contain more then 15 characters")]
        [RegularExpression("(^[a-zA-Z]{2,15}$)", ErrorMessage = "Should Contain alphabet characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        [StringLength(15, ErrorMessage = "Last name can't contain more then 15 characters")]
        [RegularExpression("(^[a-zA-Z]{2,15}$)", ErrorMessage = "Should Contain alphabet characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter phone number")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Wrong mobile")]
        [Display(Name = "Phone Number")]
        public string PhoneNum { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter e-mail address")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        public string UserName { get; set; }

        public virtual ICollection<Apartment> Apartments_inventations { get; set; }

    }
}
