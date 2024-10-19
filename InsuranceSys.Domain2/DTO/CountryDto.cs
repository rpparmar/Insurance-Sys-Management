using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Domain.DTO
{
    public class CountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public ICollection<State> States { get; set; } = new List<State>();
    }

    public class StateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
    }
}
