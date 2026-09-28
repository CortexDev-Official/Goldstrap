> [!CAUTION]
> The only official place to download Goldstrap is this GitHub repository.
> Any other websites offering downloads or claiming to be us are not controlled
> by us, do not download from them.

<div align="center">

![][banner-light]
![][banner-dark]

</div>

Goldstrap is a custom bootstrapper for Roblox based on
[Bloxstrap][bloxstrap] (pronounced blox-strap), forked from
[Fishstrap][fishstrap]. It provides additional features
to enhance your experience.

If you found any bugs, please [open an issue here][repo-new-issue]

> [!NOTE]
> Goldstrap is an application for **Windows 10 and above.** For other operating
> systems, such as Mac OS and various Linux distributions, you can try
> [AppleBlox][appleblox] and [Sober][sober] respectively.

## Features

- Detailed server information using [RoValra][rovalra]'s API
- Support for Roblox Studio
- Unhidden FastFlags editor
  - You cannot apply FastFlags not present in the allowlist. This does not
    affect Roblox Studio. [Learn more][devforum-fflags]
- Global Basic Settings editor
  - Ability to increase frame rate cap, toggle quality levels and more
- Goldstrap's own game invites
- Cache cleaner, channel switcher and many more

## Special thanks

- [returnrqt](https://github.com/returnrqt) for creating Fishstrap (the original fork)
- [Valra](https://github.com/NotValra) for providing their API
- Other independent contributors

<div align="center">

![][repo-showcase-light]
![][repo-showcase-dark]

</div>

## Built with

Goldstrap is a .NET 6 WPF application. Full dependency list: [`Bloxstrap/Goldstrap.csproj`](Bloxstrap/Goldstrap.csproj).

- [Wpf.Ui](https://github.com/lepoco/wpfui) — Fluent controls and window chrome (submodule `wpfui`)
- CommunityToolkit.Mvvm — ViewModels and commands
- XamlAnimatedGif — animated GIFs (loading, profile)
- NAudio — launch sound playback
- AvalonEdit — FastFlag / settings editor
- DiscordRichPresence — Discord rich presence
- Markdig — Markdown rendering in the UI
- SharpZipLib — Roblox package extraction
- TagLibSharp — audio metadata
- SharpVectors.Wpf — SVG rendering
- securifybv.ShellLink — shortcut creation
- Microsoft.Windows.CsWin32 — Win32 P/Invoke generation

## Architecture

- **`App.OnStartup`** parses the launch arguments and decides what to run (installer, bootstrapper, watcher, settings or the menu).
- **`Bootstrapper`** downloads and repairs the Roblox client, applies mods and FastFlags, then launches it.
- **`Watcher`** runs alongside Roblox for rich presence, server info and cleanup.
- **`Integrations/`** holds optional features (Discord RPC, activity tracking, window handling, launch sound).
- **`UI/`** is the WPF layer (Wpf.Ui) with pages, dialogs and the `ProfileWidget`.
- Settings and state are JSON files handled by **`JsonManager<T>`**: `Settings.json`, `State.json`, `RobloxState.json` and `Profile.json`.

## Building from source

```
git clone --recurse-submodules https://github.com/CortexDev-Official/Goldstrap
cd Goldstrap
dotnet build -c Release
```

Run it:

```
dotnet run --project Bloxstrap/Goldstrap.csproj
```

Single-file release build:

```
dotnet publish Bloxstrap/Goldstrap.csproj -p:PublishSingleFile=true -r win-x64 -c Release --self-contained false
```

Requirements: Windows 10+, the .NET 6 SDK, and the .NET 6 Desktop Runtime to run.

## Differences from Fishstrap

Goldstrap is forked from [Fishstrap](https://github.com/returnrqt/fishstrap) and adds:

- A local **user profile** (avatar, username, badge, font, daily streak)
- A reworked **Launch Sound** (MP3/WAV, volume, preview)
- **Roblox app settings** (background app + app theme)
- Restored Fishstrap's standard Save notification, refreshed gold theming, and a passing security/bug audit

[banner-light]: https://github.com/CortexDev-Official/Goldstrap/raw/main/Images/Goldstrap-Gold.png#gh-light-mode-only
[banner-dark]:  https://github.com/CortexDev-Official/Goldstrap/raw/main/Images/Goldstrap-Gold.png#gh-dark-mode-only

[badge-license]:   https://img.shields.io/github/license/CortexDev-Official/Goldstrap?style=flat-square
[badge-actions]:   https://img.shields.io/github/actions/workflow/status/CortexDev-Official/Goldstrap/ci-release.yml?branch=main&style=flat-square&label=builds
[badge-downloads]: https://img.shields.io/github/downloads/CortexDev-Official/Goldstrap/latest/total?style=flat-square&color=FFD700
[badge-latest]:    https://img.shields.io/github/v/release/CortexDev-Official/Goldstrap?style=flat-square&color=DAA520
[badge-discord]:   https://img.shields.io/discord/1299397064165429360?style=flat-square&logo=discord&logoColor=white&logoSize=auto&label=discord&color=FFD700
[badge-stars]:     https://img.shields.io/github/stars/CortexDev-Official/Goldstrap?style=flat-square&color=FFD700

[repo-latest]:    https://github.com/CortexDev-Official/Goldstrap/releases/latest
[repo-new-issue]: https://github.com/CortexDev-Official/Goldstrap/issues/new/choose

[repo-showcase-dark]:  https://github.com/CortexDev-Official/Goldstrap/raw/main/Images/Showcase-Dark.png#gh-dark-mode-only
[repo-showcase-light]: https://github.com/CortexDev-Official/Goldstrap/raw/main/Images/Showcase-Light.png#gh-light-mode-only

[discord-invite]: https://discord.gg/SRs5zb9BJd

[bloxstrap]: https://bloxstraplabs.com
[fishstrap]: https://github.com/returnrqt/fishstrap
[appleblox]: https://github.com/AppleBlox/appleblox
[sober]:     https://sober.vinegarhq.org
[rovalra]:   https://www.rovalra.com

[devforum-fflags]: https://devforum.roblox.com/t/allowlist-for-local-client-configuration-via-fast-flags/3966569
