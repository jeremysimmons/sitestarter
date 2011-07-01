using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.State;
using System.Web;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Web.UI.WebControls;

namespace SoftwareMonkeys.SiteStarter.Web
{
	public class SiteMap : ISiteMap
	{
		private UrlCreator urlCreator;
		/// <summary>
		/// Gets/sets the URL creator used to generate URLs.
		/// </summary>
		public UrlCreator UrlCreator
		{
			get {
				if (urlCreator == null && HttpContext.Current != null)
				{
					urlCreator = new UrlCreator(HttpContext.Current.Request.ApplicationPath, HttpContext.Current.Request.Url.ToString());
				}
				return urlCreator; }
			set { urlCreator = value; }
		}
		
		static private string defaultFilePath = String.Empty;
		/// <summary>
		/// Gets/sets the full path to the site map file.
		/// </summary>
		static public string DefaultFilePath
		{
			get
			{
				if (defaultFilePath == String.Empty)
				{
					if (HttpContext.Current != null)
					{
						string menuFile = "Menu.sitemap";
						if (PathVariation != String.Empty)
							menuFile = "Menu." + PathVariation + ".sitemap";
						
						defaultFilePath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + "/App_Data/" + menuFile);
					}
				}
				
				return defaultFilePath;
			}
			set
			{
				defaultFilePath = value;
			}
		}
		
		private string filePath = String.Empty;
		/// <summary>
		/// Gets/sets the full path to the site map file.
		/// </summary>
		public string FilePath
		{
			get
			{
				if (filePath == String.Empty)
				{
					filePath = SiteMap.DefaultFilePath;
				}
				
				return filePath;
			}
			set
			{
				filePath = value;
			}
		}
		
		static private string pathVariation = String.Empty;
		/// <summary>
		/// Gets/sets the variation applied to the menu.sitemap file.
		/// </summary>
		static public string PathVariation
		{
			get
			{
				if (pathVariation == String.Empty)
				{
					if (Config.IsInitialized)
						pathVariation = Config.Application.PathVariation;
					else if (HttpContext.Current != null)
						pathVariation = WebUtilities.GetLocationVariation(HttpContext.Current.Request.Url);
				}
				
				return pathVariation;
			}
			set
			{
				pathVariation = value;
			}
		}
		
		private List<SiteMapNode> childNodes;
		/// <summary>
		/// Gets/sets the nodes in the site map.
		/// </summary>
		public List<SiteMapNode> ChildNodes
		{
			get {
				if (childNodes == null)
					childNodes = new List<SiteMapNode>();
				return childNodes; }
			set { childNodes = value; }
		}

		public SiteMapNode this[int index]
		{
			get { return ChildNodes[index]; }
			set { ChildNodes[index] =(SiteMapNode)value; }
		}
		
		
		public SiteMap()
		{
		}

		#region Load/save functions
		/// <summary>
		/// Loads the site map from the specified path.
		/// </summary>
		/// <param name="path">The path of the site map to load.</param>
		/// <returns>The site map loaded from the specified path.</returns>
		static public SiteMap Load(string path)
		{
			SiteMap map = new SiteMap();
			map.FilePath = path;

			// load site map file
			XmlDocument nodeData = new XmlDocument();
			nodeData.Load(path);

			// skip the <xml> document element
			XmlNode first = nodeData.FirstChild;
			if (first.NodeType == XmlNodeType.XmlDeclaration)
				first = nodeData.FirstChild.NextSibling;//.FirstChild;
			else
				first = first.FirstChild;

			if (first == null)
			{
				throw new Exception("The site map contains no elements.");
			}
			else
			{
				// NodeAddRoot.Enabled = false;

				XmlToObjects(first.ChildNodes, map.ChildNodes);
				
			}

			return map;
		}
		
		/// <summary>
		/// Saves the site map.
		/// </summary>
		public void Save()
		{
			Save(FilePath);
		}

		/// <summary>
		/// Saves the site map to the specified path.
		/// </summary>
		/// <param name="path">The path of the site map file.</param>
		public void Save(string path)
		{
			XmlDocument document = new XmlDocument();

			document.AppendChild(document.CreateXmlDeclaration("1.0", "utf-8", ""));

			XmlElement rootNode = document.CreateElement("siteMap");

			document.AppendChild(rootNode);

			ObjectsToXml(ChildNodes, document, rootNode);
			
			if (!Directory.Exists(Path.GetDirectoryName(path)))
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			
			document.Save(path);
		}

		/// <summary>
		/// Creates a node from the provided element.
		/// </summary>
		/// <param name="element">The element to create a node based on.</param>
		/// <returns>The node created from the provided element.</returns>
		static private SiteMapNode CreateNode(XmlElement element)
		{
			SiteMapNode node = new SiteMapNode();
			
			node.Title = element.Attributes["title"].Value;
			
			if (element.Attributes["url"] != null)
			{
				node.Url = element.Attributes["url"].Value;
				
				// Commented out so SiteMap isn't dependent upon HttpContext.Current
				// TODO: Check if needed. Remove if not.
				//if (node.Url.IndexOf("~") > -1)
				//	node.Url = node.Url.Replace("~", HttpContext.Current.Request.ApplicationPath);
			}
			
			if (element.Attributes["description"] != null)
				node.Description = element.Attributes["description"].Value;
			
			return node;
		}

		/// <summary>
		/// Creates an XML element from the provided node.
		/// </summary>
		/// <param name="node">The node to create an XML element based on.</param>
		/// <returns>The XML element created from the provided node.</returns>
		static private XmlElement CreateXmlElement(XmlDocument document, SiteMapNode node)
		{
			XmlElement element = document.CreateElement("siteMapNode");

			XmlAttribute titleAttribute = document.CreateAttribute("title");
			titleAttribute.InnerText = node.Title;
			element.Attributes.Append(titleAttribute);

			XmlAttribute urlAttribute = document.CreateAttribute("url");
			urlAttribute.InnerText = node.Url;
			element.Attributes.Append(urlAttribute);

			XmlAttribute descriptionAttribute = document.CreateAttribute("description");
			descriptionAttribute.InnerText = node.Description;
			element.Attributes.Append(descriptionAttribute);

			return element;
		}

		/// <summary>
		/// Adds the provided elements to the node collection.
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="elements"></param>
		static private void XmlToObjects(XmlNodeList elements, List<SiteMapNode> nodes)
		{
			foreach (XmlElement subElement in elements)
			{
				SiteMapNode subNode = CreateNode(subElement);
				nodes.Add(subNode);
				XmlToObjects(subElement.ChildNodes, subNode.ChildNodes);
			}
		}

		/// <summary>
		/// Adds the provided elements to the node collection.
		/// </summary>
		/// <param name="nodes"></param>
		/// <param name="elements"></param>
		static private void ObjectsToXml(List<SiteMapNode> nodes, XmlDocument document, XmlElement parent)
		{
			foreach (SiteMapNode node in nodes)
			{
				XmlElement element = CreateXmlElement(document, node);
				parent.AppendChild(element);
				ObjectsToXml(node.ChildNodes, document, element);
			}
		}
		#endregion

		#region IXmlSerializable Members

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			try
			{
				return new XmlSchema();
			}
			catch (Exception ex)
			{
				throw ex.InnerException;
			}
			//throw new Exception("The method or operation is not implemented.");
		}

		public void ReadXml(XmlReader reader)
		{
			try
			{
				XmlNodeType type = reader.MoveToContent();

				if (type == XmlNodeType.Element && reader.LocalName.ToLower() == "sitemap")
				{
					while (reader.Read() && reader.Name.ToLower() == "sitemapnode")
					{
						SiteMapNode node = new SiteMapNode();
						node.ReadXml(reader);
						ChildNodes.Add(node);
					}
				}
			}
			catch (Exception ex)
			{
				throw ex.InnerException;
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			try
			{
				//writer.WriteStartElement("siteMap");
				
				//writer.WriteAttributeString("xmlns", "http://schemas.microsoft.com/AspNet/SiteMap-File-1.0");
				foreach (SiteMapNode node in ChildNodes)
				{
					node.WriteXml(writer);
				}
				//writer.WriteEndElement();
			}
			catch (Exception ex)
			{
				throw ex.InnerException;
			}
		}

		#endregion
		
		/// <summary>
		/// Adds the provided item to the site map.
		/// </summary>
		/// <param name="item">The item to add to the site map.</param>
		public void Add(ISiteMapNode item)
		{
			using (LogGroup logGroup = LogGroup.Start("Adding item to site map.", NLog.LogLevel.Debug))
			{
				if (item == null)
					throw new ArgumentNullException("item");
				
				if (UrlCreator == null)
					throw new InvalidOperationException("The UrlCreator property has not been initialized.");

				LogWriter.Debug("Item title: " + item.Title);
				LogWriter.Debug("Item URL: " + item.Url);
				LogWriter.Debug("Item category: " + item.Category);
				LogWriter.Debug("Item action: " + item.Action);
				LogWriter.Debug("Item type name: " + item.TypeName);
				
				if (item.Title == null || item.Title == String.Empty)
					throw new ArgumentException("An title must be specified on the provided item.");
				
				SiteMapNode node = new SiteMapNode();
				
				node.Title = item.Title;
				node.Url = item.Url;
				
				if (node.Url == String.Empty)
					node.Url = UrlCreator.CreateUrl(item.Action, item.TypeName);
				
				LogWriter.Debug("URL: " + node.Url);

				SiteMapNode rootNode = ChildNodes.Count > 0 ? ChildNodes[0] : null;
				
				// Choose the appropriate child nodes collection (depending on whether a root node is found)
				List<SiteMapNode> childNodes = (rootNode != null
				                                ? rootNode.ChildNodes
				                                : ChildNodes);

				// Check for an existing node with the same title
				// TODO: Check if the category property should be chcked too
				SiteMapNode existingNode = GetNodeByUrl(childNodes, node.Url);

				if (existingNode == null)
				{
					LogWriter.Debug("Node title: " + node.Title);
					LogWriter.Debug("Node URL:" + node.Url);
					
					if (item.Category != null && item.Category != String.Empty)
					{
						SiteMapNode categoryNode = GetNodeByTitle(rootNode.ChildNodes, item.Category);
						if (categoryNode == null)
						{
							LogWriter.Debug("Creating/adding category node: " + item.Category);
							categoryNode = new SiteMapNode();
							categoryNode.Title = item.Category;
							rootNode.ChildNodes.Add(categoryNode);
						}
						categoryNode.SelectAction = TreeNodeSelectAction.None;
						categoryNode.ChildNodes.Add(node);

						LogWriter.Debug("Added node to category.");
					}
					else
					{
						LogWriter.Debug("Added node to root.");
						childNodes.Add(node);
					}
				}
				else
				{
					LogWriter.Debug("Node exists. Skipping add.");
				}
			}
		}

		/// <summary>
		/// Removes the provided item from the site map.
		/// </summary>
		/// <param name="item">The item to remove from the site map.</param>
		public void Remove(ISiteMapNode item)
		{
			using (LogGroup logGroup = LogGroup.Start("Removing item from site map.", NLog.LogLevel.Debug))
			{
				if (item == null)
					throw new ArgumentNullException("item");
				
				if (UrlCreator == null)
					throw new InvalidOperationException("The UrlCreator property has not been initialized.");
				
				LogWriter.Debug("Item title: " + item.Title);
				LogWriter.Debug("Item URL: " + item.Url);
				LogWriter.Debug("Item category: " + item.Category);
				LogWriter.Debug("Item action: " + item.Action);
				LogWriter.Debug("Item type name: " + item.TypeName);
				
				string url = item.Url;
				
				if (url == String.Empty)
					url = UrlCreator.CreateUrl(item.Action, item.TypeName);
				
				LogWriter.Debug("URL: " + url);
				
				SiteMapNode rootNode = ChildNodes.Count > 0 ? ChildNodes[0] : null;
				
				// Choose the appropriate child nodes collection (depending on whether a root node is found)
				List<SiteMapNode> childNodes = (rootNode != null
				                                ? rootNode.ChildNodes
				                                : ChildNodes);

				SiteMapNode existingNode = GetNodeByUrl(childNodes, url);

				if (existingNode != null)
				{
					if (item.Category != null && item.Category != String.Empty)
					{
						SiteMapNode categoryNode = GetNodeByTitle(rootNode.ChildNodes, item.Category);
						
						categoryNode.ChildNodes.Remove(existingNode);
						
						if (categoryNode.ChildNodes.Count == 0)
							rootNode.ChildNodes.Remove(categoryNode);

						LogWriter.Debug("Removed node from category.");
					}
					else
					{
						LogWriter.Debug("Removed node from root.");
						
						rootNode.ChildNodes.Remove(existingNode);
					}
				}
				else
				{
					LogWriter.Debug("Node not found. Skipping remove.");
					
					throw new Exception("No existing node found with URL '" + url + "'.");
				}
			}
		}

		/// <summary>
		/// Retrieves the node from the provided collection with the provided title.
		/// </summary>
		/// <param name="nodes">The nodes to search through.</param>
		/// <param name="title">The title of the node to retrieve.</param>
		/// <returns>The node with the provided title from the provided collection.</returns>
		public SiteMapNode GetNodeByTitle(List<SiteMapNode> nodes, string title)
		{
			foreach (SiteMapNode node in nodes)
			{
				if (node.Title == title)
				{
					return node;
				}
			}
			return null;
		}

		/// <summary>
		/// Retrieves the node from the provided collection with the provided url.
		/// </summary>
		/// <param name="nodes">The nodes to search through.</param>
		/// <param name="url">The url of the node to retrieve.</param>
		/// <returns>The node with the provided url from the provided collection.</returns>
		public SiteMapNode GetNodeByUrl(List<SiteMapNode> nodes, string url)
		{
			foreach (SiteMapNode node in nodes)
			{
				if (node.Url == url)
				{
					return node;
				}

				foreach (SiteMapNode childNode in node.ChildNodes)
				{
					if (childNode.Url == url)
					{
						return childNode;
					}
				}
			}
			return null;
		}
	}
}
