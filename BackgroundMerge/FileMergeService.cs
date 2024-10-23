
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FileMergeLibrary
{
    public class FileMergeService : BackgroundService
    {
        private readonly IBackgroundServiceQueue _queue;
        private readonly ILogger<FileMergeService> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        public FileMergeService(IBackgroundServiceQueue queue, ILogger<FileMergeService> logger, IHostApplicationLifetime hostApplicationLifetime)
        {
            _queue = queue;
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            //_hostApplicationLifetime.ApplicationStarted.WaitHandle.WaitOne();

            _logger.LogInformation("Application started. Background service is now running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {


                    var mergeJob = await _queue.DequeueAsync(stoppingToken);
                    _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Processing merge job for file: {mergeJob.FileName}");
                    var connection = new HubConnectionBuilder()
                 .WithUrl("https://localhost:7259/uploadhub")
                 .Build();
                    await connection.StartAsync(stoppingToken);
                    _logger.LogInformation("Starting delay...");
                    await Task.Delay(5000, stoppingToken);
                    _logger.LogInformation("Delay completed.");

                    await MergeChunks(mergeJob, stoppingToken);

                    _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Completed");

                    await connection.InvokeAsync("SendMessage", "Merge done");


                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Task was cancelled");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing the merge job.");
                }
            }
        }


        private async Task MergeChunks(MergeJob mergeJob, CancellationToken stoppingToken)
        {
            var parentFolder = Directory.GetParent(mergeJob.ChunkFolder).FullName;

            // Generate a unique file name by appending a GUID
            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(mergeJob.FileName)}_{Guid.NewGuid()}{Path.GetExtension(mergeJob.FileName)}";

            var finalFilePath = Path.Combine(parentFolder, uniqueFileName);

            var chunkedFiles = Directory.EnumerateFiles(mergeJob.ChunkFolder, $"{mergeJob.FileName}.part*");

            using (var finalFileStream = new FileStream(finalFilePath, FileMode.Create))
            {
                foreach (var chunkFile in chunkedFiles.OrderBy(f => f))
                {
                    using (var chunkStream = new FileStream(chunkFile, FileMode.Open))
                    {
                        await chunkStream.CopyToAsync(finalFileStream, 81920, stoppingToken);
                    }
                    File.Delete(chunkFile); // Delete chunk after merging
                }
            }

            _logger.LogInformation($"Merged file saved as: {uniqueFileName}");
        }



    }

}
