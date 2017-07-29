namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambioBanqueteModelLugar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Banquetes", "lugar", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Banquetes", "lugar");
        }
    }
}
