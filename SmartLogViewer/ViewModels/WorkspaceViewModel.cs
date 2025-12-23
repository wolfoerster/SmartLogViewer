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

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using Microsoft.Win32;
using SmartLogging;
using SmartLogViewer.Models;
using SmartLogViewer.ViewModels.Basics;
using static System.IO.Path;

namespace SmartLogViewer.ViewModels;

internal class WorkspaceViewModel : PropertyChangedNotifier
{
    private static readonly SmartLogger Log = new();
    private readonly WorkspaceModel model;

    public WorkspaceViewModel(WorkspaceModel model)
    {
        this.model = model;
        Files.CollectionChanged += FilesCollectionChanged;
    }

    public static implicit operator WorkspaceViewModel(WorkspaceModel model) => new(model);

    public WorkspaceModel Model => model;

    public string Name
    {
        get => model.Name;
        set => Checkset(ref model.Name, value);
    }

    public ObservableCollection<string> Files { get; set; } = [];

    public int SelectedFileIndex
    {
        get => model.SelectedFileIndex;
        set => Checkset(ref model.SelectedFileIndex, value);
    }

    public void DoOpenFile()
    {
        var initialDir = GetTempPath();

        if (Files.Count > 0)
        {
            var directory = GetDirectoryName(Files[^1]);
            if (directory != null)
                initialDir = directory;
        }

        var dlg = new OpenFileDialog
        {
            Title = "Select a log file",
            Filter = "Log Files|*.log|All Files|*.*",
            InitialDirectory = initialDir,
        };

        if (dlg.ShowDialog() == true)
            OpenFile(dlg.FileName);
    }

    public void DoCloseFile()
    {
        Files.RemoveAt(SelectedFileIndex);
    }

    private void OpenFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            Log.Information($"File '{fileName}' does not exist");
            return;
        }

        if (Files.Contains(fileName))
        {
            Log.Information($"File '{fileName}' already open");
            return;
        }

        Files.Add(fileName);
        Log.Information($"File '{fileName}' opened");
    }

    private void FilesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add
            && e.NewItems != null && e.NewItems.Count == 1
            && e.NewItems[0] is string newFileName)
        {
            model.Files.Add(newFileName);
            return;
        }

        if (e.Action == NotifyCollectionChangedAction.Remove
            && e.OldItems != null && e.OldItems.Count == 1
            && e.OldItems[0] is string oldFileName)
        {
            model.Files.Remove(oldFileName);
            return;
        }
    }
}
