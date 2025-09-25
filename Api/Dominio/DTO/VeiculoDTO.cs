using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.DTO
{
    public record VehiclesDTO
    {
        public string Name { get; set; } = default!;
        public string Mark { get; set; } = default!;
        public int Year { get; set; }
    }
}