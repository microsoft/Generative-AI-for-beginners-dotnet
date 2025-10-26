// nvsharptop - GPU monitoring console app (single-file)
// This file implements a small GPU monitor that queries `nvidia-smi` and renders
// a live terminal UI using Spectre.Console. It is intentionally self-contained
// and uses top-level statements for .NET 10 convenience.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;

// -----------------------------
// CLI parsing using System.CommandLine
// -----------------------------
double sampleInterval = 1.0;
double displayInterval = 0.5;
bool cleanupScreen = true;
bool compactMode = false;
bool animate = false;
int utilHigh = 90;
int utilWarn = 70;
string barChar = "█";
string bgGlyph = "shade";
bool pixelMode = false;
bool cliStarWars = false;
bool sparklines = false;

// Simple manual CLI parsing to avoid depending on System.CommandLine bindings in this demo
var rawArgs = Environment.GetCommandLineArgs();
string[] cliArgs;
var dashIdx = Array.IndexOf(rawArgs, "--");
if (dashIdx >= 0)
    cliArgs = rawArgs.Skip(dashIdx + 1).ToArray();
else
    cliArgs = rawArgs.Skip(1).ToArray();

for (int i = 0; i < cliArgs.Length; i++)
{
    var a = cliArgs[i];
    switch (a)
    {
        case "--sample-interval":
            if (i + 1 < cliArgs.Length && double.TryParse(cliArgs[i + 1], out var si)) { sampleInterval = si; i++; }
            break;
        case "--display-interval":
            if (i + 1 < cliArgs.Length && double.TryParse(cliArgs[i + 1], out var di)) { displayInterval = di; i++; }
            break;
        case "--cleanup-screen":
            if (i + 1 < cliArgs.Length && bool.TryParse(cliArgs[i + 1], out var cs)) { cleanupScreen = cs; i++; } else { cleanupScreen = true; }
            break;
        case "--compact":
            compactMode = true; break;
        case "--animate":
            animate = true; break;
        case "--util-high":
            if (i + 1 < cliArgs.Length && int.TryParse(cliArgs[i + 1], out var uh)) { utilHigh = uh; i++; }
            break;
        case "--util-warn":
            if (i + 1 < cliArgs.Length && int.TryParse(cliArgs[i + 1], out var uw)) { utilWarn = uw; i++; }
            break;
        case "--bar-char":
            if (i + 1 < cliArgs.Length && !string.IsNullOrEmpty(cliArgs[i + 1])) { barChar = cliArgs[i + 1].Substring(0, 1); i++; }
            break;
        case "--bg-glyph":
            if (i + 1 < cliArgs.Length && !string.IsNullOrEmpty(cliArgs[i + 1])) { bgGlyph = cliArgs[i + 1].ToLowerInvariant(); i++; }
            break;
        case "--pixel":
            if (i + 1 < cliArgs.Length && bool.TryParse(cliArgs[i + 1], out var p)) { pixelMode = p; i++; } else { pixelMode = true; }
            break;
        case "--star-wars":
            // optional flag to start in Star Wars mode
            if (i + 1 < cliArgs.Length && bool.TryParse(cliArgs[i + 1], out var sw)) { cliStarWars = sw; i++; } else { cliStarWars = true; }
            break;
        case "--sparklines":
            // optional flag to enable sparklines
            if (i + 1 < cliArgs.Length && bool.TryParse(cliArgs[i + 1], out var sp)) { sparklines = sp; i++; } else { sparklines = true; }
            break;
        default:
            // ignore unknown
            break;
    }
}

// -----------------------------
// Collector: runs nvidia-smi and parses CSV output
// -----------------------------
List<DeviceInfo> CollectDevicesOnce()
{
    var result = new List<DeviceInfo>();
    var psi = new ProcessStartInfo("nvidia-smi",
        "--query-gpu=index,name,temperature.gpu,utilization.gpu,memory.used,memory.total --format=csv,noheader,nounits")
    {
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,
    };

    try
    {
        using var p = Process.Start(psi);
        if (p == null)
            return result;

        string? line;
        while ((line = p.StandardOutput.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            // CSV: index, name, temp, util, mem.used, mem.total
            var parts = line.Split(',');
            if (parts.Length < 6) continue;
            try
            {
                var idx = int.Parse(parts[0].Trim());
                var name = parts[1].Trim();
                var temp = int.TryParse(parts[2].Trim(), out var tt) ? tt : 0;
                var util = int.TryParse(parts[3].Trim(), out var uu) ? uu : 0;
                var memUsed = int.TryParse(parts[4].Trim(), out var mu) ? mu : 0;
                var memTotal = int.TryParse(parts[5].Trim(), out var mt) ? mt : 0;
                result.Add(new DeviceInfo(idx, name, temp, util, memUsed, memTotal));
            }
            catch
            {
                // skip malformed lines
                continue;
            }
        }

        p.WaitForExit(2000);
    }
    catch (Exception ex)
    {
        // Could not start nvidia-smi; return empty set
        AnsiConsole.MarkupLine($"[red]nvidia-smi failed: {ex.Message}[/]");
    }

    return result;
}

// -----------------------------
// Main runtime: sampling loop + render loop
// -----------------------------
var histories = new ConcurrentDictionary<int, DeviceHistory>();
var sampledSince = new ConcurrentDictionary<int, List<DeviceSample>>();

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) => { e.Cancel = true; cts.Cancel(); };

// Interactive control state
var started = false; // whether monitoring has been started from the welcome screen
var paused = false;  // pause/resume state when started
var verticalMode = true; // default to Vertical view
var starWarsMode = false; // Star Wars intro crawl mode (toggle with W)
int starWarsFrame = 0;
// The start screen should only be rendered when first shown or when the
// selected mode changes; otherwise repeated Clear()/Write() calls in the
// render loop cause a visible flicker before the user presses S.
var startScreenShown = false;
// Track console size so we can refresh the start screen when resized
var lastWindowWidth = Console.WindowWidth;
var lastWindowHeight = Console.WindowHeight;

// Start screen: show instructions and wait for key
void ShowStartScreen()
{
    AnsiConsole.Clear();
    var modeLabel = verticalMode ? "Vertical (default)" : "Horizontal";
    var optionsInfo = $"[green]Options (change before start):[/]\n  [bold]C[/] Compact: [yellow]{(compactMode ? "ON" : "OFF")}[/]\n  [bold]A[/] Animate: [yellow]{(animate ? "ON" : "OFF")}[/]\n  [bold]B[/] BG glyph: [yellow]{bgGlyph}[/]\n  [bold]P[/] Pixel mode: [yellow]{(pixelMode ? "ON" : "OFF")}[/]\n  Sample interval: [yellow]{sampleInterval:0.00}s[/]  Display interval: [yellow]{displayInterval:0.00}s[/]\n  [bold]W[/] Star Wars Mode: [yellow]{(starWarsMode || cliStarWars ? "ON" : "OFF")}[/]\n";
    optionsInfo += $"  [bold]K[/] Sparklines: [yellow]{(sparklines ? "ON" : "OFF")}[/]\n";

    var instructions = $"[bold green]GPT-5 Devs - GPU Viewer[/]\n\nCurrent mode: [yellow]{modeLabel}[/]\n\nPress [bold]S[/] or [bold]Enter[/] to start monitoring, [bold]Q[/] to quit.\nToggle mode before start: press [bold]H[/] for Horizontal or [bold]V[/] for Vertical.\n\n{optionsInfo}Use arrow keys Left/Right to decrease/increase sample interval, Up/Down to adjust display interval. Press [bold]Enter[/] or [bold]S[/] to begin.";

    var panel = new Panel(new Markup(instructions))
    { Border = BoxBorder.Double, Header = new PanelHeader("Welcome") };
    AnsiConsole.Write(panel);
}

// Do not draw the start screen here — the render loop will show it once
// when appropriate which prevents repeated Clear()/Write() flicker.

// Key processing task — always polls keys so user can change mode before starting
var keyTask = Task.Run(async () =>
{
    while (!cts.IsCancellationRequested)
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.S)
            {
                if (!started)
                {
                    started = true;
                    paused = false;
                }
                else
                {
                    // toggle pause/resume
                    paused = !paused;
                }
            }
            else if (key.Key == ConsoleKey.W)
            {
                // Toggle Star Wars mode
                starWarsMode = !starWarsMode;
                // When enabling Star Wars mode, ensure it's visible even if not started
                if (!started) startScreenShown = false;
            }
            else if (key.Key == ConsoleKey.K)
            {
                // Toggle sparklines
                sparklines = !sparklines;
                if (!started) startScreenShown = false;
            }
            else if (key.Key == ConsoleKey.Q)
            {
                cts.Cancel();
            }
            else if (key.Key == ConsoleKey.H)
            {
                verticalMode = false;
                if (!started) startScreenShown = false; // refresh start screen to show new selection
            }
            else if (key.Key == ConsoleKey.V)
            {
                verticalMode = true;
                if (!started) startScreenShown = false; // refresh start screen to show new selection
            }
            else if (!started && key.Key == ConsoleKey.C)
            {
                compactMode = !compactMode;
                startScreenShown = false;
            }
            else if (!started && key.Key == ConsoleKey.A)
            {
                animate = !animate;
                startScreenShown = false;
            }
            else if (!started && key.Key == ConsoleKey.B)
            {
                // cycle bgGlyph: space -> dot -> shade -> space
                bgGlyph = bgGlyph switch { "space" => "dot", "dot" => "shade", _ => "space" };
                startScreenShown = false;
            }
            else if (!started && key.Key == ConsoleKey.P)
            {
                pixelMode = !pixelMode;
                startScreenShown = false;
            }
            else if (!started && key.Key == ConsoleKey.LeftArrow)
            {
                // decrease sample interval by 0.1s (min 0.01)
                sampleInterval = Math.Max(0.01, Math.Round(sampleInterval - 0.1, 2));
                startScreenShown = false;
            }
            else if (!started && key.Key == ConsoleKey.RightArrow)
            {
                sampleInterval = Math.Round(sampleInterval + 0.1, 2);
                startScreenShown = false;
            }
            else if (!started && key.Key == ConsoleKey.DownArrow)
            {
                // decrease display interval by 0.5s
                displayInterval = Math.Max(0.1, Math.Round(displayInterval - 0.5, 2));
                startScreenShown = false;
            }
            else if (!started && key.Key == ConsoleKey.UpArrow)
            {
                displayInterval = Math.Round(displayInterval + 0.5, 2);
                startScreenShown = false;
            }
            else if (!started && (key.Key == ConsoleKey.Enter))
            {
                started = true;
                paused = false;
            }
        }
        else
        {
            try { await Task.Delay(100, cts.Token); } catch (OperationCanceledException) { break; }
        }
    }
}, cts.Token);

// Sampling task
var samplingTask = Task.Run(async () =>
{
    while (!cts.IsCancellationRequested)
    {
        var devices = CollectDevicesOnce().ToList();
        foreach (var d in devices)
        {
            var memPct = d.MemoryTotal == 0 ? 0 : (int)Math.Round(d.MemoryUsed * 100.0 / d.MemoryTotal);
            var sample = new DeviceSample(d.Utilization, memPct, DateTime.UtcNow);
            sampledSince.AddOrUpdate(d.Index, _ => new List<DeviceSample> { sample }, (_, list) => { list.Add(sample); return list; });
            // ensure history exists
            histories.GetOrAdd(d.Index, _ => new DeviceHistory(100));
        }

        await Task.Delay(TimeSpan.FromSeconds(Math.Max(0.01, sampleInterval)), cts.Token).ContinueWith(_ => { });
    }
}, cts.Token);

// Render loop
var lastDisplay = DateTime.UtcNow;

// Honor CLI flag to start in Star Wars mode
if (cliStarWars)
{
    starWarsMode = true;
    // if we requested Star Wars mode via CLI, consider the app "started" so render loop runs
    started = true;
    startScreenShown = false;
}
try
{
    while (!cts.IsCancellationRequested)
    {
        // If not yet started by user, show the start screen once and poll until started
        if (!started)
        {
            // If the window size changed, force a refresh of the start screen so
            // the layout adapts and we avoid visual glitches.
            var w = Console.WindowWidth;
            var h = Console.WindowHeight;
            if (w != lastWindowWidth || h != lastWindowHeight)
            {
                startScreenShown = false;
                lastWindowWidth = w;
                lastWindowHeight = h;
            }

            if (!startScreenShown)
            {
                ShowStartScreen();
                startScreenShown = true;
                // store sizes in case ShowStartScreen() depends on layout
                lastWindowWidth = Console.WindowWidth;
                lastWindowHeight = Console.WindowHeight;
            }
            try { await Task.Delay(200, cts.Token); } catch (OperationCanceledException) { break; }
            continue;
        }
        // If paused, still render but do not aggregate new samples into history
        var renderDelay = animate ? Math.Max(0.01, sampleInterval) : Math.Max(0.01, displayInterval);
        try { await Task.Delay(TimeSpan.FromSeconds(renderDelay), cts.Token); } catch (OperationCanceledException) { break; }
        var devices = CollectDevicesOnce().ToList();

        // Aggregate samples between displays
        foreach (var d in devices)
        {
            if (!paused)
            {
                if (sampledSince.TryRemove(d.Index, out var list))
                {
                    var avgUtil = (int)Math.Round(list.Average(s => s.Util));
                    var avgMem = (int)Math.Round(list.Average(s => s.MemPct));
                    histories.GetOrAdd(d.Index, _ => new DeviceHistory(100)).Add(new DeviceSample(avgUtil, avgMem, DateTime.UtcNow));
                }
                else
                {
                    // no samples collected; add current instant
                    var memPct = d.MemoryTotal == 0 ? 0 : (int)Math.Round(d.MemoryUsed * 100.0 / d.MemoryTotal);
                    histories.GetOrAdd(d.Index, _ => new DeviceHistory(100)).Add(new DeviceSample(d.Utilization, memPct, DateTime.UtcNow));
                }
            }
        }

        // Render
        AnsiConsole.Clear();

        // Star Wars mode render (separate, non-destructive)
        if (starWarsMode)
        {
            RenderStarWarsMode(devices, ref starWarsFrame, cts.Token);
            // advance frame for animation
            starWarsFrame = (starWarsFrame + 1) % 10000;
            lastDisplay = DateTime.UtcNow;
            continue;
        }

        if (!verticalMode)
        {
            var table = new Table().Border(TableBorder.Rounded).AddColumn("GPU").AddColumn("Util %").AddColumn("Mem %").AddColumn("Temp C").AddColumn("Name");
            foreach (var d in devices.OrderBy(d => d.Index))
            {
                table.AddRow(d.Index.ToString(), d.Utilization.ToString(),
                    (d.MemoryTotal == 0 ? 0 : (int)Math.Round(d.MemoryUsed * 100.0 / d.MemoryTotal)).ToString(), d.Temperature.ToString(), d.Name);
            }

            var panel = new Panel(table) { Header = new PanelHeader($"nvsharptop - {DateTime.Now:O}") };
            AnsiConsole.Write(panel);

            // Horizontal per-GPU bars (latest values)
            foreach (var d in devices.OrderBy(d => d.Index))
            {
                var hist = histories.GetOrAdd(d.Index, _ => new DeviceHistory(100)).Snapshot();
                // Latest sample or 0
                var last = hist.LastOrDefault();
                var utilVal = last?.Util ?? 0;
                var memVal = last?.MemPct ?? 0;

                var chart = new BarChart()
                    .Width(Math.Max(20, Math.Min(60, Console.WindowWidth - 10)))
                    .Label($"GPU {d.Index} - {d.Name}")
                    .CenterLabel();
                chart.AddItem("Util %", utilVal, Color.Green);
                chart.AddItem("Mem %", memVal, Color.Gold3);
                AnsiConsole.Write(chart);

                // Sparklines: compact recent utilization history
                if (sparklines)
                {
                    var spark = BuildSparkline(hist, Math.Min(24, Math.Max(6, Console.WindowWidth / 6)));
                    AnsiConsole.MarkupLine($"[grey]Spark:[/] {spark}");
                }
            }
        }
        else
        {
            // Vertical mode: scrolling per-timestamp bar chart (right = newest). Each timestamp shows two bars: Util (green) and Mem (cyan).
            foreach (var d in devices.OrderBy(d => d.Index))
            {
                var hist = histories.GetOrAdd(d.Index, _ => new DeviceHistory(400)).Snapshot();

                // Determine how many timestamps (groups) fit horizontally.
                // For stacked bars we render a single column per timestamp (mem above, util below)
                var leftLabelWidth = 5; // e.g. "100% "
                var avail = Math.Max(10, Console.WindowWidth - leftLabelWidth - 4);
                // For continuous vertical bar view we always use groupWidth = 1 so
                // bars render adjacent to each other with no spaces between groups.
                var groupWidth = 1;
                var groups = Math.Clamp(avail / groupWidth, 6, 200);

                // Build points from history: use the most recent `groups` samples (include Timestamp)
                var points = hist.Select(h => (Util: h.Util, Mem: h.MemPct, Timestamp: h.Timestamp)).ToList();
                if (points.Count == 0)
                {
                    AnsiConsole.MarkupLine($"[bold]GPU {d.Index} - {d.Name}[/] (no data yet)");
                    continue;
                }

                if (points.Count > groups)
                    points = points.Skip(points.Count - groups).ToList();
                else if (points.Count < groups)
                {
                    var pad = Enumerable.Repeat((Util: 0, Mem: 0, Timestamp: DateTime.UtcNow), groups - points.Count).ToList();
                    points = pad.Concat(points).ToList();
                }

                // Vertical resolution
                var rows = Math.Clamp(Console.WindowHeight - 12, 8, 24);

                // Header and legend (show paused if paused)
                var header = paused ? $"[bold]GPU {d.Index} - {d.Name}[/]  [red]PAUSED[/]" : $"[bold]GPU {d.Index} - {d.Name}[/]";
                AnsiConsole.MarkupLine(header);
                // show compact sparkline in vertical mode header when enabled
                if (sparklines)
                {
                    var histSmall = histories.GetOrAdd(d.Index, _ => new DeviceHistory(100)).Snapshot();
                    var sp = BuildSparkline(histSmall, Math.Min(24, Math.Max(6, Console.WindowWidth / 6)));
                    AnsiConsole.MarkupLine($"[grey]Spark:[/] {sp}");
                }
                AnsiConsole.MarkupLine($"[green]{barChar}[/] Util   [cyan]{barChar}[/] Mem    (Press H/V to toggle view while running, S to pause/resume, Q to quit)");

                // Build grid rows top->bottom (stacked bars: mem on top, util on bottom)
                var gridRows = new List<string>();
                // Prepare background glyph markup and pixel block set
                string bgMarkup;
                switch (bgGlyph)
                {
                    case "dot": bgMarkup = "[grey]·[/]"; break;
                    case "shade": bgMarkup = "[grey]░[/]"; break;
                    default: bgMarkup = "[grey] [/]"; break; // space
                }

                // Fractional blocks (increasing fill): use for pixel mode
                var fracBlocks = new[] { '▁', '▂', '▃', '▄', '▅', '▆', '▇' };
                for (int r = 0; r < rows; r++)
                {
                    var pctAtRow = (int)Math.Round((rows - r) * 100.0 / rows);
                    // Always show the percent label on every row (leftLabelWidth == 5)
                    var label = $"{pctAtRow,3}% ";
                    var sb = new System.Text.StringBuilder();
                    sb.Append(label);

                    // For each group (timestamp) draw a single stacked column: mem (top) then util (bottom)
                    foreach (var pt in points)
                    {
                        // Compute continuous heights in rows
                        var utilFloat = pt.Util * rows / 100.0;
                        var utilFull = (int)Math.Floor(utilFloat);
                        var utilFrac = utilFloat - utilFull;
                        var memFloat = pt.Mem * rows / 100.0;
                        var memFull = (int)Math.Floor(memFloat);
                        var memFrac = memFloat - memFull;
                        // If mem and util together would exceed available rows, trim mem so util always has space.
                        if (memFull + utilFull > rows)
                        {
                            var overlap = memFull + utilFull - rows;
                            memFull = Math.Max(0, memFull - overlap);
                            // If we trimmed memFull to 0, keep memFrac as-is; it will be drawn as fractional if space allows
                        }
                        var rowFromTop = r + 1;
                        var rowFromBottom = rows - r; // bottom = 1

                        // Determine what to draw at this row: mem (top priority) or util (bottom)
                        // mem occupies rows 1..memFull from the top; util occupies rows 1..utilFull from the bottom
                        if (rowFromTop <= memFull && memFull > 0)
                        {
                            // full mem block
                            sb.Append($"[cyan]{barChar}[/]");
                        }
                        else if (pixelMode && rowFromTop == memFull + 1 && memFrac > 0)
                        {
                            int idx = (int)Math.Floor(memFrac * fracBlocks.Length);
                            idx = Math.Clamp(idx, 0, fracBlocks.Length - 1);
                            sb.Append($"[cyan]{fracBlocks[idx]}[/]");
                        }
                        else if (rowFromBottom <= utilFull && utilFull > 0)
                        {
                            string utilColor = pt.Util >= utilHigh ? "red" : (pt.Util >= utilWarn ? "yellow" : "green");
                            sb.Append($"[{utilColor}]{barChar}[/]");
                        }
                        else if (pixelMode && rowFromBottom == utilFull + 1 && utilFrac > 0)
                        {
                            int idx = (int)Math.Floor(utilFrac * fracBlocks.Length);
                            idx = Math.Clamp(idx, 0, fracBlocks.Length - 1);
                            string utilColor = pt.Util >= utilHigh ? "red" : (pt.Util >= utilWarn ? "yellow" : "green");
                            sb.Append($"[{utilColor}]{fracBlocks[idx]}[/]");
                        }
                        else
                        {
                            sb.Append(bgMarkup);
                        }

                        // no spacer between groups to create a continuous vertical bar
                        // (groupWidth == 1 ensures columns are adjacent)
                    }

                    gridRows.Add(sb.ToString());
                }

                // Print grid rows (we build rows to match the visible group width so
                // avoid substring clipping which can break markup tags and introduce
                // visual artifacts). Each `line` contains markup but the visible
                // character width matches the computed group width.
                foreach (var line in gridRows)
                {
                    AnsiConsole.MarkupLine(line);
                }

                // X axis and time tick labels (newest on right)
                var axisPad = new string(' ', leftLabelWidth);
                var axisLen = Math.Min(points.Count * groupWidth, Math.Max(0, Console.WindowWidth - leftLabelWidth - 1));
                AnsiConsole.MarkupLine(axisPad + new string('─', axisLen));

                // timestamp ticks: show under every Nth group
                var tickStep = Math.Max(1, points.Count / 6);
                var tickSb = new System.Text.StringBuilder();
                tickSb.Append(axisPad);
                for (int i = 0; i < points.Count; i++)
                {
                    if (i % tickStep == 0)
                    {
                        var ts = points[i].Timestamp.ToLocalTime().ToString("HH:mm:ss");
                        var shortTs = ts.Substring(0, Math.Min(groupWidth, ts.Length));
                        // center the short timestamp inside the group width
                        var leftPad = (groupWidth - shortTs.Length) / 2;
                        tickSb.Append(new string(' ', leftPad));
                        tickSb.Append(shortTs);
                        tickSb.Append(new string(' ', groupWidth - leftPad - shortTs.Length));
                    }
                    else
                    {
                        tickSb.Append(new string(' ', groupWidth));
                    }
                }
                tickSb.Append($"  Refresh every {displayInterval}s   {DateTime.Now:HH:mm:ss}");
                var tickLine = tickSb.ToString();
                if (tickLine.Length > Console.WindowWidth - 1) tickLine = tickLine.Substring(0, Console.WindowWidth - 1);
                AnsiConsole.MarkupLine(tickLine);

                // Details table
                var table = new Table().Border(TableBorder.Rounded).AddColumn("Type").AddColumn("Id").AddColumn("Name").AddColumn("Temp").AddColumn("Util").AddColumn("Mem");
                table.AddRow("GPU", d.Index.ToString(), d.Name, $"{d.Temperature}°C", $"[green]{d.Utilization}%[/]", $"[cyan]{d.MemoryUsed}/{d.MemoryTotal}[/]");
                AnsiConsole.Write(table);
            }
        }

        lastDisplay = DateTime.UtcNow;
    }
}
catch (OperationCanceledException) { }
finally
{
    if (cleanupScreen)
    {
        AnsiConsole.Clear();
    }
}

// README note (also written to console for convenience)
AnsiConsole.MarkupLine("[grey]Run: dotnet run -- nvsharptop.cs -- --sample-interval 0.1 --display-interval 0.5 --cleanup-screen true[/]");

// Star Wars mode renderer: show per-GPU utilization bars that scroll upward (crawl) with numeric % at side.
void RenderStarWarsMode(List<DeviceInfo> devices, ref int frame, CancellationToken token)
{
    // Build lines for each device: keep the latest utilization value per GPU
    var gpus = devices.OrderBy(d => d.Index).ToList();
    if (gpus.Count == 0)
    {
        AnsiConsole.MarkupLine("[bold]Star Wars Mode[/] (no GPU data)");
        return;
    }

    // Screen geometry
    int width = Math.Max(40, Console.WindowWidth);
    int height = Math.Max(8, Console.WindowHeight - 4); // leave room for title/footer

    // Number of display rows in the scrolling region
    int scrollRows = height;

    // We create a buffer of scrollRows lines and place GPU bars at progressively higher positions
    var buffer = Enumerable.Repeat(string.Empty, scrollRows).ToArray();

    // For each GPU, compute a target vertical position that advances upward with frame
    // Spread GPUs vertically so they stagger during the crawl
    for (int i = 0; i < gpus.Count; i++)
    {
        var d = gpus[i];
        // Determine how fast each GPU climbs: base speed + small index factor
        double speed = 0.3 + (i * 0.05);
        // Compute a float position that cycles through the buffer upward
        double pos = (scrollRows + (frame * speed) - i * (scrollRows / (double)Math.Max(1, gpus.Count))) % (scrollRows + 10);
        // Convert to integer row (0 = top). We reverse so increasing frame moves the content upward
        int row = scrollRows - 1 - (int)Math.Floor(pos);
        if (row < 0 || row >= scrollRows) continue;

        // Compose a bar for this GPU: [label] [bar......] 98%
        var util = d.Utilization;
        int barArea = Math.Clamp(width - 20, 10, width - 10);
        int filled = (int)Math.Round(util * barArea / 100.0);
        string bar = new string('█', Math.Max(0, filled)) + new string('░', Math.Max(0, barArea - filled));

        string left = $"GPU {d.Index}".PadRight(8);
        string utilStr = $"{util,3}%";
        string color = util >= utilHigh ? "red" : (util >= utilWarn ? "yellow" : "green");

        var lineContent = left + " " + bar + " " + utilStr;
        if (lineContent.Length > Console.WindowWidth - 1)
            lineContent = lineContent.Substring(0, Console.WindowWidth - 1);

        // Put colored markup into buffer row (overwrite if already set — nearer GPUs may replace)
        buffer[row] = $"[{color}]{lineContent}[/]";

    }

    // Print title and buffer top->bottom so lines scroll upward visually
    AnsiConsole.MarkupLine("[bold yellow]STAR WARS - GPU BAR CRAWL[/]");
    AnsiConsole.MarkupLine("");

    for (int r = 0; r < scrollRows; r++)
    {
        var text = buffer[r];
        if (string.IsNullOrEmpty(text))
            AnsiConsole.MarkupLine(" ");
        else
            AnsiConsole.MarkupLine(text);
    }

    AnsiConsole.MarkupLine("");
    AnsiConsole.MarkupLine($"[grey]Press W to exit Star Wars mode. Frame: {frame}[/]");
}

// Build a compact sparkline from recent utilization history.
string BuildSparkline(IReadOnlyList<DeviceSample> hist, int width)
{
    if (hist == null || hist.Count == 0)
    {
        return new string('·', Math.Max(1, width));
    }

    // Choose last `width` samples (hist is oldest->newest)
    var take = Math.Min(width, hist.Count);
    var start = hist.Count - take;
    var blocks = new[] { '▁', '▂', '▃', '▄', '▅', '▆', '▇' };
    var sb = new System.Text.StringBuilder();
    for (int i = start; i < hist.Count; i++)
    {
        var s = hist[i];
        var idx = (int)Math.Floor(s.Util / 100.0 * (blocks.Length - 1));
        idx = Math.Clamp(idx, 0, blocks.Length - 1);
        sb.Append(blocks[idx]);
    }

    // Color the sparkline by the latest utilization
    var last = hist[hist.Count - 1];
    var color = last.Util >= utilHigh ? "red" : (last.Util >= utilWarn ? "yellow" : "green");
    return $"[{color}]{sb}[/]";
}
// Data models (declared after top-level statements)
// -----------------------------
record DeviceInfo(int Index, string Name, int Temperature, int Utilization, int MemoryUsed, int MemoryTotal);
// DeviceSample now includes a Timestamp so each sample can be labeled
record DeviceSample(int Util, int MemPct, DateTime Timestamp);

class DeviceHistory
{
    readonly int capacity;
    readonly Queue<DeviceSample> q;
    public DeviceHistory(int capacity)
    {
        this.capacity = Math.Max(1, capacity);
        q = new Queue<DeviceSample>(this.capacity);
    }
    public void Add(DeviceSample s)
    {
        q.Enqueue(s);
        while (q.Count > capacity) q.Dequeue();
    }
    public IReadOnlyList<DeviceSample> Snapshot() => q.ToArray();
}
