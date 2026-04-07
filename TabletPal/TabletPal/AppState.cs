using System;
using System.Collections.Generic;
using System.IO;
using TabletPal.Models;

namespace TabletPal
{
	public static class AppState
	{
		 public static readonly string CurrentDirectory = Path.GetDirectoryName(Environment.GetEnvironmentVariable("APPIMAGE"));
        public static readonly string FilesRelativePath = "files";
		public static readonly string LayoutsRelativePath = "files/layouts";
		public static readonly string ThemesRelativePath = "files/themes";

		public static readonly string FilesRoot = Path.Combine(CurrentDirectory, FilesRelativePath);
		public static readonly string LayoutsRoot = Path.Combine(CurrentDirectory, LayoutsRelativePath);
		public static readonly string ThemesRoot = Path.Combine(CurrentDirectory, ThemesRelativePath);

		public const string ConfigExtension = "*.yaml";

		public static Dictionary<string, LayoutModel> Layouts;
		public static Dictionary<string, ThemeModel> Themes;

		public static LayoutModel CurrentLayout;
		public static string CurrentLayoutName;

		/// <summary>
		/// Keeps track of which layout was set by user and not autoswitched to.
		/// </summary>
		public static string LastManuallySetLayout;

		public static ThemeModel CurrentTheme;
		public static string CurrentThemeName;

		public static Settings Settings;
	}
}
