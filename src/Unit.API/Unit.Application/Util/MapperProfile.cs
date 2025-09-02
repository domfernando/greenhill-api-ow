using AutoMapper;
using Unit.Domain.Entities.Acesso;
using Unit.Application.DTOs.Request;
using Unit.Application.DTOs.Response;
using Unit.Domain.Entities.Extra;
using Unit.Domain.Entities.Config;
using Unit.Domain.Entities.Cadastro;

namespace Unit.Application.Util
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            #region Usuario

            CreateMap<Usuario, CreateUsuarioRequest>().ReverseMap()
                  .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
                  .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<UsuarioLoginResponse, Usuario>().ReverseMap();

            CreateMap<UsuarioResponse, Usuario>().ReverseMap();
            CreateMap<Usuario, UsuarioCRUDResponse>();

            CreateMap<Usuario, UpdateUsuarioRequest>()
           .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
           .ForMember(dest => dest.NomeCompleto, opt => opt.MapFrom(src => src.NomeCompleto))
           .ForMember(dest => dest.Travado, opt => opt.MapFrom(src => src.Travado))
           .ForMember(dest => dest.Verificado, opt => opt.MapFrom(src => src.Verificado))
           .ForMember(dest => dest.Ativo, opt => opt.MapFrom(src => src.Ativo));

            #endregion

            #region Perfil

            CreateMap<Perfil, CreatePerfilRequest>()
               .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome));

            CreateMap<PerfilResponse, Perfil>();

            CreateMap<Perfil, PerfilResponse>();
            CreateMap<Usuario, UsuarioResponse>()
                .ForMember(dest => dest.Perfis, opt => opt.MapFrom(src => src.Perfis));

            CreateMap<UsuarioPerfil, UsuarioPerfilResponse>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PerfilNome, opt => opt.MapFrom(src => src.Perfil.Nome));

            #endregion

            #region Queue
            CreateMap<Queue, QueueResponse>();
            CreateMap<CreateQueueRequest, Queue>();
            CreateMap<UpdateQueueRequest, Queue>();
            #endregion

            #region Servico
            CreateMap<Servico, ServicoResponse>().ReverseMap();
            CreateMap<CreateServicoRequest, Servico>();
            CreateMap<UpdateServicoRequest, Servico>();
            #endregion

            #region Mensagem
            CreateMap<Mensagem, MensagemResponse>().ReverseMap();
            CreateMap<CreateMensagemRequest, Mensagem>();
            CreateMap<UpdateMensagemRequest, Mensagem>();
            #endregion

            #region Papel

            CreateMap<Papel, PapelResponse>().ReverseMap();
            CreateMap<CreatePapelRequest, Papel>().ReverseMap();
            CreateMap<UpdatePapelRequest, Papel>().ReverseMap();

            CreateMap<PubPapel, PubPapelResponse>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PapelID, opt => opt.MapFrom(src => src.PapelId))
                .ForMember(dest => dest.NomePublicador, opt => opt.MapFrom(src => src.Pub.Nome))
                .ForMember(dest => dest.NomePapel, opt => opt.MapFrom(src => src.Papel.Nome));

            #endregion

            #region Pessoa
            //CreateMap<Pessoa, PessoaResponse>().ReverseMap()
            //    .ForMember(dest => dest.Papeis, opt => opt.MapFrom(src => src.Papeis.Select(p => new PessoaPapelResponse
            //    {
            //        Id = p.Id,
            //        Nome = p.Nome
            //    })))
            //    .ForMember(dest => dest.Enderecos, opt => opt.MapFrom(src => src.Enderecos.Select(p => new PessoaEnderecoResponse
            //    {
            //        Id = p.Id,
            //        EnderecoId = p.EnderecoId,
            //        TipoEndereco = p.TipoEndereco,
            //        Logradouro = p.Logradouro,
            //        Numero = p.Numero,
            //        Complemento = p.Complemento,
            //        Bairro = p.Bairro,
            //        Cidade = p.Cidade,
            //        Estado = p.Estado,
            //        Cep = p.Cep,
            //    })));

            //CreateMap<CreatePessoaRequest, Pessoa>().ReverseMap();
            //CreateMap<UpdatePessoaRequest, Pessoa>().ReverseMap();

            //CreateMap<PessoaEndereco, PessoaEnderecoResponse>()
            //     .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
            //    .ForMember(dest => dest.TipoEndereco, opt => opt.MapFrom(src => src.Endereco.TipoEndereco))
            //    .ForMember(dest => dest.Logradouro, opt => opt.MapFrom(src => src.Endereco.Logradouro))
            //    .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Endereco.Numero))
            //    .ForMember(dest => dest.Complemento, opt => opt.MapFrom(src => src.Endereco.Complemento))
            //    .ForMember(dest => dest.Bairro, opt => opt.MapFrom(src => src.Endereco.Bairro))
            //    .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Endereco.Cidade))
            //    .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Endereco.Estado))
            //    .ForMember(dest => dest.Cep, opt => opt.MapFrom(src => src.Endereco.Cep));

            #endregion

            #region Endereco
            CreateMap<Endereco, EnderecoResponse>().ReverseMap();
            CreateMap<CreateEnderecoRequest, Endereco>().ReverseMap();
            CreateMap<UpdateEnderecoRequest, Endereco>().ReverseMap();
            #endregion

            #region Cong
            CreateMap<Cong, CongResponse>().ReverseMap();
            #endregion

            #region Pub
            CreateMap<Pub, PubResponse>().ReverseMap();
            CreateMap<PubResponse, PubUpdateModel>().ReverseMap();

            CreateMap<Usuario, PubUsuarioResponse>()
               .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
               .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome))
               .ForMember(dest => dest.Usr, opt => opt.MapFrom(src => src.Usr))
               .ForMember(dest => dest.Perfis, opt => opt.MapFrom(src => src.Perfis.Select(x => x.Perfil.Nome)));

            CreateMap<GrupoPub, GrupoPubResponse>()
                   .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                   .ForMember(dest => dest.GrupoID, opt => opt.MapFrom(src => src.GrupoID))
                   .ForMember(dest => dest.NomeGrupo, opt => opt.MapFrom(src => src.Grupo.Nome))
                   .ForMember(dest => dest.Papel, opt => opt.MapFrom(src => src.Papel));
            #endregion

            #region Relatorio
            CreateMap<Relatorio, RelatorioResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.PubId, opt => opt.MapFrom(src => src.PubId))
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data))
                .ForMember(dest => dest.Mes, opt => opt.MapFrom(src => src.Mes))
                .ForMember(dest => dest.Auxiliar, opt => opt.MapFrom(src => src.Auxiliar))
                .ForMember(dest => dest.Regular, opt => opt.MapFrom(src => src.Regular))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Pub.Nome))
                .ForMember(dest => dest.Privilegio, opt => opt.MapFrom(src => src.Pub.Privilegio))
                .ForMember(dest => dest.Situacao, opt => opt.MapFrom(src => src.Pub.Situacao))
                .ForMember(dest => dest.Entregue, opt => opt.MapFrom(src => src.Entregue))
                .ForMember(dest => dest.Horas, opt => opt.MapFrom(src => src.Horas))
                .ForMember(dest => dest.Estudos, opt => opt.MapFrom(src => src.Estudos))
                .ForMember(dest => dest.Obs, opt => opt.MapFrom(src => src.Obs))
                .ForMember(dest => dest.CriadoFormatado, opt => opt.MapFrom(src => src.CriadoFormatado))
                .ForMember(dest => dest.AlteradoFormatado, opt => opt.MapFrom(src => src.AlteradoFormatado));

            #endregion
        }
    }
}
