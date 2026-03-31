using System;
using System.IO;
using Avalonia.Threading;

namespace TabletPal
{
	public class FileManager
	{
		private FileSystemWatcher _watcher;
        private DateTime _lastEventTime = DateTime.MinValue;
        private readonly TimeSpan _debounceDelay = TimeSpan.FromMilliseconds(200);

		public FileManager()
		{
			_watcher = new FileSystemWatcher();
			_watcher.Path = AppState.FilesRoot;
			// _watcher.NotifyFilter = NotifyFilters.FileName
			// 	| NotifyFilters.DirectoryName
			// 	| NotifyFilters.Attributes
			// 	| NotifyFilters.Size
			// 	| NotifyFilters.LastWrite
			// 	| NotifyFilters.LastAccess
			// 	| NotifyFilters.CreationTime
			// 	| NotifyFilters.Security;
            _watcher.NotifyFilter =  NotifyFilters.FileName | NotifyFilters.LastWrite;

			_watcher.Changed += OnChanged;
			_watcher.Created += OnChanged;
			_watcher.Deleted += OnChanged;
			_watcher.EnableRaisingEvents = true;
			_watcher.IncludeSubdirectories = true;

			RefreshLists();
		}


		private void OnChanged(object sender, FileSystemEventArgs args)
		{
            // Apparently, needs a debouncer in Linux, otherwise, we're stuck in a loop.
            var now = DateTime.Now;

            if (now - _lastEventTime < _debounceDelay)
                return;

            _lastEventTime = now;
            RefreshLists();
			EventBeacon.SendEvent(Events.FilesChanged, sender, args);
		}


		private void RefreshLists()
		{
			Dispatcher.UIThread.Invoke(async ()=>
				{
					var layouts = await Importer.ImportLayouts();
					if (AppState.Layouts == null || layouts.Count > 0)
					{
						AppState.Layouts = layouts;
					}
					var themes = await Importer.ImportThemes();
					if (AppState.Themes == null || themes.Count > 0)
					{
						AppState.Themes = themes;
					}
				}
			);
			EventBeacon.SendEvent(Events.UpdateThemeList);
			EventBeacon.SendEvent(Events.UpdateLayoutList);
		}
	}
}
