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
namespace giftcard_api.Services
{
    public class RegisterBeneficiaryService
    {
        private readonly ApplicationDbContext _context;


        private readonly EmailService _emailService;


        public RegisterBeneficiaryService(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }


        public async Task<Beneficiary> RegisterGoChapOldBeneficiary(Beneficiary beneficiary, double? cartecadeau)
        {
            try
            {
                if (beneficiary == null)
                {
                    throw new ArgumentNullException(nameof(beneficiary), "L'objet beneficiary est null");
                }
                else
                {
                Console.WriteLine("Le beneficiaire n'est pas nul");}

                var rechargehistory = new BeneficiaryHistory
                {
                    IdBeneficiary = beneficiary.Id,
                    Montant = cartecadeau ?? 0.0,
                    Date = UtilityDate.GetDate(),
                    Action = BeneficiaryHistory.BeneficiaryActions.Recharge,
                };
                _context.BeneficiaryHistories.Add(rechargehistory);
                await _context.SaveChangesAsync();
                var rechargebeneficiarywallet = beneficiary.BeneficiaryWallet;
                if (rechargebeneficiarywallet == null)
                {
                    throw new InvalidOperationException("BeneficiaryWallet est null");
                }
                 else
                {
                Console.WriteLine("Le Wallet n'est pas nul");}
                rechargebeneficiarywallet.Solde = rechargebeneficiarywallet.Solde + (double)cartecadeau;
                _context.Entry(rechargebeneficiarywallet).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return beneficiary;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Enregistrement d'un ancien b {ex.Message}");
                return null;
            }


        }
        public async Task<Beneficiary> RegisterGoChapNewBeneficiary(BeneficiaryDto beneficiarydto, User existingUser, int idsubscriber)
        {
            try
            {
                existingUser.IdRole = 1;
                _context.Entry(existingUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                var beneficiaryWallet = new BeneficiaryWallet();
                _context.BeneficiaryWallets.Add(beneficiaryWallet);
                await _context.SaveChangesAsync();

                var beneficiary = new Beneficiary
                {
                    IdSubscriber = idsubscriber,
                    IdUser = existingUser.Id,
                    IdBeneficiaryWallet = beneficiaryWallet.Id,
                    Nom = beneficiarydto.Nom,
                    Email = beneficiarydto.Email,
                    Prenom = beneficiarydto.Prenom,
                    Has_gochap = beneficiarydto.Has_gochap,
                    TelephoneNumero = beneficiarydto.TelephoneNumero
                };
                _context.Beneficiaries.Add(beneficiary);
                await _context.SaveChangesAsync();
                var beneficiaryHistory = new BeneficiaryHistory
                {
                    IdBeneficiary = beneficiary.Id,
                    Montant = 0.0,
                    Date = UtilityDate.GetDate(),
                    Action = BeneficiaryHistory.BeneficiaryActions.Initial,
                };
                _context.BeneficiaryHistories.Add(beneficiaryHistory);
                await _context.SaveChangesAsync();
                return beneficiary;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }


        }
    }

}
