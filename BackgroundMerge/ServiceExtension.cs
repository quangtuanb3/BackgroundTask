using Microsoft.Extensions.DependencyInjection;
namespace FileMergeLibrary
{
    public static class ServiceExtension
    {
        public static void AddFileMergeService(this IServiceCollection services)
        {
            var applicationAssembly = typeof(ServiceExtension).Assembly;

            services.AddSingleton<IBackgroundServiceQueue, BackgroundServiceQueue>();
            services.AddHostedService<FileMergeService>();

        }
    }
}
