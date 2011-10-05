using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of DataDeleter.
	/// </summary>
	public abstract class DataDeleter : DataAdapter, IDataDeleter
	{		
		public abstract void Delete(IEntity entity);
		
		public abstract void PreDelete(IEntity entity);
		
	}
}
