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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SmartLogging;
using SmartLogViewer.Models;
using SmartLogViewer.ViewModels;
using static SmartLogViewer.Core.Helper;

namespace SmartLogViewer.Views;

public partial class MainControl : UserControl
{
    private static readonly SmartLogger Log = new();
    private readonly MainViewModel ViewModel;

    public MainControl()
    {
        Log.Information();
        ViewModel = new MainViewModel(Restore<MainModel>());
        ViewModel.Initialize();

        InitializeComponent();

        DataContext = ViewModel;
    }

    public void DoCreateWorkspace(object sender, ExecutedRoutedEventArgs e)
    {
        ViewModel.DoCreateWorkspace();
    }

    public void CanRemoveWorkspace(object sender, CanExecuteRoutedEventArgs e)
    {
        var isValidIndex = ViewModel.SelectedWorkspaceIndex.IsValidIndex(ViewModel.Workspaces);
        e.CanExecute = isValidIndex && ViewModel.Workspaces.Count > 1;
    }

    public void DoRemoveWorkspace(object sender, ExecutedRoutedEventArgs e)
    {
        ViewModel.DoRemoveWorkspace();
    }

    private void OnOpenClicked(object sender, RoutedEventArgs e)
    {
        ViewModel.OpenFileInteractive();
    }

    private void OnRemoveFile(object sender, RoutedEventArgs e)
    {
        ViewModel.SelectedWorkspace.RemoveSelectedFile();
    }
}
