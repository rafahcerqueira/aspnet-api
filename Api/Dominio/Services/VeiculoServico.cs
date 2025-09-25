using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Infrastructure.DataBase;

namespace minimal_api.Domain.Services
{
    public class VehicleServices : IVehicleService
    {
        private readonly DatabaseContext _context;

        public VehicleServices(DatabaseContext context) => _context = context;

        public void DeleteVehicle(string id)
        {
            var Vehicle = _context.Vehicles.Find(id);
            if (Vehicle != null)
                _context.Vehicles.Remove(Vehicle);
            _context.SaveChanges();
        }

        public void UpdateVehicle(Vehicle Vehicle)
        {
            _context.Vehicles.Update(Vehicle);
            _context.SaveChanges();
        }

        public Vehicle? GetById(string id) => _context.Vehicles.Find(id);

        public void IncludeVehicle(Vehicle Vehicle)
        {
            _context.Vehicles.Add(Vehicle);
            _context.SaveChanges();
        }

        public List<Vehicle> ListVehicles(int? page = 1, string? Name = null, string? Mark = null)
        {
            var query = _context.Vehicles.AsQueryable();
            
            if (!string.IsNullOrEmpty(Name))
                query = query.Where(v => EF.Functions.Like(v.Name.ToLower(), $"%{Name}%"));

            int itemsPerPage = 10;

            if (page != null)
                query = query.Skip(((int)page - 1) * itemsPerPage).Take(itemsPerPage);

            return query.ToList();
        }
    }
}