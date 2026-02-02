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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using SmartLogging;
using SmartLogViewer.Core;
using SmartLogViewer.Settings;
using SmartLogViewer.ViewModels.Basics;
using static System.IO.Path;

namespace SmartLogViewer.ViewModels;

internal class WorkspaceViewModel : PropertyChangedNotifier
{
    private static readonly SmartLogger Log = new();
    private readonly WorkspaceSettings settings;

    public WorkspaceViewModel(WorkspaceSettings settings)
    {
        this.settings = settings;

        foreach (var reader in settings.Readers)
            Readers.Add(reader);

        Readers.CollectionChanged += ReadersCollectionChanged;
    }

    public static implicit operator WorkspaceViewModel(WorkspaceSettings settings) => new(settings);

    public override string ToString()
    {
        return $"{settings}";
    }

    public WorkspaceSettings Settings => settings;

    public string Name
    {
        get => settings.Name;
        set => Checkset(ref settings.Name, value);
    }

    public ObservableCollection<LogReaderViewModel> Readers { get; set; } = [];

    public int SelectedReaderIndex
    {
        get => settings.SelectedReaderIndex;
        set => Checkset(ref settings.SelectedReaderIndex, value);
    }

    public void DoOpenFile()
    {
        var initialDir = GetTempPath();

        if (Readers.Count > 0)
        {
            var directory = GetDirectoryName(Readers[^1].FileName);
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
        Readers.RemoveAt(SelectedReaderIndex);
    }

    private void OpenFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            Log.Information($"File '{fileName}' does not exist");
            return;
        }

        var reader = Readers.FirstOrDefault(x => x.FileName == fileName);
        if (reader != null)
        {
            Log.Information($"File '{fileName}' already open");
            return;
        }

        CreateReader(fileName);
        Log.Information($"File '{fileName}' opened");
        SelectedReaderIndex = Readers.Count - 1;
        LogViewManager.Add(fileName);
    }

    private void ReadersCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add
            && e.NewItems != null && e.NewItems.Count == 1
            && e.NewItems[0] is LogReaderViewModel newReader)
        {
            var readerSettings = new LogReaderSettings { FileName = newReader.FileName };
            settings.Readers.Add(readerSettings);
            return;
        }

        if (e.Action == NotifyCollectionChangedAction.Remove
            && e.OldItems != null && e.OldItems.Count == 1
            && e.OldItems[0] is LogReaderViewModel oldReader)
        {
            settings.Readers.RemoveAll(x => x.FileName == oldReader.FileName);
            return;
        }
    }

    private void CreateReader(string fileName)
    {
        var settings = new LogReaderSettings { FileName = fileName };
        var viewModel = new LogReaderViewModel(settings);
        Readers.Add(viewModel);
    }
}
