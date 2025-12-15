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

namespace SmartLogViewer.ViewModels.Basics;

/// <summary>
/// Provides data for the <see cref="INotifyPropertyChangedPreview.PropertyChangedPreview"/> event.
/// </summary>
public class PropertyChangedPreviewEventArgs : PropertyChangedEventArgs
{
    public PropertyChangedPreviewEventArgs(object? oldValue, object? newValue, string? propertyName)
        : base(propertyName)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    public object? OldValue { get; set; }

    public object? NewValue { get; set; }

    public bool IsCancelled { get; private set; }

    public void Cancel()
    {
        IsCancelled = true;
    }
}
