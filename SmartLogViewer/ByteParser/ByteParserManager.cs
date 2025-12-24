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

/**************************************************************
 * 
 *
 * A log file might look like this:
 *
 * Line1: random text (or perhaps a hint to the file format)
 * Line2: random text or whatever
 * Line3: LogLevel TimeStamp LogContext LogMessage ...
 * Line4: ... LogMessage (continued)
 * Line5: LogLevel TimeStamp LogContext LogMessage
 * Line6: LogLevel TimeStamp LogContext LogMessage ...
 * Line7: ... LogMessage (continued)
 * Line8: LogLevel TimeStamp LogContext LogMessage
 * 
 * So to parse that file do this:
 * 
 * while (true)
 * {
 *    if (!FindNextTimeStamp)
 *        break;
 * 
 *    ReadRecord(); // which in the above example has parts BEFORE and AFTER the TimeStamp!
 * }
 * 
 ***************************************************************/

using System;

namespace SmartLogViewer.ByteParser;

internal static class ByteParserManager
{
}
