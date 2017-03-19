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
        const int CommandBufferRows = 2;
		private CursorPosition _cursorPosition;
		private int _originalCursorSize = 0;
		private int _consoleWidth;
		private int _currentCommandOffset = 1;
		private bool _insertMode = false;
		private string _blankLine;
		private string _currentCommandLine = "";
		private List<string> ScreenBuffer { get; set; } // Has line breaks to fit screen dimensions for fast output
		private List<string> ContentBuffer { get; set; } // Allows infinite line length
		private List<string> CommandBuffer { get; set; }

        private struct CursorPosition
        {
            public int Column;
            public int Row;
        }

		public MercurioConsole()
		{
			HandleGeometryChange();
			ContentBuffer = new List<string>(BufferSize);
			//CommandBuffer = new List<List<ConsoleKeyInfo>>(BufferSize);
			CommandBuffer = new List<string>(BufferSize);
			_originalCursorSize = Console.CursorSize;
			InsertMode(true);
            _cursorPosition = new CursorPosition() { Column = 0, Row = Console.WindowHeight - CommandBufferRows };
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
            ResetCommandLine();
		}

		public void WriteToConsole(string line)
		{
            AddToBuffers(line);
            WriteMonitor(ScreenBuffer);
            ResetCommandLine();
		}

		public void InsertMode(bool insertMode)
		{
			_insertMode = insertMode;
			if (_insertMode)
				Console.CursorSize = 100; // Full-cell size
			else
				Console.CursorSize = _originalCursorSize;
		}

		public void GoToLineStart()
		{
            _cursorPosition.Column = 0;
            _cursorPosition.Row = Console.WindowHeight - CommandBufferRows;
			SetCursorPosition(_cursorPosition);
		}

		public void GoToLineEnd()
		{
			ResetCommandLine(_currentCommandLine);
		}

		public void AddKey(string key)
		{
			string newCommandLine;
            _cursorPosition.Column = _cursorPosition.Column % _blankLine.Length;
            int splicePos = GetSplicePosition();
            newCommandLine = _currentCommandLine.Substring(0, splicePos); // Front part of string
			newCommandLine += key;
            if (_insertMode && splicePos < _currentCommandLine.Length)
            {
                // If splicing, splice in the back part of string
                newCommandLine += _currentCommandLine.Substring(splicePos, _currentCommandLine.Length - splicePos);
            }
			_currentCommandLine = newCommandLine;
			_cursorPosition.Column++;
			_cursorPosition = RedrawCommandRegion(_cursorPosition, _currentCommandLine);
            if ((int)(_currentCommandLine.Length / _blankLine.Length + 1) >= CommandBufferRows)
                WriteMonitor(ScreenBuffer); // Rewrite ScreenBuffer if we scrolled over it
            SetCursorPosition(_cursorPosition);
		}

        private int GetSplicePosition()
        {
            int offset = 0;
            int numLines = _currentCommandLine.Length / _blankLine.Length;
            if (numLines > 0)
                offset = numLines * _blankLine.Length;
            int newPos = offset + _cursorPosition.Column;
            return(newPos > _currentCommandLine.Length ? _currentCommandLine.Length : newPos);
        }

		public void DeleteKey()
		{
			string newCommandLine;
            int splicePos = GetSplicePosition() - 1;
			if (splicePos < 0)
				return; //splicePos = 0;
			newCommandLine = _currentCommandLine.Substring(0, splicePos);
			splicePos += 1;
			newCommandLine += _currentCommandLine.Substring(splicePos, _currentCommandLine.Length - splicePos);
			_currentCommandLine = newCommandLine;
			_cursorPosition.Column--;

            _cursorPosition = RedrawCommandRegion(_cursorPosition, _currentCommandLine);
            if ((int)(_currentCommandLine.Length / _blankLine.Length + 1) >= CommandBufferRows)
                WriteMonitor(ScreenBuffer); // Rewrite ScreenBuffer if we scrolled over it
            SetCursorPosition(_cursorPosition);
		}

		public void ResetCommandLine(string line = null)
		{
			if (line != null)
				_currentCommandLine = line;
			else
				_currentCommandLine = "";

			ClearRow(Console.WindowHeight - CommandBufferRows, line);
            _cursorPosition.Column = (line != null) ? line.Count() % _blankLine.Length : 0;
			SetCursorPosition(_cursorPosition);
		}

		private void SetCursorPosition(CursorPosition pos)
		{
            _cursorPosition = FixCursorPos(pos);
 			Console.SetCursorPosition(_cursorPosition.Column, _cursorPosition.Row);
		}

        private CursorPosition FixCursorPos(CursorPosition pos)
        {
            if (pos.Column >= _blankLine.Length)
            {
                pos.Column = 0;
                pos.Row++;
                if (pos.Row > Console.WindowHeight - 1)
                    pos.Row = Console.WindowHeight - 1;
            }
            return(pos);
        }
		public void CursorLeft()
		{
            //CursorPosition pos = new CursorPosition() {
            if (_cursorPosition.Column > 0)
                _cursorPosition.Column -= 1;
			SetCursorPosition(_cursorPosition);
		}

		public void CursorRight()
		{
            _cursorPosition.Column += 1;
			SetCursorPosition(_cursorPosition);
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
			_cursorPosition.Column = _currentCommandLine.Length;
            _cursorPosition.Row = _currentCommandLine.Length / _blankLine.Length;
			_cursorPosition = RedrawCommandRegion(_cursorPosition, _currentCommandLine);
            SetCursorPosition(_cursorPosition);
		}

		private void WriteMonitor(List<string> buffer)
		{
			Console.ForegroundColor = ConsoleColor.Gray;
            int numWindowRows = Console.WindowHeight - CommandBufferRows;
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

        private CursorPosition RedrawCommandRegion(CursorPosition pos, string line)
		{
			Console.SetCursorPosition(0, pos.Row);
			if (line == null)
				Console.Write(_blankLine);
			else
			{
				int remainingChars = (line.Length >= _blankLine.Length) ? 0 : _blankLine.Length - line.Length;
				Console.Write(line + _blankLine.Substring(0, remainingChars));
			}
            if (pos.Column == _blankLine.Length)
                pos.Column = 0;
            Console.SetCursorPosition(pos.Column, pos.Row);
            return _cursorPosition;
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