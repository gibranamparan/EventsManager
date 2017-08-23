namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salones_CapacidadMaxima : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Salons", "capacidadMaxima", c => c.Int(nullable: false));
            AlterColumn("dbo.Salons", "detalles", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Salons", "detalles", c => c.String(nullable: false));
            DropColumn("dbo.Salons", "capacidadMaxima");
        }
    }
}
