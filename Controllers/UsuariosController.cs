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
using Microsoft.Extensions.Configuration;
using Azure;
using static Google.Apis.Requests.BatchRequest;
using Newtonsoft.Json.Linq;

namespace APIREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosContext _dbContext;
        private IConfiguration _config;

        public UsuariosController(UsuariosContext dbContext, IConfiguration configuration)
        {
            try
            {
                _dbContext = dbContext;
                _config = configuration;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
         

        [HttpGet]
        [Authorize]
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

            return Ok(new { Nombre = usuario.Nombre, Email = usuario.Email, usuario.Edad });

        }

        [HttpPost]
        [Route("Login")]
        public ActionResult<Usuarios> LoginUsuarios(Usuarios usuario)
        {
            bool usuarioRegistrado = _dbContext.Usuarios.Any(u => u.Nombre == usuario.Nombre && u.Email == usuario.Email && u.Contraseña == usuario.Contraseña);
            ActionResult<Usuarios> response;
            if (!usuarioRegistrado)
            {
                return response = StatusCode(StatusCodes.Status404NotFound, new { Message = "Usuario debe registrarse" });
            }

            var token = GenerateToken(usuario);
            response = Ok(new { Token = token, Nombre = usuario.Nombre, Email = usuario.Email, usuario.Edad });
            return response;

        }



        //FUNCIONES RELACIONADAS A LA GENERACION Y FIRMA DEL TOKEN


        private string GenerateToken(Usuarios users)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, users.Nombre),
                new Claim(ClaimTypes.Email, users.Email)
                // Agregar más claims según necesites
            };

            var token = new JwtSecurityToken(
            null,
            null,
            claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: credentials
           );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }


}
