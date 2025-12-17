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
using static SmartLogViewer.Core.Helper;

namespace SmartLogViewer;

public partial class MainWindow : Window
{
    private static readonly SmartLogger Log = new();
    private readonly MainViewModel viewModel;
    private WindowLocation Location => viewModel.MainWindowLocation;

    public MainWindow()
    {
        Log.Information();
        InitializeComponent();
        viewModel = Restore<MainViewModel>();
        viewModel.UpdateColorTheme();
        DataContext = viewModel;

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
        if (Location.IsMaximized)
            WindowState = WindowState.Maximized;
    }

    private void MeClosing(object? sender, CancelEventArgs e)
    {
        Log.Information();
        LogWriter.Flush();
        StoreSizeAndPosition();
    }

    private void RestoreSizeAndPosition()
    {
        var name = Location.ScreenName;
        var screen = Screen.LookUpByName(name);
        if (screen == null)
            return;

        Top = Location.Top;
        Left = Location.Left;
        Width = Location.Width;
        Height = Location.Height;
        WindowState = WindowState.Normal;
        WindowStartupLocation = WindowStartupLocation.Manual;
    }

    private void StoreSizeAndPosition()
    {
        Location.IsMaximized = WindowState == WindowState.Maximized;

        if (WindowState != WindowState.Normal)
            WindowState = WindowState.Normal;

        var pt = new Point(Left, Top).ToPixel(this);
        var screen = Screen.LookUpByPixel(pt);
        Location.ScreenName = screen?.Name;

        Location.Top = Top;
        Location.Left = Left;
        Location.Width = Width;
        Location.Height = Height;
        viewModel.Store();
    }
}