using NAudio.Wave;

namespace Bloxstrap.Integrations
{
    internal static class LaunchSound
    {
        public const int MaxDurationSeconds = 20;

        private static readonly object _lock = new();

        private static IWavePlayer? _output;
        private static AudioFileReader? _reader;
        private static ManualResetEventSlim? _stopped;

        public static bool IsPlaying
        {
            get
            {
                lock (_lock)
                    return _output?.PlaybackState == PlaybackState.Playing;
            }
        }

        public static bool HasFile => !String.IsNullOrWhiteSpace(App.Settings.Prop.LaunchSoundPath) && File.Exists(App.Settings.Prop.LaunchSoundPath);

        public static void Play(string path, int volume)
        {
            Stop();

            if (String.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return;

            lock (_lock)
            {
                _reader = new AudioFileReader(path);

                _output = new WaveOutEvent();
                _output.Init(_reader);
                _output.Volume = Math.Clamp(volume, 0, 100) / 100f;

                _stopped = new ManualResetEventSlim(false);

                _output.PlaybackStopped += (_, _) =>
                {
                    try { _stopped?.Set(); }
                    catch (ObjectDisposedException) { }
                };

                _output.Play();
            }
        }

        public static void PlayBlocking(string path, int volume)
        {
            Play(path, volume);

            ManualResetEventSlim? stopped;

            lock (_lock)
                stopped = _stopped;

            stopped?.Wait(TimeSpan.FromSeconds(MaxDurationSeconds + 10));

            Stop();
        }

        public static void Stop()
        {
            lock (_lock)
            {
                try { _output?.Stop(); } catch (Exception) { }
                try { _output?.Dispose(); } catch (Exception) { }
                try { _reader?.Dispose(); } catch (Exception) { }

                _output = null;
                _reader = null;
                _stopped = null;
            }
        }

        public static TimeSpan GetDuration(string path)
        {
            using var reader = new AudioFileReader(path);
            return reader.TotalTime;
        }

        public static bool IsValid(string path, out TimeSpan duration)
        {
            duration = TimeSpan.Zero;

            if (String.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return false;

            try
            {
                duration = GetDuration(path);
                return duration.TotalSeconds <= MaxDurationSeconds;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
