namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreacionModeloBanquete : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Banquetes",
                c => new
                    {
                        banqueteID = c.Int(nullable: false, identity: true),
                        fechaBanquete = c.DateTime(nullable: false),
                        email = c.String(nullable: false),
                        telefono = c.String(nullable: false),
                        descripcionServicio = c.String(nullable: false),
                        clienteID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.banqueteID)
                .ForeignKey("dbo.Clientes", t => t.clienteID, cascadeDelete: true)
                .Index(t => t.clienteID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Banquetes", "clienteID", "dbo.Clientes");
            DropIndex("dbo.Banquetes", new[] { "clienteID" });
            DropTable("dbo.Banquetes");
        }
    }
}
