using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using giftcard_api.Models;
using giftcard_api.Data;
using giftcard_api.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptions()
        {
            return await _context.Subscriptions.ToListAsync();
        }

        [Authorize(Roles = "SUBSCRIBER,ADMIN")]
        [HttpGet("Subscriber/{subscriberId}")]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptionsBySubscriber(int subscriberId)
        {
            return await _context.Subscriptions
                                 .Where(s => s.IdSubscriber == subscriberId)
                                 .ToListAsync();
        }
        [Authorize(Roles = "SUBSCRIBER,ADMIN")]
        [HttpGet("ForSubscriber/{subscriberId}")]
        public async Task<ActionResult<IEnumerable<SubscriberSubscriptionDto>>> GetSubscriptionsForSubscriber(int subscriberId)
        {
            var Subscriptions = await _context.Subscriptions
                .Include(s=>s.Package)
                .Where(s => s.IdSubscriber == subscriberId)
                .ToListAsync();
            List<SubscriberSubscriptionDto> SendingSubscriptions = new List<SubscriberSubscriptionDto>();
                foreach(var subscription in Subscriptions)
                {  var montant = subscription.MontantParCarte.HasValue ? subscription.MontantParCarte : subscription.Package.MontantBase;
                    var formatdate =subscription.DateExpiration.HasValue ? UtilityDate.FormatDate((DateTime)subscription.DateExpiration) : null;
                    var sendingsubscription = new SubscriberSubscriptionDto
                    {
                        Id = subscription.Id,
                        NomPackage = subscription.Package.NomPackage,
                        NbrCarteGenere= subscription.NbrCarteGenere,
                        BudgetRestant = subscription.BudgetRestant,
                        DateSouscription = subscription.DateSouscription,
                        DateExpiration = formatdate,
                        MontantParCarte = montant,
                    };
                    SendingSubscriptions.Add(sendingsubscription);

                }
                return SendingSubscriptions;
        }

        [Authorize(Roles = "SUBSCRIBER,ADMIN")]
        [HttpGet("{Idsubscription}")]
        public async Task<ActionResult<Subscription>> GetSubscription(int Idsubscription)
        {
            var subscription = await _context.Subscriptions.FindAsync(Idsubscription);
            if (subscription == null)
            {
                return NotFound();
            }
            return subscription;
        }


        [Authorize(Roles = "ADMIN")]
        [HttpGet("Package/{packageId}")]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptionsByPackage(int packageId)
        {
            return await _context.Subscriptions
                                 .Where(s => s.IdPackage == packageId)
                                 .ToListAsync();
        }

        // Get: api/Subscription/{subscriberId}/{packageId}

        [Authorize(Roles = "SUBSCRIBER,ADMIN")]
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
                MontantParCarte = subscriptiondto.MontantParCarte,
                BudgetRestant = package.Budget,
                DateSouscription = UtilityDate.GetDate(),
                DateExpiration = package.NbrJour.HasValue ? DateTime.UtcNow.AddDays(package.NbrJour.Value) : (DateTime?)null

            };
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            var subscriberhistory = new SubscriberHistory
            {
                Action = SubscriberHistory.SubscriberActions.Souscription,
                IdSubscriber = subscriptiondto.IdSubscriber,
                Montant = subscription.Package.Budget,
                Date = UtilityDate.GetDate(),
            };

            _context.SubscriberHistories.Add(subscriberhistory);
            await _context.SaveChangesAsync();
            var wallet = await _context.SubscriberWallets.FindAsync(subscriber.IdSubscriberWallet);
            if (wallet == null)
            {
                return NotFound("Wallet not found");
            }
            var nouveausolde = wallet.Solde + package.Budget;
            wallet.Solde = nouveausolde;
            _context.Entry(wallet).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSubscription), new { Idsubscription = subscription.Id ,subscriberId = subscription.IdSubscriber, packageId = subscription.IdPackage }, subscription);
        }
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Subscription(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubscription(int id, Subscription subscription)
        {
            if (id != subscription.Id)
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
                bool subscriptionExists = _context.Subscriptions.Any(e => e.Id == id);
                if (!subscriptionExists)
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
