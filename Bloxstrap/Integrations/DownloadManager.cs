using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bloxstrap.Integrations
{
    public class DownloadManager
    {
        public event EventHandler<DownloadProgressEventArgs>? ProgressChanged;

        public string SpeedText => "";
        public string ETAText => "";

        public async Task<bool> DownloadFile(string url, string destinationPath, string? expectedHash, CancellationToken token)
        {
            const string LOG_IDENT = "DownloadManager::DownloadFile";
            const int MaxTries = 5;

            var buffer = new byte[4096];

            for (int attempt = 1; attempt <= MaxTries; attempt++)
            {
                if (token.IsCancellationRequested)
                    return false;

                int totalBytesRead = 0;

                try
                {
                    var response = await App.HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);
                    await using var stream = await response.Content.ReadAsStreamAsync(token);
                    await using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete);

                    while (true)
                    {
                        if (token.IsCancellationRequested)
                        {
                            stream.Close();
                            fileStream.Close();
                            return false;
                        }

                        int bytesRead = await stream.ReadAsync(buffer, token);

                        if (bytesRead == 0)
                            break;

                        totalBytesRead += bytesRead;

                        await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), token);

                        ProgressChanged?.Invoke(this, new DownloadProgressEventArgs
                        {
                            DownloadedBytes = totalBytesRead,
                        });
                    }

                    if (!string.IsNullOrEmpty(expectedHash))
                    {
                        fileStream.Seek(0, SeekOrigin.Begin);
                        string hash = MD5Hash.FromStream(fileStream);

                        if (hash != expectedHash)
                        {
                            App.Logger.WriteLine(LOG_IDENT, $"Hash mismatch for {Path.GetFileName(destinationPath)} ({hash} != {expectedHash})");
                            fileStream.Close();
                            File.Delete(destinationPath);

                            if (attempt >= MaxTries)
                                return false;

                            continue;
                        }
                    }

                    App.Logger.WriteLine(LOG_IDENT, $"Downloaded {Path.GetFileName(destinationPath)} ({totalBytesRead} bytes)");
                    return true;
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Download failed ({Path.GetFileName(destinationPath)}), attempt {attempt}/{MaxTries}");
                    App.Logger.WriteException(LOG_IDENT, ex);

                    if (File.Exists(destinationPath))
                        File.Delete(destinationPath);

                    if (attempt >= MaxTries)
                        throw;

                    if (ex is IOException && !url.StartsWith("http://"))
                    {
                        App.Logger.WriteLine(LOG_IDENT, $"Download failed for {url}, retrying...");
                    }

                    await Task.Delay(1000 * attempt, token);
                }
            }

            return false;
        }

        public void Reset() { }
    }

    public class DownloadProgressEventArgs : EventArgs
    {
        public long DownloadedBytes { get; set; }
    }
}
