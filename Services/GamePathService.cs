using System.IO;
using System.Text.Json;

namespace Remnant2UnlockerApp.Services;

public sealed class GamePathService
{
    private readonly string _settingsDir;
    private readonly string _settingsPath;

    public GamePathService()
    {
        _settingsDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Remnant2UnlockerApp");

        _settingsPath = Path.Combine(_settingsDir, "settings.json");

        Directory.CreateDirectory(_settingsDir);

        Settings = LoadSettings();
    }

    public UserSettings Settings { get; private set; }

    public string Win64Path => Settings.Win64Path ?? "";

    public bool IsConfigured => IsValidWin64Path(Win64Path);

    public string GetModRootPath()
    {
        return Path.Combine(Win64Path, "Mods", "Remnant2Unlocker");
    }

    public string GetItemsPath()
    {
        return Path.Combine(GetModRootPath(), "items.json");
    }

    public string GetQueuePath()
    {
        return Path.Combine(GetModRootPath(), "command_queue.json");
    }

    public string GetStatusPath()
    {
        return Path.Combine(GetModRootPath(), "status.json");
    }

    public string GetScriptsPath()
    {
        return Path.Combine(GetModRootPath(), "scripts");
    }

    public void SetWin64Path(string path)
    {
        Settings.Win64Path = path;
        SaveSettings();
    }

    public static bool IsValidWin64Path(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        var exe = Path.Combine(path, "Remnant2-Win64-Shipping.exe");
        var mods = Path.Combine(path, "Mods");
        var modRoot = Path.Combine(path, "Mods", "Remnant2Unlocker");
        var items = Path.Combine(modRoot, "items.json");
        var scripts = Path.Combine(modRoot, "scripts");

        return File.Exists(exe)
            && Directory.Exists(mods)
            && Directory.Exists(modRoot)
            && File.Exists(items)
            && Directory.Exists(scripts);
    }

    private UserSettings LoadSettings()
    {
        try
        {
            if (!File.Exists(_settingsPath))
                return new UserSettings();

            var json = File.ReadAllText(_settingsPath);

            return JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
        }
        catch
        {
            return new UserSettings();
        }
    }

    private void SaveSettings()
    {
        var json = JsonSerializer.Serialize(
            Settings,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(_settingsPath, json);
    }
}

public sealed class UserSettings
{
    public string? Win64Path { get; set; }
}