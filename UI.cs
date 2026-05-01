using Spectre.Console;

namespace LosDropV;

public static class UI
{
    // ── Color palette ────────────────────────────────────────────────────────
    private const string Gold   = "yellow";
    private const string Teal   = "cyan";
    private const string Lime   = "green";
    private const string Rose   = "red";
    private const string Violet = "magenta";
    private const string Steel  = "grey";
    private const string White  = "white";

    // ── Banner ───────────────────────────────────────────────────────────────
    public static void PrintBanner()
    {
        AnsiConsole.Clear();
        Console.Title = "LosDropV  ·  GTA V Mod Installer";

        AnsiConsole.Write(
            new FigletText("LosDropV")
                .LeftAligned()
                .Color(Color.Cyan1));

        var panel = new Panel(
            new Text("⚡ GTA V Mod Deployment Tool · v1.2.0", new Style(Color.Yellow)))
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
                .Title("[yellow]SELECT OPERATION[/]")
                .PageSize(10)
                .AddChoices(new[] {
                    "Deploy Menyoo PC",
                    "Deploy Simple Zombies",
                    "Deploy FULL SUITE (Both)",
                    "Exit"
                }));
    }

    // ── Section header ───────────────────────────────────────────────────────
    public static void PrintSection(string title)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule($"[yellow]⚡ {title.ToUpper()}[/]").LeftAligned().RuleStyle("grey"));
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
        table.AddColumn(new TableColumn("[yellow]⚡ LosDropV · DEPLOYMENT SUCCESSFUL![/]").Centered());
        
        foreach (var mod in installedMods)
            table.AddRow($"[green]✔[/] {mod}");

        table.AddFooter("[grey]Launch GTA V and enjoy your mods![/]");
        table.Caption("[magenta]Powered by LosDropV Architecture[/]");
        
        AnsiConsole.Write(table);
    }

    // ── Goodbye ──────────────────────────────────────────────────────────────
    public static void PrintGoodbye()
    {
        AnsiConsole.MarkupLine("\n[yellow]⚡ Thanks for using LosDropV![/]");
        AnsiConsole.MarkupLine("[grey]Stay safe in Los Santos. 🌆[/]\n");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────
    public static string FormatBytes(long bytes) => bytes switch
    {
        < 1024        => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
        _             => $"{bytes / (1024.0 * 1024):F2} MB"
    };
}