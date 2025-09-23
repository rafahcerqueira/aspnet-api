using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.DataBase;

namespace minimal_api.Dominio.Servicos
{
    public class VeiculoServicos : IVeiculoServico
    {
        private readonly DbContexto _contexto;

        public VeiculoServicos(DbContexto contexto) => _contexto = contexto;

        public void ApagarVeiculo(string id)
        {
            var veiculo = _contexto.Veiculos.Find(id);
            if (veiculo != null)
                _contexto.Veiculos.Remove(veiculo);
            _contexto.SaveChanges();
        }

        public void AtualizarVeiculo(Veiculo veiculo)
        {
            _contexto.Veiculos.Update(veiculo);
            _contexto.SaveChanges();
        }

        public Veiculo? BuscaPorId(string id) => _contexto.Veiculos.Find(id);

        public void IncluirVeiculo(Veiculo veiculo)
        {
            _contexto.Veiculos.Add(veiculo);
            _contexto.SaveChanges();
        }

        public List<Veiculo> ListarVeiculos(int? pagina = 1, string? nome = null, string? marca = null)
        {
            var query = _contexto.Veiculos.AsQueryable();
            
            if (!string.IsNullOrEmpty(nome))
                query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome}%"));

            int itensPorPagina = 10;

            if (pagina != null)
                query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);

            return query.ToList();
        }
    }
}