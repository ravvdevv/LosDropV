namespace LosDropV;

public static class App
{
    public static async Task RunAsync()
    {
        UI.PrintBanner();

        // ── GTA V Detection ──────────────────────────────────────────────────
        UI.PrintStep("Scanning for GTA V installation...");
        Console.WriteLine();

        UI.StartSpinner("Checking known locations...");
        await Task.Delay(400); // brief pause so spinner is visible
        string? gtaDir = DirectoryDetector.FindGtaDirectory();
        UI.StopSpinner();

        if (gtaDir is null)
        {
            UI.PrintWarning("GTA V was not found automatically.");
            UI.PrintInfo("Please enter the path manually below.");
            Console.WriteLine();
            gtaDir = DirectoryDetector.PromptForDirectory();
        }

        Console.WriteLine();
        UI.PrintSuccess($"GTA V located  →  {gtaDir}");
        UI.PrintDivider();
        Console.WriteLine();

        // ── Main Loop ────────────────────────────────────────────────────────
        bool running = true;
        while (running)
        {
            UI.PrintMenu();
            string? input = Console.ReadLine()?.Trim();
            Console.WriteLine();

            switch (input)
            {
                case "1":
                    await RunInstallation(gtaDir, installMenyoo: true,  installZombies: false);
                    break;
                case "2":
                    await RunInstallation(gtaDir, installMenyoo: false, installZombies: true);
                    break;
                case "3":
                    await RunInstallation(gtaDir, installMenyoo: true,  installZombies: true);
                    break;
                case "4":
                    running = false;
                    UI.PrintGoodbye();
                    break;
                default:
                    UI.PrintError("Invalid choice. Enter 1, 2, 3, or 4.");
                    Console.WriteLine();
                    break;
            }
        }
    }

    // ── Installation orchestrator ─────────────────────────────────────────────
    private static async Task RunInstallation(string gtaDir, bool installMenyoo, bool installZombies)
    {
        string tempDir = Path.Combine(
            Path.GetTempPath(), "LosDropV_" + Guid.NewGuid().ToString("N")[..8]);

        var installed = new List<string>();

        try
        {
            Directory.CreateDirectory(tempDir);

            if (installMenyoo)
            {
                bool ok = await InstallSingleMod(ModDefinitions.Menyoo, gtaDir, tempDir);
                if (ok) installed.Add(ModDefinitions.Menyoo.Name);
            }

            if (installZombies)
            {
                bool ok = await InstallSingleMod(ModDefinitions.SimpleZombies, gtaDir, tempDir);
                if (ok) installed.Add(ModDefinitions.SimpleZombies.Name);
            }

            if (installed.Count > 0)
                UI.PrintSuccessSummary([.. installed]);
            else
                UI.PrintWarning("No mods were installed successfully.");
        }
        catch (Exception ex)
        {
            UI.PrintError($"Unexpected error: {ex.Message}");
        }
        finally
        {
            UI.StartSpinner("Cleaning up temporary files...");
            CleanupTemp(tempDir);
            await Task.Delay(600);
            UI.StopSpinner("Temp files removed.", success: true);
            Console.WriteLine();
        }
    }

    // ── Single mod pipeline ───────────────────────────────────────────────────
    private static async Task<bool> InstallSingleMod(ModInfo mod, string gtaDir, string tempDir)
    {
        UI.PrintSection($" {mod.Name} ");
        Console.WriteLine();

        string archivePath = Path.Combine(tempDir, mod.FileName);

        // 1. Download
        bool downloaded = await Downloader.DownloadFileAsync(mod.Url, archivePath);
        if (!downloaded)
        {
            UI.PrintError($"Skipping {mod.Name} — download failed.");
            UI.PrintSectionEnd();
            Console.WriteLine();
            return false;
        }

        Console.WriteLine();

        // 2. Extract
        UI.StartSpinner($"Extracting {mod.FileName}...");
        string extractDir = Path.Combine(tempDir, mod.Name.Replace(" ", "_"));
        Directory.CreateDirectory(extractDir);
        bool extracted = Extractor.ExtractArchive(archivePath, extractDir);
        UI.StopSpinner(
            extracted ? $"Extracted successfully" : $"Extraction failed",
            success: extracted);

        if (!extracted)
        {
            UI.PrintError($"Skipping {mod.Name} — extraction failed.");
            UI.PrintSectionEnd();
            Console.WriteLine();
            return false;
        }

        // 3. Install
        UI.StartSpinner($"Installing files into GTA V directory...");
        await Task.Delay(300); // let spinner breathe
        ModInstaller.InstallMod(extractDir, gtaDir);
        UI.StopSpinner("Files copied to GTA V directory.", success: true);

        UI.PrintSectionEnd();
        Console.WriteLine();
        return true;
    }

    private static void CleanupTemp(string tempDir)
    {
        try
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
        catch { }
    }
}
