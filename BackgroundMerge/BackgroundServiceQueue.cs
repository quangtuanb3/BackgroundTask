using System.Threading.Channels;
namespace FileMergeLibrary

{
    public class BackgroundServiceQueue : IBackgroundServiceQueue
    {
        private readonly Channel<MergeJob> _queue;

        public BackgroundServiceQueue()
        {
            // Initialize the channel with a bounded capacity
            _queue = Channel.CreateBounded<MergeJob>(new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = true,
            });
        }

        public async Task QueueMergeJob(MergeJob mergeJob)
        {
            // Write the merge job to the channel
            await _queue.Writer.WriteAsync(mergeJob);
        }

        public async ValueTask<MergeJob> DequeueAsync(CancellationToken cancellationToken)
        {
            // Read the merge job from the channel
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
