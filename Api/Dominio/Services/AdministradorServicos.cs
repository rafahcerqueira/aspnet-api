using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTO;
using minimal_api.Domain.Entities;
using minimal_api.Infrastructure.DataBase;
using minimal_api.Infrastructure.Interfaces;

namespace minimal_api.Domain.Services
{
    public class AdministratorServices : IAdministratorServices
    {
        private readonly DatabaseContext _context;

        public AdministratorServices(DatabaseContext context) => _context = context;

        public Administrator Include(Administrator Administrator)
        {
            _context.Administrators.Add(Administrator);
            _context.SaveChanges();

            return Administrator;
        }

        public List<Administrator> ListAdministrators(int page = 1)
        {
            return _context.Administrators.Skip((page - 1) * 10).Take(10).ToList();
        }

        public Administrator? Login(LoginDTO loginDTO)
        {
            var adm = _context.Administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
            return adm;
        }
    }
}