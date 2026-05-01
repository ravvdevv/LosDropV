namespace LosDropV;

public static class UI
{
    // ── Color palette ────────────────────────────────────────────────────────
    private const ConsoleColor Gold      = ConsoleColor.Yellow;
    private const ConsoleColor Teal      = ConsoleColor.Cyan;
    private const ConsoleColor Lime      = ConsoleColor.Green;
    private const ConsoleColor Rose      = ConsoleColor.Red;
    private const ConsoleColor Violet    = ConsoleColor.Magenta;
    private const ConsoleColor Steel     = ConsoleColor.Gray;
    private const ConsoleColor White     = ConsoleColor.White;
    private const ConsoleColor DarkSteel = ConsoleColor.DarkGray;

    // ── Banner ───────────────────────────────────────────────────────────────
    public static void PrintBanner()
    {
        Console.Clear();
        Console.Title = "EZMod V  ⭐  by Raven";

        // ★ Star row
        Console.ForegroundColor = Gold;
        Console.WriteLine();
        Console.WriteLine(@"        ·  ★  ·  ·  ·  ★  ·  ·  ★  ·  ·  ·  ★  ·");
        Console.WriteLine();

        // EZMod V — big ASCII logo
        // EZ  in gold, Mod in teal, V in white
        Console.ForegroundColor = Gold;
        Console.WriteLine(@"   ███████╗███████╗");
        Console.WriteLine(@"   ██╔════╝╚════██║");
        Console.ForegroundColor = Gold;
        Console.Write(@"   █████╗      ███╔╝");
        Console.ForegroundColor = Teal;
        Console.WriteLine(@"  ███╗  ██████╗ ██████╗ ");
        Console.ForegroundColor = Gold;
        Console.Write(@"   ██╔══╝     ███╔╝ ");
        Console.ForegroundColor = Teal;
        Console.WriteLine(@"  ████╗ ██╔═══██╗██╔══██╗");
        Console.ForegroundColor = Gold;
        Console.Write(@"   ███████╗ ███╔╝  ");
        Console.ForegroundColor = Teal;
        Console.WriteLine(@"  ██╔██╗██║   ██║██║  ██║");
        Console.ForegroundColor = Gold;
        Console.Write(@"   ╚══════╝███████╗");
        Console.ForegroundColor = Teal;
        Console.WriteLine(@"  ██║╚████║   ██║██║  ██║");
        Console.ForegroundColor = Teal;
        Console.WriteLine(@"              ╚══════╝  ██║ ╚███╚██████╔╝██████╔╝");
        Console.WriteLine(@"                        ╚═╝  ╚══╝╚═════╝ ╚═════╝ ");

        //  big V
        Console.WriteLine();
        Console.ForegroundColor = White;
        Console.WriteLine(@"   ██╗   ██╗");
        Console.WriteLine(@"   ██║   ██║");
        Console.WriteLine(@"   ╚██╗ ██╔╝");
        Console.WriteLine(@"    ╚████╔╝ ");
        Console.WriteLine(@"     ╚═══╝  ");

        // Info strip
        Console.WriteLine();
        Console.ForegroundColor = DarkSteel;
        Console.WriteLine("  ╔══════════════════════════════════════════════════════╗");
        Console.ForegroundColor = Gold;
        Console.Write("  ║  ⭐  EZMod V");
        Console.ForegroundColor = Steel;
        Console.Write("  ·  GTA V Mod Installer  ·  v1.0.2d         ");
        Console.ForegroundColor = DarkSteel;
        Console.WriteLine("║");
        Console.ForegroundColor = DarkSteel;
        Console.Write("  ║  ");
        Console.ForegroundColor = Violet;
        Console.Write("✦  made by Raven");
        Console.ForegroundColor = Steel;
        Console.Write("  ·  Menyoo PC  +  Simple Zombies          ");
        Console.ForegroundColor = DarkSteel;
        Console.WriteLine("║");
        Console.WriteLine("  ╚══════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    // ── Main menu ────────────────────────────────────────────────────────────
    public static void PrintMenu()
    {
        Console.ForegroundColor = DarkSteel;
        Console.WriteLine("  ╔══════════════════════════════════════════════════╗");
        Console.ForegroundColor = Gold;
        Console.Write("  ║  ⚡ ");
        Console.ForegroundColor = White;
        Console.Write("LosDropV");
        Console.ForegroundColor = DarkSteel;
        Console.Write("  ·  ");
        Console.ForegroundColor = Steel;
        Console.Write("SELECT OPERATION");
        Console.ForegroundColor = DarkSteel;
        Console.WriteLine("                     ║");
        Console.WriteLine("  ╠══════════════════════════════════════════════════╣");

        Console.Write("  ║  ");
        Console.ForegroundColor = Teal;   Console.Write("[1]");
        Console.ForegroundColor = White;  Console.WriteLine("  Deploy Menyoo PC                           ║");

        Console.Write("  ║  ");
        Console.ForegroundColor = Violet; Console.Write("[2]");
        Console.ForegroundColor = White;  Console.WriteLine("  Deploy Simple Zombies                      ║");

        Console.Write("  ║  ");
        Console.ForegroundColor = Lime;   Console.Write("[3]");
        Console.ForegroundColor = White;  Console.WriteLine("  Deploy FULL SUITE (Both)                   ║");

        Console.Write("  ║  ");
        Console.ForegroundColor = Rose;   Console.Write("[4]");
        Console.ForegroundColor = White;  Console.WriteLine("  Exit                                       ║");

        Console.ForegroundColor = DarkSteel;
        Console.WriteLine("  ╚══════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
        Console.ForegroundColor = Gold;
        Console.Write("  ▶  Selection: ");
        Console.ResetColor();
    }

    // ── Section header ───────────────────────────────────────────────────────
    public static void PrintSection(string title)
    {
        Console.WriteLine();
        Console.ForegroundColor = Gold;
        Console.Write("  ╔══ ⚡ LosDropV  ·  ");
        Console.ForegroundColor = White;
        Console.Write(title.ToUpper());
        Console.ForegroundColor = Gold;
        int pad = Math.Max(0, 30 - title.Length);
        Console.WriteLine(" " + new string('═', pad) + "╗");
        Console.ResetColor();
    }

    public static void PrintSectionEnd()
    {
        Console.ForegroundColor = Gold;
        Console.WriteLine("  ╚" + new string('═', 51) + "╝");
        Console.ResetColor();
    }

    // ── Step / status messages ───────────────────────────────────────────────
    public static void PrintStep(string message)
    {
        Console.ForegroundColor = Teal;
        Console.Write("  ◈  ");
        Console.ForegroundColor = White;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void PrintSuccess(string message)
    {
        Console.ForegroundColor = Lime;
        Console.Write("  ✔  ");
        Console.ForegroundColor = White;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void PrintError(string message)
    {
        Console.ForegroundColor = Rose;
        Console.Write("  ✘  ");
        Console.ForegroundColor = White;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void PrintWarning(string message)
    {
        Console.ForegroundColor = Gold;
        Console.Write("  ⚠  ");
        Console.ForegroundColor = White;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void PrintInfo(string message)
    {
        Console.ForegroundColor = Steel;
        Console.WriteLine($"     {message}");
        Console.ResetColor();
    }

    // ── Divider ──────────────────────────────────────────────────────────────
    public static void PrintDivider()
    {
        Console.ForegroundColor = DarkSteel;
        Console.WriteLine("  " + new string('─', 52));
        Console.ResetColor();
    }

    // ── Spinner ──────────────────────────────────────────────────────────────
    private static readonly string[] SpinnerFrames = ["⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏"];
    private static CancellationTokenSource? _spinnerCts;
    private static Task? _spinnerTask;

    public static void StartSpinner(string message)
    {
        _spinnerCts = new CancellationTokenSource();
        CancellationToken token = _spinnerCts.Token;

        _spinnerTask = Task.Run(async () =>
        {
            int frame = 0;
            Console.CursorVisible = false;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    Console.ForegroundColor = Gold;
                    Console.Write($"\r  {SpinnerFrames[frame % SpinnerFrames.Length]}  ");
                    Console.ForegroundColor = Steel;
                    Console.Write(("LosDropV  ·  " + message).PadRight(55));
                    Console.ResetColor();
                    frame++;
                    await Task.Delay(80, token);
                }
                catch { break; }
            }
        }, token);
    }

    public static void StopSpinner(string? doneMessage = null, bool success = true)
    {
        _spinnerCts?.Cancel();
        try { _spinnerTask?.Wait(500); } catch { }
        Console.Write("\r" + new string(' ', 70) + "\r");
        Console.CursorVisible = true;
        if (doneMessage is not null)
        {
            if (success) PrintSuccess(doneMessage);
            else         PrintError(doneMessage);
        }
    }

    // ── Download progress bar ────────────────────────────────────────────────
    public static void RenderDownloadBar(long downloaded, long? total, double speedBps)
    {
        const int barWidth = 28;
        string speed = FormatSpeed(speedBps);
        try
        {
            if (total.HasValue && total.Value > 0)
            {
                double pct = Math.Min(1.0, (double)downloaded / total.Value);
                int filled = (int)(pct * barWidth);

                Console.ForegroundColor = DarkSteel;
                Console.Write("\r  [");
                Console.ForegroundColor = Lime;
                Console.Write(new string('█', filled));
                Console.ForegroundColor = DarkSteel;
                Console.Write(new string('░', barWidth - filled));
                Console.Write("] ");
                Console.ForegroundColor = Gold;
                Console.Write($"{pct:P0}".PadLeft(4));
                Console.ForegroundColor = Steel;
                Console.Write($"  {FormatBytes(downloaded)}/{FormatBytes(total.Value)}");
                Console.ForegroundColor = Teal;
                Console.Write($"  ↓ {speed}   ");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = DarkSteel;
                Console.Write("\r  [");
                Console.ForegroundColor = Teal;
                int pos = (int)(downloaded / 81920) % barWidth;
                for (int i = 0; i < barWidth; i++)
                    Console.Write(i == pos ? '█' : '░');
                Console.ForegroundColor = DarkSteel;
                Console.Write("] ");
                Console.ForegroundColor = Steel;
                Console.Write($"{FormatBytes(downloaded)}  ↓ {speed}   ");
                Console.ResetColor();
            }
        }
        catch { }
    }

    public static void ClearProgressLine()
    {
        Console.Write("\r" + new string(' ', 72) + "\r");
    }

    // ── Final summary box ────────────────────────────────────────────────────
    public static void PrintSuccessSummary(string[] installedMods)
    {
        Console.WriteLine();
        Console.ForegroundColor = Lime;
        Console.WriteLine("  ┌──────────────────────────────────────────────────────┐");
        Console.ForegroundColor = Gold;
        Console.WriteLine("  │   ⚡ LosDropV  ·  DEPLOYMENT SUCCESSFUL!  🎮         │");
        Console.ForegroundColor = Lime;
        Console.WriteLine("  ├──────────────────────────────────────────────────────┤");
        foreach (string mod in installedMods)
        {
            string line = $"  │   ✔  {mod}";
            Console.ForegroundColor = White;
            Console.WriteLine(line.PadRight(55) + "│");
        }
        Console.ForegroundColor = Lime;
        Console.WriteLine("  ├──────────────────────────────────────────────────────┤");
        Console.ForegroundColor = Steel;
        Console.WriteLine("  │   Launch GTA V and enjoy your mods!                  │");
        Console.ForegroundColor = Violet;
        Console.WriteLine("  │   Powered by LosDropV Architecture                   │");
        Console.ForegroundColor = Lime;
        Console.WriteLine("  └──────────────────────────────────────────────────────┘");
        Console.ResetColor();
        Console.WriteLine();
    }

    // ── Goodbye ──────────────────────────────────────────────────────────────
    public static void PrintGoodbye()
    {
        Console.WriteLine();
        Console.ForegroundColor = Gold;
        Console.WriteLine("  ⚡ Thanks for using LosDropV!");
        Console.ForegroundColor = Steel;
        Console.WriteLine("  Stay safe in Los Santos. 🌆");
        Console.ForegroundColor = DarkSteel;
        Console.WriteLine("  " + new string('─', 40));
        Console.ResetColor();
        Console.WriteLine();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────
    public static string FormatBytes(long bytes) => bytes switch
    {
        < 1024        => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
        _             => $"{bytes / (1024.0 * 1024):F2} MB"
    };

    private static string FormatSpeed(double bps) => bps switch
    {
        < 1024        => $"{bps:F0} B/s",
        < 1024 * 1024 => $"{bps / 1024:F1} KB/s",
        _             => $"{bps / (1024 * 1024):F2} MB/s"
    };
}
