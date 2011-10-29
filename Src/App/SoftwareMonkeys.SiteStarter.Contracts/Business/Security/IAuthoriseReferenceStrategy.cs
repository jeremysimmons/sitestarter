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
		IEntity SourceEntity { get;set; }
		string SourceProperty { get;set; }
		
		/// <summary>
		/// Authorises the reference on the source property of the source entity.
		/// </summary>
		void Authorise();
		
		IEntity[] Authorise(IEntity[] toEntities);
	}
}
