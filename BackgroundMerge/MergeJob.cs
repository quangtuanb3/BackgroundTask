namespace FileMergeLibrary
{
    public class MergeJob
    {
        public string FileName { get; set; } = default!;
        public int TotalChunks { get; set; }
        public string ChunkFolder { get; set; } = default!;
        public string ConnectionId { get; set; } = default!;
    }
}
