using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Memory;
using System.Xml.Linq;

namespace com.dekirai.pangya
{
    [PluginActionId("com.dekirai.pangya.cheats")]
    public class Cheats : PluginBase
    {
        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                return new PluginSettings();
            }

            [JsonProperty(PropertyName = "cheats")]
            public string Cheats { get; set; }

            [FilenameProperty]
            [JsonProperty(PropertyName = "pangyaCheats")]
            public string PangyaCheats { get; set; }
        }

        private readonly PluginSettings Settings;
        public static Mem mem = new Mem();

        #region Private Members

        private PluginSettings settings;

        #endregion
        public Cheats(SDConnection connection, InitialPayload payload) : base(connection, payload)
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
            switch (Settings.Cheats)
            {
                case "0": // Always Pangya ON
                    mem.WriteMemory("ProjectG.exe+43530", "bytes", $"0x00 0x89 0x88 0xE4 0x00 0x00 0x00 0x8B 0x15 0x20");
                    break;
                case "1": // Always Pangya OFF
                    mem.WriteMemory("ProjectG.exe+43530", "bytes", $"0x00 0x89 0x88 0xD4 0x00 0x00 0x00 0x8B 0x15 0x20");
                    break;
                case "2": // Skip Shot
                    mem.WriteMemory("ProjectG.exe+670558", "byte", $"0x01");
                    break;
                case "3": // Room Crasher
                    mem.WriteMemory("ProjectG.exe+69B108", "byte", $"0xFF");
                    break;
                case "4": // Teleport Ball to Hole
                    float holeX = mem.ReadFloat("ProjectG.exe+71F33C");
                    float holeY = mem.ReadFloat("ProjectG.exe+71F340");
                    float holeZ = mem.ReadFloat("ProjectG.exe+71F344");
                    mem.WriteMemory("ProjectG.exe+670540", "float", $"{holeX}");
                    mem.WriteMemory("ProjectG.exe+670544", "float", $"{holeY}");
                    mem.WriteMemory("ProjectG.exe+670548", "float", $"{holeZ}");
                    break;
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