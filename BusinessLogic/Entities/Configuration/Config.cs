namespace EventManager.BusinessLogic.Entities.Configuration
{
    public class Config
    {
        public string BaseURL { get; set; } // mandatoy
        public int MaxRetries { get; set; } = 999;
        public int RequestRate { get; set; } = 312;
    }
}
