# GitHub Copilot Prompt: Create a GPU-monitoring console app in C# (.NET 10)

Goal
-----

Generate a .NET 10 C# console application that monitors local NVIDIA GPU usage and displays a live terminal UI with graphs and a table, similar to `nvsharptop.cs` in this repository.

Requirements / Acceptance Criteria
---------------------------------

- Buildable with .NET 10 SDK (using top-level statements or Program.cs). The project must run with `dotnet run` and should also be publishable as a single-file executable.
- Use the `Spectre.Console` package to render the terminal UI (graphs, table, colors, markup).
- Query local NVIDIA GPUs using the `nvidia-smi` command with the same fields: `index,name,temperature.gpu,utilization.gpu,memory.used,memory.total`.
- Parse `nvidia-smi` CSV output safely and handle no-GPU cases without crashing.
- Continuously sample GPU stats at a configurable `--sample-interval` (default `1.0s`) and refresh the display at `--display-interval` (default `3s`).
- Provide a `--cleanup-screen <true|false>` option (default true) to clear the console on exit.
- Handle Ctrl+C gracefully and restore console state according to `--cleanup-screen`.
- Maintain per-GPU history buffers and render utilization and memory percent graphs with configurable graph width based on the console size.
- Provide a `--help` / `-h` option that prints usage and exits.
- Include build and run instructions and a short README section in the generated code comments or project README.

Interactive start screen
------------------------

- The program should display a welcome/start screen on launch that shows interactive controls and the current view mode and the current CLI-configurable options.
- Controls on the start screen:
  - `S` or `Enter` to start monitoring, `Q` to quit
  - `H` to select Horizontal mode, `V` to select Vertical mode (Vertical is the default)
  - `C` to toggle `--compact` (compact dense layout)
  - `A` to toggle `--animate` (render at sample rate)
  - `B` to cycle `--bg-glyph` through `space` → `dot` → `shade`
  - `P` to toggle `--pixel` (pixel-graded fractional blocks)
  - Left / Right arrows to decrease / increase `--sample-interval` (step ~0.1s, minimum 0.01s)
  - Down / Up arrows to decrease / increase `--display-interval` (step ~0.5s, minimum 0.1s)

- The welcome screen should be shown once at startup (or when the user changes H/V or any of the start-screen options before starting) to avoid repeated clear/write cycles that cause flicker; it is also refreshed when the console is resized.
- The start screen mirrors CLI options: any configuration available via command-line flags (for example `--bg-glyph` and `--pixel`) is visible on the welcome screen and may be changed interactively before starting. This makes the tool usable both via CLI args and via the interactive start screen.
- Precedence: CLI arguments (if provided) initialize the values shown on the start screen. Any changes the user makes on the start screen before starting will override the CLI-provided values for that run.

Notes
-----

- Default background glyph: the app defaults to `--bg-glyph shade` (uses a light shaded block for empty cells).
- Start key: the welcome screen accepts `S` or `Enter` to start monitoring.

Additional runtime behavior
--------------------------

- While the app is running the user should also be able to press `H` or `V` to toggle the view mode without restarting. `S` should act as pause/resume.
- The vertical view should render a stacked utilization+memory history with a percentage axis, simple time axis, and a small details table per GPU. The rendering should adapt to console width/height and use color-coded blocks and background dots for readability. In the stacked layout memory is shown above utilization within the same timestamp column.
- Add a fun optional "Star Wars Mode" toggled by `W` which renders GPU lines in an animated Star Wars-style intro crawl; this must not affect the existing Horizontal/Vertical/Compact modes and should be an independent rendering path.
- The default sampling interval should be `1.0` second. The default display/refresh interval should be `0.5` seconds. The vertical view may alternatively render per-timestamp bar charts (one stacked column per sample) for Util and Mem when the console width is sufficient. The vertical view renders scrolling per-timestamp bar groups with newest on the right. The view supports additional options: `--compact`, `--animate`, `--bar-char`, `--util-high`, `--util-warn`, `--bg-glyph`, and `--pixel`.

Default vertical view settings (should be the app defaults):

- `sample-interval`: `1.0` seconds
- `display-interval`: `0.5` seconds
- `compact`: `false`
- `animate`: `false`
- `bar-char`: `█`
- `util-high`: `90`
- `util-warn`: `70`
- `bg-glyph`: `shade`
- `pixel`: `false`

Project structure the prompt should ask the Copilot to create
-----------------------------------------------------------

- `nvsharptop.cs` (single-file top-level program) or `Program.cs` + `nvsharptop` classes — choose top-level for .NET 10 convenience.
- `nvsharptop.csproj` with target framework `net10.0` and a `PackageReference` for `Spectre.Console`.
- A minimal `README.md` with run and publish commands.

Functional contract for main components (to include in the generated code)
---------------------------------------------------------------------

- CliParameters
  - Inputs: `string[] args`
  - Outputs: `SampleInterval` (double), `DisplayInterval` (double), `CleanupScreen` (bool)
  - Error modes: invalid numeric args -> fallback to defaults; `--help` prints and exits
- DeviceInfo / DeviceSample / DeviceHistory
  - DeviceInfo: Id, Name, Temp (int), Util (int), MemUsed (int), MemTotal (int), Type enum
  - DeviceSample: Util (int), MemPct (int)
  - DeviceHistory: fixed-length queue of DeviceSample with AddSample
- DeviceCollector
  - Reads `nvidia-smi` using `ProcessStartInfo` and `RedirectStandardOutput`
  - Returns `IEnumerable<DeviceInfo>`; handles missing `nvidia-smi` by returning empty list and optionally showing a message
  - Should trim fields and guard against parse exceptions
- Renderer
  - Maintains per-device sample buffers, converts to history at display time (averaging samples between displays)
  - Computes graph width from console window width and chosen layout
  - Renders a graph per GPU (utilization stacked with memory) and a table with current values

CLI details and examples (include exact usage lines Copilot should generate)
-----------------------------------------------------------------

- Usage examples to include in generated README and `--help` text:
  - `dotnet run -- nvsharptop.cs --sample-interval 0.2 --display-interval 1 --cleanup-screen false`
  - `dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o out`
  - `.\out\nvsharptop.exe --sample-interval 0.1 --display-interval 3`

Edge cases to handle (explicitly instruct Copilot)
-----------------------------------------------

- No `nvidia-smi` found: print a friendly message once and keep retrying or exit with non-zero code depending on a command-line flag.
- `nvidia-smi` returns malformed CSV lines: skip those lines and continue.
- Zero or negative console width/height: fallback to sensible defaults.
- Very small graph width: ensure code doesn't divide by zero and shows at least a minimal placeholder.
- When `--cleanup-screen false` ensure console is left where the app finished without clearing.

Testing and validation to include in the generated repo
----------------------------------------------------

- Small self-check in `Main` that `nvidia-smi` can be executed; if not found print instructions.
- Unit-test-friendly separations: parsing `nvidia-smi` output should be in a separate method/class so tests can be added later.

Implementation notes for the Copilot prompt
---------------------------------------

- Use modern C# features: top-level statements, `record` types, target-typed new, pattern matching, `Queue<T>`, `IEnumerable<T>`.
- Keep the code in a single file (`nvsharptop.cs`) unless necessary; otherwise produce a small set of files as above.
- Use `Spectre.Console` primitives: `Table`, `Grid`, `Markup`, and colored Markup tags.
- Use `Spectre.Console` primitives: `Table`, `Grid`, `Markup`, and colored Markup tags.
- Implement a welcome/start screen (interactive) that displays the current mode and instructions to start or quit.
- Avoid external network calls. Read only `nvidia-smi` output locally.
- Depend on `Spectre.Console` version compatible with .NET 10 (latest stable available).

Prompt text (paste this into GitHub Copilot or GitHub Copilot Chat)
----------------------------------------------------------------

"""
Create a .NET 10 C# console application (single-file `nvsharptop.cs` and `nvsharptop.csproj`) that monitors NVIDIA GPUs using `nvidia-smi` and renders a live terminal UI using Spectre.Console. The app must:

- Target `net10.0` and include a `PackageReference` to `Spectre.Console`.
- Be runnable with `dotnet run nvsharptop.cs -- [args]` and publishable with `dotnet publish` as a single-file executable.
- Parse command-line options `--sample-interval <seconds>` (double, default `0.1`), `--display-interval <seconds>` (double, default `3`), `--cleanup-screen <true|false>` (bool, default `true`), and `--help`/`-h`.
- Parse command-line options `--sample-interval <seconds>` (double, default `1.0`), `--display-interval <seconds>` (double, default `3`), `--cleanup-screen <true|false>` (bool, default `true`), and `--help`/`-h`. Also support `--compact`, `--animate`, `--bar-char`, `--util-high`, `--util-warn`, `--bg-glyph`, and `--pixel`.
- Parse command-line options `--sample-interval <seconds>` (double, default `1.0`), `--display-interval <seconds>` (double, default `3`), `--cleanup-screen <true|false>` (bool, default `true`), and `--help`/`-h`. Include additional flags: `--compact`, `--animate`, `--bar-char`, `--util-high`, `--util-warn`, `--bg-glyph`, `--pixel`.
- Use `ProcessStartInfo` to run `nvidia-smi --query-gpu=index,name,temperature.gpu,utilization.gpu,memory.used,memory.total --format=csv,noheader,nounits` and parse CSV output safely into `DeviceInfo` records.
- Maintain per-device rolling history buffers and sample at `--sample-interval`, averaging samples between displays and then rendering graphs and a table on each display refresh (`--display-interval`).
- Use Spectre.Console to render a visually clear live UI: per-GPU graphs (utilization + memory stacked), a table of current metrics, and a header/footer showing timestamp and refresh rate.
- Handle no `nvidia-smi` gracefully (print message and exit or keep retrying) and guard against malformed lines.
- Handle Ctrl+C to exit cleanly and respect `--cleanup-screen`.

Interactive start-screen requirements for generated app:

- The generated program should include an interactive welcome/start screen that mirrors the CLI-configurable options and lets the user change them prior to starting. The welcome screen should display current values for `--sample-interval`, `--display-interval`, `--compact`, `--animate`, `--bg-glyph`, and `--pixel`.
- The welcome screen should accept the following keys to change options:
  - `C` toggle `--compact`
  - `A` toggle `--animate`
  - `B` cycle `--bg-glyph` through `space` → `dot` → `shade`
  - `P` toggle `--pixel`
  - Left / Right to decrease / increase `--sample-interval`
  - Down / Up to decrease / increase `--display-interval`
  - `S` or `Enter` to start and `Q` to quit

This makes the generated code usable both via CLI arguments and via the interactive start screen.

Include helpful inline comments, a short README explanation, and usage examples. Make functions small and testable (parsing logic separated).
"""
