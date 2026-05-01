using Spectre.Console;

namespace LosDropV;

public static class Downloader
{
    private static readonly HttpClient _httpClient = new(new HttpClientHandler
    {
        AllowAutoRedirect = true,
        MaxAutomaticRedirections = 10,
    })
    {
        Timeout = TimeSpan.FromMinutes(10),
    };

    static Downloader()
    {
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
            "(KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
    }

    public static async Task<bool> DownloadFileAsync(string url, string destinationPath)
    {
        UI.PrintStep($"Downloading [white]{Path.GetFileName(destinationPath)}[/]");
        UI.PrintInfo($"URL: [blue]{url}[/]");

        try
        {
            using HttpResponseMessage response = await _httpClient.GetAsync(
                url, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                UI.PrintError($"HTTP {(int)response.StatusCode} {response.ReasonPhrase}");
                return false;
            }

            long? totalBytes = response.Content.Headers.ContentLength;
            await using Stream contentStream = await response.Content.ReadAsStreamAsync();
            await using FileStream fileStream = new(destinationPath, FileMode.Create,
                FileAccess.Write, FileShare.None, 81920, true);

            await UI.RunWithProgress("Download", async (ctx) =>
            {
                var task = ctx.AddTask($"[cyan]{Path.GetFileName(destinationPath)}[/]", maxValue: totalBytes ?? 100);
                
                byte[] buffer = new byte[81920];
                long downloadedBytes = 0;
                int bytesRead;

                while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                    downloadedBytes += bytesRead;
                    
                    if (totalBytes.HasValue)
                        task.Value = downloadedBytes;
                    else
                        task.Increment(bytesRead);
                }
            });

            UI.PrintSuccess($"Downloaded [white]{UI.FormatBytes(downloadedBytes)}[/]");
            return true;
        }
        catch (Exception ex)
        {
            UI.PrintError($"Download failed: {ex.Message}");
            return false;
        }
    }
}