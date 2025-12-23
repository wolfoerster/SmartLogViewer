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
using System.Windows;
using System.Windows.Input;

namespace SmartLogViewer.Core;

internal static class Commands
{
    public static RoutedUICommand CreateWorkspace =
        CreateCommand("Create", "Create workspace (Ctrl+W)", Key.W, ModifierKeys.Control, "Ctrl+W");

    public static RoutedUICommand RemoveWorkspace = 
        CreateCommand("Remove", "Remove workspace (Ctrl+R)", Key.R, ModifierKeys.Control, "Ctrl+R");

    public static RoutedUICommand OpenFile =
        CreateCommand("Open", "Open file (Ctrl+O)", Key.O, ModifierKeys.Control, "Ctrl+O");

    public static RoutedUICommand CloseFile =
        CreateCommand("Close", "Close file (Ctrl+F4)", Key.F4, ModifierKeys.Control, "Ctrl+F4");

    private static RoutedUICommand CreateCommand(string name, string description, Key key, ModifierKeys modifier, string str)
        => CreateCommand(name, description, new KeyGesture(key, modifier, str));

    private static RoutedUICommand CreateCommand(string name, string description, KeyGesture keyGesture)
        => new(description, name, typeof(FrameworkElement), new InputGestureCollection(new List<InputGesture>() { keyGesture }));
}
