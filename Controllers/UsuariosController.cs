using APIREST.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;


namespace APIREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosContext _dbContext;

        public UsuariosController(UsuariosContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuarios()
        {
            if (_dbContext.Usuarios == null)
            {
                return NotFound();
            }
            return await _dbContext.Usuarios.ToListAsync();
        }
        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<Usuarios>>> GetUsuarios([FromQuery] string nombre)
        {
            if (_dbContext.Usuarios == null)
            {
                return NotFound();
            }

            var usuarios = await _dbContext.Usuarios.Where(u => u.Nombre.Contains(nombre)).ToListAsync(); // funciona como un like

            if (usuarios.Count == 0)
            {
                return NotFound();
            }

            return usuarios;
        }





        [HttpPost("Registro")]
        public async Task<ActionResult<Usuarios>> RegistroUsuarios(Usuarios usuario)
        {
            bool usuarioRegistrado = _dbContext.Usuarios.Any(u => u.Nombre == usuario.Nombre || u.Email == usuario.Email);

            if (usuarioRegistrado)
            {
                return BadRequest(new { Message = "El usuario ya está registrado" });
            }

            _dbContext.Usuarios.Add(usuario);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuarios), new { id = usuario.Id }, usuario);

        }

        [HttpPost("Login")]
        public ActionResult<Usuarios> LoginUsuarios(Usuarios usuario)
        {
            bool usuarioRegistrado = _dbContext.Usuarios.Any(u => u.Nombre == usuario.Nombre || u.Email == usuario.Email);

            if (!usuarioRegistrado)
            {
                return BadRequest(new { Message = "El usuario no está registrado" });
            }

            return Ok(new { Message = "El usuario está registrado" });


        }



    }


}
