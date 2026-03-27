using TabletFriend.Models;
using TabletFriend.Docking;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using IconPacks.Avalonia.MaterialDesign;
using Avalonia;
using Avalonia.Styling;

namespace TabletFriend
{
	public static class TitlebarManager
	{

		private const PackIconMaterialDesignKind _defaultIcon = PackIconMaterialDesignKind.CircleOutline;
		private const PackIconMaterialDesignKind _minimizedIcon = PackIconMaterialDesignKind.Circle;
		private const double _baseTitlebarHeight = 12;

		private static bool _minimizedMode = false;
		private static bool _minimized = false;
		public static bool Minimized => _minimized;

		private static PackIconMaterialDesign _ico;
		private static MainWindow _window;
		private static ThemeModel _theme;
		private static LayoutModel _layout;

		private static double _maximizedWindowHeight;

		/// <summary>
		/// Blocks the first minimize to make the feel a little bit nicer.
		/// </summary>
		private static bool _grace = false;
		public static double GetTitlebarHeight(LayoutModel layout)
		{
			if (AppState.Settings.DockingMode == DockingMode.None)
			{
				return _baseTitlebarHeight + layout.Margin * 2;
			}
			return 0;
		}

		public static void CreateTitlebar(MainWindow window, ThemeModel theme, LayoutModel layout, double maximizedWindowHeight, bool minimized)
		{
			_minimized = minimized;

			_window = window;
			_theme = theme;
			_layout = layout;
			_maximizedWindowHeight = maximizedWindowHeight;

			_window.PointerEntered -= OnMouseEnter;
			_window.PointerExited -= OnMouseLeave;

			if (AppState.Settings.DockingMode != DockingMode.None)
			{
				return;
			}


			CreateButton();

			_window.PointerEntered += OnMouseEnter;
			_window.PointerExited += OnMouseLeave;

			_grace = true;

			if (_minimized)
			{
				_window.Height = GetTitlebarHeight(_layout);
			}
		}


		private static void CreateButton()
		{
			var uiButton = new Button();

			uiButton.Width = 32;
			uiButton.Height = GetTitlebarHeight(_layout);

			uiButton.Styles.Add(Application.Current.Resources["shy"] as Styles);
			_ico = new PackIconMaterialDesign();
			if (_minimizedMode)
			{
				_ico.Kind = _minimizedIcon;
			}
			else
			{
				_ico.Kind = _defaultIcon;
			}
			uiButton.Content = _ico;
			uiButton.Click += OnClick;

			Canvas.SetTop(uiButton, 0);
			Canvas.SetLeft(uiButton, (_window.Width - uiButton.Width) / 2);
			_window.MainCanvas.Children.Add(uiButton);

		}

		private static void OnMouseLeave(object sender, PointerEventArgs e)
		{
			if (_minimizedMode && !_grace)
			{
				Minimize();
			}
			_grace = false;
		}

		private static void OnMouseEnter(object sender, PointerEventArgs e)
		{
			if (_minimizedMode)
			{
				Maximize();
			}
			_grace = false;
		}

		private static void OnClick(object sender, RoutedEventArgs e)
		{
			if (_minimizedMode)
			{
				Maximize();
				_ico.Kind = _defaultIcon;
			}
			else
			{
				if (!_window.IsPointerOver)
				{
					Minimize();
				}
				_ico.Kind = _minimizedIcon;
			}
			_minimizedMode = !_minimizedMode;
		}

		public static void Minimize()
		{
			if (!_minimized)
			{
				_minimized = true;
				_maximizedWindowHeight = _window.Height;
				_window.Height = GetTitlebarHeight(_layout);
			}
		}

		public static void Maximize()
		{
			if (_minimized)
			{
				_minimized = false;
				_window.Height = _maximizedWindowHeight;
			}
		}


	}
}
