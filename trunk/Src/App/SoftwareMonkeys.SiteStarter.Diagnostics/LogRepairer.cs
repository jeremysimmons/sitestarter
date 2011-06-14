using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Description of LogRepairer.
	/// </summary>
	public class LogRepairer
	{
		private string logFileName = "Log.xml";
		/// <summary>
		/// Gets/sets the name of the repaired log file.
		/// </summary>
		public string LogFileName
		{
			get { return logFileName; }
			set { logFileName = value; }
		}		
		
		/// <summary>
		/// Gets the full path to the threads index file.
		/// </summary>
		public string LogFilePath
		{
			get { return LogsDirectoryPath + Path.DirectorySeparatorChar + LogDateStamp + Path.DirectorySeparatorChar + LogFileName; }
		}
		
		private string repairedLogFileName = "RepairedLog.xml";
		/// <summary>
		/// Gets/sets the name of the repaired log file.
		/// </summary>
		public string RepairedLogFileName
		{
			get { return repairedLogFileName; }
			set { repairedLogFileName = value; }
		}
		
		
		private string logsDirectoryPath;
		/// <summary>
		/// Gets/sets the full path to the directory containing the logs.
		/// </summary>
		public string LogsDirectoryPath
		{
			get { return logsDirectoryPath; }
			set { logsDirectoryPath = value; }
		}
		
		private string logDateStamp;
		/// <summary>
		/// Gets/sets the short date stamp of the log being threaded.
		/// </summary>
		public string LogDateStamp
		{
			get { return logDateStamp; }
			set { logDateStamp = value; }
		}		
		
		/// <summary>
		/// Sets the logs directory path.
		/// </summary>
		/// <param name="logsDirectoryPath">The full path to the logs directory.</param>
		/// <param name="logDateStamp">The short date stamp of the log being threaded.</param>
		public LogRepairer(string logsDirectoryPath, string logDateStamp)
		{
			LogsDirectoryPath = logsDirectoryPath;
			LogDateStamp = logDateStamp.Replace("/", "-");
		}
		
		public void RepairLog()
		{
			CheckLogsDirectoryPath();
			CheckDateStamp();
			
			string logPath = LogsDirectoryPath + Path.DirectorySeparatorChar + LogDateStamp + Path.DirectorySeparatorChar + LogFileName;
			string fixedLogPath = LogsDirectoryPath + Path.DirectorySeparatorChar + LogDateStamp + Path.DirectorySeparatorChar + RepairedLogFileName;
			
			string logContents;
			
			using (StreamReader reader = new StreamReader(logPath))
			{
				logContents = reader.ReadToEnd();
				reader.Close();
			}
			
			
			// Remove any duplicate <Log>...</Log> tags
			logContents = new Regex("</Log>(.|\r\n)*?<Log>", RegexOptions.IgnoreCase | RegexOptions.Multiline)
				.Replace(logContents, "");
			
			// Add the start <Log> tag if necessary
			string startTag = "<Log>";
			if (logContents.IndexOf(startTag) == -1)
				logContents = startTag + "\r\n" + logContents;
			
			// Add the XML declaration if necessary
			// This must be done before after the start tag prepend won't work
			string xmlDeclaration = "<?xml version=\'1.0\'?>";
			if (logContents.IndexOf(xmlDeclaration) == -1)
				logContents = xmlDeclaration + "\r\n" + logContents;

			// Add the ending </Log> tag if necessary
			if (logContents.IndexOf("</Log>") == -1)
				logContents = logContents + "</Log>";
			
			
			using (StreamWriter writer = File.CreateText(fixedLogPath))
			{
				writer.Write(logContents);
				writer.Close();
			}

		}
		
		public void CheckLogsDirectoryPath()
		{
			if (LogsDirectoryPath == String.Empty || LogsDirectoryPath == null)
				throw new InvalidOperationException("The LogsDirectoryPath property hasn't been set.");
		}
		
		public void CheckDateStamp()
		{
			if (LogDateStamp == String.Empty || LogDateStamp == null)
				throw new InvalidOperationException("The LogDateStamp property hasn't been set.");
		}
	}
}
