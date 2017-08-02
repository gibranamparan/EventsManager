using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel;

namespace Jerry.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [DisplayName("Rol")]
        public string role{ get {
                string rol = string.Empty;
                foreach(var rolT in this.Roles)
                    if (this.Roles.Contains(rolT)){
                        rol = rolT.RoleId;
                        break;
                    }
                return rol;
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public static class UserRoles
        {
            public const string ADMIN = "Administrador";
            public const string CLIENTE = "Cliente";
            public const string ASISTENTE = "Asistente";
        }

        public static string[] rolesArray { get
            {
                return new string[]{
                    UserRoles.ADMIN,UserRoles.ASISTENTE,UserRoles.CLIENTE,
                };
            }
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //En esta parte defino las tablas del sistema
        public DbSet<Cliente> clientes { get; set; }
        public DbSet<Salon> salones { get; set; }
        public DbSet<Reservacion> reservaciones { get; set; }
        public DbSet<Pago> pagos { get; set; }
        public DbSet<Correo> Correos { get; set; }
        public DbSet<Banquete> Banquetes { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<ServiciosEnReservacion> ServiciosEnReservaciones { get; set; }
        public DbSet<SesionDeReservacion> sesionesEnReservaciones { get; set; }
    }
}