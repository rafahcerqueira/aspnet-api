using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Interfaces
{
    public interface IVeiculoServico
    {
        List<Veiculo> ListarVeiculos(int pagina = 1, string? nome = null, string? marca = null);
        Veiculo? BuscaPorId(string id);
        void IncluirVeiculo(Veiculo veiculo);
        void AtualizarVeiculo(Veiculo veiculo);
        void ApagarVeiculo(Veiculo veiculo);
    }
}