using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetoUpBrasil.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string NomeCliente { get; set; }
        public string Descricao { get; set; }
        public int Ativo { get; set; }
        public DateTime DtCadastro { get; set; }
    }
}