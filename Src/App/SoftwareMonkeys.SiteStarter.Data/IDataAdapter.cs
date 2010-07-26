using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of IDataAdapter.
	/// </summary>
	public interface IDataAdapter
	{
		IDataStore GetDataStore(IEntity entity);
		IDataStore GetDataStore(Type type);
		DataProvider Provider {get;}
	}
}
