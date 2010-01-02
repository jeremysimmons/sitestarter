using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Description of LogUtilities.
	/// </summary>
	public class LogUtilities
	{
		
		static public void AnalyzeLog(string dir)
		{
			string file = dir.TrimEnd('\\') + @"\FinalizedLog.xml";
			string indexFile = dir.TrimEnd('\\') + @"\Detail\Index.xml";
			
			Guid threadID = Guid.Empty;
			
			FinalizeLog(dir);
			
			if (File.Exists(file))
			{			
				XmlDocument indexDoc = new XmlDocument();
				indexDoc.AppendChild(indexDoc.CreateElement("Index"));
				
				
				XmlDocument doc = new XmlDocument();
				doc.Load(file);

				XmlNodeList nodeList;
				XmlNode root = doc.DocumentElement;
				
				//	throw new Exception("sdf" + root.Name);

				nodeList=root.SelectNodes("Entry");
				
				XmlNode threadRoot = null;
				
				XmlDocument threadDoc = null;
				XmlDocument previousThreadDoc = null;
				
				
				string threadTitle = string.Empty;
				string threadFile = string.Empty;
				
				string previousThreadTitle = String.Empty;
				
				// OBSOLETE COMMENT: The analyses should only run if the /Detail/ directory isn't already there. If it is there then it's already been run and should be skipped.
				if (!Directory.Exists(dir.TrimEnd('\\') + @"\Detail\"))
				{
					Directory.CreateDirectory(dir.TrimEnd('\\') + @"\Detail\");
				}
				
				//Change the price on the books.
				foreach (XmlNode node in nodeList)
				{
					
					int indent = Convert.ToInt32(node.SelectSingleNode("Indent").InnerText);
					
					// If this is the thread root then move it to a new thread
					if (indent == 0)
					{						
						// Save the previous thread
						if (previousThreadDoc != null)
						{
							SaveThread(dir, previousThreadDoc, threadID);
							AddThreadToIndex(indexDoc, threadID, previousThreadTitle);
						}
						
						threadID = Guid.NewGuid();
						
						// Switch the current thread to the previous one
						previousThreadDoc = threadDoc;
						previousThreadTitle = threadTitle;
						
						// Create the title of the new thread
						string componentName = node.SelectSingleNode("Component").InnerText;
						string methodName = node.SelectSingleNode("Method").InnerText;
						//string nextTimeStampString = node.SelectSingleNode("Timestamp").InnerText;
						
						threadTitle = componentName + "." + methodName;
						
						threadDoc = CreateNewThread();
						
						
					}
					//else
					//{
					
					//}
					
					threadDoc.DocumentElement.AppendChild(threadDoc.ImportNode(node, true));
					
					//throw new Exception(indent.ToString());
				}
				
				
				if (threadDoc != null)
				{
					SaveThread(dir, threadDoc, threadID);
					AddThreadToIndex(indexDoc, threadID, threadTitle);
				}
				//}
				
				indexDoc.Save(indexFile);
			}
		}
		
		static public void SaveThread(string rootDir, XmlDocument threadDoc, Guid threadID)
		{
			if (threadDoc != null)
			{
				string threadFile = rootDir.TrimEnd('\\') + @"\Detail\" + threadID + ".xml";
				threadDoc.Save(threadFile);
			}
		}
		
		static public XmlDocument CreateNewThread()
		{
			XmlDocument threadDoc = new XmlDocument();
			
			XmlNode documentNode = threadDoc.CreateNode(XmlNodeType.Element, "Log", "");
			threadDoc.AppendChild(documentNode);
			
			return threadDoc;
			
		}
		
		static public void AddThreadToIndex(XmlDocument indexDoc, Guid threadID, string threadTitle)
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
		
		static public string PrepareFileName(string text)
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
		
		static public void FinalizeLog(string dir)
		{
			string logPath = dir.TrimEnd('\\') + @"\Log.xml";
			string fixedLogPath = dir.TrimEnd('\\') + @"\FinalizedLog.xml";

			string logContents;
			
			using (StreamReader reader = new StreamReader(logPath))
			{
				logContents = reader.ReadToEnd();
				reader.Close();
			}

			// Add the ending </Log> tag if necessary
			if (logContents.IndexOf("</Log>") == -1)
				logContents = logContents + "</Log>";
			
			
			using (StreamWriter writer = new StreamWriter(fixedLogPath))
			{
				writer.Write(logContents);
				writer.Close();
			}

		}
		
		static public string[] GetRootParts(string dir)
		{
			List<string> list = new List<string>();
			foreach (string file in Directory.GetFiles(dir))
			{
				list.Add(Path.GetFileName(file));
			}
			return list.ToArray();
		}
	}
	
}
