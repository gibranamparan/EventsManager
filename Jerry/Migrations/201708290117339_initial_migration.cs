namespace Jerry.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial_migration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Eventoes",
                c => new
                    {
                        eventoID = c.Int(nullable: false, identity: true),
                        CantidadPersonas = c.Int(nullable: false),
                        clienteID = c.Int(nullable: false),
                        fechaReservacion = c.DateTime(nullable: false),
                        fechaEventoInicial = c.DateTime(nullable: false),
                        fechaEventoFinal = c.DateTime(nullable: false),
                        costo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Detalles = c.String(nullable: false),
                        TipoContrato = c.Int(nullable: false),
                        lugar = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.eventoID)
                .ForeignKey("dbo.Clientes", t => t.clienteID, cascadeDelete: true)
                .Index(t => t.clienteID);
            
            CreateTable(
                "dbo.Clientes",
                c => new
                    {
                        clienteID = c.Int(nullable: false, identity: true),
                        nombre = c.String(nullable: false),
                        apellidoP = c.String(nullable: false),
                        apellidoM = c.String(nullable: false),
                        email = c.String(nullable: false),
                        telefono = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.clienteID);
            
            CreateTable(
                "dbo.Pagoes",
                c => new
                    {
                        pagoID = c.Int(nullable: false, identity: true),
                        eventoID = c.Int(nullable: false),
                        cantidad = c.Decimal(nullable: false, precision: 18, scale: 2),
                        fechaPago = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.pagoID)
                .ForeignKey("dbo.Eventoes", t => t.eventoID, cascadeDelete: true)
                .Index(t => t.eventoID);
            
            CreateTable(
                "dbo.ServiciosEnReservacions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        reservacionID = c.Int(nullable: false),
                        serviciosID = c.Int(),
                        nota = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Eventoes", t => t.reservacionID, cascadeDelete: true)
                .ForeignKey("dbo.Servicios", t => t.serviciosID)
                .Index(t => t.reservacionID)
                .Index(t => t.serviciosID);
            
            CreateTable(
                "dbo.Servicios",
                c => new
                    {
                        serviciosID = c.Int(nullable: false, identity: true),
                        nombre = c.String(),
                        costo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        descripcion = c.String(),
                    })
                .PrimaryKey(t => t.serviciosID);
            
            CreateTable(
                "dbo.SesionDeReservacions",
                c => new
                    {
                        SesionDeReservacionID = c.Int(nullable: false, identity: true),
                        periodoDeSesion_startDate = c.DateTime(nullable: false),
                        periodoDeSesion_endDate = c.DateTime(nullable: false),
                        salonID = c.Int(nullable: false),
                        reservacionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SesionDeReservacionID)
                .ForeignKey("dbo.Eventoes", t => t.reservacionID, cascadeDelete: true)
                .ForeignKey("dbo.Salons", t => t.salonID, cascadeDelete: true)
                .Index(t => t.salonID)
                .Index(t => t.reservacionID);
            
            CreateTable(
                "dbo.Salons",
                c => new
                    {
                        salonID = c.Int(nullable: false, identity: true),
                        nombre = c.String(nullable: false),
                        detalles = c.String(),
                        capacidadMaxima = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.salonID);
            
            CreateTable(
                "dbo.Correos",
                c => new
                    {
                        correoID = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                        Body = c.String(),
                        correoAdmin = c.String(),
                        contrasena = c.String(),
                        smtpHost = c.String(),
                        puertoCorreo = c.Int(nullable: false),
                        sslEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.correoID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.SesionDeReservacions", "salonID", "dbo.Salons");
            DropForeignKey("dbo.SesionDeReservacions", "reservacionID", "dbo.Eventoes");
            DropForeignKey("dbo.ServiciosEnReservacions", "serviciosID", "dbo.Servicios");
            DropForeignKey("dbo.ServiciosEnReservacions", "reservacionID", "dbo.Eventoes");
            DropForeignKey("dbo.Pagoes", "eventoID", "dbo.Eventoes");
            DropForeignKey("dbo.Eventoes", "clienteID", "dbo.Clientes");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.SesionDeReservacions", new[] { "reservacionID" });
            DropIndex("dbo.SesionDeReservacions", new[] { "salonID" });
            DropIndex("dbo.ServiciosEnReservacions", new[] { "serviciosID" });
            DropIndex("dbo.ServiciosEnReservacions", new[] { "reservacionID" });
            DropIndex("dbo.Pagoes", new[] { "eventoID" });
            DropIndex("dbo.Eventoes", new[] { "clienteID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Correos");
            DropTable("dbo.Salons");
            DropTable("dbo.SesionDeReservacions");
            DropTable("dbo.Servicios");
            DropTable("dbo.ServiciosEnReservacions");
            DropTable("dbo.Pagoes");
            DropTable("dbo.Clientes");
            DropTable("dbo.Eventoes");
        }
    }
}
