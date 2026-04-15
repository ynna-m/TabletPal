using System;
using System.Linq;
using System.Runtime.InteropServices;

public class X11InputExtraction : IDisposable
{
    private IntPtr _display;

    public X11InputExtraction()
    {
        _display = XOpenDisplay(IntPtr.Zero);
        if (_display == IntPtr.Zero)
            throw new Exception("Cannot open X display");
    }

    public void Dispose()
    {
        if (_display != IntPtr.Zero)
        {
            XCloseDisplay(_display);
            _display = IntPtr.Zero;
        }
    }

    // ---------------------------
    // KEYBOARD
    // ---------------------------

    public void KeyDown(string[] keys)
    {
        foreach (var key in keys)
        {
            var keycode = GetKeycode(key);
            XTestFakeKeyEvent(_display, keycode, true, IntPtr.Zero);
        }

        XFlush(_display);
    }

    public void KeyUp(string[] keys)
    {
        for (int i = keys.Length - 1; i >= 0; i--)
        {
            var keycode = GetKeycode(keys[i]);
            XTestFakeKeyEvent(_display, keycode, false, IntPtr.Zero);
        }

        XFlush(_display);
    }

    public void KeyPress(string key)
    {
        var keys = new string[] { key };
        KeyDown(keys);
        KeyUp(keys);
    }
    public void KeyChord(string[] keys)
    {
        if (keys.Length == 0)
                return;

        var modifiers = keys.Take(keys.Length - 1).ToArray();
        var mainKey = keys.Last();

        // Press modifiers
        foreach (var mod in modifiers)
        {
            var keycode = GetKeycode(mod);
            XTestFakeKeyEvent(_display, keycode, true, IntPtr.Zero);
        }

        // Press main key
        var mainCode = GetKeycode(mainKey);
        XTestFakeKeyEvent(_display, mainCode, true, IntPtr.Zero);
        XTestFakeKeyEvent(_display, mainCode, false, IntPtr.Zero);

        // Release modifiers (reverse)
        for (int i = modifiers.Length - 1; i >= 0; i--)
        {
            var keycode = GetKeycode(modifiers[i]);
            XTestFakeKeyEvent(_display, keycode, false, IntPtr.Zero);
        }

        XFlush(_display);
    }
    // ---------------------------
    // MOUSE
    // ---------------------------

    public void MouseMove(int x, int y)
    {
        XTestFakeMotionEvent(_display, -1, x, y, IntPtr.Zero);
        XFlush(_display);
    }

    public void MouseClick(int button)
    {
        XTestFakeButtonEvent(_display, button, true, IntPtr.Zero);
        XTestFakeButtonEvent(_display, button, false, IntPtr.Zero);
        XFlush(_display);
    }

    // ---------------------------
    // HELPERS
    // ---------------------------

    private uint GetKeycode(string key)
    {
        var keysym = XStringToKeysym(key);
        if (keysym == IntPtr.Zero)
            throw new Exception($"Invalid key: {key}");

        return XKeysymToKeycode(_display, keysym);
    }

    // ---------------------------
    // P/Invoke
    // ---------------------------

    [DllImport("libX11.so.6")]
    private static extern IntPtr XOpenDisplay(IntPtr display);

    [DllImport("libX11.so.6")]
    private static extern int XCloseDisplay(IntPtr display);

    [DllImport("libX11.so.6")]
    private static extern IntPtr XStringToKeysym(string str);

    [DllImport("libX11.so.6")]
    private static extern uint XKeysymToKeycode(IntPtr display, IntPtr keysym);

    [DllImport("libX11.so.6")]
    private static extern int XFlush(IntPtr display);

    [DllImport("libXtst.so.6")]
    private static extern int XTestFakeKeyEvent(IntPtr display, uint keycode, bool is_press, IntPtr delay);

    [DllImport("libXtst.so.6")]
    private static extern int XTestFakeButtonEvent(IntPtr display, int button, bool is_press, IntPtr delay);

    [DllImport("libXtst.so.6")]
    private static extern int XTestFakeMotionEvent(IntPtr display, int screen_number, int x, int y, IntPtr delay);
}