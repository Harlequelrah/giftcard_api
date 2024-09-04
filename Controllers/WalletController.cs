using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using giftcard_api.Models;
using giftcard_api.Services;
using giftcard_api.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace giftcard_api.Controllers
{
    [Authorize(Policy = "IsActive")]
    [Authorize(Roles = "ADMIN")]
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
         private readonly IHubContext<NotificationHub> _hubContext;


        public WalletController(IHubContext<NotificationHub> hubContext,ApplicationDbContext context)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPut("{type}/{idWallet}")]
        public async Task<IActionResult> UpdateWallet(int idWallet, string type, [FromBody] WalletUpdateDto walletUpdateDto)
        {
            Wallet wallet;
            User user  ;
            if (type == "subscriber")
            {
                var subscriber = await _context.Subscribers.FindAsync(walletUpdateDto.Id);
                if (subscriber == null)
                {
                    return NotFound("Souscripteur Non Trouvé");
                }
                 user = await _context.Users.FirstOrDefaultAsync(u => u.Id == subscriber.IdUser);
                var subscriberhistory = new SubscriberHistory
                {
                    Action = SubscriberHistory.SubscriberActions.MaintenanceSolde,
                    IdSubscriber = walletUpdateDto.Id,
                    Montant = walletUpdateDto.Montant,
                    Date = UtilityDate.GetDate(),
                };
                _context.SubscriberHistories.Add(subscriberhistory);
                await _context.SaveChangesAsync();
                wallet = await _context.SubscriberWallets.FindAsync(idWallet);
            }
            else if (type == "beneficiary")
            {
                                var beneficiary = await _context.Beneficiaries.FindAsync(walletUpdateDto.Id);
                if (beneficiary == null)
                {
                    return NotFound("Beneficiaire Non Trouvé");
                } user = await _context.Users.FirstOrDefaultAsync(u => u.Id == beneficiary.IdUser);
                var beneficiaryhistory = new BeneficiaryHistory
                {
                    Action = BeneficiaryHistory.BeneficiaryActions.MaintenanceSolde,
                    IdBeneficiary = walletUpdateDto.Id,
                    Montant = walletUpdateDto.Montant,
                    Date = UtilityDate.GetDate(),
                };
                _context.BeneficiaryHistories.Add(beneficiaryhistory);
                await _context.SaveChangesAsync();
                wallet = await _context.BeneficiaryWallets.FindAsync(idWallet);
            }
            else if (type == "merchant")
            {
                var  merchant = await _context.Merchants.FindAsync(walletUpdateDto.Id);
                if (merchant == null)
                {
                    return NotFound("Marchand Non Trouvé");
                }
                user = await _context.Users.FirstOrDefaultAsync(u => u.Id == merchant.IdUser);
                var merchanthistory = new MerchantHistory
                {
                    Action = MerchantHistory.MerchantActions.MaintenanceSolde,
                    IdMerchant = walletUpdateDto.Id,
                    Montant = walletUpdateDto.Montant,
                    Date = UtilityDate.GetDate(),
                };
                _context.MerchantHistories.Add(merchanthistory);
                await _context.SaveChangesAsync();

                wallet = await _context.MerchantWallets.FindAsync(idWallet);
            }
            else
            {
                return BadRequest("Invalid Wallet Type");
            }

            if (wallet == null)
            {
                return NotFound("Wallet Not Found");
            }
            wallet.Solde = walletUpdateDto.Montant;

            _context.Entry(wallet).State = EntityState.Modified;

            try
            {
                var Id=(user.Id).ToString();
                Console.WriteLine("Id: " + Id);
                var message = "Le solde de votre Carte Cadeau a été mis à jour à  Par Le Support GoChap.";
                await _hubContext.Clients.User(Id).SendAsync("ReceiveMessage", message);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "A problem occurred while updating the wallet.");
            }

            return NoContent();
        }
    }
}
