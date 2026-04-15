// /////////////////////////////////////////////////////////////
// /// OLD COMMENT:
// /// All credit goes to https://github.com/beavis28/AppBar
// /// You are my savior.
// ////////////////////////////////////////////////////////////
// /// NEW COMMENT:
// /// The code below is generated mostly by ChatGPT except for
// /// the last two functions. The last two functions ensure that 
// /// AppBarFunction state gets updated when starting up app as 
// /// docked position. Otherwise, undocking would set the height
// /// to what is set in MainWindow.axaml.
// /// I could've probably coded something better, but I'm too
// /// lazy.
// /// Also, one more thing, the docking experience in Linux
// /// isn't the same as Windows. It's difficult to dock as in
// /// move whatever taskbar and window you have full screened 
// /// open in Linux since environment varies, e.g. there is 
// /// KDE, GNOME, etc.
// /////////////////////////////////////////////////////////////

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;

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