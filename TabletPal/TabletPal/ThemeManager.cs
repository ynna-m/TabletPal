// using System.Windows;
using TabletPal.Models;
using TabletPal.Docking;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Threading.Tasks;

namespace TabletPal
{
	public class ThemeManager
	{
		/// <summary>
		/// If all else fails, use this theme.
		/// </summary>
		private static ThemeModel _fallbackTheme = new ThemeModel();
		
		public ThemeManager()
		{
			EventBeacon.Subscribe(Events.FilesChanged, OnFilesChanged);
			EventBeacon.Subscribe(Events.ChangeTheme, OnChangeTheme);
		}


		private void OnFilesChanged(object[] args)
		{
			Dispatcher.UIThread.Invoke(
				()=>
				{
					LoadTheme(AppState.CurrentThemeName);
				}
			);
		}

		private void OnChangeTheme(object[] obj)
		{
			var firstLoad = AppState.CurrentTheme == null;
			var path = (string)obj[0];

			LoadTheme(path);
			if (!firstLoad)
			{
				EventBeacon.SendEvent(Events.UpdateSettings);
			}
		}


		public async void LoadTheme(string path)
		{
			// try
            // {
                if (AppState.Themes.Count == 0)
                {
                    var box = MessageBoxManager.GetMessageBoxStandard(
                        "Load failure!",
                        "No themes found!",
                        ButtonEnum.Ok,
                        Icon.Error
                    );
                    await box.ShowAsync();
                    // MessageBox.Show(
                    // 	"No themes found!",
                    // 	"Load failure!",
                    // 	MessageBoxButton.OK,
                    // 	MessageBoxImage.Error
                    // );
                    return;
                }
                // Console.WriteLine($"ThemeManager.cs - LoadTheme() - Attempting to load theme: {path} {AppState.Themes.Count}");
                if (!AppState.Themes.TryGetValue(path, out var theme)) // TODO: fix the check lul.
                {
                    if (AppState.Themes.ContainsKey("default"))
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard(
                            "Load failure!",
                            $"Cannot load '{path}'! Trying to fall back to default theme.",
                            ButtonEnum.Ok,
                            Icon.Error
                        );
                        await box.ShowAsync();
                        // MessageBox.Show(
                        // 	"Cannot load '" + path + "'! Trying to fall back to default theme.",
                        // 	"Load failure!",
                        // 	MessageBoxButton.OK,
                        // 	MessageBoxImage.Error
                        // );
                        theme = AppState.Themes["default"];
                    }
                    else
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard(
                            "Man you really screwed up",
                            "No default theme found! Make sure you have a valid theme named 'default.yaml'",
                            ButtonEnum.Ok,
                            Icon.Error
                        );
                        await box.ShowAsync();
                        // MessageBox.Show(
                        // 	"No default theme found! Make sure you have a valid theme named 'default.yaml'",
                        // 	"Man you really screwed up",
                        // 	MessageBoxButton.OK,
                        // 	MessageBoxImage.Error
                        // );

                        theme = _fallbackTheme;
                    }
                }

                if (theme == null)
                {
                    return;
                }

                AppState.CurrentTheme = theme;
                AppState.CurrentThemeName = path;
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine($"ThemeManager.cs - LoadTheme() - error: {e.Message}");
            // }
		}

	}
}
