namespace LosDropV;

public static class DirectoryDetector
{
    private const string GtaExecutable = "GTA5.exe";

    // High-priority known locations checked instantly before the full scan
    private static readonly string[] KnownPaths =
    [
        @"C:\Program Files (x86)\Steam\steamapps\common\Grand Theft Auto V",
        @"C:\Program Files\Steam\steamapps\common\Grand Theft Auto V",
        @"C:\Program Files\Epic Games\GTAV",
        @"C:\Program Files\Rockstar Games\Grand Theft Auto V",
        @"C:\Program Files (x86)\Rockstar Games\Grand Theft Auto V",
        @"C:\Games\Grand Theft Auto V",
        @"C:\GTAV",
        @"D:\Steam\steamapps\common\Grand Theft Auto V",
        @"D:\SteamLibrary\steamapps\common\Grand Theft Auto V",
        @"D:\Games\Grand Theft Auto V",
        @"D:\Rockstar Games\Grand Theft Auto V",
        @"D:\Epic Games\GTAV",
        @"D:\GTAV",
        @"D:\User\GAMES 3\GTA-V (Multiplayer)",
        @"D:\Users\GAMES 3\GTA-V (Multiplayer)",
        @"E:\SteamLibrary\steamapps\common\Grand Theft Auto V",
        @"E:\Games\Grand Theft Auto V",
        @"E:\Rockstar Games\Grand Theft Auto V",
        @"F:\SteamLibrary\steamapps\common\Grand Theft Auto V",
        @"F:\Games\Grand Theft Auto V",
    ];

    // Folder names that are extremely unlikely to contain GTA V — skip entirely for speed
    private static readonly HashSet<string> SkippedFolders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Windows", "System32", "SysWOW64", "WinSxS", "Temp", "$Recycle.Bin",
        "System Volume Information", "Recovery", "MSOCache", "ProgramData",
        "Intel", "AMD", "NVIDIA", "Drivers", "Boot", "EFI",
        "node_modules", ".git", "__pycache__",
    };

    /// <summary>
    /// Full detection pipeline:
    /// 1. Check known paths instantly
    /// 2. Full recursive scan of all available drives
    /// Returns the GTA V directory path, or null if not found.
    /// </summary>
    public static string? FindGtaDirectory()
    {
        UI.PrintInfo("Phase 1 — Checking known install locations...");

        foreach (string path in KnownPaths)
        {
            UI.PrintInfo($"  Checking: {path}");
            if (IsValidGtaDirectory(path))
            {
                UI.PrintSuccess($"  Found GTA5.exe at: {path}");
                return path;
            }
        }

        UI.PrintWarning("  Not found in known paths.");
        UI.PrintInfo("Phase 2 — Full drive scan (this may take a moment)...");

        return ScanAllDrives();
    }

    /// <summary>
    /// Enumerates all fixed/removable drives and recursively searches each one for GTA5.exe.
    /// </summary>
    private static string? ScanAllDrives()
    {
        DriveInfo[] drives = DriveInfo.GetDrives()
            .Where(d => d.IsReady && d.DriveType is DriveType.Fixed or DriveType.Removable)
            .ToArray();

        UI.PrintInfo($"  Found {drives.Length} drive(s): {string.Join(", ", drives.Select(d => d.Name))}");

        foreach (DriveInfo drive in drives)
        {
            UI.PrintInfo($"  Scanning drive {drive.Name} ...");
            string? result = ScanDirectory(drive.RootDirectory.FullName, depth: 0, maxDepth: 8);
            if (result is not null)
            {
                UI.PrintSuccess($"  Found GTA5.exe at: {result}");
                return result;
            }
        }

        return null;
    }

    /// <summary>
    /// Recursively walks a directory tree looking for a folder that contains GTA5.exe.
    /// Skips system/junk folders and limits depth to avoid runaway recursion.
    /// </summary>
    private static string? ScanDirectory(string dir, int depth, int maxDepth)
    {
        if (depth > maxDepth)
            return null;

        // Check the current directory first
        if (IsValidGtaDirectory(dir))
            return dir;

        string[]? subDirs;
        try
        {
            subDirs = Directory.GetDirectories(dir);
        }
        catch (UnauthorizedAccessException)  { return null; }
        catch (IOException)                  { return null; }

        foreach (string sub in subDirs)
        {
            string folderName = Path.GetFileName(sub);

            // Skip folders that can never contain GTA V
            if (SkippedFolders.Contains(folderName))
                continue;

            // Print current folder at depth 1 so the user sees progress
            if (depth == 0)
                UI.PrintInfo($"    [{folderName}]");

            string? found = ScanDirectory(sub, depth + 1, maxDepth);
            if (found is not null)
                return found;
        }

        return null;
    }

    /// <summary>
    /// Prompts the user to manually enter the GTA V directory and validates it.
    /// Loops until a valid path is provided.
    /// </summary>
    public static string PromptForDirectory()
    {
        while (true)
        {
            Console.Write("\nEnter your GTA V directory path: ");
            string? input = Console.ReadLine()?.Trim().Trim('"');

            if (string.IsNullOrWhiteSpace(input))
            {
                UI.PrintError("Path cannot be empty. Please try again.");
                continue;
            }

            if (!Directory.Exists(input))
            {
                UI.PrintError($"Directory does not exist: {input}");
                continue;
            }

            if (!IsValidGtaDirectory(input))
            {
                UI.PrintError($"GTA5.exe not found in: {input}");
                UI.PrintInfo("Please make sure you're pointing to the correct GTA V folder.");
                continue;
            }

            return input;
        }
    }

    /// <summary>
    /// Returns true if the given directory exists and contains GTA5.exe.
    /// </summary>
    public static bool IsValidGtaDirectory(string path)
    {
        if (!Directory.Exists(path))
            return false;

        return File.Exists(Path.Combine(path, GtaExecutable));
    }
}
