using System;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Used to split logs into threads.
	/// A thread is a section of logging output and usually has one thread per application request/operation.
	/// </summary>
	public class LogThreader
	{
		private string threadsDirectoryName = "Threads";
		/// <summary>
		/// Gets/sets the name of the directory containing the log threads.
		/// </summary>
		public string ThreadsDirectoryName
		{
			get { return threadsDirectoryName; }
			set { threadsDirectoryName = value; }
		}
		
		private string threadsIndexFileName = "Index.xml";
		/// <summary>
		/// Gets/sets the name of the file containing an index of all the threads.
		/// </summary>
		public string ThreadsIndexFileName
		{
			get { return threadsIndexFileName; }
			set { threadsIndexFileName = value; }
		}
		
		/// <summary>
		/// Gets the full path to the threads index file.
		/// </summary>
		public string ThreadsIndexFilePath
		{
			get { return ThreadsDirectoryPath + Path.DirectorySeparatorChar + ThreadsIndexFileName; }
		}
			
		/// <summary>
		/// Gets the full path to the threads directory.
		/// </summary>
		public string ThreadsDirectoryPath
		{
			get {
				CheckLogsDirectoryPath();
				CheckDateStamp();
				return LogsDirectoryPath + Path.DirectorySeparatorChar + LogDateStamp + Path.DirectorySeparatorChar + ThreadsDirectoryName; }
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
		
		/// <summary>
		/// Sets the logs directory path.
		/// </summary>
		/// <param name="logsDirectoryPath">The full path to the logs directory.</param>
		/// <param name="logDateStamp">The short date stamp of the log being threaded.</param>
		public LogThreader(string logsDirectoryPath, string logDateStamp)
		{
			LogsDirectoryPath = logsDirectoryPath;
			LogDateStamp = logDateStamp.Replace("/", "-");
		}
		
		/// <summary>
		/// Splits the log in the specified directory into threads and places those threads into the threads directory.
		/// </summary>
		/// <param name="dir"></param>
		public void SplitThreads()
		{
			CheckLogsDirectoryPath();
			
			string file = LogsDirectoryPath + Path.DirectorySeparatorChar + LogDateStamp + Path.DirectorySeparatorChar + RepairedLogFileName;
			string indexFile = ThreadsDirectoryPath + Path.DirectorySeparatorChar + ThreadsIndexFileName;
			
			Guid threadID = Guid.Empty;
			
			RepairLog();
			
			if (File.Exists(file))
			{
				XmlDocument indexDoc = new XmlDocument();
				indexDoc.AppendChild(indexDoc.CreateElement("Index"));
				
				
				XmlDocument doc = new XmlDocument();
				doc.Load(file);

				XmlNodeList nodeList;
				XmlNode root = doc.DocumentElement;
				
				if (root == null)
					throw new Exception("Document element missing.");
				
				//	throw new Exception("sdf" + root.Name);

				nodeList=root.SelectNodes("Entry");
				
				if (nodeList == null)
					throw new Exception("No 'Entry' nodes found.");
				
				XmlNode threadRoot = null;
				
				XmlDocument threadDoc = CreateNewThread();
				XmlDocument previousThreadDoc = null;
				
				
				string threadTitle = string.Empty;
				string threadFile = string.Empty;
				
				string previousThreadTitle = String.Empty;
				
				// OBSOLETE COMMENT: The analyses should only run if the /Detail/ directory isn't already there. If it is there then it's already been run and should be skipped.
				if (!Directory.Exists(LogsDirectoryPath + Path.DirectorySeparatorChar + ThreadsDirectoryName))
				{
					Directory.CreateDirectory(LogsDirectoryPath + Path.DirectorySeparatorChar + ThreadsDirectoryName);
				}
				
				//Change the price on the books.
				foreach (XmlNode node in nodeList)
				{
					XmlNode indentNode = node.SelectSingleNode("Indent");
					if (indentNode == null)
						throw new Exception("Indent node not found.");
					
					
					int indent = Convert.ToInt32(indentNode.InnerText);
					
					// If this is the thread root then move it to a new thread
					if (IsThreadRoot(node))
					{
						// Save the previous thread
						if (previousThreadDoc != null)
						{
							SaveThread(previousThreadDoc, threadID);
							AddThreadToIndex(indexDoc, threadID, previousThreadTitle);
						}
						
						threadID = Guid.NewGuid();
						
						// Switch the current thread to the previous one
						previousThreadDoc = threadDoc;
						previousThreadTitle = threadTitle;
						
						XmlNode componentNode = node.SelectSingleNode("Component");
						
						if (componentNode == null)
							throw new Exception("No component node found.");
						
						XmlNode methodNode = node.SelectSingleNode("Method");
						
						if (methodNode == null)
							throw new Exception("No method node found.");
						
						// Create the title of the new thread
						string componentName = componentNode.InnerText;
						string methodName = methodNode.InnerText;
						//string nextTimeStampString = node.SelectSingleNode("Timestamp").InnerText;
						
						threadTitle = componentName + "." + methodName;
						
						threadDoc = CreateNewThread();
						
						
					}
					//else
					//{
					
					//}
					if (threadDoc == null)
						throw new Exception("Thread doc == null");
					
					threadRoot = threadDoc.DocumentElement;
					
					if (threadRoot == null)
						throw new Exception("Thread root node is null");
					
					threadRoot.AppendChild(threadDoc.ImportNode(node, true));
					
					//throw new Exception(indent.ToString());
				}
				
				
				if (threadDoc != null)
				{
					SaveThread(threadDoc, threadID);
					AddThreadToIndex(indexDoc, threadID, threadTitle);
				}
				//}
				
				if (indexDoc == null)
					throw new Exception("Index doc == null");
				
				indexDoc.Save(indexFile);
			}
		}
		
		private bool IsThreadRoot(XmlNode node)
		{
			XmlNode subNode = node.SelectSingleNode("Indent");
			
			if (subNode != null && subNode.InnerText.ToLower() == 0.ToString())
				return true;
			
			return false;
		}
		
		public void SaveThread(XmlDocument threadDoc, Guid threadID)
		{
			if (threadDoc != null)
			{
				string threadFile = ThreadsDirectoryPath + Path.DirectorySeparatorChar + threadID + ".xml";
				
				if (!Directory.Exists(Path.GetDirectoryName(threadFile)))
					Directory.CreateDirectory(Path.GetDirectoryName(threadFile));
				
				threadDoc.Save(threadFile);
			}
		}
		
		public XmlDocument CreateNewThread()
		{
			XmlDocument threadDoc = new XmlDocument();
			
			XmlNode documentNode = threadDoc.CreateNode(XmlNodeType.Element, "Log", "");
			threadDoc.AppendChild(documentNode);
			
			return threadDoc;
			
		}
		
		public void AddThreadToIndex(XmlDocument indexDoc, Guid threadID, string threadTitle)
		{
			// Add the thread to the index
			XmlNode threadIndexNode = indexDoc.CreateElement("Thread");
			
			XmlAttribute idAttribute = indexDoc.CreateAttribute("ID");
			idAttribute.Value = threadID.ToString();
			threadIndexNode.Attributes.Append(idAttribute);
			
			XmlAttribute titleAttribute = indexDoc.CreateAttribute("Title");
			titleAttribute.Value = threadTitle.ToString();
			threadIndexNode.Attributes.Append(titleAttribute);
			
			indexDoc.DocumentElement.AppendChild(threadIndexNode);
			
		}
		
		public string PrepareFileName(string text)
		{
			
			// first trim the raw string

			string safe = text.Trim();

			

			// replace spaces with hyphens

			safe = safe.Replace(" ", "-");

			

			// replace any 'double spaces' with singles

			if(safe.IndexOf("--") > -1)

				while(safe.IndexOf("--") > -1)
				safe = safe.Replace("--", "-");
			
			// trim out illegal characters
			safe = Regex.Replace(safe, "[^a-zA-Z0-9\\-]", "_");
			
			// trim the length
			if(safe.Length > 50)
				safe = safe.Substring(0, 49);
			
			// clean the beginning and end of the filename
			char[] replace = {'-','.'};
			safe = safe.TrimStart(replace);
			safe = safe.TrimEnd(replace);

			return safe;
		}
		
		public void RepairLog()
		{
			new LogRepairer(LogsDirectoryPath, LogDateStamp).RepairLog();

		}
		
		public string[] GetRootParts(string dir)
		{
			List<string> list = new List<string>();
			foreach (string file in Directory.GetFiles(dir))
			{
				list.Add(Path.GetFileName(file));
			}
			return list.ToArray();
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
