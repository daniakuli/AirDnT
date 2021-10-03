using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AirDnT.Models
{
    public class ApartmentAddress
    {
        [ForeignKey("Apartment")]
        public int ApartmentAddressId { get; set; }

        [Required(ErrorMessage = "Please enter counrty")]
        [StringLength(20, ErrorMessage = "The country can contain only 20 characters")]
        [Display(Name = "Counrty")]
        public string Country { get; set; }

        [StringLength(20, ErrorMessage = "The city can contain only 20 characters")]
        [Required(ErrorMessage = "Please enter city")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter Street address")]
        [StringLength(25, ErrorMessage = "The Street address can contain only 25 characters")]
        [Display(Name = "Street Address")]
        public string StreetName { get; set; }

        [Required(ErrorMessage = "Please enter zip code")]
        [StringLength(10, ErrorMessage = "The zip code can contain only 10 characters")]
        [Display(Name = "Zip code")]
        public string Zip { get; set; }

        public virtual Apartment Apartment { get; set; }

    }
}
