using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Web.Properties;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used to control the operation of deleting an entity.
	/// </summary>
	[Controller("Delete", "IEntity")]
	public class DeleteController : BaseController
	{
		public override string Action
		{
			get { return "Delete"; }
		}
		
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
		public void Delete(IEntity entity)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Deleting the provided entity.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (entity == null)
					AppLogger.Debug("Entity: [null]");
				else
					AppLogger.Debug("Entity: " + entity.GetType().FullName);
				
				Deleter.Delete(entity);
				
				AppLogger.Debug("Done");
				
				// Display the result
				Result.Display(DynamicLanguage.GetEntityText("EntityDeleted", entity.ShortTypeName));
			}
		}
		
		/// <summary>
		/// Deletes the entity specified by the query string.
		/// </summary>
		public void Delete()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Deleting the entity specified in the query string.", NLog.LogLevel.Debug))
			{
				IEntity entity = Load();
				
				Delete(entity);
				
				if (entity == null)
					AppLogger.Debug("Entity: [null]");
				else
					AppLogger.Debug("Entity: " + entity.GetType().FullName);
			}
			
		}
		
		/// <summary>
		/// Loads the entity specified by the query string.
		/// </summary>
		/// <returns></returns>
		public IEntity Load()
		{
			return (IEntity)Reflector.InvokeGenericMethod(this, "Load",
			                              new Type[] {Container.Type},
			                              new object[] {});
		}
		
		/// <summary>
		/// Loads the entity specified by the query string.
		/// </summary>
		/// <returns></returns>
		public T Load<T>()
			where T : IEntity
		{
			T entity = default(T);
			
			using (LogGroup logGroup = AppLogger.StartGroup("Loading the entity specified in the query string.", NLog.LogLevel.Debug))
			{
				Guid id = QueryStrings.GetID(typeof(T).Name);
				string uniqueKey = QueryStrings.GetUniqueKey(typeof(T).Name);
			
				AppLogger.Debug("Entity ID: " + id.ToString());
				AppLogger.Debug("Unique key: " + uniqueKey);
				
				if (id != Guid.Empty)
					entity = Retriever.Retrieve<T>(id);
				else
					entity = Retriever.Retrieve<T>(uniqueKey);
				
				if (entity == null)
					AppLogger.Debug("Entity: [null]");
				else
					AppLogger.Debug("Entity: " + entity.GetType().FullName);
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
	}
}
