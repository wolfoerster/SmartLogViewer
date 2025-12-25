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
using System.IO;
using SmartLogging;
using SmartLogViewer.Common;

namespace SmartLogViewer.Core;

internal class FileReader
{
    protected readonly SmartLogger Log;
    protected string fileName;
    protected bool isNewLogFile;
    private long prevLength;

    public FileReader(string fileName)
    {
        this.fileName = fileName;
        prevLength = 0;
        var name = Path.GetFileName(fileName);
        Log = new SmartLogger($"{GetType().Name}.{name}");
        Log.Information(new { fileName });
    }

    /// <summary>
    /// The name of the related file.
    /// </summary>
    public string FileName => fileName;

    /// <summary>
    /// The known size of the related file.
    /// </summary>
    public long FileSize => prevLength;

    /// <summary>
    /// Read latest bytes from the related file.
    /// </summary>
    public byte[] ReadNextBytes() => ReadNextBytes(fileName);

    /// <summary>
    /// Read latest bytes from a specified file.
    /// </summary>
    private byte[] ReadNextBytes(string path)
    {
        isNewLogFile = false;

        try
        {
            //--- if the file size did not change, we're done
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Length == prevLength)
                return [];

            //--- if the file is smaller now it most probably has been deleted in the meantime
            if (fileInfo.Length < prevLength)
            {
                isNewLogFile = true;
                prevLength = 0;
            }

            //--- now read the next bytes
            int count = (int)(fileInfo.Length - prevLength);
            var bytes = Utils.ReadBytes(path, prevLength, count);
            prevLength = fileInfo.Length;
            return bytes;
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }

        return [];
    }

    /// <summary>
    /// Read all bytes from a specified file.
    /// </summary>
    protected byte[] ReadBytes(string path)
    {
        try
        {
            FileInfo fileInfo = new FileInfo(path);
            prevLength = fileInfo.Length;
            return Utils.ReadBytes(path, 0, fileInfo.Length);
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }

        return [];
    }
}
