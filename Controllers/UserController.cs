using Microsoft.AspNetCore.Mvc;
using giftcard_api.Models;
using giftcard_api.Data;
using giftcard_api.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace giftcard_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _configuration;


        public UserController(ApplicationDbContext context, JwtService jwtService, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _jwtService = jwtService;
        }
        [HttpPost("register/subscriber")]
        public async Task<IActionResult> RegisterSubscriber(SubscriberDto subscriberdto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Vérifiez si l'utilisateur existe déjà
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == subscriberdto.Email);
                    if (existingUser != null)
                    {
                        return BadRequest(new
                        {
                            errors = new Dictionary<string, string[]>
                        {
                            { "email", new[] { "Ce Email est déjà utilisé." } }
                        }
                        });
                    }
                    var user = new User
                    {
                        IdRole = 2,
                        Email = subscriberdto.Email,
                        Password = subscriberdto.Password,
                        Adresse = subscriberdto.Adresse,
                        Telephone = subscriberdto.Telephone,
                        DateInscription = DateTime.UtcNow,
                        RefreshToken = _jwtService.GenerateRefreshToken(),
                        RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
                        // Ajouter d'autres propriétés si nécessaire
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    var subscriberWallet = new SubscriberWallet();
                    _context.SubscriberWallets.Add(subscriberWallet);
                    await _context.SaveChangesAsync();
                    var subscriber = new Subscriber
                    {
                        IdUser = user.Id,
                        IdSubscriberWallet = subscriberWallet.Id,
                        SubscriberName = subscriberdto.SubscriberName
                    };
                    _context.Subscribers.Add(subscriber);
                    await _context.SaveChangesAsync();
                    var subscriberHistory = new SubscriberHistory
                    {
                        IdSubscriber = subscriber.Id,
                        Montant = 0.0,
                        Date = DateTime.UtcNow,
                        Action = SubscriberHistory.SubscriberActions.Initial,
                    };
                    _context.SubscriberHistories.Add(subscriberHistory);
                    await _context.SaveChangesAsync();

                    var token =  await _jwtService.GenerateToken(user);
                    return Ok(new { Token = token, user, subscriber, subscriberWallet, subscriberHistory });
                }
                catch (Exception ex)
                {
                    // Log l'exception et retournez une réponse d'erreur appropriée
                    return StatusCode(500, new { message = "Une erreur est survenue.", details = ex.Message });
                }
            }
            return BadRequest(ModelState);
        }
        [HttpPost("register/merchant")]
        public async Task<IActionResult> RegisterMerchant(MerchantDto merchantdto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Vérifiez si l'utilisateur existe déjà
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == merchantdto.Email);
                    if (existingUser != null)
                    {
                        return BadRequest(new
                        {
                            errors = new Dictionary<string, string[]>
                        {
                            { "email", new[] { "Ce Email est déjà utilisé." } }
                        }
                        });
                    }
                    var user = new User
                    {
                        IdRole = 3,
                        Email = merchantdto.Email,
                        Password = merchantdto.Password,
                        Adresse = merchantdto.Adresse,
                        Telephone = merchantdto.Telephone,
                        DateInscription = DateTime.UtcNow,
                        RefreshToken = _jwtService.GenerateRefreshToken(),
                        RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
                        // Ajouter d'autres propriétés si nécessaire
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    var merchantWallet = new MerchantWallet();
                    _context.MerchantWallets.Add(merchantWallet);
                    await _context.SaveChangesAsync();
                    var merchant = new Merchant
                    {
                        IdUser = user.Id,
                        IdMerchantWallet = merchantWallet.Id,
                        Nom = merchantdto.Nom,
                        Prenom = merchantdto.Prenom,
                    };
                    _context.Merchants.Add(merchant);
                    await _context.SaveChangesAsync();
                    var merchantHistory = new MerchantHistory
                    {
                        IdMerchant = merchant.Id,
                        Montant = 0.0,
                        Date = DateTime.UtcNow,
                        Action = MerchantHistory.MerchantActions.Initial,
                    };
                    _context.MerchantHistories.Add(merchantHistory);
                    await _context.SaveChangesAsync();

                    var token = await _jwtService.GenerateToken(user);
                    return Ok(new { Token = token, user, merchant, merchantWallet, merchantHistory });
                }
                catch (Exception ex)
                {
                    // Log l'exception et retournez une réponse d'erreur appropriée
                    return StatusCode(500, new { message = "Une erreur est survenue.", details = ex.Message });
                }
            }

            return BadRequest(ModelState);
        }
        [HttpPost("register/beneficiary")]
        public async Task<IActionResult> RegisterBeneficiary(BeneficiaryDto beneficiarydto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Vérifiez si l'utilisateur existe déjà
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == beneficiarydto.Email);
                    if (existingUser != null)
                    {
                        return BadRequest(new
                        {
                            errors = new Dictionary<string, string[]>
                        {
                            { "email", new[] { "Ce Email est déjà utilisé." } }
                        }
                        });
                    }
                    var user = new User
                    {
                        IdRole = 1,
                        Email = beneficiarydto.Email,
                        Password = beneficiarydto.Password,
                        Adresse = beneficiarydto.Adresse,
                        Telephone = beneficiarydto.Telephone,
                        DateInscription = DateTime.UtcNow,
                        RefreshToken = _jwtService.GenerateRefreshToken(),
                        RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
                        // Ajouter d'autres propriétés si nécessaire
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    if (beneficiarydto.Has_gochap)
                    {
                        var beneficiaryWallet = new BeneficiaryWallet();
                        _context.BeneficiaryWallets.Add(beneficiaryWallet);
                        await _context.SaveChangesAsync();
                        var beneficiary = new Beneficiary
                        {

                            IdUser = user.Id,
                            IdBeneficiaryWallet = beneficiaryWallet.Id,
                            Nom = beneficiarydto.Nom,
                            Prenom = beneficiarydto.Prenom,
                            Has_gochap = beneficiarydto.Has_gochap,
                        };
                        _context.Beneficiaries.Add(beneficiary);
                        await _context.SaveChangesAsync();
                        var beneficiaryHistory = new BeneficiaryHistory
                        {
                            IdBeneficiary = beneficiary.Id,
                            Montant = 0.0,
                            Date = DateTime.UtcNow,
                            Action = BeneficiaryHistory.BeneficiaryActions.Initial,
                        };
                        _context.BeneficiaryHistories.Add(beneficiaryHistory);
                        await _context.SaveChangesAsync();


                        var token = await _jwtService.GenerateToken(user);
                        return Ok(new { Token = token, user, beneficiary, beneficiaryWallet, beneficiaryHistory });

                    }
                    else
                    {
                        var beneficiary = new Beneficiary
                        {
                            IdUser = user.Id,
                            Nom = beneficiarydto.Nom,
                            Prenom = beneficiarydto.Prenom,
                            Has_gochap = beneficiarydto.Has_gochap,
                        };
                        _context.Beneficiaries.Add(beneficiary);
                        await _context.SaveChangesAsync();
                        await _context.SaveChangesAsync();
                        var token = await _jwtService.GenerateToken(user);
                        return Ok(new { Token = token, user, beneficiary });
                    }
                }
                catch (Exception ex)
                {
                    // Log l'exception et retournez une réponse d'erreur appropriée
                    return StatusCode(500, new { message = "Une erreur est survenue.", details = ex.Message });
                }
            }
            return BadRequest(ModelState);
        }




        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);

            if (existingUser != null)
            {
                // Générez un jeton JWT pour l'utilisateur authentifié
                var token = _jwtService.GenerateToken(existingUser);
                var refreshToken = _jwtService.GenerateRefreshToken();
                _jwtService.SaveRefreshToken(existingUser, refreshToken);

                return Ok(new { Token = token });
            }

            return Unauthorized("Nom d'utilisateur ou mot de passe incorrect.");
        }
        [Authorize]
        [HttpGet("get-refresh-token")]
        public async Task<IActionResult> GetRefreshToken()
        {
            var email = User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user.RefreshToken);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest refreshRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshRequest.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized("Invalid refresh token.");
            }

            // Génération du nouveau token
            var newToken = await _jwtService.GenerateToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            _jwtService.SaveRefreshToken(user, newRefreshToken);

            return Ok(new { Token = newToken });
        }

        public class RefreshRequest
        {
            public string RefreshToken { get; set; }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
        [Authorize]
        [HttpGet("byrole/{idRole}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(int idRole)
        {
            var usersByRole = await _context.Users
                                            .Where(user => user.IdRole == idRole)
                                            .ToListAsync();

            if (usersByRole == null || !usersByRole.Any())
            {
                return NotFound();
            }

            return Ok(usersByRole);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (GetUser == null)
            {
                return NotFound();
            }
            return user;
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }
            _context.Entry(user).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.Id == id))
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
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}
