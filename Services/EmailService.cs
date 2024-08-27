using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using QRCoder;
using Microsoft.AspNetCore.SignalR;

namespace giftcard_api.Services
{
    public class EmailService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;


        public EmailService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task<bool> SendPayementEmailAsync(string email, string montant, string marchand, string solderestant)
        {
            var url = _configuration["SendMailAPI:Url"];
            var token = _configuration["SendMailAPI:Token"];
            var emailMessage = new EmailMessage
            {
                From = new EmailAddress
                {
                    Email = "info@gochap.solutions",
                    Name = "GoChap"
                },
                To = new List<EmailAddress>
                            {
                                new EmailAddress { Email = email }
                            },
                Subject = "Vous avez effectué un achat par Carte Cadeau !",
                Html = $@"
                            <div style='text-align:justify; margin:10px 0;justify-content:center;'>
                            Vous venez d'effectuer un achat par Carte Cadeau d'un montant de {montant} auprès du marchant {marchand};</br>
                            Votre solde de carte cadeau restant est de {solderestant} XOF.</br>
                            Si vous n'êtes pas l'auteur de cet achat.Veuillez concactez le Support Gochap par email à info@gochap.solutions .
                            </div>
                            ",
                Category = "Envoi de Confirmation d'Achat"
            };
            var jsonContent = JsonSerializer.Serialize(emailMessage);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Ajout du token d'authentification
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Envoi de la requête POST
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Email sent successfully: {responseContent}");
                return true;
            }
            else
            {
                Console.WriteLine($"Failed to send email. Status code: {response.StatusCode}");
                return false;
            }

        }

        public async Task<bool> SendEmailAsync(string email, string qrCodeToken, string montant)
        {
            var url = _configuration["SendMailAPI:Url"];
            var token = _configuration["SendMailAPI:Token"];

            // Création du générateur de QR Code
            using (var qrGenerator = new QRCodeGenerator())
            {
                // Création des données QR Code
                using (var qrCodeData = qrGenerator.CreateQrCode(qrCodeToken, QRCodeGenerator.ECCLevel.Q))
                {
                    // Création du QR Code en format PNG
                    using (var qrCode = new PngByteQRCode(qrCodeData))
                    {
                        byte[] qrCodeImage = qrCode.GetGraphic(20);

                        // Conversion de l'image QR Code en Base64
                        var qrCodeBase64 = Convert.ToBase64String(qrCodeImage);
                        var qrCodeImageSrc = $"data:image/png;base64,{qrCodeBase64}";

                        // Création du message email
                        var emailMessage = new EmailMessage
                        {
                            From = new EmailAddress
                            {
                                Email = "info@gochap.solutions",
                                Name = "GoChap"
                            },
                            To = new List<EmailAddress>
                            {
                                new EmailAddress { Email = email }
                            },
                            Subject = "Vous avez reçu une Carte Cadeau !",
                            Html = $@"
                                Félicitations ! Vous venez de recevoir une carte cadeau sous forme de QR code d'une valeur de {montant} XOF.
                                Dans le but de fidéliser sa clientèle, GoChap lance sa toute dernière innovation.
                                Cette carte cadeau vous permet d'opérer des achats auprès de nos marchands qui acceptent le paiement par carte cadeau.
                                <div style='text-align:center; margin:10px 0;'>
                                <img src='{qrCodeImageSrc}' alt='QR Code' style='max-width:30%; height:auto;' />
                                </div>
                            ",
                            Category = "Envoi de Carte Cadeau"
                        };

                        // Sérialisation de l'email en JSON
                        var jsonContent = JsonSerializer.Serialize(emailMessage);
                        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                        // Ajout du token d'authentification
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        // Envoi de la requête POST
                        var response = await _httpClient.PostAsync(url, content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Email sent successfully: {responseContent}");
                            return true;
                        }
                        else
                        {
                            Console.WriteLine($"Failed to send email. Status code: {response.StatusCode}");
                            return false;
                        }
                    }
                }
            }
        }
        public async Task<bool> SendSubscriberRegistrationEmailAsync(string email, string nom )
        {
            var url = _configuration["SendMailAPI:Url"];
            var token = _configuration["SendMailAPI:Token"];
            var emailMessage = new EmailMessage
            {
                From = new EmailAddress
                {
                    Email = "info@gochap.solutions",
                    Name = "GoChap"
                },
                To = new List<EmailAddress>
                            {
                                new EmailAddress { Email = email }
                            },
                Subject = "Vôtre Inscription comme Souscripteur au Service de Carte Cadeau de GoChap est un succès !",
                Html = $@"
                            <div style='text-align:center; margin:10px 0;'>
                                Félicitations ! {nom} , Pour votre enregistrement en tant que Souscripteur
                                dans le Service de Carte Cadeau de GoChap.Cette dernière innovation de GoChap Vous permet d'enregistrer vos
                                employés comme bénéficiaire afin de leur octroyer une carte cadeau permettant de faire des achats au près de
                                nos marchands les acceptant comme moyen de payement.
                                </div>
                            ",
                Category = "Confirmation d'inscription"
            };


            var jsonContent = JsonSerializer.Serialize(emailMessage);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");


            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Email sent successfully: {responseContent}");
                return true;
            }
            else
            {
                Console.WriteLine($"Failed to send email. Status code: {response.StatusCode}");
                return false;
            }
        }
                public async Task<bool> SendAdminRegistrationEmailAsync(string email, string nom,string password)
        {
            var url = _configuration["SendMailAPI:Url"];
            var token = _configuration["SendMailAPI:Token"];
            var emailMessage = new EmailMessage
            {
                From = new EmailAddress
                {
                    Email = "info@gochap.solutions",
                    Name = "GoChap"
                },
                To = new List<EmailAddress>
                            {
                                new EmailAddress { Email = email }
                            },
                Subject = "Vôtre Inscription comme Administrateur au Service de Carte Cadeau de GoChap est un succès !",
                Html = $@"
                            <div style='text-align:center; margin:10px 0;'>
                                Félicitations ! {nom} , Pour votre enregistrement en tant que Administrateur
                                dans le Service de Carte Cadeau de GoChap. Vous pouvez utiliser ce mot de passe
                                Pour vous connecter sur la plateforme Web <{password}>.
                                </div>
                            ",
                Category = "Confirmation d'inscription"
            };


            var jsonContent = JsonSerializer.Serialize(emailMessage);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");


            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Email sent successfully: {responseContent}");
                return true;
            }
            else
            {
                Console.WriteLine($"Failed to send email. Status code: {response.StatusCode}");
                return false;
            }
        }
    }



        public class EmailAddress
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }

        public class EmailMessage
        {
            public EmailAddress From { get; set; }
            public List<EmailAddress> To { get; set; }
            public string Subject { get; set; }
            public string Html { get; set; }
            public string Category { get; set; }
        }
    }
