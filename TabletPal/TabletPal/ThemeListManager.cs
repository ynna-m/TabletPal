using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace TabletPal
{
	public class ThemeListManager
	{
		public MenuItem Menu;


		public ThemeListManager()
		{
			Menu = new MenuItem() { Header = "themes" };

			OnUpdateThemeList();
			EventBeacon.Subscribe(Events.ChangeTheme, OnChangeTheme);
			EventBeacon.Subscribe(Events.UpdateThemeList, OnUpdateThemeList);
		}

		private void OnChangeTheme(object[] obj)
		{
			var path = (string)obj[0];
			foreach (MenuItem otherItem in Menu.Items)
			{
				otherItem.IsChecked = (string)otherItem.DataContext == path;
			}
		}

		private void OnUpdateThemeList(object[] obj = null)
		{
			Dispatcher.UIThread.Invoke(
				()=>
				{
					Menu.Items.Clear();
					foreach (var theme in AppState.Themes.Keys)
					{
						var item = new MenuItem()
						{
							Header = Path.GetFileNameWithoutExtension(theme).Replace("_", " "),
							DataContext = theme,
							ToggleType = MenuItemToggleType.Radio,
							IsChecked = theme == AppState.CurrentThemeName
						};
						Menu.Items.Add(item);
						item.Click += OnClick;
					}
				}
			);
		}

		private void OnClick(object sender, RoutedEventArgs e)
		{
			var item = (MenuItem)sender;
			EventBeacon.SendEvent(Events.ChangeTheme, item.DataContext);
			EventBeacon.SendEvent(Events.ChangeLayout, AppState.CurrentLayoutName);
		}

        private void OnClick(object sender, EventArgs e)
		{
			var item = (MenuItem)sender;
			EventBeacon.SendEvent(Events.ChangeTheme, item.DataContext);
			EventBeacon.SendEvent(Events.ChangeLayout, AppState.CurrentLayoutName);
		}
		public MenuItem CloneMenu()
		{
			var menu = new MenuItem() { Header = "themes" };

			var items = new List<MenuItem>();

			foreach (MenuItem item in Menu.Items)
			{
				var newItem = new MenuItem()
				{
					Header = item.Header,
					DataContext = item.DataContext,
					ToggleType = item.ToggleType,
					IsChecked = item.IsChecked,
				};
				menu.Items.Add(newItem);
				newItem.Click += OnClick;
			}

			return menu;
		}
        public NativeMenuItem GetNativeMenu()
        {
            var nativeMenu = new NativeMenuItem() { 
                Header = "themes", 
                Menu = new NativeMenu(),
                IsEnabled = Menu.Items.Count > 0
            };
            foreach (MenuItem item in Menu.Items)
            {
                NativeMenuItemToggleType toggleType = item.ToggleType switch
                {
                    MenuItemToggleType.None => NativeMenuItemToggleType.None,
                    MenuItemToggleType.CheckBox => NativeMenuItemToggleType.CheckBox,
                    MenuItemToggleType.Radio => NativeMenuItemToggleType.Radio,
                    _ => NativeMenuItemToggleType.None
                };
                var nativeItem = new NativeMenuItem(item.Header?.ToString() ?? "")
                {
                    Header = item.Header?.ToString() ?? "",
                    ToggleType = toggleType,
					IsChecked = item.IsChecked,
                };
                nativeItem.Click += (s, e) => { EventBeacon.SendEvent(Events.ChangeTheme, item.DataContext);
                    EventBeacon.SendEvent(Events.ChangeLayout, AppState.CurrentLayoutName);
                };
                nativeMenu.Menu.Items.Add(nativeItem);
            }
            return nativeMenu;
        }
	}
}
