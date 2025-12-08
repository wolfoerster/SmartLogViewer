using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLogViewer.Common;

public class ByteParser : IByteParser
{
    protected static readonly byte CR = 0x0D; // '\r'
    protected static readonly byte LF = 0x0A; // '\n'
    protected static readonly byte Space = 0x20; // ' '
    protected static readonly byte Pipe = 0x7C; // '|'
    protected static readonly byte Plus = 0x2B; // '+'
    protected static readonly byte Minus = 0x2D; // '-'
    protected static readonly byte Colon = 0x3A; // ':'
    protected static readonly byte Comma = 0x2C; // ','
    protected static readonly byte Point = 0x2E; // '.'
    protected byte[] bytes = [];
    protected int lastPos;

    public virtual byte[] Bytes
    {
        get => bytes;
        set
        {
            bytes = value;
            lastPos = 0;
        }
    }

    public int CurrentPosition => lastPos;

    public virtual bool CheckFormat(byte[] bytes, out string? newFileName)
    {
        newFileName = null;
        return true;
    }

    public string? ReadNextEntry(out LogRecord logRecord)
    {
        int nRemain = bytes.Length - lastPos;
        if (nRemain < 1)
        {
            logRecord = new();
            return "try to read beyond buffer";
        }

        try
        {
            return ReadEntry(out logRecord);
        }
        catch (Exception ex)
        {
            logRecord = new();
            return ex.ToString();
        }
    }

    /// <summary>
    /// Override this method in derived classes to read a log entry at the current position.
    /// Return an error string or null if no error ocurred.
    /// </summary>
    protected virtual string? ReadEntry(out LogRecord logRecord)
    {
        logRecord = new LogRecord { LogMessage = GetNextLine() };
        return null;
    }

    /// <summary>
    /// Reads from the current position to the end of the line and moves current position behind it.
    /// </summary>
    protected string GetNextLine()
    {
        int i = GetIndexOfNext(CR, LF);
        string result = GetString(i - lastPos);

        // move on to the next byte which is neither CR nor LF
        lastPos = GetIndexOfNextNot(CR, LF);
        return result;
    }

    /// <summary>
    /// Reads from the current position a number of bytes and moves current position behind it.
    /// </summary>
    protected string GetString(int numBytes)
    {
        string result = Utils.BytesToString(bytes, lastPos, numBytes);
        lastPos += numBytes;
        return result;
    }

    /// <summary>
    /// Looks for the first occurrence of one of the given bytes starting at the current position.
    /// </summary>
    protected int GetIndexOfNext(params byte[] bites)
    {
        return GetIndexOfNext(false, bites);
    }

    /// <summary>
    /// Looks for the first occurrence of a byte which is NOT one of the given bytes starting at the current position.
    /// </summary>
    protected int GetIndexOfNextNot(params byte[] bites)
    {
        return GetIndexOfNext(true, bites);
    }

    /// <summary>
    /// Reads the next bytes until a Space character or a line terminator is found.
    /// </summary>
    protected virtual string? GetNext(int numBytes = -1)
    {
        if (lastPos == bytes.Length)
            return null;

        int i = numBytes < 0 ? GetIndexOfNext(Space, CR, LF) : lastPos + numBytes;
        string result = GetString(i - lastPos);

        if (numBytes > 0)
            result = result.Trim();

        if (bytes[i] == CR || bytes[i] == LF)
            lastPos = GetIndexOfNextNot(CR, LF);
        else
            lastPos = GetIndexOfNextNot(Space);

        return result;
    }

    /// <summary>
    /// Check if an expected string is at a certain position of a bytes array.
    /// </summary>
    protected static bool CheckForString(string expected, byte[] bytes, int position)
    {
        if (bytes == null || position + expected.Length > bytes.Length)
            return false;

        string extracted = Utils.BytesToString(bytes, position, expected.Length);
        return extracted.equals(expected);
    }

    /// <summary>
    /// Looks for the first occurrence of one of the given bytes starting at the current position.
    /// If 'invert' is true, looks for the first occurrence of a byte which is NOT one the given ones.
    /// </summary>
    private int GetIndexOfNext(bool invert, params byte[] searchedBytes)
    {
        int i = lastPos;

        for (; i < bytes.Length; i++)
        {
            var found = false;
            foreach (var searchedByte in searchedBytes)
            {
                if (bytes[i] == searchedByte)
                {
                    found = true;
                    break;
                }
            }

            if (invert)
                found = !found;

            if (found)
                break;
        }

        return i;
    }
}
