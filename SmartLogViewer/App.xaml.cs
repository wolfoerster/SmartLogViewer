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
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using SmartLogging;
using SmartLogViewer.Core;

namespace SmartLogViewer;

public partial class App : Application
{
    private static readonly Dictionary<int, ThemeMode> ThemeModes = new ()
    {
        {0, ThemeMode.Light },
        {1, ThemeMode.Dark },
        {2, ThemeMode.System },
        {3, ThemeMode.None },
    };

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
    }

    public static Version Version { get; }

    public static AppSettings Settings { get; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        UpdateThemeMode();
    }

    public static void UpdateThemeMode()
    {
        Application.Current.ThemeMode = ThemeModes[Settings.ThemeModeIndex];
    }
}
