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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using SmartLogging;
using SmartLogViewer.ViewModels;
using SmartLogViewer.ViewModels.Basics;

namespace SmartLogViewer.Views;

public partial class MainControl : UserControl
{
    private static readonly SmartLogger Log = new();
    private readonly MainViewModel ViewModel;

    public MainControl()
    {
        Log.Information();
        ViewModel = new MainViewModel();
        ViewModel.PropertyChangedPreview += ViewModelPropertyChangedPreview;

        InitializeComponent();

        DataContext = ViewModel;
    }

    private void ViewModelPropertyChangedPreview(object? sender, PropertyChangedPreviewEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.SelectedWorkspaceIndex))
        {
            if (e.NewValue is int newValue && newValue == -1)
            {
                if (e.OldValue is int oldValue)
                {
                    var timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(10) };

                    timer.Tick += (s, e) =>
                    {
                        timer.Stop();
                        ViewModel.SelectedWorkspaceIndex = oldValue;
                    };

                    timer.Start();
                }
            }
        }
    }

    public void DoCreateWorkspace(object sender, ExecutedRoutedEventArgs e)
    {
        ViewModel.DoCreateWorkspace();
    }

    public void CanRemoveWorkspace(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ViewModel.Workspaces.Count > 1;
    }

    public void DoRemoveWorkspace(object sender, ExecutedRoutedEventArgs e)
    {
        ViewModel.DoRemoveWorkspace();
    }

    private void DoOpenFile(object sender, ExecutedRoutedEventArgs e)
    {
        ViewModel.SelectedWorkspace.DoOpenFile();
    }

    public void CanCloseFile(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ViewModel.SelectedWorkspace.SelectedFileIndex >= 0;
    }

    private void DoCloseFile(object sender, ExecutedRoutedEventArgs e)
    {
        ViewModel.SelectedWorkspace.DoCloseFile();
    }
}
