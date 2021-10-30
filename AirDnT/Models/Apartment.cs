using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirDnT.Models
{
    public class Apartment
    {
        public int ApartmentId { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please enter property name")]
        [Display(Name = "Property name")]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "Please enter the price")]
        [Range(1, 1000000, ErrorMessage = "Price has to be between 1 to 1M")]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Please enter the amount of the rooms")]
        [Range(1,10, ErrorMessage = "The number of rooms has to be between 1 to 10")]
        [Display(Name = "Number of rooms")]
        public int RoomsNumber { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Enter start date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Start date")]
        public DateTime sAvailability { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Enter end date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "End date")]
        public DateTime eAvailability { get; set; }

        public int OwnerId { get; set; }

        public Owner Owner { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }

        public virtual ApartmentAddress Address { get; set; }
    }
}
