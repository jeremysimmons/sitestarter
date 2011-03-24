using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of IDataDeleter.
	/// </summary>
	public interface IDataDeleter
	{
		void Delete(IEntity entity);
		
		void PreDelete(IEntity entity);
		
	}
}
