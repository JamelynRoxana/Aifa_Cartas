using System;
using System.Data.Entity;
using System.Linq;

namespace Cartas_Aifa_Api.Models
{
    public class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<Model1>());
        }

        // --- TODAS LAS TABLAS (DbSet) ---
        public virtual DbSet<ConfiguracionGlobal> ConfiguracionesGlobales { get; set; }

        // REEMPLAZO: Ahora tenemos Catálogo y Configuración
        public virtual DbSet<ImagenCatalogo> ImagenesCatalogo { get; set; }
        public virtual DbSet<ConfiguracionVisual> ConfiguracionesVisuales { get; set; }

        public virtual DbSet<Universidad> Universidades { get; set; }
        public virtual DbSet<Facultad> Facultades { get; set; }
        public virtual DbSet<DirectorEscolar> DirectoresEscolares { get; set; }
        public virtual DbSet<DireccionAifa> DireccionesAifa { get; set; }
        public virtual DbSet<SubdireccionAifa> SubdireccionesAifa { get; set; }
        public virtual DbSet<AutoridadAifa> AutoridadesAifa { get; set; }
        public virtual DbSet<Estudiante> Estudiantes { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }

        public virtual DbSet<DetalleEtapa> DetallesEtapas { get; set; }
        public virtual DbSet<DetallesTipoCarta> DetallesTipoCartas { get; set; }
        public virtual DbSet<EtapaAifa> EtapasAifa { get; set; }
        public virtual DbSet<TramiteAcademico> TramitesAcademicos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // 1. Configuración de Llaves Primarias
            modelBuilder.Entity<TramiteAcademico>().HasKey(t => t.Id);
            modelBuilder.Entity<EtapaAifa>().HasKey(e => e.Id);
            modelBuilder.Entity<ImagenCatalogo>().HasKey(i => i.Id);
            modelBuilder.Entity<ConfiguracionVisual>().HasKey(c => c.Id);

            // 2. Precisión para decimales (Nuevas configuraciones visuales)
            modelBuilder.Entity<ConfiguracionVisual>().Property(c => c.CoordX).HasPrecision(18, 2);
            modelBuilder.Entity<ConfiguracionVisual>().Property(c => c.CoordY).HasPrecision(18, 2);
            modelBuilder.Entity<ConfiguracionVisual>().Property(c => c.Ancho).HasPrecision(18, 2);
            modelBuilder.Entity<ConfiguracionVisual>().Property(c => c.Alto).HasPrecision(18, 2);

            // --- 3. RELACIÓN: Configuración Visual -> Catálogo de Imágenes ---
            modelBuilder.Entity<ConfiguracionVisual>()
                .HasRequired(c => c.Imagen)
                .WithMany() // Una imagen del catálogo puede estar en muchas configuraciones
                .HasForeignKey(c => c.ImagenId)
                .WillCascadeOnDelete(false);

            // --- 4. RELACIONES DE ETAPA AIFA ---
            modelBuilder.Entity<EtapaAifa>()
                .HasRequired(e => e.Estudiante)
                .WithMany()
                .HasForeignKey(e => e.IdEstudiante)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EtapaAifa>()
                .HasRequired(e => e.DetalleEtapa)
                .WithMany()
                .HasForeignKey(e => e.IdDetalleEtapa)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EtapaAifa>()
                .HasRequired(e => e.Direccion)
                .WithMany()
                .HasForeignKey(e => e.IdDireccion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<EtapaAifa>()
                .HasRequired(e => e.Subdireccion)
                .WithMany()
                .HasForeignKey(e => e.IdSubdireccion)
                .WillCascadeOnDelete(false);

            // --- 5. RELACIONES DE TRÁMITE ACADÉMICO ---
            modelBuilder.Entity<TramiteAcademico>()
                .HasRequired(t => t.Estudiante)
                .WithMany()
                .HasForeignKey(t => t.IdEstudiante)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TramiteAcademico>()
                .HasRequired(t => t.DetalleEtapa)
                .WithMany()
                .HasForeignKey(t => t.IdDetalleEtapa)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TramiteAcademico>()
                .HasRequired(t => t.Autoridad)
                .WithMany()
                .HasForeignKey(t => t.IdAut)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TramiteAcademico>()
                .HasRequired(t => t.TipoCarta)
                .WithMany()
                .HasForeignKey(t => t.IdTipoCarta)
                .WillCascadeOnDelete(false);

            // --- 6. RELACIÓN CATÁLOGO: Etapa -> Tipo de Carta ---
            modelBuilder.Entity<DetallesTipoCarta>()
                .HasRequired(d => d.DetalleEtapa)
                .WithMany(e => e.TiposDeCartas)
                .HasForeignKey(d => d.IdEtapa)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}