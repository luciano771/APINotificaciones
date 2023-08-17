using System.ComponentModel.DataAnnotations;

namespace APIREST.Models
{
    public class Usuarios
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Email { get; set; }

        public int Edad { get; set; }

        public string? Token { get; set; }

    }
}
