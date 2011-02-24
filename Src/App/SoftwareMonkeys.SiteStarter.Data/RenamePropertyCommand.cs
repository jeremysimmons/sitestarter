using System;
using System.Xml;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// A command used to rename a property in the data schema.
	/// </summary>
	[Serializable]
	[XmlType("RenameProperty")]
	[XmlRoot("RenameProperty")]
	public class RenamePropertyCommand : DataSchemaCommand
	{
		private string propertyName;
		/// <summary>
		/// Gets/sets the name of the property being renamed.
		/// </summary>
		public string PropertyName
		{
			get { return propertyName; }
			set { propertyName = value; }
		}
		
		private string newPropertyName;
		/// <summary>
		/// Gets/set the new name of the property.
		/// </summary>
		public string NewPropertyName
		{
			get { return newPropertyName; }
			set { newPropertyName = value; }
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public RenamePropertyCommand()
		{
		}
		
		
		/// <summary>
		/// Executes the schema  change command on the provided serialized reference.
		/// </summary>
		/// <param name="document">The reference serialized to an XML document.</param>
		public override void ExecuteOnReference(XmlDocument document)
		{
			using (LogGroup logGroup = LogGroup.Start("Executing the rename property command on the provided entity reference (as an XML document).", NLog.LogLevel.Debug))
			{
				XmlNode type1Node = document.DocumentElement.SelectSingleNode("Type1Name");
				XmlNode property1Node = document.DocumentElement.SelectSingleNode("Property1Name");
				XmlNode type2Node = document.DocumentElement.SelectSingleNode("Type2Name");
				XmlNode property2Node = document.DocumentElement.SelectSingleNode("Property2Name");
				
				if (type1Node == null)
					throw new ArgumentException("The provided document doesn't have a 'Type1Name' node.");
				
				if (property1Node == null)
					throw new ArgumentException("The provided document doesn't have a 'Property1Name' node.");
				
				if (type2Node == null)
					throw new ArgumentException("The provided document doesn't have a 'Type2Name' node.");
				
				if (property2Node == null)
					throw new ArgumentException("The provided document doesn't have a 'Property2Name' node.");
				
				if (type1Node.InnerText == TypeName &&
				    property1Node.InnerText == PropertyName)
				{
					property1Node.InnerText = newPropertyName;
				}
				
				if (type2Node.InnerText == TypeName &&
				    property2Node.InnerText == PropertyName)
				{
					property2Node.InnerText = newPropertyName;
				}
			}
		}
		
		/// <summary>
		/// Executes the schema change command on the provided serialized entity.
		/// </summary>
		/// <param name="document">The entity serialized to an XML document.</param>
		public override void ExecuteOnEntity(XmlDocument document)
		{
			using (LogGroup logGroup = LogGroup.Start("Executing the rename property command on the provided serialized entity (as an XML document).", NLog.LogLevel.Debug))
			{
				for (int i = 0; i < document.DocumentElement.ChildNodes.Count; i++)
				{
					XmlNode node = document.DocumentElement.ChildNodes[i];
					
					if (node.Name == PropertyName)
					{
						LogWriter.Debug("Found matching node '" + node.Name + "'...renaming.");
						XmlUtilities.RenameNode(node, NewPropertyName);
					}
				}
			}
		}
	}
}
