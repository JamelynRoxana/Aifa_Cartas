namespace Cartas_Aifa_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImagenCatalogoRequieredURL : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ImagenesCatalogo", "UrlImagen", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ImagenesCatalogo", "UrlImagen", c => c.String(nullable: false));
        }
    }
}
