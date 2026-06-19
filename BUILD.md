# Building SOTMDecks

A cross-platform .NET 10 console app for simulating Sentinels of the Multiverse hero decks.

## Prerequisites

- **.NET 10 SDK** (any platform). Verify with `dotnet --version` (should report `10.0.x`).
  - Linux: `curl -fsSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 10.0` (installs to `~/.dotnet`; add it to `PATH` and set `DOTNET_ROOT=$HOME/.dotnet`).
  - Windows: install via Visual Studio 2026 (includes the .NET 10 SDK), the official .NET 10 SDK installer, or `winget install Microsoft.DotNet.SDK.10`.
  - macOS: `brew install --cask dotnet-sdk` or use the official installer.

No additional system packages are required — the project depends only on `Newtonsoft.Json` and `Optional`, which `dotnet restore` pulls from NuGet.

> The publish examples below use `-r linux-x64` / `win-x64` / `osx-x64`. On Windows with Visual Studio 2026 you can also just build and run from the IDE.

## Build

From the repository root:

```bash
dotnet restore
dotnet build -c Release
```

A successful build prints `Build succeeded.` with `0 Error(s)`. Existing nullable-reference warnings (`CS8602`, `CS8604`, etc.) are pre-existing and do not block the build.

## Run

```bash
dotnet run -c Release --no-build
dotnet run -c Release --no-build -- hero Haka
dotnet run -c Release --no-build -- env Megalopolis
dotnet run -c Release --no-build -- hero Haka /custom/path/to/character_files
```

### CLI arguments

| Position | Meaning | Default |
|---|---|---|
| 1 | Mode: `hero`/`h` or `environment`/`env`/`e` | prompts interactively |
| 2 | Filename without `.json` (e.g. `Haka`, `Megalopolis`) | prompts interactively |
| 3 | Directory containing `*.json` files | `<exe directory>/character_files` (hero) or `<exe directory>/environment_files` (env) |

Type `q` at any prompt to quit.

### Modes

- **Hero mode** — loads a deck from `character_files/`, runs the full hero game (HP, hand, play area, modifiers, all hero commands).
- **Environment mode** — loads a deck from `environment_files/`, runs a slimmer environment loop:
  - `reveal` — turn the top card of the deck into the play area
  - `pa` / `play area`, `dp` / `discard pile`, `targets`, `deck count`
  - `damage card` / `damage cards` / `damage all`, `heal card` / `heal cards` / `heal all`
  - `discard` (move from PA to DP), `destroy` (PA → DP and trigger on-destroy text), `remove card` (PA → KO)
  - `shuffle`, `dp to deck`, `key words`, `q` / `exit`

## Publish (standalone-ish binaries)

The project copies every `character_files/**/*.json` next to the executable on build and publish, so the published folder is fully self-contained for runtime data.

### Linux (framework-dependent — needs .NET 10 runtime on target)

```bash
dotnet publish -c Release -r linux-x64 --self-contained false -o ./out/linux
./out/linux/SOTMDecks hero Haka
./out/linux/SOTMDecks env Megalopolis
```

### Windows

```powershell
dotnet publish -c Release -r win-x64 --self-contained false -o .\out\win
.\out\win\SOTMDecks.exe hero Haka
```

### macOS

```bash
dotnet publish -c Release -r osx-x64 --self-contained false -o ./out/mac
./out/mac/SOTMDecks hero Haka
```

For a fully self-contained build (no runtime needed on target), pass `--self-contained true` and add `-p:PublishSingleFile=true` if you want a single binary. Output size grows by ~70 MB.

## Hero and environment data files

`character_files/*.json` (hero decks) and `environment_files/*.json` (environment decks) are the canonical sources. Both directories are copied into the output folder on `dotnet build` and `dotnet publish` via `Content Include` rules in `SOTMDecks.csproj`, so released builds always ship with the latest deck JSON.

Three stub environment decks are included for testing: `Megalopolis`, `InsulaPrimalis`, `WagnerMarsBase`.

## Troubleshooting

- **`File doesn't exist: …/character_files/Foo.json`** — filename is case-sensitive and must match a file in `character_files/` (or the directory passed as the second CLI arg).
- **`Playing as ` prints with a blank name** — known pre-existing issue: `HeroDeck.Name` is never populated. The deck still works correctly.
- **`dotnet: command not found` on Linux after install script** — add the install dir to `PATH`:
  ```bash
  export PATH="$HOME/.dotnet:$PATH"
  export DOTNET_ROOT="$HOME/.dotnet"
  ```
- **Build fails with `Microsoft.VisualBasic.FileIO` errors** — you have an outdated checkout. The current code does not depend on `Microsoft.VisualBasic`; pull latest.
