using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class WebConfiguration : IWebConfiguration
    {
        public bool EnableSwagger { get; set; }
        public string ConnectionStrings { get; set; }
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public int SesssionTimeoutMinutes { get; set; }
    }

    public class WhatsAppSettings : IWhatsAppSettings
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string SenderNumber { get; set; }
        public string ReceiverNumber { get; set; }
    }
}
