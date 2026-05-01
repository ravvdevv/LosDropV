using Microsoft.Win32;
using System.Runtime.Versioning;

namespace LosDropV;

[SupportedOSPlatform("windows")]
public static class DirectoryDetector
{
    private const string GtaExecutable = "GTA5.exe";

    public static string? FindGtaDirectory()
    {
        UI.PrintInfo("Checking high-priority custom paths...");

        string[] priorityPaths = 
        [
            @"D:\User\GAMES 3\GTA-V (Multiplayer)",
            @"D:\Users\GAMES 3\GTA-V (Multiplayer)"
        ];

        foreach (var path in priorityPaths)
        {
            UI.PrintInfo($"  Scanning: {path}");
            if (IsValidGtaDirectory(path)) return path;
        }

        UI.PrintInfo("Locating GTA V via Windows Registry...");

        // 1. Check Steam
        string? steamPath = GetSteamPath();
        if (steamPath != null) return steamPath;

        // 2. Check Epic Games
        string? epicPath = GetEpicPath();
        if (epicPath != null) return epicPath;

        // 3. Check Rockstar Games
        string? rockstarPath = GetRockstarPath();
        if (rockstarPath != null) return rockstarPath;

        return null;
    }

    private static string? GetSteamPath()
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam");
            string? installPath = key?.GetValue("InstallPath") as string;
            if (string.IsNullOrEmpty(installPath)) return null;

            string libraryFolders = Path.Combine(installPath, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(libraryFolders)) return null;

            // Simple check in main library first
            string mainPath = Path.Combine(installPath, "steamapps", "common", "Grand Theft Auto V");
            if (IsValidGtaDirectory(mainPath)) return mainPath;

            // A full VDF parser would be better, but this is a common manual backup location check
            return null; 
        }
        catch { return null; }
    }

    private static string? GetEpicPath()
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Epic Games\EpicGamesLauncher");
            string? appDataPath = key?.GetValue("AppDataPath") as string; // Usually C:\ProgramData\Epic\EpicGamesLauncher\Data
            if (string.IsNullOrEmpty(appDataPath)) return null;

            // Epic metadata is usually in %ProgramData%\Epic\EpicGamesLauncher\Data\Manifests
            string manifestDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Epic", "EpicGamesLauncher", "Data", "Manifests");
            if (!Directory.Exists(manifestDir)) return null;

            foreach (string file in Directory.GetFiles(manifestDir, "*.item"))
            {
                string content = File.ReadAllText(file);
                if (content.Contains("GrandTheftAutoV", StringComparison.OrdinalIgnoreCase))
                {
                    // Very crude parsing of JSON-like manifest
                    int index = content.IndexOf("\"InstallLocation\": \"");
                    if (index != -1)
                    {
                        int start = index + "\"InstallLocation\": \"".Length;
                        int end = content.IndexOf("\"", start);
                        string path = content[start..end].Replace("\\\\", "\\");
                        if (IsValidGtaDirectory(path)) return path;
                    }
                }
            }
        }
        catch { }
        return null;
    }

    private static string? GetRockstarPath()
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Rockstar Games\Grand Theft Auto V");
            string? path = key?.GetValue("InstallFolder") as string;
            if (IsValidGtaDirectory(path)) return path;
        }
        catch { }
        return null;
    }

    public static string PromptForDirectory()
    {
        while (true)
        {
            Console.Write("\nEnter your GTA V directory path: ");
            string? input = Console.ReadLine()?.Trim().Trim('"');

            if (string.IsNullOrWhiteSpace(input))
            {
                UI.PrintError("Path cannot be empty.");
                continue;
            }

            if (IsValidGtaDirectory(input)) return input;

            UI.PrintError($"GTA5.exe not found in: {input}");
        }
    }

    public static bool IsValidGtaDirectory(string? path)
    {
        if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            return false;

        return File.Exists(Path.Combine(path, GtaExecutable));
    }
}
