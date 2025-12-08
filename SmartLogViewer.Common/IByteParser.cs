namespace SmartLogViewer.Common;

public interface IByteParser
{
    /// <summary>
    /// Check if there are log entries of known format.
    /// If yes, the ByteParser can preprocess the file and store the result in a new file
    /// which is then opened with the LogReader. This step is optional.
    /// </summary>
    bool CheckFormat(byte[] bytes, out string? newFileName);

    /// <summary>
    /// The bytes which contain the log entries.
    /// </summary>
    byte[] Bytes { get; set; }

    /// <summary>
    /// Gets the current position, i.e. the position of the next log entry.
    /// </summary>
    int CurrentPosition { get; }

    /// <summary>
    /// Reads the next log entry and updates the current position.
    /// Returns an error string or null if no error ocurred.
    /// </summary>
    string? ReadNextEntry(out LogRecord logRecord);
}
