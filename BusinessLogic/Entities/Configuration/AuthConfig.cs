namespace EventManager.BusinessLogic.Entities.Configuration
{
    public class AuthConfig
    {
        public string Type { get; set; } = nameof(AuthType.None);
        // Basic
        public string Username { get; set; }
        public string Password { get; set; }

        //OAuth
        public string LoginEndpoint { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        //public string Username { get; set; }
        // Username is also used for OAuth
        // Password is also used for OAuth
        public string Token { get; set; }

        // For custom auth providers
        public string CustomAuthProviderName { get; set; }
    }
}
