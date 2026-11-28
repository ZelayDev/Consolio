using System.Text.Json;

namespace Consolio
{
    public abstract class Game
    {
        public static readonly JsonSerializerOptions JsonOptions = new()
        {
            //TODO
        };
        
        protected abstract string Name { get; }
        
        public abstract IScreen Screen { get; }
        
        public abstract Task Run();
        

        protected TConfig LoadConfig<TConfig>(TConfig Default) where TConfig : class
        {
            var FilePath = GetConfigFilePath();

            if (!File.Exists(FilePath))
            {
                var Json = JsonSerializer.Serialize(Default, JsonOptions);
                File.WriteAllText(FilePath, Json);
                return Default;
            }
            
            try
            {
                var Json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<TConfig>(Json, JsonOptions) ?? Default;
            }
            catch
            {
                return Default;
            }
        }
        
        private string GetConfigFilePath()
        {
            return Path.Combine
            (
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "consolio",
                Name,
                "config.json"
            );
        }
    }
}