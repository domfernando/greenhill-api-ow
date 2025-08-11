namespace Unit.Infra.Persistence.Context
{
    using Unit.Domain.Entities.Acesso;
    using Unit.Domain.Entities.Config;
    using Microsoft.EntityFrameworkCore;
    using Unit.Domain.Entities.Extra;
    using Unit.Domain.Entities.Cadastro;

    public class DatabaseContext : DbContext
    {
        public DatabaseContext() { }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Perfil> Perfil { get; set; }
        public DbSet<UsuarioPerfil> UsuarioPerfil { get; set; }
        public DbSet<Mensagem> Mensagem { get; set; }
        public DbSet<Pessoa> Pessoa { get; set; }
        public DbSet<Papel> Papel { get; set; }
        public DbSet<PessoaEndereco> PessoaEndereco { get; set; }
        public DbSet<PessoaPapel> PessoaPapel { get; set; }
        public DbSet<Servico> Servico { get; set; }
        public DbSet<Queue> Queue { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<UsuarioPerfil>(userrole =>
            //{
            //    userrole.HasKey(ur => new
            //    {
            //        ur.UsuarioId,
            //        ur.PerfilId
            //    });

            //    userrole.HasOne(x => x.Usuario)
            //          .WithMany(x => x.Perfis)
            //          .HasForeignKey(x => x.UsuarioId);

            //    userrole.HasOne(x => x.Perfil)
            //            .WithMany(x => x.Usuarios)
            //            .HasForeignKey(x => x.PerfilId);
            //    .IsRequired();
            //    .IsRequired();
            //});

            base.OnModelCreating(builder);
        }
    }
}