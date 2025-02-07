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

        private readonly OBSWebsocket _obs = new OBSWebsocket();

        private long _nextStopTime = 0;

        private string _connectionString = String.Empty;
        private string _password = String.Empty;

        public async void Connect()
        {
            try
            {
                _obs.ConnectAsync(_connectionString, _password);
            }
            catch (AuthFailureException)
            {
                throw new AuthorizationFailedException();
            }

            await Task.Delay(
                3000); // wait 3 sec to promise success because OBS.ConnectAsync is an async function but not awaitable
            if (!_obs.IsConnected)
            {
                throw new ConnectionFailedException();
            }
        }

        public void UpdateSettings(string ip, string port, string password = "")
        {
            _connectionString = $"ws://{ip}:{port}";
            _password = password;
        }

        internal void StartRecording()
        {
            if (_nextStopTime > 0)
            {
                _nextStopTime = 0;
                StopRecording();
            }

            if (_obs.IsConnected)
            {
                Debug.WriteLine("About to start recording.");
                _obs.StartRecord();
                Debug.WriteLine("Recording started.");
            }
        }

        internal void StopAfter(long millis)
        {
            _nextStopTime = Timestamp + millis;
        }

        internal void Unload()
        {
            _obs.Disconnect();
        }

        internal void Update()
        {
            if (_nextStopTime > 0 && (Timestamp > _nextStopTime || Hearthstone_Deck_Tracker.API.Core.Game.IsInMenu))
            {
                _nextStopTime = 0;
                StopRecording();
            }
        }

        internal void PauseRecord()
        {
            if (_obs.IsConnected && _obs.GetRecordStatus().IsRecording)
            {
                Debug.WriteLine("About to pause recording.");
                _obs.PauseRecord();
                Debug.WriteLine("Recording paused.");
            }
        }

        internal Boolean IsRecordingPaused => _obs.GetRecordStatus().IsRecordingPaused;

        internal void ResumeRecord()
        {
            if (!_obs.IsConnected || !_obs.GetRecordStatus().IsRecordingPaused)
            {
                return;
            }

            Debug.WriteLine("About to resume recording.");
            _obs.ResumeRecord();
            Debug.WriteLine("Recording resumed.");
        }

        private long Timestamp => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

        private void StopRecording()
        {
            if (_obs.IsConnected && _obs.GetRecordStatus().IsRecording)
            {
                Debug.WriteLine("About to stop recording.");
                _obs.StopRecord();
                Debug.WriteLine("Recording stopped.");
            }
        }
    }
}