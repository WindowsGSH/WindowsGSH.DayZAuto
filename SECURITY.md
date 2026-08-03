# Security policy

## Security and trust

The DayZ module executes C# and starts the DayZ dedicated server with the current user's Windows permissions. WindowsGSH cannot guarantee arbitrary third-party or modified modules. Review source, manifests, dependencies, and download origins before use.

## Download modules safely

Obtain the module from the official repository or another trusted source. Install server files and mods from legitimate Steam/Workshop sources, review mod provenance, and avoid untrusted repackaged binaries.

## Protect credentials and server data

Protect Steam credentials, server/admin passwords, private keys, player data, profiles, logs, configs, mod settings, and backups. Redact command lines and diagnostics.

## Report a vulnerability

Use the [private repository advisory page](https://github.com/WindowsGSH/WindowsGSH.DayZAuto/security/advisories/new). Do not publish exploits, credentials, private server data, or unredacted diagnostics.

## Include in a report

Include module/WindowsGSH/server versions, package and mod provenance, reproduction steps, impact, and sanitized diagnostics.

## Supported versions

Security fixes target the latest module release and current WindowsGSH module API unless stated otherwise.

