using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.ModelViews
{
    public struct Home
    {
        public string Documentacao { get => "/swagger"; }

        public string Mensagem { get => "API de veículos - Minimal API"; }
    }
}