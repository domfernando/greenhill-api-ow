using Unit.Domain.Entities.Acesso;
using Unit.Domain.Entities.Cadastro;
using Unit.Domain.Entities.Config;
using Unit.Domain.Entities.Extra;

namespace Unit.Application.Base
{
    public interface IUnitOfWork
    {
        IRepository<Usuario> Usuarios { get; }
        IRepository<Perfil> Perfis { get; }
        IRepository<UsuarioPerfil> UsuarioPerfis { get; }
        IRepository<Queue> Queues { get; }
        IRepository<Servico> Servicos { get; }
        IRepository<Mensagem> Mensagens { get; }
        IRepository<Papel> Papeis { get; }
        IRepository<Pessoa> Pessoas { get; }
        IRepository<Endereco> Enderecos { get; }
        IRepository<PubPapel> PubPapeis { get; }
        IRepository<PessoaEndereco> PessoaEnderecos { get; }
        IRepository<Cong> Congs { get; }
        IRepository<Grupo> Grupos { get; }
        IRepository<GrupoPub> GrupoPubs { get; }
        IRepository<Pub> Pubs { get; }
        IRepository<Arranjo> Arranjos { get; }
        IRepository<NVMC> NVMCs { get; }
        IRepository<NVMCParte> NVMCPartes { get; }
        IRepository<Discurso> Discursos { get; }
        IRepository<Tema> Temas { get; }
        IRepository<OradorTema> OradorTemas { get; }
        IRepository<DiscursoNotificacao> DiscursoNotificacoes { get; }
        IRepository<ArranjoNotificacao> ArranjoNotificacoes { get; }
        IRepository<Evento> Eventos { get; }
        IRepository<EventoPapel> EventoPapeis { get; }
        IRepository<Relatorio> Relatorios { get; }
        Task<int> CommitAsync();
    }
}
