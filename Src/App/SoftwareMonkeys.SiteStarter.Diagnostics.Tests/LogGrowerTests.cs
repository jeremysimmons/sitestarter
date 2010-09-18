using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests;
using System.Xml;

namespace SoftwareMonkeys.SiteStarter.Diagnostics.Tests
{
	[TestFixture]
	public class LogGrowerTests : BaseDiagnosticsTestFixture
	{
		public LogGrowerTests()
		{
		}
		
		[Test]
		public void Test_CreateTreeNode()
		{
			XmlDocument targetDocument = new XmlDocument();
			
			XmlDocument sourceDoc = CreateMockThreadDocument();
			
			XmlElement sourceElement = (XmlElement)sourceDoc.DocumentElement.ChildNodes[0];
			
			LogGrower grower = new LogGrower(TestUtilities.GetTestingPath(this));
			
			XmlElement newElement = grower.CreateTreeNode(targetDocument, sourceElement);
			
			Assert.AreEqual(7, newElement.Attributes.Count, "Invalid number of sub nodes found in the entry.");
			
			for (int i = 0; i < 7; i++)
			{
				Assert.AreEqual(sourceElement.ChildNodes[i].InnerText, newElement.Attributes[i].InnerText, "The inner text of the sub elements wasn't transferred to the attributes.");
			}
		}
		
		/// <summary>
		/// Tests the AddNodeToTree function where the previous element passed is null.
		/// </summary>
		[Test]
		public void Test_AddNodeToTree_NullPrevious()
		{
			XmlDocument treeDoc = CreateMockThreadDocument();
			
			XmlElement element = CreateMockTreeElement(treeDoc, 1);
			
			LogGrower grower = new LogGrower(TestUtilities.GetTestDataPath(this, "MockApplication"));
			grower.AddNodeToTree(treeDoc, null, element);
			
			int expected = 2; // Existing mock entry plus new entry = 2 total
			Assert.AreEqual(expected, treeDoc.DocumentElement.ChildNodes.Count, "The element wasn't added to the document element.");
		}
		
		/// <summary>
		/// Tests the AddNodeToTree function where the previous element passed is the parent of the new element.
		/// </summary>
		[Test]
		public void Test_AddNodeToTree_PreviousIsParent()
		{
			XmlDocument treeDoc = CreateMockTreeDocument();
			
			XmlElement element = CreateMockTreeElement(treeDoc, 2);
			
			XmlElement parentElement = (XmlElement)treeDoc.DocumentElement.ChildNodes[0];
			
			LogGrower grower = new LogGrower(TestUtilities.GetTestDataPath(this, "MockApplication"));
			grower.AddNodeToTree(treeDoc, parentElement, element);
			
			Assert.AreEqual(1, treeDoc.DocumentElement.ChildNodes.Count, "Document element has invalid number of child elements.");
			Assert.AreEqual(1, treeDoc.DocumentElement.ChildNodes[0].ChildNodes.Count, "New element wasn't added to parent.");
		}
			
		/// <summary>
		/// Tests the AddNodeToTree function where the previous element passed is a sibling of the new element.
		/// </summary>
		[Test]
		public void Test_AddNodeToTree_PreviousIsSibling()
		{
			XmlDocument treeDoc = CreateMockTreeDocument();
			
			XmlElement element = CreateMockTreeElement(treeDoc, 1);
			
			XmlElement parentElement = (XmlElement)treeDoc.DocumentElement.ChildNodes[0];
			
			LogGrower grower = new LogGrower(TestUtilities.GetTestDataPath(this, "MockApplication"));
			grower.AddNodeToTree(treeDoc, parentElement, element);
			
			Assert.AreEqual(2, treeDoc.DocumentElement.ChildNodes.Count, "Document element has invalid number of child elements.");
		}
		
		
			
		/// <summary>
		/// Tests the AddNodeToTree function where the previous element passed is indented further than the new element.
		/// </summary>
		[Test]
		public void Test_AddNodeToTree_PreviousIsDeeper()
		{
			XmlDocument treeDoc = CreateMockTreeDocument();
			
			XmlElement deeperElement = CreateMockTreeElement(treeDoc, 2);
			
			treeDoc.DocumentElement.ChildNodes[0].AppendChild(deeperElement);
			
			XmlElement element = CreateMockTreeElement(treeDoc, 1);
						
			LogGrower grower = new LogGrower(TestUtilities.GetTestDataPath(this, "MockApplication"));
			grower.AddNodeToTree(treeDoc, deeperElement, element);
			
			int expectedCount = 2; // default one plus new one = 2
			Assert.AreEqual(expectedCount, treeDoc.DocumentElement.ChildNodes[0].ChildNodes.Count, "Invalid number of elements. New element wasn't put in correct location.");
		}

		/// <summary>
		/// Tests the GetParent function where the previous node is the parent of the new node.
		/// </summary>		
		[Test]
		public void Test_GetParent_PreviousIsParent()
		{
			XmlDocument treeDoc = CreateMockTreeDocument();
			
			XmlElement newNode = CreateMockTreeElement(treeDoc, 2);
			
			XmlElement previous = (XmlElement)treeDoc.DocumentElement.ChildNodes[0];
			
			LogGrower grower = new LogGrower(TestUtilities.GetTestDataPath(this, "MockApplication"));
			
			XmlElement parent = grower.GetParent(previous, newNode);
			
			Assert.AreEqual(previous, parent, "Incorrect parent returned.");
		}
		
		/// <summary>
		/// Tests the GetParent function where the previous node is at a deeper indent level than the new node.
		/// </summary>		
		[Test]
		public void Test_GetParent_PreviousIsDeeper()
		{
			
			XmlDocument treeDoc = CreateMockTreeDocument();
			
			XmlElement previous = CreateMockTreeElement(treeDoc, 2);
			
			treeDoc.DocumentElement.ChildNodes[0].AppendChild(previous);
			
			XmlElement newNode = CreateMockTreeElement(treeDoc, 1);
			
			LogGrower grower = new LogGrower(TestUtilities.GetTestDataPath(this, "MockApplication"));
			
			XmlElement parent = grower.GetParent(previous, newNode);
			
			int expectedParentIndent = Convert.ToInt32(treeDoc.DocumentElement.ChildNodes[0].Attributes["Indent"].Value);
			int actualParentIndent = Convert.ToInt32(parent.Attributes["Indent"].Value);
			
			Assert.AreEqual(expectedParentIndent, actualParentIndent, "Incorrect parent indent.");
		}
		
			
		/// <summary>
		/// Tests the GetParent function where the previous node is a sibling of the new node.
		/// </summary>		
		[Test]
		public void Test_GetParent_PreviousIsSibling()
		{
			XmlDocument treeDoc = CreateMockTreeDocument();
			
			XmlElement newNode = CreateMockTreeElement(treeDoc, 1);
			
			XmlElement previous = (XmlElement)treeDoc.DocumentElement.ChildNodes[0];
			
			LogGrower grower = new LogGrower(TestUtilities.GetTestDataPath(this, "MockApplication"));
			
			XmlElement parent = grower.GetParent(previous, newNode);
			
			Assert.AreEqual(treeDoc.DocumentElement, parent, "Incorrect parent returned.");
		}
		
		public XmlDocument CreateMockThreadDocument()
		{
			XmlDocument doc = new XmlDocument();
			
			XmlElement logElement = doc.CreateElement("Log");
			doc.AppendChild(logElement);
			
			logElement.AppendChild(CreateMockThreadElement(doc, 1));

			return doc;
		}
		
		
		public XmlDocument CreateMockTreeDocument()
		{
			XmlDocument doc = new XmlDocument();
			
			XmlElement logElement = doc.CreateElement("Log");
			doc.AppendChild(logElement);
			
			logElement.AppendChild(CreateMockTreeElement(doc, 1));

			return doc;
		}
		
		public XmlElement CreateMockThreadElement(XmlDocument doc, int indent)
		{
			// Entry group element
			XmlElement entryElement = doc.CreateElement("Entry");	
			
			// Group ID element
			XmlElement groupIDElement = doc.CreateElement("GroupID");
			groupIDElement.InnerText = Guid.NewGuid().ToString();
			entryElement.AppendChild(groupIDElement);
			
			// Log level element
			XmlElement logLevelElement = doc.CreateElement("LogLevel");
			logLevelElement.InnerText = "Debug";
			entryElement.AppendChild(logLevelElement);
			
			// Time stamp element
			XmlElement timeStampElement = doc.CreateElement("Timestamp");
			timeStampElement.InnerText = DateTime.Now.ToString();
			entryElement.AppendChild(timeStampElement);

			// Indent element
			XmlElement indentElement = doc.CreateElement("Indent");
			indentElement.InnerText = indent.ToString();
			entryElement.AppendChild(indentElement);

			// Component element
			XmlElement componentElement = doc.CreateElement("Component");
			componentElement.InnerText = "SoftwareMonkeys.SiteStarter.MockNamespace.MockComponent";
			entryElement.AppendChild(componentElement);

			// Method element
			XmlElement methodElement = doc.CreateElement("Method");
			methodElement.InnerText = "MockMethod";
			entryElement.AppendChild(methodElement);

			// Data element
			XmlElement dataElement = doc.CreateElement("Data");
			dataElement.InnerText = "...mock data ....";
			entryElement.AppendChild(dataElement);
			
			// Stack trace element
			XmlElement stackTraceElement = doc.CreateElement("StackTrace");
			stackTraceElement.InnerText = "...........";
			entryElement.AppendChild(stackTraceElement);
			
			return entryElement;
		}
		
		
		public XmlElement CreateMockTreeElement(XmlDocument doc, int indent)
		{
			// Entry group element
			XmlElement entryElement = doc.CreateElement("Entry");	
			
			// Group ID element
			XmlAttribute groupIDAttribute = doc.CreateAttribute("GroupID");
			groupIDAttribute.InnerText = Guid.NewGuid().ToString();
			entryElement.Attributes.Append(groupIDAttribute);
			
			// Log level element
			XmlAttribute logLevelAttribute = doc.CreateAttribute("LogLevel");
			logLevelAttribute.InnerText = "Debug";
			entryElement.Attributes.Append(logLevelAttribute);
			
			// Time stamp element
			XmlAttribute timeStampAttribute = doc.CreateAttribute("Timestamp");
			timeStampAttribute.InnerText = DateTime.Now.ToString();
			entryElement.Attributes.Append(timeStampAttribute);

			// Indent element
			XmlAttribute indentAttribute = doc.CreateAttribute("Indent");
			indentAttribute.InnerText = indent.ToString();
			entryElement.Attributes.Append(indentAttribute);

			// Component element
			XmlAttribute componentAttribute = doc.CreateAttribute("Component");
			componentAttribute.InnerText = "SoftwareMonkeys.SiteStarter.MockNamespace.MockComponent";
			entryElement.Attributes.Append(componentAttribute);

			// Method element
			XmlAttribute methodAttribute = doc.CreateAttribute("Method");
			methodAttribute.InnerText = "MockMethod";
			entryElement.Attributes.Append(methodAttribute);

			// Data element
			XmlAttribute dataAttribute = doc.CreateAttribute("Data");
			dataAttribute.InnerText = "...mock data ....";
			entryElement.Attributes.Append(dataAttribute);
			
			return entryElement;
		}
	}
}
