using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TabletPal.InputSender
{
    public class X11Sender : IInputSender
    {
        // private readonly 
        private readonly X11InputExtraction _x11InputExtraction;

        public X11Sender()
        {
            _x11InputExtraction = new X11InputExtraction();
        }

        public void SendChord(string[] keys)
        {
            // var keysSplit = keys.Split('+');

            // var down = string.Join(" ", keysSplit.Select(k => $"keydown {k}"));
            // var up = string.Join(" ", keysSplit.Reverse().Select(k => $"keyup {k}"));

            // await Run($"{down} {up}");
            _x11InputExtraction.KeyChord(keys);
        }

        public void SendClick(string key)
        {
            // await Run($"key {keys}");
            _x11InputExtraction.KeyPress(key);
        }

        public void SendHold(string[] keys)
        {
            // var keysSplit = keys.Split('+');
            // var args = string.Join(" ", keys.Select(k => $"keydown {k}"));
            _x11InputExtraction.KeyDown(keys);
        }

        public void SendRelease(string[] keys)
        {
            // var keysSplit = keys.Split('+');
            // var args = string.Join(" ", keysSplit.Select(k => $"keyup {k}"));
            // await Run(args);
            _x11InputExtraction.KeyUp(keys);
        }
    }
}