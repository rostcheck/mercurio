using System;
using System.Collections.Generic;
using System.Linq;

namespace MercurioShell
{
	/// <summary>
	/// Preserves information (buffer, window sizing, etc) needed to display info to the console.
	/// Internally it contains a list of command history and a microbuffer for the current command line.
	/// </summary>
	public class MercurioConsole
	{
		const int MonitorRow = 0;
		const int BufferSize = 100;
		private int _writerRow;
		private int _cursorPosition = 0;
		private int _originalCursorSize = 0;
		private int _consoleWidth;
		private int _currentCommandOffset = 1;
		private bool _insertMode = false;
		private string _blankLine;
		private string _currentCommandLine = "";
		private List<string> ScreenBuffer { get; set; } // Has line breaks to fit screen dimensions for fast output
		private List<string> ContentBuffer { get; set; } // Allows infinite line length
		private List<string> CommandBuffer { get; set; }

		public MercurioConsole()
		{
			HandleGeometryChange();
			ContentBuffer = new List<string>(BufferSize);
			//CommandBuffer = new List<List<ConsoleKeyInfo>>(BufferSize);
			CommandBuffer = new List<string>(BufferSize);
			_originalCursorSize = Console.CursorSize;
			InsertMode(true);
		}			

		public List<string> CreateScreenBuffer()
		{
			ScreenBuffer = new List<string>();
			if (ContentBuffer != null)
			{
				foreach (var line in ContentBuffer)
				{
					AddToScreenBuffer(line);
				}
			}

			return ScreenBuffer;
		}

		public void WriteToConsole(IEnumerable<string> lines)
		{
			foreach (var line in lines)
				AddToBuffers(line);
			AddToBuffers(_blankLine);			
			WriteMonitor(ScreenBuffer);
		}

		public void WriteToConsole(string line)
		{
			AddToBuffers(line);
			//AddToBuffers(System.Environment.NewLine);
			WriteMonitor(ScreenBuffer);
		}

		public void InsertMode(bool insertMode)
		{
			_insertMode = insertMode;
			if (_insertMode)
				Console.CursorSize = 100; // Full-cell size
			else
				Console.CursorSize = _originalCursorSize;
		}

		public void AddKey(string key)
		{
			string newCommandLine;
			newCommandLine = _currentCommandLine.Substring(0, _cursorPosition);
			newCommandLine += key;
			int splicePos = _cursorPosition;
			if (!_insertMode && splicePos < _currentCommandLine.Length)
				splicePos++;
			newCommandLine += _currentCommandLine.Substring(splicePos, _currentCommandLine.Length - splicePos);
			_currentCommandLine = newCommandLine;
			_cursorPosition++;
			RedrawRow(_writerRow, _currentCommandLine, _cursorPosition);
		}

		public void DeleteKey()
		{
			string newCommandLine;
			int splicePos = _cursorPosition - 1;
			if (splicePos < 0) 
				splicePos = 0;
			newCommandLine = _currentCommandLine.Substring(0, splicePos);
			splicePos += 1;
			newCommandLine += _currentCommandLine.Substring(splicePos, _currentCommandLine.Length - splicePos);
			_currentCommandLine = newCommandLine;
			_cursorPosition--;
			RedrawRow(_writerRow, _currentCommandLine, _cursorPosition);
		}

		public void ResetCommandLine(string line = null)
		{
			if (line != null)
				_currentCommandLine = line;
			else
				_currentCommandLine = "";

			ClearRow(_writerRow, line);
			SetCursorPosition(line != null ? line.Count() % _blankLine.Length : 0);
		}

		private void SetCursorPosition(int column)
		{
			_cursorPosition = column;
			Console.SetCursorPosition(_cursorPosition, _writerRow);
		}

		public void CursorLeft()
		{
			SetCursorPosition(_cursorPosition > 0 ? _cursorPosition - 1 : _cursorPosition);
		}

		public void CursorRight()
		{
			SetCursorPosition(_cursorPosition + 1);
		}

		public void ResetHistory()
		{
			_currentCommandOffset = 0;			
		}

		/// <summary>
		/// Return the current command line and push it into the history buffer
		/// </summary>
		/// <returns>The current command line.</returns>
		public string PushCommandLine()
		{
			CommandBuffer.Add(_currentCommandLine);
			return _currentCommandLine;
			//_currentCommandLine = "";
		}

		public void BackHistory()
		{
			if (_currentCommandOffset < CommandBuffer.Count)
				_currentCommandOffset++;
			HistoryToCommandRow();
		}

		public void ForwardHistory()
		{
			if (_currentCommandOffset > 0)
				_currentCommandOffset--;
			HistoryToCommandRow();
		}

		private void HistoryToCommandRow()
		{
			if (CommandBuffer.Count == 0 || _currentCommandOffset == 0)
				return;
			
			ResetCommandLine(CommandBuffer[CommandBuffer.Count - _currentCommandOffset]);
		}

		public void TabComplete(MercurioCommandShell shell)
		{
			var newCommandLine = shell.ResolveCommand(_currentCommandLine);
			if (newCommandLine != _currentCommandLine)
				Console.Beep();
			_currentCommandLine = newCommandLine;
			_cursorPosition = _currentCommandLine.Length;
			RedrawRow(_writerRow, _currentCommandLine, _cursorPosition);

		}

		private void WriteMonitor(List<string> buffer)
		{
			Console.ForegroundColor = ConsoleColor.Gray;
			int numWindowRows = Console.WindowHeight - 1;
			int rowsToWrite = (buffer.Count > numWindowRows) ? numWindowRows : buffer.Count;
			int bufferRowStart = (buffer.Count > numWindowRows) ? buffer.Count - numWindowRows : 0;
			string line = null;
			for (int row = 0; row < numWindowRows; row++)				
			{
				line = null;
				if (row < rowsToWrite)
				{
					int bufferRow = bufferRowStart + row;
					int thisRowLength = buffer[bufferRow].Length;
					line = buffer[bufferRow] + _blankLine.Substring(thisRowLength, _blankLine.Length - thisRowLength); 
				}
				ClearRow(row, line);
			}		
			Console.ForegroundColor = ConsoleColor.Green;
		}

		private void ClearRow(int rowNumber, string line)
		{
			Console.SetCursorPosition(0, rowNumber);
			if (line == null)
				Console.Write(_blankLine);
			else
			{
				int remainingChars = (line.Length >= _blankLine.Length) ? 0 : _blankLine.Length - line.Length;
				Console.Write(line + _blankLine.Substring(0, remainingChars));
			}
		}

		private void RedrawRow(int rowNumber, string line, int finalCursorPos)
		{
			Console.SetCursorPosition(0, rowNumber);
			if (line == null)
				Console.Write(_blankLine);
			else
			{
				int remainingChars = (line.Length >= _blankLine.Length) ? 0 : _blankLine.Length - line.Length;
				Console.Write(line + _blankLine.Substring(0, remainingChars));
			}
			Console.SetCursorPosition(finalCursorPos, rowNumber);
		}

		private void AddToBuffers(string line)
		{
			if (Console.WindowWidth != _consoleWidth)
			{				
				HandleGeometryChange();
			}
			ContentBuffer.Add(line);
			AddToScreenBuffer(line);
		}

		private void HandleGeometryChange()
		{
			_consoleWidth = Console.WindowWidth;
			ScreenBuffer = CreateScreenBuffer(); // Regen for new geometry
			_blankLine = new string(' ', Console.WindowWidth);
			_writerRow = Console.WindowHeight - 1;			
		}

		private void AddToScreenBuffer(string line)
		{
			int charsLeft = line.Length;
			int startChar = 0;
			do {
				int charsToTake = ((line.Length - startChar) > _consoleWidth) ? _consoleWidth : line.Length - startChar;
				ScreenBuffer.Add(line.Substring(startChar, charsToTake));
				startChar += charsToTake + 1;
				charsLeft -= _consoleWidth;
			}
			while(charsLeft > 0);			
		}
	}
}