namespace Cartas_Aifa_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AutoridadesAifa",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                        Rango = c.String(),
                        Puesto = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ConfiguracionesGlobales",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LeyendaAnualVigente = c.String(nullable: false),
                        UltimaActualizacion = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ConfiguracionesVisuales",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImagenId = c.Int(nullable: false),
                        CoordX = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CoordY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Ancho = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Alto = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AnioAplicacion = c.Int(nullable: false),
                        MostrarEn = c.String(),
                        FechaInicio = c.DateTime(nullable: false),
                        FechaFin = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ImagenesCatalogo", t => t.ImagenId)
                .Index(t => t.ImagenId);
            
            CreateTable(
                "dbo.ImagenesCatalogo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NombreArchivo = c.String(nullable: false),
                        UrlImagen = c.String(nullable: false),
                        FechaCarga = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DetallesEtapas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TipoEtapa = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DetallesTipoCarta",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NombreCarta = c.String(nullable: false),
                        IdEtapa = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DetallesEtapas", t => t.IdEtapa)
                .Index(t => t.IdEtapa);
            
            CreateTable(
                "dbo.DireccionesAifa",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NombreDir = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DirectoresEscolares",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdF = c.Int(nullable: false),
                        Nombre = c.String(nullable: false),
                        Rango = c.String(),
                        Puesto = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Facultades", t => t.IdF, cascadeDelete: true)
                .Index(t => t.IdF);
            
            CreateTable(
                "dbo.Facultades",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdU = c.Int(nullable: false),
                        NombreF = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Universidades", t => t.IdU, cascadeDelete: true)
                .Index(t => t.IdU);
            
            CreateTable(
                "dbo.Universidades",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NombreU = c.String(nullable: false),
                        DireccionU = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Estudiantes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NombreCompleto = c.String(nullable: false),
                        Matricula = c.String(nullable: false),
                        Carrera = c.String(),
                        IdF = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Facultades", t => t.IdF, cascadeDelete: true)
                .Index(t => t.IdF);
            
            CreateTable(
                "dbo.EtapasAifa",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdEstudiante = c.Int(nullable: false),
                        IdDetalleEtapa = c.Int(nullable: false),
                        IdDireccion = c.Int(nullable: false),
                        IdSubdireccion = c.Int(nullable: false),
                        FechaInicio = c.DateTime(nullable: false),
                        FechaFin = c.DateTime(nullable: false),
                        Activa = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DetallesEtapas", t => t.IdDetalleEtapa)
                .ForeignKey("dbo.DireccionesAifa", t => t.IdDireccion)
                .ForeignKey("dbo.Estudiantes", t => t.IdEstudiante)
                .ForeignKey("dbo.SubdireccionesAifa", t => t.IdSubdireccion)
                .Index(t => t.IdEstudiante)
                .Index(t => t.IdDetalleEtapa)
                .Index(t => t.IdDireccion)
                .Index(t => t.IdSubdireccion);
            
            CreateTable(
                "dbo.SubdireccionesAifa",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdDir = c.Int(nullable: false),
                        NombreSub = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DireccionesAifa", t => t.IdDir, cascadeDelete: true)
                .Index(t => t.IdDir);
            
            CreateTable(
                "dbo.TramitesAcademicos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Folio = c.String(nullable: false, maxLength: 50),
                        IdEstudiante = c.Int(nullable: false),
                        IdDetalleEtapa = c.Int(nullable: false),
                        IdAut = c.Int(nullable: false),
                        IdTipoCarta = c.Int(nullable: false),
                        FechaRegistro = c.DateTime(nullable: false),
                        EtapaAifa_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AutoridadesAifa", t => t.IdAut)
                .ForeignKey("dbo.DetallesEtapas", t => t.IdDetalleEtapa)
                .ForeignKey("dbo.Estudiantes", t => t.IdEstudiante)
                .ForeignKey("dbo.DetallesTipoCarta", t => t.IdTipoCarta)
                .ForeignKey("dbo.EtapasAifa", t => t.EtapaAifa_Id)
                .Index(t => t.Folio, unique: true)
                .Index(t => t.IdEstudiante)
                .Index(t => t.IdDetalleEtapa)
                .Index(t => t.IdAut)
                .Index(t => t.IdTipoCarta)
                .Index(t => t.EtapaAifa_Id);
            
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Correo = c.String(),
                        Rol = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TramitesAcademicos", "EtapaAifa_Id", "dbo.EtapasAifa");
            DropForeignKey("dbo.TramitesAcademicos", "IdTipoCarta", "dbo.DetallesTipoCarta");
            DropForeignKey("dbo.TramitesAcademicos", "IdEstudiante", "dbo.Estudiantes");
            DropForeignKey("dbo.TramitesAcademicos", "IdDetalleEtapa", "dbo.DetallesEtapas");
            DropForeignKey("dbo.TramitesAcademicos", "IdAut", "dbo.AutoridadesAifa");
            DropForeignKey("dbo.EtapasAifa", "IdSubdireccion", "dbo.SubdireccionesAifa");
            DropForeignKey("dbo.SubdireccionesAifa", "IdDir", "dbo.DireccionesAifa");
            DropForeignKey("dbo.EtapasAifa", "IdEstudiante", "dbo.Estudiantes");
            DropForeignKey("dbo.EtapasAifa", "IdDireccion", "dbo.DireccionesAifa");
            DropForeignKey("dbo.EtapasAifa", "IdDetalleEtapa", "dbo.DetallesEtapas");
            DropForeignKey("dbo.Estudiantes", "IdF", "dbo.Facultades");
            DropForeignKey("dbo.DirectoresEscolares", "IdF", "dbo.Facultades");
            DropForeignKey("dbo.Facultades", "IdU", "dbo.Universidades");
            DropForeignKey("dbo.DetallesTipoCarta", "IdEtapa", "dbo.DetallesEtapas");
            DropForeignKey("dbo.ConfiguracionesVisuales", "ImagenId", "dbo.ImagenesCatalogo");
            DropIndex("dbo.TramitesAcademicos", new[] { "EtapaAifa_Id" });
            DropIndex("dbo.TramitesAcademicos", new[] { "IdTipoCarta" });
            DropIndex("dbo.TramitesAcademicos", new[] { "IdAut" });
            DropIndex("dbo.TramitesAcademicos", new[] { "IdDetalleEtapa" });
            DropIndex("dbo.TramitesAcademicos", new[] { "IdEstudiante" });
            DropIndex("dbo.TramitesAcademicos", new[] { "Folio" });
            DropIndex("dbo.SubdireccionesAifa", new[] { "IdDir" });
            DropIndex("dbo.EtapasAifa", new[] { "IdSubdireccion" });
            DropIndex("dbo.EtapasAifa", new[] { "IdDireccion" });
            DropIndex("dbo.EtapasAifa", new[] { "IdDetalleEtapa" });
            DropIndex("dbo.EtapasAifa", new[] { "IdEstudiante" });
            DropIndex("dbo.Estudiantes", new[] { "IdF" });
            DropIndex("dbo.Facultades", new[] { "IdU" });
            DropIndex("dbo.DirectoresEscolares", new[] { "IdF" });
            DropIndex("dbo.DetallesTipoCarta", new[] { "IdEtapa" });
            DropIndex("dbo.ConfiguracionesVisuales", new[] { "ImagenId" });
            DropTable("dbo.Usuarios");
            DropTable("dbo.TramitesAcademicos");
            DropTable("dbo.SubdireccionesAifa");
            DropTable("dbo.EtapasAifa");
            DropTable("dbo.Estudiantes");
            DropTable("dbo.Universidades");
            DropTable("dbo.Facultades");
            DropTable("dbo.DirectoresEscolares");
            DropTable("dbo.DireccionesAifa");
            DropTable("dbo.DetallesTipoCarta");
            DropTable("dbo.DetallesEtapas");
            DropTable("dbo.ImagenesCatalogo");
            DropTable("dbo.ConfiguracionesVisuales");
            DropTable("dbo.ConfiguracionesGlobales");
            DropTable("dbo.AutoridadesAifa");
        }
    }
}
