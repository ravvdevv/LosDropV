using System.IO.Compression;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;

namespace LosDropV;

public static class Extractor
{
    /// <summary>
    /// Extracts a .zip or .rar archive to the specified output directory.
    /// Returns true on success, false on failure.
    /// </summary>
    public static bool ExtractArchive(string archivePath, string outputDirectory)
    {
        UI.PrintStep($"Extracting: {Path.GetFileName(archivePath)}");

        if (!File.Exists(archivePath))
        {
            UI.PrintError($"  Archive not found: {archivePath}");
            return false;
        }

        try
        {
            string extension = Path.GetExtension(archivePath).ToLowerInvariant();

            bool success = extension switch
            {
                ".zip" => ExtractZip(archivePath, outputDirectory),
                ".rar" => ExtractRar(archivePath, outputDirectory),
                _ => ExtractWithSharpCompress(archivePath, outputDirectory)
            };

            if (success)
                UI.PrintSuccess($"  Extracted to: {outputDirectory}");

            return success;
        }
        catch (Exception ex)
        {
            UI.PrintError($"  Extraction failed: {ex.Message}");
            return false;
        }
    }

    private static bool ExtractZip(string zipPath, string outputDirectory)
    {
        try
        {
            using ZipArchive archive = ZipFile.OpenRead(zipPath);
            int total = archive.Entries.Count;
            int current = 0;

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                current++;
                UI.PrintInfo($"  [{current}/{total}] {entry.FullName}");

                // Skip directory entries
                if (string.IsNullOrEmpty(entry.Name))
                    continue;

                string destinationPath = Path.GetFullPath(
                    Path.Combine(outputDirectory, entry.FullName));

                // Security: guard against path traversal
                if (!destinationPath.StartsWith(
                    Path.GetFullPath(outputDirectory) + Path.DirectorySeparatorChar))
                {
                    UI.PrintWarning($"  Skipping suspicious path: {entry.FullName}");
                    continue;
                }

                string? destDir = Path.GetDirectoryName(destinationPath);
                if (destDir is not null)
                    Directory.CreateDirectory(destDir);

                entry.ExtractToFile(destinationPath, overwrite: true);
            }

            return true;
        }
        catch (InvalidDataException)
        {
            UI.PrintError("  The ZIP file appears to be corrupt or incomplete.");
            return false;
        }
    }

    private static bool ExtractRar(string rarPath, string outputDirectory)
    {
        try
        {
            using RarArchive archive = RarArchive.Open(rarPath);
            var entries = archive.Entries.Where(e => !e.IsDirectory).ToList();
            int total = entries.Count;
            int current = 0;

            foreach (var entry in entries)
            {
                current++;
                UI.PrintInfo($"  [{current}/{total}] {entry.Key}");

                string destinationPath = Path.GetFullPath(
                    Path.Combine(outputDirectory, entry.Key!));

                // Security: guard against path traversal
                if (!destinationPath.StartsWith(
                    Path.GetFullPath(outputDirectory) + Path.DirectorySeparatorChar))
                {
                    UI.PrintWarning($"  Skipping suspicious path: {entry.Key}");
                    continue;
                }

                string? destDir = Path.GetDirectoryName(destinationPath);
                if (destDir is not null)
                    Directory.CreateDirectory(destDir);

                entry.WriteToFile(destinationPath, new ExtractionOptions
                {
                    ExtractFullPath = true,
                    Overwrite = true
                });
            }

            return true;
        }
        catch (Exception ex) when (ex is InvalidOperationException or IOException)
        {
            UI.PrintError($"  RAR extraction error: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Fallback using SharpCompress auto-detection for other archive formats.
    /// </summary>
    private static bool ExtractWithSharpCompress(string archivePath, string outputDirectory)
    {
        UI.PrintInfo("  Using SharpCompress auto-detection...");

        using IArchive archive = ArchiveFactory.Open(archivePath);
        foreach (var entry in archive.Entries.Where(e => !e.IsDirectory))
        {
            entry.WriteToDirectory(outputDirectory, new ExtractionOptions
            {
                ExtractFullPath = true,
                Overwrite = true
            });
        }

        return true;
    }
}
