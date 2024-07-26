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

    // [Authorize(Roles="ADMIN")]
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


        [HttpPost]
        public async Task<ActionResult<Role>> PostRole(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, RoleDto roledto)
        {
            if ((id <= 0 ) || (id!=roledto.Id))
            {
                return BadRequest("Invalid ID");
            }
            var role = new Role{
                Id = roledto.Id,
                RoleNom = roledto.RoleNom
            };

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


    }
}
