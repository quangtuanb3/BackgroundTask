
namespace FileMergeLibrary
{
    public interface IBackgroundServiceQueue
    {
        Task QueueMergeJob(MergeJob mergeJob);
        ValueTask<MergeJob> DequeueAsync(CancellationToken cancellationToken);
    }
}
