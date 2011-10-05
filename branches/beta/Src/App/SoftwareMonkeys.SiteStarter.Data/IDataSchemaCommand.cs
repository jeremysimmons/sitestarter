using System;
using System.Xml;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Defines the interface required by all schema edit commands.
	/// </summary>
	public interface IDataSchemaCommand
	{
		/// <summary>
		/// Gets/sets the short name of the corresponding type.
		/// </summary>
		string TypeName {get;set;}
		
		/// <summary>
		/// Checks whether the command applies to the type serialized in the provided XML document.
		/// </summary>
		/// <param name="document">A serialized entity as an XML document.</param>
		/// <returns>A boolean flag indicating whether or not the command applies to the specified type.</returns>
		bool AppliesTo(XmlDocument document);
		
		/// <summary>
		/// Executes the commands and applies the effects to the provided XML document.
		/// </summary>
		/// <param name="document">The entity serialized and loaded into an XML document.</param>
		void Execute(XmlDocument document);
	}
}
