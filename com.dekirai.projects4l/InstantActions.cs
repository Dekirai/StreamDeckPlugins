using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Memory;
using System.Xml.Linq;
using System.Diagnostics;
using System.Threading;
using System;
using System.Runtime.InteropServices;
using Memory;
using System.Windows.Forms;

namespace com.dekirai.projects4l
{
    [PluginActionId("com.dekirai.projects4l.instantactions")]
    public class InstantActions : PluginBase
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                return new PluginSettings();
            }

            [JsonProperty(PropertyName = "instantactions")]
            public string InstantActions { get; set; }

            [FilenameProperty]
            [JsonProperty(PropertyName = "ps4lInstantActions")]
            public string ps4lInstantActions { get; set; }
        }

        private readonly PluginSettings Settings;
        public static Mem mem = new Mem();
        public static string process = "S4Client";

        #region Private Members

        private PluginSettings settings;

        #endregion
        public InstantActions(SDConnection connection, InitialPayload payload) : base(connection, payload)
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
            string processName = "S4Client";
            Process[] processes = Process.GetProcessesByName(processName);
            Process s4process = processes[0];
            if (processes.Length == 0)
            {
                Connection.ShowAlert();
                return;
            }
            IntPtr mainWindowHandle = s4process.MainWindowHandle;
            SetForegroundWindow(mainWindowHandle);
            Thread.Sleep(300);
            SendKeys.SendWait("{Enter}");
            Thread.Sleep(100);
            mem.WriteMemory($"{process}.exe+017293C0,0xAC,0x2C,0x18,0x21C,0x0", "string", $"{Settings.InstantActions}\0");
            Thread.Sleep(100);
            SendKeys.SendWait("{Enter}");
            SendKeys.SendWait("{Enter}");
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