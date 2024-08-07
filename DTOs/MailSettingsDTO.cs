namespace YourAssetManager.Server.DTOs
{
    public class MailSettingsDTO
    {
        public string? SmtpServer { get; set; }
        public string? From { get; set; }
        public string? FromPassword { get; set; }
        public int Port { get; set; }
        public int Timeout { get; set; }
        public bool EnableSsl { get; set; }
    }
}
