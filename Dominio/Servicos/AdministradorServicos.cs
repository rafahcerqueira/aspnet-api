using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTO;
using minimal_api.Dominio.Entidades;
using minimal_api.Infraestrutura.DataBase;
using minimal_api.Infraestrutura.Interfaces;

namespace minimal_api.Dominio.Servicos
{
    public class AdministradorServicos : IAdministradorServicos
    {
        private readonly DbContexto _contexto;

        public AdministradorServicos(DbContexto contexto) => _contexto = contexto;

        public Administrador? Login(LoginDTO loginDTO)
        {
            var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
            return adm;
        }
    }
}