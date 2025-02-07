using System;
using System.Reflection;
using System.Timers;
using System.Windows.Media;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Plugins;
using ToastManager = Hearthstone_Deck_Tracker.Utility.Toasts.ToastManager;
using Hearthstone_Deck_Tracker.Utility.Logging;

namespace RecorderPlugin
{
    public class RecorderPlugin : IPlugin
    {
        private const int DelayAfterGameEndSeconds = 10;

        // setting idle threshold
        private const int IdleTimeThresholdSeconds = 5;
        private readonly Recorder _recorder = new Recorder();
        private readonly SettingsDialog _settingsDialog;
        private readonly SettingStore _settingStore = new SettingStore();
        private readonly Timer _idleTimer;

        public string ButtonText => "Settings";
        public string Name => "Hearthstone Smart OBS recorder";
        public string Author => "gweesin";

        public string Description =>
            "Starts recording in OBS when HS game begins and stops when the game ends. To use this you will need to install OBS and the obs-websocket plugin.";

        public Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public System.Windows.Controls.MenuItem MenuItem => null;

        public RecorderPlugin()
        {
            if (SettingStore.Load() is SettingStore.Settings settings)
            {
                _recorder.UpdateSettings(settings.IpAddress, settings.Port, settings.Password);
                _settingsDialog = new SettingsDialog(settings.IpAddress, settings.Port, settings.Password);
            }
            else
            {
                _settingsDialog = new SettingsDialog("127.0.0.1", "4444", "");
                _recorder.UpdateSettings("127.0.0.1", "4444", "");
            }

            _settingsDialog.SettingsChanged += OnSettingsChanged;

            _idleTimer = new Timer(IdleTimeThresholdSeconds * 1000);
            _idleTimer.Elapsed += OnIdleTimeElapsed;
            _idleTimer.AutoReset = false;
        }

        private void OnSettingsChanged(object _, SettingsDialog.SettingsChangedEvent e)
        {
            _recorder.UpdateSettings(e.IPAddress, e.Port, e.Password);

            if (!Connect())
            {
                return;
            }

            SettingStore.Save(e.IPAddress, e.Port, e.Password);
            _settingsDialog.Close();
        }

        private bool Connect()
        {
            try
            {
                _recorder.Connect();
            }
            catch (Recorder.ConnectionFailedException ex)
            {
                Log.Error(ex.Message);
                ToastManager.ShowCustomToast(Toasts.MakeErrorToast("Connection to OBS failed!"));
                return false;
            }
            catch (Recorder.AuthorizationFailedException ex)
            {
                Log.Error(ex.Message);
                ToastManager.ShowCustomToast(Toasts.MakeErrorToast("Invalid password!"));
                return false;
            }

            ToastManager.ShowCustomToast(Toasts.MakeSuccessToast("Connected to OBS!"));

            return true;
        }

        public void OnLoad()
        {
            GameEvents.OnGameEnd.Add(() => _recorder.StopAfter(DelayAfterGameEndSeconds));
            GameEvents.OnGameStart.Add(_recorder.StartRecording);
            GameEvents.OnPlayerPlay.Add((card) => OnPlayerAction());
            GameEvents.OnPlayerDraw.Add((card) => OnPlayerAction());
            GameEvents.OnPlayerHandDiscard.Add((card) => OnPlayerAction());
            GameEvents.OnPlayerDeckDiscard.Add((card) => OnPlayerAction());
            GameEvents.OnPlayerPlayToDeck.Add((card) => OnPlayerAction());
            GameEvents.OnPlayerPlayToHand.Add((card) => OnPlayerAction());
            GameEvents.OnPlayerPlayToGraveyard.Add((card) => OnPlayerAction());
            GameEvents.OnPlayerCreateInDeck.Add((card) => OnPlayerAction());
            GameEvents.OnPlayerCreateInPlay.Add((card) => OnPlayerAction());
            GameEvents.OnPlayerJoustReveal.Add((card) => OnPlayerAction());
            GameEvents.OnPlayerDeckToPlay.Add((card) => OnPlayerAction());
            GameEvents.OnPlayerHeroPower.Add(OnPlayerAction);
            GameEvents.OnPlayerFatigue.Add((fatigue) => OnPlayerAction());
            GameEvents.OnPlayerMinionMouseOver.Add((card) => OnPlayerAction());
            GameEvents.OnPlayerHandMouseOver.Add((card) => OnPlayerAction());
            GameEvents.OnPlayerMinionAttack.Add((attackInfo) => OnPlayerAction());

            GameEvents.OnOpponentPlay.Add((card) => OnPlayerAction());
            GameEvents.OnOpponentDraw.Add(OnPlayerAction);
            GameEvents.OnOpponentHandDiscard.Add((card) => OnPlayerAction());
            GameEvents.OnOpponentDeckDiscard.Add((card) => OnPlayerAction());
            GameEvents.OnOpponentPlayToDeck.Add((card) => OnPlayerAction());
            GameEvents.OnOpponentHandToDeck.Add((card) => OnPlayerAction());
            GameEvents.OnOpponentPlayToHand.Add((card) => OnPlayerAction());
            GameEvents.OnOpponentPlayToGraveyard.Add((card) => OnPlayerAction());
            GameEvents.OnOpponentSecretTriggered.Add((card) => OnPlayerAction());
            GameEvents.OnOpponentCreateInDeck.Add((card) => OnPlayerAction());
            GameEvents.OnOpponentCreateInPlay.Add((card) => OnPlayerAction());
            GameEvents.OnOpponentJoustReveal.Add((card) => OnPlayerAction());
            GameEvents.OnOpponentDeckToPlay.Add((card) => OnPlayerAction());
            GameEvents.OnOpponentHeroPower.Add(OnPlayerAction);
            GameEvents.OnOpponentFatigue.Add((fatigue) => OnPlayerAction());
            GameEvents.OnOpponentMinionMouseOver.Add((card) => OnPlayerAction());
            GameEvents.OnOpponentMinionAttack.Add((attackInfo) => OnPlayerAction());

            Connect();
        }

        public void OnUnload()
        {
            _recorder.Unload();
        }

        public void OnUpdate()
        {
            _recorder.Update();
        }

        public void OnButtonPress()
        {
            if (!_settingsDialog.Visible)
            {
                _settingsDialog.ShowDialog();
            }

            _settingsDialog.Focus();
        }

        private void OnPlayerAction()
        {
            if (_idleTimer.Enabled)
            {
                _idleTimer.Stop();
            }

            if (_recorder.IsRecordingPaused)
            {
                Log.Info("Resuming recording due to player action.");
                _recorder.ResumeRecord();
            }

            Log.Info("Player action detected, restarting idle timer.");
            _idleTimer.Start();
        }

        private void OnIdleTimeElapsed(object sender, ElapsedEventArgs e)
        {
            Log.Info("Idle time threshold reached, pausing recording.");
            _recorder.PauseRecord();
        }

        private static class Toasts
        {
            public static System.Windows.Controls.UserControl MakeSuccessToast(string message)
            {
                var content = new System.Windows.Controls.UserControl();
                content.Background = (Brush)(new BrushConverter()).ConvertFromString("#00a124");
                content.Content = message;
                content.Foreground = Brushes.White;
                content.FontSize = 36;
                content.FontFamily = new FontFamily("Arial");
                content.Padding = new System.Windows.Thickness(12);
                return content;
            }

            public static System.Windows.Controls.UserControl MakeErrorToast(string error)
            {
                var content = new System.Windows.Controls.UserControl
                {
                    Background = (Brush)(new BrushConverter()).ConvertFromString("#a20010"),
                    Content = error,
                    Foreground = Brushes.White,
                    FontSize = 36,
                    FontFamily = new FontFamily("Arial"),
                    Padding = new System.Windows.Thickness(12)
                };
                return content;
            }
        }
    }
}