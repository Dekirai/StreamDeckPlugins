using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Memory;
using System.Xml.Linq;
using System.Diagnostics;
using System.Threading;
using System;

namespace com.dekirai.kingdomheartsii
{
    [PluginActionId("com.dekirai.kingdomheartsii.forms")]
    public class Forms : PluginBase
    {
        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                return new PluginSettings();
            }

            [JsonProperty(PropertyName = "forms")]
            public string Forms { get; set; }

            [FilenameProperty]
            [JsonProperty(PropertyName = "khiiForms")]
            public string khiiForms { get; set; }
        }

        private readonly PluginSettings Settings;
        public static Mem mem = new Mem();
        public static string process = "KINGDOM HEARTS II FINAL MIX";

        #region Private Members

        private PluginSettings settings;

        #endregion
        public Forms(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                Settings = PluginSettings.CreateDefaultSettings();
                Connection.SetSettingsAsync(JObject.FromObject(Settings));
            }
            else
            {
                Settings = payload.Settings.ToObject<PluginSettings>();
            }
        }

        public override void Dispose()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Destructor called");
        }

        public override void KeyPressed(KeyPayload payload)
        {
            GetPID();
            int _isForm = mem.ReadByte($"{process}.exe+9AA5D4");
            if (_isForm > 0)
            {
                Connection.ShowAlert();
                return;
            }
            mem.WriteMemory($"{process}.exe+2A5A096", "bytes", $"{Settings.Forms}");
            Thread.Sleep(250);
            mem.WriteMemory($"{process}.exe+2A5A096", "bytes", "0x00 0x00 0x00 0x00");
        }

        public override void KeyReleased(KeyPayload payload) { }

        public override void OnTick() { }

        public async override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            // New in StreamDeck-Tools v2.0:
            Tools.AutoPopulateSettings(Settings, payload.Settings);

            // Return fixed filename back to the Property Inspector
            await Connection.SetSettingsAsync(JObject.FromObject(Settings)).ConfigureAwait(false);
        }

        private void GetPID()
        {
            int pid = mem.GetProcIdFromName(process);
            bool openProc = false;

            if (pid > 0) openProc = mem.OpenProcess(pid);
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Got PID: {pid}");
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        #region Private Methods

        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }

        #endregion
    }
}