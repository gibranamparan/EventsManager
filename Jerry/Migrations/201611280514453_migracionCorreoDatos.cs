namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migracionCorreoDatos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Correos", "correoAdmin", c => c.String());
            AddColumn("dbo.Correos", "contrasena", c => c.String());
            AddColumn("dbo.Correos", "smtpHost", c => c.String());
            AddColumn("dbo.Correos", "puertoCorreo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Correos", "puertoCorreo");
            DropColumn("dbo.Correos", "smtpHost");
            DropColumn("dbo.Correos", "contrasena");
            DropColumn("dbo.Correos", "correoAdmin");
        }
    }
}
