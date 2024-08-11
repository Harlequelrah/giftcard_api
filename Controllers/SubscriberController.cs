using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using giftcard_api.Models;
using System.Text.Json.Serialization;
using giftcard_api.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace giftcard_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriberController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubscriberController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subscriber>>> GetSubscribers()
        {
            return await _context.Subscribers.Include(s=>s.SubscriberWallet).ToListAsync();
        }

        [Authorize(Roles = "SUBSCRIBER,ADMIN")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Subscriber>> GetSubscriber(int id)
        {
            var subscriber = await _context.Subscribers.FindAsync(id);

            if (subscriber == null)
            {
                return NotFound();
            }

            return subscriber;
        }

        [Authorize(Roles = "SUBSCRIBER,ADMIN")]
        [HttpGet("ByUser/{iduser}")]
        public async Task<ActionResult<Subscriber>> GetSubscriberByUser(int iduser)
        {
            var subscriber = await _context.Subscribers
            .Include(s => s.SubscriberWallet)
            .FirstOrDefaultAsync(sw => sw.IdUser == iduser);
            if (subscriber == null)
            {
                return NotFound("Subscriber Not Found");
            }
            return subscriber;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubscriber(int id, Subscriber subscriber)
        {
            if (id != subscriber.Id)
            {
                return BadRequest();
            }

            _context.Entry(subscriber).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriberExists(id))
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

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscriber(int id)
        {
            var subscriber = await _context.Subscribers.FindAsync(id);
            if (subscriber == null)
            {
                return NotFound();
            }

            _context.Subscribers.Remove(subscriber);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "SUBSCRIBER,ADMIN")]
        [HttpGet("beneficiaries/{idSubscriber}")]
        public async Task<ActionResult<IEnumerable<Beneficiary>>> GetBeneficiaries(int idSubscriber)
        {
            var beneficiaries = await _context.Beneficiaries
                 .Where(sh => sh.IdSubscriber == idSubscriber)
                 .ToListAsync();

            if (beneficiaries == null || !beneficiaries.Any())
            {
                return NotFound();
            }

            return beneficiaries;

        }


        [Authorize(Roles = "SUBSCRIBER,ADMIN")]
        [HttpGet("wallet/{idSubscriber}")]
        public async Task<ActionResult<SubscriberWallet>> GetSubscriberWallet(int idSubscriber)
        {
            var subscriber = await _context.Subscribers
                .Include(s => s.SubscriberWallet)
                .FirstOrDefaultAsync(sw => sw.Id == idSubscriber);
            if (subscriber == null)
            {
                return NotFound("Subscriber Not Found");
            }

            var subscriberWallet = subscriber.SubscriberWallet;

            if (subscriberWallet == null)
            {
                return NotFound("SubscriberWallet Not Found");
            }

            return subscriberWallet;
        }
        

        [Authorize(Roles = "SUBSCRIBER,ADMIN")]
        [HttpGet("history/{idSubscriber}")]
        public async Task<ActionResult<IEnumerable<SubscriberHistory>>> GetSubscriberHistories(int idSubscriber)
        {
            var histories = await _context.SubscriberHistories
                .Where(sh => sh.IdSubscriber == idSubscriber)
                .ToListAsync();

            if (histories == null || !histories.Any())
            {
                return NotFound();
            }

            return histories;
        }

        private bool SubscriberExists(int id)
        {
            return _context.Subscribers.Any(e => e.Id == id);
        }
    }
}
