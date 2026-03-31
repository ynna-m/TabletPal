using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Controls.ApplicationLifetimes;
using System.Collections.Generic;
using TabletPal.Models;

namespace TabletPal;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // InitializeAppState();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }


        // private void InitializeAppState()
        // {
        //     // Initialize collections to prevent null crashes
        //     AppState.Layouts = new Dictionary<string, LayoutModel>();
        //     AppState.Themes = new Dictionary<string, ThemeModel>();

            

        //     // Set defaults if still null
        //     if (AppState.CurrentTheme == null)
        //     {
        //         AppState.CurrentTheme = new ThemeModel();
        //     }

        //     if (AppState.CurrentLayout == null)
        //     {
        //         AppState.CurrentLayout = new LayoutModel();
        //     }
            
        //     // Load settings
        //     Settings settings = new Settings();
        //     settings.Save();
        //     Settings.Load();

        //     // Then load themes/layouts
        //     var themeManager = new ThemeManager();
        //     var layoutManager = new LayoutManager();
        // }
}