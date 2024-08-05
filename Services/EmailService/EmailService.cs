
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using YourAssetManager.server.Services.EmailService;
using YourAssetManager.Server.Models;

namespace YourAssetManager.Server.Services
{
    public class EmailService(MailSettings mailSettings) : IEmailService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="appSettingJson">Configuration options for the application settings.</param>
        private readonly MailSettings _mailSettings = mailSettings;

        /// <summary>
        /// Sends an email asynchronously to the specified recipient.
        /// </summary>
        /// <param name="toEmail">The email address of the recipient.</param>
        /// <param name="subject">The subject line of the email.</param>
        /// <param name="message">The content of the email message, formatted as HTML.</param>
        /// <returns>A boolean value indicating the success of the email sending operation.</returns>
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string message)
        {
            // Create the email message
            MailMessage mail = CreateMailMessage(toEmail, subject, message);
            // Send the email and get the result
            var result = await SendAsync(mail);
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="MailMessage"/> object with the specified parameters.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="subject">The subject line of the email.</param>
        /// <param name="message">The message content to include in the email.</param>
        /// <returns>A <see cref="MailMessage"/> object configured for the email.</returns>
        private MailMessage CreateMailMessage(string toEmail, string subject, string message)
        {
            // Construct the final mail message with HTML body
            MailMessage finalMailMessage = new()
            {
                From = new MailAddress(_mailSettings.From ?? throw new ArgumentNullException(nameof(_mailSettings.From))),
                Subject = subject,
                IsBodyHtml = true,
                Body = $@"
                        <!DOCTYPE html>
                        <html lang=""en"">
                        <head>
                            <meta charset=""UTF-8"">
                            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                            <style>
                                body {{
                                    font-family: 'Arial', sans-serif;
                                    margin: 0;
                                    padding: 0;
                                    background-color: #f4f4f4;
                                }}
                                .container {{
                                    width: 100%;
                                    max-width: 600px;
                                    margin: 0 auto;
                                    background-color: #ffffff;
                                    border-radius: 8px;
                                    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
                                    overflow: hidden;
                                }}
                                .header {{
                                    background-color: #007BFF;
                                    color: white;
                                    padding: 30px;
                                    text-align: center;
                                }}
                                .header h1 {{
                                    margin: 0;
                                    font-size: 24px;
                                    letter-spacing: 1px;
                                }}
                                .content {{
                                    padding: 20px;
                                    line-height: 1.6;
                                }}
                                .content h2 {{
                                    font-size: 20px;
                                    color: #333;
                                }}
                                .content p {{
                                    font-size: 16px;
                                    color: #555;
                                }}
                                .button {{
                                    display: inline-block;
                                    background-color: #007BFF;
                                    color: white;
                                    padding: 10px 15px;
                                    text-decoration: none;
                                    border-radius: 5px;
                                    margin-top: 15px;
                                    transition: background-color 0.3s;
                                }}
                                .button:hover {{
                                    background-color: #0056b3;
                                }}
                                .footer {{
                                    text-align: center;
                                    padding: 15px;
                                    font-size: 12px;
                                    color: #777777;
                                    border-top: 1px solid #f0f0f0;
                                }}
                                .footer p {{
                                    margin: 5px 0;
                                }}
                                a {{
                                    color: #007BFF;
                                    text-decoration: none;
                                }}
                                a:hover {{
                                    text-decoration: underline;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class=""container"">
                                <div class=""header"">
                                    <h1>Your Asset Manager (YAM)</h1>
                                </div>
                                <div class=""content"">
                                    <h2>Hello,</h2>
                                    <p>{message}</p>
                                    <p>Thank you for using YAM!</p>
                                    <a href=""https://www.yourassetmanager.com"" class=""button"">Visit Our Website</a>
                                </div>
                                <div class=""footer"">
                                    <p>&copy; 2024 Your Asset Manager (YAM). All rights reserved.</p>
                                </div>
                            </div>
                        </body>
                        </html>"
            };
            // Add the recipient's email address to the message
            finalMailMessage.To.Add(toEmail);
            return finalMailMessage;
        }

        /// <summary>
        /// Sends the specified email message asynchronously.
        /// </summary>
        /// <param name="message">The <see cref="MailMessage"/> to be sent.</param>
        /// <returns>A boolean value indicating the success of the email sending operation.</returns>
        private async Task<bool> SendAsync(MailMessage message)
        {
            // Create a new SmtpClient using the SMTP server settings
            SmtpClient smtpClient = new(_mailSettings.SmtpServer)
            {
                Port = _mailSettings.Port,
                Credentials = new NetworkCredential(_mailSettings.From, _mailSettings.FromPassword),
                EnableSsl = _mailSettings.EnableSsl,
                Timeout = _mailSettings.Timeout
            };
            try
            {
                // Send the email message asynchronously
                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch
            {
                // Handle any exceptions that occur during the sending process
                return false;
            }
            finally
            {
                // Ensure that the SmtpClient is disposed of properly
                smtpClient.Dispose();
            }
        }
    }
}