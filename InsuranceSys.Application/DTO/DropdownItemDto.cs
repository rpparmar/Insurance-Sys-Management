using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Application.DTO
{
    public class DropdownItemDto
    {
        public string Value { get; set; } = default!;
        public string Text { get; set; } = default!;
        public bool Selected { get; set; } = false;
    }
}
