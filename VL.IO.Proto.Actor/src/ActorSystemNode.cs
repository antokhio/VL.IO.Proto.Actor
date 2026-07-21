using Proto;
using Proto.Remote;
using Proto.Remote.GrpcNet;
using VL.Core;
using VL.Core.Import;

namespace VL.IO.Proto.Actor
{
    [ProcessNode]
    public class ActorSystemNode : IDisposable
    {
        private readonly ActorSystem _actorSystem;
        private GrpcNetRemote? _remote;

        public ActorSystemNode(NodeContext nodeContext)
        {
            Log.SetLoggerFactory(new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory());
            _actorSystem = new ActorSystem();
        }

        private string? _host;
        private int? _port;

        public void SetConfig(string host = "127.0.0.1", int port = 0)
        {
            if (_host == host && _port == port)
                return;

            var config = RemoteConfig.BindTo(host, port);

            _remote?.ShutdownAsync().Wait();
            _remote = new GrpcNetRemote(_actorSystem, config);

            _host = host;
            _port = port;

            _remote?.StartAsync().Wait();
        }

        public bool IsRunning => _remote?.Started ?? false;

        public ActorSystem Output => _actorSystem;

        public void Dispose()
        {
            _remote?.ShutdownAsync().Wait();
            _actorSystem?.DisposeAsync().AsTask().Wait();
        }
    }
}
