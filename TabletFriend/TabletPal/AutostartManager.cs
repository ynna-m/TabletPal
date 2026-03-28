/* *
* Unneeded. Linux users can just add the app via their preferred Desktop Environment's autostart manager.
**/
// using Microsoft.Win32;
// using System.IO;
// using System.Reflection;

// namespace TabletPal
// {
// 	public class AutostartManager
// 	{
// 		private const string _key = "TabletPal";

// 		private static readonly string _appPath =
// 			'"' + Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "TabletPal.exe") + '"';

// 		public static bool IsAutostartSet => (string)GetKey().GetValue(_key) == _appPath;

// 		public static void SetAutostart()
// 		{
// 			if (!IsAutostartSet)
// 			{
// 				GetKey().SetValue(_key, _appPath);
// 			}
// 		}

// 		public static void ResetAutostart()
// 		{
// 			if (IsAutostartSet)
// 			{
// 				GetKey().DeleteValue(_key);
// 			}
// 		}

// 		private static RegistryKey GetKey() =>
// 			Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
// 	}
// }
