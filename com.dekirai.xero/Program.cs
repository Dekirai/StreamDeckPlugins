using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Timers;
using StreamDeckCS;
using StreamDeckCS.EventsReceived;

namespace com.dekirai.xero
{
    internal class Program
    {
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        const int KEYEVENTF_EXTENDEDKEY = 0x1;
        const int KEYEVENTF_KEYUP = 0x2;

        static async Task Main(string[] args)
        {
            Plugin plugin = new Plugin(args);
            await plugin.startPluginAsync();
        }

        public class Plugin
        {
            StreamdeckCore core;
            //Timer timer;

            // fields
            //private int numPressed = 0;
            //private int timerDuration = 2000;
            //private string context = null;

            public Plugin(string[] args)
            {
                core = new StreamdeckCore(args);
                //timer = new Timer(timerDuration);

                // subscribe to events
                //timer.Elapsed += Timer_Elapsed;
                core.KeyUpEvent += Core_KeyUpEvent;
                core.KeyDownEvent += Core_KeyDownEvent;
                core.WillAppearEvent += Core_WillAppearEvent;

                //timer.Enabled = true;

            }

            // raises when button is held for timerDuration (2000 ms i.e 2 seconds), resets counter
            private void Timer_Elapsed(object sender, ElapsedEventArgs e)
            {
                //Not used for now
            }
            private void Core_KeyDownEvent(object sender, KeyDown e)
            {
                switch (e.action)
                {
                    case "com.dekirai.xero.test":
                        break;
                }
            }

            // when we release key, stop timer and set title of the key to next num in fibonacci sequence
            private void Core_KeyUpEvent(object sender, KeyUp e)
            {
                //timer.Stop();
            }

            private void Core_WillAppearEvent(object sender, WillAppear e)
            {
                //Not used for now
            }

            // starts the plugin
            public async Task startPluginAsync()
            {
                await core.Start();
            }

        }
    }
}