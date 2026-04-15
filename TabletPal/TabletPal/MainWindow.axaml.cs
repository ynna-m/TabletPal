using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using TabletPal.Docking;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;


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
            var focusMonitor = new AppFocusMonitor();
            // this.Opacity = MinOpacity;
            // this.PointerEntered += OnPointerEnter;
            // this.PointerExited += OnPointerLeave;

            Directory.SetCurrentDirectory(AppState.CurrentDirectory);

            Topmost = true;

            PointerPressed += OnPointerPressed;

            _file = new FileManager();

            _theme = new ThemeManager();
            _layout = new LayoutManager();

            Settings.Load(this);

            Installer.TryInstall();
            _ = UpdateChecker.Check();

            _layoutList = new LayoutListManager();
            _themeList = new ThemeListManager();

            ContextMenu = new ContextMenu();

            OnUpdateLayoutList();

            _layoutSwitcher = new AutomaticLayoutSwitcher(focusMonitor); 
            _tray = new TrayManager(this, _layoutList, _themeList, focusMonitor);

            EventBeacon.Subscribe(Events.ToggleMinimize, OnToggleMinimize);
            EventBeacon.Subscribe(Events.Maximize, OnMaximize);
            EventBeacon.Subscribe(Events.Minimize, OnMinimize);
            EventBeacon.Subscribe(Events.UpdateLayoutList, OnUpdateLayoutList);
            EventBeacon.Subscribe(Events.ChangeLayout, OnUpdateLayoutList);
            EventBeacon.Subscribe(Events.DockingChanged, OnDockingChanged);

            Opened += OnOpened;
        }
        private async void OnPointerEnter(object? sender, PointerEventArgs e)
        {
            await AnimateOpacity(this.Opacity, MaxOpacity, FadeInDurationMs);
        }

        private async void OnPointerLeave(object? sender, PointerEventArgs e)
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
                this.IsVisible=true;
                Topmost=false;
                Topmost=true;
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
                this.IsVisible=false;
            }
        }
         private void OnMinimize(object[] obj)
        {
            if (IsVisible)
            {
                AppBarFunctions.SetAppBar(this, DockingMode.None);
                this.IsVisible=false;
            }
        }
        private void OnMaximize(object[] obj)
        {
            if (!IsVisible)
            {
                // Show();
                this.IsVisible=true;
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
        private IntPtr _lastWindowId;
        private IntPtr _display = XOpenDisplay(IntPtr.Zero);
        private const int RevertToParent = 2;

        public void CaptureFocusedWindow()
        {
            _display = XOpenDisplay(IntPtr.Zero);
            XGetInputFocus(_display, out var window, out _);
            _lastWindowId = window;
        }
        public void RestoreFocus()
        {
            if (_lastWindowId == IntPtr.Zero)
                return;
            XSetInputFocus(_display, _lastWindowId, RevertToParent, IntPtr.Zero);
            XFlush(_display);
        }
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                SetX11NoFocus(this);
            }
        }
        // The following code below is an X11 equivalent to WS_EX_NOACTIVATE
        private void SetX11NoFocus(Window window)
        {
            // Get the underlying X11 Window ID (XID)
            var xid = window.TryGetPlatformHandle()?.Handle;
            if (xid == null || xid == IntPtr.Zero) return;

            IntPtr display = XOpenDisplay(IntPtr.Zero);
            if (display == IntPtr.Zero) return;

            try
            {
                // 1. Set WM_HINTS to tell the WM not to give this window input focus
                IntPtr hintsPtr = XAllocWMHints();
                XWMHints hints = Marshal.PtrToStructure<XWMHints>(hintsPtr);
                
                hints.flags = (IntPtr)InputHint;
                hints.input = 0; // False: Window does not want input focus

                Marshal.StructureToPtr(hints, hintsPtr, false);
                XSetWMHints(display, xid.Value, hintsPtr);
                XFree(hintsPtr);

                // 2. Optional: Set _NET_WM_STATE_SKIP_TASKBAR to make it feel like a tool/overlay
                IntPtr stateAtom = XInternAtom(display, "_NET_WM_STATE", false);
                IntPtr skipTaskbarAtom = XInternAtom(display, "_NET_WM_STATE_SKIP_TASKBAR", false);
                
                XChangeProperty(display, xid.Value, stateAtom, (IntPtr)4, 32, 0, 
                    new IntPtr[] { skipTaskbarAtom }, 1);

                XFlush(display);
            }
            finally
            {
                XCloseDisplay(display);
            }
        }

        #region X11 P/Invoke
        private const string X11Lib = "libX11.so.6";
        private const long InputHint = 1L << 0;

        [DllImport(X11Lib)] private static extern IntPtr XOpenDisplay(IntPtr display);
        [DllImport(X11Lib)] private static extern int XCloseDisplay(IntPtr display);
        [DllImport(X11Lib)] private static extern IntPtr XAllocWMHints();
        [DllImport(X11Lib)] private static extern int XSetWMHints(IntPtr display, IntPtr window, IntPtr hints);
        [DllImport(X11Lib)] private static extern int XFree(IntPtr data);
        [DllImport(X11Lib)] private static extern int XFlush(IntPtr display);
        [DllImport(X11Lib)] private static extern IntPtr XInternAtom(IntPtr display, string name, bool only_if_exists);
        [DllImport(X11Lib)] private static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type, int format, int mode, IntPtr[] data, int nelements);
        [DllImport(X11Lib)] private static extern int XGetInputFocus(IntPtr display, out IntPtr focus, out int revert_to);
        [DllImport("libX11.so.6")]  private static extern int XSetInputFocus(IntPtr display, IntPtr focus, int revert_to, IntPtr time);

        [StructLayout(LayoutKind.Sequential)]
        struct XWMHints
        {
            public IntPtr flags;
            public int input;
            public int initial_state;
            public IntPtr icon_pixmap;
            public IntPtr icon_window;
            public int icon_x;
            public int icon_y;
            public IntPtr icon_mask;
            public IntPtr window_group;
        }
        #endregion
    }
}