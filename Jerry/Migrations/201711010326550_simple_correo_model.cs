namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class simple_correo_model : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Correos", "contrasena");
            DropColumn("dbo.Correos", "smtpHost");
            DropColumn("dbo.Correos", "puertoCorreo");
            DropColumn("dbo.Correos", "sslEnabled");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Correos", "sslEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Correos", "puertoCorreo", c => c.Int(nullable: false));
            AddColumn("dbo.Correos", "smtpHost", c => c.String());
            AddColumn("dbo.Correos", "contrasena", c => c.String());
        }
    }
}
