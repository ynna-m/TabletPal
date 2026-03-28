using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace TabletFriend
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
			Dispatcher.UIThread.Post(
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
        public IEnumerable<NativeMenuItemBase> GetNativeMenuItems()
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
	}
}
