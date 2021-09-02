using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirDnT.Models
{
    public class ApartmentAddress
    {
        [ForeignKey("Apartment")]
        public int ApartmentAddressId { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string StreetName { get; set; }

        public string Zip { get; set; }

        public virtual Apartment Apartment { get; set; }

    }
}
