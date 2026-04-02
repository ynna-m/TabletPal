using System.Collections.Generic;
using System.IO;
// using System.Windows;
// using System.Windows.Controls;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Interactivity;
using System;
namespace TabletPal
{
	public class LayoutListManager
	{
		public MenuItem Menu;


		public LayoutListManager()
		{
			Menu = new MenuItem() { Header = "layouts" };

			OnUpdateLayoutList();
			EventBeacon.Subscribe(Events.ChangeLayout, OnChangeLayout);
			EventBeacon.Subscribe(Events.UpdateLayoutList, OnUpdateLayoutList);
		}

		private void OnChangeLayout(object[] obj)
		{
			var path = (string)obj[0];
			foreach (MenuItem otherItem in Menu.Items)
			{
				otherItem.IsChecked = (string)otherItem.DataContext == path;
			}
		}

		private void OnUpdateLayoutList(object[] obj = null)
		{
			Dispatcher.UIThread.Invoke(()=>
				{
					Menu.Items.Clear();
					foreach (var layout in AppState.Layouts.Keys)
					{
						var item = new MenuItem()
						{
							Header = Path.GetFileNameWithoutExtension(layout).Replace("_", " "),
							DataContext = layout,
							ToggleType = MenuItemToggleType.Radio,
							IsChecked = layout == AppState.CurrentLayoutName
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
			EventBeacon.SendEvent(Events.ChangeLayout, item.DataContext);
		}

		
		public MenuItem[] GetClonedItems()
		{
			var items = new List<MenuItem>();

			foreach(MenuItem item in Menu.Items)
			{
				var newItem = new MenuItem()
				{
					Header = item.Header,
					DataContext = item.DataContext,
					ToggleType = MenuItemToggleType.Radio,
					IsChecked = item.IsChecked,
				};
				items.Add(newItem);
				newItem.Click += OnClick;
			}

			return items.ToArray();
		}

		public MenuItem CloneMenu()
		{
			var menu = new MenuItem() { Header = "layouts" };

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
        // New method for tray icon
        public NativeMenuItem GetNativeMenu()
        {
            var nativeMenu = new NativeMenuItem() { 
                Header = "layouts", 
                Menu = new NativeMenu(),
                IsEnabled = Menu.Items.Count > 0
            };
            // var nativeMenu = new NativeMenu();
            Console.WriteLine($"LayoutListManager.cs - GetNativeMenu(). Menu has {Menu.Items.Count} items.");
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
                nativeItem.Click += (s, e) => EventBeacon.SendEvent(Events.ChangeLayout, item.DataContext);
                nativeMenu.Menu.Items.Add(nativeItem);
                // yield return nativeItem;
            }
            return nativeMenu;
        }
	}
}
