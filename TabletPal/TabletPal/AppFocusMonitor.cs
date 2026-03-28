using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
// using System.Windows.Automation;

// namespace TabletPal
// {
// 	public class AppFocusMonitor : IDisposable
// 	{
// 		public HashSet<string> IgnoredApps = new HashSet<string>
// 		{
// 			"TabletPal",
// 			"explorer" // Explorer takes over when you click the taskbar. 
// 		};

// 		public string FocusedApp { get; private set; }

// 		public event Action<string> OnAppChanged;

// 		public AppFocusMonitor()
// 		{
// 			Automation.AddAutomationFocusChangedEventHandler(OnFocusChanged);
// 		}


// 		private void OnFocusChanged(object sender, AutomationFocusChangedEventArgs e)
// 		{
// 			try
// 			{
// 				var focusedElement = (AutomationElement)sender;
// 				if (focusedElement != null)
// 				{
// 					using (var process = Process.GetProcessById(focusedElement.Current.ProcessId))
// 					{
// 						if (!IgnoredApps.Contains(process.ProcessName) && FocusedApp != process.ProcessName)
// 						{
// 							FocusedApp = process.ProcessName;
// 							OnAppChanged?.Invoke(FocusedApp);
// 						}
// 					}
// 				}
// 			}
// 			catch
// 			{ }
// 		}


// 		public void Dispose()
// 		{
// 			Automation.RemoveAutomationFocusChangedEventHandler(OnFocusChanged);
// 		}
// 	}
// }
namespace TabletPal
{
    public class AppFocusMonitor : IDisposable
    {
        public HashSet<string> IgnoredApps = new()
        {
            "TabletPal"
        };

        public string FocusedApp { get; private set; }

        public event Action<string> OnAppChanged;

        private CancellationTokenSource _cts = new();

        public AppFocusMonitor()
        {
            _ = MonitorLoop(_cts.Token);
        }

        private async Task MonitorLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var app = await GetFocusedApp();

                    if (!string.IsNullOrEmpty(app) &&
                        !IgnoredApps.Contains(app) &&
                        app != FocusedApp)
                    {
                        FocusedApp = app;
                        OnAppChanged?.Invoke(FocusedApp);
                    }
                }
                catch
                {
                    // swallow errors like original
                }

                await Task.Delay(300, token); // adjust responsiveness here
            }
        }

        private async Task<string> GetFocusedApp()
        {
            // Step 1: get PID of focused window
            var pidOutput = await RunProcess("xdotool", "getwindowfocus getwindowpid");
            if (!int.TryParse(pidOutput.Trim(), out var pid))
                return null;

            // Step 2: get process name
            var nameOutput = await RunProcess("ps", $"-p {pid} -o comm=");
            return nameOutput.Trim();
        }

        private async Task<string> RunProcess(string file, string args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = file,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                }
            };

            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            return output;
        }

        public void Dispose()
        {
            _cts.Cancel();
        }
    }
}