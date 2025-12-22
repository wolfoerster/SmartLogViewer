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

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using SmartLogging;
using SmartLogViewer.Core;
using SmartLogViewer.ViewModels;
using SmartLogViewer.Views;

namespace SmartLogViewer;

public partial class MainWindow : Window
{
    private static readonly SmartLogger Log = new();

    public MainWindow()
    {
        Log.Information();
        InitializeComponent();
        Title = $"SmartLogViewer {App.Version.Major}.{App.Version.Minor}.{App.Version.Build}";

        Loaded += MeLoaded;
        Closing += MeClosing;
        RestoreSizeAndPosition();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == Key.Escape)
            Close();
    }

    private void MeLoaded(object sender, RoutedEventArgs e)
    {
        Log.Information();
        if (App.Settings.IsMaximized)
            WindowState = WindowState.Maximized;
    }

    private void MeClosing(object? sender, CancelEventArgs e)
    {
        Log.Information();

        if (Content is MainControl mainControl && mainControl.DataContext is MainViewModel mainViewModel)
            mainViewModel.Shutdown();

        LogWriter.Flush();
        StoreSizeAndPosition();
    }

    private void RestoreSizeAndPosition()
    {
        var name = App.Settings.ScreenName;
        var screen = Screen.LookUpByName(name);
        if (screen == null)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Width = 800;
            Height = 500;
            return;
        }

        Top = App.Settings.Top;
        Left = App.Settings.Left;
        Width = App.Settings.Width;
        Height = App.Settings.Height;
        WindowState = WindowState.Normal;
        WindowStartupLocation = WindowStartupLocation.Manual;
    }

    private void StoreSizeAndPosition()
    {
        App.Settings.IsMaximized = WindowState == WindowState.Maximized;
        WindowState = WindowState.Normal;

        var pt = new Point(Left, Top).ToPixel(this);
        var screen = Screen.LookUpByPixel(pt);
        App.Settings.ScreenName = screen?.Name;

        App.Settings.Top = Top;
        App.Settings.Left = Left;
        App.Settings.Width = Width;
        App.Settings.Height = Height;
        App.Settings.Store();
    }
}