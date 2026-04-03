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
            "TabletPal",
            "dotnet", // when running from terminal
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
                catch(Exception ex)
                {
                    Console.WriteLine($"Error occurred while monitoring app focus: {ex.Message}");
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
// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.IO;
// using System.Threading.Tasks;

// namespace TabletPal{
//     public class AppFocusMonitor : IDisposable
//     {
//         public HashSet<string> IgnoredApps = new()
//         {
//             "TabletPal",
//             "dotnet", // when running from terminal
//         };

//         public string FocusedApp { get; private set; }

//         public event Action<string> OnAppChanged;

//         private Process _xpropProcess;

//         public AppFocusMonitor()
//         {
//             StartListening();
//         }

//         private void StartListening()
//         {
//             var psi = new ProcessStartInfo
//             {
//                 FileName = "xprop",
//                 Arguments = "-root -spy _NET_ACTIVE_WINDOW",
//                 RedirectStandardOutput = true,
//                 UseShellExecute = false
//             };

//             _xpropProcess = Process.Start(psi);

//             _ = Task.Run(async () =>
//             {
//                 using var reader = _xpropProcess.StandardOutput;

//                 while (!reader.EndOfStream)
//                 {
//                     var line = await reader.ReadLineAsync();
//                     if (string.IsNullOrWhiteSpace(line))
//                         continue;

//                     var windowId = ExtractWindowId(line);
//                     if (windowId == null)
//                         continue;

//                     var app = GetProcessNameFromWindow(windowId);

//                     if (string.IsNullOrEmpty(app))
//                         continue;

//                     if (IgnoredApps.Contains(app))
//                         continue;

//                     if (app == FocusedApp)
//                         continue;

//                     FocusedApp = app;
//                     OnAppChanged?.Invoke(app);
//                 }
//             });
//         }

//         private string ExtractWindowId(string line)
//         {
//             // Example line:
//             // _NET_ACTIVE_WINDOW(WINDOW): window id # 0x3e00007

//             var parts = line.Split(' ');
//             if (parts.Length == 0)
//                 return null;

//             return parts[^1]; // last token = window id
//         }

//         private string GetProcessNameFromWindow(string windowId)
//         {
//             try
//             {
//                 // Get PID
//                 var pidPsi = new ProcessStartInfo
//                 {
//                     FileName = "xprop",
//                     Arguments = $"-id {windowId} _NET_WM_PID",
//                     RedirectStandardOutput = true,
//                     UseShellExecute = false
//                 };

//                 using var pidProcess = Process.Start(pidPsi);
//                 var pidOutput = pidProcess.StandardOutput.ReadToEnd();
//                 pidProcess.WaitForExit();

//                 var pid = ExtractPid(pidOutput);
//                 if (pid == null)
//                     return null;

//                 return Process.GetProcessById(pid.Value).ProcessName;
//             }
//             catch
//             {
//                 return null;
//             }
//         }

//         private int? ExtractPid(string input)
//         {
//             // Example:
//             // _NET_WM_PID(CARDINAL) = 12345

//             var parts = input.Split('=');
//             if (parts.Length < 2)
//                 return null;

//             var value = parts[1].Trim();

//             if (int.TryParse(value, out var pid))
//                 return pid;

//             return null;
//         }

//         public void Dispose()
//         {
//             try
//             {
//                 if (_xpropProcess != null && !_xpropProcess.HasExited)
//                 {
//                     _xpropProcess.Kill();
//                 }
//             }
//             catch { }
//         }
//     }
// }

// using System;
// using System.Collections.Generic;
// using System.Runtime.InteropServices;
// using System.Threading;

// namespace TabletPal
// {
//     public class AppFocusMonitor : IDisposable
//     {
//         public HashSet<string> IgnoredApps = new HashSet<string> { "TabletPal", "dotnet", "gnome-shell" };
//         public string FocusedApp { get; private set; }
//         public event Action<string> OnAppChanged;

//         private IntPtr _display;
//         private IntPtr _root;
//         private IntPtr _activeWindowAtom;
//         private CancellationTokenSource _cts = new CancellationTokenSource();

//         public AppFocusMonitor()
//         {
//             // Crucial: Initialize threads for Xlib before opening display
//             XInitThreads();
            
//             _display = XOpenDisplay(null);
//             if (_display == IntPtr.Zero) throw new Exception("Unable to open X display.");

//             _root = XDefaultRootWindow(_display);
//             _activeWindowAtom = XInternAtom(_display, "_NET_ACTIVE_WINDOW", false);

//             XSelectInput(_display, _root, (IntPtr)EventMask.PropertyChangeMask);

//             Thread eventThread = new Thread(ListenForEvents) { IsBackground = true };
//             eventThread.Start();
//         }

//         private void ListenForEvents()
//         {
//             XEvent ev = new XEvent();
//             while (!_cts.IsCancellationRequested)
//             {
//                 XNextEvent(_display, ref ev); // Blocks here
//                 if (ev.type == XEventName.PropertyNotify && ev.PropertyEvent.atom == _activeWindowAtom)
//                 {
//                     UpdateFocusedApp();
//                 }
//             }
//         }

//         private void UpdateFocusedApp()
//         {
//             XGetWindowProperty(_display, _root, _activeWindowAtom, IntPtr.Zero, (IntPtr)1, false, (IntPtr)33, 
//                 out _, out _, out _, out _, out IntPtr propRet);
            
//             if (propRet == IntPtr.Zero) return;

//             IntPtr windowId = Marshal.ReadIntPtr(propRet);
//             XFree(propRet);

//             if (windowId == IntPtr.Zero) return;

//             if (XGetClassHint(_display, windowId, out XClassHint hint) != 0)
//             {
//                 string name = Marshal.PtrToStringAnsi(hint.res_name);
//                 XFree(hint.res_name);
//                 XFree(hint.res_class);

//                 if (!string.IsNullOrEmpty(name) && !IgnoredApps.Contains(name) && FocusedApp != name)
//                 {
//                     FocusedApp = name;
//                     OnAppChanged?.Invoke(FocusedApp);
//                 }
//             }
//         }

//         public void Dispose()
//         {
//             _cts.Cancel();
//             if (_display != IntPtr.Zero) XCloseDisplay(_display);
//         }

//         #region X11 P/Invoke
//         private const string X11Lib = "libX11.so.6";

//         [DllImport(X11Lib)] private static extern int XInitThreads();
//         [DllImport(X11Lib)] private static extern IntPtr XOpenDisplay(string display);
//         [DllImport(X11Lib)] private static extern IntPtr XDefaultRootWindow(IntPtr display);
//         [DllImport(X11Lib)] private static extern int XSelectInput(IntPtr display, IntPtr window, IntPtr mask);
//         [DllImport(X11Lib)] private static extern IntPtr XInternAtom(IntPtr display, string name, bool only_if_exists);
//         [DllImport(X11Lib)] private static extern int XNextEvent(IntPtr display, ref XEvent xevent);
//         [DllImport(X11Lib)] private static extern int XFree(IntPtr data);
//         [DllImport(X11Lib)] private static extern int XCloseDisplay(IntPtr display);
//         [DllImport(X11Lib)] private static extern int XGetClassHint(IntPtr display, IntPtr window, out XClassHint hint);
//         [DllImport(X11Lib)] private static extern int XGetWindowProperty(IntPtr display, IntPtr window, IntPtr atom, IntPtr long_offset, IntPtr long_length, bool delete, IntPtr req_type, out IntPtr actual_type, out int actual_format, out IntPtr nitems, out IntPtr bytes_after, out IntPtr prop);

//         [StructLayout(LayoutKind.Sequential)]
//         struct XClassHint { public IntPtr res_name; public IntPtr res_class; }

//         enum XEventName { PropertyNotify = 28 }
//         enum EventMask { PropertyChangeMask = 1 << 22 }

//         [StructLayout(LayoutKind.Explicit, Size = 192)] // Padded to prevent segfaults on 64-bit
//         struct XEvent
//         {
//             [FieldOffset(0)] public XEventName type;
//             [FieldOffset(0)] public XPropertyEvent PropertyEvent;
//         }

//         [StructLayout(LayoutKind.Sequential)]
//         struct XPropertyEvent
//         {
//             public int type;
//             public IntPtr serial;
//             public bool send_event;
//             public IntPtr display;
//             public IntPtr window;
//             public IntPtr atom;
//             public IntPtr time;
//             public int state;
//         }
//         #endregion
//     }
// }