using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using WindowsGSH.Core.Modules;

namespace WindowsGSH.Modules.DayZ;

public sealed class DayZModule : ManifestBackedGameServerModule, IModuleExistingServerImportCapability
{
    private const string ConfigFile = "serverDZ.cfg";
    private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);
    private static readonly Regex Assignment = new(@"^(?<indent>\s*)(?<key>[A-Za-z][A-Za-z0-9]*)\s*=\s*(?<value>.*?);(?<tail>\s*(?://.*)?)$", RegexOptions.Compiled);

    public bool CanImport(string path) => ExistingInstallImport.CanImport(this, path);

    public Task<ModuleExistingServerImportProbe> PreviewImportAsync(string path, CancellationToken cancellationToken) =>
        ExistingInstallImport.PreviewAsync(this, path, cancellationToken);

    public override Task<IReadOnlyDictionary<string, object?>> ReadConfigFileSettingsAsync(ServerInstance instance, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        var path = Path.Combine(instance.InstallPath, ConfigFile);
        if (!File.Exists(path)) return Task.FromResult<IReadOnlyDictionary<string, object?>>(result);

        foreach (var line in File.ReadLines(path))
        {
            var match = Assignment.Match(line);
            if (!match.Success) continue;
            var key = match.Groups["key"].Value;
            var value = match.Groups["value"].Value.Trim();
            switch (key.ToLowerInvariant())
            {
                case "hostname": result["server.name"] = Unquote(value); break;
                case "description": result["server.description"] = Unquote(value); break;
                case "password": result["server.password"] = Unquote(value); break;
                case "passwordadmin": result["server.adminPassword"] = Unquote(value); break;
                case "maxplayers": result["server.maxPlayers"] = value; break;
                case "instanceid": result["server.instanceId"] = value; break;
                case "disable3rdperson": result["server.thirdPerson"] = value != "1"; break;
                case "disablecrosshair": result["server.crosshair"] = value != "1"; break;
                case "template": result["server.mission"] = Unquote(value); break;
                case "steamqueryport" when int.TryParse(value, out var query) && query > 3: result["network.port"] = query - 3; break;
            }
        }
        return Task.FromResult<IReadOnlyDictionary<string, object?>>(result);
    }

    public override Task WriteConfigFileSettingsAsync(ServerInstance instance, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = Path.Combine(instance.InstallPath, ConfigFile);
        var lines = File.Exists(path) ? File.ReadAllLines(path).ToList() : DefaultConfig();
        Set(lines, "hostname", Quote(GetSetting(instance, "server.name", "DayZ Dedicated Server")));
        Set(lines, "description", Quote(GetSetting(instance, "server.description", "A WindowsGSH DayZ server")));
        Set(lines, "password", Quote(GetSetting(instance, "server.password", "")));
        Set(lines, "passwordAdmin", Quote(GetSetting(instance, "server.adminPassword", "")));
        Set(lines, "maxPlayers", GetSetting(instance, "server.maxPlayers", "60"));
        Set(lines, "instanceId", GetSetting(instance, "server.instanceId", "1"));
        Set(lines, "disable3rdPerson", GetBoolean(instance, "server.thirdPerson", true) ? "0" : "1");
        Set(lines, "disableCrosshair", GetBoolean(instance, "server.crosshair", true) ? "0" : "1");
        var basePort = int.TryParse(GetSetting(instance, "network.port", "2302"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) ? parsed : 2302;
        Set(lines, "steamQueryPort", (basePort + 3).ToString(CultureInfo.InvariantCulture));
        Set(lines, "template", Quote(GetSetting(instance, "server.mission", "dayzOffline.chernarusplus")));

        Directory.CreateDirectory(instance.InstallPath);
        var temporary = path + ".windowsgsh.tmp";
        try { File.WriteAllLines(temporary, lines, Utf8NoBom); File.Move(temporary, path, true); }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
        return Task.CompletedTask;
    }

    private static List<string> DefaultConfig() =>
    [
        "hostname = \"DayZ Dedicated Server\";",
        "description = \"A WindowsGSH DayZ server\";",
        "password = \"\";",
        "passwordAdmin = \"\";",
        "maxPlayers = 60;",
        "verifySignatures = 2;",
        "forceSameBuild = 1;",
        "disable3rdPerson = 0;",
        "disableCrosshair = 0;",
        "instanceId = 1;",
        "storageAutoFix = 1;",
        "steamQueryPort = 2305;",
        "class Missions",
        "{",
        "    class DayZ",
        "    {",
        "        template = \"dayzOffline.chernarusplus\";",
        "    };",
        "};"
    ];

    private static void Set(List<string> lines, string key, string value)
    {
        for (var index = 0; index < lines.Count; index++)
        {
            var match = Assignment.Match(lines[index]);
            if (!match.Success || !match.Groups["key"].Value.Equals(key, StringComparison.OrdinalIgnoreCase)) continue;
            lines[index] = $"{match.Groups["indent"].Value}{key} = {value};{match.Groups["tail"].Value}";
            return;
        }
        lines.Add($"{key} = {value};");
    }

    private static string Quote(string value) => $"\"{value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal)}\"";
    private static string Unquote(string value) => value.Length >= 2 && value[0] == '"' && value[^1] == '"' ? value[1..^1].Replace("\\\"", "\"", StringComparison.Ordinal).Replace("\\\\", "\\", StringComparison.Ordinal) : value;
    private static bool GetBoolean(ServerInstance instance, string key, bool fallback) =>
        instance.Settings.TryGetValue(key, out var value) && bool.TryParse(value?.ToString(), out var parsed) ? parsed : fallback;
}
