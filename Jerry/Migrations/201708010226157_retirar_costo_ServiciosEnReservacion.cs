namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class retirar_costo_ServiciosEnReservacion : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ServiciosEnReservacions", "costo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ServiciosEnReservacions", "costo", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
