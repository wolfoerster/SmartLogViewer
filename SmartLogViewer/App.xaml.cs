//******************************************************************************************
// Copyright © 2017 - 2026 Wolfgang Foerster (wolfoerster@gmx.de)
//
// This file is part of the SmartLogViewer project which can be found on github.com.
//
// SmartLogViewer is free software: you can redistribute it and/or modify it under the terms 
// of the GNU General Public License as published by the Free Software Foundation, 
// either version 3 of the License, or (at your option) any later version.
// 
// SmartLogViewer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//******************************************************************************************

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using SmartLogging;
using SmartLogViewer.Core;
using SmartLogViewer.Models;

namespace SmartLogViewer;

public partial class App : Application
{
    private static readonly SmartLogger Log;

    static App()
    {
        ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(UIElement), new FrameworkPropertyMetadata(30000));
        ToolTipService.ShowOnDisabledProperty.OverrideMetadata(typeof(UIElement), new FrameworkPropertyMetadata(true));

        var assemblyName = Assembly.GetExecutingAssembly().GetName();
        Version = assemblyName?.Version ?? new Version(0, 0, 0);
        Settings = Helper.Restore<AppSettings>();

        LogWriter.Init(new LogSettings 
        { 
            LogToFile = true, 
            MaxLogFileSize = 4 * 1024 * 1024,
            MinimumLogLevel = Settings.LogLevel,
        });

        Log = new SmartLogger();
        Log.Information(new { Settings });
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (Settings.ThemeModeIndex < 4)
        {
            Application.Current.ThemeMode = GetThemeMode();
            return;
        }

        Application.Current.ThemeMode = ThemeMode.None;

        var theme = Settings.ThemeModeIndex switch
        {
            4 => "PresentationFramework.Aero;V3.0.0.0;31bf3856ad364e35;component\\themes/aero.normalcolor.xaml",
            5 => "/PresentationFramework.Royale;v3.0.0.0;31bf3856ad364e35;Component/themes/royale.normalcolor.xaml",
            6 => "/PresentationFramework.Luna;v3.0.0.0;31bf3856ad364e35;Component/themes/luna.normalcolor.xaml",
            7 => "/PresentationFramework.Luna;v3.0.0.0;31bf3856ad364e35;Component/themes/luna.homestead.xaml",
            8 => "/PresentationFramework.Luna;v3.0.0.0;31bf3856ad364e35;Component/themes/luna.metallic.xaml",
            _ => "/PresentationFramework.Classic;v3.0.0.0;31bf3856ad364e35;Component/themes/classic.xaml",
        };

        var uri = new Uri(theme, UriKind.Relative);
        Resources.MergedDictionaries.Add(Application.LoadComponent(uri) as ResourceDictionary);
    }

    private static ThemeMode GetThemeMode() => Settings.ThemeModeIndex switch
    {
        0 => ThemeMode.Light,
        1 => ThemeMode.Dark,
        2 => ThemeMode.System,
        _ => ThemeMode.None,
    };

    public static void UpdateThemeMode()
    {
        //var msg = "You have to restart the application!\r\n\r\nRestart now?";
        //var res = MessageBox.Show(msg, "Switching Theme", MessageBoxButton.YesNo, MessageBoxImage.Question);
        //if (res == MessageBoxResult.Yes)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            var starter = Path.Combine(dir, "Starter.exe");
            using var process = new Process();
            process.StartInfo.FileName = starter;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = true;
            process.Start();

            Application.Current.Shutdown();
        }
    }

    public static Version Version { get; }

    public static AppSettings Settings { get; }
}
