namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambiosBanqueteModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Banquetes", "cantidadPersonas", c => c.Int(nullable: false));
            AddColumn("dbo.Banquetes", "costo", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Banquetes", "tipoContrato", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Banquetes", "tipoContrato");
            DropColumn("dbo.Banquetes", "costo");
            DropColumn("dbo.Banquetes", "cantidadPersonas");
        }
    }
}
