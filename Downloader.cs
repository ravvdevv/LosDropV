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
        UI.PrintStep($"Downloading  →  {Path.GetFileName(destinationPath)}");
        UI.PrintInfo($"URL: {url}");
        Console.WriteLine();

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

            byte[] buffer = new byte[81920];
            long downloadedBytes = 0;
            int bytesRead;

            // Speed tracking
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            long lastBytes = 0;
            long lastTick = stopwatch.ElapsedMilliseconds;

            Console.CursorVisible = false;

            while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                downloadedBytes += bytesRead;

                long now = stopwatch.ElapsedMilliseconds;
                long elapsed = now - lastTick;
                double speed = elapsed > 0
                    ? (downloadedBytes - lastBytes) / (elapsed / 1000.0)
                    : 0;

                if (elapsed >= 100) // update every 100ms
                {
                    lastBytes = downloadedBytes;
                    lastTick  = now;
                }

                UI.RenderDownloadBar(downloadedBytes, totalBytes, speed);
            }

            UI.ClearProgressLine();
            Console.CursorVisible = true;

            if (totalBytes.HasValue && downloadedBytes < totalBytes.Value)
            {
                UI.PrintError("Download incomplete — connection dropped.");
                return false;
            }

            UI.PrintSuccess($"Downloaded  {UI.FormatBytes(downloadedBytes)}  →  saved to temp folder");
            return true;
        }
        catch (TaskCanceledException)
        {
            UI.ClearProgressLine();
            Console.CursorVisible = true;
            UI.PrintError("Download timed out. Check your internet connection.");
            return false;
        }
        catch (HttpRequestException ex)
        {
            UI.ClearProgressLine();
            Console.CursorVisible = true;
            UI.PrintError($"Network error: {ex.Message}");
            return false;
        }
        catch (IOException ex)
        {
            UI.ClearProgressLine();
            Console.CursorVisible = true;
            UI.PrintError($"File write error: {ex.Message}");
            return false;
        }
    }
}
