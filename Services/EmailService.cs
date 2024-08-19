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

        public async Task<bool> SendEmailAsync(string email, string qrCodeToken)
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
                            Text = $@"
                                Félicitations ! Vous venez de recevoir une carte cadeau sous forme de QR code.
                                Dans le but de fidéliser sa clientèle, GoChap lance sa toute dernière innovation.
                                Cette carte cadeau vous permet d'opérer des achats auprès de nos marchands qui acceptent le paiement par carte cadeau.

                                <img src='{qrCodeImageSrc}' alt='QR Code' />
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
        public string Text { get; set; }
        public string Category { get; set; }
    }
}
