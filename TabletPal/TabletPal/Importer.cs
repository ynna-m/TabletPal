using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
// using System.Windows;
// using System.Windows.Controls.Primitives;
using TabletPal.Data;
using TabletPal.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace TabletPal
{
	public static class Importer
	{
		private static readonly IDeserializer _deserializer = new DeserializerBuilder()
			.WithNamingConvention(UnderscoredNamingConvention.Instance)
			.IgnoreUnmatchedProperties()
			.Build();


		public static async Task<Dictionary<string, LayoutModel>> ImportLayouts()
		{
			var items = new Dictionary<string, LayoutModel>();
			foreach (var file in Directory.GetFiles(AppState.LayoutsRoot, AppState.ConfigExtension))
			{
				var layout = await ImportLayout(file);
				if (layout != null)
				{
					items.Add(Path.GetFileNameWithoutExtension(file), layout);				
				}
			}

			return items;
		}

		public async static Task<Dictionary<string, ThemeModel>> ImportThemes()
		{
			var items = new Dictionary<string, ThemeModel>();
			foreach (var file in Directory.GetFiles(AppState.ThemesRoot, AppState.ConfigExtension))
			{
				var theme = await ImportTheme(file);
				if (theme != null)
				{
					items.Add(Path.GetFileNameWithoutExtension(file), theme);
				}
			}

			return items;
		}

		private async static Task<LayoutModel> ImportLayout(string path)
		{
			try
			{
				var data = await Import<LayoutData>(path);

				if (data == null)
				{
					throw new Exception("Layout import failed!");
				}

				return new LayoutModel(data);
			}
			catch (Exception e)
			{
                var box = MessageBoxManager.GetMessageBoxStandard(
                    "Load failure!",$"Cannot load '{path}': {e.Message}",
                    ButtonEnum.Ok, Icon.Error);
                await box.ShowAsync();
				// MessageBox.Show(
				// 	"Cannot load '" + path + "': " + e.Message,
				// 	"Load failure!",
				// 	MessageBoxButton.OK,
				// 	MessageBoxImage.Error
				// );
			}
			return null;
		}

		private async static Task<ThemeModel> ImportTheme(string path)
		{
			try
			{
				var data = await Import<ThemeData>(path);

				return new ThemeModel(data);
			}
			catch (Exception e)
			{
                var box = MessageBoxManager.GetMessageBoxStandard(
                    "Load failure!",$"Cannot load '{path}': {e.Message}",
                    ButtonEnum.Ok, Icon.Error);
                await box.ShowAsync();
				// MessageBox.Show(
				// 	"Cannot load '" + path + "': " + e.Message,
				// 	"Load failure!",
				// 	MessageBoxButton.OK,
				// 	MessageBoxImage.Error
				// );
			}
			return null;
		}


		private async static Task<T> Import<T>(string path)
		{
			string layout = null;

			if (!File.Exists(path))
			{
                var box = MessageBoxManager.GetMessageBoxStandard(
                    "File not found!",$"'{path}' does not exist!",
                    ButtonEnum.Ok, Icon.Error);
                await box.ShowAsync();
				// MessageBox.Show(
				// 	"'" + path + "' does not exist!",
				// 	"File not found!",
				// 	MessageBoxButton.OK,
				// 	MessageBoxImage.Error
				// );

				return default(T);
			}

			for (var i = 0; i < 32; i += 1)
			{
				try
				{
					layout = File.ReadAllText(path)
						.Replace("\t", "  "); // The thing doesn't like tabs.

					if (!string.IsNullOrEmpty(layout))
					{
						break;
					}
				}
				catch
				{
					Thread.Sleep(100);
				}
			}

			var data = _deserializer.Deserialize<T>(layout);
			if (data == null)
			{
				throw new Exception("Failed to parse yaml!");
			}
			return data;
		}
	}
}
