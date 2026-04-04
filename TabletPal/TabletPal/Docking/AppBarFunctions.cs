// /////////////////////////////////////////////////////////////
// /// All credit goes to https://github.com/beavis28/AppBar
// /// You are my savior.
// /////////////////////////////////////////////////////////////

// using System;
// using System.Collections.Generic;
// using System.Runtime.CompilerServices;
// using System.Runtime.InteropServices;
// // using System.Windows;
// // using System.Windows.Interop;
// // using System.Windows.Threading;
// using Avalonia.Controls;

// namespace WpfAppBar
// {
// 	public enum DockingMode : int
// 	{
// 		Left = 0,
// 		Top,
// 		Right,
// 		Bottom,
// 		None
// 	}

// 	public static class AppBarFunctions
// 	{
// 		private class RegisterInfo
// 		{
// 			public int CallbackId { get; set; }
// 			public bool IsRegistered { get; set; }
// 			public Window Window { get; set; }
// 			public DockingMode Edge { get; set; }
// 			public WindowStyle OriginalStyle { get; set; }
// 			public Point OriginalPosition { get; set; }
// 			public Size OriginalSize { get; set; }
// 			public ResizeMode OriginalResizeMode { get; set; }


// 			public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam,
// 									IntPtr lParam, ref bool handled)
// 			{
// 				if (msg == CallbackId)
// 				{
// 					if (wParam.ToInt32() == (int)Interop.ABNotify.ABN_POSCHANGED)
// 					{
// 						ABSetPos(Edge, Window);
// 						handled = true;
// 					}
// 				}
// 				return IntPtr.Zero;
// 			}

// 		}
// 		private static readonly Dictionary<Window, RegisterInfo> RegisteredWindowInfo
// 			= new Dictionary<Window, RegisterInfo>();
// 		private static RegisterInfo GetRegisterInfo(Window appbarWindow)
// 		{
// 			RegisterInfo reg;
// 			if (RegisteredWindowInfo.ContainsKey(appbarWindow))
// 			{
// 				reg = RegisteredWindowInfo[appbarWindow];
// 			}
// 			else
// 			{
// 				reg = new RegisterInfo()
// 				{
// 					CallbackId = 0,
// 					Window = appbarWindow,
// 					IsRegistered = false,
// 					Edge = DockingMode.Top,
// 					OriginalStyle = appbarWindow.WindowStyle,
// 					OriginalPosition = new Point(appbarWindow.Left, appbarWindow.Top),
// 					OriginalSize =
// 						new Size(appbarWindow.ActualWidth, appbarWindow.ActualHeight),
// 					OriginalResizeMode = appbarWindow.ResizeMode,
// 				};
// 				RegisteredWindowInfo.Add(appbarWindow, reg);
// 			}
// 			return reg;
// 		}

// 		private static void RestoreWindow(Window appbarWindow)
// 		{
// 			var info = GetRegisterInfo(appbarWindow);

// 			appbarWindow.WindowStyle = info.OriginalStyle;
// 			appbarWindow.ResizeMode = info.OriginalResizeMode;
// 			//appbarWindow.Topmost = false;

// 			var rect = new Rect(info.OriginalPosition.X, info.OriginalPosition.Y,
// 				info.OriginalSize.Width, info.OriginalSize.Height);
// 			appbarWindow.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
// 					new ResizeDelegate(DoResize), appbarWindow, rect);

// 		}

// 		private static DockingMode _currentEdge = DockingMode.None;

// 		public static void SetAppBar(Window appbarWindow, DockingMode edge, bool forceRedock = false)
// 		{
// 			if (_currentEdge == edge && !forceRedock)
// 			{
// 				return;
// 			}

// 			var info = GetRegisterInfo(appbarWindow);
// 			info.Edge = edge;
// 			_currentEdge = edge;

// 			var abd = new Interop.APPBARDATA();
// 			abd.cbSize = Marshal.SizeOf(abd);
// 			abd.hWnd = new WindowInteropHelper(appbarWindow).Handle;

// 			if (abd.hWnd.ToInt32() == 0)
// 			{
// 				return;
// 			}

// 			int renderPolicy;

// 			if (edge == DockingMode.None)
// 			{
// 				if (info.IsRegistered)
// 				{
// 					Interop.SHAppBarMessage((int)Interop.ABMsg.ABM_REMOVE, ref abd);
// 					info.IsRegistered = false;
// 				}
// 				//RestoreWindow(appbarWindow);

// 				// Restore normal desktop window manager attributes
// 				renderPolicy = (int)Interop.DWMNCRenderingPolicy.UseWindowStyle;

// 				Interop.DwmSetWindowAttribute(abd.hWnd, (int)Interop.DWMWINDOWATTRIBUTE.DWMA_EXCLUDED_FROM_PEEK, ref renderPolicy, sizeof(int));
// 				Interop.DwmSetWindowAttribute(abd.hWnd, (int)Interop.DWMWINDOWATTRIBUTE.DWMA_DISALLOW_PEEK, ref renderPolicy, sizeof(int));

// 				return;
// 			}

// 			if (!info.IsRegistered)
// 			{
// 				info.IsRegistered = true;
// 				info.CallbackId = Interop.RegisterWindowMessage("AppBarMessage");
// 				abd.uCallbackMessage = info.CallbackId;

// 				var ret = Interop.SHAppBarMessage((int)Interop.ABMsg.ABM_NEW, ref abd);

// 				var source = HwndSource.FromHwnd(abd.hWnd);
// 				source.AddHook(info.WndProc);
// 			}

// 			appbarWindow.WindowStyle = WindowStyle.None;
// 			appbarWindow.ResizeMode = ResizeMode.NoResize;
// 			appbarWindow.Topmost = true;

// 			// Set desktop window manager attributes to prevent window
// 			// from being hidden when peeking at the desktop or when
// 			// the 'show desktop' button is pressed
// 			renderPolicy = (int)Interop.DWMNCRenderingPolicy.UseWindowStyle;
// 			//renderPolicy = (int)Interop.DWMNCRenderingPolicy.Enabled; // From vanilla code, but works worse. No idea why. :D

// 			Interop.DwmSetWindowAttribute(abd.hWnd, (int)Interop.DWMWINDOWATTRIBUTE.DWMA_EXCLUDED_FROM_PEEK, ref renderPolicy, sizeof(int));
// 			Interop.DwmSetWindowAttribute(abd.hWnd, (int)Interop.DWMWINDOWATTRIBUTE.DWMA_DISALLOW_PEEK, ref renderPolicy, sizeof(int));

// 			ABSetPos(info.Edge, appbarWindow);
// 		}

// 		private delegate void ResizeDelegate(Window appbarWindow, Rect rect);
// 		private static void DoResize(Window appbarWindow, Rect rect)
// 		{
// 			appbarWindow.Width = rect.Width;
// 			appbarWindow.Height = rect.Height;
// 			appbarWindow.Top = rect.Top;
// 			appbarWindow.Left = rect.Left;
// 		}



// 		private static void ABSetPos(DockingMode edge, Window appbarWindow)
// 		{
// 			var barData = new Interop.APPBARDATA();
// 			barData.cbSize = Marshal.SizeOf(barData);
// 			barData.hWnd = new WindowInteropHelper(appbarWindow).Handle;
// 			barData.uEdge = (int)edge;

// 			// Transforms a coordinate from WPF space to Screen space
// 			var toPixel = PresentationSource.FromVisual(appbarWindow).CompositionTarget.TransformToDevice;
// 			// Transforms a coordinate from Screen space to WPF space
// 			var toWpfUnit = PresentationSource.FromVisual(appbarWindow).CompositionTarget.TransformFromDevice;

// 			// Transform window size from wpf units (1/96 ") to real pixels, for win32 usage
// 			var sizeInPixels = toPixel.Transform(new Vector(appbarWindow.ActualWidth, appbarWindow.ActualHeight));
// 			// Even if the documentation says SystemParameters.PrimaryScreen{Width, Height} return values in 
// 			// "pixels", they return wpf units instead.
// 			var screenSizeInPixels =
// 				toPixel.Transform(new Vector(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight));

// 			if (barData.uEdge == (int)DockingMode.Left || barData.uEdge == (int)DockingMode.Right)
// 			{
// 				barData.rc.top = 0;
// 				barData.rc.bottom = (int)screenSizeInPixels.Y;
// 				if (barData.uEdge == (int)DockingMode.Left)
// 				{
// 					barData.rc.left = 0;
// 					barData.rc.right = (int)Math.Round(sizeInPixels.X);
// 				}
// 				else
// 				{
// 					barData.rc.right = (int)screenSizeInPixels.X;
// 					barData.rc.left = barData.rc.right - (int)Math.Round(sizeInPixels.X);
// 				}
// 			}
// 			else
// 			{
// 				barData.rc.left = 0;
// 				barData.rc.right = (int)screenSizeInPixels.X;
// 				if (barData.uEdge == (int)DockingMode.Top)
// 				{
// 					barData.rc.top = 0;
// 					barData.rc.bottom = (int)Math.Round(sizeInPixels.Y);
// 				}
// 				else
// 				{
// 					barData.rc.bottom = (int)screenSizeInPixels.Y;
// 					barData.rc.top = barData.rc.bottom - (int)Math.Round(sizeInPixels.Y);
// 				}
// 			}

// 			Interop.SHAppBarMessage((int)Interop.ABMsg.ABM_QUERYPOS, ref barData);
// 			Interop.SHAppBarMessage((int)Interop.ABMsg.ABM_SETPOS, ref barData);

// 			// transform back to wpf units, for wpf window resizing in DoResize. 
// 			var location = toWpfUnit.Transform(new Point(barData.rc.left, barData.rc.top));
// 			var dimension = toWpfUnit.Transform(new Vector(barData.rc.right - barData.rc.left,
// 				barData.rc.bottom - barData.rc.top));

// 			var rect = new Rect(location, new Size(dimension.X, dimension.Y));

// 			//This is done async, because WPF will send a resize after a new appbar is added.  
// 			//if we size right away, WPFs resize comes last and overrides us.
// 			appbarWindow.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
// 				new ResizeDelegate(DoResize), appbarWindow, rect);
// 		}
// 	}
// }
using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;

namespace TabletPal.Docking
{
    public enum DockingMode
    {
        Left,
        Top,
        Right,
        Bottom,
        None
    }

    public static class AppBarFunctions
    {
        private class WindowState
        {
            public PixelPoint Position { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public bool Topmost { get; set; }
            public DockingMode PreviousDockingMode { get; set; }
        }

        private static readonly Dictionary<Window, WindowState> _states = new();

        private static WindowState GetState(Window window)
        {
            if (_states.TryGetValue(window, out var state))
                return state;

            state = new WindowState
            {
                Position = window.Position,
                Width = window.Width,
                Height = window.Height,
                Topmost = true
            };

            _states[window] = state;
            return state;
        }

        public static void SetAppBar(Window window, DockingMode mode)
        {
            if (mode == DockingMode.None)
            {
                Undock(window);
                return;
            }

            var screen = window.Screens.ScreenCount > 1
                ? window.Screens.ScreenFromPoint(window.Position)
                : window.Screens.Primary;
            if (screen == null) return;

            var bounds = screen.Bounds;

            // Save original state before docking
            GetState(window);
            // Save new position on every dock, to make sure we can restore to the last position when undocking, even after multiple docks.
            if(_states[window].PreviousDockingMode == DockingMode.None){
                _states[window].Position = window.Position;
            }

            _states[window].PreviousDockingMode = mode;
            // Make it look like a dock
            window.SystemDecorations = SystemDecorations.None;
            window.Topmost = true;


            Console.WriteLine($"AppBarFunctions.cs - SetAppBar() - Get Window State Height {_states[window].Height.ToString()} {window.Height.ToString()}");

            switch (mode)
            {
                case DockingMode.Top:
                    window.Position = new PixelPoint(bounds.X, bounds.Y);
                    window.Width = bounds.Width;
                    break;

                case DockingMode.Bottom:
                    window.Position = new PixelPoint(bounds.X, bounds.Bottom - (int)window.Height);
                    window.Width = bounds.Width;
                    break;

                case DockingMode.Left:
                    window.Position = new PixelPoint(bounds.X, bounds.Y);
                    window.Height = bounds.Height;
                    break;

                case DockingMode.Right:
                    window.Position = new PixelPoint(bounds.Right - (int)window.Width, bounds.Y);
                    window.Height = bounds.Height;
                    break;
            }
        }

        public static void Undock(Window window)
        {
            if (!_states.TryGetValue(window, out var state))
                return;
            Console.WriteLine($"AppBarFunctions.cs - Undock() - Height {state.Height.ToString()}");
            window.SystemDecorations = SystemDecorations.None;
            window.Topmost = true;

            window.Position = state.Position;
            window.Width = state.Width;
            window.Height = state.Height;
            state.PreviousDockingMode = DockingMode.None;
        }
                public static void OnChangeLayoutHeight(MainWindow window, double height)
        {
            if (!_states.TryGetValue(window, out var state))
                return;
            if(state.Height != height)
            {
                state.Height = height;
            }
        }
        public static void OnChangeLayoutWidth(MainWindow window, double width)
        {
            if (!_states.TryGetValue(window, out var state))
                return;
            if(state.Width != width)
            {
                state.Width = width;
            }
        }
    }
}