﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirDnT.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNum { get; set; }

        public string Email { get; set; }

        public List<Apartment> apartments_inventations { get; set; }

    }
}
