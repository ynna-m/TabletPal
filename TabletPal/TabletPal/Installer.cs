using System;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Media;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using TabletPal.Models;
/***
*Needs to be updated to just installing to Home directory in Linux.
**/
namespace TabletPal
{
	public static class Installer
	{
		private static readonly string _preferredDirectory =
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "TabletPal");
        private static string appImagePath = Path.GetDirectoryName(Environment.GetEnvironmentVariable("APPIMAGE"));
        private static string appDir = AppContext.BaseDirectory;

		public async static void TryInstall()
		{
			if (!AppState.Settings.FirstLaunch)
			{
				return;
			}
			if (AppState.CurrentDirectory.TrimEnd('/') == _preferredDirectory.TrimEnd('/'))
			{
				return;
			}
            var theme = new ThemeModel();
            Application.Current.Resources["MaterialPrimaryMidBrush"] = new SolidColorBrush(theme.PrimaryColor);
			Application.Current.Resources["MaterialPrimaryForegroundBrush"] = new SolidColorBrush(theme.SecondaryColor);
			Application.Current.Resources["MaterialPrimaryMidForegroundBrush"] = new SolidColorBrush(theme.SecondaryColor);

			Application.Current.Resources["MaterialDesignPaper"] = new SolidColorBrush(theme.BackgroundColor);
			Application.Current.Resources["MaterialDesignFont"] = new SolidColorBrush(theme.SecondaryColor);
			Application.Current.Resources["MaterialDesignBody"] = new SolidColorBrush(theme.SecondaryColor);

            var box = MessageBoxManager.GetMessageBoxStandard(
                    "Hello!"
                    ,"Welcome to Tablet Pal! See the https://github.com/ynna-m/TabletPal#readme if you have any questions. Would you like to move Tablet Pal to your Home directory?",
                    ButtonEnum.YesNo
                    ,Icon.Question);
            var result = await box.ShowAsync();

			if (result == ButtonResult.Yes)
			{
				try
				{
					if (Directory.Exists(_preferredDirectory))
					{

                        var boxLayoutInfo = MessageBoxManager.GetMessageBoxStandard(
                            "Update"
                            ,"Another version of Tablet Pal detected. 'files' directory will be overwritten. " +
                            "Previous version's layouts, themes and icons will be moved to 'files.backup'. " +
                            Environment.NewLine +
                            "WARNING: If you are updating from 1.0 to 2.0, layout and theme structure has been changed. " +
                            "If you have your own layouts you will need to update them manually. See https://github.com/ynna-m/TabletPal#readme for the instructions."
                            , ButtonEnum.YesNo
                            ,Icon.Info);
                        await boxLayoutInfo.ShowAsync();

						if (Directory.Exists(Path.Combine(_preferredDirectory, "files")))
						{
							DirectoryCopy(
								Path.Combine(_preferredDirectory, "files"),
								Path.Combine(_preferredDirectory, "files.backup")
							);
						}
					}
                    //Needs to be checked and audited
					// DirectoryCopy(AppState.CurrentDirectory, _preferredDirectory, "*.dll");
					// DirectoryCopy(AppState.CurrentDirectory, _preferredDirectory, "*.exe");
					// DirectoryCopy(AppState.CurrentDirectory, _preferredDirectory, "*.json");
					// Process.Start(Path.Combine(_preferredDirectory, "TabletPal.exe"));

                    // DirectoryCopy(Path.Combine(appDir,AppState.FilesRelativePath), Path.Combine(_preferredDirectory, AppState.FilesRelativePath), "*.*");
                    DirectoryCopy(appImagePath, _preferredDirectory, "*.AppImage");
					Process.Start(Path.Combine(_preferredDirectory, "TabletPal*.AppImage"));

				}
				catch (Exception e)
				{
                    var boxError = MessageBoxManager.GetMessageBoxStandard(
                        "Error!",$"Failed to copy the files: {e.Message}. Make sure all other instances of Tablet Pal are closed and try again.",
                        ButtonEnum.Ok, Icon.Error);
                    await boxError.ShowAsync();
				}
				Process.GetCurrentProcess().Kill();
				Environment.Exit(0);
			}
		}


		private static void DirectoryCopy(string sourceDirName, string destDirName, string pattern = "*.*")
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			DirectoryInfo[] dirs = dir.GetDirectories();

			// If the destination directory doesn't exist, create it.       
			Directory.CreateDirectory(destDirName);

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles(pattern);
			foreach (FileInfo file in files)
			{
				string tempPath = Path.Combine(destDirName, file.Name);
				file.CopyTo(tempPath, true);
			}


			foreach (DirectoryInfo subdir in dirs)
			{
				string tempPath = Path.Combine(destDirName, subdir.Name);
				DirectoryCopy(subdir.FullName, tempPath);
			}
		}
	}
}
