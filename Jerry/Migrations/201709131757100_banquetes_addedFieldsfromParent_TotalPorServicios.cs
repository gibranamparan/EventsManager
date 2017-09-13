namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class banquetes_addedFieldsfromParent_TotalPorServicios : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Eventoes", "totalPorServicios", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Eventoes", "totalPorServicios", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
