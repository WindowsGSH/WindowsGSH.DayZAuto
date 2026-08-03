using WindowsGSH.Core.Modules;

namespace WindowsGSH.Modules.DayZ;

internal static class ExistingInstallImport
{
    public static bool CanImport(IGameServerModule module, string path) =>
        !string.IsNullOrWhiteSpace(path) && Directory.Exists(path) && File.Exists(Path.Combine(Resolve(module, path), module.Runtime.StartPath));

    public static async Task<ModuleExistingServerImportProbe> PreviewAsync(IGameServerModule module, string path, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var source = Path.GetFullPath(path);
        var install = Resolve(module, source);
        var instance = new ServerInstance(Path.GetFileName(source), module.Name, module.Id, install, install, Path.Combine(source, "ServerConfig.json"), new Dictionary<string, object?>());
        var settings = new Dictionary<string, object?>(await module.ReadConfigFileSettingsAsync(instance, cancellationToken), StringComparer.OrdinalIgnoreCase);
        var warnings = settings.Count == 0 ? new[] { "No supported serverDZ.cfg settings were detected; review defaults before importing." } : Array.Empty<string>();
        return new ModuleExistingServerImportProbe(module.GetServerName(settings), install, settings, warnings);
    }

    private static string Resolve(IGameServerModule module, string path)
    {
        var source = Path.GetFullPath(path);
        if (File.Exists(Path.Combine(source, module.Runtime.StartPath))) return source;
        var serverFiles = Path.Combine(source, "serverfiles");
        return File.Exists(Path.Combine(serverFiles, module.Runtime.StartPath)) ? serverFiles : source;
    }
}
