using System.Net;

namespace YourAssetManager.Server.Helpers
{
    public class HelperFunctions
    {
        public static string TokenLinkCreated(string baseUrl, string endpoint, string token, string email)
        { // URL encode the token and email address
            var encodedToken = WebUtility.UrlEncode(token);
            var encodedEmail = WebUtility.UrlEncode(email);

            //$"http://localhost:4200/auth/EmailConfirmation?token={encodedToken}&email={encodedEmail}";
            var confirmationLink = $"{baseUrl}/{endpoint}?token={encodedToken}&email={encodedEmail}";
            return confirmationLink;
        }
    }
}