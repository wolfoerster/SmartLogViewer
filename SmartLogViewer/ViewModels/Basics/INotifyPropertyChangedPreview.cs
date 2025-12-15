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

namespace SmartLogViewer.ViewModels.Basics;

/// <summary>
/// Represents the method that will handle the <see cref="INotifyPropertyChangedPreview.PropertyChangedPreview"/>
/// event raised when a property is about to be changed on a component.
/// </summary>
public delegate void PropertyChangedPreviewEventHandler(object? sender, PropertyChangedPreviewEventArgs e);

/// <summary>
/// Notifies clients that a property value is about to be changed.
/// </summary>
public interface INotifyPropertyChangedPreview
{
    /// <summary>
    /// Occurs right before a property value is about to be changed.
    /// To prevent changing the value, set the IsCancelled property
    /// of the <see cref="PropertyChangedPreviewEventArgs"/> to true.
    /// </summary>
    event PropertyChangedPreviewEventHandler? PropertyChangedPreview;
}
