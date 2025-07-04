using Microsoft.AspNetCore.Mvc;
using giftcard_api.Models;
using giftcard_api.Data;
using giftcard_api.Services;
using System.Collections.Generic;
using BCrypt.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
namespace giftcard_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;
        private readonly RegisterBeneficiaryService _registerBeneficiaryService;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;


        public UserController(IHubContext<NotificationHub> hubContext, ApplicationDbContext context, JwtService jwtService, IConfiguration configuration, EmailService emailService, RegisterBeneficiaryService registerBeneficairyService)
        {
            _context = context;
            _configuration = configuration;
            _jwtService = jwtService;
            _emailService = emailService;
            _hubContext = hubContext;
            _registerBeneficiaryService = registerBeneficairyService;
        }




        [Authorize(Policy = "IsActive")]
        [Authorize]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest resetUser)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == resetUser.Email);
                    if (existingUser == null) return NotFound("Utilisateur Non Trouvé");
                    else
                    {
                        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(resetUser.CurrentPassword, existingUser.Password);
                        if (!isPasswordValid) return Unauthorized(new { message = "Email ou mot de passe incorrect." });
                        existingUser.Password = BCrypt.Net.BCrypt.HashPassword(resetUser.NewPassword);
                        _context.Entry(existingUser).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Une erreur est survenue.", details = ex.Message });
                }
            }
            return BadRequest(ModelState);

        }



        [Authorize(Policy = "IsActive")]
        [Authorize(Roles = "ADMIN")]
        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin(UserDto userdto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userdto.Email);
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
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userdto.Password);
                    var user = new User
                    {
                        IdRole = 4,
                        NomComplet = userdto.NomComplet,
                        Email = userdto.Email,
                        Password = hashedPassword,
                        Adresse = userdto.Adresse,
                        Telephone = userdto.Telephone,
                        DateInscription = UtilityDate.GetDate(),
                        RefreshToken = _jwtService.GenerateRefreshToken(),
                        RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();


                    string token = await _jwtService.GenerateToken(user);
                    Console.WriteLine($"userdto.Password:{userdto.Password}");
                    bool emailresponse = await _emailService.SendAdminRegistrationEmailAsync(user.Email, user.NomComplet, userdto.Password);
                    return Ok(new { Token = token, EmailResponse = emailresponse });
                }
                catch (Exception ex)
                {

                    return StatusCode(500, new { message = "Une erreur est survenue.", details = ex.Message });
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("register/user")]
        public async Task<IActionResult> RegisterUser(UserDto userdto)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userdto.Email);
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
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userdto.Password);
                    var user = new User
                    {
                        Email = userdto.Email,
                        NomComplet = userdto.NomComplet,
                        Password = hashedPassword,
                        Adresse = userdto.Adresse,
                        Telephone = userdto.Telephone,
                        DateInscription = UtilityDate.GetDate(),
                        RefreshToken = _jwtService.GenerateRefreshToken(),
                        RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)

                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();


                    string token = await _jwtService.GenerateToken(user);
                    return Ok(new { Token = token });
                }
                catch (Exception ex)
                {

                    return StatusCode(500, new { message = "Une erreur est survenue.", details = ex.Message });
                }
            }
            return BadRequest(ModelState);
        }
        [HttpPost("register/subscriber")]
        public async Task<IActionResult> RegisterSubscriber(SubscriberDto subscriberdto)
        {
            if (ModelState.IsValid)
            {
                try
                {

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
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(subscriberdto.Password);
                    var user = new User
                    {
                        IdRole = 2,
                        Email = subscriberdto.Email,
                        Password = hashedPassword,
                        NomComplet = subscriberdto.SubscriberName,
                        Adresse = subscriberdto.Adresse,
                        Telephone = subscriberdto.Telephone,
                        DateInscription = UtilityDate.GetDate(),
                        RefreshToken = _jwtService.GenerateRefreshToken(),
                        RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)

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
                        Date = UtilityDate.GetDate(),
                        Action = SubscriberHistory.SubscriberActions.Initial,
                    };
                    _context.SubscriberHistories.Add(subscriberHistory);
                    await _context.SaveChangesAsync();

                    var token = await _jwtService.GenerateToken(user);
                    await _emailService.SendSubscriberRegistrationEmailAsync(user.Email,subscriber.SubscriberName);
                    return Ok(new { Token = token, subscriber, subscriberWallet, subscriberHistory });
                }
                catch (Exception ex)
                {

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
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(merchantdto.Password);
                    var nomcomplet = $"{merchantdto.Nom} {merchantdto.Prenom}";
                    var user = new User
                    {
                        IdRole = 3,
                        Email = merchantdto.Email,
                        NomComplet = nomcomplet,
                        Password = hashedPassword,
                        Adresse = merchantdto.Adresse,
                        Telephone = merchantdto.Telephone,
                        DateInscription = UtilityDate.GetDate(),
                        RefreshToken = _jwtService.GenerateRefreshToken(),
                        RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)

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
                        Date = UtilityDate.GetDate(),
                        Action = MerchantHistory.MerchantActions.Initial,
                    };
                    _context.MerchantHistories.Add(merchantHistory);
                    await _context.SaveChangesAsync();

                    var token = await _jwtService.GenerateToken(user);
                    return Ok(new { Token = token, merchant, merchantWallet, merchantHistory });
                }
                catch (Exception ex)
                {

                    return StatusCode(500, new { message = "Une erreur est survenue.", details = ex.Message });
                }
            }

            return BadRequest(ModelState);
        }
        [Authorize(Policy = "IsActive")]
        [Authorize(Roles = "SUBSCRIBER")]
        [HttpPost("register/beneficiary/bysubscriber/{idsubscriber}/value/{amount}")]
        public async Task<IActionResult> RegisterBeneficiary(int idsubscriber, double? amount, BeneficiaryDto beneficiarydto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var subscription = await _context.Subscriptions
                        .Include(s => s.Package)
                        .Include(s => s.Subscriber)
                            .ThenInclude(sub => sub.SubscriberWallet)
                        .FirstOrDefaultAsync(u => u.Id == beneficiarydto.IdSubscription);
                    if (subscription == null)
                    {
                        return NotFound("Subscription Not Found");
                    }
                    var package = subscription.Package;
                    if (package == null)
                    {
                        return NotFound("Package Not Found");
                    }
                    double? cartecadeau;

                    if (amount == -1.0)
                    {
                        cartecadeau = subscription.MontantParCarte == null ? package.MontantBase : subscription.MontantParCarte;
                    }
                    else
                    {
                        cartecadeau = amount;
                    }

                    if (subscription.BudgetRestant - cartecadeau < 0)
                    {
                        return BadRequest("Le budget restant n'est pas suffisant pour générer une carte de cadeau");
                    }
                    if ((DateTime.UtcNow >= subscription.DateExpiration) && (subscription.DateExpiration != null))
                    {
                        return BadRequest("La souscription est  expirée");
                    }

                    subscription.NbrCarteGenere++;
                    subscription.BudgetRestant -= (double)cartecadeau;
                    _context.Entry(subscription).State = EntityState.Modified;
                    await _context.SaveChangesAsync();


                    var subscriber = subscription.Subscriber;
                    if (subscriber == null)
                    {
                        return NotFound("Subscriber Not Found");
                    }

                    var wallet = subscriber.SubscriberWallet;
                    if (wallet == null)
                    {
                        return NotFound("SubscriberWallet Not Found");
                    }

                    var nouveausolde = wallet.Solde - cartecadeau;
                    wallet.Solde = nouveausolde ?? 0.0;
                    _context.Entry(wallet).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    var subscriberHistory = new SubscriberHistory
                    {
                        IdSubscriber = subscriber.Id,
                        Montant = cartecadeau ?? 0.0,
                        Date = UtilityDate.GetDate(),
                        Action = SubscriberHistory.SubscriberActions.Enregistrement,
                    };
                    _context.SubscriberHistories.Add(subscriberHistory);
                    await _context.SaveChangesAsync();

                    if (beneficiarydto.Has_gochap)
                    {
                        var Message = "";
                        bool? rechargemailresponse = null;
                        Console.WriteLine("Le beneficiaire a gochap");
                        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == beneficiarydto.Email);
                        if (existingUser == null)
                        {
                            return NotFound("Utilisateur Non trouvé");
                        }

                        var existingUserBeneficiary = await _context.Beneficiaries
                                            .Include(b => b.BeneficiaryWallet)
                                            .FirstOrDefaultAsync(b => b.IdUser == existingUser.Id);

                        if (existingUserBeneficiary == null)
                        {

                            Console.WriteLine("Il a jamais été beneficiaire");
                            var newbeneficiary = await _registerBeneficiaryService.RegisterGoChapNewBeneficiary(beneficiarydto, existingUser, idsubscriber);
                            if (newbeneficiary == null)
                            {
                                Console.WriteLine("Enregistrement non effective");
                            }

                            else
                            {

                                Console.WriteLine("Enregistrement non effective");
                                var beneficiary = await _registerBeneficiaryService.RegisterGoChapOldBeneficiary(newbeneficiary, cartecadeau);
                                if (newbeneficiary == null) Console.WriteLine("Mise a jour non effective");
                                else Console.WriteLine("Mise a jour  effective");
                            }
                        }
                        else
                        {
                            Message = "Le bénéficiaire a déjà un souscripteur ainsi n'apparaitra pas dans votre liste de bénéficiare mais aura son  solde a  mise à jour";
                            Console.WriteLine("Il a deja été beneficiaire");
                            var beneficiary = await _registerBeneficiaryService.RegisterGoChapOldBeneficiary(existingUserBeneficiary, cartecadeau);


                            rechargemailresponse = await _emailService.SendRechargeEmailAsync(beneficiary.Email,$"{beneficiary.Nom} {beneficiary.Prenom}",subscriber.SubscriberName,$"{cartecadeau}");


                        }
                        var message = $"Votre Carte Cadeau a eté rechargée d'un montant de {cartecadeau} Par Le Souscripteur {subscriber.SubscriberName}";
                        await _hubContext.Clients.User(existingUser.Id.ToString()).SendAsync("ReceiveMessage", message);



                        return Ok(new {Message = Message,Emailresponse=rechargemailresponse});

                    }
                    else
                    {
                        var beneficiary = new Beneficiary();
                        var existingBeneficiary = await _context.Beneficiaries.FirstOrDefaultAsync(x => x.Email == beneficiarydto.Email && x.IdSubscriber == idsubscriber);
                        if (existingBeneficiary != null)
                        {
                            var existingBeneficiaryWallet = await _context.BeneficiaryWallets.FindAsync(existingBeneficiary.IdBeneficiaryWallet);
                            existingBeneficiaryWallet.Solde += (double)cartecadeau;
                            _context.Entry(existingBeneficiaryWallet).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                            beneficiary = existingBeneficiary;
                            var beneficiaryHistory3 = new BeneficiaryHistory
                            {
                                IdBeneficiary = beneficiary.Id,
                                Montant = cartecadeau ?? 0.0,
                                Date = UtilityDate.GetDate(),
                                Action = BeneficiaryHistory.BeneficiaryActions.Recharge,
                            };
                            _context.BeneficiaryHistories.Add(beneficiaryHistory3);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            var beneficiaryWallet = new BeneficiaryWallet
                            {
                                Solde = (double)cartecadeau
                            };
                            _context.BeneficiaryWallets.Add(beneficiaryWallet);
                            await _context.SaveChangesAsync();
                            beneficiary = new Beneficiary
                            {
                                IdSubscriber = idsubscriber,
                                IdUser = null,
                                IdBeneficiaryWallet = beneficiaryWallet.Id,
                                Nom = beneficiarydto.Nom,
                                Email = beneficiarydto.Email,
                                Prenom = beneficiarydto.Prenom,
                                Has_gochap = beneficiarydto.Has_gochap,
                                TelephoneNumero = beneficiarydto.TelephoneNumero
                            };
                            _context.Beneficiaries.Add(beneficiary);
                            await _context.SaveChangesAsync();
                        }
                        var beneficiaryHistory4 = new BeneficiaryHistory
                        {
                            IdBeneficiary = beneficiary.Id,
                            Montant = 0.0,
                            Date = UtilityDate.GetDate(),
                            Action = BeneficiaryHistory.BeneficiaryActions.Initial,
                        };
                        _context.BeneficiaryHistories.Add(beneficiaryHistory4);
                        await _context.SaveChangesAsync();

                        var beneficiaryHistory5 = new BeneficiaryHistory
                        {
                            IdBeneficiary = beneficiary.Id,
                            Montant = cartecadeau ?? 0.0,
                            Date = UtilityDate.GetDate(),
                            Action = BeneficiaryHistory.BeneficiaryActions.Recharge,
                        };
                        _context.BeneficiaryHistories.Add(beneficiaryHistory5);
                        await _context.SaveChangesAsync();
                        var token = await _jwtService.GenerateBeneficiaryToken(beneficiary);
                        var subscriberUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == subscriber.IdUser);


                        var emailresponse = await _emailService.SendEmailAsync(beneficiary.Email, token , $"{(double)cartecadeau}",subscriber.SubscriberName);
                        




                        return Ok(new {Emailresponse = emailresponse });
                    }

                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Une erreur est survenue.", details = ex.Message });
                }
            }
            return BadRequest(ModelState);
        }





        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser != null)
            {
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password);
                if (!isPasswordValid)
                {
                    return Unauthorized(new { message = "Email ou mot de passe incorrect." });
                }
                if (existingUser.IsActive == false)
                {
                    return Unauthorized("L'utilisateur est désactivé.");
                }

                var token = await _jwtService.GenerateToken(existingUser);
                var refreshToken = _jwtService.GenerateRefreshToken();
                _jwtService.SaveRefreshToken(existingUser, refreshToken);

                return Ok(new { Token = token });
            }

            return Unauthorized("Email ou mot de passe incorrect.");
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
            var token = refreshRequest.Token;
            if (!_jwtService.ValidateTokenWithoutExpiration(token))
            {
                return BadRequest("Le token est invalide");
            }
            var claims = _jwtService.ParseJwtToken(token);
            int IdUser;
            var IdUserClaim = claims.FirstOrDefault(c => c.Key == "nameid");
            if (!string.IsNullOrEmpty(IdUserClaim.Value) && int.TryParse(IdUserClaim.Value, out int id))
            {
                IdUser = id;
            }
            else
            {
                return NotFound("Utilisateur Non trouvé");
            }
            var user = await _context.Users.FindAsync(IdUser);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized("Invalid refresh token.");
            }

            var newToken = await _jwtService.GenerateToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            _jwtService.SaveRefreshToken(user, newRefreshToken);

            return Ok(new { Token = newToken });
        }


        [Authorize(Policy = "IsActive")]
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
        [Authorize(Policy = "IsActive")]
        [Authorize(Roles = "ADMIN")]
        [HttpGet("GetFormatedUsers")]
        public async Task<ActionResult<IEnumerable<FullUser>>> GetFormatedUsers()
        {
            var users = await _context.Users.Include(u => u.Role).ToListAsync();
            List<FullUser> fullusers = new List<FullUser>();
            foreach (var user in users)
            {
                fullusers.Add(new FullUser
                {
                    Id = user.Id,
                    Email = user.Email,
                    Telephone = user.Telephone,
                    NomComplet = user.NomComplet,
                    Adresse = user.Adresse,
                    DateInscription = user.DateInscription,
                    IsActive = user.IsActive,
                    NomRole = user.Role.RoleNom
                });

            }
            fullusers.Sort((u1, u2) => u1.Id.CompareTo(u2.Id));
            return fullusers;
        }
        [Authorize(Policy = "IsActive")]
        [Authorize(Roles = "SUBSCRIBER,ADMIN")]
        [HttpGet("GetIdSubscriber/{id}")]
        public async Task<ActionResult<int>> GetIdSusbcriber(int id)
        {
            var subscriber = await _context.Subscribers.FirstOrDefaultAsync(u => u.IdUser == id); ;
            if (subscriber != null)
            {
                return subscriber.Id;
            }
            else
            {
                return NotFound();
            }
        }
        [Authorize(Policy = "IsActive")]
        [Authorize(Roles = "ADMIN")]
        [HttpGet("byrole/{idRole}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(int? idRole)
        {
            if (idRole == 0) idRole = null;
            var usersByRole = await _context.Users
                                            .Where(user => user.IdRole == idRole)
                                            .ToListAsync();

            if (usersByRole == null || !usersByRole.Any())
            {
                return NotFound();
            }

            return Ok(usersByRole);
        }
        [Authorize(Policy = "IsActive")]
        [Authorize(Roles = "ADMIN")]
        [HttpGet("byIsActive/{isActive}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByActivity(bool isActive)
        {
            var usersByActivity = await _context.Users
                                            .Where(user => user.IsActive == isActive)
                                            .ToListAsync();

            if (usersByActivity == null || !usersByActivity.Any())
            {
                return NotFound();
            }

            return Ok(usersByActivity);
        }
        [Authorize(Policy = "IsActive")]
        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [Authorize(Policy = "IsActive")]
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto user)
        {
            if (id != user.Id)
            {
                return BadRequest("Les identifiants ne correspondent pas");
            }
            var existinguser = await _context.Users.FindAsync(id);
            var hashedPassword = user.Password != null ? BCrypt.Net.BCrypt.HashPassword(user.Password) : existinguser.Password;
            existinguser.Email = user.Email ?? existinguser.Email;
            existinguser.Password = hashedPassword;
            existinguser.NomComplet = user.NomComplet ?? existinguser.NomComplet;
            existinguser.Adresse = user.Adresse ?? existinguser.Adresse;
            existinguser.Telephone = user.Telephone ?? existinguser.Telephone;
            existinguser.IsActive = user.IsActive;
            _context.Entry(existinguser).State = EntityState.Modified;
            if (existinguser == null)
            {
                return NotFound();
            }

            _context.Entry(existinguser).State = EntityState.Modified;
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
        [Authorize(Policy = "IsActive")]
        [Authorize(Roles = "ADMIN")]
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
