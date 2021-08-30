﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AirDnT.Models
{
    public class Owner
    {

        public int OwnerId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNum { get; set; }

        public string Email { get; set; }

        public virtual User User { get; set; }

        public List<Apartment> apartments { get; set; }

    }
}
