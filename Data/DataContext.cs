using IS_Kactus_Expenses.Model;
using Microsoft.EntityFrameworkCore;

namespace IS_Kactus_Expenses.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioConfiguracion> UsuarioConfiguraciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);

                entity.ToTable("Usuario");

                entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
                entity.Property(e => e.Activo).HasDefaultValue(false);
                entity.Property(e => e.BitAprobacion).HasDefaultValue(true);
                entity.Property(e => e.BitEstado).HasDefaultValue(true);
                entity.Property(e => e.BitTercero).HasDefaultValue(false);
                entity.Property(e => e.Cedula)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Celular)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.CiudadBase)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Clave)
                    .HasMaxLength(25)
                    .IsUnicode(false);
                entity.Property(e => e.Correo)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Cupo).HasDefaultValue(0);
                entity.Property(e => e.Direccion)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.IdCompania).HasColumnName("idCompania");
                entity.Property(e => e.IdPadrino)
                    .HasDefaultValue(1)
                    .HasColumnName("ID_PADRINO");
                entity.Property(e => e.IdPerfil).HasColumnName("idPerfil");
                entity.Property(e => e.Ipuser)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("IPUser");
                entity.Property(e => e.Nit)
                    .HasMaxLength(25)
                    .IsUnicode(false);
                entity.Property(e => e.NombreCompleto)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Observaciones)
                    .HasMaxLength(255)
                    .IsUnicode(false);
                entity.Property(e => e.RazonSocial)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Ts)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("ts");
                entity.Property(e => e.TsActivo)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime")
                    .HasColumnName("tsActivo");
            });

            modelBuilder.Entity<UsuarioConfiguracion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Usuario_Configuracion");

            entity.Property(e => e.CentroCostos)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.CentroOperacion)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("Centro_Operacion");
            entity.Property(e => e.IdCompania).HasColumnName("idCompania");
            entity.Property(e => e.IdCondicionPago)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("id_CondicionPago");
            entity.Property(e => e.IdConfiguracion)
                .ValueGeneratedOnAdd()
                .HasColumnName("id_Configuracion");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Moneda)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Motivo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Servicios)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TipoDocumento)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("Tipo_Documento");
            entity.Property(e => e.TipoProveedor)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UnidadNegocio)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Unidad_Negocio");
        });
        
            base.OnModelCreating(modelBuilder);
        }
    }
}
