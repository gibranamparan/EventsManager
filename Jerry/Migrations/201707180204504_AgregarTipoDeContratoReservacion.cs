namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregarTipoDeContratoReservacion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservacions", "TipoContrato", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservacions", "TipoContrato");
        }
    }
}
