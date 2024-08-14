using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using giftcard_api.Models;
using giftcard_api.Services;
using giftcard_api.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace giftcard_api.Controllers
{
    [Authorize(Policy = "IsActive")]
    [Authorize(Roles = "ADMIN")]
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WalletController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPut("{type}/{idWallet}")]
        public async Task<IActionResult> UpdateWallet(int idWallet, string type, [FromBody] WalletUpdateDto walletUpdateDto)
        {
            Wallet wallet;
            if (type == "subscriber")
            {
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
