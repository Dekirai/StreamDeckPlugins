using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Memory;

namespace com.dekirai.pangya
{
    [PluginActionId("com.dekirai.pangya.functions")]
    public class Functions : PluginBase
    {
        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                return new PluginSettings();
            }

            [JsonProperty(PropertyName = "functions")]
            public string Functions { get; set; }

            [FilenameProperty]
            [JsonProperty(PropertyName = "pangyaCheats")]
            public string PangyaCheats { get; set; }
        }

        private readonly PluginSettings Settings;
        public static Mem mem = new Mem();

        #region Private Members

        private PluginSettings settings;

        #endregion
        public Functions(SDConnection connection, InitialPayload payload) : base(connection, payload)
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
            if (Settings.Functions == "NoSlope")
            {                
                mem.WriteMemory("ProjectG.exe+00B006E8,0x8,0x10,0x30,0x0,0x21C,0x1C", "float", "0");
                mem.WriteMemory("ProjectG.exe+00B006E8,0x8,0x10,0x30,0x0,0x21C,0x24", "float", "0");
            }
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
            int pid = mem.GetProcIdFromName("ProjectG");
            bool openProc = false;

            if (pid > 0) openProc = mem.OpenProcess(pid);
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