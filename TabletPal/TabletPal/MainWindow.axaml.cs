using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using TabletPal.Docking;
using System.Threading;
using System.Threading.Tasks;


namespace TabletPal
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // private const double MaxOpacity = 1.0;
        // private const double MinOpacity = 0.3;
        private const int FadeInDurationMs = 300;
        private const int FadeOutDurationMs = 500;
        private const int FadeOutDelayMs = 5000;

        private LayoutManager _layout;
		private ThemeManager _theme;
		private LayoutListManager _layoutList;
		private ThemeListManager _themeList;
		private AutomaticLayoutSwitcher _layoutSwitcher;
		private TrayManager _tray;
		private FileManager _file;
        private Settings _settings;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public MainWindow()
        {
            InitializeComponent();
            this.Opacity = MinOpacity;
            this.PointerEntered += OnPointerEnter;
            this.PointerExited += OnPointerLeave;

            var focusMonitor = new AppFocusMonitor();

            Console.WriteLine($"Current directory: {AppState.CurrentDirectory}");
            Directory.SetCurrentDirectory(AppState.CurrentDirectory);

            Topmost = true;

            PointerPressed += OnPointerPressed;

            

            _file = new FileManager();

            ToggleManager.Init();

            _theme = new ThemeManager();
            _layout = new LayoutManager();

            // _settings = new Settings();
            // _settings.Apply();
            // _settings.Save();
            Settings.Load();

            Installer.TryInstall();
            _ = UpdateChecker.Check();

            _layoutList = new LayoutListManager();
            _themeList = new ThemeListManager();

            ContextMenu = new ContextMenu();

            OnUpdateLayoutList();

            _layoutSwitcher = new AutomaticLayoutSwitcher(focusMonitor);
            _tray = new TrayManager(this, _layoutList, _themeList, focusMonitor);

            // if (AppState.Settings.AddToAutostart)
            //     AutostartManager.SetAutostart();
            // else
            //     AutostartManager.ResetAutostart();

            EventBeacon.Subscribe(Events.ToggleMinimize, OnToggleMinimize);
            EventBeacon.Subscribe(Events.Maximize, OnMaximize);
            EventBeacon.Subscribe(Events.Minimize, OnMinimize);
            EventBeacon.Subscribe(Events.UpdateLayoutList, OnUpdateLayoutList);
            EventBeacon.Subscribe(Events.ChangeLayout, OnUpdateLayoutList);
            EventBeacon.Subscribe(Events.DockingChanged, OnDockingChanged);

            Opened += OnOpened;
        }
        // private void InitializeComponent()
        // {
        //     AvaloniaXamlLoader.Load(this);
        // }
        private async void OnPointerEnter(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            await AnimateOpacity(this.Opacity, MaxOpacity, FadeInDurationMs);
        }

        private async void OnPointerLeave(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            await Task.Delay(FadeOutDelayMs);
            // Only fade out if the pointer is still not over the window
            if (!this.IsPointerOver)
            {
                await AnimateOpacity(this.Opacity, MinOpacity, FadeOutDurationMs);
            }
        }

        private async Task AnimateOpacity(double from, double to, int durationMs)
        {
            var steps = 30;
            var delay = durationMs / steps;
            for (int i = 1; i <= steps; i++)
            {
                var t = (double)i / steps;
                this.Opacity = from + (to - from) * t;
                await Task.Delay(delay);
            }
            this.Opacity = to;
        }
        private void OnOpened(object sender, EventArgs e)
        {
            EventBeacon.SendEvent(Events.DockingChanged, AppState.Settings.DockingMode);
        }
        private void OnSizeChanged(object sender, EventArgs eventArgs)
		{
			UiFactory.CreateUi(AppState.CurrentLayout, this);
		}
        private double _maxOpacity;
		public double MaxOpacity
		{
			get => _maxOpacity;
			set
			{
				_maxOpacity = value;
				OnPropertyChanged(nameof(MaxOpacity));
			}
		}


		private double _minOpacity;
		public double MinOpacity
		{
			get => _minOpacity;
			set
			{
				_minOpacity = value;
				OnPropertyChanged(nameof(MinOpacity));
			}
		}

        private void OnUpdateLayoutList(object[] obj = null)
		{
			// Secondary quick access context menu.
			Dispatcher.UIThread.Invoke(
				() =>
				{
					ContextMenu.Items.Clear();
					DockingMenuFactory.CreateDockingMenu(ContextMenu);

					ContextMenu.Items.Add(new Separator());
					var items = _layoutList.GetClonedItems();
					foreach (var item in items)
					{
						ContextMenu.Items.Add(item);
					}
				}
			);
		}
        private void OnMouseDown(object sender, PointerPressedEventArgs e)
        {
            if (AppState.Settings.DockingMode == DockingMode.None)
            {
                BeginMoveDrag(e);
            }
        }

        private void OnDockingChanged(params object[] args)
        {
            var side = (DockingMode)args[0];

            if (side != DockingMode.None && side != AppState.Settings.DockingMode)
            {
                AppBarFunctions.SetAppBar(this, DockingMode.None);
            }

            AppState.Settings.DockingMode = side;

            UiFactory.CreateUi(AppState.CurrentLayout, this);

            if (IsVisible)
            {
                AppBarFunctions.SetAppBar(this, side);
            }

            if (side != DockingMode.None)
            {
                MinOpacity = AppState.CurrentLayout.MaxOpacity;
                MaxOpacity = AppState.CurrentLayout.MaxOpacity;
                Opacity = AppState.CurrentLayout.MaxOpacity;
            }
            else
            {
                MinOpacity = AppState.CurrentLayout.MinOpacity;
                MaxOpacity = AppState.CurrentLayout.MaxOpacity;
                Opacity = AppState.CurrentLayout.MaxOpacity;
            }

            EventBeacon.SendEvent(Events.UpdateSettings);
        }
        private static bool _firstToggle = true;

        private void OnToggleMinimize(object[] obj)
        {
            if (!IsVisible)
            {
                Show();

                if (!_firstToggle)
                {
                    AppBarFunctions.SetAppBar(this, AppState.Settings.DockingMode);
                }
                else
                {
                    Thread.Sleep(500);
                    AppBarFunctions.SetAppBar(this, AppState.Settings.DockingMode);
                    Thread.Sleep(100);
                    AppBarFunctions.SetAppBar(this, DockingMode.None);
                    Thread.Sleep(100);
                    AppBarFunctions.SetAppBar(this, AppState.Settings.DockingMode);
                    _firstToggle = false;
                }
            }
            else
            {
                AppBarFunctions.SetAppBar(this, DockingMode.None);
                Hide();
            }
        }
         private void OnMinimize(object[] obj)
        {
            if (IsVisible)
            {
                AppBarFunctions.SetAppBar(this, DockingMode.None);
                Hide();
            }
        }
        private void OnMaximize(object[] obj)
        {
            if (!IsVisible)
            {
                Show();
                AppBarFunctions.SetAppBar(this, AppState.Settings.DockingMode);
            }
        }
        private void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (
                e.GetCurrentPoint(this).Properties.IsLeftButtonPressed &&
                AppState.Settings.DockingMode == DockingMode.None
            )
            {
                BeginMoveDrag(e);
            }
        }
        public async void OnPointerEnteredFade(object sender, PointerEventArgs e)
        {
            await FadeTo(MaxOpacity, 100);
        }

        public async void OnPointerExitedFade(object sender, PointerEventArgs e)
        {
            await FadeTo(MinOpacity, 300);
        }

        private async Task FadeTo(double target, int durationMs)
        {
            double start = Opacity;
            int steps = 20;
            int delay = durationMs / steps;

            for (int i = 1; i <= steps; i++)
            {
                Opacity = start + (target - start) * i / steps;
                await Task.Delay(delay);
            }
        }    
    }
}