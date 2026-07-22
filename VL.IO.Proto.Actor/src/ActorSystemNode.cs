using Proto;
using Proto.Remote.GrpcNet;
using VL.Core;
using VL.Core.Import;

[assembly: ImportType(typeof(ActorSystem), Category = "IO.Proto.Actor")]
[assembly: ImportType(typeof(ActorSystemConfig), Category = "IO.Proto.Actor")]

namespace VL.IO.Proto.Actor
{
    [ProcessNode(Name = "ActorSystem")]
    public class ActorSystemNode : IDisposable
    {
        private readonly ActorSystem _actorSystem;
        private GrpcNetRemote? _remote;

        public ActorSystemNode(NodeContext nodeContext)
        {
            _actorSystem = new ActorSystem();
        }

        private string? _host;
        private int? _port;

        public void SetConfig(string host = "127.0.0.1", int port = 0)
        {
            if (_host == host && _port == port)
                return;

            var config = GrpcNetRemoteConfig.BindTo(host, port);

            _remote?.ShutdownAsync().Wait();
            _remote = new GrpcNetRemote(_actorSystem, config);

            _host = host;
            _port = port;

            _remote?.StartAsync().Wait();
        }

        public ActorSystem Output => _actorSystem;

        public bool IsRunning => _remote?.Started ?? false;

        public void Dispose()
        {
            _remote?.ShutdownAsync().Wait();
            _actorSystem?.DisposeAsync().AsTask().Wait();
        }
    }
}
