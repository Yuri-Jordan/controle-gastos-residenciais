using System;

namespace ControleGastosResidenciais.Core.Entities
{
    /// <summary>
    /// Representa uma pessoa no sistema que pode ter transações financeiras
    /// </summary>
    public class Pessoa
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        /// <summary>
        /// Nome completo da pessoa - comprimento máximo de 200 caracteres
        /// </summary>
        public string Nome { get; set; } = string.Empty;
        
        /// <summary>
        /// Idade da pessoa - usada para validação de regras de negócio (menores só podem criar despesas)
        /// </summary>
        public int Idade { get; set; }
        
        /// <summary>
        /// Propriedade de navegação para todas as transações associadas a esta pessoa
        /// </summary>
        public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
        
        /// <summary>
        /// Propriedade auxiliar para verificar se a pessoa é menor de idade (idade < 18)
        /// </summary>
        public bool EhMenorDeIdade => Idade < 18;
    }
}