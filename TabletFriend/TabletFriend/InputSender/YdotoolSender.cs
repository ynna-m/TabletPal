using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TabletFriend.InputSender
{
    public class YdotoolSender : IInputSender
    {
        private async Task Run(string args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "bash",
                    Arguments = $"-c \"ydotool {args}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };

            process.Start();
            await process.WaitForExitAsync();
        }
        private string Build(string keys)
        {
            return $"key $(ydokey -k \"{keys}\")";
        }
        public async Task SendChord(string keys)
        {
            await Run(keys);
        }

        public async Task SendClick(string keys)
        {
            await Run(keys); // same as click for ydotool
        }

        public async Task SendHold(string keys)
        {
            throw new NotImplementedException();
        }

        public async Task SendRelease(string keys)
        {
            throw new NotImplementedException();
        }
    }
}