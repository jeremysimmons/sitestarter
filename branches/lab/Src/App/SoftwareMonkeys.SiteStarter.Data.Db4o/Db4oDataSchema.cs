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
	/// Refacters the data schema to reflect changes in entities without breaking the code.
	/// </summary>
	public class Db4oDataSchema : DataSchema
	{
		
		/// <summary>
		/// Sets the data provider and data store of the adapter.
		/// </summary>
		/// <param name="provider">The data provider of adapter.</param>
		/// <param name="store">The data store to tie the adapter to, or [null] to automatically select store.</param>
		public Db4oDataSchema(Db4oDataProvider provider, Db4oDataStore store)
		{
			Initialize(provider, store);
		}
		
		
		private IConfiguration db4oConfiguration;
		public IConfiguration Db4oConfiguration
		{
			get
			{
				if (db4oConfiguration == null)
					db4oConfiguration = Db4oFactory.NewConfiguration();
				return db4oConfiguration;
			}
		}
		
	}
}
