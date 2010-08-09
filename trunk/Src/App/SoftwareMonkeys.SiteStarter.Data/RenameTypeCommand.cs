using System;
using System.Xml;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// A command used to rename a type in the data schema.
	/// </summary>
	[Serializable]
	[XmlType("RenameType")]
	[XmlRoot("RenameType")]
	public class RenameTypeCommand : DataSchemaCommand
	{
		private string newTypeName;
		/// <summary>
		/// Gets/sets the new short type name.
		/// </summary>
		public string NewTypeName
		{
			get { return newTypeName; }
			set { newTypeName = value; }
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public RenameTypeCommand()
		{
		}
		
		
		
		/// <summary>
		/// Executes the schema  change command on the provided serialized reference.
		/// </summary>
		/// <param name="document">The reference serialized to an XML document.</param>
		public override void ExecuteOnReference(XmlDocument document)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Renaming the type on the provided reference.", NLog.LogLevel.Debug))
			{
				XmlNode type1Node = document.DocumentElement.SelectSingleNode("Type1Name");
				
				XmlNode type2Node = document.DocumentElement.SelectSingleNode("Type2Name");
				
				if (type1Node == null)
					throw new ArgumentException("The provided document doesn't have a 'Type1Name' node.");
				
				if (type2Node == null)
					throw new ArgumentException("The provided document doesn't have a 'Type2Name' node.");
				
				if (type1Node.InnerText == TypeName)
				{
					AppLogger.Debug("Found matching node '" + type1Node.InnerText + "'...replacing with '" + newTypeName + "'.");
					type1Node.InnerText = newTypeName;
				}
				
				if (type2Node.InnerText == TypeName)
				{
					AppLogger.Debug("Found matching node '" + type2Node.InnerText + "'...replacing with '" + newTypeName + "'.");
					type2Node.InnerText = newTypeName;
				}
			}
		}
		
		/// <summary>
		/// Executes the schema change command on the provided serialized entity.
		/// </summary>
		/// <param name="document">The entity serialized to an XML document.</param>
		public override void ExecuteOnEntity(XmlDocument document)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Executing the rename type command on the provided entity.", NLog.LogLevel.Debug))
			{
				string typeName = document.DocumentElement.Name;
				
				AppLogger.Debug("Type name: " + typeName);
				
				if (typeName == TypeName)
				{
					AppLogger.Debug("Type name matches. Renaming to: " + NewTypeName);
					
					XmlUtilities.RenameNode(document.DocumentElement, NewTypeName);
				}
			}
		}
	}
}
