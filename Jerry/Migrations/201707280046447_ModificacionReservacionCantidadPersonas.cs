namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModificacionReservacionCantidadPersonas : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservacions", "CantidadPersonas", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservacions", "CantidadPersonas");
        }
    }
}
