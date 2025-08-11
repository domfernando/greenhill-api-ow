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
        IRepository<PessoaPapel> PessoaPapeis { get; }
        IRepository<PessoaEndereco> PessoaEnderecos { get; }

        Task<int> CommitAsync();
    }
}
