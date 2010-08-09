using System;
using System.Xml;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Provides a base implementation for all data schema editor commands.
	/// </summary>
	public abstract class DataSchemaCommand : IDataSchemaCommand
	{
		private string typeName;
		/// <summary>
		/// Gets/sets the short name of the corresponding type.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
			set { typeName = value; }
		}
		
		/// <summary>
		/// Checks whether the command applies to the type serialized in the provided XML document.
		/// </summary>
		/// <param name="document">A serialized entity as an XML document.</param>
		/// <returns>A boolean flag indicating whether or not the command applies to the specified type.</returns>
		public virtual bool AppliesTo(XmlDocument document)
		{
			bool doesApply = false;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the schema command applies to the provided document.", NLog.LogLevel.Debug))
			{
				if (document == null)
					throw new ArgumentNullException("document");
				
				if (TypeName == String.Empty)
					throw new InvalidOperationException("The TypeName property is not set.");
				
				if (document.DocumentElement == null)
					throw new ArgumentException("The document.DocumentElement property is null.", "document.DocumentElement");
				
				string documentType = document.DocumentElement.Name;
				
				if (documentType == String.Empty)
					throw new ArgumentException("The documentType is String.Empty.");
				
				AppLogger.Debug("Document type: "+ documentType);
				
				if (documentType == "EntityReference" ||
				    documentType == "EntityIDReference")
				{
					AppLogger.Debug("Checking applicability to reference.");
					doesApply = AppliesToReference(document);
				}
				else
				{
					AppLogger.Debug("Checking applicability to entity.");
					doesApply = AppliesToEntity(document);
				}
			}
			return doesApply;
		}
		
		/// <summary>
		/// Checks whether the command applies to the reference provided as a serialized XML document.
		/// </summary>
		/// <param name="document">The serialized reference as an XML document.</param>
		/// <returns>A boolean flag indicating whether the command applies to the provided XML document.</returns>
		public bool AppliesToReference(XmlDocument document)
		{
			bool doesApply = false;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the command applies to the provided XML document.", NLog.LogLevel.Debug))
			{
				XmlNode type1Node = document.DocumentElement.SelectSingleNode("Type1Name");
				
				XmlNode type2Node = document.DocumentElement.SelectSingleNode("Type2Name");
				
				if (type1Node == null)
					throw new ArgumentException("The provided document doesn't have a 'Type1Name' node.");
				
				if (type2Node == null)
					throw new ArgumentException("The provided document doesn't have a 'Type2Name' node.");
				
				AppLogger.Debug("Type 1 name: " + type1Node.InnerText);
				AppLogger.Debug("Type 2 name: " + type2Node.InnerText);
				AppLogger.Debug("Expected type name: " + TypeName);
				
				// The command applies if either of the type nodes match the type on the command
				doesApply = type1Node.InnerText == TypeName ||
					type2Node.InnerText == TypeName;
			}
			
			return doesApply;
		}
		
		/// <summary>
		/// Checks whether the command applies to the entity provided as a serialized XML document.
		/// </summary>
		/// <param name="document">The serialized entity as an XML document.</param>
		/// <returns>A boolean flag indicating whether the command applies to the provided XML document.</returns>
		public bool AppliesToEntity(XmlDocument document)
		{
			bool doesApply = false;
			using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the command applies to the provided XML document.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Type: " + document.DocumentElement.Name);
				
				doesApply = TypeName.ToLower() == document.DocumentElement.Name.ToLower();
				
				AppLogger.Debug("Does apply: " + doesApply.ToString());
			}
			return doesApply;
		}
		
		/// <summary>
		/// Executes the commands and applies the effects to the provided XML document.
		/// </summary>
		/// <param name="document">The entity serialized and loaded into an XML document.</param>
		public virtual void Execute(XmlDocument document)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Executing the rename property command on the provided XML document.", NLog.LogLevel.Debug))
			{
				string typeName = document.DocumentElement.Name;
				
				AppLogger.Debug("Type name: "+ typeName);
				
				if (typeName == "EntityReference" ||
				    typeName == "EntityIDReference")
				{
					AppLogger.Debug("Reference entity");
					
					ExecuteOnReference(document);
				}
				else
				{
					AppLogger.Debug("Standard entity");
					
					ExecuteOnEntity(document);
				}
			}
		}
		
		/// <summary>
		/// Executes the schema change command on the provided serialized reference.
		/// </summary>
		/// <param name="document">The reference serialized to an XML document.</param>
		public abstract void ExecuteOnReference(XmlDocument document);
		
		/// <summary>
		/// Executes the schema change command on the provided serialized entity.
		/// </summary>
		/// <param name="document">The entity serialized to an XML document.</param>
		public abstract void ExecuteOnEntity(XmlDocument document);
	}
}
