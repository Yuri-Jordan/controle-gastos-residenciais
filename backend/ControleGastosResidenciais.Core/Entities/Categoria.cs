using System;
using ControleGastosResidenciais.Core.Enums;

namespace ControleGastosResidenciais.Core.Entities
{
    /// <summary>
    /// Representa uma categoria para classificar transações
    /// </summary>
    public class Categoria
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        /// <summary>
        /// Descrição da categoria - comprimento máximo de 400 caracteres
        /// </summary>
        public string Descricao { get; set; } = string.Empty;
        
        /// <summary>
        /// Determina com quais tipos de transação esta categoria pode ser usada
        /// Despesa: Apenas para transações de despesa
        /// Receita: Apenas para transações de receita
        /// Ambos: Pode ser usada para ambos os tipos
        /// </summary>
        public FinalidadeCategoria Finalidade { get; set; }
        
        /// <summary>
        /// Propriedade de navegação para todas as transações que utilizam esta categoria
        /// </summary>
        public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }
}