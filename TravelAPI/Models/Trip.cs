
using System;
using System.Collections.Generic;

namespace TravelAPI.Models
{
    public class Trip
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int MaxPeople { get; set; }
        public ICollection<Client> Clients { get; set; }
        public ICollection<Country> Countries { get; set; }
    }
}
