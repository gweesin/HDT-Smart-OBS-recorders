using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace RecorderPlugin
{
    internal class Recorder
    {
        private ClientWebSocket websocket;
        private Uri serverUri;
        private string Password = string.Empty;

        public class ConnectionFailedException : Exception { }
        public class AuthorizationFailedException : Exception { }

        private long NextStopTime = 0;

        private string ConnectionString = String.Empty;

        public void Connect()
        {
            try
            {
                websocket = new ClientWebSocket();
                serverUri = new Uri(ConnectionString);

                // If you have a password for WebSocket
                if (!string.IsNullOrEmpty(Password))
                {
                    // You can send an authentication message here
                    // This is a simplified example, and might need adjustments depending on OBS WebSocket protocol
                }

                websocket.ConnectAsync(serverUri, CancellationToken.None).Wait();

                if (websocket.State != WebSocketState.Open)
                {
                    throw new ConnectionFailedException();
                }
            }
            catch (Exception)
            {
                throw new ConnectionFailedException();
            }
        }

        public void UpdateSettings(string ip, string port, string password = "")
        {
            ConnectionString = $"ws://{ip}:{port}";
            Password = password;
        }

        internal void StartRecording()
        {
            if (NextStopTime > 0)
            {
                NextStopTime = 0;
                StopRecording();
            }

            // Send WebSocket command to start recording
            SendCommand("StartRecording");
        }

        internal void StopAfter(long millis)
        {
            NextStopTime = Timestamp + millis;
        }

        internal void Unload()
        {
            if (websocket != null && websocket.State == WebSocketState.Open)
            {
                websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnect", CancellationToken.None).Wait();
            }
        }

        internal void Update()
        {
            if (NextStopTime > 0 && (Timestamp > NextStopTime || Hearthstone_Deck_Tracker.API.Core.Game.IsInMenu))
            {
                NextStopTime = 0;
                StopRecording();
            }
        }

        private long Timestamp => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

        private void StopRecording()
        {
            // Send WebSocket command to stop recording
            SendCommand("StopRecording");
        }

        private void SendCommand(string command)
        {
            if (websocket != null && websocket.State == WebSocketState.Open)
            {
                var message = Encoding.UTF8.GetBytes(command);
                websocket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None).Wait();
            }
        }
    }
}
