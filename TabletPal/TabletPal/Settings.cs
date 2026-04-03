using System;
using System.IO;
using System.Windows;
// using WpfAppBar;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using TabletPal.Docking;
using Avalonia;
using Avalonia.Controls;

namespace TabletPal
{
	public class Settings
	{
		public bool AddToAutostart = true;
		public int WindowX = 0;
		public int WindowY = 0;
		public string Layout = "default";
		public string Theme = "default";
		public DockingMode DockingMode = DockingMode.None;
		
		public bool FirstLaunch = true;

		public bool UpdateCheckingEnabled = true;
		public bool ToolbarAutohideEnabled = false;
		public bool PerAppLayoutsEnabled = true;

		public Settings()
		{
			EventBeacon.Subscribe(Events.UpdateSettings, OnUpdateSettings);
		}

		public void Apply(MainWindow window)
		{
			if (AppState.CurrentTheme == null || Theme != AppState.CurrentThemeName)
			{
				EventBeacon.SendEvent(Events.ChangeTheme, Theme);
			}
			if (AppState.CurrentLayout == null || Layout != AppState.CurrentLayoutName)
			{
				EventBeacon.SendEvent(Events.ChangeLayout, Layout);
			}
			// Application.Current.MainWindow.Left = WindowX;
			// Application.Current.MainWindow.Top = WindowY;

            if (window != null)
            {
                window.Position = new PixelPoint((int)WindowX, (int)WindowY);
                Console.WriteLine($"Settings.cs - Apply() - Get WindowPosition{WindowX} {WindowY}");
            }
            Console.WriteLine($"Settings.cs - Apply() - Get WindowPosition{WindowX} {WindowY}");
            
			
			EventBeacon.SendEvent(Events.DockingChanged, DockingMode);
		}


		private void OnUpdateSettings(object[] obj)
		{
            // try
            // {
                // Console.WriteLine($"Settings.cs - OnUpdateSettings() - Updating settings... {obj.Length} {AppState.CurrentLayoutName} args");
                FirstLaunch = false;
                if (AppState.LastManuallySetLayout == null)
                {
                    Layout = Path.GetRelativePath(AppState.CurrentDirectory, AppState.CurrentLayoutName);
                }
                else
                {
                    Layout = Path.GetRelativePath(AppState.CurrentDirectory, AppState.LastManuallySetLayout);
                }
                Theme = Path.GetRelativePath(AppState.CurrentDirectory, AppState.CurrentThemeName);
                // Console.WriteLine($"Settings.cs - OnUpdateSettings() - Theme {Theme} CurrentThemeName {AppState.CurrentThemeName} args");
                // if (!double.IsNaN(Application.Current.MainWindow.Left))
                // {
                // 	WindowX = Application.Current.MainWindow.Left;
                // }
                // if (!double.IsNaN(Application.Current.MainWindow.Top))
                // {
                // 	WindowY = Application.Current.MainWindow.Top;
                // }
                if (Application.Current?.ApplicationLifetime 
                    is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var window = desktop.MainWindow;
                    if (window != null)
                    {
                        Console.WriteLine($"Settings.cs - OnUpdateSettings() - Get Window Position {window.Position.ToString()}");
                        WindowX = window.Position.X;
                        WindowY = window.Position.Y;
                    }
                }
                Save();
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine($"Settings.cs - OnUpdateSettings() - error: {e.Message}");
            // }
			
		}

		public void Save()
		{
            try
            {
                // Console.WriteLine("Settings.cs - Save() - Saving settings...");
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
                    .Build();

                File.WriteAllText(SettingsPath, serializer.Serialize(this));
            }
            catch (Exception e)
            {
                // Console.WriteLine($"Settings.cs - Save() - error: {e.Message}");
                // throw;
            }
			
		}


		public static readonly string SettingsPath =
			Path.Combine(AppState.CurrentDirectory, "settings.yaml");

		public static void Load(MainWindow window)
		{
			var deserializer = new DeserializerBuilder()
				.WithNamingConvention(UnderscoredNamingConvention.Instance)
				.Build();

			try
			{
				string text = null;
				for (var i = 0; i < 32; i += 1)
				{
					try
					{
						text = File.ReadAllText(SettingsPath)
							.Replace("\t", "  "); // The thing doesn't like tabs.
						break;
					}
					catch (Exception e)
					{
                        Console.WriteLine($"Settings.cs - Load() - error: {e.Message}");
					}
				}
                Console.WriteLine($"Settings.cs - Load() - text string {text}");
				AppState.Settings = deserializer.Deserialize<Settings>(text);
                Console.WriteLine($"Settings.cs - Load() - AppState Settings Window Position {AppState.Settings.WindowX} {AppState.Settings.WindowY}");

			}
			catch
			{
				AppState.Settings = new Settings();
			}

			// Maintaining backwards compatibility.
			AppState.Settings.Layout = Path.GetFileNameWithoutExtension(AppState.Settings.Layout);
			AppState.Settings.Theme = Path.GetFileNameWithoutExtension(AppState.Settings.Theme);

			AppState.Settings.Apply(window);
		}
	}
}
