using System;
using System.Collections.Generic;
using System.Linq;

namespace MercurioShell
{
	/// <summary>
	/// Preserves information (buffer, window sizing, etc) needed to display info to the console
	/// </summary>
	public class MercurioConsole
	{
		const int MonitorRow = 0;
		const int BufferSize = 100;
		private int _writerRow;
		private int _consoleWidth;
		private int _currentCommandOffset = 1;
		private string _blankLine;
		private List<string> ScreenBuffer { get; set; } // Has line breaks to fit screen dimensions for fast output
		private List<string> ContentBuffer { get; set; } // Allows infinite line length
		private List<List<ConsoleKeyInfo>> CommandBuffer { get; set; }

		public MercurioConsole()
		{
			HandleGeometryChange();
			ContentBuffer = new List<string>(BufferSize);
			CommandBuffer = new List<List<ConsoleKeyInfo>>(BufferSize);
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

		public void ResetCommandLine(string line = null)
		{
			ClearRow(_writerRow, line);
			Console.SetCursorPosition(line != null ? line.Count() % _blankLine.Length : 0, _writerRow);
		}

		public void ResetHistory()
		{
			_currentCommandOffset = 0;			
		}

		public void PushCommandLine(List<ConsoleKeyInfo> keySequence)
		{
			CommandBuffer.Add(new List<ConsoleKeyInfo>(keySequence));
		}

		public List<ConsoleKeyInfo> BackHistory()
		{
			if (_currentCommandOffset < CommandBuffer.Count)
				_currentCommandOffset++;
			return HistoryToCommandRow();
		}

		public List<ConsoleKeyInfo> ForwardHistory()
		{
			if (_currentCommandOffset > 0)
				_currentCommandOffset--;
			return HistoryToCommandRow();
		}

		private List<ConsoleKeyInfo> HistoryToCommandRow()
		{
			if (CommandBuffer.Count == 0 || _currentCommandOffset == 0)
				return null;
			
			var keySequence = CommandBuffer[CommandBuffer.Count - _currentCommandOffset];
			string command = string.Join("", keySequence.Select(s => s.KeyChar).ToList());
			ClearRow(_writerRow, command);
			Console.SetCursorPosition(command.Length % _blankLine.Length, _writerRow);
			return keySequence;			
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

