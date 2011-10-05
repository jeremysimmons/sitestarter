using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Web.Projections;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Represents a node of the site map.
	/// </summary>
	[Serializable]
	public class SiteMapNode : TreeNode, IXmlSerializable, ISiteMapNode
	{
		private List<SiteMapNode> childNodes;
		/// <summary>
		/// Gets/sets the child node collection.
		/// </summary>
		public new List<SiteMapNode> ChildNodes
		{
			get {
				if (childNodes == null)
					childNodes = new List<SiteMapNode>();
				return childNodes; }
			set { childNodes = value; }
		}

		private string url = String.Empty;
		/// <summary>
		/// Gets/sets the URL of the page.
		/// </summary>
		public string Url
		{
			get { return url; }
			set { url = value; }
		}

		private string title = String.Empty;
		/// <summary>
		/// Gets/sets the title of the page.
		/// </summary>
		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		private string description = String.Empty;
		/// <summary>
		/// Gets/sets the page description.
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}
		
		private string action = String.Empty;
		public string Action
		{
			get { return action; }
			set { action = value; }
		}

		private string typeName = String.Empty;
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		private string category = String.Empty;
		public string Category
		{
			get { return category; }
			set { category = value; }
		}
		
		public SiteMapNode()
		{
			
		}
		
		public SiteMapNode(string category, string title, string action, string typeName)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Constructing SiteMapNode from the provided parameters."))
			{
				Category = category;
				Title = title;
				Action = action;
				TypeName = typeName;
				
				LogWriter.Debug("Category: " + Category);
				LogWriter.Debug("Title: " + Title);
				LogWriter.Debug("Action: " + Action);
				LogWriter.Debug("Type name: " + TypeName);
			}
		}
		
		public SiteMapNode(string title, string url)
		{
			Title = title;
			Url = url;
		}
		
		public SiteMapNode(ProjectionInfo projection)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Constructing SiteMapNode from the provided projection."))
			{
				Category = projection.MenuCategory;
				Title = projection.MenuTitle;
				Action = projection.Action;
				TypeName = projection.TypeName;
				
				LogWriter.Debug("Category: " + Category);
				LogWriter.Debug("Title: " + Title);
				LogWriter.Debug("Action: " + Action);
				LogWriter.Debug("Type name: " + TypeName);
			}
		}
		
		#region IXmlSerializable Members

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			// TODO: Clean up
			// XmlNodeType type = reader.MoveToContent();
			// if (type == XmlNodeType.Element && reader.LocalName.ToLower() == "sitemapnode")
			// {
			Title = reader["Title"];
			Description = reader["Description"];
			Url = reader["Url"];

			while (reader.Read() && reader.Name.ToLower() == "sitemapnode")
			{
				SiteMapNode node = new SiteMapNode();
				node.ReadXml(reader);
				ChildNodes.Add(node);
			}
			//}
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("SiteMapNode");

			writer.WriteAttributeString("Title", Title);
			writer.WriteAttributeString("Description", "Description");
			writer.WriteAttributeString("Url", "Url");

			foreach (SiteMapNode node in ChildNodes)
			{
				node.WriteXml(writer);
			}
			writer.WriteEndElement();
		}

		#endregion
	}
}
