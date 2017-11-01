namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class servicios_seleccionados : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiciosEnReservacions", "nombre", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiciosEnReservacions", "nombre");
        }
    }
}
