using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using giftcard_api.Models;
using giftcard_api.Services;
using giftcard_api.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;


namespace giftcard_api.Controllers
{
    [Authorize(Policy = "IsActive")]
    [Authorize(Roles="BENEFICIARY,SUBSCRIBER,MERCHANT")]
    [Route("api/[controller]")]
    [ApiController]
    public class BeneficiaryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public BeneficiaryController(ApplicationDbContext context,JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Beneficiary>>> GetBeneficiaries()
        {
            return await _context.Beneficiaries.Include(s=>s.BeneficiaryWallet).ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Beneficiary>> GetBeneficiary(int id)
        {
            var beneficiary = await _context.Beneficiaries.FindAsync(id);

            if (beneficiary == null)
            {
                return NotFound();
            }

            return beneficiary;
        }

        [HttpGet("User/{id}")]
        public async Task<ActionResult<AppUser>> GetBeneficiaryUser(int id)
        {
            var beneficiary = await _context.Beneficiaries
                .Include(b => b.BeneficiaryWallet)
                .FirstOrDefaultAsync(b => b.Id == id);
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Utilisateur Non Trouvé");
            }
            if (beneficiary == null)
            {
                return NotFound("Beneficiaire Non Trouvé");
            }
            var solde = $"{beneficiary.BeneficiaryWallet.Devise} {beneficiary.BeneficiaryWallet.Solde}";
            var beneficiaryuser = new AppUser()
            {
                SpecialId= beneficiary.Id,
                NomComplet=user.NomComplet,
                Solde=solde,
                Email=user.Email
            };
            return beneficiaryuser;
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBeneficiary(int id, Beneficiary beneficiary)
        {
            if (id != beneficiary.Id)
            {
                return BadRequest();
            }

            _context.Entry(beneficiary).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BeneficiaryExists(id))
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
        public async Task<IActionResult> DeleteBeneficiary(int id)
        {
            var beneficiary = await _context.Beneficiaries.FindAsync(id);
            if (beneficiary == null)
            {
                return NotFound();
            }

            _context.Beneficiaries.Remove(beneficiary);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("hasgochap/{hasGochap}")]
        public async Task<ActionResult<IEnumerable<Beneficiary>>> GetBeneficiariesByHasGochap(bool hasGochap)
        {
            var beneficiaries = await _context.Beneficiaries
                .Where(b => b.Has_gochap == hasGochap)
                .ToListAsync();

            if (beneficiaries == null || !beneficiaries.Any())
            {
                return NotFound();
            }

            return beneficiaries;
        }


        [HttpGet("wallet/{idBeneficiary}")]
        public async Task<ActionResult<BeneficiaryWallet>> GetBeneficiaryWallet(int idBeneficiary)
        {
            var beneficiary = await _context.Beneficiaries
                .Include(b => b.BeneficiaryWallet)
                .FirstOrDefaultAsync(bw => bw.Id == idBeneficiary);
            if (beneficiary == null || !beneficiary.Has_gochap)
            {
                return NotFound("Beneficiary Not Found Or Does Not Have Gochap");
            }


            var beneficiaryWallet = beneficiary.BeneficiaryWallet;

            if (beneficiaryWallet == null)
            {
                return NotFound("Wallet Not Found");
            }

            return beneficiaryWallet;
        }


        [HttpGet("history/{idBeneficiary}")]
        public async Task<ActionResult<IEnumerable<BeneficiaryHistory>>> GetBeneficiaryHistories(int idBeneficiary)
        {
            var beneficiary = await _context.Beneficiaries.FindAsync(idBeneficiary);

            if (beneficiary == null || !beneficiary.Has_gochap)
            {
                return NotFound();
            }

            var histories = await _context.BeneficiaryHistories
                .Where(bh => bh.IdBeneficiary == idBeneficiary)
                .ToListAsync();

            if (histories == null || !histories.Any())
            {
                return NotFound();
            }

            return histories;
        }

        private bool BeneficiaryExists(int id)
        {
            return _context.Beneficiaries.Any(e => e.Id == id);
        }
    }
}
