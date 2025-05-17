namespace Gizmosbuy.Core.Interfaces
{
    public interface IWebConfiguration
    {
        public bool EnableSwagger { get; set; }
        public string ConnectionStrings { get; set; }
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public int SesssionTimeoutMinutes { get; set; }
    }
}
