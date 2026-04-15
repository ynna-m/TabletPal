using Avalonia.Controls;

namespace TabletPal.Docking
{
	public static class DockingMenuFactory
	{
		public static void CreateDockingMenu(ContextMenu menu)
		{
			var docking = new MenuItem() { Header = "docking" };

			var item = new MenuItem() { Header = "none" };
			item.Click += (sender, e) => OnDocking(DockingMode.None);
			docking.Items.Add(item);

			item = new MenuItem() { Header = "left" };
			item.Click += (sender, e) => OnDocking(DockingMode.Left);
			docking.Items.Add(item);

			item = new MenuItem() { Header = "top" };
			item.Click += (sender, e) => OnDocking(DockingMode.Top);
			docking.Items.Add(item);

			item = new MenuItem() { Header = "right" };
			item.Click += (sender, e) => OnDocking(DockingMode.Right);
			docking.Items.Add(item);

			// Bottom docking is broken as fuck. Maybe will fix it someday.
			//item = new MenuItem() {Header = "bottom"};
			//item.Click += (sender, e) => OnDocking(ABEdge.Bottom);
			//menu.Items.Add(item);

			menu.Items.Add(docking);
		}
        public static NativeMenuItem CreateNativeDockingMenu()
		{
			
            var menu = new NativeMenuItem()
            {
                Header = "docking",
                Menu = new NativeMenu()
            };
			var item = new NativeMenuItem() { Header = "none" };
			item.Click += (sender, e) => OnDocking(DockingMode.None);
			menu.Menu.Items.Add(item);

			item = new NativeMenuItem() { Header = "left" };
			item.Click += (sender, e) => OnDocking(DockingMode.Left);
			menu.Menu.Items.Add(item);
            
			item = new NativeMenuItem() { Header = "top" };
			item.Click += (sender, e) => OnDocking(DockingMode.Top);
			menu.Menu.Items.Add(item);

			item = new NativeMenuItem() { Header = "right" };
			item.Click += (sender, e) => OnDocking(DockingMode.Right);
			menu.Menu.Items.Add(item);

			// Bottom docking is broken as fuck. Maybe will fix it someday.
			//item = new MenuItem() {Header = "bottom"};
			//item.Click += (sender, e) => OnDocking(ABEdge.Bottom);
			//menu.Items.Add(item);

			// menu.Menu.Items.Add(docking);
            return menu;
		}
		private static void OnDocking(DockingMode side) =>
			EventBeacon.SendEvent(Events.DockingChanged, side);
	}
}
