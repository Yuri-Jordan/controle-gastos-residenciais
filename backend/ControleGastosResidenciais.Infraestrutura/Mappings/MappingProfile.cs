using AutoMapper;
using ControleGastosResidenciais.Core.DTOs;
using ControleGastosResidenciais.Core.Entities;

namespace ControleGastosResidenciais.Infraestrutura.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeamentos de Pessoa
            CreateMap<Pessoa, PessoaDto>()
                .ForMember(dest => dest.EhMenorDeIdade, opt => opt.MapFrom(src => src.Idade < 18));
            CreateMap<CriarPessoaDto, Pessoa>();
            CreateMap<AtualizarPessoaDto, Pessoa>();
            
            // Mapeamentos de Categoria
            CreateMap<Categoria, CategoriaDto>()
                .ForMember(dest => dest.ExibirFinalidade, 
                    opt => opt.MapFrom(src => src.Finalidade.ToString()));
            CreateMap<CriarCategoriaDto, Categoria>();
            
            // Mapeamentos de Transação
            CreateMap<Transacao, TransacaoDto>()
                .ForMember(dest => dest.ExibirTipo, 
                    opt => opt.MapFrom(src => src.Tipo.ToString()))
                .ForMember(dest => dest.DescricaoCategoria, 
                    opt => opt.MapFrom(src => src.Categoria.Descricao))
                .ForMember(dest => dest.NomePessoa, 
                    opt => opt.MapFrom(src => src.Pessoa.Nome));
            CreateMap<CriarTransacaoDto, Transacao>();
        }
    }
}