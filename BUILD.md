# Building SOTMDecks

A cross-platform .NET 6 console app for simulating Sentinels of the Multiverse hero decks.

## Prerequisites

- **.NET 6 SDK** (any platform). Verify with `dotnet --version` (should report `6.0.x`).
  - Linux: `curl -fsSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 6.0` (installs to `~/.dotnet`; add it to `PATH` and set `DOTNET_ROOT=$HOME/.dotnet`).
  - Windows: install via the official .NET 6 SDK installer or `winget install Microsoft.DotNet.SDK.6`.
  - macOS: `brew install --cask dotnet-sdk` or use the official installer.

No additional system packages are required — the project depends only on `Newtonsoft.Json` and `Optional`, which `dotnet restore` pulls from NuGet.

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
dotnet run -c Release --no-build -- Haka
dotnet run -c Release --no-build -- Haka /custom/path/to/character_files
```

### CLI arguments

| Position | Meaning | Default |
|---|---|---|
| 1 | Hero filename without `.json` extension (e.g. `Haka`) | prompts interactively |
| 2 | Directory containing `*.json` hero files | `<exe directory>/character_files` |

Type `q` at the filename prompt to quit.

## Publish (standalone-ish binaries)

The project copies every `character_files/**/*.json` next to the executable on build and publish, so the published folder is fully self-contained for runtime data.

### Linux (framework-dependent — needs .NET 6 runtime on target)

```bash
dotnet publish -c Release -r linux-x64 --self-contained false -o ./out/linux
./out/linux/SOTMDecks Haka
```

### Windows

```powershell
dotnet publish -c Release -r win-x64 --self-contained false -o .\out\win
.\out\win\SOTMDecks.exe Haka
```

### macOS

```bash
dotnet publish -c Release -r osx-x64 --self-contained false -o ./out/mac
./out/mac/SOTMDecks Haka
```

For a fully self-contained build (no runtime needed on target), pass `--self-contained true` and add `-p:PublishSingleFile=true` if you want a single binary. Output size grows by ~70 MB.

## Hero data files

`character_files/*.json` is the canonical source of hero decks. Both `dotnet build` and `dotnet publish` copy the entire directory tree into the output folder via the `Content Include` rule in `SOTMDecks.csproj`, so released builds always ship with the latest deck JSON.

## Troubleshooting

- **`File doesn't exist: …/character_files/Foo.json`** — filename is case-sensitive and must match a file in `character_files/` (or the directory passed as the second CLI arg).
- **`Playing as ` prints with a blank name** — known pre-existing issue: `HeroDeck.Name` is never populated. The deck still works correctly.
- **`dotnet: command not found` on Linux after install script** — add the install dir to `PATH`:
  ```bash
  export PATH="$HOME/.dotnet:$PATH"
  export DOTNET_ROOT="$HOME/.dotnet"
  ```
- **Build fails with `Microsoft.VisualBasic.FileIO` errors** — you have an outdated checkout. The current code does not depend on `Microsoft.VisualBasic`; pull latest.
