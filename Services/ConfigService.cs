using FIFOTasker_wpf.Data;
using System.IO;
using System.Text.Json;

public class ConfigService
{
    private readonly string _configPath;
    private AppConfiguration _config = new();

    public ConfigService()
    {
        _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FIFOTaskerConfig.json");
        Load();
    }

    private void Load()
    {
        if (File.Exists(_configPath))
        {
            try
            {
                var json = File.ReadAllText(_configPath);
                _config = JsonSerializer.Deserialize<AppConfiguration>(json) ?? new();
            }
            catch { /* ignore corrupt config */ }
        }
    }

    public void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, json);
        }
        catch { /* silent fail */ }
    }

    public bool IsFirstRun()
    {
        return !_config.FirstRunCompleted;
    }

    public void MarkFirstRunCompleted()
    {
        _config.FirstRunCompleted = true;
        Save();
    }
}