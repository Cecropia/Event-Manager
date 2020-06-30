using EventManager.BusinessLogic.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EventManager.BusinessLogic.Services
{
    public class ReaderDatabase : IReaderDatabase
    {
        private readonly IConfiguration configuration;
        public ReaderDatabase(string[] files)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory());

            foreach (var file in files)
                builder.AddJsonFile(file, optional: true);

            configuration = builder.Build();

        }
        public string GetConnectionStringValue(string key)
        {
            string conString = configuration.GetConnectionString(key);
            return conString;
        }
    }
}
