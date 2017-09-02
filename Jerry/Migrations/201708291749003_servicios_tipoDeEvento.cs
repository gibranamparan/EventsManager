namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class servicios_tipoDeEvento : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiciosEnReservacions", "cantidad", c => c.Int(nullable: false));
            AddColumn("dbo.Servicios", "tipoDeEvento", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Servicios", "tipoDeEvento");
            DropColumn("dbo.ServiciosEnReservacions", "cantidad");
        }
    }
}
