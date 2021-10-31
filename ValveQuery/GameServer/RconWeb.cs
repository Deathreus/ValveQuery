using System;
using System.Threading.Tasks;
using WebSocket4Net;

namespace ValveQuery.GameServer
{
    class RconWeb : Rcon
    {
        private const string NotSupportedMessage = "Method is not supported in WebRcon. Use SendCommand to execute some command.";

        private readonly WebSocket WebSocket;
        private readonly object LockObject = new object();

        internal RconWeb(string connectionString, ConnectionInfo conInfo)
            : base(conInfo)
        {
            WebSocket = new WebSocket(connectionString);
            WebSocket.Open();
        }

        internal new static Rcon Authorize(ConnectionInfo conInfo, string msg)
        {
            var rcon = new RconWeb($"ws://{conInfo.EndPoint.Address}:{conInfo.EndPoint.Port}/{msg}", conInfo);

            if (rcon.WebSocket.State == WebSocketState.Open)
            {
                return rcon;
            }

            rcon.Dispose();

            return null;
        }

        public override string SendCommand(string cmd, bool isMultiPacketResponse = false)
        {
            lock (LockObject)
            {
                return SendCommandAsync(cmd).GetAwaiter().GetResult();
            }
        }

        public override void AddlogAddress(string ip, ushort port)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        public override void RemovelogAddress(string ip, ushort port)
        {
            throw new NotSupportedException(NotSupportedMessage);
        }

        private async Task<string> SendCommandAsync(string message)
        {
            var task = new TaskCompletionSource<byte[]>();
            void EventHandler(object sender, DataReceivedEventArgs args) => task.TrySetResult(args.Data);

            try
            {
                WebSocket.DataReceived += EventHandler;

                WebSocket.Send(message);

                return BitConverter.ToString(await task.Task);
            }
            finally
            {
                WebSocket.DataReceived -= EventHandler;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            WebSocket.Close();
            base.Dispose(disposing);
        }
    }
}
