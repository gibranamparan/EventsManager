namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eventos_addedField_totalPorServicios : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Eventoes", "totalPorServicios", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Eventoes", "totalPorServicios");
        }
    }
}
