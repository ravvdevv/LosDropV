namespace LosDropV;

public static class ModInstaller
{
    /// <summary>
    /// Copies all files from <paramref name="sourceDirectory"/> into <paramref name="gtaDirectory"/>,
    /// preserving the folder structure and overwriting existing files.
    /// </summary>
    public static void InstallMod(string sourceDirectory, string gtaDirectory)
    {
        UI.PrintStep("Copying mod files...");

        if (!Directory.Exists(sourceDirectory))
        {
            UI.PrintError($"  Source directory not found: {sourceDirectory}");
            return;
        }

        if (!Directory.Exists(gtaDirectory))
        {
            UI.PrintError($"  GTA V directory not found: {gtaDirectory}");
            return;
        }

        string[] allFiles = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);

        if (allFiles.Length == 0)
        {
            UI.PrintWarning("  No files found to install.");
            return;
        }

        int copied = 0;
        int failed = 0;

        foreach (string sourcePath in allFiles)
        {
            try
            {
                // Compute relative path from the source root
                string relativePath = Path.GetRelativePath(sourceDirectory, sourcePath);

                // Build destination path inside GTA V folder
                string destinationPath = Path.Combine(gtaDirectory, relativePath);

                // Security: ensure we're not writing outside GTA V dir
                string fullDest = Path.GetFullPath(destinationPath);
                string fullGtaDir = Path.GetFullPath(gtaDirectory) + Path.DirectorySeparatorChar;

                if (!fullDest.StartsWith(fullGtaDir, StringComparison.OrdinalIgnoreCase))
                {
                    UI.PrintWarning($"  Skipping suspicious path: {relativePath}");
                    continue;
                }

                // Create subdirectory if needed
                string? destDir = Path.GetDirectoryName(destinationPath);
                if (destDir is not null && !Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                UI.PrintInfo($"  Copying: {relativePath}");
                File.Copy(sourcePath, destinationPath, overwrite: true);
                copied++;
            }
            catch (UnauthorizedAccessException)
            {
                UI.PrintError($"  Access denied: {sourcePath}");
                UI.PrintWarning("  Try running the installer as Administrator.");
                failed++;
            }
            catch (IOException ex)
            {
                UI.PrintError($"  IO error copying {Path.GetFileName(sourcePath)}: {ex.Message}");
                failed++;
            }
        }

        Console.WriteLine();
        UI.PrintSuccess($"  Copied {copied} file(s) to GTA V directory.");

        if (failed > 0)
            UI.PrintWarning($"  {failed} file(s) failed to copy.");
    }
}
