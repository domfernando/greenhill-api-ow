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
        public DbSet<PubPapel> PubPapel { get; set; }
        public DbSet<Servico> Servico { get; set; }
        public DbSet<Queue> Queue { get; set; }
        public DbSet<Endereco> Endereco { get; set; }
        public DbSet<Cong> Cong { get; set; }
        public DbSet<Grupo> Grupo { get; set; }
        public DbSet<GrupoPub> GrupoPub { get; set; }
        public DbSet<Pub> Pub { get; set; }
        public DbSet<Arranjo> Arranjo { get; set; }
        public DbSet<NVMC> NVMC { get; set; }
        public DbSet<NVMCParte> NVMCParte { get; set; }
        public DbSet<Tema> Tema { get; set; }
        public DbSet<OradorTema> OradorTema { get; set; }
        public DbSet<Discurso> Discurso { get; set; }
        public DbSet<Evento> Evento { get; set; }
        public DbSet<EventoPapel> EventoPapel { get; set; }
        public DbSet<Relatorio> Relatorio { get; set; }
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