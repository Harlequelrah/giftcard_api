using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using giftcard_api.Models;
using giftcard_api.Data;
using giftcard_api.Services;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace giftcard_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MerchantController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Merchant>>> GetMerchants()
        {
            return await _context.Merchants.Include(s=>s.MerchantWallet).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Merchant>> GetMerchant(int id)
        {
            var merchant = await _context.Merchants.FindAsync(id);

            if (merchant == null)
            {
                return NotFound();
            }

            return merchant;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutMerchant(int id, Merchant merchant)
        {
            if (id != merchant.Id)
            {
                return BadRequest();
            }

            _context.Entry(merchant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MerchantExists(id))
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
        public async Task<IActionResult> DeleteMerchant(int id)
        {
            var merchant = await _context.Merchants.FindAsync(id);
            if (merchant == null)
            {
                return NotFound();
            }

            _context.Merchants.Remove(merchant);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("wallet/{idMerchant}")]
        public async Task<ActionResult<MerchantWallet>> GetMerchantWallet(int idMerchant)
        {
            var merchant = await _context.Merchants
                .Include(x => x.MerchantWallet)
                .FirstOrDefaultAsync(mw => mw.Id == idMerchant);
            if (merchant == null)
            {
                return NotFound("Merchant not found");
            }
            var merchantWallet = merchant.MerchantWallet;

            if (merchantWallet == null)
            {
                return NotFound();
            }

            return merchantWallet;
        }


        [HttpGet("history/{idMerchant}")]
        public async Task<ActionResult<IEnumerable<MerchantHistory>>> GetMerchantHistories(int idMerchant)
        {
            var histories = await _context.MerchantHistories
                .Where(mh => mh.IdMerchant == idMerchant)
                .ToListAsync();

            if (histories == null || !histories.Any())
            {
                return NotFound();
            }

            return histories;
        }

        private bool MerchantExists(int id)
        {
            return _context.Merchants.Any(e => e.Id == id);
        }
        [HttpPost("processpayement/bymerchant/{idmerchant}")]
        public async Task<IActionResult> ProcessPayement(int idmerchant, PayementDto payementdto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var beneficiary = await _context.Beneficiaries
                        .Include(b => b.BeneficiaryWallet)
                        .FirstOrDefaultAsync(u => u.Id == payementdto.IdBeneficiary);

                    if (beneficiary == null)
                    {
                        return NotFound("beneficiary Not Found");
                    }
                    var beneficiaryWallet = beneficiary.BeneficiaryWallet;
                    if (beneficiaryWallet == null)
                    {
                        return NotFound("BeneficiaryWallet Not Found");
                    }
                    if (beneficiaryWallet.Solde - payementdto.Montant < 0)
                    {
                        return BadRequest("Le solde du portefeuille du beneficiaire ne permet pas le payement");
                    }
                    var merchant = await _context.Merchants
                        .Include(b => b.MerchantWallet)
                        .FirstOrDefaultAsync(u => u.Id == idmerchant);

                    if (merchant == null)
                    {
                        return BadRequest("Merchant Not Found");
                    }
                    var merchantWallet = merchant.MerchantWallet;
                    if (merchantWallet == null)
                    {
                        return BadRequest("MerchantWallet Not Found");
                    }

                    merchantWallet.Solde += payementdto.Montant;
                    _context.Entry(merchantWallet).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    beneficiaryWallet.Solde -= payementdto.Montant;
                    _context.Entry(beneficiaryWallet).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    var merchantHistory = new MerchantHistory
                    {
                        IdMerchant = idmerchant,
                        Montant = payementdto.Montant,
                        Date = UtilityDate.GetDate(),
                        Action = MerchantHistory.MerchantActions.Encaissement,
                    };
                    _context.MerchantHistories.Add(merchantHistory);
                    await _context.SaveChangesAsync();
                    if (beneficiary.Has_gochap)
                    {
                        var beneficiaryHistory = new BeneficiaryHistory
                        {
                            IdBeneficiary = beneficiary.Id,
                            Montant = payementdto.Montant,
                            Date = UtilityDate.GetDate(),
                            Action = BeneficiaryHistory.BeneficiaryActions.Depense,
                        };
                        _context.BeneficiaryHistories.Add(beneficiaryHistory);
                        await _context.SaveChangesAsync();
                    }
                    return Ok(new {  beneficiaryWallet,merchantHistory,merchantWallet });

                }

                catch (Exception ex)
                {
                // Log l'exception et retournez une réponse d'erreur appropriée
                return StatusCode(500, new { message = "Une erreur est survenue.", details = ex.Message });
            }
        }
            return BadRequest(ModelState);
    }
}
}
