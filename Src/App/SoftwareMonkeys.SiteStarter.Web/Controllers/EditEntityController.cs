using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using System.Web.UI.WebControls;
using SoftwareMonkeys.SiteStarter.Web.Properties;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Used to control standard edit entity projections.
	/// </summary>
	public class EditEntityController : BaseController
	{
		private IRetrieveStrategy retriever;
		/// <summary>
		/// Gets/sets the strategy used to retrieve an entity.
		/// </summary>
		public IRetrieveStrategy Retriever
		{
			get {
				if (retriever == null)
				{
					if (Type == null)
						throw new InvalidOperationException("Type property hasn't been initialized.");
					retriever = StrategyState.Strategies.Creator.NewRetriever(Type.Name);
				}
				return retriever; }
			set { retriever = value; }
		}
		
		private IUpdateStrategy updater;
		/// <summary>
		/// Gets/sets the strategy used to update an entity.
		/// </summary>
		public IUpdateStrategy Updater
		{
			get {
				if (updater == null)
				{
					if (Type == null)
						throw new InvalidOperationException("Type property hasn't been initialized.");
					updater = StrategyState.Strategies.Creator.NewUpdater(Type.Name);
				}
				return updater; }
			set { updater = value; }
		}
		
		private IActivateStrategy activator;
		/// <summary>
		/// Gets/sets the strategy used to activate an entity.
		/// </summary>
		public IActivateStrategy Activator
		{
			get {
				if (activator == null)
				{
					if (Type == null)
						throw new InvalidOperationException("Type property hasn't been initialized.");
					activator = StrategyState.Strategies.Creator.NewActivator(Type.Name);
				}
				return activator; }
			set { activator = value; }
		}
		
		
		public EditEntityController()
		{
		}
		
		/// <summary>
		/// Begins the edit process.
		/// </summary>
		public void StartEdit()
		{
			if (EnsureAuthorised())
			{
				string typeName = Type.Name;
				
				OperationManager.StartOperation("Edit" + typeName, null);
			}
		}
		
		
		/// <summary>
		/// Prepares the edit form with the entity specified by the ID or unique key in the query string.
		/// </summary>
		/// <returns>The entity specified in the query string.</returns>
		public T PrepareEdit<T>()
		{
			return (T)PrepareEdit();
		}
		
		/// <summary>
		/// Prepares the edit form with the entity specified by the ID or unique key in the query string.
		/// </summary>
		/// <returns>The entity specified in the query string.</returns>
		public IEntity PrepareEdit()
		{
			Guid id = QueryStrings.GetID(Type.Name);
			string uniqueKey = QueryStrings.GetUniqueKey(Type.Name);
			
			if (id != Guid.Empty)
				return PrepareEdit(id);
			else if (uniqueKey != String.Empty)
				return PrepareEdit(uniqueKey);
			else
				throw new InvalidOperationException("Cannot edit entity. No identifier found.");
		}
		
		/// <summary>
		/// Prepares the edit form with the entity specified by the provided ID.
		/// </summary>
		/// <returns>The entity specified by the provided ID.</returns>
		public IEntity PrepareEdit(Guid entityID)
		{
			return Load(entityID);
		}
		
		/// <summary>
		/// Prepares the edit form with the entity specified by the provided unique key.
		/// </summary>
		/// <returns>The entity specified by the provided unique key.</returns>
		public IEntity PrepareEdit(string uniqueKey)
		{
			return Load(uniqueKey);
		}
		
		
		/// <summary>
		/// Starts the edit process for the entity provided.
		/// </summary>
		/// <returns>The entity provided.</returns>
		public IEntity Edit(IEntity entity)
		{
			StartEdit();
			
			Load(entity);
			
			return entity;
		}
		
		/// <summary>
		/// Assigns the provided entity to the DataSource property.
		/// </summary>
		/// <param name="entity">The entity to assign to the DataSource</param>
		/// <returns>The assigned entity.</returns>
		public IEntity Load(IEntity entity)
		{
			DataSource = entity;
			
			Activator.Activate(DataSource);
			return entity;
		}
		
		/// <summary>
		/// Loads the entity of the specified type with the provided ID and assigns it to the DataSource property.
		/// </summary>
		/// <param name="uniqueKey">The unique key of the entity to load.</param>
		/// <returns>The loaded and activated entity.</returns>
		public IEntity Load(string uniqueKey)
		{
			DataSource = Retriever.Retrieve(Type, "UniqueKey", uniqueKey);
			
			return DataSource;
		}
		
		/// <summary>
		/// Loads the entity of the specified type with the provided ID and assigns it to the DataSource property.
		/// </summary>
		/// <param name="id">The ID of the entity to load.</param>
		/// <returns>The loaded and activated entity.</returns>
		public IEntity Load(Guid id)
		{
			DataSource = Retriever.Retrieve(Type, "ID", id);
			
			return DataSource;
		}
		
		/// <summary>
		/// Updates the entity from the form back to the data store.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns>A bool value indicating the success of the update. If it fails it's due to the unique key being in use already.</returns>
		public bool Update(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			// Update the entity
			if (Updater.Update(entity))
			{
				Result.Display(DynamicLanguage.GetEntityText("EntityUpdated", Type.Name));

				return true;
			}
			else
			{
				Result.DisplayError(DynamicLanguage.GetEntityText("EntityExists", Type.Name));
				
				return false;
			}
		}
		
		/// <summary>
		/// Ensures that the user is authorised to edit an entity of the specified type.
		/// </summary>
		/// <returns>A value indicating whether the user is authorised.</returns>
		public bool EnsureAuthorised()
		{
			bool isAuthorised = AuthoriseUpdateStrategy.New(Type.Name).Authorise(Type.Name);
			
			if (!isAuthorised)
				FailAuthorisation();
			
			return isAuthorised;
		}
		
		public static EditEntityController CreateController(IControllable container, Type type)
		{
			EditEntityController controller = new EditEntityController();
			
			controller.Container = container;
			controller.Type = type;
			
			return controller;
		}
	}
}
