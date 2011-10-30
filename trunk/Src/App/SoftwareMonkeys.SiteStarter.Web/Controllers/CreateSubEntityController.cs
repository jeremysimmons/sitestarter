using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Web.Navigation;
using SoftwareMonkeys.SiteStarter.Web.Security;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Web;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	[Controller("Create", "ISubEntity")]
	public class CreateSubEntityController : CreateController
	{
		public CreateSubEntityController()
		{
		}
		
		/// <summary>
		/// Creates a new sub entity and assigns the parent entity specified in the query strings.
		/// </summary>
		/// <returns></returns>
		public override IEntity Create()
		{
			IEntity entity = null;
			
			using (LogGroup logGroup = LogGroup.Start("Preparing the form to create a new sub entity.", NLog.LogLevel.Debug))
			{
				if (EnsureAuthorised())
				{
					Guid parentID = Guid.Empty;
					string parentUniqueKey = String.Empty;
					
					if (QueryStrings.Available)
					{
						parentID = QueryStrings.GetID("Parent");
						parentUniqueKey = QueryStrings.GetUniqueKey("Parent");
					}
					else
						throw new InvalidOperationException("Query strings aren't available. Provide parent ID or parent unique key manually.");
					
					LogWriter.Debug("Parent ID: " + parentID);
					LogWriter.Debug("Parent unique key: " + parentUniqueKey);
					
					entity = Create(parentID, parentUniqueKey);
				}
			}
			
			return entity;
		}
		
		/// <summary>
		/// Creates a new sub entity and assigns the parent entity specified in the query strings.
		/// Note: Either the parent ID or parent unique key need to be provided, not both.
		/// </summary>
		/// <param name="parentID">The ID of the parent entity.</param>
		/// <param name="uniqueKey">The unique key of the parent entity.</param>
		/// <returns>The newly created sub entity.</returns>
		public IEntity Create(Guid parentID, string parentUniqueKey)
		{
			IEntity entity = null;
			
			using (LogGroup logGroup = LogGroup.Start("Creating a new sub entity.", NLog.LogLevel.Debug))
			{
				CreateSubEntityStrategy createStrategy = (CreateSubEntityStrategy)CreateStrategy.New(Command.TypeName, Container.RequireAuthorisation);
				
				entity = createStrategy.Create(parentID, parentUniqueKey);
				
				ExecuteCreate(entity);
				
			}
			return entity;
		}
		
		/// <summary>
		/// Sets the parent to the new entity.
		/// </summary>
		/// <param name="entity">The new default parent entity.</param>
		public override void ExecuteCreate(IEntity entity)
		{
			using (LogGroup logGroup = LogGroup.Start("Creating a new entity.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (!(entity is ISubEntity))
					throw new ArgumentException("Invalid type '" + entity.GetType() + "'. Expected 'ISubEntity'.");
				
				if (EnsureAuthorised(entity))
				{
					DataSource = entity;
					
					base.ExecuteCreate(entity);
				}
			}
		}
		
		public override bool ExecuteSave(IEntity entity)
		{
			if (entity is ISubEntity)
			{
				return ExecuteSave((ISubEntity)entity);
			}
			else
				throw new ArgumentException("The provided entity type '" + entity.GetType().FullName + "' is not supported. The entity must be of type 'ISubEntity'.");
		}
		
		public virtual bool ExecuteSave(ISubEntity entity)
		{
			bool success = false;
			
			using (LogGroup logGroup = LogGroup.Start("Saving the new entity.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (entity.Parent == null)
					ActivateStrategy.New<ISubEntity>().Activate(entity, entity.ParentPropertyName);
				
				if (entity.Parent == null)
					throw new Exception("No parent found for entity with ID: " + entity.ID.ToString());
				
				DataSource = entity;
				
				success = base.ExecuteSave(entity);
				
				
			}
			
			return success;
		}
		
		public override void NavigateAfterSave()
		{
			using (LogGroup logGroup = LogGroup.Start("Navigating after a save operation.", NLog.LogLevel.Debug))
			{
				if (DataSource == null)
					throw new InvalidOperationException("The DataSource property isn't set.");
				
				ISubEntity entity = (ISubEntity)DataSource;
				
				if (entity.Parent == null)
				{
					LogWriter.Debug("No parent found. Activating entity.Parent.");
					
					ActivateStrategy.New<ISubEntity>().Activate(entity, entity.ParentPropertyName);
				}
				
				if (entity.Parent == null)
					throw new Exception("No parent assigned to entity.");
				
				Navigator.Current.NavigateAfterOperation("View", entity.Parent);
			}
		}
	}
}
