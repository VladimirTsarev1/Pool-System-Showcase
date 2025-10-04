using System.Threading;
using System.Threading.Tasks;

namespace Showcase.Scripts.Providers
{
    public static class ConfigProvider
    {
        public static PoolConfigProvider Pools { get; private set; }
        public static ColorConfigProvider Colors { get; private set; }
        public static AudioConfigProvider Audio { get; private set; }
        public static SceneConfigProvider Scene { get; private set; }

        private static Task _initTask;

        public static Task InitAsync(CancellationToken cancellationToken)
        {
            return _initTask ??= InitInternalAsync(cancellationToken);
        }

        private static async Task InitInternalAsync(CancellationToken cancellationToken)
        {
            Pools = new PoolConfigProvider();
            Colors = new ColorConfigProvider();
            Audio = new AudioConfigProvider();
            Scene = new SceneConfigProvider();

            var tasks = new Task[]
            {
                Pools.InitAsync(cancellationToken),
                Colors.InitAsync(cancellationToken),
                Audio.InitAsync(cancellationToken),
                Scene.InitAsync(cancellationToken),
            };

            await Task.WhenAll(tasks);
        }
    }
}