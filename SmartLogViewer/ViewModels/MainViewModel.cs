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
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;
using SmartLogging;
using SmartLogViewer.Core;
using SmartLogViewer.Models;
using SmartLogViewer.ViewModels.Basics;

namespace SmartLogViewer.ViewModels;

internal class MainViewModel : PropertyChangedNotifier
{
    private static readonly SmartLogger Log = new();
    private readonly MainModel model;

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

    public MainViewModel(MainModel model)
    {
        this.model = model;
    }

    public static RoutedUICommand CreateWorkspace => Commands.CreateWorkspace;

    public static RoutedUICommand RemoveWorkspace => Commands.RemoveWorkspace;

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
        set => Checkset(ref model.SelectedWorkspaceIndex, value);
    }

    public WorkspaceViewModel SelectedWorkspace { get; set; } = new WorkspaceModel();

    public void Initialize()
    {
        InitWorkspaces();
    }

    public void Shutdown()
    {
        Log.Information();
        this.Store();
    }

    public void DoCreateWorkspace()
    {
        Workspaces.Add(new WorkspaceModel { Name = $"Workspace {Workspaces.Count + 1}" });
    }

    public void DoRemoveWorkspace()
    {
        var newIndex = Math.Max(0, SelectedWorkspaceIndex - 1);
        Workspaces.RemoveAt(SelectedWorkspaceIndex);
        SelectedWorkspaceIndex = newIndex;
    }

    public void OpenFileInteractive()
    {
        var dlg = new OpenFileDialog
        {
            Title = "Select a log file",
            Filter = "Log Files|*.log|All Files|*.*",
            InitialDirectory = SelectedWorkspace.GetDirectory(),
        };

        if (dlg.ShowDialog() == true)
            OpenFile(dlg.FileName);
    }

    private void OpenFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            Log.Information($"File '{fileName}' does not exist");
            return;
        }

        if (SelectedWorkspace.Contains(fileName))
        {
            Log.Information($"File '{fileName}' already open");
            return;
        }

        SelectedWorkspace.Add(fileName);
        Log.Information($"File '{fileName}' opened");
    }

    private void InitWorkspaces()
    {
        if (model.Workspaces.Count == 0)
            model.Workspaces.Add(new WorkspaceModel { Name = "Workspace 1" });

        for (int i = 0; i < model.Workspaces.Count; i++)
            Workspaces.Add(model.Workspaces[i]);

        Workspaces.CollectionChanged += WorkspacesCollectionChanged;
        RaisePropertyChanged(nameof(SelectedWorkspaceIndex));
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
}
