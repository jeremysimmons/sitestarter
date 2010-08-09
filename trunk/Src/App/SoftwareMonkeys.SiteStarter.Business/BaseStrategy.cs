using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the base class used by all business strategy components.
	/// </summary>
	public class BaseStrategy<T>
		where T : IEntity
	{
		private string virtualServerID;
		/// <summary>
		/// Gets/sets the ID of the virtual server that the strategy applies to.
		/// </summary>
		public string VirtualServerID
		{
			get { return virtualServerID; }
			set { virtualServerID = value; }
		}
		
		/// <summary>
		/// Gets the data store that corresponds with the specified type and virtual server.
		/// </summary>
		public IDataStore DataStore
		{
			get
			{
				return DataAccess.Data.Stores
				[
					VirtualServerID,
					typeof(T)
				];
			}
		}
		
		public BaseStrategy()
		{
		}
	}
}
