using Microsoft.EntityFrameworkCore;
using cartas_aifa.Models;

namespace cartas_aifa.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Universidad> Universidades => Set<Universidad>();
    public DbSet<Facultad> Facultades => Set<Facultad>();
    public DbSet<DirectorEscolar> DirectoresEscolares => Set<DirectorEscolar>();
    public DbSet<DireccionAifa> DireccionesAifa => Set<DireccionAifa>();
    public DbSet<SubdireccionAifa> SubdireccionesAifa => Set<SubdireccionAifa>();
    public DbSet<AutoridadCarta> AutoridadesAifa => Set<AutoridadCarta>();
    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<DetalleEtapa> DetallesEtapas => Set<DetalleEtapa>();
    public DbSet<DetallesTipoCarta> DetallesTipoCartas => Set<DetallesTipoCarta>();
    public DbSet<EtapaAifa> EtapasAifa => Set<EtapaAifa>();
    public DbSet<TramiteAcademico> TramitesAcademicos => Set<TramiteAcademico>();
    public DbSet<ImagenCatalogo> ImagenesCatalogo => Set<ImagenCatalogo>();
    public DbSet<ConfiguracionVisual> ConfiguracionesVisuales => Set<ConfiguracionVisual>();
    public DbSet<Carrera> Carreras => Set<Carrera>();
    public DbSet<Leyenda> Leyendas => Set<Leyenda>();
    public DbSet<PiePagina> PiesDePagina => Set<PiePagina>();
    public DbSet<CodigoAcceso> CodigosAcceso => Set<CodigoAcceso>();
    public DbSet<ConfiguracionRegistro> ConfiguracionesRegistro => Set<ConfiguracionRegistro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Precisión para decimales
        modelBuilder.Entity<ConfiguracionVisual>().Property(c => c.CoordX).HasPrecision(18, 2);
        modelBuilder.Entity<ConfiguracionVisual>().Property(c => c.CoordY).HasPrecision(18, 2);
        modelBuilder.Entity<ConfiguracionVisual>().Property(c => c.Ancho).HasPrecision(18, 2);
        modelBuilder.Entity<ConfiguracionVisual>().Property(c => c.Alto).HasPrecision(18, 2);

        // Folio único en TramiteAcademico
        modelBuilder.Entity<TramiteAcademico>()
            .HasIndex(t => t.Folio)
            .IsUnique();

        // Relaciones TramiteAcademico
        modelBuilder.Entity<TramiteAcademico>()
            .HasOne(t => t.Estudiante).WithMany().HasForeignKey(t => t.IdEstudiante).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TramiteAcademico>()
            .HasOne(t => t.DetalleEtapa).WithMany().HasForeignKey(t => t.IdDetalleEtapa).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TramiteAcademico>()
            .HasOne(t => t.Autoridad).WithMany().HasForeignKey(t => t.IdAut).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TramiteAcademico>()
            .HasOne(t => t.TipoCarta).WithMany().HasForeignKey(t => t.IdTipoCarta).OnDelete(DeleteBehavior.Restrict);

        // Relaciones EtapaAifa
        modelBuilder.Entity<EtapaAifa>()
            .HasOne(e => e.Estudiante).WithMany().HasForeignKey(e => e.IdEstudiante).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EtapaAifa>()
            .HasOne(e => e.DetalleEtapa).WithMany().HasForeignKey(e => e.IdDetalleEtapa).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EtapaAifa>()
            .HasOne(e => e.Direccion).WithMany().HasForeignKey(e => e.IdDireccion).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EtapaAifa>()
            .HasOne(e => e.Subdireccion).WithMany().HasForeignKey(e => e.IdSubdireccion).OnDelete(DeleteBehavior.Restrict);

        // Ignorar la navegación inversa Tramites para evitar columna shadow
        modelBuilder.Entity<EtapaAifa>().Ignore(e => e.Tramites);

        // DetallesTipoCarta -> DetalleEtapa
        modelBuilder.Entity<DetallesTipoCarta>()
            .HasOne(d => d.DetalleEtapa).WithMany(e => e.TiposDeCartas).HasForeignKey(d => d.IdEtapa).OnDelete(DeleteBehavior.Restrict);

        // ConfiguracionVisual -> ImagenCatalogo
        modelBuilder.Entity<ConfiguracionVisual>()
            .HasOne(c => c.Imagen).WithMany().HasForeignKey(c => c.ImagenId).OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
