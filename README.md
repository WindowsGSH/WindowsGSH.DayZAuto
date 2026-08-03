# DayZ Dedicated Server

[![WindowsGSH](.github/assets/windowsgsh-badge.svg)](https://windowsgsh.com)
[![Status](https://img.shields.io/badge/status-needs_live_test-F59E0B)](#status)
[![Module version](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fraw.githubusercontent.com%2FWindowsGSH%2FWindowsGSH.DayZAuto%2Fmain%2FDayZAuto.mod%2Fmodule.json&query=%24.version&prefix=v&label=module&color=0F766E)](DayZAuto.mod/module.json)
[![Requires WindowsGSH](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fraw.githubusercontent.com%2FWindowsGSH%2FWindowsGSH.DayZAuto%2Fmain%2FDayZAuto.mod%2Fmodule.json%3Fbadge%3Dminimum&query=%24.minimumWindowsGshVersion&prefix=v&label=requires%20WindowsGSH&color=2563EB)](DayZAuto.mod/module.json)
[![Licence](https://img.shields.io/badge/licence-MIT-64748B)](LICENSE.md)

This WindowsGSH module installs, configures, starts, stops, monitors, imports, and backs up a stable DayZ dedicated server.

## Status

**NEEDS LIVE TEST.** The module manages Bohemia's real `serverDZ.cfg`, launch/profile paths, UDP range, and persistence locations. Remote joining, live socket capture, shutdown safety, and Workshop-mod operation still require testing.

The historical repository and module ID contain `DayZAuto`, but this release does **not** claim automatic Workshop mod installation or updating. The ID remains stable so existing WindowsGSH server configurations continue loading.

## Installation

The module installs stable Steam app `223350` and launches `DayZServer_x64.exe` with `serverDZ.cfg` and the local `profiles` directory. Import `DayZAuto.mod`, add a server, install it, configure it, and start it.

### Import an existing server

WindowsGSH accepts either a direct DayZ installation or a WindowsGSM folder containing `serverfiles`. Preview verifies the executable and reads supported `serverDZ.cfg` settings. Copy and Adopt remain user choices; preview does not alter the source. Review external Workshop content separately.

## Configuration

WindowsGSH preservation-updates server name and description, join/admin passwords, player limit, instance ID, third-person/crosshair choices, Steam query port, and mission template in `serverDZ.cfg`. Unknown assignments, blocks, comments, and unrelated formatting survive; writes use atomic replacement.

The base port is passed with `-port`; `steamQueryPort` is written as base + 3. Additional arguments are trusted raw text. They may include `-mod`/`-serverMod`, but this module does not download, link, copy keys for, validate, or update those mods.

## Networking

| Purpose | Default | Protocol | Exposure |
| --- | ---: | --- | --- |
| DayZ game, Steam, and query range | `2302-2305` | UDP | Public; eligible for opt-in UPnP. |

The range follows current hosting guidance and places query at `2305`. Capture actual sockets and test remote joining before beta certification.

## Query, console, and administration

Status is process-based until reliable player-query behavior is proven. The console tails `profiles\*.RPT`; redirected stdin is not advertised. Administrator login uses `passwordAdmin`. BattlEye RCon requires a separate `profiles\BattlEye\BEServer_x64.cfg` and compatible client; WindowsGSH does not currently claim RCon support.

## Files and backups

| Purpose | Path |
| --- | --- |
| Executable | `DayZServer_x64.exe` |
| Managed configuration | `serverDZ.cfg` |
| Missions and persistence | `mpmissions` |
| Profiles, logs, and BattlEye | `profiles` |

The three user-data areas are backup targets. Steam binaries and Workshop downloads are not duplicated by backup.

## Known limitations

- No automatic Workshop mod installation/update workflow is implemented.
- Graceful Windows shutdown requires live proof; forced termination can damage persistence.
- Player queries and counts are not claimed.
- Passwords must exist in vendor configuration; restrict filesystem access and redact diagnostics.

## Beta verification checklist

- [ ] Fresh-install Steam app `223350`; start Chernarus and another owned terrain.
- [ ] Verify every managed value while unknown config blocks and comments survive.
- [ ] Capture UDP `2302-2305`; test discovery, direct joining, and opt-in UPnP.
- [ ] Test direct/WindowsGSM import, Copy, Adopt, source preservation, and imported lifecycle.
- [ ] Test a signed Workshop mod manually; confirm no automatic-update behavior is implied.
- [ ] Test Stop, app exit, Windows session ending, reattachment, update, Verify Files, crash diagnostics, backup, and restore.

## Support

Report issues at <https://github.com/WindowsGSH/WindowsGSH.DayZAuto> with versions, a redacted support bundle, and relevant output.

## Support development

If you like the work I do and would like to support continued WindowsGSH and module development, you can contribute here:

- [Ko-fi](https://ko-fi.com/shenniko)
- [PayPal](https://paypal.me/shenniko)

## Trust and source

Modules execute with WindowsGSH's permissions. Review `DayZAuto.mod/module.json`, the C# source, [SECURITY.md](SECURITY.md), and [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md) before importing an unfamiliar build. Configuration follows [Bohemia's server reference](https://community.bistudio.com/wiki/DayZ:Server_Configuration).
