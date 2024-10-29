using Microsoft.Identity.Client;

namespace MergeWeb.Entities;

public class FIleUploadTracker
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public int TotalChunk { get; set; }
    public int CurrentChunk { get; set; }
    public string Status { get; set; }
    public bool IsComplete { get; set; }
    public string TempPath { get; set; }
    public string Path { get; set; }


}
