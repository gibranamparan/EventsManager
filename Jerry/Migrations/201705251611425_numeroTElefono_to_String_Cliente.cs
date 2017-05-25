namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class numeroTElefono_to_String_Cliente : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Clientes", "telefono", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Clientes", "telefono", c => c.Int(nullable: false));
        }
    }
}
