using System;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;

namespace SoftwareMonkeys.SiteStarter.State
{
	/// <summary>
	/// Holds a collection of virtual servers and loads them as needed.
	/// </summary>
	public class VirtualServerCollection : List<IVirtualServer>
	{
		private IRetrieverStrategy<IVirtualServer> retrieverStrategy;
		/// <summary>
		/// Gets/sets the retriever strategy used to load virtual servers as needed.
		/// </summary>
		public IRetrieverStrategy<IVirtualServer> RetrieverStrategy
		{
			get { return retrieverStrategy; }
			set { retrieverStrategy = value; }
		}
		
		/*private ICreatorStrategy<IVirtualServer> creatorStrategoy;
		/// <summary>
		/// Gets/sets the creato strategy used to create virtual servers as needed.
		/// </summary>
		public ICreatorStrategy<IVirtualServer> CreatoryStrategy;
		{
			get { return creatorStrategoy; }
			set { creatorStrategoy = value; }
		}*/
		
		/// <summary>
		/// Gets/sets the virtual server with the specified ID, or loads it if it's not yet in the list.
		/// </summary>
		public IVirtualServer this[Guid virtualServerID]
		{
			get {
				
				IVirtualServer server = GetByID(virtualServerID);
				
				if (server == null)
				{
					server = RetrieverStrategy.Retrieve(virtualServerID);
					Add(server);
				}
				
				return server;
			}
		}
		
		/// <summary>
		/// Retrieves the virtual server in the collection with the provided ID.
		/// </summary>
		/// <param name="virtualServerID">The ID of the virtual server to retrieve.</param>
		/// <returns>The virtual server with the provided ID.</returns>
		public IVirtualServer GetByID(Guid virtualServerID)
		{
			IVirtualServer server = null;
			
			foreach (IVirtualServer s in this)
			{
				if (s.ID == virtualServerID)
				{
					server = s;
				}
			}
			
			return server;
		}
	}
}
