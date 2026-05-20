using BattleGame.Shared.Config;
using System.Text.Json;

namespace BattleGame.Client.Config
{
    public class ClientConfig
    {
        public string ServerIP { get; private set; } = GameConstants.ServerHost;
        public int ServerPort { get; private set; } = GameConstants.ServerPort;
        public string Profile { get; private set; } = "Default";

        public ClientConfig()
        {
            Load();
        }

        private void Load()
        {
            var settings = ClientNetworkSettings.Load();
            string activeProfile = FirstNonEmpty(
                    Environment.GetEnvironmentVariable("BATTLEGAME_CLIENT_PROFILE"),
                    settings.ActiveProfile,
                    "Local")
                ?? "Local";

            var profile = settings.GetProfile(activeProfile);
            if (profile != null)
            {
                Profile = activeProfile;
                ApplyProfile(profile);
            }

            string? hostOverride = Environment.GetEnvironmentVariable("BATTLEGAME_SERVER_HOST");
            if (!string.IsNullOrWhiteSpace(hostOverride))
                ServerIP = hostOverride.Trim();

            string? portOverride = Environment.GetEnvironmentVariable("BATTLEGAME_SERVER_PORT");
            if (int.TryParse(portOverride, out int port) && port > 0)
                ServerPort = port;
        }

        private void ApplyProfile(ClientNetworkProfile profile)
        {
            if (!string.IsNullOrWhiteSpace(profile.ServerHost))
                ServerIP = profile.ServerHost.Trim();

            if (profile.ServerPort > 0)
                ServerPort = profile.ServerPort;
        }

        private static string? FirstNonEmpty(params string?[] values)
        {
            foreach (string? value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    return value.Trim();
            }

            return null;
        }
    }

    internal sealed class ClientNetworkSettings
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        public string? ActiveProfile { get; set; }
        public Dictionary<string, ClientNetworkProfile> Profiles { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public ClientNetworkProfile? GetProfile(string profileName)
        {
            return Profiles.TryGetValue(profileName, out var profile) ? profile : null;
        }

        public static ClientNetworkSettings Load()
        {
            var merged = new ClientNetworkSettings();
            foreach (string fileName in new[] { "clientsettings.json", "clientsettings.Local.json" })
            {
                foreach (string path in CandidatePaths(fileName))
                {
                    if (!File.Exists(path))
                        continue;

                    try
                    {
                        var loaded = JsonSerializer.Deserialize<ClientNetworkSettings>(
                            File.ReadAllText(path),
                            JsonOptions);
                        if (loaded != null)
                            merged.Merge(loaded);
                    }
                    catch
                    {
                    }
                }
            }

            return merged;
        }

        private void Merge(ClientNetworkSettings other)
        {
            if (!string.IsNullOrWhiteSpace(other.ActiveProfile))
                ActiveProfile = other.ActiveProfile.Trim();

            if (other.Profiles == null)
                return;

            foreach (var pair in other.Profiles)
            {
                if (string.IsNullOrWhiteSpace(pair.Key) || pair.Value == null)
                    continue;

                Profiles[pair.Key.Trim()] = pair.Value;
            }
        }

        private static IEnumerable<string> CandidatePaths(string fileName)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string[] roots =
            {
                AppContext.BaseDirectory,
                Directory.GetCurrentDirectory(),
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."))
            };

            foreach (string root in roots)
            {
                string path = Path.GetFullPath(Path.Combine(root, fileName));
                if (seen.Add(path))
                    yield return path;
            }
        }
    }

    internal sealed class ClientNetworkProfile
    {
        public string? ServerHost { get; set; }
        public int ServerPort { get; set; }
    }
}
