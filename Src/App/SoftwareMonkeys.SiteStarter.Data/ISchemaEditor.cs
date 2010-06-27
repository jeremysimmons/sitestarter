/*
 * Created by SharpDevelop.
 * User: Jose
 * Date: 25/06/2010
 * Time: 8:51 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of ISchemaEditor.
	/// </summary>
	public interface ISchemaEditor
	{
		string[] Messages {get;}
		void ApplySchema(string schemaDirectory);
		void RenameProperty(string type, string originalProperty, string newProperty);
		void RenameType(string originalName, string newName);
		string[] Suspend();
		void Resume();
	}
}
