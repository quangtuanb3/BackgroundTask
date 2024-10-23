
using MergeWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FileMergeLibrary;

namespace MergeWeb.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly IBackgroundServiceQueue _backgroundServiceQueue;

        public FileUploadController(IBackgroundServiceQueue backgroundServiceQueue)
        {
            _backgroundServiceQueue = backgroundServiceQueue;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("UploadUsingCronJob")]
        public IActionResult UploadUsingCronJob()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadChunk(IFormFile fileChunk, string fileName, int chunkIndex, int totalChunks)
        {
            var connectionId = Request.Headers["X-Connection-Id"].ToString();
            var chunkFolder = Path.Combine("wwwroot", "uploads", "chunks");
            Directory.CreateDirectory(chunkFolder);

            if (fileChunk == null || string.IsNullOrEmpty(fileName) || totalChunks <= 0)
            {
                return BadRequest("Invalid request data.");
            }

            var chunkFilePath = Path.Combine(chunkFolder, $"{fileName}.part{chunkIndex}");
            using (var stream = new FileStream(chunkFilePath, FileMode.Create))
            {
                await fileChunk.CopyToAsync(stream);
            }

            // Check if all chunks are uploaded, then queue for merging
            if (chunkIndex == totalChunks - 1)
            {

                // Create the merge job
                var mergeJob = new MergeJob
                {
                    FileName = fileName,
                    TotalChunks = totalChunks,
                    ChunkFolder = chunkFolder,
                    ConnectionId = connectionId
                };

                // Queue the job for the background service
                await _backgroundServiceQueue.QueueMergeJob(mergeJob);
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadChunkToCronjob(IFormFile fileChunk, string fileName, int chunkIndex, int totalChunks)
        {
            var connectionId = Request.Headers["X-Connection-Id"].ToString();
            var chunkFolder = Path.Combine("wwwroot", "uploads", "chunks", fileName);
            Directory.CreateDirectory(chunkFolder);

            if (fileChunk == null || string.IsNullOrEmpty(fileName) || totalChunks <= 0)
            {
                return BadRequest("Invalid request data.");
            }

            var chunkFilePath = Path.Combine(chunkFolder, $"{fileName}.part{chunkIndex}");
            using (var stream = new FileStream(chunkFilePath, FileMode.Create))
            {
                await fileChunk.CopyToAsync(stream);
            }

            var metadataFilePath = Path.Combine(chunkFolder, "metadata.json");
            var metadata = new
            {
                FileName = fileName,
                TotalChunks = totalChunks,
                UploadDate = DateTime.Now,
            };

            var metadataJson = System.Text.Json.JsonSerializer.Serialize(metadata);
            await System.IO.File.WriteAllTextAsync(metadataFilePath, metadataJson);

            return Ok();
        }

    }

}
