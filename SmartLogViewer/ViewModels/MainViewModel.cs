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

using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using Microsoft.Win32;
using Newtonsoft.Json;
using SmartLogging;
using SmartLogViewer.Core;
using SmartLogViewer.ViewModels.Basics;

namespace SmartLogViewer.ViewModels;

internal class MainViewModel : PropertyChangedNotifier
{
    private const string Default = "Default";
    private static readonly SmartLogger Log = new();

    public ThemeViewModel ColorTheme { get; set; } = new();

    private bool isDarkMode;
    public bool IsDarkMode
    {
        get => isDarkMode;
        set
        {
            if (Checkset(ref isDarkMode, value))
            {
                UpdateColorTheme();
            }
        }
    }

    public string LastWorkspace { get; set; } = Default;

    public Dictionary<string, WorkspaceViewModel> Workspaces { get; set; } = [];

    [JsonIgnore]
    public WorkspaceViewModel CurrentWorkspace { get; set; } = new();

    public void Initialize()
    {
        UpdateColorTheme();
        InitWorkspaces();
    }

    public void Shutdown()
    {
        Log.Information();
        this.Store();
    }

    public void OpenFileInteractive()
    {
        var dlg = new OpenFileDialog
        {
            Title = "Select a log file",
            Filter = "Log Files|*.log|All Files|*.*",
            InitialDirectory = CurrentWorkspace.GetDirectory(),
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

        if (CurrentWorkspace.Contains(fileName))
        {
            Log.Information($"File '{fileName}' already open");
            return;
        }

        CurrentWorkspace.Add(fileName);
        Log.Information($"File '{fileName}' opened");
    }

    private void UpdateColorTheme()
    {
        var index = isDarkMode ? 0 : 1;
        ColorTheme.Background = CreateBrush(ThemeColors.Background[index]);
        ColorTheme.Foreground = CreateBrush(ThemeColors.Foreground[index]);

        static SolidColorBrush CreateBrush(Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }
    }

    private void InitWorkspaces()
    {
        if (Workspaces.Count == 0)
            Workspaces.Add(Default, new WorkspaceViewModel());

        if (!Workspaces.ContainsKey(LastWorkspace))
            LastWorkspace = Default;

        CurrentWorkspace = Workspaces[LastWorkspace];
    }
}
