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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Newtonsoft.Json;
using SmartLogViewer.ViewModels.Basics;
using static System.IO.Path;

namespace SmartLogViewer.ViewModels;

internal class WorkspaceViewModel : PropertyChangedNotifier
{
    public static uint WorkspaceCount;

    public WorkspaceViewModel()
        : this($"New workspace {WorkspaceCount}")
    {
    }

    public WorkspaceViewModel(string name)
    {
        Name = name;
        WorkspaceCount++;
        FileCollection.CollectionChanged += FileCollectionChanged;
        FileCollection.Add("Eine Datei");
    }

    private string name = "";
    public string Name
    {
        get => name;
        set => Checkset(ref name, value);
    }

    public List<string> Files { get; set; } = [];

    [JsonIgnore]
    public ObservableCollection<string> FileCollection { get; set; } = [];

    private int selectedFileIndex;
    public int SelectedFileIndex
    {
        get => selectedFileIndex;
        set => Checkset(ref selectedFileIndex, value);
    }

    public string GetDirectory()
    {
        if (Files.Count > 0)
        {
            var directory = GetDirectoryName(Files[^1]);
            if (directory != null)
                return directory;
        }

        return GetTempPath();
    }

    public bool Contains(string fileName) => Files.Contains(fileName);

    public void Add(string fileName)
    {
        FileCollection.Add(fileName);
    }

    private void FileCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add
            && e.NewItems != null && e.NewItems.Count == 1
            && e.NewItems[0] is string newFileName)
        {
            Files.Add(newFileName);
            return;
        }

        if (e.Action == NotifyCollectionChangedAction.Remove
            && e.OldItems != null && e.OldItems.Count == 1
            && e.OldItems[0] is string oldFileName)
        {
            Files.Remove(oldFileName);
            return;
        }
    }

    public void RemoveSelectedFile()
    {
    }
}
