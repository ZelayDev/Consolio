using System.Text.Json;

namespace Consolio
{
    public abstract class Game
    {
        public abstract string Name { get; }
        
        public abstract Task Run();


        protected TConfig? LoadConfig<TConfig>() where TConfig : class
        {
            try
            {
                var FilePath = GetConfigFilePath();
                var Content  = File.ReadAllText(FilePath);

                return JsonSerializer.Deserialize<TConfig>(Content);
            }
            catch
            {
                return null;
            }
        }
        
        private string GetConfigFilePath()
        {
            return Path.Combine
            (
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "consolio",
                Name.ToLowerInvariant(),
                "config.json"
            );
        }
    }
}