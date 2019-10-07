using BikeShop.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BikeShop.API.Controllers
 {
     [ApiController]
     [Route("api/[controller]")]
     public class MessageController : ControllerBase
     {
         private readonly HeaderMessageConfiguration _headerMessageConfiguration;

         public MessageController(IOptions<HeaderMessageConfiguration> headerMessageConfiguration)
         {
             _headerMessageConfiguration = headerMessageConfiguration.Value;
         }

         [HttpGet]
         public Message Get()
         {
             if (_headerMessageConfiguration.Visible)
             {
                 return new Message
                 {
                     Title = _headerMessageConfiguration.Title,
                     Body = _headerMessageConfiguration.Body
                 };
             }

             return new Message() { Title = "Error", Body = "Null message or not visible" };
         }
     }
 }