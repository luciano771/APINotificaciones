using System.ComponentModel.DataAnnotations;

namespace APIREST.Models
{
    public class Notificaciones
    {
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public string Url { get; set; }
        public string Token { get; set; }
    }
}
