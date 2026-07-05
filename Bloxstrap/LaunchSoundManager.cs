using NAudio.Wave;

namespace Bloxstrap
{
    /// <summary>
    /// Handles validation and playback of the custom launch sound.
    /// Supports MP3 and WAV. Only one sound plays at a time.
    /// </summary>
    public static class LaunchSoundManager
    {
        public const int MaxDurationSeconds = 20;

        private static readonly string[] SupportedExtensions = { ".mp3", ".wav" };

        private static readonly object _lock = new();
        private static WaveOutEvent? _output;
        private static AudioFileReader? _reader;

        public static bool IsPlaying
        {
            get
            {
                lock (_lock)
                    return _output?.PlaybackState == PlaybackState.Playing;
            }
        }

        /// <summary>
        /// Returns an error message, or null if the file is a valid launch sound.
        /// </summary>
        public static string? ValidateSoundFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return "The selected file does not exist.";

            string ext = Path.GetExtension(path).ToLowerInvariant();

            if (!SupportedExtensions.Contains(ext))
                return "Unsupported file format. Please select an MP3 or WAV file.";

            try
            {
                using var reader = new AudioFileReader(path);

                if (reader.TotalTime.TotalSeconds > MaxDurationSeconds)
                {
                    int totalSec = (int)reader.TotalTime.TotalSeconds;
                    string timeStr = totalSec >= 60 ? $"{totalSec / 60}m {totalSec % 60}s" : $"{totalSec}s";
                    return $"The selected sound is {timeStr} long. The maximum allowed length is {MaxDurationSeconds} seconds.";
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("LaunchSoundManager::ValidateSoundFile", $"Failed to read audio file: {ex.Message}");
                return "Could not read the audio file. It may be corrupt or in an unsupported format.";
            }

            return null;
        }

        public static void Play(string path, int volumePercent = 100)
        {
            const string LOG_IDENT = "LaunchSoundManager::Play";

            lock (_lock)
            {
                StopInternal();

                if (!File.Exists(path))
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Launch sound file not found: {path}");
                    return;
                }

                try
                {
                    _reader = new AudioFileReader(path)
                    {
                        Volume = Math.Clamp(volumePercent, 0, 100) / 100f
                    };

                    _output = new WaveOutEvent();
                    _output.Init(_reader);
                    _output.PlaybackStopped += (_, _) => Stop();
                    _output.Play();
                }
                catch (Exception ex)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Failed to play launch sound: {ex.Message}");
                    StopInternal();
                }
            }
        }

        public static void Stop()
        {
            lock (_lock)
                StopInternal();
        }

        private static void StopInternal()
        {
            try
            {
                _output?.Dispose();
                _reader?.Dispose();
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("LaunchSoundManager::StopInternal", $"Cleanup failed: {ex.Message}");
            }
            finally
            {
                _output = null;
                _reader = null;
            }
        }
    }
}
