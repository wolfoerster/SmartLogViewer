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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;

namespace SmartLogViewer.Core;

internal static class Helper
{
    public static bool IsValidIndex(this int index, IList collection)
        => index >= 0 && index < collection.Count;

    public static RoutedUICommand CreateCommand(string name, string description, Key key, ModifierKeys modifier, string str)
        => CreateCommand(name, description, new KeyGesture(key, modifier, str));

    public static RoutedUICommand CreateCommand(string name, string description, KeyGesture keyGesture)
        => new(description, name, typeof(FrameworkElement), new InputGestureCollection(new List<InputGesture>() { keyGesture }));

    public static string GetFullPath(string fileName)
    {
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SmartLogViewer");

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        return Path.Combine(dir, fileName);
    }

    public static void Store(this object obj)
    {
        var text = JsonConvert.SerializeObject(obj, Formatting.Indented);
        var path = GetFullPath(obj.GetType().Name + ".json");
        File.WriteAllText(path, text);
    }

    public static T Restore<T>() where T : new()
    {
        var path = GetFullPath(typeof(T).Name + ".json");
        if (!File.Exists(path))
            return new T();

        try
        {
            var text = File.ReadAllText(path);
            var obj = JsonConvert.DeserializeObject<T>(text);
            return obj != null ? obj : new T();
        }
        catch (Exception ex)
        {
            Debug.Write(ex.ToString());
            return new T();
        }
    }

    public static Point ToPixel(this Point pointInDip, Visual visual)
    {
        var source = PresentationSource.FromVisual(visual);
        return source.CompositionTarget.TransformToDevice.Transform(pointInDip);
    }

    public static Point ToDip(this Point pointInPixel, Visual visual)
    {
        var source = PresentationSource.FromVisual(visual);
        return source.CompositionTarget.TransformFromDevice.Transform(pointInPixel);
    }
}
