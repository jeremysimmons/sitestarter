using System;
using System.IO;
using System.Xml;

namespace SoftwareMonkeys.SiteStarter.Diagnostics
{
	/// <summary>
	/// Used to turn log thread information into a tree that can be easily displayed.
	/// A log tree is simply a version of a log thread where entries are grouped within the parent entry, with a potentially infinite number of levels.
	/// </summary>
	public class LogGrower
	{
		private string logsDirectoryPath;
		/// <summary>
		/// Gets/sets the full path to the directory containing the logs.
		/// </summary>
		public string LogsDirectoryPath
		{
			get { return logsDirectoryPath; }
			set { logsDirectoryPath = value; }
		}
		
		private string threadsDirectoryName = "Threads";
		/// <summary>
		/// Gets/sets the name of the directory containing the log threads.
		/// </summary>
		public string ThreadsDirectoryName
		{
			get { return threadsDirectoryName; }
			set { threadsDirectoryName = value; }
		}
		
		private string treesDirectoryName = "Trees";
		/// <summary>
		/// Gets/sets the name of the directory containing the log trees.
		/// </summary>
		public string TreesDirectoryName
		{
			get { return treesDirectoryName; }
			set { treesDirectoryName = value; }
		}
		
		private string stackStraceDirectoryName = "StackTrace";
		/// <summary>
		/// Gets/sets the name of the directory containing the stack traces of all the log entries.
		/// </summary>
		public string StackStraceDirectoryName
		{
			get { return stackStraceDirectoryName; }
			set { stackStraceDirectoryName = value; }
		}
		
		/// <summary>
		/// Sets the logs directory path.
		/// </summary>
		/// <param name="logsDirectoryPath"></param>
		public LogGrower(string logsDirectoryPath)
		{
			LogsDirectoryPath = logsDirectoryPath;
		}
		
		/// <summary>
		/// Creates tree formatted logs that can easily be rendered as a tree.
		/// </summary>
		public void CreateTrees()
		{
			string threadsDirectoryPath = LogsDirectoryPath + Path.DirectorySeparatorChar + ThreadsDirectoryName;
			
			foreach (string file in Directory.GetFiles(threadsDirectoryPath))
			{
				CreateTree(file);
			}
		}
		
		/// <summary>
		/// Creates a log tree from the specified thread file.
		/// </summary>
		/// <param name="threadFile">The full path to the thread file to convert into a log tree.</param>
		public void CreateTree(string threadFile)
		{
			XmlDocument treeDoc = new XmlDocument();
			
			XmlDocument threadDoc = CreateThreadDocument(threadFile);
			
			XmlElement previousNode = null;
			
			foreach (XmlNode node in threadDoc.DocumentElement.ChildNodes)
			{
				if (!(node is XmlElement))
					throw new Exception("XmlElement expected but it was: " + node.NodeType.ToString());
				
				XmlElement newNode = CreateTreeNode(treeDoc, (XmlElement)node);
				
				AddNodeToTree(treeDoc, previousNode, newNode);
				
				previousNode = newNode;
			}
			
			/*
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					// Create the node
					XmlElement treeNode = CreateTreeNode(reader);
					
					// Position the node in relation to the parent node
					PositionTreeNode(treeDoc, previousNode, treeNode);
					
					previousNode = treeNode;
				}
			}*/
		}
		
		/// <summary>
		/// Creates an XML document to read the specified thread file.
		/// </summary>
		/// <param name="threadFile">The full path to the thread file being read.</param>
		/// <returns>An XML document containing the specified thread.</returns>
		public XmlDocument CreateThreadDocument(string threadFile)
		{
			XmlDocument document = new XmlDocument();
			document.Load(threadFile);
			
			return document;
		}
		
		/// <summary>
		/// Creates a tree node for the current element on the provided reader.
		/// </summary>
		/// <param name="targetDocument">The document that is going to hold the newly created node.</param>
		/// <param name="reader">The XML node reader to get the log element from.</param>
		/// <returns>The XML node created from the current element of the provided reader.</returns>
		public XmlElement CreateTreeNode(XmlDocument targetDocument, XmlElement element)
		{
			XmlElement newNode = targetDocument.CreateElement(element.Name);
			
			foreach (XmlNode n in element.ChildNodes)
			{
				// Skip the stack trace
				if (n.Name != "StackTrace")
				{
					XmlAttribute attribute = targetDocument.CreateAttribute(n.Name);
					
					attribute.Value = n.InnerText;
					
					newNode.Attributes.Append(attribute);
				}
			}
			
			return newNode;
		}
		
		/// <summary>
		/// Adds the provided new node to the tree.
		/// </summary>
		/// <param name="treeDoc">The document containing the log tree.</param>
		/// <param name="previousNode">The previous node added to the tree.</param>
		/// <param name="newNode">The new node to add to the tree.</param>
		public void AddNodeToTree(XmlDocument treeDoc, XmlElement previousNode, XmlElement newNode)
		{
			// If the previous node is null then add the new node directly to the document element of the tree doc
			if (previousNode == null)
				treeDoc.DocumentElement.AppendChild(newNode);
			else
			{
				XmlElement parent = GetParent(previousNode, newNode);
				parent.AppendChild(newNode);
			}
		}
		
		/// <summary>
		/// Retrieves the parent node of the new node based on the indent level.
		/// </summary>
		/// <param name="previousNode">The node previously added.</param>
		/// <param name="newNode">The new node to get the parent of.</param>
		/// <returns>The potential parent element of the new node.</returns>
		public XmlElement GetParent(XmlElement previousNode, XmlElement newNode)
		{
			
			int previousIndent = Convert.ToInt32(previousNode.Attributes["Indent"].Value);
			int nextIndent = Convert.ToInt32(newNode.Attributes["Indent"].Value);
			
			int difference = previousIndent - nextIndent;
			
			XmlElement parent = previousNode;
			
			// If the next indent is higher than the previous one then the new node is to be added to the previous node
			if (nextIndent == previousIndent)
			{
				parent = (XmlElement)previousNode.ParentNode;
			}
			else
			{
				// Keep moving up the parent chain to the appropriate level
				for (int i = 0; i < difference; i++)
				{
					parent = (XmlElement)parent.ParentNode;
				}
			}
			
			return parent;
		}
	}
}
