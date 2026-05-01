using System.Runtime.Versioning;

namespace LosDropV;

[SupportedOSPlatform("windows")]
public static class App
{
    public static async Task RunAsync()
    {
        UI.PrintBanner();

        // ── GTA V Detection ──────────────────────────────────────────────────
        UI.PrintStep("Looking for your GTA V folder...");
        
        string? gtaDir = null;
        await UI.RunWithStatus("Quick scan running...", async () =>
        {
            await Task.Delay(400); // UI feel
            gtaDir = DirectoryDetector.FindGtaDirectory();
        });

        if (gtaDir is null)
        {
            UI.PrintWarning("Couldn't auto-find GTA V this time.");
            UI.PrintInfo("Pick your GTA V folder from the list below.");
            gtaDir = DirectoryDetector.PromptForDirectory();
        }

        UI.PrintSuccess($"Found it: [white]{gtaDir}[/]");
        UI.PrintDivider();

        // ── Main Loop ────────────────────────────────────────────────────────
        bool running = true;
        while (running)
        {
            string choice = UI.PromptMenu();

            switch (choice)
            {
                case "Install Menyoo PC":
                    await RunInstallation(gtaDir, installMenyoo: true,  installZombies: false);
                    break;
                case "Install Simple Zombies":
                    await RunInstallation(gtaDir, installMenyoo: false, installZombies: true);
                    break;
                case "Install both mods":
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
                UI.PrintWarning("Nothing got installed.");
        }
        catch (Exception ex)
        {
            UI.PrintError($"Something went wrong: {ex.Message}");
        }
        finally
        {
            await UI.RunWithStatus("Cleaning up temp files...", async () =>
            {
                CleanupTemp(tempDir);
                await Task.Delay(600);
            });
            UI.PrintInfo("Temp files cleaned.");
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
            UI.PrintError("Couldn't unpack the archive.");
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