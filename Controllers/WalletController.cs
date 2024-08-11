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

        [HttpPut("{idWallet}")]
        public async Task<IActionResult> UpdateWallet(int idWallet, [FromBody] WalletUpdateDto walletUpdateDto)
        {
            var wallet = await _context.SubscriberWallets.FindAsync(idWallet);
            if (wallet == null)
            {
                return NotFound("Wallet Not Found");
            }
            wallet.Solde += walletUpdateDto.Montant;

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
