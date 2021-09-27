using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirDnT.Models
{
    public class GroupedApartment <K, T>
    {
        public K Rooms { get; set; }

        public IEnumerable<T> apartments { get; set; }
    }
}
