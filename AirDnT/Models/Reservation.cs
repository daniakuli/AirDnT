using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AirDnT.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationID { get; set; }

        public int CustomerID { get; set; }

        public int ApartmentID { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Start date")]
        public DateTime sAvailability { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "End date")]
        public DateTime eAvailability { get; set; }

        public virtual Customer Customers { get; set; }

        public virtual Apartment Apartment { get; set; }
    }
}
