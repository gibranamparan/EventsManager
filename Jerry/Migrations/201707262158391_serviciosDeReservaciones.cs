namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class serviciosDeReservaciones : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ServiciosEnReservacions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        reservacionID = c.Int(nullable: false),
                        serviciosID = c.Int(nullable: false),
                        costo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        nota = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Reservacions", t => t.reservacionID, cascadeDelete: true)
                .ForeignKey("dbo.Servicios", t => t.serviciosID, cascadeDelete: false)
                .Index(t => t.reservacionID)
                .Index(t => t.serviciosID);
            
            CreateTable(
                "dbo.Servicios",
                c => new
                    {
                        serviciosID = c.Int(nullable: false, identity: true),
                        nombre = c.String(),
                        costo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        descripcion = c.String(),
                    })
                .PrimaryKey(t => t.serviciosID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiciosEnReservacions", "serviciosID", "dbo.Servicios");
            DropForeignKey("dbo.ServiciosEnReservacions", "reservacionID", "dbo.Reservacions");
            DropIndex("dbo.ServiciosEnReservacions", new[] { "serviciosID" });
            DropIndex("dbo.ServiciosEnReservacions", new[] { "reservacionID" });
            DropTable("dbo.Servicios");
            DropTable("dbo.ServiciosEnReservacions");
        }
    }
}
