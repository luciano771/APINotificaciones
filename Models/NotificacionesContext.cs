using Microsoft.EntityFrameworkCore;
namespace APIREST.Models
{
    public class NotificacionesContext : DbContext
    {
        public NotificacionesContext(DbContextOptions<NotificacionesContext> options) : base(options)
        {

        }

        public DbSet<Usuarios> Notificacion { get; set; }
    }
}
