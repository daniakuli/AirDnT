using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirDnT.Models
{
    public class Apartment
    {

        public int Id { get; set; }
        public string Address { get; set; }

        public double Price { get; set; }

        public DateTime Availability { get; set; }

        public Owner owner { get; set; }

        public List<Customer> history_customers { get; set; }
       
    }
}
