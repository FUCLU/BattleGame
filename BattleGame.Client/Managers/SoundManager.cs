using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace BattleGame.Client.Managers
{
    internal class SoundManager
    {
        private static readonly object SettingsLock = new();
        private static AudioSettingsState _settings = AudioSettingsState.Load();
        private static AudioFileReader? _audioFile;
        private static WaveOutEvent? _waveOut;
        private static string _currentBgm = string.Empty;
        private static bool _isStopping;

        public static event EventHandler? SettingsChanged;

        public static float MusicVolume => _settings.MusicVolume;
        public static float SfxVolume => _settings.SfxVolume;
        public static string PreferredBgm => _settings.PreferredBgm;

        public static void PlayBGM(string fileName)
        {
            string requested = string.IsNullOrWhiteSpace(_settings.PreferredBgm)
                ? fileName
                : _settings.PreferredBgm;

            if (string.Equals(_currentBgm, requested, StringComparison.OrdinalIgnoreCase) &&
                _waveOut != null &&
                _audioFile != null)
            {
                SetAudioReaderVolume();
                return;
            }

            StopBGM();

            string path = ResolveBgmPath(requested);
            if (!File.Exists(path))
                path = ResolveBgmPath(fileName);

            if (!File.Exists(path))
                return;

            _audioFile = new AudioFileReader(path);
            SetAudioReaderVolume();

            _waveOut = new WaveOutEvent();
            _waveOut.Init(_audioFile);
            _waveOut.PlaybackStopped += (_, _) =>
            {
                if (_isStopping || _audioFile == null || _waveOut == null)
                    return;

                _audioFile.Position = 0;
                _waveOut.Play();
            };

            _currentBgm = Path.GetFileName(path);
            _waveOut.Play();
        }

        public static void StopBGM()
        {
            _isStopping = true;
            _waveOut?.Stop();
            _waveOut?.Dispose();
            _audioFile?.Dispose();
            _waveOut = null;
            _audioFile = null;
            _currentBgm = string.Empty;
            _isStopping = false;
        }

        public static void SetVolume(float volume)
        {
            SetMusicVolume(volume);
        }

        public static void SetMusicVolume(float volume)
        {
            lock (SettingsLock)
            {
                _settings.MusicVolume = ClampVolume(volume);
                _settings.Save();
            }

            SetAudioReaderVolume();
            SettingsChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void SetSfxVolume(float volume)
        {
            lock (SettingsLock)
            {
                _settings.SfxVolume = ClampVolume(volume);
                _settings.Save();
            }

            SettingsChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void SetPreferredBgm(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            lock (SettingsLock)
            {
                _settings.PreferredBgm = fileName.Trim();
                _settings.Save();
            }

            SettingsChanged?.Invoke(null, EventArgs.Empty);
            PlayBGM(_settings.PreferredBgm);
        }

        public static IReadOnlyList<string> GetAvailableBgmFiles()
        {
            string folder = ResolveBgmFolder();
            if (!Directory.Exists(folder))
                return Array.Empty<string>();

            return Directory.GetFiles(folder, "*.mp3")
                .Select(Path.GetFileName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Cast<string>()
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static void PlayButtonClick()
        {
            if (PlaySfx("ui_click.wav"))
                return;

            try
            {
                var signal = new SignalGenerator
                {
                    Gain = Math.Clamp(SfxVolume * 0.18, 0.01, 0.18),
                    Frequency = 880,
                    Type = SignalGeneratorType.Sin
                }.Take(TimeSpan.FromMilliseconds(55));

                var waveOut = new WaveOutEvent();
                waveOut.Init(signal);
                waveOut.PlaybackStopped += (_, _) => waveOut.Dispose();
                waveOut.Play();
            }
            catch
            {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        public static void PlayBattleMove()
        {
            PlaySfx("move_step.wav");
        }

        public static void PlayBattleGuard()
        {
            PlaySfx("guard.wav");
        }

        public static void PlayBattleAttack()
        {
            PlaySfx("attack.wav");
        }

        public static void PlayBattleDash()
        {
            PlaySfx("dash.wav");
        }

        public static void PlayBattleSkill()
        {
            PlaySfx("skill.wav");
        }

        public static void PlayBattleHit()
        {
            PlaySfx("hit.wav");
        }

        public static void PlayRoundAnnouncement(int roundNumber, bool suddenDeath)
        {
            if (suddenDeath)
            {
                PlaySfx("sudden_death.wav");
                return;
            }

            string roundFile = $"round_{Math.Max(1, roundNumber)}.wav";
            if (!PlaySfx(roundFile))
                PlaySfx("round_start.wav");
        }

        public static bool PlaySfx(string fileName)
        {
            float volume = SfxVolume;
            if (volume <= 0.001f || string.IsNullOrWhiteSpace(fileName))
                return false;

            string path = ResolveSfxPath(fileName);
            if (!File.Exists(path))
                return false;

            try
            {
                var reader = new AudioFileReader(path) { Volume = volume };
                var waveOut = new WaveOutEvent();
                waveOut.Init(reader);
                waveOut.PlaybackStopped += (_, _) =>
                {
                    waveOut.Dispose();
                    reader.Dispose();
                };
                waveOut.Play();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void SetAudioReaderVolume()
        {
            if (_audioFile != null)
                _audioFile.Volume = MusicVolume;
        }

        private static string ResolveBgmPath(string fileName)
            => Path.Combine(ResolveBgmFolder(), fileName);

        private static string ResolveSfxPath(string fileName)
            => Path.Combine(ResolveSfxFolder(), fileName);

        private static string ResolveBgmFolder()
        {
            string outputPath = Path.Combine(Application.StartupPath, "Assets", "Sounds", "BGM");
            if (Directory.Exists(outputPath))
                return outputPath;

            return Path.GetFullPath(Path.Combine(
                Application.StartupPath,
                "..", "..", "..",
                "Assets", "Sounds", "BGM"));
        }

        private static string ResolveSfxFolder()
        {
            string outputPath = Path.Combine(Application.StartupPath, "Assets", "Sounds", "SFX");
            if (Directory.Exists(outputPath))
                return outputPath;

            return Path.GetFullPath(Path.Combine(
                Application.StartupPath,
                "..", "..", "..",
                "Assets", "Sounds", "SFX"));
        }

        private static float ClampVolume(float volume)
            => Math.Clamp(volume, 0f, 1f);

        private sealed class AudioSettingsState
        {
            public float MusicVolume { get; set; } = 1.0f;
            public float SfxVolume { get; set; } = 0.75f;
            public string PreferredBgm { get; set; } = "xtremefreddy.mp3";

            public static AudioSettingsState Load()
            {
                try
                {
                    string path = SettingsPath();
                    if (!File.Exists(path))
                        return new AudioSettingsState();

                    var state = JsonSerializer.Deserialize<AudioSettingsState>(File.ReadAllText(path));
                    return state ?? new AudioSettingsState();
                }
                catch
                {
                    return new AudioSettingsState();
                }
            }

            public void Save()
            {
                try
                {
                    string path = SettingsPath();
                    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                    File.WriteAllText(path, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
                }
                catch
                {
                    // Audio settings should never block the game flow.
                }
            }

            private static string SettingsPath()
                => Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "BattleGame.Client",
                    "audio-settings.json");
        }
    }
}
