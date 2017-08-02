namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nonRequieres_servicioID_in_RerservacionServicioTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ServiciosEnReservacions", "serviciosID", "dbo.Servicios");
            DropIndex("dbo.ServiciosEnReservacions", new[] { "serviciosID" });
            AlterColumn("dbo.ServiciosEnReservacions", "serviciosID", c => c.Int());
            CreateIndex("dbo.ServiciosEnReservacions", "serviciosID");
            AddForeignKey("dbo.ServiciosEnReservacions", "serviciosID", "dbo.Servicios", "serviciosID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiciosEnReservacions", "serviciosID", "dbo.Servicios");
            DropIndex("dbo.ServiciosEnReservacions", new[] { "serviciosID" });
            AlterColumn("dbo.ServiciosEnReservacions", "serviciosID", c => c.Int(nullable: false));
            CreateIndex("dbo.ServiciosEnReservacions", "serviciosID");
            AddForeignKey("dbo.ServiciosEnReservacions", "serviciosID", "dbo.Servicios", "serviciosID", cascadeDelete: true);
        }
    }
}
