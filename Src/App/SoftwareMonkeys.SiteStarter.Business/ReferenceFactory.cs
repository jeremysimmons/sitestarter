using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.ComponentModel;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Provides an interface for interacting with users.
	/// </summary>
	[DataObject(true)]
	public class ReferenceFactory : UserFactory<Entities.User>
	{
		
	}
	
	
	/// <summary>
	/// Provides an interface for interacting with users.
	/// </summary>
	[DataObject(true)]
	public class ReferenceFactory<T> : BaseFactory
		where T : EntityIDReference
	{
		static private ReferenceFactory<T> current;
		static public ReferenceFactory<T> Current
		{
			get {
				if (current == null)
					current = new ReferenceFactory<T>();
				return current; }
		}
		
	
	}
}
