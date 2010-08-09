using System;
using System.IO;
using Db4objects.Db4o;
using System.Xml;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using Db4objects.Db4o.Config;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// Exports data from the data stores to XML files.
	/// </summary>
	public class Db4oDataExporter : DataExporter
	{
		/// <summary>
		/// Sets the data provider and data store of the adapter.
		/// </summary>
		/// <param name="provider">The data provider of adapter.</param>
		/// <param name="store">The data store to tie the adapter to, or [null] to automatically select store.</param>
		public Db4oDataExporter(Db4oDataProvider provider, Db4oDataStore store)
		{
			Initialize(provider, store);
		}
	}
}
