using APIREST.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace APIREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase


    {
        private readonly UsuariosContext _dbContext;
        private readonly string? secretkey;

        public UsuariosController(UsuariosContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            secretkey = config.GetSection("settings").GetSection("secretkey").ToString();
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



 
        [HttpPost]
        [Route("Registro")]
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

        [HttpPost]
        [Route("Login")]
        public ActionResult<Usuarios> LoginUsuarios(Usuarios usuario)
        {
            bool usuarioRegistrado = _dbContext.Usuarios.Any(u => u.Nombre == usuario.Nombre && u.Email == usuario.Email);

            if (!usuarioRegistrado)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { Message = "Usuario debe registrarse" });
            }

            var KeyBytes = Encoding.ASCII.GetBytes(secretkey);
            var claims = new ClaimsIdentity();

            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Email));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(KeyBytes), SecurityAlgorithms.HmacSha256Signature),
                
            };


            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            string tokencreado = tokenHandler.WriteToken(tokenConfig);

            return StatusCode(StatusCodes.Status200OK,new { Message = tokencreado });


        }



    }


}
