using System;
using System.ComponentModel;
using System.IO;
using SoftwareMonkeys.SiteStarter.State;
using System.Diagnostics;
using System.Text;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Writes log entries to the log file.
	/// </summary>
	public class LogFileWriter : Component
	{
		static public string LogFileWriterKey = "Diagnostics.LogFileWriter.Current";
		
		/// <summary>
		/// Gets a single instance of the log file writer held in application state. The log stream writer will be closed when this instance is disposed.
		/// </summary>
		static public LogFileWriter Current
		{
			get {
				if (!StateAccess.State.ContainsApplication(LogFileWriterKey))
					StateAccess.State.SetApplication(LogFileWriterKey, new LogFileWriter());
				return (LogFileWriter)StateAccess.State.GetApplication(LogFileWriterKey); }
		}
		
		static private string logFilePath = String.Empty;
		/// <summary>
		/// Gets/sets the path to the current log file.
		/// </summary>
		static public string LogFilePath
		{
			get {
				if (logFilePath == String.Empty)
					logFilePath = GetLogFilePath();
				return logFilePath; }
			set { logFilePath = value; }
		}
		
		protected string BufferDataKey = "Diagnostics.LogFileWriter.BufferData";
		
		/// <summary>
		/// Gets/sets the current log data held in the buffer.
		/// </summary>
		public string BufferData
		{
			get {
				if (StateAccess.IsInitialized)
				{
					if (StateAccess.State.ContainsApplication(BufferDataKey))
						return (string)StateAccess.State.GetApplication(BufferDataKey);
				}
				else
					throw new InvalidOperationException("Cannot access log buffer when the state access has not been initialized.");
				
				return String.Empty;
			}
			set
			{
				if (StateAccess.IsInitialized)
					StateAccess.State.SetApplication(BufferDataKey, value);
				else
					throw new InvalidOperationException("Cannot write to log file when the state access has not been initialized.");
			}
		}
		
		protected string PreviousCommitTimeKey = "Diagnostics.LogFileWriter.PreviousCommitTime";
		
		/// <summary>
		/// Gets/sets the time that the previous commit occurred.
		/// </summary>
		public DateTime PreviousCommitTime
		{
			get {
				if (StateAccess.State.ContainsApplication(PreviousCommitTimeKey))
					return (DateTime)StateAccess.State.GetApplication(PreviousCommitTimeKey);
				return DateTime.MinValue;
			}
			set
			{
				StateAccess.State.SetApplication(PreviousCommitTimeKey, value);
			}
		}
		
		/// <summary>
		/// The duration to buffer data (in seconds).
		/// </summary>
		// TODO: Allow this to be specified in the Web.config but have a default value here in case it's not specified
		public int BufferDuration = 2;
		
		/// <summary>
		/// Opens the file stream writer instance.
		/// </summary>
		static public StreamWriter OpenStreamWriter()
		{
			string logFilePath = LogFilePath;
			
			StreamWriter writer = null;
			
			try
			{
				// If the log exists then open it
				if (File.Exists(logFilePath))
					writer = File.AppendText(logFilePath);
				// Otherwise create a new log
				else
				{
					// Create the log directory if not foudn
					if (!Directory.Exists(Path.GetDirectoryName(logFilePath)))
						Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
					
					// Create a new log file
					writer = File.CreateText(logFilePath);
					
					// Write the header to the log file
					writer.Write(CreateLogHeader());
				}
			}
			catch (IOException)
			{
				// The file is locked. Return a null value and the commit will be skipped until next time
			}
			
			return writer;
		}
		
		public LogFileWriter()
		{
		}
		
		internal void Write(string entry)
		{
			AddToBuffer(entry);
		}
		
		public void Write(Guid entryID, string entry)
		{
			Write(entry);
			
			WriteStackTrace(entryID);
		}
		
		/// <summary>
		/// Adds the provided data to the buffer and commits it if it's time to do so.
		/// </summary>
		/// <param name="data"></param>
		public void AddToBuffer(string data)
		{
			BufferData = BufferData + data;
			
			if (TimeToCommit())
			{
				CommitBuffer();
			}
		}
		
		/// <summary>
		/// Checks whether it's time to commit the buffer.
		/// </summary>
		/// <returns></returns>
		public bool TimeToCommit()
		{
			return PreviousCommitTime.AddSeconds(BufferDuration) < DateTime.Now;
		}
		
		/// <summary>
		/// Commits the buffered data to file.
		/// </summary>
		public void CommitBuffer()
		{
			if (BufferData != String.Empty)
			{
				using (StreamWriter writer = OpenStreamWriter())
				{
					// TODO: Check if needed
					// If the write is null then the file was locked and the commit can be skipped until next time
					if (writer != null)
					{
						writer.Write(BufferData);
						
						BufferData = String.Empty;
						PreviousCommitTime = DateTime.Now;
					}
				}
			}
		}
		
		/// <summary>
		/// Saves the current stack trace to file with the provided ID.
		/// </summary>
		/// <param name="id"></param>
		public void WriteStackTrace(Guid id)
		{
			string trace = CreateStackTrace();
			
			string path = GetStackTracePath(id);
			
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			
			using (StreamWriter writer = File.CreateText(path))
			{
				writer.Write(trace);
				writer.Close();
			}
		}

		
		static public string GetLogFilePath()
		{
			string dateStamp = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
			
			return StateAccess.State.PhysicalApplicationPath.TrimEnd('\\')
				+ Path.DirectorySeparatorChar + "App_Data"
				+ Path.DirectorySeparatorChar + "Logs"
				+ Path.DirectorySeparatorChar + dateStamp
				+ Path.DirectorySeparatorChar + "Log.xml";
		}
		
		static public string CreateLogHeader()
		{
			string header = @"<?xml version='1.0'?>
<Log>
";
			
			return header;
		}
		
		private static string GetStackTracePath(Guid id)
		{
			return GetStackTraceDirectoryPath() + Path.DirectorySeparatorChar + id.ToString() + ".trace";
		}
		
		/// <summary>
		/// Creates the stack trace for the current entry.
		/// </summary>
		/// <returns></returns>
		private static string CreateStackTrace()
		{
			StringBuilder builder = new StringBuilder();
			
			StackTrace stackTrace = new StackTrace();           // get call stack
			StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

			// write call stack method names
			foreach (StackFrame stackFrame in stackFrames)
			{
				if (!SkipFrame(stackFrame))
				{
					builder.Append(stackFrame.GetMethod().DeclaringType.ToString());
					builder.Append(":");
					builder.Append(stackFrame.GetMethod().ToString());// write method name
					builder.Append("\n");
				}
			}
			
			return builder.ToString();
		}
		
		private static bool SkipFrame(StackFrame frame)
		{
			Type component = frame.GetMethod().DeclaringType;
			
			string[] skippableComponents = new string[] {
				"AppLogger",
				"LogWriter",
				"DiagnosticState"
			};
			
			foreach (string skippableComponent in skippableComponents)
			{
				if (component.Name == skippableComponent)
					return true;
			}
			
			return false;
		}
		
		private static string GetStackTraceDirectoryPath()
		{
			return StateAccess.State.PhysicalApplicationPath + Path.DirectorySeparatorChar + "App_Data"
				+ Path.DirectorySeparatorChar + "Logs"
				+ Path.DirectorySeparatorChar + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day
				+ Path.DirectorySeparatorChar + "StackTrace";
		}
		
		private void EndLog()
		{
			Write("</Log>");
			
			CommitBuffer();
		}
		
		protected override void Dispose(bool disposing)
		{
			// If state isn't initialized then don't try to end the log as the state data is needed for the buffer
			if (StateAccess.IsInitialized)
				EndLog();
			
			base.Dispose(disposing);
		}
	}
}
