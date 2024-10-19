using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Domain.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<State> States { get; set; } = new List<State>();
    }

    public class State
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
    }
}
