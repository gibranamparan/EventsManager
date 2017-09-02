namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class banquete_detalles_notReq : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Eventoes", "Detalles", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Eventoes", "Detalles", c => c.String(nullable: false));
        }
    }
}
