using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Diagnostics;
using System.Threading;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace com.dekirai.xero
{
    [PluginActionId("com.dekirai.xero.functions")]
    public class Functions : PluginBase
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                return new PluginSettings();
            }

            [JsonProperty(PropertyName = "functions")]
            public string Functions { get; set; }

            [FilenameProperty]
            [JsonProperty(PropertyName = "xeroFunctions")]
            public string XeroFunctions { get; set; }
        }

        private readonly PluginSettings Settings;

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
            string processName = "xerogame";
            Process[] processes = Process.GetProcessesByName(processName);
            Process xeroGameProcess = processes[0];
            IntPtr mainWindowHandle = xeroGameProcess.MainWindowHandle;
            SetForegroundWindow(mainWindowHandle);
            Thread.Sleep(450);
            switch (Settings.Functions)
            {
                case "0":
                    // Simulate key presses
                    SendKeys.SendWait("{F11}");
                    SendKeys.SendWait("toggle_pvp");
                    SendKeys.SendWait("{Enter}");
                    SendKeys.SendWait("{F11}");
                    break;
                case "1":
                    // Simulate key presses
                    SendKeys.SendWait("{F11}");
                    SendKeys.SendWait("pos");
                    SendKeys.SendWait("{Enter}");
                    SendKeys.SendWait("{F11}");
                    break;
                case "2":
                    // Simulate key presses
                    SendKeys.SendWait("{F11}");
                    SendKeys.SendWait("tp");
                    SendKeys.SendWait("{Enter}");
                    SendKeys.SendWait("{F11}");
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

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        #region Private Methods

        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }

        #endregion
    }
}