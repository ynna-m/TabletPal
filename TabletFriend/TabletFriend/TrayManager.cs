using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TabletFriend.Docking;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System.Collections.Generic;

namespace TabletFriend
{
	public class TrayManager
	{
		private TrayIcon _icon;
        private ContextMenu _dockingContextMenu;
		private MenuItem _autostartMenuItem;
		private MenuItem _autoUpdateMenuItem;
		// private MenuItem _autohideMenuItem;
		private MenuItem _perAppLayoutsMenuItem;
		// private readonly string _iconPathBlack = AppState.CurrentDirectory + "/files/icons/tray/tray_black.ico";
		private readonly string _iconPathWhite = AppState.CurrentDirectory + "/files/icons/tray/tray_white.ico";
		private readonly MainWindow _window;
		private readonly LayoutListManager _layoutList;
		private readonly ThemeListManager _themeList;

		private AppFocusMonitor _focusMonitor;
		private MenuItem _focusedApp;
		public TrayManager(MainWindow window, LayoutListManager layoutList, ThemeListManager themeList, AppFocusMonitor focusMonitor)
		{
			_window = window;
			_layoutList = layoutList;
			_themeList = themeList;
			_focusMonitor = focusMonitor;
            
            _dockingContextMenu = new ContextMenu();
			_icon = new TrayIcon();

			// if (_isLightTheme)
			// {
			// 	_icon.Icon = new WindowIcon(_iconPathBlack);
			// }
			// else
			// { 
				_icon.Icon = new WindowIcon(_iconPathWhite);
			// }

			_icon.IsVisible = true;

			_icon.Menu = new NativeMenu();
			_icon.Clicked += MouseDown;
			CreateMenu();

			EventBeacon.Subscribe(Events.ChangeLayout, OnUpdateLayoutList);
			EventBeacon.Subscribe(Events.FilesChanged, OnUpdateLayoutList);
		}

		private void MouseDown(object sender, EventArgs e)
		{
			EventBeacon.SendEvent(Events.ToggleMinimize);
		}

		private void OnUpdateLayoutList(object[] obj = null)
		{
			// Secondary quick access context menu.
			Dispatcher.UIThread.Post(
				() =>
				{
					_icon.Menu.Items.Clear();
					_icon.Menu = new NativeMenu();
					CreateMenu();
				}
			);
		}

		private void CreateMenu()
		{
			foreach (var nativeItem in _layoutList.GetNativeMenuItems())
            {
                _icon.Menu.Items.Add(nativeItem);
            }
            foreach (var nativeItem in _themeList.GetNativeMenuItems())
            {
                _icon.Menu.Items.Add(nativeItem);
            }
			// _icon.ContextMenu.Items.Add(_layoutList.CloneMenu());
			// _icon.ContextMenu.Items.Add(_themeList.CloneMenu());
            _dockingContextMenu.Items.Add(_layoutList.CloneMenu());
            _dockingContextMenu.Items.Add(_themeList.CloneMenu());
			DockingMenuFactory.CreateDockingMenu(_dockingContextMenu);

			var settings = new MenuItem() { Header = "settings" };

			// if (AppState.Settings.AddToAutostart)
			// {
			// 	_autostartMenuItem = AddSubmenuItem(settings, "remove from autostart", OnAutostartToggle);
			// }
			// else
			// {
			// 	_autostartMenuItem = AddSubmenuItem(settings, "add to autostart", OnAutostartToggle);
			// }

			if (AppState.Settings.UpdateCheckingEnabled)
			{
				_autoUpdateMenuItem = AddSubmenuItem(settings, "don't check for updates", OnAutoUpdateToggle);
			}
			else
			{
				_autoUpdateMenuItem = AddSubmenuItem(settings, "check for updates", OnAutoUpdateToggle);
			}

			// if (AppState.Settings.ToolbarAutohideEnabled)
			// {
			// 	_autohideMenuItem = AddSubmenuItem(settings, "disable toolbar autohide", OnAutohideToggle);
			// 	ToolbarAutohider.StartWatching();
			// }
			// else
			// {
			// 	_autohideMenuItem = AddSubmenuItem(settings, "enable toolbar autohide", OnAutohideToggle);
			// 	ToolbarAutohider.StopWatching();
			// }

			if (AppState.Settings.PerAppLayoutsEnabled)
			{
				_perAppLayoutsMenuItem = AddSubmenuItem(settings, "disable per-app layouts", OnPerAppLayoutsToggle);
			}
			else
			{
				_perAppLayoutsMenuItem = AddSubmenuItem(settings, "enable per-app layouts", OnPerAppLayoutsToggle);
			}


			AddSubmenuItem(settings, "open files directory...", OnOpenLayoutsDirectory);
			_focusedApp = AddSubmenuItem(settings, "focused app: none");
			_focusMonitor.OnAppChanged += OnAppChanged;

            foreach (var nativeItem in ConvertToNativeMenuItems(settings))
            {
                _icon.Menu.Items.Add(nativeItem);
            }
			
			_icon.Menu.Add(new NativeMenuItemSeparator());
			AddMenuItem("about", OnAbout);
			AddMenuItem("quit", OnQuit);
		}

		private void OnAppChanged(string app)
		{
			// _focusedApp.Dispatcher.Invoke(
			// 	() =>
			// 	{
			// 		_focusedApp.Header = "focused app: " + app;
			// 	}
			// );	
            Dispatcher.UIThread.Post(
                () =>
                {
                    _focusedApp.Header = "focused app: " + app;
                }
            );
		}

		// private void OnAutostartToggle(object sender, RoutedEventArgs e)
		// {
		// 	AppState.Settings.AddToAutostart = !AppState.Settings.AddToAutostart;

		// 	if (AppState.Settings.AddToAutostart)
		// 	{
		// 		AutostartManager.SetAutostart();
		// 		_autostartMenuItem.Header = "remove from autostart";
		// 	}
		// 	else
		// 	{
		// 		AutostartManager.ResetAutostart();
		// 		_autostartMenuItem.Header = "add to autostart";
		// 	}
		// 	EventBeacon.SendEvent(Events.UpdateSettings);
		// }

		private void OnAutoUpdateToggle(object sender, RoutedEventArgs e)
		{
			AppState.Settings.UpdateCheckingEnabled = !AppState.Settings.UpdateCheckingEnabled;

			if (AppState.Settings.UpdateCheckingEnabled)
			{
				_autoUpdateMenuItem.Header = "don't check for updates";
			}
			else
			{
				_autoUpdateMenuItem.Header = "check for updates";
			}
			EventBeacon.SendEvent(Events.UpdateSettings);
		}

		// private void OnAutohideToggle(object sender, RoutedEventArgs e)
		// {
		// 	AppState.Settings.ToolbarAutohideEnabled = !AppState.Settings.ToolbarAutohideEnabled;

		// 	if (AppState.Settings.ToolbarAutohideEnabled)
		// 	{
		// 		_autohideMenuItem.Header = "disable toolbar autohide";
		// 		ToolbarAutohider.StartWatching();
		// 	}
		// 	else
		// 	{
		// 		_autohideMenuItem.Header = "enable toolbar autohide";
		// 		ToolbarAutohider.StopWatching();
		// 	}
		// 	EventBeacon.SendEvent(Events.UpdateSettings);
		// }

		private void OnPerAppLayoutsToggle(object sender, RoutedEventArgs e)
		{
			AppState.Settings.PerAppLayoutsEnabled = !AppState.Settings.PerAppLayoutsEnabled;

			if (AppState.Settings.PerAppLayoutsEnabled)
			{
				_perAppLayoutsMenuItem.Header = "disable per-app layouts";
			}
			else
			{
				_perAppLayoutsMenuItem.Header = "enable per-app layouts";
			}
			EventBeacon.SendEvent(Events.UpdateSettings);
		}


		private void OnAbout(object sender, RoutedEventArgs e)
		{
			var startInfo = new ProcessStartInfo()
			{
				Arguments = AppState.LayoutsRoot,
				FileName = "http://github.com/ynna-m/TabletFriend",
				UseShellExecute = true,
			};
			Process.Start(startInfo);
		}

		private MenuItem AddMenuItem(string header, EventHandler<RoutedEventArgs> click = null)
		{
			var item = new MenuItem() { Header = header };
			if (click != null)
			{
				item.Click += click;
			}
			else
			{
				item.IsEnabled = false;
			}
            foreach (var nativeItem in ConvertToNativeMenuItems(item))
            {
                _icon.Menu.Items.Add(nativeItem);
            }
			// _icon.Menu.Items.Add(item);
			return item;
		}

		private MenuItem AddSubmenuItem(MenuItem menu, string header, EventHandler<RoutedEventArgs> click = null)
		{
			var item = new MenuItem() { Header = header };
			if (click != null)
			{
				item.Click += click;
			}
			else
			{
				item.IsEnabled = false;
			}
			menu.Items.Add(item);
			return item;
		}

		private void OnOpenLayoutsDirectory(object sender, RoutedEventArgs e)
		{
			var startInfo = new ProcessStartInfo()
			{
				Arguments = Path.Combine(AppState.CurrentDirectory, "files"),
				FileName = "explorer.exe"
			};
			Process.Start(startInfo);
		}


		private void OnQuit(object sender, RoutedEventArgs e)
		{
			AppBarFunctions.SetAppBar(_window, DockingMode.None);
			_icon.Dispose();
			EventBeacon.SendEvent(Events.UpdateSettings);
			Process.GetCurrentProcess().Kill();
		}

        public IEnumerable<NativeMenuItemBase> ConvertToNativeMenuItems(MenuItem Menu)
        {
            // var nativeMenu = new NativeMenu();
            foreach (MenuItem item in Menu.Items)
            {
                var nativeItem = new NativeMenuItem(item.Header?.ToString() ?? "");
                nativeItem.Click += (s, e) => EventBeacon.SendEvent(Events.ChangeLayout, item.DataContext);
                // nativeMenu.Items.Add(nativeItem);
                yield return nativeItem;
            }
            // return nativeMenu;
        }
		// private bool _isLightTheme
		// {
		// 	get
		// 	{
		// 		var key = Registry.GetValue(
		// 			@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", 
		// 			"SystemUsesLightTheme", 
		// 			0
		// 		);
		// 		return !(key == null || (int)key == 0);
		// 	}
		// }
        
	}
}
