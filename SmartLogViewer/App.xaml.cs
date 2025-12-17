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
using System.Configuration;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using SmartLogging;

namespace SmartLogViewer;

public partial class App : Application
{
    private static readonly SmartLogger Log;

    static App()
    {
        OverrideMetadata(ToolTipService.ShowDurationProperty, 30000);
        OverrideMetadata(ToolTipService.ShowOnDisabledProperty, true);

        var assembly = Assembly.GetExecutingAssembly();
        var name = assembly.GetName();
        Version = name.Version ?? new Version(0, 0, 0);

        LogWriter.Init(new LogSettings 
        { 
            LogToFile = true, 
            MaxLogFileSize = 4 * 1024 * 1024,
            MinimumLogLevel = GetMinimumLogLevel(),
        });

        Log = new SmartLogger();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        //var theme = "PresentationFramework.Aero;V3.0.0.0;31bf3856ad364e35;component\\themes/aero.normalcolor.xaml";
        //var theme = "/PresentationFramework.Classic;v3.0.0.0;31bf3856ad364e35;Component/themes/classic.xaml";
        var theme = "/PresentationFramework.Royale;v3.0.0.0;31bf3856ad364e35;Component/themes/royale.normalcolor.xaml";
        //var theme = "/PresentationFramework.Luna;v3.0.0.0;31bf3856ad364e35;Component/themes/luna.normalcolor.xaml";
        //var theme = "/PresentationFramework.Luna;v3.0.0.0;31bf3856ad364e35;Component/themes/luna.homestead.xaml";
        //var theme = "/PresentationFramework.Luna;v3.0.0.0;31bf3856ad364e35;Component/themes/luna.metallic.xaml";

        var uri = new Uri(theme, UriKind.Relative);
        Resources.MergedDictionaries.Add(LoadComponent(uri) as ResourceDictionary);
        Log.Debug(new { theme });
    }

    public static Version Version { get; }

    private static void OverrideMetadata(DependencyProperty property, object value)
        => property.OverrideMetadata(typeof(UIElement), new FrameworkPropertyMetadata(value));

    private static LogLevel GetMinimumLogLevel()
    {
        var text = ConfigurationManager.AppSettings["MinimumLogLevel"];

        if (Enum.TryParse(text, out LogLevel logLevel))
            return logLevel;

        return LogLevel.Information;
    }
}
