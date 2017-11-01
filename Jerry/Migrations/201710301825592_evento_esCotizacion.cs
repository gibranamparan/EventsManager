namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class evento_esCotizacion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Eventoes", "esCotizacion", c => c.Boolean(nullable: false,defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Eventoes", "esCotizacion");
        }
    }
}
