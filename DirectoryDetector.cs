using Microsoft.Win32;
using Spectre.Console;
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
            var candidates = GetCandidateDirectories();
            var choices = candidates
                .Select(path => $"[green]{Markup.Escape(path)}[/]")
                .ToList();

            choices.Add("[yellow]Scan again[/]");
            choices.Add("[grey]Type a path myself[/]");

            string selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Pick your GTA V folder[/]")
                    .PageSize(12)
                    .AddChoices(choices));

            if (selected == "[yellow]Scan again[/]")
                continue;

            if (selected == "[grey]Type a path myself[/]")
            {
                string? manual = AnsiConsole.Prompt(
                    new TextPrompt<string>("[yellow]Paste your GTA V folder path:[/]")
                        .PromptStyle("white")
                        .AllowEmpty());

                string normalized = (manual ?? string.Empty).Trim().Trim('"');
                if (!string.IsNullOrWhiteSpace(normalized) && IsValidGtaDirectory(normalized))
                    return normalized;

                UI.PrintError("That folder doesn't look like GTA V (GTA5.exe not found).");
                continue;
            }

            string selectedPath = Markup.Remove(selected);
            if (IsValidGtaDirectory(selectedPath))
                return selectedPath;

            UI.PrintError("That folder isn't valid anymore, pick another one.");
        }
    }

    private static List<string> GetCandidateDirectories()
    {
        string[] priorityPaths =
        [
            @"D:\User\GAMES 3\GTA-V (Multiplayer)",
            @"D:\Users\GAMES 3\GTA-V (Multiplayer)"
        ];

        var results = new List<string>();

        foreach (var path in priorityPaths)
        {
            if (IsValidGtaDirectory(path))
                results.Add(path);
        }

        string? steam = GetSteamPath();
        string? epic = GetEpicPath();
        string? rockstar = GetRockstarPath();

        if (IsValidGtaDirectory(steam)) results.Add(steam!);
        if (IsValidGtaDirectory(epic)) results.Add(epic!);
        if (IsValidGtaDirectory(rockstar)) results.Add(rockstar!);

        foreach (var scannedPath in ScanCommonLocations())
        {
            if (IsValidGtaDirectory(scannedPath))
                results.Add(scannedPath);
        }

        return results
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static IEnumerable<string> ScanCommonLocations()
    {
        string[] commonRoots =
        [
            @"C:\Program Files\Rockstar Games",
            @"C:\Program Files (x86)\Steam\steamapps\common",
            @"D:\SteamLibrary\steamapps\common",
            @"E:\SteamLibrary\steamapps\common"
        ];

        var found = new List<string>();

        foreach (var root in commonRoots)
        {
            if (!Directory.Exists(root))
                continue;

            try
            {
                foreach (var dir in Directory.EnumerateDirectories(root))
                {
                    if (IsValidGtaDirectory(dir))
                        found.Add(dir);
                }
            }
            catch
            {
                // Ignore inaccessible folders
            }
        }

        return found;
    }

    public static bool IsValidGtaDirectory(string? path)
    {
        if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            return false;

        return File.Exists(Path.Combine(path, GtaExecutable));
    }
}
