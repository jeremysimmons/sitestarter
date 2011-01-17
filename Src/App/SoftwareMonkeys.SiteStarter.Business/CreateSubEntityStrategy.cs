using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to create new instances of sub items.
	/// </summary>
	[Strategy("Create", "ISubEntity")]
	public class CreateSubEntityStrategy : CreateStrategy
	{
		/// <summary>
		/// Creates a new instance of the specified type of entity.
		/// </summary>
		public override T Create<T>()
		{
			return (T)Create();
		}
		
		/// <summary>
		/// Creates a new instance of the specified type of entity.
		/// </summary>
		/// <param name="shortTypeName">The short name of the type of entity to create an instance of.</param>
		public override IEntity Create()
		{
			ISubEntity entity = null;
			using (LogGroup logGroup = LogGroup.Start("Creating a new entity of type '" + TypeName + "'.", NLog.LogLevel.Debug))
			{
				if (!EntityState.IsType(TypeName))
					throw new InvalidOperationException("The TypeName property needs to be set to the type of an entity. Invalid type: " + TypeName);
				
				Type type = EntityState.GetType(TypeName);
				
				AppLogger.Debug("Authorisation required: " + RequireAuthorisation.ToString());
				
				if (RequireAuthorisation)
				{
					AuthoriseCreateStrategy.New(TypeName).EnsureAuthorised(TypeName);
				}
				
				entity = (ISubEntity)Activator.CreateInstance(type);
				entity.ID = Guid.NewGuid();
			}
			return entity;
		}
		
		public virtual T Create<T>(Guid parentID, string parentUniqueKey)
			where T : ISubEntity
		{
			return (T)Create(parentID, parentUniqueKey);
		}
		
		public virtual ISubEntity Create(Guid parentID, string parentUniqueKey)
		{
			ISubEntity entity = null;
			
			using (LogGroup logGroup = LogGroup.Start("Creating a new sub entity with the specified parent.", NLog.LogLevel.Debug))
			{
				entity = (ISubEntity)Create();
				
				LogWriter.Debug("Parent ID: " + parentID.ToString());
				LogWriter.Debug("Parent unique key: " + parentUniqueKey);
				LogWriter.Debug("Parent type: " + entity.ParentTypeName);
				
				entity.Parent = GetParent(entity.ParentTypeName, parentID, parentUniqueKey);
				
				SetNumber(entity);
			}
			return entity;
		}
		
		public IEntity GetParent(string parentTypeName, Guid parentID, string parentUniqueKey)
		{
			IEntity parent = null;
			
			using (LogGroup logGroup = LogGroup.Start("Retrieving the parent for entity being created.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Parent ID: " + parentID.ToString());
				LogWriter.Debug("Parent unique key: " + parentUniqueKey);
				LogWriter.Debug("Parent type: " + parentTypeName);
				
				IRetrieveStrategy retrieveStrategy = RetrieveStrategy.New(parentTypeName, RequireAuthorisation);
				
				if (parentUniqueKey != String.Empty)
				{
					parent = retrieveStrategy.Retrieve("UniqueKey", parentUniqueKey);
				}
				else if (parentID != Guid.Empty)
				{
					parent = retrieveStrategy.Retrieve("ID", parentID);
				}
				else
					throw new Exception("No unique key or ID found for the parent.");
			}
			return parent;
		}
		
		public virtual ISubEntity Create(IEntity parent)
		{
			ISubEntity entity = null;
			using (LogGroup logGroup = LogGroup.Start("Creating a new sub entity with the provided parent.", NLog.LogLevel.Debug))
			{
				entity.Parent = parent;
				
				SetNumber(entity);
			}
			return entity;
		}
		
		public virtual int SetNumber(ISubEntity entity)
		{
			using (LogGroup logGroup = LogGroup.Start("Setting the number of the new sub entity.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (entity.Parent == null)
					throw new Exception("No parent is assigned to this item.");
				
				IEntity parent = entity.Parent;
				
				ActivateStrategy.New(parent).Activate(parent, entity.ItemsPropertyName);
				
				ISubEntity[] items = Collection<ISubEntity>.ConvertAll(EntitiesUtilities.GetPropertyValue(parent, entity.ItemsPropertyName));
				
				LogWriter.Debug("Existing items: " + items.Length.ToString());
				
				entity.Number = items.Length+1;
				
				LogWriter.Debug("Entity number: " + entity.Number.ToString());
			}
			return entity.Number;
		}
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for creating an instance of the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public ICreateStrategy New()
		{
			return New(true);
		}
		
		/// <summary>
		/// Creates a new strategy for creating an instance of the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public ICreateStrategy New(bool requiresAuthorisation)
		{
			
			ICreateStrategy strategy = StrategyState.Strategies.Creator.NewCreator("ISubEntity");
			
			strategy.RequireAuthorisation = requiresAuthorisation;
			
			return strategy;
		}
		#endregion
	}
}
