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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Windows.Threading;
using SmartLogging;
using SmartLogViewer.Helper;
using SmartLogViewer.Models;
using SmartLogViewer.ViewModels.Basics;
using static SmartLogViewer.Helper.Utils;
using static SmartLogViewer.Common.Utils;

namespace SmartLogViewer.ViewModels;

internal class MainViewModel : PropertyChangedNotifier
{
    private static readonly SmartLogger Log = new();
    private readonly MainModel model;

    public MainViewModel()
    {
        Log.Information();
        model = Restore<MainModel>();

        if (model.Workspaces.Count == 0)
            model.Workspaces.Add(new WorkspaceModel { Name = "Workspace 1" });

        for (int i = 0; i < model.Workspaces.Count; i++)
            Workspaces.Add(model.Workspaces[i]);

        model.SelectedWorkspaceIndex = Clamp(model.SelectedWorkspaceIndex, 0, Workspaces.Count - 1);
        SelectedWorkspace = Workspaces[SelectedWorkspaceIndex];
        Workspaces.CollectionChanged += WorkspacesCollectionChanged;
    }

    public void Shutdown()
    {
        Log.Information();
        model.Store();
    }

    static MainViewModel()
    {
        ReadModes =
        [
            "All records",
            "Last session",
            "Last 24 hours",
            "Last 8 hours",
            "Last hour",
        ];

        LogLevels = 
        [
            "Verbose",
            "Debug",
            "Information",
            "Warning",
            "Error",
            "Fatal",
        ];

        ThemeModes =
        [
            "Light",
            "Dark",
            "System",
            "None",
            "Aero",
            "Royal",
            "Normal",
            "Homestead",
            "Metallic",
            "Classic",
        ];
    }

    public static RoutedUICommand CreateWorkspace => Commands.CreateWorkspace;

    public static RoutedUICommand RemoveWorkspace => Commands.RemoveWorkspace;

    public static RoutedUICommand OpenFile => Commands.OpenFile;

    public static RoutedUICommand CloseFile => Commands.CloseFile;

    public static List<string> ReadModes { get; set; } = [];

    public static List<string> LogLevels { get; set; } = [];

    public static List<string> ThemeModes { get; set; } = [];

    public int ThemeModeIndex
    {
        get => App.Settings.ThemeModeIndex;
        set => Checkset(ref App.Settings.ThemeModeIndex, value, () => App.UpdateThemeMode());
    }

    public ObservableCollection<WorkspaceViewModel> Workspaces { get; set; } = [];

    public int SelectedWorkspaceIndex
    {
        get => model.SelectedWorkspaceIndex;
        set => ChecksetSelectedWorkspaceIndex(value);
    }

    public WorkspaceViewModel SelectedWorkspace { get; set; }

    public void DoCreateWorkspace()
    {
        Workspaces.Add(new WorkspaceModel { Name = $"Workspace {Workspaces.Count + 1}" });
        SelectedWorkspaceIndex = Workspaces.Count - 1;
    }

    public void DoRemoveWorkspace()
    {
        var newIndex = Math.Max(0, SelectedWorkspaceIndex - 1);
        Workspaces.RemoveAt(SelectedWorkspaceIndex);
        SelectedWorkspaceIndex = newIndex;
    }

    private void WorkspacesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add
            && e.NewItems != null && e.NewItems.Count == 1
            && e.NewItems[0] is WorkspaceViewModel newWorkspace)
        {
            model.Workspaces.Add(newWorkspace.Model);
            return;
        }

        if (e.Action == NotifyCollectionChangedAction.Remove
            && e.OldItems != null && e.OldItems.Count == 1
            && e.OldItems[0] is WorkspaceViewModel oldWorkspace)
        {
            model.Workspaces.Remove(oldWorkspace.Model);
            return;
        }
    }

    private void ChecksetSelectedWorkspaceIndex(int newIndex)
    {
        var oldIndex = model.SelectedWorkspaceIndex;
        if (oldIndex != newIndex)
        {
            if (newIndex < 0 && oldIndex >= 0)
                StartTimer(oldIndex);

            model.SelectedWorkspaceIndex = newIndex;
            RaisePropertyChanged(nameof(SelectedWorkspaceIndex));

            if (newIndex >= 0)
            {
                SelectedWorkspace = Workspaces[newIndex];
                RaisePropertyChanged(nameof(SelectedWorkspace));
            }
        }
    }

    private void StartTimer(int oldIndex)
    {
        var timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(30) };

        timer.Tick += (s, e) =>
        {
            timer.Stop();
            SelectedWorkspaceIndex = oldIndex;
        };

        timer.Start();
    }
}
