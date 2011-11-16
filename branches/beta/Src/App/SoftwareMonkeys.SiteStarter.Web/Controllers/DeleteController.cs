using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.Navigation;
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
					CheckContainer();
					Container.CheckCommand();
					
					retriever = StrategyState.Strategies.Creator.NewRetriever(Command.TypeName);
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
					CheckContainer();
					Container.CheckCommand();
					
					deleter = StrategyState.Strategies.Creator.NewDeleter(Command.TypeName);
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
				
				if (EnsureAuthorised(entity))
				{
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
			Container.CheckCommand();
			
			Type type = EntityState.GetType(Command.TypeName);
			
			return (IEntity)Reflector.InvokeGenericMethod(this, "Load",
			                                              new Type[] {type},
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
		
		public virtual void NavigateAfterDelete(IEntity entity)
		{
			Navigator.Current.Go("Index", entity.ShortTypeName);
		}
	}
}
