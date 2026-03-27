using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TabletFriend.InputSender
{
    public class XdotoolSender : IInputSender
    {
        private async Task Run(string args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "xdotool",
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };

            process.Start();
            await process.WaitForExitAsync();
        }
        public async Task SendChord(string keys)
        {
            var keysSplit = keys.Split('+');

            var down = string.Join(" ", keysSplit.Select(k => $"keydown {k}"));
            var up = string.Join(" ", keysSplit.Reverse().Select(k => $"keyup {k}"));

            await Run($"{down} {up}");
        }

        public async Task SendClick(string keys)
        {
            await Run($"key {keys}");
        }

        public async Task SendHold(string keys)
        {
            var keysSplit = keys.Split('+');
            var args = string.Join(" ", keys.Select(k => $"keydown {k}"));
            await Run(args);
        }

        public async Task SendRelease(string keys)
        {
            var keysSplit = keys.Split('+');
            var args = string.Join(" ", keysSplit.Select(k => $"keyup {k}"));
            await Run(args);
        }
    }
}