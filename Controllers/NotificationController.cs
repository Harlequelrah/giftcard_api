using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using giftcard_api.Models;
using giftcard_api.Data;
using giftcard_api.Services;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace giftcard_api.Controllers
{
    [Authorize(Roles="ADMIN")]
    [Authorize(Policy = "IsActive")]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ApplicationDbContext _dbContext;

        public NotificationController(IHubContext<NotificationHub> hubContext, ApplicationDbContext dbContext)
        {
            _hubContext = hubContext;
            _dbContext = dbContext;
        }

        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotification(string userId, string message)
        {
            // Vérifier si l'utilisateur existe dans la base de données
            var userExists = await _dbContext.Users.AnyAsync(u => u.Id == int.Parse(userId));

            if (!userExists)
            {
                return NotFound(new { message = "Utilisateur non trouvé." });
            }

            // Envoyer la notification si l'utilisateur existe
            await _hubContext.Clients.User(userId).SendAsync("ReceiveMessage", message);
            return Ok(new { message = "Notification envoyée avec succès." });
        }
    }

    public class NotificationHub : Hub
    {
 public override async Task OnConnectedAsync()
{
    var userId = Context.UserIdentifier;
    if (string.IsNullOrEmpty(userId))
    {

        Context.Abort();
    }
    else
    {
        // Continuer avec la connexion si l'identifiant existe
        await base.OnConnectedAsync();
    }
}

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            await base.OnDisconnectedAsync(exception);
        }
    }
}
