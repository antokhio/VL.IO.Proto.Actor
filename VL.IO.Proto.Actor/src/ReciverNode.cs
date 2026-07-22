using System.Reactive.Linq;
using System.Reactive.Subjects;
using Proto;
using VL.Core.Import;

[assembly: ImportType(typeof(PID), Category = "IO.Proto")]
[assembly: ImportType(typeof(Props), Category = "IO.Proto")]

namespace VL.IO.Proto.Actor
{
    public class ObservableActor : IActor
    {
        public Subject<object?> MessageStream { get; } = new Subject<object?>();

        public Task ReceiveAsync(IContext context)
        {
            if (MessageStream.IsDisposed)
                return Task.CompletedTask;

            MessageStream?.OnNext(context.Message);

            return Task.CompletedTask;
        }
    }

    [ProcessNode(Name = "Reciver")]
    public class ReciverNode : IDisposable
    {
        const string DefaultActorName = "actor";

        private ActorSystem? _actorSystem;
        private ObservableActor _actor;
        private Props _props;

        private string _actorName = DefaultActorName;
        private PID? _pid;
        private readonly Subject<object?> _subject;

        public ReciverNode()
        {
            _actor = new ObservableActor();
            _subject = _actor.MessageStream;
            _props = Props.FromProducer(() => _actor);

            Messages = _subject.AsObservable();
        }

        public void SetActorSystem(ActorSystem? actorSystem)
        {
            if (ReferenceEquals(_actorSystem, actorSystem))
                return;

            _actorSystem = actorSystem;

            if (_actorSystem is ActorSystem)
                Invalidate();
        }

        public void SetName(string actorName = DefaultActorName)
        {
            if (_actorName == actorName)
                return;

            _actorName = actorName;

            Invalidate();
        }

        private void Invalidate()
        {
            if (_pid is PID)
                _actorSystem?.Root.Stop(_pid);

            // missing validatin

            _pid = _actorSystem?.Root.SpawnNamed(_props, _actorName);
        }

        public void Dispose()
        {
            if (_pid is PID)
                _actorSystem?.Root.Stop(_pid);
            _subject?.OnCompleted();
            _subject?.Dispose();
        }

        public IObservable<object?> Messages { get; protected set; }
        public PID? Pid => _pid;
    }
}
