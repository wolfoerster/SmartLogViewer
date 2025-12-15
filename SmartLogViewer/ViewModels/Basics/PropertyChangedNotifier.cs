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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartLogViewer.ViewModels.Basics;

/// <summary>
/// An implementation of INotifyPropertyChanged and INotifyPropertyChangedPreview.
/// </summary>
public class PropertyChangedNotifier : INotifyPropertyChanged, INotifyPropertyChangedPreview
{
    /// <summary>
    /// Occurs right before a property value is about to be changed.
    /// </summary>
    public event PropertyChangedPreviewEventHandler? PropertyChangedPreview;

    /// <summary>
    /// Occurs right after a property value has been changed.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChangedPreview event.
    /// Returns false if the event is cancelled, otherwise true.
    /// </summary>
    protected bool RaisePropertyChangedPreview(
        object? oldValue,
        object? newValue,
        [CallerMemberName] string? propertyName = null)
    {
        var e = new PropertyChangedPreviewEventArgs(oldValue, newValue, propertyName);
        PropertyChangedPreview?.Invoke(this, e);
        return !e.IsCancelled;
    }

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    /// <summary>
    /// Sets a property value to a new value if the values are different.
    /// Raises the PropertyChangedPreview event before the value is changed.
    /// If this event is not cancelled, the property value is changed and the PropertyChanged event is raised.
    /// </summary>
    /// <returns>True, if the property value was changed, otherwise false.</returns>
    protected bool Checkset<T>(ref T backingField, T newValue, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(backingField, newValue))
        {

            if (RaisePropertyChangedPreview(backingField, newValue, propertyName))
            {

                backingField = newValue;
                RaisePropertyChanged(propertyName);

                return true;
            }
        }

        return false;
    }
}
