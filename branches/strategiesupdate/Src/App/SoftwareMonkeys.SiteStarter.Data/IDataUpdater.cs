using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of IDataUpdater.
	/// </summary>
	public interface IDataUpdater
	{
		void Update(IEntity entity);
		
		void PreUpdate(IEntity entity);
		
	}
}
