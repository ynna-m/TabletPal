using System.Diagnostics;
using System.Threading.Tasks;

namespace TabletFriend.Actions
{
	public class TerminalAction : ButtonAction
	{
		private readonly string _terminal;

		public TerminalAction(string terminal)
		{
			_terminal = terminal;
		}

		public override Task Invoke()
		{
			var process = new Process();
			var startInfo = new ProcessStartInfo();
			// startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			// startInfo.FileName = "cmd.exe";
			// startInfo.Arguments = "/C " + _terminal;
            startInfo.FileName = "/bin/bash";
            startInfo.Arguments = "-c \"" + _terminal + "\"";
            startInfo.RedirectStandardOutput = false;
            startInfo.RedirectStandardError = false;
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = true;
			process.StartInfo = startInfo;
			process.Start();
			return Task.CompletedTask;
		}
	}
}
