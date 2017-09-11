namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class baquetes_addedFiedls_Platillos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Eventoes", "platillo", c => c.String());
            AddColumn("dbo.Eventoes", "numTiemposPlatillo", c => c.Int(defaultValue:0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Eventoes", "numTiemposPlatillo");
            DropColumn("dbo.Eventoes", "platillo");
        }
    }
}
