using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Description of LogUtilities.
	/// </summary>
	public class LogUtilities
	{
		static public void RepairLog(string logsDirectoryPath, string logDateStamp)
		{
			new LogRepairer(logsDirectoryPath, logDateStamp).RepairLog();
		}
		
		static public string[] GetLogDates()
		{
			if (!StateAccess.IsInitialized)
				throw new InvalidOperationException("The state has not been initialized.");
			
			string logsDirectory = StateAccess.State.PhysicalApplicationPath + @"\App_Data\Logs\";
			
			List<string> list = new List<string>();
			
			foreach (string directory in Directory.GetDirectories(logsDirectory))
			{
				// Add just the date section
				list.Add(Path.GetFileName(directory));
				
			}
			
			return list.ToArray();
		}
		
		static public XmlElement[] GetErrors(string dateStamp)
		{
			List<XmlElement> errors = new List<XmlElement>();
			
			string logsDirectory = StateAccess.State.PhysicalApplicationPath + @"\App_Data\Logs";
			
			RepairLog(logsDirectory, dateStamp);
			
			string logDirectory = logsDirectory + @"\" + dateStamp;
			
			string logFile = logDirectory + @"\RepairedLog.xml";
			
			XmlDocument doc = new XmlDocument();
			
			doc.Load(logFile);
			
			foreach (XmlNode node in doc.DocumentElement.ChildNodes)
			{
				string level = node.SelectSingleNode("LogLevel").InnerText;
				
				if (level == "Error")
					errors.Add((XmlElement)node);
			}
			
			return errors.ToArray();
		}
		
		static public string GetMessage(XmlElement element)
		{
			return element.SelectSingleNode("Data").InnerText;
		}
		
		static public string GetTime(XmlElement element)
		{
			return element.SelectSingleNode("Timestamp").InnerText;
		}
		
		static public string GetComponent(XmlElement element)
		{
			return element.SelectSingleNode("Component").InnerText;
		}
		
		static public string GetMethod(XmlElement element)
		{
			return element.SelectSingleNode("Method").InnerText;
		}
	}
	
}
