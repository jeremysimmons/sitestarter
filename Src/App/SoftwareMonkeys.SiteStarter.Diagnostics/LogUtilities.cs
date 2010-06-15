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
			if (dir == null)
				throw new ArgumentNullException("dir");
			
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
				if (!Directory.Exists(dir.TrimEnd('\\') + @"\Detail\"))
				{
					Directory.CreateDirectory(dir.TrimEnd('\\') + @"\Detail\");
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
							SaveThread(dir, previousThreadDoc, threadID);
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
					SaveThread(dir, threadDoc, threadID);
					AddThreadToIndex(indexDoc, threadID, threadTitle);
				}
				//}
				
				if (indexDoc == null)
					throw new Exception("Index doc == null");
				
				indexDoc.Save(indexFile);
			}
		}
		
		static private bool IsThreadRoot(XmlNode node)
		{
			XmlNode subNode = node.SelectSingleNode("Indent");
			
			if (subNode != null && subNode.InnerText.ToLower() == 0.ToString())
				return true;
			
			return false;
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
