using System;

namespace ControleGastosResidenciais.Core.DTOs
{
    public class PessoaDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
        public bool EhMenorDeIdade { get; set; }
    }

    public class CriarPessoaDto
    {
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
    }

    public class AtualizarPessoaDto
    {
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
    }
}