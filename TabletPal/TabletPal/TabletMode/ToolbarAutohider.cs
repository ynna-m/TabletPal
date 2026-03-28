/** 
* Since this relies on TabletMode detection, which is actually very difficult to implement on Linux since protocols may vary, 
* e.g. Wayland, XInput, etc. So I'm just going to disable this feature. I don't really want the Toolbar to autohide anyway.
**/
// using System.Timers;
// using System.Windows;

// namespace TabletPal.TabletMode
// {
// 	public static class ToolbarAutohider
// 	{
// 		private static Timer _timer;

// 		static ToolbarAutohider()
// 		{
// 			_timer = new Timer(1000);
// 			_timer.AutoReset = true;
// 			_timer.Start();
// 			_timer.Elapsed += OnElapsed;
// 		}

// 		private static bool _firstRun = true;
// 		private static bool _oldValue = false;


// 		private static void OnElapsed(object sender, ElapsedEventArgs e)
// 		{
// 			var tabletMode = TabletModeDetector.IsTabletMode;

// 			var changed = (tabletMode != _oldValue) || _firstRun;
// 			_firstRun = false;
// 			_oldValue = tabletMode;

// 			if (changed)
// 			{
// 				Application.Current.Dispatcher.Invoke(
// 					() =>
// 					{
// 						if (!AppState.Settings.ToolbarAutohideEnabled)
// 						{ 
// 							return;
// 						}
// 						if (tabletMode)
// 						{
// 							EventBeacon.SendEvent(Events.Maximize);
// 						}
// 						else
// 						{
// 							EventBeacon.SendEvent(Events.Minimize);
// 						}
// 					}
// 				);
// 			}
// 		}

// 		public static void StopWatching()
// 		{ 
// 			_timer.Stop();
// 		}
// 		public static void StartWatching()
// 		{
// 			_timer.Start();
// 		}
// 	}
// }
