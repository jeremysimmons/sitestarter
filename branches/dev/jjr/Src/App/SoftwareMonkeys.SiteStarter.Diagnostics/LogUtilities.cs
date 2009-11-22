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
			
			FinalizeLog(dir);
			
			if (File.Exists(file))
			{
				/*XmlReader reader = new XmlTextReader(file);
				
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if (reader.Name == "Entry")
						{
							string indentString = reader.ReadElementString("Indent");
							
							int indent = Int32.Parse(indentString);
							
							if (indent == 0)
							{
								string component = reader.ReadElementString("Component");
								string method = reader.ReadElementString("Method");
								
								string title = component + "." + method;
								
								throw new Exception(title);
							}
						}
					}
				}*/
				
				XmlDocument doc = new XmlDocument();
				doc.Load(file);

				XmlNodeList nodeList;
				XmlNode root = doc.DocumentElement;
				
				//	throw new Exception("sdf" + root.Name);

				nodeList=root.SelectNodes("Entry");
				
				XmlNode threadRoot = null;
				
				XmlDocument threadDoc = null;
				
				
				string threadTitle = string.Empty;
				string threadFile = string.Empty;
				
				// OBSOLETE COMMENT: The analyses should only run if the /Detail/ directory isn't already there. If it is there then it's already been run and should be skipped.
				if (!Directory.Exists(dir.TrimEnd('\\') + @"\Detail\"))
				{
					Directory.CreateDirectory(dir.TrimEnd('\\') + @"\Detail\");
				}
					
					//Change the price on the books.
					foreach (XmlNode node in nodeList)
					{
						
						
						int indent = Convert.ToInt32(node.SelectSingleNode("Indent").InnerText);
						
						// If this is the thread root
						if (indent == 0)
						{
							
							string componentName = node.SelectSingleNode("Component").InnerText;
							string methodName = node.SelectSingleNode("Method").InnerText;
							
							threadTitle = componentName + "." + methodName;
							threadFile = dir.TrimEnd('\\') + @"\Detail\" + PrepareFileName(threadTitle) + ".xml";
							
							// Save the previous thread
							
							if (threadDoc != null)
							{
								threadDoc.Save(threadFile);
							}
							
							
							// Create the new thread
							
							
							threadDoc = new XmlDocument();
							XmlNode documentNode = threadDoc.CreateNode(XmlNodeType.Element, "Log", "");
							threadDoc.AppendChild(documentNode);
							threadRoot = node;
							
						}
						//else
						//{
						
						//}
						
						threadDoc.DocumentElement.AppendChild(threadDoc.ImportNode(node, true));
						
						//throw new Exception(indent.ToString());
					}
					
					
					if (threadDoc != null)
					{
						threadDoc.Save(threadFile);
					}
				//}
			}
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
