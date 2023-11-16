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
    [PluginActionId("com.dekirai.kingdomheartsii.refills")]
    public class Refills : PluginBase
    {
        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                return new PluginSettings();
            }

            [JsonProperty(PropertyName = "refills")]
            public string Refills { get; set; }

            [FilenameProperty]
            [JsonProperty(PropertyName = "khiiRefills")]
            public string khiiRefills { get; set; }
        }

        private readonly PluginSettings Settings;
        public static Mem mem = new Mem();
        public static string process = "KINGDOM HEARTS II FINAL MIX";

        #region Private Members

        private PluginSettings settings;

        #endregion
        public Refills(SDConnection connection, InitialPayload payload) : base(connection, payload)
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
            switch (Settings.Refills)
            {
                case "0":
                    mem.WriteMemory($"{process}.exe+2A20C98", "int", $"{mem.ReadInt($"{process}.exe+2A20C9C")}");
                    break;
                case "1":
                    mem.WriteMemory($"{process}.exe+2A20E18", "int", $"{mem.ReadInt($"{process}.exe+2A20E1C")}");
                    mem.WriteMemory($"{process}.exe+2A20E54", "float", "0");
                    break;
                case "2":
                    mem.WriteMemory($"{process}.exe+2A20E49", "byte", $"{mem.ReadByte($"{process}.exe+2A20E4A")}");
                    mem.WriteMemory($"{process}.exe+2A20E48", "byte", "0x63");
                    break;
                case "3":
                    mem.WriteMemory($"{process}.exe+2A20E4C", "float", "6000");
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