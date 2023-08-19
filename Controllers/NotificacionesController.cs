using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APIREST.Models;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;

namespace APIREST.Controllers
{   
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class NotificacionesController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] Notificaciones notificaciones)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("private_key.json")
                });
            }

  
            var message = new Message()
            {
                Token = notificaciones.Token,

                Notification = new Notification()
                {
                    Title = notificaciones.Titulo,
                    Body = notificaciones.Mensaje,
                    // si por post no mando nada, imageurl dara un exepcion que dice url no valida. aca manejamos eso con un operador ternario (? :)
                    ImageUrl = notificaciones.Url == "" ? "Http://nada.png" : notificaciones.Url
                }
            };

            string response = string.Empty;

            try
            {
                response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
