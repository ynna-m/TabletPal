using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using TabletPal.Docking;
using Avalonia;

namespace TabletPal
{
	public class Settings
	{
        // Commented out to let you guys know that I'm not implementing an option here to do autostart. You can do this with your desktop environment manager in Linux
		// public bool AddToAutostart = true;
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

            if (window != null)
            {
                window.Position = new PixelPoint((int)WindowX, (int)WindowY);
            }
            
			
			EventBeacon.SendEvent(Events.DockingChanged, DockingMode);
		}


		private void OnUpdateSettings(object[] obj)
		{
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
            
            if (Application.Current?.ApplicationLifetime 
                is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                var window = desktop.MainWindow;
                if (window != null)
                {
                    WindowX = window.Position.X;
                    WindowY = window.Position.Y;
                }
            }
            Save();			
		}

		public void Save()
		{
            var serializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            File.WriteAllText(SettingsPath, serializer.Serialize(this));
	
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
                    text = File.ReadAllText(SettingsPath)
                        .Replace("\t", "  "); // The thing doesn't like tabs.
                    break;
				}
				AppState.Settings = deserializer.Deserialize<Settings>(text);
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
