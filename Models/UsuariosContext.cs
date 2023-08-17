using Microsoft.EntityFrameworkCore;
namespace APIREST.Models
{
    public class UsuariosContext: DbContext
    {
        public UsuariosContext(DbContextOptions<UsuariosContext> options) : base(options)
        {
            
        }

        public DbSet<Usuarios> Usuarios { get; set; }
    }
}
