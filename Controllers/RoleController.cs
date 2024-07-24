using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using giftcard_api.Models;
using giftcard_api.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace giftcard_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return role;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Role>> PostRole(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, Role role)
        {
            if (id != role.Id)
            {
                return BadRequest();
            }

            _context.Entry(role).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
        [Authorize]
        [HttpPut("update-user-role/{userId}/{roleId}")]
        public async Task<IActionResult> UpdateUserRole(int userId, int roleId)
        {
            // Récupération de l'utilisateur avec l'Id spécifié
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(); // L'utilisateur n'existe pas
            }

            // Vérification que le rôle existe
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                return NotFound(); // Le rôle n'existe pas
            }

            // Mise à jour du champ IdRole de l'utilisateur
            user.IdRole = roleId;

            // Indiquer que l'entité User a été modifiée
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                // Sauvegarde des changements dans la base de données
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Gestion des exceptions de concurrence
                if (!_context.Users.Any(e => e.Id == userId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
    }
}
