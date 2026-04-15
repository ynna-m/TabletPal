using System;
using System.Diagnostics;
using System.IO;
using TabletPal.Docking;
using Avalonia.Controls;
using Avalonia.Threading;
using System.Collections.Generic;

namespace TabletPal
{
	public class TrayManager
	{
		private TrayIcon _icon;
        private NativeMenuItem _dockingMenu;
        // Commented out to let you guys know that I'm not implementing an option here to do autostart. You can do this with your desktop environment manager in Linux
		// private NativeMenuItem _autostartMenuItem;
		private NativeMenuItem _autoUpdateMenuItem;
		private NativeMenuItem _perAppLayoutsMenuItem;
        // I haven't implemented light and dark theme. Currently, it's set to dark theme.
		// private readonly string _iconPathBlack = AppState.CurrentDirectory + "/files/icons/tray/tray_black.ico";
		private readonly string _iconPathWhite = AppState.CurrentDirectory + "/files/icons/tray/tray_white.ico";
		private readonly MainWindow _window;
		private readonly LayoutListManager _layoutList;
		private readonly ThemeListManager _themeList;

		private AppFocusMonitor _focusMonitor;
		private NativeMenuItem _focusedApp;
		public TrayManager(MainWindow window, LayoutListManager layoutList, ThemeListManager themeList, AppFocusMonitor focusMonitor)
        {
			_window = window;
			_layoutList = layoutList;
			_themeList = themeList;
			_focusMonitor = focusMonitor;
            
			_icon = new TrayIcon();

            // See comment on _iconPathBlack
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
			Dispatcher.UIThread.Invoke(
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

            var layoutsMenu = _layoutList.GetNativeMenu();
            _icon.Menu.Items.Add(layoutsMenu);

            var themesMenu = _themeList.GetNativeMenu();
            _icon.Menu.Items.Add(themesMenu);
            
			_dockingMenu = DockingMenuFactory.CreateNativeDockingMenu();
            _icon.Menu.Items.Add(_dockingMenu);
			var settings = new NativeMenuItem() { 
                Header = "settings",
                Menu = new NativeMenu()
            };

			if (AppState.Settings.UpdateCheckingEnabled)
			{
				_autoUpdateMenuItem = AddSubmenuItem(settings, "don't check for updates", OnAutoUpdateToggle);
			}
			else
			{
				_autoUpdateMenuItem = AddSubmenuItem(settings, "check for updates", OnAutoUpdateToggle);
			}
            // Not implementing auto hide either, but retaining this just in case.
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


			_icon.Menu.Items.Add(settings);
			_icon.Menu.Add(new NativeMenuItemSeparator());
			AddMenuItem("about", OnAbout);
			AddMenuItem("quit", OnQuit);
		}

		private void OnAppChanged(string app)
		{

            Dispatcher.UIThread.Invoke(
                () =>
                {
                    _focusedApp.Header = "focused app: " + app;
                }
            );
		}

		private void OnAutoUpdateToggle(object sender, EventArgs e)
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


		private void OnPerAppLayoutsToggle(object sender, EventArgs e)
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


		private void OnAbout(object sender, EventArgs e)
		{
			var startInfo = new ProcessStartInfo()
			{
				Arguments = AppState.LayoutsRoot,
				FileName = "http://github.com/ynna-m/TabletPal",
				UseShellExecute = true,
			};
			Process.Start(startInfo);
		}

		private NativeMenuItem AddMenuItem(string header, EventHandler click = null)
		{
			var item = new NativeMenuItem() { Header = header };
			if (click != null)
			{
				item.Click += click;
			}
			else
			{
				item.IsEnabled = false;
			}
			_icon.Menu.Items.Add(item);
			return item;
		}

		private NativeMenuItem AddSubmenuItem(NativeMenuItem menu, string header, EventHandler click = null)
		{
			var item = new NativeMenuItem() { Header = header };
			if (click != null)
			{
				item.Click += click;
			}
			else
			{
				item.IsEnabled = false;
			}
			menu.Menu.Items.Add(item);
			return item;
		}

		private void OnOpenLayoutsDirectory(object sender, EventArgs e)
		{
			var startInfo = new ProcessStartInfo()
			{
				FileName = "xdg-open"
			};
            startInfo.ArgumentList.Add(Path.Combine(AppState.CurrentDirectory, "files"));
			Process.Start(startInfo);
		}


		private void OnQuit(object sender, EventArgs e)
		{
			AppBarFunctions.SetAppBar(_window, DockingMode.None);
			_icon.Dispose();
			EventBeacon.SendEvent(Events.UpdateSettings);
			Process.GetCurrentProcess().Kill();
		}        
	}
}
