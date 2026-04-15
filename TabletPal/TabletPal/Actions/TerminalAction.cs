using System.Diagnostics;
using System.Threading.Tasks;

namespace TabletPal.Actions
{
	public class TerminalAction : ButtonAction
	{
		private readonly string _terminal;

		public TerminalAction(string terminal)
		{
			_terminal = terminal;
		}

		public override void Invoke()
		{
			var process = new Process();
			var startInfo = new ProcessStartInfo();
            startInfo.FileName = "/bin/bash";
            startInfo.Arguments = "-c \"" + _terminal + "\"";
            startInfo.RedirectStandardOutput = false;
            startInfo.RedirectStandardError = false;
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = true;
			process.StartInfo = startInfo;
			process.Start();
		}
	}
}
