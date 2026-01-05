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
using SmartLogViewer.Settings;
using SmartLogViewer.ViewModels.Basics;
using static SmartLogViewer.Common.Utils;
using static SmartLogViewer.Helper.Utils;

namespace SmartLogViewer.ViewModels;

internal class MainViewModel : PropertyChangedNotifier
{
    private static readonly SmartLogger Log = new();
    private readonly MainSettings settings;
    private readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(30) };
    private int previousWorkspaceIndex;

    public MainViewModel()
    {
        Log.Information();
        timer.Tick += TimerTick;
        settings = Restore<MainSettings>();

        if (settings.Workspaces.Count == 0)
            settings.Workspaces.Add(new WorkspaceSettings { Name = "Workspace 1" });

        for (int i = 0; i < settings.Workspaces.Count; i++)
            Workspaces.Add(settings.Workspaces[i]);

        Workspaces.CollectionChanged += WorkspacesCollectionChanged;

        settings.SelectedWorkspaceIndex = Clamp(settings.SelectedWorkspaceIndex, 0, Workspaces.Count - 1);
        SelectedWorkspace = Workspaces[settings.SelectedWorkspaceIndex];
    }

    public void Shutdown()
    {
        Log.Information();
        settings.Store();
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
        get => settings.SelectedWorkspaceIndex;
        set => ChangeSelectedWorkspaceIndex(value);
    }

    public WorkspaceViewModel SelectedWorkspace { get; set; }

    public void DoCreateWorkspace()
    {
        Workspaces.Add(new WorkspaceSettings { Name = $"Workspace {Workspaces.Count + 1}" });
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
            settings.Workspaces.Add(newWorkspace.Settings);
            return;
        }

        if (e.Action == NotifyCollectionChangedAction.Remove
            && e.OldItems != null && e.OldItems.Count == 1
            && e.OldItems[0] is WorkspaceViewModel oldWorkspace)
        {
            settings.Workspaces.Remove(oldWorkspace.Settings);
            return;
        }
    }

    private void ChangeSelectedWorkspaceIndex(int newIndex)
    {
        previousWorkspaceIndex = settings.SelectedWorkspaceIndex;
        if (newIndex == previousWorkspaceIndex)
            return;

        settings.SelectedWorkspaceIndex = newIndex;
        RaisePropertyChanged(nameof(SelectedWorkspaceIndex));

        if (newIndex >= 0)
        {
            SelectedWorkspace = Workspaces[newIndex];
            RaisePropertyChanged(nameof(SelectedWorkspace));
        }

        if (newIndex < 0 /* nothing IS selected */
            && previousWorkspaceIndex >= 0 /* something WAS selected */)
        {
            timer.Start(); // re-select previous selected workspace
        }
    }

    private void TimerTick(object? sender, EventArgs e)
    {
        timer.Stop();
        SelectedWorkspaceIndex = previousWorkspaceIndex;
    }
}
