namespace FileMergeLibrary
{
    public class MergeJob
    {
        public string UserId { get; set; } = default!;
        public string SessionId { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public int TotalChunks { get; set; }
        public string ChunkFolder { get; set; } = default!;
        public string ConnectionId { get; set; } = default!;
    }
}
