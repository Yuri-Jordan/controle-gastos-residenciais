using Microsoft.EntityFrameworkCore;
using ControleGastosResidenciais.Core.Entities;

namespace ControleGastosResidenciais.Infraestrutura.Data
{
    /// <summary>
    /// DbContext do Entity Framework Core para o sistema de controle de despesas
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração de Pessoa
            modelBuilder.Entity<Pessoa>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.Idade)
                    .IsRequired();
                
                // Exclusão em cascata: quando uma pessoa é excluída, todas as suas transações são excluídas
                entity.HasMany(p => p.Transacoes)
                    .WithOne(t => t.Pessoa)
                    .HasForeignKey(t => t.PessoaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração de Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(400);
                entity.Property(e => e.Finalidade)
                    .IsRequired();
                
                // Índice para buscas mais rápidas
                entity.HasIndex(e => e.Finalidade);
            });

            // Configuração de Transação
            modelBuilder.Entity<Transacao>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(400);
                entity.Property(e => e.Valor)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
                
                // Garantir que o valor seja positivo
                entity.ToTable(t => t.HasCheckConstraint("CK_Transacao_Valor_Positivo", "Valor > 0"));
                
                entity.Property(e => e.Tipo)
                    .IsRequired();
                
                entity.Property(e => e.CriadoEm)
                    .IsRequired();
                
                // Relacionamentos
                entity.HasOne(t => t.Categoria)
                    .WithMany(c => c.Transacoes)
                    .HasForeignKey(t => t.CategoriaId)
                    .OnDelete(DeleteBehavior.Restrict); // Não excluir transações se a categoria for excluída
                
                entity.HasOne(t => t.Pessoa)
                    .WithMany(p => p.Transacoes)
                    .HasForeignKey(t => t.PessoaId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Índices para performance
                entity.HasIndex(e => e.Tipo);
                entity.HasIndex(e => e.CategoriaId);
                entity.HasIndex(e => e.PessoaId);
                entity.HasIndex(e => e.CriadoEm);
            });
        }
    }
}