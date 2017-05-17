namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModeloDeCorreo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Correos",
                c => new
                    {
                        correoID = c.Int(nullable: false, identity: true),
                        To = c.String(),
                        Subject = c.String(),
                        Body = c.String(),
                    })
                .PrimaryKey(t => t.correoID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Correos");
        }
    }
}
