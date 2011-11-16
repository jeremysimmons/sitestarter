﻿using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Web.Properties;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used to control the operation of deleting an entity.
	/// </summary>
	[Controller("Delete", "IEntity")]
	public class DeleteController : BaseController
	{
		private IRetrieveStrategy retriever;
		public IRetrieveStrategy Retriever
		{
			get {
				if (retriever == null)
				{
					if (Container.Type == null)
						throw new InvalidOperationException("Type property hasn't been initialized.");
					retriever = StrategyState.Strategies.Creator.NewRetriever(Container.Type.Name);
				}
				return retriever; }
			set { retriever = value; }
		}
		
		private IDeleteStrategy deleter;
		public IDeleteStrategy Deleter
		{
			get {
				if (deleter == null)
				{
					if (Container.Type == null)
						throw new InvalidOperationException("Type property hasn't been initialized.");
					deleter = StrategyState.Strategies.Creator.NewDeleter(Container.Type.Name);
				}
				return deleter; }
			set { deleter = value; }
		}
		
		public DeleteController()
		{
		}
		
		/// <summary>
		/// Deletes the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		public virtual void Delete(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
				
			ExecuteDelete(entity);
		}
		
		/// <summary>
		/// Deletes the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		public virtual void ExecuteDelete(IEntity entity)
		{
			using (LogGroup logGroup = LogGroup.Start("Deleting the provided entity.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (entity == null)
					LogWriter.Debug("Entity: [null]");
				else
					LogWriter.Debug("Entity: " + entity.GetType().FullName);
				
				Deleter.Delete(entity);
				
				LogWriter.Debug("Done");
				
				// Display the result
				Result.Display(DynamicLanguage.GetEntityText("EntityDeleted", entity.ShortTypeName));
				
				NavigateAfterDelete(entity);
			}
		}
		
		/// <summary>
		/// Deletes the entity specified by the query string.
		/// </summary>
		public virtual void Delete()
		{
			using (LogGroup logGroup = LogGroup.Start("Deleting the entity specified in the query string.", NLog.LogLevel.Debug))
			{
				IEntity entity = Load();
				
				Delete(entity);
				
				if (entity == null)
					LogWriter.Debug("Entity: [null]");
				else
					LogWriter.Debug("Entity: " + entity.GetType().FullName);
			}
			
		}
		
		/// <summary>
		/// Loads the entity specified by the query string.
		/// </summary>
		/// <returns></returns>
		public virtual IEntity Load()
		{
			Container.CheckType();
			
			return (IEntity)Reflector.InvokeGenericMethod(this, "Load",
			                              new Type[] {Container.Type},
			                              new object[] {});
		}
		
		/// <summary>
		/// Loads the entity specified by the query string.
		/// </summary>
		/// <returns></returns>
		public virtual T Load<T>()
			where T : IEntity
		{
			T entity = default(T);
			
			using (LogGroup logGroup = LogGroup.Start("Loading the entity specified in the query string.", NLog.LogLevel.Debug))
			{
				Guid id = QueryStrings.GetID(typeof(T).Name);
				string uniqueKey = QueryStrings.GetUniqueKey(typeof(T).Name);
			
				LogWriter.Debug("Entity ID: " + id.ToString());
				LogWriter.Debug("Unique key: " + uniqueKey);
				
				if (id != Guid.Empty)
					entity = Retriever.Retrieve<T>(id);
				else if (uniqueKey != String.Empty)
					entity = Retriever.Retrieve<T>(uniqueKey);
				else
					throw new Exception("No ID or unique key found in the URL.");
				
				if (entity == null)
					LogWriter.Debug("Entity: [null]");
				else
					LogWriter.Debug("Entity: " + entity.GetType().FullName);
			}
			return entity;
		}
		
		public static DeleteController New(IControllable container, Type type)
		{
			DeleteController controller = ControllerState.Controllers.Creator.NewDeleter(type.Name);
			
			controller.Container = container;
			
			return controller;
		}
		
		
		
		/*public void Delete(Type type, Guid entityID)
		{
			string action = "Delete";
			object[] parameters = new object[]{};
			if (Commands.CommandExists(action, type.Name, Commands.GetTypes(parameters)))
			{
				Commands.Execute(action, type.Name, parameters);
			}
			else
			{
				DefaultDelete(type, entityID);
			}
		}*/
		
		/*public void DefaultDelete(Type type, Guid entityID)
		{
			IEntity entity = (IEntity)Factory.Get(type, entityID);
			
			if (Commands.CommandExists("Delete", type.Name, Commands.GetTypes(new object[]{entityID})))
			{
				Commands.Execute("Delete", type.Name, new object[]{entityID});
			}
			else
			{
				Delete(entity, Container.Messages[type.Name + "Deleted"]);
			}
		}*/
		
		public virtual void NavigateAfterDelete(IEntity entity)
		{
			string url = UrlCreator.Current.CreateUrl("Index", entity.ShortTypeName);
			
			HttpContext.Current.Response.Redirect(url);
		}
	}
}