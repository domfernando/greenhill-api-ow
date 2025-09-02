using Unit.Application.Base;
using Unit.Domain.Entities.Acesso;
using Unit.Domain.Entities.Cadastro;
using Unit.Domain.Entities.Config;
using Unit.Domain.Entities.Extra;
using Unit.Infra.Persistence.Context;

namespace Unit.Infra.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;
        public IRepository<Usuario> Usuarios { get; }
        public IRepository<Perfil> Perfis { get; }
        public IRepository<UsuarioPerfil> UsuarioPerfis { get; }
        public IRepository<Servico> Servicos { get; }
        public IRepository<Queue> Queues { get; }
        public IRepository<Mensagem> Mensagens { get; }
        public IRepository<Papel> Papeis { get; }
        public IRepository<Pessoa> Pessoas { get; }
        public IRepository<Endereco> Enderecos { get; }
        public IRepository<PubPapel> PubPapeis { get; }
        public IRepository<PessoaEndereco> PessoaEnderecos { get; }
        public IRepository<Cong> Congs { get; }
        public IRepository<Grupo> Grupos { get; }
        public IRepository<GrupoPub> GrupoPubs { get; }
        public IRepository<Pub> Pubs { get; }
        public IRepository<Arranjo> Arranjos { get; }
        public IRepository<NVMC> NVMCs { get; }
        public IRepository<NVMCParte> NVMCPartes { get; }
        public IRepository<Discurso> Discursos { get; }
        public IRepository<Tema> Temas { get; }
        public IRepository<OradorTema> OradorTemas { get; }
        public IRepository<DiscursoNotificacao> DiscursoNotificacoes { get; }
        public IRepository<ArranjoNotificacao> ArranjoNotificacoes { get; }
        public IRepository<Evento> Eventos { get; }
        public IRepository<EventoPapel> EventoPapeis { get; }
        public IRepository<Relatorio> Relatorios { get; }
        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
            Usuarios = new Repository<Usuario>(context);
            Perfis = new Repository<Perfil>(context);
            UsuarioPerfis = new Repository<UsuarioPerfil>(context);
            Servicos = new Repository<Servico>(context);
            Queues = new Repository<Queue>(context);
            Mensagens = new Repository<Mensagem>(context);
            Papeis = new Repository<Papel>(context);
            Pessoas = new Repository<Pessoa>(context);
            Enderecos = new Repository<Endereco>(context);
            PubPapeis = new Repository<PubPapel>(context);
            PessoaEnderecos = new Repository<PessoaEndereco>(context);
            Congs = new Repository<Cong>(context);
            Grupos = new Repository<Grupo>(context);
            GrupoPubs = new Repository<GrupoPub>(context);
            Pubs = new Repository<Pub>(context);
            Arranjos = new Repository<Arranjo>(context);
            NVMCs = new Repository<NVMC>(context);
            NVMCPartes = new Repository<NVMCParte>(context);
            Discursos = new Repository<Discurso>(context);
            Temas = new Repository<Tema>(context);
            OradorTemas = new Repository<OradorTema>(context);
            DiscursoNotificacoes = new Repository<DiscursoNotificacao>(context);
            ArranjoNotificacoes = new Repository<ArranjoNotificacao>(context);
            Eventos = new Repository<Evento>(context);
            EventoPapeis = new Repository<EventoPapel>(context);
            Relatorios = new Repository<Relatorio>(context);
        }

        public async Task<int> CommitAsync() => await _context.SaveChangesAsync();
    }
}
