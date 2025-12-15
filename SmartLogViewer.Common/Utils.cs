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
using System.Globalization;
using System.IO;
using System.Text;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0057 // Use range operator
#pragma warning disable IDE1006 // Naming style

namespace SmartLogViewer.Common;

public static class Utils
{
    /// <summary>
    /// Returns an error string or null if no error ocurred.
    /// </summary>
    public static string? DeleteFile(string path)
    {
        if (!File.Exists(path))
            return null;

        try
        {
            File.Delete(path);
            return null;
        }
        catch (Exception e)
        {
            return e.ToString();
        }
    }

    /// <summary>
    /// Returns an error string or null if no error ocurred.
    /// </summary>
    public static string? CopyFile(string source, string dest)
    {
        try
        {
            File.Copy(source, dest, true);
            return null;
        }
        catch (Exception e)
        {
            return e.ToString();
        }
    }

    /// <summary>
    /// Clamps the specified x value to the given range.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="xMin">The minimum x value.</param>
    /// <param name="xMax">The maximum x value.</param>
    /// <returns></returns>
    public static double Clamp(double x, double xMin, double xMax)
    {
        return Math.Min(Math.Max(x, xMin), xMax);
    }

    public static int Clamp(int x, int xMin, int xMax)
    {
        return Math.Min(Math.Max(x, xMin), xMax);
    }

    public static bool IsValidIndex(this IList list, int index)
    {
        return index >= 0 && index < list.Count;
    }

    /// <summary>
    /// Returns an error string or null if no error ocurred.
    /// </summary>
    static public string? GetFileInfo(string name, out FileInfo? fileInfo)
    {
        fileInfo = null;

        if (!File.Exists(name))
            return $"File >{name}< does not exist";

        try
        {
            fileInfo = new FileInfo(name);
            return null;
        }
        catch (Exception e)
        {
            return e.ToString();
        }
    }

    /// <summary>
    /// Performs a case insensitive Equals().
    /// </summary>
    static public bool equals(this string str, string value)
    {
        return str.Equals(value, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Performs a case insensitive StartsWith().
    /// </summary>
    static public bool startsWith(this string str, string value)
    {
        return str.StartsWith(value, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Performs a case insensitive EndsWith().
    /// </summary>
    static public bool endsWith(this string str, string value)
    {
        return str.EndsWith(value, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Performs a case insensitive Contains().
    /// </summary>
    static public bool contains(this string str, string value)
    {
        int index = str.IndexOf(value, StringComparison.OrdinalIgnoreCase);
        return index > -1;
    }

    public static void OnlyOnce(object field, object value)
    {
        if (field != null)
            throw new Exception("Field must be null!");

        if (value == null)
            throw new Exception("Value must not be null!");
    }

    /// <summary>
    /// Rounds a double using the specified number of significant figures.
    /// </summary>
    /// <param name="value">A double value.</param>
    /// <param name="sigFigures">The number of significant figures.</param>
    /// <param name="roundingPosition">The rounding position.</param>
    /// <returns>The rounded double value.</returns>
    private static double RoundSignificantDigits(double value, int sigFigures, out int roundingPosition)
    {
        // this method will return a rounded double value at a number of signifigant figures.
        // the sigFigures parameter must be between 0 and 15, exclusive.
        roundingPosition = 0;

        if (double.IsNaN(value))
            return double.NaN;

        if (double.IsPositiveInfinity(value))
            return double.PositiveInfinity;

        if (double.IsNegativeInfinity(value))
            return double.NegativeInfinity;

        //--- WF: we have to set a limit somewhere
        if (Math.Abs(value) <= 1e-98)
            return 0;

        //--- WF: don't throw exceptions if sigFigures is out of range
        sigFigures = Clamp(sigFigures, 1, 14);

        // The resulting rounding position will be negative for rounding at whole numbers, and positive for decimal places.
        roundingPosition = sigFigures - 1 - (int)(Math.Floor(Math.Log10(Math.Abs(value))));

        // try to use a rounding position directly, if no scale is needed.
        // this is because the scale mutliplication after the rounding can introduce error, although 
        // this only happens when you're dealing with really tiny numbers, i.e 9.9e-14.
        if (roundingPosition > 0 && roundingPosition < 15)
            return Math.Round(value, roundingPosition, MidpointRounding.AwayFromZero);

        // Shouldn't get here unless we need to scale it.
        // Set the scaling value, for rounding whole numbers or decimals past 15 places
        double scale = Math.Pow(10, Math.Ceiling(Math.Log10(Math.Abs(value))));
        return Math.Round(value / scale, sigFigures, MidpointRounding.AwayFromZero) * scale;
    }

    /// <summary>
    /// Extended ToString() method. Converts the numeric value of this double to a string, using the specified format.<para/>
    /// There are two additional features on top of the basic double.ToString() implementation:<para/>
    /// 1. New format specifier 's' or 'S' to specify the number of significant figures.<para/>
    /// 2. Optimized scientific notation (remove dispensable characters, e.g. 1.2300e+004 will become 1.23e4)
    /// </summary>
    /// <param name="value">The double value.</param>
    /// <param name="format">The format specifier. If this is null or empty, "g4" is used.</param>
    public static string ToStringExt(this double value, string? format = null)
    {
        NumberFormatInfo currentInfo = CultureInfo.CurrentCulture.NumberFormat;

        //--- Do we have a special value?
        if (double.IsNaN(value))
            return currentInfo.NaNSymbol;

        if (double.IsPositiveInfinity(value))
            return currentInfo.PositiveInfinitySymbol;

        if (double.IsNegativeInfinity(value))
            return currentInfo.NegativeInfinitySymbol;

        if (format == null || format.Length == 0)
            format = "g4";

        if (format[0] == 's' || format[0] == 'S')
        {
            #region Significant figures

            // If you round '0.002' to 3 significant figures, the resulting string should be '0.00200'.
            if (!int.TryParse(format.Remove(0, 1), out int sigFigures))
                sigFigures = 4;

            double roundedValue = RoundSignificantDigits(value, sigFigures, out int roundingPosition);

            //--- 0 shall be formatted as 1 or any other integer < 10:
            if (roundedValue == 0.0d)
            {
                sigFigures = Clamp(sigFigures, 1, 14);
                return string.Format(currentInfo, "{0:F" + (sigFigures - 1) + "}", value);
            }

            // Check if the rounding position is positive and is not past 100 decimal places.
            // If the rounding position is greater than 100, string.format won't represent the number correctly.
            // ToDo:  What happens when the rounding position is greater than 100?
            if (roundingPosition > 0 && roundingPosition < 100)
                return string.Format(currentInfo, "{0:F" + roundingPosition + "}", roundedValue);

            return roundedValue.ToString("F0", currentInfo);

            #endregion Significant figures
        }

        //--- Convert to string using format
        string text = value.ToString(format, currentInfo);

        //--- If text is not in scientific notation, just return it as is:
        int e = text.IndexOfAny(['e', 'E']);
        if (e < 0)
            return text;

        #region Optimize scientific notation

        //--- Remove trailing zeros and possibly decimal separator from the mantissa
        char sep = currentInfo.NumberDecimalSeparator[0];
        string mantissa = text.Substring(0, e);

        mantissa = mantissa.TrimEnd(['0', sep]);
        if (mantissa.Length == 0)
            return "0";

        //--- Remove leading zeros and possibly plus sign from the exponent
        char negativeSign = currentInfo.NegativeSign[0];
        char positiveSign = currentInfo.PositiveSign[0];

        string exponent = text.Substring(e + 1);
        bool isNegative = exponent[0] == negativeSign;

        exponent = exponent.Trim(['0', positiveSign, negativeSign]);
        if (exponent.Length == 0)
            return mantissa;

        //--- Build up the result
        if (isNegative)
            return mantissa + text[e] + negativeSign + exponent;

        return mantissa + text[e] + exponent;

        #endregion Optimise scientific notation
    }

    public static string? GetFileSizeString(double fileSize)
    {
        string[] appendix = ["Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"];
        for (int i = 0; i < appendix.Length; i++, fileSize /= 1e3)
        {
            if (fileSize < 1e3)
            {
                string str = fileSize.ToStringExt("s3");
                return $"{str} {appendix[i]}";
            }
        }

        return null;
    }

    public static string BytesToString(byte[] bytes, int index, int count)
    {
        if (count < 0 || count + index > bytes.Length)
            count = bytes.Length - index;

        string result = Encoding.Default.GetString(bytes, index, count);
        return result;
    }

    public static string ToStringN(this DateTime time)
    {
        return time.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }

    public static bool IsDateTimeOffset(this string value, out DateTimeOffset time)
    {
        time = DateTimeOffset.MinValue;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (!DateTimeOffset.TryParseExact(value, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out time))
            return false;

        return true;
    }

    /// <summary>
    /// Read some bytes from the specified file.
    /// Returns an error string or null if no error ocurred.
    /// </summary>
    public static string? ReadBytes(string path, out byte[] bytes, long startPosition = 0, long count = 0)
    {
        try
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new BinaryReader(fs);

            if (startPosition > 0)
                reader.BaseStream.Seek(startPosition, SeekOrigin.Begin);

            if (count <= 0)
                count = fs.Length - startPosition;

            bytes = reader.ReadBytes((int)count);
            return null;
        }
        catch (Exception e)
        {
            bytes = [];
            return e.ToString();
        }
    }
}
