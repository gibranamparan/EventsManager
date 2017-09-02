namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class servicios_tipoDeEvento2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ServiciosEnReservacions", "serviciosID", "dbo.Servicios");
            DropIndex("dbo.ServiciosEnReservacions", new[] { "serviciosID" });
            RenameColumn(table: "dbo.ServiciosEnReservacions", name: "reservacionID", newName: "eventoID");
            RenameIndex(table: "dbo.ServiciosEnReservacions", name: "IX_reservacionID", newName: "IX_eventoID");
            AlterColumn("dbo.ServiciosEnReservacions", "serviciosID", c => c.Int(nullable: false));
            CreateIndex("dbo.ServiciosEnReservacions", "serviciosID");
            AddForeignKey("dbo.ServiciosEnReservacions", "serviciosID", "dbo.Servicios", "serviciosID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiciosEnReservacions", "serviciosID", "dbo.Servicios");
            DropIndex("dbo.ServiciosEnReservacions", new[] { "serviciosID" });
            AlterColumn("dbo.ServiciosEnReservacions", "serviciosID", c => c.Int());
            RenameIndex(table: "dbo.ServiciosEnReservacions", name: "IX_eventoID", newName: "IX_reservacionID");
            RenameColumn(table: "dbo.ServiciosEnReservacions", name: "eventoID", newName: "reservacionID");
            CreateIndex("dbo.ServiciosEnReservacions", "serviciosID");
            AddForeignKey("dbo.ServiciosEnReservacions", "serviciosID", "dbo.Servicios", "serviciosID");
        }
    }
}
