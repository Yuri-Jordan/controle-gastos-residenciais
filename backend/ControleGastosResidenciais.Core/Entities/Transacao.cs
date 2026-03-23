using System;
using ControleGastosResidenciais.Core.Enums;

namespace ControleGastosResidenciais.Core.Entities
{
    /// <summary>
    /// Representa uma transação financeira (despesa ou receita)
    /// </summary>
    public class Transacao
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        /// <summary>
        /// Descrição da transação - comprimento máximo de 400 caracteres
        /// </summary>
        public string Descricao { get; set; } = string.Empty;
        
        /// <summary>
        /// Valor da transação - deve ser positivo
        /// </summary>
        public decimal Valor { get; set; }
        
        /// <summary>
        /// Tipo da transação: Despesa ou Receita
        /// </summary>
        public TipoTransacao Tipo { get; set; }
        
        /// <summary>
        /// Chave estrangeira para Categoria
        /// </summary>
        public Guid CategoriaId { get; set; }
        
        /// <summary>
        /// Propriedade de navegação para Categoria
        /// </summary>
        public Categoria Categoria { get; set; } = null!;
        
        /// <summary>
        /// Chave estrangeira para Pessoa
        /// </summary>
        public Guid PessoaId { get; set; }
        
        /// <summary>
        /// Propriedade de navegação para Pessoa
        /// </summary>
        public Pessoa Pessoa { get; set; } = null!;
        
        /// <summary>
        /// Data e hora em que a transação foi criada
        /// </summary>
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}