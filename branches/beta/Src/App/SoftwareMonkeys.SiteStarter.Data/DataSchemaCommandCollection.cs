using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Holds a collection of DataSchemaCommand objects and presents them for interaction by other components.
	/// </summary>
	[XmlType("Commands")]
	[XmlRoot("Commands")]
	public class DataSchemaCommandCollection : List<DataSchemaCommand>
	{
		public DataSchemaCommandCollection()
		{
		}
	}
}
