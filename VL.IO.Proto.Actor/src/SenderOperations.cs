using Proto;

namespace VL.IO.Proto.Actor
{
    public static class SenderOperations
    {
        /// <summary>
        /// Sends a fire-and-forget message to a Target PID.
        /// </summary>
        public static void SendMessage(ActorSystem system, PID target, object message)
        {
            if (system == null || target == null || message == null)
                return;

            system.Root.Send(target, message);
        }

        /// <summary>
        /// Helper to construct a remote PID from an IP string.
        /// </summary>
        public static PID CreateRemotePid(string address, string actorName)
        {
            return PID.FromAddress(address, actorName);
        }
    }
}
