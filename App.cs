namespace LosDropV;

public static class App
{
    public static async Task RunAsync()
    {
        UI.PrintBanner();

        // ── GTA V Detection ──────────────────────────────────────────────────
        UI.PrintStep("Scanning for GTA V installation...");
        
        string? gtaDir = null;
        await UI.RunWithStatus("Checking registry...", async () =>
        {
            await Task.Delay(400); // UI feel
            gtaDir = DirectoryDetector.FindGtaDirectory();
        });

        if (gtaDir is null)
        {
            UI.PrintWarning("GTA V was not found automatically via registry.");
            UI.PrintInfo("Please enter the path manually below.");
            gtaDir = DirectoryDetector.PromptForDirectory();
        }

        UI.PrintSuccess($"GTA V located → [white]{gtaDir}[/]");
        UI.PrintDivider();

        // ── Main Loop ────────────────────────────────────────────────────────
        bool running = true;
        while (running)
        {
            string choice = UI.PromptMenu();

            switch (choice)
            {
                case "Deploy Menyoo PC":
                    await RunInstallation(gtaDir, installMenyoo: true,  installZombies: false);
                    break;
                case "Deploy Simple Zombies":
                    await RunInstallation(gtaDir, installMenyoo: false, installZombies: true);
                    break;
                case "Deploy FULL SUITE (Both)":
                    await RunInstallation(gtaDir, installMenyoo: true,  installZombies: true);
                    break;
                case "Exit":
                    running = false;
                    UI.PrintGoodbye();
                    break;
            }
        }
    }

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
            await UI.RunWithStatus("Cleaning up temporary files...", async () =>
            {
                CleanupTemp(tempDir);
                await Task.Delay(600);
            });
            UI.PrintInfo("Temp files removed.");
        }
    }

    private static async Task<bool> InstallSingleMod(ModInfo mod, string gtaDir, string tempDir)
    {
        UI.PrintSection(mod.Name);

        string archivePath = Path.Combine(tempDir, mod.FileName);

        // 1. Download
        bool downloaded = await Downloader.DownloadFileAsync(mod.Url, archivePath);
        if (!downloaded) return false;

        // 2. Extract
        string extractDir = Path.Combine(tempDir, "Extracted_" + mod.Name.Replace(" ", ""));
        bool extracted = false;
        
        await UI.RunWithStatus($"Extracting {mod.FileName}...", async () =>
        {
            extracted = Extractor.ExtractArchive(archivePath, extractDir);
            await Task.Delay(400);
        });

        if (!extracted)
        {
            UI.PrintError("Extraction failed.");
            return false;
        }

        // 3. Deploy
        ModInstaller.InstallMod(extractDir, gtaDir);
        return true;
    }

    private static void CleanupTemp(string path)
    {
        try
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive: true);
        }
        catch { }
    }
}