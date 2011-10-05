using System;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of XmlUtilities.
	/// </summary>
	public class XmlUtilities
	{
		public XmlUtilities()
		{
		}
		
		
		/// <summary>
		/// Renames the provided node to the name specified.
		/// </summary>
		/// <param name="node">The node to rename.</param>
		/// <param name="qualifiedName">The new name for the node.</param>
		/// <returns>A renamed version of the original node.</returns>
		static public XmlNode RenameNode (XmlNode node,
		                                  string qualifiedName)
		{
			string namespaceURI = String.Empty;
			
			if (node.NodeType == XmlNodeType.Element) {
				XmlElement oldElement = (XmlElement) node;
				XmlElement newElement =
					node.OwnerDocument.CreateElement(qualifiedName, namespaceURI);
				while (oldElement.HasAttributes) {

					newElement.SetAttributeNode(oldElement.RemoveAttributeNode(oldElement.Attributes[0]));
				}
				while (oldElement.HasChildNodes) {
					newElement.AppendChild(oldElement.FirstChild);
				}
				if (oldElement.ParentNode != null) {
					oldElement.ParentNode.ReplaceChild(newElement, oldElement);
				}
				return newElement;
			}
			else {
				return null;
			}
		}
		
		
		/// <summary>
		/// Deserializes an xml document back into an object
		/// </summary>
		/// <param name="xml">The xml data to deserialize</param>
		/// <param name="type">The type of the object being deserialized</param>
		/// <returns>A deserialized object</returns>
		public static object DeserializeFromDocument(XmlDocument xml, Type type)
		{
			XmlSerializer s = new XmlSerializer(type);
			string xmlString = xml.OuterXml.ToString();
			byte[] buffer = ASCIIEncoding.UTF8.GetBytes(xmlString);
			MemoryStream ms = new MemoryStream(buffer);
			XmlReader reader = new XmlTextReader(ms);
			Exception caught = null;

			try
			{
				object o = s.Deserialize(reader);
				return o;
			}

			catch(Exception e)
			{
				caught = e;
			}
			finally
			{
				reader.Close();

				if(caught != null)
					throw caught;
			}
			return null;
		}

		/// <summary>
		/// Serializes an object into an Xml Document
		/// </summary>
		/// <param name="o">The object to serialize</param>
		/// <returns>An Xml Document consisting of said object's data</returns>
		public static XmlDocument SerializeToDocument(object o)
		{
			XmlSerializer s = new XmlSerializer(o.GetType());

			MemoryStream ms = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(ms, new UTF8Encoding());
			writer.Formatting = Formatting.Indented;
			writer.IndentChar = ' ';
			writer.Indentation = 5;
			Exception caught = null;

			try
			{
				s.Serialize(writer, o);
				XmlDocument xml = new XmlDocument();
				string xmlString = ASCIIEncoding.UTF8.GetString(ms.ToArray());
				xml.LoadXml(xmlString);
				return xml;
			}
			catch(Exception e)
			{
				caught = e;
			}
			finally
			{
				writer.Close();
				ms.Close();

				if(caught != null)
					throw caught;
			}
			return null;
		}
	}
}
