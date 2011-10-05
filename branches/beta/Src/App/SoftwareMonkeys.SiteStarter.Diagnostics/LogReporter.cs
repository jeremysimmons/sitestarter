using System;
using System.IO;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Used to report the logs into a separate directory so they can be reviewed and won't be cleared. In a live application the default path is /App_Data/LogReports/.
	/// </summary>
	public class LogReporter
	{
		private string logInputPath = String.Empty;
		public string LogInputPath
		{
			get { return logInputPath; }
			set { logInputPath = value; }
		}
		
		private string logOutputPath = String.Empty;
		public string LogOutputPath
		{
			get { return logOutputPath; }
			set { logOutputPath = value; }
		}
		
		private string newFileName = String.Empty;
		public string NewFileName
		{
			get { return newFileName; }
			set { newFileName = value; }
		}
		
		public LogReporter()
		{
			LogOutputPath = StateAccess.State.PhysicalApplicationPath + Path.DirectorySeparatorChar
				+ "App_Data" + Path.DirectorySeparatorChar
				+ "LogReports";
		}
		
		public LogReporter(string inputPath, string outputPath, string newFileName)
		{
			LogInputPath = GetLogPath(inputPath);
			LogOutputPath = outputPath;
			NewFileName = newFileName;
		}
		
		public LogReporter(string outputPath)
		{
			LogInputPath = GetLogPath();
			LogOutputPath = outputPath;
		}
		
		public void Report(string label)
		{
			string logPath = LogInputPath;
			
			string newLogPath = LogOutputPath.TrimEnd('\\') + Path.DirectorySeparatorChar
				+ label;
			
			if (logPath != String.Empty)
			{
				// Create the parent folder
				if (!Directory.Exists(Path.GetDirectoryName(newLogPath)))
					Directory.CreateDirectory(Path.GetDirectoryName(newLogPath));
				
				//throw new Exception(newLogPath);
				Move(logPath, newLogPath);
			}
			// otherwise skip log reporting
		}
		
		public void Move(string from, string to)
		{
			if (Directory.Exists(from))
			{				
				foreach (string file in Directory.GetFiles(from))
				{
					string fileName = Path.GetFileName(file);
					
					// If a new file name was specified then rename Log.xml file
					if (fileName == "Log.xml" && NewFileName != String.Empty)
						fileName = NewFileName;
					
					string toFile = to + Path.DirectorySeparatorChar + fileName;
					
					if (!Directory.Exists(to))
						Directory.CreateDirectory(to);
					
					if (File.Exists(toFile))
						File.Delete(toFile);
					
					try
					{
						File.Move(file, toFile);
					}
					catch (PathTooLongException ex)
					{
						throw new Exception("The path is too long: " + toFile, ex);
					}
				}
				
				foreach (string subFrom in Directory.GetDirectories(from))
				{
					string subTo = to + Path.DirectorySeparatorChar + Path.GetFileName(subFrom);
					
					Move(subFrom, subTo);
				}
			}
		}
		
		public string GetLogPath()
		{
			return GetLogPath(StateAccess.State.PhysicalApplicationPath + @"\App_Data\Logs");
		}

		/// <summary>
		/// Gets the date specific log path from the provided base logs path.
		/// </summary>
		/// <param name="baseLogsPath"></param>
		/// <returns></returns>
		public string GetLogPath(string baseLogsPath)
		{
			string logsDirectoryPath = baseLogsPath;

			string path = String.Empty;
			
			if (Directory.Exists(logsDirectoryPath))
			{
				// Get the first (and should be only) directory
				foreach (string directory in Directory.GetDirectories(logsDirectoryPath))
				{
					path = directory;
				}
			}
			
			return path;
		}
		
	}
}
