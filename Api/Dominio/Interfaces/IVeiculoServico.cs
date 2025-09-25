using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.Entities;

namespace minimal_api.Domain.Interfaces
{
    public interface IVehicleService
    {
        List<Vehicle> ListVehicles(int? page = 1, string? Name = null, string? Mark = null);
        Vehicle? GetById(string id);
        void IncludeVehicle(Vehicle vehicle);
        void UpdateVehicle(Vehicle vehicle);
        void DeleteVehicle(string id);
    }
}