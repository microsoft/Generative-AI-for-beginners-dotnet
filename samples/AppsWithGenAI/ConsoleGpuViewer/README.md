# ConsoleGpuViewer (Inspired on nvsharptop)

GPT-5 Devs - GPU Viewer

***Note:** This is based on the chat we had during the .NET & AI Community StandUp show with the author here (https://www.youtube.com/watch?v=ptdNWGj8CN8)
The original GPU viewer is this one: https://github.com/tjwald/nvsharptop
The full list of resources are here: https://learn.microsoft.com/collections/yjwzhet31ez28w*

This small console app queries local NVIDIA GPUs via `nvidia-smi` and renders a live terminal UI using `Spectre.Console`.

Quick start

```pwsh
dotnet run --project ConsoleGpuViewer\ConsoleGpuViewer.csproj -- --sample-interval 0.1 --display-interval 0.5 --cleanup-screen true
```

Publish (single-file Windows x64)

```pwsh
dotnet publish ConsoleGpuViewer\ConsoleGpuViewer.csproj -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
```

Interactive controls

- Press `S` to start / pause
- Press `S` or `Enter` to start / pause
- Press `Q` to quit
- Press `H` to switch to Horizontal view
- Press `V` to switch to Vertical view
- Press `W` to toggle Star Wars Mode (animated Star Wars intro crawl of GPU stats)

- Start-screen interactive options

- Before pressing `S` you can change some options directly on the welcome/start screen. These mirror CLI flags and will be applied when monitoring starts:
  - `C` toggle compact layout (`--compact`)
  - `A` toggle animate mode (`--animate`)
  - `B` cycle background glyph (`--bg-glyph`)
  - `P` toggle pixel-graded mode (`--pixel`)
  - Left / Right arrows decrease / increase `--sample-interval` by 0.1s
  - Down / Up arrows decrease / increase `--display-interval` by 0.5s

You can still pass all the same options on the command line; the start screen simply provides a quick interactive way to change them before starting.

Default view

- The default view is now Vertical. Toggle to Horizontal on the welcome screen before starting or while running using `H`/`V`.

Recent visual updates
---------------------

- Continuous vertical bar: the Vertical view now renders per-timestamp columns adjacent to each other (no extra spacer), producing a continuous vertical history bar. This makes it easier to visually read utilization/memory trends across time.
- Y-axis labels on every row: the percent labels on the Y axis now appear on every row (previously they appeared on every other row). The label width is 5 characters (e.g. `100%`) to align the chart grid.
- Tick density: because columns are now 1-character wide in the Vertical view, timestamp labels along the X axis are condensed. The renderer attempts to center short time labels under groups; if you prefer more readable timestamps, increase the console width or use a larger `display-interval` so fewer groups are shown.

Options of interest

- `--sample-interval <seconds>` : sampling frequency (default `1.0`)
-- `--display-interval <seconds>` : how often the UI aggregates samples and refreshes (default `0.5`)
- `--compact` : use a denser compact layout with smaller spacers between timestamp groups
- `--animate` : render at the sample rate (smooth scrolling) instead of the display interval
- `--bar-char <char>` : choose the character used for solid bars (default `█`)
- `--util-high <int>` : utilization percentage threshold that renders red (default `90`)
- `--util-warn <int>` : utilization percentage threshold that renders yellow (default `70`)
- `--bg-glyph <space|dot|shade>` : background glyph when a cell is empty (default `shade`)
- `--pixel <true|false>` : enable pixel-graded bar mode using Unicode fractional blocks for partial-row fills

Default vertical view settings

- `Vertical` view is the default and the default configuration used when you start without interactive changes:
  - `--sample-interval`: `1.0` seconds
  - `--display-interval`: `0.5` seconds
  - `--compact`: `false`
  - `--animate`: `false`
  - `--bar-char`: `█`
  - `--util-high`: `90`
  - `--util-warn`: `70`
  - `--bg-glyph`: `shade`
  - `--pixel`: `false`

Example with new options

```pwsh
dotnet run --project ConsoleGpuViewer\ConsoleGpuViewer.csproj -- --sample-interval 0.2 --display-interval 1 --bg-glyph dot --pixel true
```

Notes

- Requires `nvidia-smi` to be available in PATH (NVIDIA drivers).
- The app targets `net10.0`.
- Use a Unicode-capable terminal for best visual results.
