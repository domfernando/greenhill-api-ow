using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public IRepository<PessoaPapel> PessoaPapeis { get; }
        public IRepository<PessoaEndereco> PessoaEnderecos { get; }
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
            PessoaPapeis = new Repository<PessoaPapel>(context);
            PessoaEnderecos = new Repository<PessoaEndereco>(context);
        }

        public async Task<int> CommitAsync() => await _context.SaveChangesAsync();
    }
}
