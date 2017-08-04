namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changed_salon_fromReservacion_ToSesion : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reservacions", "salonID", "dbo.Salons");
            DropIndex("dbo.Reservacions", new[] { "salonID" });
            AddColumn("dbo.SesionDeReservacions", "salonID", c => c.Int(nullable: false));
            CreateIndex("dbo.SesionDeReservacions", "salonID");
            AddForeignKey("dbo.SesionDeReservacions", "salonID", "dbo.Salons", "salonID", cascadeDelete: true);
            DropColumn("dbo.Reservacions", "salonID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reservacions", "salonID", c => c.Int(nullable: false));
            DropForeignKey("dbo.SesionDeReservacions", "salonID", "dbo.Salons");
            DropIndex("dbo.SesionDeReservacions", new[] { "salonID" });
            DropColumn("dbo.SesionDeReservacions", "salonID");
            CreateIndex("dbo.Reservacions", "salonID");
            AddForeignKey("dbo.Reservacions", "salonID", "dbo.Salons", "salonID", cascadeDelete: true);
        }
    }
}
