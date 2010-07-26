using System;
using System.Collections.Generic;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Description of IFactory.
	/// </summary>
	public interface IFactory : IAppComponent
	{
		IEntity[] GetPage(Type type, IPagingLocation location, string sortExpression);
		IEntity[] GetPage(Type type, Dictionary<string, object> filterValues, IPagingLocation location, string sortExpression);
		IEntity[] GetPage(Type type, string propertyName, object propertyValue, IPagingLocation location, string sortExpression);
		
		IEntity[] GetEntities(Type type);
		
		IEntity[] GetEntities(Type type, params object[] parameters);
		
		IEntity Get(Type type, string uniqueKey);
		IEntity Get(Type type, Guid id);
		T Get<T>(string uniqueKey);
		T Get<T>(Guid id);
		
		IEntity Create(Type type);
		
		bool Update(IEntity entity);
		bool Save(IEntity entity);
		void Delete(IEntity entity);
		
		void Activate(IEntity entity);
		
		object Execute(string action, string typeName, params object[] parameters);
		
		void RegisterFactory(params Type[] types);
		void DeregisterFactory();
		
	}
}
