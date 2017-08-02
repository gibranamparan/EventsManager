namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reservaciones_dividadesPor_Sesiones : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SesionDeReservacions",
                c => new
                    {
                        SesionDeReservacionID = c.Int(nullable: false, identity: true),
                        periodoDeSesion_startDate = c.DateTime(nullable: false),
                        periodoDeSesion_endDate = c.DateTime(nullable: false),
                        reservacionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SesionDeReservacionID)
                .ForeignKey("dbo.Reservacions", t => t.reservacionID, cascadeDelete: true)
                .Index(t => t.reservacionID);
            
            DropColumn("dbo.Reservacions", "fechaEventoInicial");
            DropColumn("dbo.Reservacions", "fechaEventoFinal");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reservacions", "fechaEventoFinal", c => c.DateTime(nullable: false));
            AddColumn("dbo.Reservacions", "fechaEventoInicial", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.SesionDeReservacions", "reservacionID", "dbo.Reservacions");
            DropIndex("dbo.SesionDeReservacions", new[] { "reservacionID" });
            DropTable("dbo.SesionDeReservacions");
        }
    }
}
