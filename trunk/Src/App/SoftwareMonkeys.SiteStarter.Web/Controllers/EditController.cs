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
	[Controller("Edit", "IEntity")]
	public class EditController : BaseController
	{
		public override string Action
		{
			get { return "Edit"; }
		}
		
		private IRetrieveStrategy retriever;
		/// <summary>
		/// Gets/sets the strategy used to retrieve an entity.
		/// </summary>
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
		
		private IUpdateStrategy updater;
		/// <summary>
		/// Gets/sets the strategy used to update an entity.
		/// </summary>
		public IUpdateStrategy Updater
		{
			get {
				if (updater == null)
				{
					if (Container.Type == null)
						throw new InvalidOperationException("Type property hasn't been initialized.");
					updater = StrategyState.Strategies.Creator.NewUpdater(Container.Type.Name);
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
					if (Container.Type == null)
						throw new InvalidOperationException("Type property hasn't been initialized.");
					activator = StrategyState.Strategies.Creator.NewActivator(Container.Type.Name);
				}
				return activator; }
			set { activator = value; }
		}
		
		
		private string entityUpdatedLanguageKey = "EntityUpdated";
		public string EntityUpdatedLanguageKey
		{
			get { return entityUpdatedLanguageKey; }
			set { entityUpdatedLanguageKey = value; }
		}
		
		private string entityPropertyTakenLanguageKey = "EntityPropertyTaken";
		public string EntityPropertyTakenLanguageKey
		{
			get { return entityPropertyTakenLanguageKey; }
			set { entityPropertyTakenLanguageKey = value; }
		}
		
		
		public EditController()
		{
		}
		
		/// <summary>
		/// Begins the edit process.
		/// </summary>
		public void StartEdit()
		{
			if (Container.EnsureAuthorised())
			{
				string typeName = Container.Type.Name;
				
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
			Guid id = QueryStrings.GetID(Container.Type.Name);
			string uniqueKey = QueryStrings.GetUniqueKey(Container.Type.Name);
			
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
			DataSource = Retriever.Retrieve("UniqueKey", uniqueKey);
			
			return DataSource;
		}
		
		/// <summary>
		/// Loads the entity of the specified type with the provided ID and assigns it to the DataSource property.
		/// </summary>
		/// <param name="id">The ID of the entity to load.</param>
		/// <returns>The loaded and activated entity.</returns>
		public IEntity Load(Guid id)
		{
			DataSource = Retriever.Retrieve("ID", id);
			
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
				Result.Display(DynamicLanguage.GetEntityText(EntityUpdatedLanguageKey, Container.Type.Name));

				return true;
			}
			else
			{
				CheckUniquePropertyName();
				
				// Get the "entity exists" language entry
				string error = DynamicLanguage.GetEntityText(EntityPropertyTakenLanguageKey, Container.Type.Name);
				
				// Insert the name of the unique property
				error = error.Replace("${property}", DynamicLanguage.GetText(UniquePropertyName).ToLower());
				
				// Display the error
				Result.DisplayError(error);
				
				return false;
			}
		}
		
		public static EditController New(IControllable container, Type type)
		{
			return New(container, type);
		}
		
		public static EditController New(IControllable container, Type type, string uniquePropertyName)
		{
			EditController controller = ControllerState.Controllers.Creator.NewEditor(type.Name);
			
			controller.Container = container;
			controller.UniquePropertyName = uniquePropertyName;
			
			return controller;
		}
		
		public void CheckUniquePropertyName()
		{
			if (UniquePropertyName == null || UniquePropertyName == String.Empty)
				throw new InvalidOperationException("The UniquePropertyName property has not been set.");
		}
	}
}
