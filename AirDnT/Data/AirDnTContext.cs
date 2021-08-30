using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirDnT.Models;

namespace AirDnT.Data
{
    public class AirDnTContext : DbContext
    {
        public AirDnTContext (DbContextOptions<AirDnTContext> options)
            : base(options)
        {
        }

        public DbSet<AirDnT.Models.Apartment> Apartment { get; set; }

        public DbSet<AirDnT.Models.Customer> Customer { get; set; }

        public DbSet<AirDnT.Models.Owner> Owner { get; set; }

        public DbSet<AirDnT.Models.User> User { get; set; }
    }
}
