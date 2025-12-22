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
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using SmartLogging;
using SmartLogViewer.Core;
using static System.IO.Path;

namespace SmartLogViewer;

public partial class App : Application
{
    private static readonly SmartLogger Log;
    private const string TEMP = ".tmp.dll";

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

    public App()
    {
        var (dir, name) = GetExeInfo();
        Log.Information($"Starting {Combine(dir, name)}");

        if (name.EndsWith(TEMP))
        {
            name = name.Replace(TEMP, "");
            var realFile = Path.Combine(dir, name);
            Start(realFile);
            Shutdown();
        }
        else
        {
            var tempFile = Path.Combine(dir, name + TEMP);
            DeleteFile(tempFile);
        }
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (Settings.ThemeModeIndex < 4)
        {
            Application.Current.ThemeMode = Settings.ThemeModeIndex switch
            {
                0 => ThemeMode.Light,
                1 => ThemeMode.Dark,
                2 => ThemeMode.System,
                _ => ThemeMode.None,
            };

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

    public static void UpdateThemeMode()
    {
        Settings.Store();

        var (dir, name) = GetExeInfo();
        var tempFile = Path.Combine(dir, name + TEMP);
        DeleteFile(tempFile);

        if (CopyFile(Combine(dir, name), tempFile))
        {
            Start(tempFile);
            Application.Current.Shutdown(0);
        }
    }

    public static Version Version { get; }

    public static AppSettings Settings { get; }

    private static (string Directory, string Name) GetExeInfo()
    {
        var path = Assembly.GetExecutingAssembly().Location;
        return (GetDirectoryName(path)!, GetFileName(path));
    }

    private static void Start(string path)
    {
        Log.Information($"About to start {path}");
        using var process = new Process();
        process.StartInfo.FileName = "dotnet.exe";
        process.StartInfo.Arguments = $"{path}";
        process.StartInfo.UseShellExecute = true;
        process.Start();
        Log.Information(new { process.HasExited });
    }

    private static bool CopyFile(string source, string dest)
    {
        Log.Information(new { source, dest });

        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                if (TryCopyFile(source, dest))
                {
                    Log.Information("success");
                    return true;
                }

                Thread.Sleep(60);
            }

            var msg = string.Format("could not copy file '{0}' to target '{1}'. Check if target is running. Try again?", source, dest);
            var res = MessageBox.Show(msg, "Copy File Error", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res != MessageBoxResult.Yes)
                break;
        }

        Log.Information("cancelled");
        return false;
    }

    private static bool TryCopyFile(string source, string dest)
    {
        try
        {
            File.Copy(source, dest, true);
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }

        return false;
    }

    private static bool DeleteFile(string path)
    {
        if (!File.Exists(path))
            return true;

        Log.Information(new { path });

        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                if (TryDeleteFile(path))
                {
                    Log.Information("success");
                    return true;
                }

                Thread.Sleep(60);
            }

            string msg = string.Format("could not delete file '{0}'. Check if file is running. Try again?", path);
            MessageBoxResult res = MessageBox.Show(msg, "Delete File Error", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res != MessageBoxResult.Yes)
                break;
        }

        Log.Information("cancelled");
        return false;
    }

    private static bool TryDeleteFile(string path)
    {
        try
        {
            File.Delete(path);
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }

        return false;
    }
}
