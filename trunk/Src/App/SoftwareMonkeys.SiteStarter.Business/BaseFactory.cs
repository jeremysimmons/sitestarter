using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used as a base for all factory objects.
	/// </summary>
	[DataObject(true)]
	public class BaseFactory : IFactory
	{
		private Dictionary<string, Type> defaultTypes;
		public virtual Dictionary<string, Type> DefaultTypes
		{
			get { return defaultTypes; }
			set { defaultTypes = value; }
		}
		
		/// <summary>
		/// Activates the entity by loading all references.
		/// </summary>
		/// <param name="entity">The entity to activate.</param>
		public virtual void Activate(IEntity entity)
		{
			Activate(entity, 1);
		}
		
		/// <summary>
		/// Activates the entity by loading all references to the specified depth.
		/// </summary>
		/// <param name="entity">The entity to activate.</param>
		public virtual void Activate(IEntity entity, int depth)
		{
			DataAccess.Data.Activate(entity, depth);
		}
		
		/// <summary>
		/// Activates all the entities provided by loading all references for each entity.
		/// </summary>
		/// <param name="entities">The array of entities to activate.</param>
		public virtual void Activate(Array entities)
		{
			Activate(entities, 1);
		}
		
		/// <summary>
		/// Activates all the entities provided by loading all references for each entity to the specified depth.
		/// </summary>
		/// <param name="entities">The array of entities to activate.</param>
		public virtual void Activate(Array entities, int depth)
		{
			if (entities != null)
			{
				foreach (IEntity entity in entities)
				{
					if (entity != null)
						Activate(entity, depth);
				}
			}
		}
		
		/// <summary>
		/// Activates the specified property of all the entities provided, by loading all references for the specified property of each entity.
		/// </summary>
		/// <param name="entities">The array of entities to activate.</param>
		/// <param name="propertyName">The name of the property to activate.</param>
		public virtual void Activate(Array entities, string propertyName)
		{
			if (entities != null)
			{
				foreach (IEntity entity in entities)
				{
					if (entity != null)
						Activate(entity, propertyName);
				}
			}
		}
		
		/// <summary>
		/// Activates the specified property of the provided entity by loading the references for the property.
		/// </summary>
		/// <param name="entity">The entity to activate.</param>
		/// <param name="propertyName">The name of the property to activate.</param>
		public virtual void Activate(IEntity entity, string propertyName)
		{
			DataAccess.Data.Activate(entity, propertyName);
		}
		
		// TODO: Remove if not needed
		/*/// <summary>
		/// Activates all the entities provided by loading all references for each entity.
		/// </summary>
		/// <param name="entities">The array of entities to activate.</param>
		public virtual void Activate(object entities)
		{
			if (typeof(IEntity).IsAssignableFrom(entities.GetType()))
			{
				Activate((IEntity)entities);
			}
			else if (typeof(IEnumerable).IsAssignableFrom(entities.GetType()))
			{
				foreach (object entity in (IEnumerable)entities)
				{
					if (entity != null)
					{
						if (entity is IEntity)
							Activate((IEntity)entity);
					}
				}
			}
		}*/
	}
}
