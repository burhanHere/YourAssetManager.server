using Microsoft.Extensions.Options;
using YourAssetManager.Server.Models;

namespace YourAssetManager.server.Services.EmailService
{
    /// <summary>or the Email Service responsible for sending emails.
    ///
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email asynchronously to the specified recipient.
        /// </summary>
        /// <param name="toEmail">The email address of the recipient.</param>
        /// <param name="subject">The subject line of the email.</param>
        /// <param name="message">The content of the email message, formatted as HTML.</param>
        /// <returns>A boolean value indicating the success of the email sending operation.</returns>
        public Task<bool> SendEmailAsync(string toEmail, string subject, string message);
    }
}