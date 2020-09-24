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

        /// <summary>
        /// Is used to specify the `customAuthProvider` name that this
        /// auth should invoke. This option is only relevant when using
        /// a custom auth provider.
        /// </summary>
        /// <value></value>
        public string CustomAuthProviderName { get; set; }
    }
}
