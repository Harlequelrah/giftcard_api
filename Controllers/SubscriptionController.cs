using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using giftcard_api.Models;
using giftcard_api.Data;
using giftcard_api.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace giftcard_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get: api/Subscription
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptions()
        {
            return await _context.Subscriptions.ToListAsync();
        }

        // Get: api/Subscription/Subscriber/{subscriberId}
        [HttpGet("Subscriber/{subscriberId}")]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptionsBySubscriber(int subscriberId)
        {
            return await _context.Subscriptions
                                 .Where(s => s.IdSubscriber == subscriberId)
                                 .ToListAsync();
        }

        // Get: api/Subscription/Package/{packageId}
        [HttpGet("Package/{packageId}")]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptionsByPackage(int packageId)
        {
            return await _context.Subscriptions
                                 .Where(s => s.IdPackage == packageId)
                                 .ToListAsync();
        }

        // Get: api/Subscription/{subscriberId}/{packageId}
        [HttpGet("{subscriberId}/{packageId}")]
        public async Task<ActionResult<Subscription>> GetSubscription(int subscriberId, int packageId)
        {
            var subscription = await _context.Subscriptions
                                             .FirstOrDefaultAsync(s => s.IdSubscriber == subscriberId && s.IdPackage == packageId);

            if (subscription == null)
            {
                return NotFound();
            }

            return subscription;
        }

        // Post: api/Subscription
        [HttpPost]
        public async Task<ActionResult<Subscription>> PostSubscription(SubscriptionDto subscriptiondto)
        {
            var package = await _context.Packages.FindAsync(subscriptiondto.IdPackage);
            if (package == null)
            {
                return NotFound("Package not found");
            }
            var subscriber = await _context.Subscribers.FindAsync(subscriptiondto.IdSubscriber);
            if (subscriber == null)
            {
                return NotFound("Subscriber not found");
            }
            var subscription = new Subscription
            {
                IdSubscriber = subscriptiondto.IdSubscriber,
                IdPackage = subscriptiondto.IdPackage,
                DateSouscription = UtilityDate.GetDate(),
                DateExpiration = DateTime.UtcNow.AddDays(package.NbrJour),
            };
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            var subscriberhistory = new SubscriberHistory
            {
                Action=SubscriberHistory.SubscriberActions.Souscription,
                IdSubscriber=subscriptiondto.IdSubscriber,
                Montant = subscription.Package.Budget,
                Date = UtilityDate.GetDate(),
            };

            _context.SubscriberHistories.Add(subscriberhistory);
            await _context.SaveChangesAsync();
            SubscriberWallet subscriberwallet=subscriber.SubscriberWallet;
            subscriberwallet.Solde += package.Budget;

            _context.Entry(subscriberwallet).State = EntityState.Modified;
            await _context.SaveChangesAsync();



            return CreatedAtAction("GetSubscription", new { subscriberId = subscription.IdSubscriber, packageId = subscription.IdPackage }, subscription);
        }

        // Put: api/Subscription/{subscriberId}/{packageId}
        [HttpPut("{subscriberId}/{packageId}")]
        public async Task<IActionResult> PutSubscription(int subscriberId, int packageId, Subscription subscription)
        {
            if (subscriberId != subscription.IdSubscriber || packageId != subscription.IdPackage)
            {
                return BadRequest();
            }

            _context.Entry(subscription).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriptionExists(subscriberId, packageId))
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

        // Delete: api/Subscription/{subscriberId}/{packageId}
        [HttpDelete("{subscriberId}/{packageId}")]
        public async Task<IActionResult> DeleteSubscription(int subscriberId, int packageId)
        {
            var subscription = await _context.Subscriptions
                                             .FirstOrDefaultAsync(s => s.IdSubscriber == subscriberId && s.IdPackage == packageId);
            if (subscription == null)
            {
                return NotFound();
            }

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubscriptionExists(int subscriberId, int packageId)
        {
            return _context.Subscriptions.Any(e => e.IdSubscriber == subscriberId && e.IdPackage == packageId);
        }
    }
}
