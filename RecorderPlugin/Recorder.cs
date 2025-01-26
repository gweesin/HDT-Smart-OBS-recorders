using OBSWebsocketDotNet;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RecorderPlugin
{
    internal class Recorder
    {
        public class ConnectionFailedException : Exception
        {
        }

        public class AuthorizationFailedException : Exception
        {
        }

        OBSWebsocket OBS = new OBSWebsocket();

        private long NextStopTime = 0;

        private string ConnectionString = String.Empty;
        private string Password = String.Empty;

        public async void Connect()
        {
            try
            {
                OBS.ConnectAsync(ConnectionString, Password);
            }
            catch (AuthFailureException)
            {
                throw new AuthorizationFailedException();
            }

            await Task.Delay(3000); // wait 3 sec to promise success because OBS.ConnectAsync is an async function but not awaitable
            if (!OBS.IsConnected)
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

            if (OBS.IsConnected)
            {
                Debug.WriteLine("About to start recording.");
                OBS.StartRecord();
                Debug.WriteLine("Recording started.");
            }
        }

        internal void StopAfter(long millis)
        {
            NextStopTime = Timestamp + millis;
        }

        internal void Unload()
        {
            OBS.Disconnect();
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
            if (OBS.IsConnected && OBS.GetRecordStatus().IsRecording)
            {
                Debug.WriteLine("About to stop recording.");
                OBS.StopRecord();
                Debug.WriteLine("Recording stopped.");
            }
        }
    }
}