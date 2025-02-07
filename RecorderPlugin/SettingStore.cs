using Newtonsoft.Json;
using System;
using System.IO;
using HDT = Hearthstone_Deck_Tracker;

namespace RecorderPlugin
{
    internal class SettingStore
    {
        private static string DataDir => Path.Combine(HDT.Config.Instance.DataDir, "OBSRecorder");

        private const string ConfigFile = "OBSRecorder.json";

        private static string ConfigFilePath => Path.Combine(DataDir, ConfigFile);

        public static void Save(string ip, string port, string password)
        {
            if (!Directory.Exists(DataDir))
            {
                Directory.CreateDirectory(DataDir);
            }

            Settings config = new Settings { IpAddress = ip, Port = port, Password = password };
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(ConfigFilePath, json);
        }

        public static Settings? Load()
        {
            try
            {
                return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(ConfigFilePath));
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
        }

        public struct Settings
        {
            public string IpAddress;
            public string Port;
            public string Password;
        }
    }
}
