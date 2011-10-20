using System;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// 
	/// </summary>
	public interface IAuthoriseReferenceStrategy : IAuthoriseStrategy
	{
		bool Authorise(IEntity fromEntity, IEntity toEntity);
		
		IEntity[] Authorise(IEntity fromEntity, IEntity[] toEntities);
			
		void Authorise(IEntity fromEntity, PropertyInfo property);
		
		void Authorise(IEntity fromEntity, string propertyName);
	}
}
