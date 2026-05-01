using Spectre.Console;

namespace LosDropV;

public static class UI
{
    // ── Banner ───────────────────────────────────────────────────────────────
    public static void PrintBanner()
    {
        AnsiConsole.Clear();
        Console.Title = "LosDropV  ·  GTA V Mod Installer";

        AnsiConsole.Write(
            new FigletText("LosDropV")
                .Color(Color.Cyan1));

        var panel = new Panel(
            new Text("⚡ GTA V mod installer · v1.2.0 · made by raven", new Style(Color.Yellow)))
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 0, 1, 0)
        };
        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }

    // ── Main menu ────────────────────────────────────────────────────────────
    public static string PromptMenu()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]What would you like to do?[/]")
                .PageSize(10)
                .AddChoices(new[] {
                    "Install Menyoo PC",
                    "Install Simple Zombies",
                    "Install both mods",
                    "Exit"
                }));
    }

    // ── Section header ───────────────────────────────────────────────────────
    public static void PrintSection(string title)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule($"[yellow]⚡ {title.ToUpper()}[/]").RuleStyle("grey"));
    }

    // ── Step / status messages ───────────────────────────────────────────────
    public static void PrintStep(string message) => AnsiConsole.MarkupLine($"[cyan]◈[/] {message}");
    public static void PrintSuccess(string message) => AnsiConsole.MarkupLine($"[green]✔[/] {message}");
    public static void PrintError(string message) => AnsiConsole.MarkupLine($"[red]✘[/] {message}");
    public static void PrintWarning(string message) => AnsiConsole.MarkupLine($"[yellow]⚠[/] {message}");
    public static void PrintInfo(string message) => AnsiConsole.MarkupLine($"   [grey]{message}[/]");

    // ── Divider ──────────────────────────────────────────────────────────────
    public static void PrintDivider() => AnsiConsole.Write(new Rule().RuleStyle("grey30"));

    // ── Status/Spinner ───────────────────────────────────────────────────────
    public static async Task RunWithStatus(string message, Func<Task> action)
    {
        await AnsiConsole.Status()
            .StartAsync(message, async ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.SpinnerStyle(Style.Parse("yellow"));
                await action();
            });
    }

    // ── Final summary box ────────────────────────────────────────────────────
    public static void PrintSuccessSummary(string[] installedMods)
    {
        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn(new TableColumn("[yellow]⚡ All good - install complete![/]").Centered());
        
        foreach (var mod in installedMods)
            table.AddRow($"[green]✔[/] {mod}");

        table.Caption("[magenta]made by raven[/]");
        
        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("[grey]Open GTA V and have fun.[/]");
    }

    // ── Goodbye ──────────────────────────────────────────────────────────────
    public static void PrintGoodbye()
    {
        AnsiConsole.MarkupLine("\n[yellow]⚡ Thanks for using LosDropV![/]");
        AnsiConsole.MarkupLine("[grey]made by raven · have fun in Los Santos 🌆[/]\n");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────
    public static string FormatBytes(long bytes) => bytes switch
    {
        < 1024        => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
        _             => $"{bytes / (1024.0 * 1024):F2} MB"
    };

    // ── Progress Bar Wrapper ─────────────────────────────────────────────────
    public static async Task RunWithProgress(string taskName, Func<ProgressContext, Task> action)
    {
        await AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn())
            .StartAsync(async ctx =>
            {
                await action(ctx);
            });
    }
}