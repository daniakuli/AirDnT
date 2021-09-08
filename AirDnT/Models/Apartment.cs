using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirDnT.Models
{
    public class Apartment
    {
        public int ApartmentId { get; set; }

        public string DisplayName { get; set; }

        public double Price { get; set; }

        public int RoomsNumber { get; set; }

        public DateTime Availability { get; set; }

        public int OwnerId { get; set; }

        public Owner Owner { get; set; }

        public ICollection<Customer> history_customers { get; set; }

        public virtual ApartmentAddress Address { get; set; }
    }
}
