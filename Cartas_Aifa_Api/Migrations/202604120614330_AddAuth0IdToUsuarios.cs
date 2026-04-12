namespace Cartas_Aifa_Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuth0IdToUsuarios : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Usuarios", "Auth0_Id", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Usuarios", "Auth0_Id");
        }
    }
}
