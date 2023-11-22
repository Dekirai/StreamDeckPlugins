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
    [PluginActionId("com.dekirai.projects4l.presets")]
    public class Presets : PluginBase
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                return new PluginSettings();
            }

            [JsonProperty(PropertyName = "preset")]
            public string Preset { get; set; }
            [JsonProperty(PropertyName = "nickname")]
            public string Nickname { get; set; }

            [FilenameProperty]
            [JsonProperty(PropertyName = "ps4lPreset")]
            public string ps4lPreset { get; set; }
        }

        private readonly PluginSettings Settings;
        public static Mem mem = new Mem();
        public static string process = "S4Client";

        #region Private Members

        private PluginSettings settings;

        #endregion
        public Presets(SDConnection connection, InitialPayload payload) : base(connection, payload)
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
            switch (Settings.Preset)
            {
                case "0":
                    //GM Hair
                    mem.WriteMemory($"{process}.exe+017293C0,0xAC,0x2C,0x18,0x21C,0x0", "string", $"To Server : gm additem {Settings.Nickname} 1000000 0\0");
                    SendKeys.SendWait("{Enter}");
                    Thread.Sleep(200);
                    //GM Suit
                    mem.WriteMemory($"{process}.exe+017293C0,0xAC,0x2C,0x18,0x21C,0x0", "string", $"To Server : gm additem {Settings.Nickname} 1020000 0\0");
                    SendKeys.SendWait("{Enter}");
                    Thread.Sleep(200);
                    //GM Leg
                    mem.WriteMemory($"{process}.exe+017293C0,0xAC,0x2C,0x18,0x21C,0x0", "string", $"To Server : gm additem {Settings.Nickname} 1030000 0\0");
                    SendKeys.SendWait("{Enter}");
                    Thread.Sleep(200);
                    //GM Gloves
                    mem.WriteMemory($"{process}.exe+017293C0,0xAC,0x2C,0x18,0x21C,0x0", "string", $"To Server : gm additem {Settings.Nickname} 1040040 0\0");
                    SendKeys.SendWait("{Enter}");
                    SendKeys.SendWait("{Enter}");
                    break;
                    case "1":
                    //Plasma Sword
                    mem.WriteMemory($"{process}.exe+017293C0,0xAC,0x2C,0x18,0x21C,0x0", "string", $"To Server : gm additem {Settings.Nickname} 2000001 7\0");
                    SendKeys.SendWait("{Enter}");
                    Thread.Sleep(200);
                    //Counter Sword
                    mem.WriteMemory($"{process}.exe+017293C0,0xAC,0x2C,0x18,0x21C,0x0", "string", $"To Server : gm additem {Settings.Nickname} 2000002 3\0");
                    SendKeys.SendWait("{Enter}");
                    Thread.Sleep(200);
                    //Hand Gun
                    mem.WriteMemory($"{process}.exe+017293C0,0xAC,0x2C,0x18,0x21C,0x0", "string", $"To Server : gm additem {Settings.Nickname} 2010007 3\0");
                    SendKeys.SendWait("{Enter}");
                    Thread.Sleep(200);
                    //Smash Rifle
                    mem.WriteMemory($"{process}.exe+017293C0,0xAC,0x2C,0x18,0x21C,0x0", "string", $"To Server : gm additem {Settings.Nickname} 2010006 4\0");
                    SendKeys.SendWait("{Enter}");
                    SendKeys.SendWait("{Enter}");
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